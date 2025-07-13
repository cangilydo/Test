using AutoMapper;

using Shared.Domains;
using Shared.Dto;
using Shared.Enums;
using Shared.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<V_OrderDetail, OrderDetailDto>()
                .ForMember(s => s.StatusStr, x => x.MapFrom(m => m.Status != null ? ((EOrderStatus)((int)m.Status.Value)).DescriptionAttr() : string.Empty))
                .ForMember(s => s.CreatedOnStr, x => x.MapFrom(m => m.CreatedOn != null ? m.CreatedOn.Value.ToString("dd/MM/yyyy - HH:mm:ss") : string.Empty));
        }
    }
}
