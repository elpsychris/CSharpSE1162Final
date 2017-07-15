using System;
using System.Collections.Generic;

namespace OnlineLibrary.Models
{
    public partial class Title
    {
        public Title()
        {
            Book = new HashSet<Book>();
            FavoriteList = new HashSet<FavoriteList>();
            RatingNavigation = new HashSet<Rating>();
            TitleCategoryDetail = new HashSet<TitleCategoryDetail>();
        }

        public int Seq { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Publisher { get; set; }
        public int? PublishYear { get; set; }
        public string Isbn { get; set; }
        public int? AvailableQuan { get; set; }
        public string Illu { get; set; }
        public string Description { get; set; }
        public int? AuthorId { get; set; }
        public int? Views { get; set; }
        public double? Rating { get; set; }
        public int? ReturnType { get; set; }
        public int? RatingNo { get; set; }

        public virtual ICollection<Book> Book { get; set; }
        public virtual ICollection<FavoriteList> FavoriteList { get; set; }
        public virtual ICollection<Rating> RatingNavigation { get; set; }
        public virtual ICollection<TitleCategoryDetail> TitleCategoryDetail { get; set; }
        public virtual Author Author { get; set; }
        public virtual ReturnType ReturnTypeNavigation { get; set; }
    }
}
