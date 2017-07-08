using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.Models;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineLibrary.Controllers
{
    public class DashboardController : Controller
    {
        [HttpGet]
        public IActionResult ShowBorrowedBooks()
        {
            string UID = "E00001";
            List<ActivityHistory> result = null;
            using (var context = new OnlineLibraryContext())
            {
                result = context.ActivityHistory.Where(ah => ah.Mid.Equals(UID) && ah.ReturnDate == null).Include(m => m.M).Include(b => b.B.TitleNavigation).ToList();
            }
            ViewBag.result = result;
            return View();
        }
    }
}
