using System;
using System.Collections.Generic;

namespace OnlineLibrary.Models
{
    public partial class FineType
    {
        public FineType()
        {
            FineHistory = new HashSet<FineHistory>();
        }

        public int FineTypeId { get; set; }
        public string FineTypeName { get; set; }
        public double? Amount { get; set; }
        public string Description { get; set; }

        public virtual ICollection<FineHistory> FineHistory { get; set; }
    }
}
