using System;
using System.Collections.Generic;

namespace OnlineLibrary.Models
{
    public partial class Account
    {
        public Account()
        {
            ActivityHistory = new HashSet<ActivityHistory>();
            FavoriteList = new HashSet<FavoriteList>();
            Rating = new HashSet<Rating>();
        }

        public int Seq { get; set; }
        public string Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Password { get; set; }
        public string Sex { get; set; }
        public DateTime? Dob { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Rid { get; set; }
        public bool? Status { get; set; }
        public string Avatar { get; set; }

        public virtual ICollection<ActivityHistory> ActivityHistory { get; set; }
        public virtual ICollection<FavoriteList> FavoriteList { get; set; }
        public virtual ICollection<Rating> Rating { get; set; }
        public virtual Role R { get; set; }
    }
}
