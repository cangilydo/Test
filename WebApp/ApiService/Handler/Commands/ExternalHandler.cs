using MediatR;

using Shared.Domains;
using Shared.Requests.Commands;

namespace ApiService.Handler.Commands
{
    public class ExternalHandler : IRequestHandler<ExeProductCmd, string>,
        IRequestHandler<ExeEmailCmd, string>,
        IRequestHandler<PaymentCmd, string>
    {
        public ExternalHandler()
        {

        }
        public async Task<string> Handle(ExeProductCmd request, CancellationToken cancellationToken)
        {
            try
            {
                var checkChar = request.ProductId.First();
                if (!int.TryParse(checkChar.ToString(), out int number) || number % 2 == 1)
                    return string.Empty;

                return "OK";
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task<string> Handle(ExeEmailCmd request, CancellationToken cancellationToken)
        {
            try
            {
                var checkChar = request.EmailId.First();
                if (!int.TryParse(checkChar.ToString(), out int number) || number % 2 == 0)
                    return string.Empty;

                return "OK";
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task<string> Handle(PaymentCmd request, CancellationToken cancellationToken)
        {
            try
            {
                if(request.Price % 2 == 0)
                    return string.Empty;

                return "OK";
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
