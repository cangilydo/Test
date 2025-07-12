using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domains
{
    public class Queue
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public int? Status { get; set; }
        public int? Type { get; set; }
        public int? RetryTime { get; set; }
        public string? ErrorMsg { get; set; }
        public virtual Order Order { get; set; }
    }
}
