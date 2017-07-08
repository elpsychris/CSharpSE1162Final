using System;
using System.Collections.Generic;

namespace OnlineLibrary.Models
{
    public partial class Book
    {
        public Book()
        {
            ActivityHistory = new HashSet<ActivityHistory>();
            BookCategoryDetail = new HashSet<BookCategoryDetail>();
            FavoriteList = new HashSet<FavoriteList>();
        }

        public int Seq { get; set; }
        public string Id { get; set; }
        public int? Title { get; set; }
        public string Author { get; set; }
        public int? ReturnType { get; set; }
        public DateTime ImportDate { get; set; }
        public int? StatusId { get; set; }
        public int? ViewCount { get; set; }
        public double? Rating { get; set; }

        public virtual ICollection<ActivityHistory> ActivityHistory { get; set; }
        public virtual ICollection<BookCategoryDetail> BookCategoryDetail { get; set; }
        public virtual ICollection<FavoriteList> FavoriteList { get; set; }
        public virtual ReturnType ReturnTypeNavigation { get; set; }
        public virtual BookStatus Status { get; set; }
        public virtual Title TitleNavigation { get; set; }
    }
}
