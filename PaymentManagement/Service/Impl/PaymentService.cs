using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Entity;
using Net.payOS;
using Net.payOS.Types;
using PaymentManagement.DTOs;
using PaymentManagement.DTOs.PaymentDto;
using PaymentManagement.Repository;
using PaymentManagement.Ultility;

namespace PaymentManagement.Service.Impl;

public class PaymentService(IMapper mapper, IUnitOfWork unitOfWork, PayOS payOs) : IPaymentService
{
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly PayOS _payOs = payOs;
    
    public async Task<ResponseDto> GetPayments()
    {
        var response = new ResponseDto(null, "", true, StatusCodes.Status200OK);
        try
        {

        }
        catch (Exception e)
        {
            return new ResponseDto(null, e.Message, false, StatusCodes.Status500InternalServerError);
        }

        return response;
    }

    public async Task<ResponseDto> GetPaymentById(int id)
    {
        var response = new ResponseDto(null, "", true, StatusCodes.Status200OK);
        try
        {
            var payment = await _unitOfWork.PaymentRepo.GetByConditionAsync(p => p.PaymentId == id, p => p, p => p.Booking);

            if (payment == null)
                return new ResponseDto(null, "There are no payments with this id", false, StatusCodes.Status404NotFound);
            
            
        }
        catch (Exception e)
        {
            return new ResponseDto(null, e.Message, false, StatusCodes.Status500InternalServerError);
        }

        return response;
    }

    public async Task<ResponseDto> ProcessPayment(PaymentCreateDto paymentCreateDto)
    {
        var response = new ResponseDto(null, "", true, StatusCodes.Status200OK);
        try
        {
            
            if (paymentCreateDto.BookingId.HasValue)
            {
                response = await ProcessPaymentForBooking(paymentCreateDto);

            }
            else if (paymentCreateDto.PaymentCreateForSub != null)
            {
                response = paymentCreateDto.PaymentCreateForSub.UserType.ToLower() switch
                {
                    "user" => await ProcessPaymentForUser(paymentCreateDto),
                    "courtowner" => await ProcessPaymentForCourtOwner(paymentCreateDto),
                    _ => response
                };
            }
            else 
                return new ResponseDto(null, "Booking and Subscription cannot be stay parallel", false, 
                StatusCodes.Status400BadRequest);
            
        }
        catch (Exception e)
        {
            return new ResponseDto(null, e.Message, false, StatusCodes.Status500InternalServerError);
        }

        return response;
    }

    public async Task<ResponseDto> HandlePayment(WebhookType webhookType)
    {
        var response = new ResponseDto(null, "", true, StatusCodes.Status200OK);
        try
        {
            var data = _payOs.verifyPaymentWebhookData(webhookType);

            var id = (int) data.orderCode;
            
            var payment = await _unitOfWork.PaymentRepo.GetByConditionAsync( p => p.PaymentId == id, p => p);

            if (payment == null)
                return new ResponseDto(null, "There are no payments with this id", false, StatusCodes.Status404NotFound);
            
            if (webhookType.success)
            {
            
                if (payment.BookingId.HasValue)
                {
                    //update booking status to Reserved
                    var booking = _unitOfWork.BookingRepo.GetByConditionAsync(b => b.BookingId == payment.BookingId.Value,
                        b => b).Result;

                    booking!.Status = 2;

                    await _unitOfWork.BookingRepo.UpdateAsync(booking);

                }else if (payment.UserId.HasValue)
                {
                    //update user status to subscribed and subId
                    var user = _unitOfWork.UserRepo.GetByConditionAsync(u => u.UserId == payment.UserId.Value,
                        u => u).Result;

                    user!.SubId = payment.SubId!.Value;

                    user.Status = 1;
                    
                    await _unitOfWork.UserRepo.UpdateAsync(user);

                }else if (payment.CourtOwnerId.HasValue)
                {
                    //update court owner status to subscribed and subId
                    var courtOwner = _unitOfWork.CourtOwnerRepo.GetByConditionAsync(co => co.CourtOwnerId == payment.CourtOwnerId.Value,
                        co => co).Result;
                    
                    courtOwner!.SubId = payment.SubId!.Value;

                    courtOwner.Status = 1;
                    
                    await _unitOfWork.CourtOwnerRepo.UpdateAsync(courtOwner);
                }
            }
            else
            {
                await _unitOfWork.PaymentRepo.DeletePayment(payment);

                return new ResponseDto(null, "Payment has failed", false, int.Parse(webhookType.code));
            }
            

        }
        catch (Exception e)
        {
            return new ResponseDto(null, e.Message, false, StatusCodes.Status500InternalServerError);
        }

        return response;
    }

