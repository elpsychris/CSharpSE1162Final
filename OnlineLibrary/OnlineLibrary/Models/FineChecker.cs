using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineLibrary.Models;

namespace OnlineLibrary.Models
{
    public class FineChecker
    {
        public bool isLate(ActivityHistory ah)
        {
            int remainDays = (int)Math.Ceiling((decimal)ah.BorrowDate.Value.AddDays((double)ah.B.TitleNavigation.ReturnTypeNavigation.NoOfDay).Subtract(DateTime.Now).TotalDays);
            return remainDays < 0 ? true : false;
        }
    }
}
