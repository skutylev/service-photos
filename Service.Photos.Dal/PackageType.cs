using System.ComponentModel.DataAnnotations;

namespace Service.Photos.Dal
{
    public enum PackageType
    {
        [Display(Name = "Договор")]
        Contract = 1,
        [Display(Name = "Заявление на LK")]
        LoungeKey = 2,
        [Display(Name = "Заявление на ВЗР")]
        TravelInsurance = 3
    }
}