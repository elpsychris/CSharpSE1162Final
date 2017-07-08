using System;
using System.Collections.Generic;

namespace OnlineLibrary.Model
{
    public partial class FavoriteList
    {
        public int Seq { get; set; }
        public int? Bid { get; set; }
        public int? Mid { get; set; }

        public virtual Book B { get; set; }
        public virtual Account M { get; set; }
    }
}
