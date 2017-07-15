using System;
using System.Collections.Generic;

namespace OnlineLibrary.Models
{
    public partial class ActivityHistory
    {
        public ActivityHistory()
        {
            FineHistory = new HashSet<FineHistory>();
        }

        public int Seq { get; set; }
        public string Mid { get; set; }
        public string Bid { get; set; }
        public DateTime? BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public virtual ICollection<FineHistory> FineHistory { get; set; }
        public virtual Book B { get; set; }
        public virtual Account M { get; set; }
    }
}
