using MediatR;

using Shared.Dto;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Requests.Commands
{
    public class CheckOutCmd : IRequest<BaseRes<string>>
    {
        public Dictionary<Guid, Guid> OrderIdPair { get; set; }
    }
}
