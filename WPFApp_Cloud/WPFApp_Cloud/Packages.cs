using System;
using System.Collections.Generic;
using System.Text;

namespace WPFApp_Cloud
{
    class Packages
    {
        public int PackageId { get; set; }
        public string PkgName { get; set; }
        public DateTime? PkgStartDate { get; set; }
        public DateTime? PkgEndDate { get; set; }
        public string PkgDesc { get; set; }
        public decimal PkgBasePrice { get; set; }
        public string PkgImage { get; set; }
        public decimal? PkgAgencyCommission { get; set; }
    }
}
