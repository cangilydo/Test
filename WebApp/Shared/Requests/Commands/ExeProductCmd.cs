using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Requests.Commands
{
    public class ExeProductCmd : IRequest<string>
    {
        public string ProductId { get; set; }
    }

    public class ExeEmailCmd : IRequest<string>
    {
        public string EmailId { get; set; }
    }

    public class PaymentCmd : IRequest<string>
    {
        public int Price { get; set; }
    }
}
