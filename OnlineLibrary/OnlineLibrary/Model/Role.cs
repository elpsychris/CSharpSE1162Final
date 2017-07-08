using System;
using System.Collections.Generic;

namespace OnlineLibrary.Model
{
    public partial class Role
    {
        public Role()
        {
            Account = new HashSet<Account>();
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Account> Account { get; set; }
    }
}
