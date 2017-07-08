using System;
using System.Collections.Generic;

namespace OnlineLibrary.Model
{
    public partial class ActivityHistory
    {
        public int Seq { get; set; }
        public string Mid { get; set; }
        public string Bid { get; set; }
        public DateTime? BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public virtual FineHistory FineHistory { get; set; }
        public virtual Book B { get; set; }
        public virtual Account M { get; set; }
    }
}
