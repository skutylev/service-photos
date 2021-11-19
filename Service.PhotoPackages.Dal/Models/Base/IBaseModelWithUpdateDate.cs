using System;

namespace Service.PhotoPackages.Dal.Models.Base
{
    internal interface IBaseModelWithUpdateDate
    {
        public DateTime? UpdateDate { get; set; }
    }
}