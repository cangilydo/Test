using ApiService.Handler.Products;
using ApiService.Services;

using MediatR;

using Shared.Dto;
using Shared.Enums;
using Shared.Repositories;
using Shared.Requests.Commands;

namespace ApiService.Handler.Commands
{
    public class CheckOutHandler : IRequestHandler<CheckOutCmd, BaseRes<string>>
    {
        private readonly ICheckOutService _checkOutService;
        private readonly ILogger<CheckOutHandler> _logger;
        public CheckOutHandler(ICheckOutService checkOutService,
            ILogger<CheckOutHandler> logger)
        {
            _checkOutService = checkOutService;
            _logger = logger;
        }

        public async Task<BaseRes<string>> Handle(CheckOutCmd request, CancellationToken cancellationToken)
        {
            try
            {
                var auditRes = await _checkOutService.AuditCheck(request);
                if (!string.IsNullOrEmpty(auditRes))
                {
                    return new BaseRes<string>()
                    {
                        Status = EResStatus.ValidateError,
                        Data = auditRes
                    };
                }
                var orderIds = new List<Guid>();
                foreach (var item in request.OrderIdPair)
                {
                    orderIds.Add(item.Value);
                }
                var orderLst = _checkOutService.GetOrderByIds(orderIds);

                var paymentRes = await _checkOutService.CallPayment();
                if (!string.IsNullOrEmpty(paymentRes))
                {
                    orderLst.ForEach(s =>
                    {
                        s.Status = (int)EOrderStatus.ErrorPaid;
                        s.UpdatedOn = DateTime.Now.ToUniversalTime();
                    });
                    return new BaseRes<string>()
                    {
                        Status = EResStatus.ValidateError,
                        Data = paymentRes
                    };
                }
                else
                {
                    orderLst.ForEach(s =>
                    {
                        s.Status = (int)EOrderStatus.Paid;
                        s.UpdatedOn = DateTime.Now.ToUniversalTime();
                    });
                }

                await _checkOutService.UpdateOrder(orderLst);

                await _checkOutService.HandleProcess(orderLst);

                await _checkOutService.DoneAudit(request);

                return new BaseRes<string>()
                {
                    Status = EResStatus.Success,
                    Data = "processing"
                }; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CheckOutCmd exception: ");
                return new BaseRes<string>()
                {
                    Status = EResStatus.SystemError,
                    Data = ex.Message
                };
            }
        }
    }
}
