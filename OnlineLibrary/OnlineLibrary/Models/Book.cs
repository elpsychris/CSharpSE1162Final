using System;
using System.Collections.Generic;

namespace OnlineLibrary.Models
{
    public partial class Book
    {
        public Book()
        {
            ActivityHistory = new HashSet<ActivityHistory>();
        }

        public int Seq { get; set; }
        public string Id { get; set; }
        public int? Title { get; set; }
        public DateTime ImportDate { get; set; }
        public int? StatusId { get; set; }

        public virtual ICollection<ActivityHistory> ActivityHistory { get; set; }
        public virtual BookStatus Status { get; set; }
        public virtual Title TitleNavigation { get; set; }
    }
}
