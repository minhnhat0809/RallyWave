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
            response.IsSucceed = false;
            response.Message = e.Message;
            response.StatusCode = StatusCodes.Status500InternalServerError;
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
            {
                return new ResponseDto(null, "There are no payments with this id", false, StatusCodes.Status404NotFound);
            }
            
            
        }
        catch (Exception e)
        {
            response.IsSucceed = false;
            response.Message = e.Message;
            response.StatusCode = StatusCodes.Status500InternalServerError;
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
            {
                response.IsSucceed = false;
                response.Message = "Booking and Subscription cannot be stay parallel";
                response.StatusCode = StatusCodes.Status400BadRequest;
                return response;
            }
        }
        catch (Exception e)
        {
            response.IsSucceed = false;
            response.Message = e.Message;
            response.StatusCode = StatusCodes.Status500InternalServerError;
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
            {
                return new ResponseDto(null, "There are no payments with this id", false,
                    StatusCodes.Status404NotFound);
            }
            
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
            response.IsSucceed = false;
            response.Message = e.Message;
            response.StatusCode = StatusCodes.Status500InternalServerError;
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
            var sub = await _unitOfWork.SubscriptionRepo.GetByConditionAsync(
                s => s.SubId == paymentCreateDto.PaymentCreateForSub!.SubId,
                s => s);
            
            //get user
            var checkUser =
                await _unitOfWork.UserRepo.GetByConditionAsync(
                    u => u.UserId == paymentCreateDto.PaymentCreateForSub!.UserId, u => u);

            if (sub == null)
            {
                response.Message = "There are no subscriptions with this id";
                response.IsSucceed = false;
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            if (checkUser == null)
            {
                response.Message = "There are no users with this id";
                response.IsSucceed = false;
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            var type = paymentCreateDto.Type;

            var userId = paymentCreateDto.PaymentCreateForSub!.UserId;

            if (!type.Equals("cash") && !type.Equals("banking"))
            {
                type = "banking";
            }

            var paymentDetail = new PaymentDetail
            {
                UserId = userId,
                SubId = sub.SubId,
                Type = type,
                Total = sub.Price,
                Status = 0
            };

            await _unitOfWork.PaymentRepo.CreateAsync(paymentDetail);

            try
            {

                //add item 
                var item = new ItemData(sub.SubName, 1, (int) sub.Price);

                var items = new List<ItemData> { item };

                //start create payment
                var paymentData = new PaymentData(paymentDetail.PaymentId, 1000, "user",
                    items, paymentCreateDto.SuccessUrl, paymentCreateDto.CancelUrl, null, checkUser.UserName, 
                    checkUser.Email, checkUser.PhoneNumber.ToString());
                
                var createPayment = await _payOs.createPaymentLink(paymentData);
            
                //update sub in table user
                checkUser.SubId = paymentCreateDto.PaymentCreateForSub.SubId;
                await _unitOfWork.UserRepo.UpdateAsync(checkUser);

                response.Message = createPayment.status;
            }
            catch (Exception e)
            {
                await _unitOfWork.PaymentRepo.DeletePayment(paymentDetail);
                
                response.Message = e.Message;
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.IsSucceed = false;
                return response;
            }

            return response;
        }
        catch (Exception e)
        {
            response.Message = e.Message;
            response.StatusCode = StatusCodes.Status500InternalServerError;
            response.IsSucceed = false;
            return response;
        }
    }
    
    private async Task<ResponseDto> ProcessPaymentForCourtOwner(PaymentCreateDto paymentCreateDto)
    {
        var response = new ResponseDto(null, "", true, StatusCodes.Status200OK);
        try
        {
            //get subscription 
            var sub = await _unitOfWork.SubscriptionRepo.GetByConditionAsync(
                s => s.SubId == paymentCreateDto.PaymentCreateForSub!.SubId,
                s => s);
            
            //get user
            var checkUser =
                await _unitOfWork.CourtOwnerRepo.GetByConditionAsync(
                    u => u.CourtOwnerId == paymentCreateDto.PaymentCreateForSub!.UserId, u => u);

            if (sub == null)
            {
                response.Message = "There are no subscriptions with this id";
                response.IsSucceed = false;
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            if (checkUser == null)
            {
                response.Message = "There are no court owners with this id";
                response.IsSucceed = false;
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            var type = paymentCreateDto.Type;

            var userId = paymentCreateDto.PaymentCreateForSub!.UserId;

            if (!type.Equals("cash") && !type.Equals("banking"))
            {
                type = "banking";
            }

            var paymentDetail = new PaymentDetail
            {
                UserId = userId,
                SubId = sub.SubId,
                Type = type,
                Total = sub.Price,
                Status = 0
            };

            await _unitOfWork.PaymentRepo.CreateAsync(paymentDetail);

            try
            {

                //add item
                var item = new ItemData(sub.SubName, 1, (int) sub.Price);

                var items = new List<ItemData> { item };

                //start create payment
                var paymentData = new PaymentData(paymentDetail.PaymentId, 1000, "courtOwner",
                    items, paymentCreateDto.SuccessUrl, paymentCreateDto.CancelUrl, null, checkUser.Name, 
                    checkUser.Email, checkUser.PhoneNumber.ToString());
                
                var createPayment = await _payOs.createPaymentLink(paymentData);
            
                //add subId in table court owner
                checkUser.SubId = paymentCreateDto.PaymentCreateForSub.SubId;
                await _unitOfWork.CourtOwnerRepo.UpdateAsync(checkUser);

                response.Message = createPayment.status;
            }
            catch (Exception e)
            {
                await _unitOfWork.PaymentRepo.DeletePayment(paymentDetail);
                
                response.Message = e.Message;
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.IsSucceed = false;
                return response;
            }

            return response;
        }
        catch (Exception e)
        {
            response.Message = e.Message;
            response.StatusCode = StatusCodes.Status500InternalServerError;
            response.IsSucceed = false;
            return response;
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
            {
                response.Message = "There are no bookings with this id";
                response.IsSucceed = false;
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }
            
            //get court name 
            var courtName =
                await _unitOfWork.CourtRepo.GetByConditionAsync(c => c.CourtId == booking.CourtId,
                    c => c.CourtName);
            
            var type = paymentCreateDto.Type;

            if (!type.Equals("cash") && !type.Equals("banking"))
            {
                type = "banking";
            }
            
            
            var paymentDetail = new PaymentDetail
            {
                BookingId = booking.BookingId,
                Type = type,
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
                    var user = await _unitOfWork.UserRepo.GetByConditionAsync(u => u.UserId == match!.CreateBy, u => new
                    {
                        u.UserName,
                        u.Email,
                        u.PhoneNumber
                    });
                
                    var paymentData = new PaymentData(paymentDetail.PaymentId, 1000, "booking",
                        items, paymentCreateDto.SuccessUrl, paymentCreateDto.CancelUrl, null, user!.UserName, 
                        user.Email, user.PhoneNumber.ToString()); 
                
                    var createPayment = await _payOs.createPaymentLink(paymentData);

                    response.Message = createPayment.status;

                }
                else if (booking.UserId.HasValue)
                {
                    //get user in database
                    var user = await _unitOfWork.UserRepo.GetByConditionAsync(u => u.UserId == booking.UserId.Value, 
                        u => new {
                            u.UserName,
                            u.Email,
                            u.PhoneNumber
                        });
                
                    var paymentData = new PaymentData(paymentDetail.PaymentId, 1000, "Thanh toan tien san",
                        items, paymentCreateDto.SuccessUrl, paymentCreateDto.CancelUrl, null, user!.UserName, 
                        user.Email, user.PhoneNumber.ToString()); 
                
                    var createPayment = await _payOs.createPaymentLink(paymentData);
                
                    response.Message = createPayment.status;
                }
                else
                {
                    response.Message = "This booking is created by court owner";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.IsSucceed = false;
                    return response;
                }
            
            }
            catch (Exception e)
            {
                await _unitOfWork.PaymentRepo.DeletePayment(paymentDetail);
                
                response.Message = e.Message;
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.IsSucceed = false;
                return response;
            }
            
            return response;
        }
        catch (Exception e)
        {
            response.Message = e.Message;
            response.StatusCode = StatusCodes.Status500InternalServerError;
            response.IsSucceed = false;
            return response;
        }
    }
}