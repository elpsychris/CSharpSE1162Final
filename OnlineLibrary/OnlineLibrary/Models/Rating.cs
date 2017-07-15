using System;
using System.Collections.Generic;

namespace OnlineLibrary.Models
{
    public partial class Rating
    {
        public int RateId { get; set; }
        public int? TitleSeq { get; set; }
        public int? Mseq { get; set; }
        public int? Rating1 { get; set; }

        public virtual Account MseqNavigation { get; set; }
        public virtual Title TitleSeqNavigation { get; set; }
    }
}
