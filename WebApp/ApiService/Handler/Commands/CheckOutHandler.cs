using ApiService.Handler.Products;
using ApiService.Services;

using MediatR;

using Shared.Enums;
using Shared.Repositories;
using Shared.Requests.Commands;

namespace ApiService.Handler.Commands
{
    public class CheckOutHandler : IRequestHandler<CheckOutCmd, string>
    {
        private readonly ICheckOutService _checkOutService;
        private readonly ILogger<CheckOutHandler> _logger;
        public CheckOutHandler(ICheckOutService checkOutService,
            ILogger<CheckOutHandler> logger)
        {
            _checkOutService = checkOutService;
            _logger = logger;
        }

        public async Task<string> Handle(CheckOutCmd request, CancellationToken cancellationToken)
        {
            try
            {
                var auditRes = await _checkOutService.AuditCheck(request);
                if (!string.IsNullOrEmpty(auditRes))
                {
                    return auditRes;
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
                    return paymentRes;
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

                _checkOutService.HandleProcess(orderLst);

                return "Processing";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CheckOutCmd exception: ");
                return string.Empty;
            }
        }
    }
}
