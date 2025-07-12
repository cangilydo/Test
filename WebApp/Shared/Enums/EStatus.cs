using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums
{
    public enum EOrderStatus
    {
        [Description("Create")]
        Create = 0,
        [Description("Payment processing...")]
        Pay = 1,
        [Description("Paid")]
        Paid = 2,
        [Description("Product processing...")]
        SendingProduct = 3,
        [Description("Sent Product")]
        SentProduct = 4,
        [Description("Emailed")]
        Email = 5,
        [Description("Complete")]
        Complete = 6,
        [Description("Payment error")]
        ErrorPaid = 7,
        [Description("Product error")]
        ErrorProduct = 8,
        [Description("Email error")]
        ErrorEmail = 9,
        [Description("Fail to retry")]
        FailRetry = 10
    }

    public enum EAudit
    {
        CheckOut = 0,
        Done = 1
    }

    public enum EQueueStatus
    {
        Create = 0,
        Processing = 1,
        Error = 2,
        FailRetry = 3,
        Complete = 4,
        Pending = 5 
    }

    public enum EQueueType
    {
        Email = 0,
        Product = 1
    }
}
