using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Requests.Commands
{
    public class CheckOutCmd : IRequest<string>
    {
        public Dictionary<Guid, Guid> OrderIdPair { get; set; }
    }
}
