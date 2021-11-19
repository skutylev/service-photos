using System;

namespace Service.PhotoPackages.Dal.Models
{
    public static class PhotoPackageExtensions
    {
        public static bool IsUnderage(this PhotoPackage photoPackage, DateTime? checkDate = default)
        {
            checkDate ??= DateTime.UtcNow;
            var age = checkDate.Value.Year - photoPackage.ClientBirthday.Year;
            if (photoPackage.ClientBirthday > checkDate.Value.AddYears(-age)) age--;
            return age < 18;
        }
    }
}