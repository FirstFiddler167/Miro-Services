using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Entities.Base
{
    public interface IBaseEntity
    {
        string Id { get; set; }
        string CreatedBy { get; set; }
        DateTime CreatedDate { get; set; }
        string LastModifiedBy { get; set; }
        DateTime LastModifiedDate { get; set; }
    }
}
