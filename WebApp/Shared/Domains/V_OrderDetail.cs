using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domains
{
    public class V_OrderDetail
    {
        public Guid ProductId { get; set; }
        public Guid OrderId { get; set; }
        public string? Name { get; set; }
        public string? NormalizeName { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
