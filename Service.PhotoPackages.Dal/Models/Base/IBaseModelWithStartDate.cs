using System;

namespace Service.PhotoPackages.Dal.Models.Base
{
    internal interface IBaseModelWithStartDate
    {
        public DateTime StartDate { get; set; }
    }
}