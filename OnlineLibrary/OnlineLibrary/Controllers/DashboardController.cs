using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using OnlineLibrary.Models;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineLibrary.Controllers
{
    public class DashboardController : Controller
    {

        [HttpPost]
        public IActionResult GetMemberBorrowList(string MID, bool notUsed)
        {
            List<ActivityHistory> result = null;
            using (var context = new OnlineLibraryContext())
            {
                if (MID == null)
                {
                    result = context.ActivityHistory.Where(ah => ah.ReturnDate == null).Include(m => m.M).Include(m => m.M.R).ToList();
                    ViewBag.result = result;
                }
                else
                {
                    result = context.ActivityHistory.Where(ah => ah.ReturnDate == null && ah.Mid.Equals(MID)).Include(m => m.M).Include(m => m.M.R).ToList();
                    ViewBag.result = result;
                }
                ViewBag.lastSearchValue = MID;
                return View();
            }

        }

        [HttpGet]
        public IActionResult GetMemberBorrowList(string MID)
        {
            List<ActivityHistory> result = null;
            using (var context = new OnlineLibraryContext())
            {
                if (MID.Equals("all"))
                {
                    result = context.ActivityHistory.Where(ah => ah.ReturnDate == null).Include(m => m.M).Include(m => m.M.R).ToList();
                    ViewBag.result = result;
                }
                else
                {
                    result = context.ActivityHistory.Where(ah => ah.ReturnDate == null && ah.Mid.Equals(MID)).Include(m => m.M).Include(m => m.M.R).ToList();
                    ViewBag.result = result;
                }
                ViewBag.lastSearchValue = MID;
                return View();
            }

        }

        [HttpGet]
        public IActionResult ShowBorrowedBooks(string MID, string lsv)
        {
            List<ActivityHistory> result = null;
            Account acc = null;
            using (var context = new OnlineLibraryContext())
            {
                result = context.ActivityHistory.Where(ah => ah.Mid.Equals(MID) && ah.ReturnDate == null).Include(b => b.B.TitleNavigation).Include(b => b.B.TitleNavigation.ReturnTypeNavigation).ToList();
                acc = context.Account.Where(a => a.Id.Equals(MID)).First();
            }
            ViewBag.result = result;
            ViewBag.account = acc;
            ViewBag.lastSearchValue = lsv;
            return View();
        }

        [HttpGet]
        public IActionResult ShowBorrowedHistory(string MID, string lsv)
        {
            List<ActivityHistory> result = null;
            Account acc = null;
            using (var context = new OnlineLibraryContext())
            {
                result = context.ActivityHistory.Where(ah => ah.Mid.Equals(MID) && ah.ReturnDate != null).Include(b => b.B.TitleNavigation).ToList();
                acc = context.Account.Where(a => a.Id.Equals(MID)).First();
            }
            ViewBag.result = result;
            ViewBag.account = acc;
            ViewBag.lastSearchValue = lsv;
            return View();
        }

        [HttpPost]
        public IActionResult MarkReturnedBooks(string[] returned, string MID, string lsv)
        {
            ActivityHistory ah = null;
            List<string> returnedList = returned.ToList();
            bool isLate = false;
            FineChecker fc = new FineChecker();
            if (returnedList.Count != 0)
            {
                using (var context = new OnlineLibraryContext())
                {
                    foreach (string book in returnedList)
                    {
                        ah = context.ActivityHistory.Where(r => r.Bid.Equals(book) && r.Mid.Equals(MID)).Include(b => b.B.TitleNavigation.ReturnTypeNavigation).First();
                        isLate = fc.isLate(ah);
                        int daysLate = (int)Math.Abs((decimal)Math.Ceiling((decimal)ah.BorrowDate.Value.AddDays((double)ah.B.TitleNavigation.ReturnTypeNavigation.NoOfDay).Subtract(DateTime.Now).TotalDays));
                        if (isLate)
                        {
                            var fh = new FineHistory { Aseq = ah.Seq, FineTypeId = 3, FineAmount = (2 * daysLate) };
                            context.Add<FineHistory>(fh);
                        }
                        ah.ReturnDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }
            }
            return RedirectToAction("ShowBorrowedBooks", new { MID = MID, lsv = lsv });
        }
    }
}
