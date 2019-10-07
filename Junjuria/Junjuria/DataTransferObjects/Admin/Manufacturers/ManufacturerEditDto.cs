using Junjuria.Common.Interfaces.AutoMapper;
using Junjuria.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Junjuria.DataTransferObjects.Admin.Manufacturers
{
    public class ManufacturerEditDto:ManufacturerInDto,IMapFrom<Manufacturer>,IMapTo<Manufacturer>
    {
        public int Id { get; set; }

    }
}
