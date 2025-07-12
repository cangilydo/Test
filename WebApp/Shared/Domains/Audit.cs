using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domains
{
    public class Audit
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public int? Status { get; set; }
        public virtual Order Order { get; set; }
    }
}
