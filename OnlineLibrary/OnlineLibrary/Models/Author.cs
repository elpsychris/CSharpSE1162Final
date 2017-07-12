using System;
using System.Collections.Generic;

namespace OnlineLibrary.Models
{
    public partial class Author
    {
        public Author()
        {
            Title = new HashSet<Title>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public virtual ICollection<Title> Title { get; set; }
    }
}