    public async Task<ResponseDto> ConfirmWebHook(string url)
    {
        try
        {
            await _payOs.confirmWebhook(url);
            
            return new ResponseDto(null, "Ok", true, StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            return new ResponseDto(null, e.Message, true, StatusCodes.Status500InternalServerError);
        }
    }

    private async Task<ResponseDto> ProcessPaymentForUser(PaymentCreateDto paymentCreateDto)
    {
        var response = new ResponseDto(null, "", true, StatusCodes.Status200OK);
        try
        {
            //get subscription 
            var sub = await GetSubscription(paymentCreateDto.PaymentCreateForSub!.SubId);
            
            //get user
            var checkUser = await GetUser(paymentCreateDto.PaymentCreateForSub!.UserId);

            if (sub == null) 
                return new ResponseDto(null,"There are no subscriptions with this id", false, StatusCodes.Status404NotFound);

            if (checkUser == null) 
                return new ResponseDto(null,"There are no users with this id", false, StatusCodes.Status404NotFound);

            if (!paymentCreateDto.Type.Equals("cash") && !paymentCreateDto.Type.Equals("banking")) 
                paymentCreateDto.Type = "banking";

            var paymentDetail = new PaymentDetail
            {
                UserId = paymentCreateDto.PaymentCreateForSub!.UserId,
                SubId = sub.SubId,
                Type = paymentCreateDto.Type,
                Signature = "",
                Total = sub.Price,
                Status = 0
            };

            await _unitOfWork.PaymentRepo.CreateAsync(paymentDetail);

            try
            {
                //add item 
                var item = new ItemData(sub.SubName, 1, (int) sub.Price);

                var items = new List<ItemData> { item };
                
                var signature = GenerateSignature(paymentDetail.PaymentId, 
                    (int) sub.Price, checkUser.UserName, checkUser.Email ?? "", checkUser.PhoneNumber);

                paymentDetail.Signature = signature;

                await _unitOfWork.PaymentRepo.UpdateAsync(paymentDetail);

                //start create payment
                var paymentData = new PaymentData(paymentDetail.PaymentId, (int) sub.Price, "Thanh toan goi premium",
                    items, paymentCreateDto.SuccessUrl, paymentCreateDto.CancelUrl, signature, checkUser.UserName, 
                    checkUser.Email, checkUser.PhoneNumber.ToString());
                
                var createPayment = await _payOs.createPaymentLink(paymentData);

                response.Result = createPayment.checkoutUrl;
                response.Message = "Processing";
            }
            catch (Exception e)
            {
                await _unitOfWork.PaymentRepo.DeletePayment(paymentDetail);
                
                return new ResponseDto(null, e.Message, false, StatusCodes.Status500InternalServerError);
            }

            return response;
        }
        catch (Exception e)
        {
            return new ResponseDto(null, e.Message, false, StatusCodes.Status500InternalServerError);
        }
    }
    
    private async Task<ResponseDto> ProcessPaymentForCourtOwner(PaymentCreateDto paymentCreateDto)
    {
        var response = new ResponseDto(null, "", true, StatusCodes.Status200OK);
        try
        {
            //get subscription 
            var sub = await GetSubscription(paymentCreateDto.PaymentCreateForSub!.SubId);
            
            //get court owner
            var checkUser =
                await _unitOfWork.CourtOwnerRepo.GetByConditionAsync(
                    u => u.CourtOwnerId == paymentCreateDto.PaymentCreateForSub!.UserId, u => u);

            if (sub == null)
                return new ResponseDto(null,"There are no subscriptions with this id", false, StatusCodes.Status404NotFound);

            if (checkUser == null)
                return new ResponseDto(null,"There are no users with this id", false, StatusCodes.Status404NotFound);

            var userId = paymentCreateDto.PaymentCreateForSub!.UserId;

            if (!paymentCreateDto.Type.Equals("cash") && !paymentCreateDto.Type.Equals("banking")) paymentCreateDto.Type = "banking";

            var paymentDetail = new PaymentDetail
            {
                UserId = userId,
                SubId = sub.SubId,
                Signature = "",
                Type = paymentCreateDto.Type,
                Total = sub.Price,
                Status = 0
            };

            await _unitOfWork.PaymentRepo.CreateAsync(paymentDetail);

            try
            {
                //add item
                var item = new ItemData(sub.SubName, 1, (int) sub.Price);

                var items = new List<ItemData> { item };

                var signature = GenerateSignature(paymentDetail.PaymentId,
                        (int) sub.Price, checkUser.Name!, checkUser.Email!, checkUser.PhoneNumber ?? 0);
                
                paymentDetail.Signature = signature;
                
                await _unitOfWork.PaymentRepo.UpdateAsync(paymentDetail);
                

                //start create payment
                var paymentData = new PaymentData(paymentDetail.PaymentId, 1000, "Thanh toan goi premium",
                    items, paymentCreateDto.SuccessUrl, paymentCreateDto.CancelUrl, signature, checkUser.Name, 
                    checkUser.Email, checkUser.PhoneNumber.ToString());
                
                var createPayment = await _payOs.createPaymentLink(paymentData);

                response.Result = createPayment.checkoutUrl;
                response.Message = "Processing";
            }
            catch (Exception e)
            {
                await _unitOfWork.PaymentRepo.DeletePayment(paymentDetail);
                
                return new ResponseDto(null, e.Message, false, StatusCodes.Status500InternalServerError);
            }

            return response;
        }
        catch (Exception e)
        {
            return new ResponseDto(null, e.Message, false, StatusCodes.Status500InternalServerError);
        }
         
    }

    private async Task<ResponseDto> ProcessPaymentForBooking(PaymentCreateDto paymentCreateDto)
    {
        var response = new ResponseDto(null, "", true, StatusCodes.Status200OK);
        try
        {
            //check booking in database
            var booking =
                await _unitOfWork.BookingRepo.GetByConditionAsync(b => b.BookingId == paymentCreateDto.BookingId,
                    b => 
                        new
                        {
                            b.BookingId, 
                            b.MatchId, 
                            b.UserId, 
                            b.CourtId,
                            b.Cost
                        });
            

            if (booking == null)
                return new ResponseDto(null,"There are no bookings with this id", false, StatusCodes.Status404NotFound);
            
            //get court name 
            var courtName =
                await _unitOfWork.CourtRepo.GetByConditionAsync(c => c.CourtId == booking.CourtId,
                    c => c.CourtName);

            if (!paymentCreateDto.Type.Equals("cash") && !paymentCreateDto.Type.Equals("banking"))
                paymentCreateDto.Type = "banking";
            
            
            var paymentDetail = new PaymentDetail
            {
                BookingId = booking.BookingId,
                Signature = "",
                Type = paymentCreateDto.Type,
                Total = Math.Round(booking.Cost * 10 / 100, 2),
                Status = 0
            };

            await _unitOfWork.PaymentRepo.CreateAsync(paymentDetail);

            try
            {
                
                //add item
                var item = new ItemData(courtName!, 1, (int) booking.Cost);

                var items = new List<ItemData> { item };

                if (booking.MatchId.HasValue)
                {
                    //get match name and owner of match
                    var match =
                        await _unitOfWork.MatchRepo.GetByConditionAsync(m => m.MatchId == booking.MatchId.Value,
                            m => new
                            {
                                m.MatchName,
                                m.CreateBy
                            });

                    //get user 
                    var user = await GetUser(match!.CreateBy);
                    
                    if (user == null) 
                        return new ResponseDto(null,"There are no users with this id", false, StatusCodes.Status404NotFound);
                    
                    var signature = GenerateSignature(paymentDetail.PaymentId, 
                        (int) booking.Cost, user.UserName, user.Email ?? "", user.PhoneNumber);

                    paymentDetail.Signature = signature;

                    await _unitOfWork.PaymentRepo.UpdateAsync(paymentDetail);
                    
                    try
                    {
                        var paymentData = new PaymentData(paymentDetail.PaymentId, (int) paymentDetail.Total, "Thanh toan tien san",
                            items, paymentCreateDto.SuccessUrl, paymentCreateDto.CancelUrl, signature, user.UserName, 
                            user.Email, user.PhoneNumber.ToString()); 
                
                        var createPayment = await _payOs.createPaymentLink(paymentData);

                        response.Result = createPayment.checkoutUrl;
                        response.Message = "Processing";
                    }
                    catch (Exception e)
                    {
                        return new ResponseDto(null, e.Message, false, StatusCodes.Status500InternalServerError);
                    }

                }
                else if (booking.UserId.HasValue)
                {
                    //get user in database
                    var user = await GetUser(booking.UserId.Value);
                    
                    var signature = GenerateSignature(paymentDetail.PaymentId, 
                        (int) booking.Cost, user!.UserName, user.Email ?? "", user.PhoneNumber);

                    paymentDetail.Signature = signature;

                    await _unitOfWork.PaymentRepo.UpdateAsync(paymentDetail);
                
                    try
                    {
                        var paymentData = new PaymentData(paymentDetail.PaymentId, (int) paymentDetail.Total, "booking",
                            items, paymentCreateDto.SuccessUrl, paymentCreateDto.CancelUrl, null, user.UserName, 
                            user.Email, user.PhoneNumber.ToString()); 
                
                        var createPayment = await _payOs.createPaymentLink(paymentData);

                        response.Result = createPayment.checkoutUrl;
                        response.Message = "Processing";
                    }
                    catch (Exception e)
                    {
                        return new ResponseDto(null, e.Message, false, StatusCodes.Status500InternalServerError);
                    }
                    
                    response.Message = "Processing";
                }
                else
                {
                    return new ResponseDto(null, "This booking is created by court owner", false, StatusCodes.Status400BadRequest);
                }
            
            }
            catch (Exception e)
            {
                await _unitOfWork.PaymentRepo.DeletePayment(paymentDetail);
                
                return new ResponseDto(null, e.Message, false, StatusCodes.Status500InternalServerError);
            }
            
            return response;
        }
        catch (Exception e)
        {
            return new ResponseDto(null, e.Message, false, StatusCodes.Status500InternalServerError);
        }
    }

    private static string GenerateSignature(int paymentId, int amount, string userName, string email, int phoneNumber)
    {
        var secret = new GetSecret();

        var checkSumKey = secret.GetPayOsCredentials().Result!.CheckSumKey;
        
        // Prepare data for signing by sorting and concatenating fields
        var sortedData = new Dictionary<string, string>
        {
            { "PaymentId", paymentId.ToString() },
            { "Amount", amount.ToString() },
            { "UserName", userName },
            { "Email", email },
            { "PhoneNumber", phoneNumber.ToString() }
        };

        var concatenatedData = string.Join("&", sortedData.OrderBy(kvp => kvp.Key)
            .Select(kvp => $"{kvp.Key}={kvp.Value}"));

        // Hash the concatenated string with HMAC-SHA256
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checkSumKey));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenatedData));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
    
    private async Task<dynamic?> GetUser(int userId) => 
        await _unitOfWork.UserRepo.GetByConditionAsync(u => u.UserId == userId, 
            u => new {
                u.UserName,
                u.Email,
                u.PhoneNumber
            });
    
    private async Task<dynamic?> GetSubscription(int subId) => 
        await _unitOfWork.SubscriptionRepo.GetByConditionAsync(s => s.SubId == subId, s => new { s.SubId, s.SubName, s.Price });
}