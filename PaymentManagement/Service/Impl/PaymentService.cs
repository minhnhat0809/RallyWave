using AutoMapper;
using Entity;
using Net.payOS;
using Net.payOS.Types;
using PaymentManagement.DTOs;
using PaymentManagement.DTOs.PaymentDto;
using PaymentManagement.Repository;
using PaymentManagement.Ultility;

namespace PaymentManagement.Service.Impl;

public class PaymentService(IMapper mapper, IUnitOfWork unitOfWork, GetSecret getSecret) : IPaymentService
{
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly GetSecret _getSecret = getSecret;
    
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

    private async Task<ResponseDto> ProcessPaymentForUser(PaymentCreateDto paymentCreateDto)
    {
        var response = new ResponseDto(null, "", true, StatusCodes.Status200OK);
        try
        {
            //get subscription in database
            var sub = await _unitOfWork.SubscriptionRepo.GetByConditionAsync(
                s => s.SubId == paymentCreateDto.PaymentCreateForSub!.SubId,
                s => s);

            if (sub == null)
            {
                response.Message = "There are no subscriptions with this id";
                response.IsSucceed = false;
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }
        
            //check user in database
            var checkUser =
                await _unitOfWork.UserRepo.GetByConditionAsync(
                    u => u.UserId == paymentCreateDto.PaymentCreateForSub!.UserId, u => u);

            if (checkUser == null)
            {
                response.Message = "There are no users with this id";
                response.IsSucceed = false;
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            var type = paymentCreateDto.Type;

            var userId = paymentCreateDto.PaymentCreateForSub!.UserId;

            if (!type.Equals("cash") || !type.Equals("banking"))
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
        
            var payOsCredentials =  await _getSecret.GetPayOsCredentials();

            if (payOsCredentials == null)
            {
                response.Message = "There is an error in processing payment with pay-os";
                response.IsSucceed = false;
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;
            }

            var payOs = new PayOS(payOsCredentials.ClientId, payOsCredentials.ApiKey, payOsCredentials.CheckSumKey);

            //add item 
            var item = new ItemData(sub.SubName, 1, (int) sub.Price);

            var items = new List<ItemData> { item };

            //start create payment
            var paymentData = new PaymentData(paymentDetail.PaymentId, 1000, "Dang ki thanh vien cao cap",
                items, paymentCreateDto.SuccessUrl, paymentCreateDto.CancelUrl, null, checkUser.UserName, 
                checkUser.Email, checkUser.PhoneNumber.ToString());
                
            var createPayment = await payOs.createPaymentLink(paymentData);
            
            //update sub in table user
            checkUser.SubId = paymentCreateDto.PaymentCreateForSub.SubId;
            await _unitOfWork.UserRepo.UpdateAsync(checkUser);

            response.Message = createPayment.status;

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
            //get subscription in database
            var sub = await _unitOfWork.SubscriptionRepo.GetByConditionAsync(
                s => s.SubId == paymentCreateDto.PaymentCreateForSub!.SubId,
                s => s);

            if (sub == null)
            {
                response.Message = "There are no subscriptions with this id";
                response.IsSucceed = false;
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }
        
            //check user in database
            var checkUser =
                await _unitOfWork.CourtOwnerRepo.GetByConditionAsync(
                    u => u.CourtOwnerId == paymentCreateDto.PaymentCreateForSub!.UserId, u => u);

            if (checkUser == null)
            {
                response.Message = "There are no court owners with this id";
                response.IsSucceed = false;
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            var type = paymentCreateDto.Type;

            var userId = paymentCreateDto.PaymentCreateForSub!.UserId;

            if (!type.Equals("cash") || !type.Equals("banking"))
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
        
            var payOsCredentials =  await _getSecret.GetPayOsCredentials();

            if (payOsCredentials == null)
            {
                response.Message = "There is an error in processing payment with pay-os";
                response.IsSucceed = false;
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;
            }

            var payOs = new PayOS(payOsCredentials.ClientId, payOsCredentials.ApiKey, payOsCredentials.CheckSumKey);

            //add item
            var item = new ItemData(sub.SubName, 1, (int) sub.Price);

            var items = new List<ItemData> { item };

            //start create payment
            var paymentData = new PaymentData(paymentDetail.PaymentId, 1000, "Dang ki thanh vien cao cap",
                items, paymentCreateDto.SuccessUrl, paymentCreateDto.CancelUrl, null, checkUser.Name, 
                checkUser.Email, checkUser.PhoneNumber.ToString());
                
            var createPayment = await payOs.createPaymentLink(paymentData);
            
            //add subId in table court owner
            checkUser.SubId = paymentCreateDto.PaymentCreateForSub.SubId;
            await _unitOfWork.CourtOwnerRepo.UpdateAsync(checkUser);

            response.Message = createPayment.status;

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
                    b => b);
            

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

            var userId = paymentCreateDto.PaymentCreateForSub!.UserId;

            if (!type.Equals("cash") || !type.Equals("banking"))
            {
                type = "banking";
            }
            
            var paymentDetail = new PaymentDetail
            {
                BookingId = booking.BookingId,
                Type = type,
                Total = booking.Cost,
                Status = 0
            };

            await _unitOfWork.PaymentRepo.CreateAsync(paymentDetail);
            
            var payOsCredentials =  await _getSecret.GetPayOsCredentials();

            if (payOsCredentials == null)
            {
                response.Message = "There is an error in processing payment with pay-os";
                response.IsSucceed = false;
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;
            }
            
            //add item
            var item = new ItemData(courtName!, 1, (int) booking.Cost);

            var items = new List<ItemData> { item };
            
            //initiate payos
            var payOs = new PayOS(payOsCredentials.ClientId, payOsCredentials.ApiKey, payOsCredentials.CheckSumKey);

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
                
                var paymentData = new PaymentData(paymentDetail.PaymentId, 1000, "Thanh toan tien san",
                    items, paymentCreateDto.SuccessUrl, paymentCreateDto.CancelUrl, null, user!.UserName, 
                    user.Email, user.PhoneNumber.ToString()); 
                
                var createPayment = await payOs.createPaymentLink(paymentData);

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
                
                var createPayment = await payOs.createPaymentLink(paymentData);
                
                response.Message = createPayment.status;
            }
            else
            {
                response.Message = "This booking is created by court owner";
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.IsSucceed = false;
                return response;
            }
            
            //update status after paid
            booking.Status = 2;
            await _unitOfWork.BookingRepo.UpdateAsync(booking);
            
            
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