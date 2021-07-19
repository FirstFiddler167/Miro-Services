using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly ILogger<DiscountService> _logger;
        private readonly IDiscountRepository _repository;
        private readonly IMapper _mapper;

        public DiscountService(ILogger<DiscountService> logger, IDiscountRepository repository, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _repository.GetDiscount(request.ProductName);
            if (coupon == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.ProductName} is not found."));
            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }
        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            var result = await _repository.CreateDiscount(coupon);
            if (!result)
                throw new RpcException(new Status(StatusCode.NotFound, $"Create Discount is not Success."));
            _logger.LogInformation("Discount is successfully created. ProductName : {ProductName}", coupon.ProductName);
            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }
        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            var result = await _repository.UpdateDiscount(coupon);
            if (!result)
                throw new RpcException(new Status(StatusCode.NotFound, $"Update Discount is not Success."));
            _logger.LogInformation("Discount is successfully Update. ProductName : {ProductName}", coupon.ProductName);
            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }
        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var result = await _repository.DeleteDiscount(request.ProductName);
            if (!result)
                throw new RpcException(new Status(StatusCode.NotFound, $"Delete Discount is not Success."));
            _logger.LogInformation("Discount is successfully Delete. ProductName : {ProductName}", coupon.ProductName);
            return new DeleteDiscountResponse { Success = result };
        }
        
       

    }
}
