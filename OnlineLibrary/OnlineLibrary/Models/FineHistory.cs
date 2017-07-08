using System;
using System.Collections.Generic;

namespace OnlineLibrary.Models
{
    public partial class FineHistory
    {
        public int Seq { get; set; }
        public int? Aseq { get; set; }
        public int? FineTypeId { get; set; }
        public double? FineAmount { get; set; }

        public virtual ActivityHistory AseqNavigation { get; set; }
        public virtual FineType FineType { get; set; }
    }
}
