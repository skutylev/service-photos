using System.Collections.Generic;

namespace Service.PhotoPackages.ServiceLayer.Constants
{
    public static class PhotoPackageProcessTypes
    {
        public const string CreditOnline = "CREDIT_ONLINE";
        public const string CreditSigned = "CREDIT_SIGNED";
        public const string DebitOnline = "DEBIT_ONLINE";
        public const string DebitOffline = "DEBIT_OFFLINE";
        public const string DebitSigned = "DEBIT_SIGNED";

        public static readonly IEnumerable<string> VerifiableProcesses = new[] {CreditOnline, DebitOnline};
    }
}