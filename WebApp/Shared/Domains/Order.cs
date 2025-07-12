using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domains
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? Type { get; set; }
        public virtual Product Product { get; set; }
    }
}
