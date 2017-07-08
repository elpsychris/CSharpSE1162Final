﻿using System;
using System.Collections.Generic;

namespace OnlineLibrary.Models
{
    public partial class Title
    {
        public Title()
        {
            Book = new HashSet<Book>();
        }

        public int Seq { get; set; }
        public string Name { get; set; }
        public string Publisher { get; set; }
        public int? PublishYear { get; set; }
        public string Isbn { get; set; }
        public int? AvailableQuan { get; set; }
        public string Illu { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Book> Book { get; set; }
    }
}