using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Models;
using System.Collections.Generic;
using System.Linq;

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
                result = context.ActivityHistory.Where(ah => ah.Mid.Equals(UID) && ah.ReturnDate == null).Include(b => b.B.TitleNavigation).Include(b => b.B.TitleNavigation.ReturnTypeNavigation).ToList();
            }
            ViewBag.result = result;
            return View();
        }

        [HttpGet]
        public IActionResult ShowBorrowedHistory()
        {
            string UID = "E00001";
            List<ActivityHistory> result = null;
            using (var context = new OnlineLibraryContext())
            {
                result = context.ActivityHistory.Where(ah => ah.Mid.Equals(UID) && ah.ReturnDate != null).Include(b => b.B.TitleNavigation).ToList();
            }
            ViewBag.result = result;
            return View();
        }
    }
}
