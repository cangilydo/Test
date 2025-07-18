﻿using Shared.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class PagedResult<T>
    {
        public int Count { get; set; }
        public List<T> Result { get; set; }
    }

    public class PagedQueryAbleResult<T>
    {
        public int Count { get; set; }
        public IQueryable<T> Result { get; set; }
    }

    public class BaseRes<T>
    {
        public EResStatus Status { get; set; }
        public T Data { get; set; }
    }
}
