using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineLibrary.Controllers
{
    public class DashboardController : Controller
    {
        //For Login
        [HttpPost]
        public IActionResult Check_login_admin(Account reqAcc)
        {
            ////Case 1: Login with session in the past
            //if (reqAcc == null)
            //{
            //    reqAcc = new Account
            //    {
            //        Id = HttpContext.Session.Get<Dictionary<string, string>>("CurStaff")["ID"]
            //    };
            //}
            //Case 2: First login
            Account empAcc = getEmpAcc(reqAcc);
            if (empAcc != null)
            {
                Dictionary<string, string> accountInfo = new Dictionary<string, string>
                {
                    ["Seq"] = empAcc.Seq.ToString(),
                    ["ID"] = empAcc.Id,
                    ["Avatar"] = $@"{empAcc.Avatar}",
                    ["FirstName"] = $"{empAcc.FirstName}",
                    ["LastName"] = $"{empAcc.LastName}"
                };

                HttpContext.Session.Set("CurAcc", accountInfo);
                TempData["curAcc"] = accountInfo;
                return RedirectToAction("GetStatistic");
            }
            else
            {
                ViewBag.msg = "Login Failed\nYour Staff ID or Password is not correct!";
            }

            return View("~/Views/Shared/Login_admin.cshtml");

        }

        private Dictionary<string, string> checkCurAcc()
        {
            Dictionary<string, string> curAcc = HttpContext.Session.Get<Dictionary<string, string>>("CurAcc");
            return curAcc;
        }

        private Account getEmpAcc(Account acc)
        {
            using (var context = new OnlineLibraryContext())
            {
                if (acc.Password == null)
                {
                    return context.Account.FirstOrDefault(a => a.Id == acc.Id && a.Rid == "EMP" && a.Status == true);
                }
                else
                {
                    return context.Account.FirstOrDefault(a => a.Id == acc.Id && a.Password == acc.Password && a.Rid == "EMP" && a.Status == true);
                }

            }

        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        //End login
        public IActionResult GetStatistic()
        {
            ViewBag.curAcc = TempData["curAcc"];
            if (ViewBag.curAcc == null)
            {
                //Check cur user
                var curAcc = checkCurAcc();
                if (curAcc == null)
                {
                    return View("Login_admin");
                }
                else
                {
                    ViewBag.CurAcc = curAcc;
                }
                //End check
            }
            using (var context = new OnlineLibraryContext())
            {
                var titles = context.Title.ToList();
                double viewSum = 0;
                int titleRating = 0;
                foreach (var title in titles)
                {
                    viewSum += (double)title.Views;
                    titleRating += (int)title.RatingNo;
                }
                //Data for top nav
                ViewBag.TitleRatingSum = titleRating;
                ViewBag.ViewSum = viewSum;
                ViewBag.BookSum = context.Book.ToList().Count();
                ViewBag.TitleSum = context.Title.ToList().Count();
                ViewBag.MemberSum = context.Account.Where(acc => acc.Rid == "MEM").ToList().Count();
                ViewBag.Activity = context.ActivityHistory.ToList().Count();
                //Data for tables
                ViewBag.NewTitles = context.Title.OrderByDescending(t => t.Seq).Take(5).ToList();
                ViewBag.NewCircu = context.ActivityHistory.OrderByDescending(t => t.Seq).Take(5)
                    .Include(c => c.M)
                    .Include(c => c.B)
                        .ThenInclude(b => b.TitleNavigation)
                    .ToList();
                ViewBag.newMem = context.Account.Where(acc => acc.Rid == "MEM").OrderByDescending(m => m.Seq).Take(5).ToList();
                ViewBag.newBook = context.Book.OrderByDescending(b => b.Seq).Take(5)
                    .Include(b => b.TitleNavigation)
                        .ThenInclude(t => t.Author)
                    .Include(b => b.Status).ToList();
                ViewBag.newRating = context.Rating.OrderByDescending(t => t.RateId).Take(5)
                    .Include(r => r.MseqNavigation)
                    .Include(r => r.TitleSeqNavigation).ToList();
            }
            return View("../Dashboard/Index");
        }
        //For Report in Dashboard's index page

        //End Report

        [HttpPost]
        public IActionResult RegisterMemberToBorrow(Book b, string MID)
        {
            RegistrationMemberBookErrorObject rmbeo = new RegistrationMemberBookErrorObject();
            rmbeo.setMessageIfIFBD(MID);
            rmbeo.setMessageIfIFBAR(MID, b.Id);
            rmbeo.setMessageIfINF(MID);
            if (!rmbeo.hasError)
            {
                using (var context = new OnlineLibraryContext())
                {
                    ActivityHistory ah = new ActivityHistory { Mid = MID, Bid = b.Id, BorrowDate = DateTime.Now };
                    context.ActivityHistory.Add(ah);
                    context.Book.Where(book => book.Id.Equals(book.Id)).Single().StatusId = 3;
                    context.SaveChanges();
                }
            }
            else
            {
                HttpContext.Session.Set<RegistrationMemberBookErrorObject>("RegistrationMemberBookErrorObject", rmbeo);
            }
            return RedirectToAction("ShowTitleBooks", new { TID = HttpContext.Session.GetString("currentlyViewingTID") });
        }

        [HttpPost]
        public IActionResult BookCRUD(string btnAction, Book b)
        {
            BookErrorObject beo = new BookErrorObject();
            using (var context = new OnlineLibraryContext())
            {
                switch (btnAction)
                {
                    case "create":
                        beo.setErrorMessageIfIdDuplicated(b.Id);
                        if (!beo.hasError)
                        {
                            context.Book.Add(b);
                            context.SaveChanges();
                        }
                        break;
                    case "update":
                        beo.setErrorMessageIfIdNotFound(b.Id);
                        if (!beo.hasError)
                        {
                            context.Book.Update(b);
                            context.SaveChanges();
                        }
                        break;
                }
            }
            if (beo.hasError)
            {
                HttpContext.Session.Set<BookErrorObject>("BookErrorObject", beo);
            }
            return RedirectToAction("ShowTitleBooks", new { TID = HttpContext.Session.GetString("currentlyViewingTID") });
        }

        [HttpPost]
        public IActionResult TitleCRUD(string btnAction, Title t, string[] categories)
        {
            TitleCategoryDetail tcd = null;
            List<string> categoriesList = categories.ToList();
            using (var context = new OnlineLibraryContext())
            {
                switch (btnAction)
                {
                    case "create":
                        t.Rating = 0;
                        t.Views = 0;
                        t.RatingNo = 0;
                        context.Title.Add(t);
                        foreach (string c in categoriesList)
                        {
                            tcd = new TitleCategoryDetail { TitleId = t.Seq, CategoryId = Int32.Parse(c) };
                            context.TitleCategoryDetail.Add(tcd);
                        }
                        context.SaveChanges();
                        break;
                    case "update":
                        var title = context.Title.Where(a => a.Seq == t.Seq).Single();
                        title.Name = t.Name;
                        title.FullName = t.FullName;
                        title.Publisher = t.Publisher;
                        title.PublishYear = t.PublishYear;
                        title.Isbn = t.Isbn;
                        title.AvailableQuan = t.AvailableQuan;
                        title.Illu = t.Illu;
                        title.Description = t.Description;
                        title.AuthorId = t.AuthorId;
                        title.ReturnType = t.ReturnType;
                        context.SaveChanges();
                        context.TitleCategoryDetail.RemoveRange(context.TitleCategoryDetail.Where(a => a.TitleId == t.Seq));
                        context.SaveChanges();
                        foreach (string c in categoriesList)
                        {
                            tcd = new TitleCategoryDetail { TitleId = t.Seq, CategoryId = Int32.Parse(c) };
                            context.TitleCategoryDetail.Add(tcd);
                        }
                        context.SaveChanges();
                        break;
                }
            }
            return RedirectToAction("GetTitleList");
        }

        [HttpPost]
        public IActionResult SearchBook(string BID, string titleID)
        {
            List<Book> result = null;
            List<BookStatus> statuses = null;
            using (var context = new OnlineLibraryContext())
            {
                if (BID == null)
                {
                    result = context.Book.Where(b => b.Title == Int32.Parse(titleID)).Include(b => b.Status).Include(b => b.TitleNavigation).ToList();
                    BID = "";
                }
                else
                {
                    result = context.Book.Where(b => b.Title == Int32.Parse(titleID) && b.Id.Contains(BID)).Include(b => b.Status).Include(b => b.TitleNavigation).ToList();
                }
                statuses = context.BookStatus.ToList();
            }
            ViewBag.statuses = statuses;
            ViewBag.result = result;
            if (BID != null)
            {
                HttpContext.Session.SetString("lastSearchValueSub2", BID);
            }
            return View("ShowTitleBooks");
        }

        [HttpGet]
        public IActionResult ViewBook(string BID)
        {
            //Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            List<Book> result = null;
            Book bookBeingViewed = null;
            List<BookStatus> statuses = null;
            string titleID = HttpContext.Session.GetString("currentlyViewingTID");
            string lsv = HttpContext.Session.GetString("lastSearchValueSub2");
            using (var context = new OnlineLibraryContext())
            {
                if (lsv == null || lsv.Equals(""))
                {
                    result = context.Book.Where(b => b.Title == Int32.Parse(titleID)).Include(b => b.Status).Include(b => b.TitleNavigation).ToList();
                }
                else
                {
                    result = context.Book.Where(b => b.Id.Contains(lsv) && b.Title == Int32.Parse(titleID)).Include(b => b.Status).Include(b => b.TitleNavigation).ToList();
                }
                statuses = context.BookStatus.ToList();
                foreach (Book b in result)
                {
                    if (b.Id.Equals(BID))
                    {
                        bookBeingViewed = b;
                        break;
                    }
                }
                ViewBag.result = result;
                ViewBag.bookBeingViewed = bookBeingViewed;
                ViewBag.statuses = statuses;
            }



            return View("ShowTitleBooks");
        }

        [HttpGet]
        public IActionResult ShowTitleBooks(string TID)
        {
            // Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check

            List<Book> result = null;
            List<BookStatus> statuses = null;
            using (var context = new OnlineLibraryContext())
            {
                result = context.Book.Where(b => b.Title == Int32.Parse(TID)).Include(b => b.Status).Include(b => b.TitleNavigation).ToList();
                statuses = context.BookStatus.ToList();
            }
            ViewBag.result = result;
            ViewBag.statuses = statuses;
            HttpContext.Session.SetString("currentlyViewingTID", TID);
            return View();
        }

        [HttpGet]
        public IActionResult SearchTitleList(string TitleName, bool useToOverload)
        {
            // Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            List<Title> result = null;
            List<Author> authors = null;
            List<ReturnType> returnTypes = null;
            List<Category> categories = null;
            using (var context = new OnlineLibraryContext())
            {
                if (TitleName == null)
                {
                    result = context.Title.Include(t => t.Author).Include(t => t.TitleCategoryDetail).ThenInclude(cd => cd.Category).ToList();
                    TitleName = "";
                }
                else
                {
                    result = context.Title.Where(t => t.Name.Contains(TitleName)).Include(t => t.Author).Include(t => t.TitleCategoryDetail).ThenInclude(cd => cd.Category).ToList();
                }
                ViewBag.result = result;
                if (TitleName != null)
                {
                    HttpContext.Session.SetString("lastSearchValue", TitleName);
                }
                categories = context.Category.ToList();
                returnTypes = context.ReturnType.ToList();
                authors = context.Author.ToList();
                ViewBag.categories = categories;
                ViewBag.authors = authors;
                ViewBag.returnTypes = returnTypes;
            }
            return View("GetTitleList");
        }

        [HttpPost]
        public IActionResult SearchTitleList(string TitleName)
        {
            // Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            List<Title> result = null;
            List<Author> authors = null;
            List<ReturnType> returnTypes = null;
            List<Category> categories = null;
            using (var context = new OnlineLibraryContext())
            {
                if (TitleName == null)
                {
                    result = context.Title.Include(t => t.Author).Include(t => t.TitleCategoryDetail).ThenInclude(cd => cd.Category).ToList();
                    TitleName = "";
                }
                else
                {
                    result = context.Title.Where(t => t.Name.Contains(TitleName)).Include(t => t.Author).Include(t => t.TitleCategoryDetail).ThenInclude(cd => cd.Category).ToList();
                }
                categories = context.Category.ToList();
                returnTypes = context.ReturnType.ToList();
                authors = context.Author.ToList();
                ViewBag.categories = categories;
                ViewBag.authors = authors;
                ViewBag.returnTypes = returnTypes;
                ViewBag.result = result;
                if (TitleName != null)
                {
                    HttpContext.Session.SetString("lastSearchValue", TitleName);
                }
            }
            return View("GetTitleList");
        }

        [HttpGet]
        public IActionResult ViewTitle(string TID)
        {
            // Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            List<Title> result = null;
            List<Author> authors = null;
            List<ReturnType> returnTypes = null;
            List<Category> categories = null;
            Title titleBeingViewed = null;
            string lsv = HttpContext.Session.GetString("lastSearchValue");
            using (var context = new OnlineLibraryContext())
            {
                if (lsv == null || lsv.Equals(""))
                {
                    result = context.Title.Include(t => t.Author).Include(t => t.TitleCategoryDetail).ThenInclude(cd => cd.Category).ToList();
                }
                else
                {
                    result = context.Title.Where(t => t.Name.Contains(lsv)).Include(t => t.Author).Include(t => t.TitleCategoryDetail).ThenInclude(cd => cd.Category).ToList();
                }
                foreach (Title t in result)
                {
                    if (t.Seq == Int32.Parse(TID))
                    {
                        titleBeingViewed = t;
                        break;
                    }
                }
                categories = context.Category.ToList();
                returnTypes = context.ReturnType.ToList();
                authors = context.Author.ToList();
                ViewBag.categories = categories;
                ViewBag.authors = authors;
                ViewBag.returnTypes = returnTypes;
                ViewBag.result = result;
                ViewBag.titleBeingViewed = titleBeingViewed;
            }
            return View("GetTitleList");
        }

        [HttpGet]
        public IActionResult GetTitleList()
        {
            //Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            List<Title> result = null;
            List<Author> authors = null;
            List<ReturnType> returnTypes = null;
            List<Category> categories = null;
            using (var context = new OnlineLibraryContext())
            {
                result = context.Title.Include(t => t.Author).Include(rt => rt.ReturnTypeNavigation).Include(t => t.TitleCategoryDetail).ThenInclude(cd => cd.Category).ToList();
                authors = context.Author.ToList();
                returnTypes = context.ReturnType.ToList();
                authors = context.Author.ToList();
                categories = context.Category.ToList();
            }
            ViewBag.categories = categories;
            ViewBag.authors = authors;
            ViewBag.returnTypes = returnTypes;
            ViewBag.result = result;
            ViewBag.authors = authors;

            return View();
        }

        [HttpPost]
        public IActionResult UpdateFineHistory(FineHistory fh)
        {
            //Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            FineHistory fhistory = null;
            using (var context = new OnlineLibraryContext())
            {
                fhistory = context.FineHistory.Where(f => f.Aseq == fh.Aseq).First();
                fhistory.FineTypeId = fh.FineTypeId;
                fhistory.FineAmount = fh.FineAmount;
                context.SaveChanges();
            }
            return RedirectToAction("GetFineHistoryList");
        }

        [HttpPost]
        public IActionResult SearchFineHistory(string AID)
        {
            //Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            List<FineHistory> result = null;
            List<FineType> fineTypes = null;
            using (var context = new OnlineLibraryContext())
            {
                if (AID == null)
                {
                    result = context.FineHistory.Include(ft => ft.FineType).ToList();
                    AID = "";
                }
                else
                {
                    result = context.FineHistory.Where(fh => fh.Aseq == Int32.Parse(AID)).Include(ft => ft.FineType).ToList();
                }
                fineTypes = context.FineType.ToList();
                ViewBag.result = result;
                ViewBag.fineTypes = fineTypes;
                if (AID != null)
                {
                    HttpContext.Session.SetString("lastSearchValue", AID);
                }
            }
            return View("GetFineHistoryList");
        }

        [HttpGet]
        public IActionResult ViewFineHistory(string AID)
        {
            //Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            List<FineHistory> result = null;
            List<FineType> fineTypes = null;
            FineHistory fineHistoryBeingViewed = null;
            string lsv = HttpContext.Session.GetString("lastSearchValue");
            using (var context = new OnlineLibraryContext())
            {
                if (lsv == null || lsv.Equals(""))
                {
                    result = context.FineHistory.Include(ft => ft.FineType).ToList();
                }
                else
                {
                    result = context.FineHistory.Where(fh => fh.Aseq == Int32.Parse(AID)).Include(ft => ft.FineType).ToList();
                }
                fineTypes = context.FineType.ToList();
                foreach (FineHistory fh in result)
                {
                    if (fh.Aseq == Int32.Parse(AID))
                    {
                        fineHistoryBeingViewed = fh;
                        break;
                    }
                }
                ViewBag.fineTypes = fineTypes;
                ViewBag.result = result;
                ViewBag.fineHistoryBeingViewed = fineHistoryBeingViewed;
            }
            return View("GetFineHistoryList");
        }

        [HttpGet]
        public IActionResult GetFineHistoryList()
        {
            //Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            List<FineHistory> result = null;
            List<FineType> fineTypes = null;
            using (var context = new OnlineLibraryContext())
            {
                result = context.FineHistory.Include(ft => ft.FineType).ToList();
                fineTypes = context.FineType.ToList();
            }
            ViewBag.result = result;
            ViewBag.fineTypes = fineTypes;
            return View();
        }

        [HttpGet]
        public IActionResult ViewUser(string MID)
        {
            //Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            List<Account> result = null;
            List<Role> roles = null;
            Account accountBeingViewed = null;
            string lsv = HttpContext.Session.GetString("lastSearchValue");
            using (var context = new OnlineLibraryContext())
            {
                if (lsv == null || lsv.Equals(""))
                {
                    result = context.Account.ToList();
                }
                else
                {
                    result = context.Account.Where(a => a.Id.Contains(lsv)).ToList();
                }
                roles = context.Role.ToList();
                foreach (Account acc in result)
                {
                    if (acc.Id.Equals(MID))
                    {
                        accountBeingViewed = acc;
                        break;
                    }
                }
                ViewBag.roles = roles;
                ViewBag.result = result;
                ViewBag.accountBeingViewed = accountBeingViewed;
            }
            return View("GetUserList");
        }

        [HttpPost]
        public IActionResult SearchUser(string MID)
        {
            //Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            List<Account> result = null;
            List<Role> roles = null;
            using (var context = new OnlineLibraryContext())
            {
                if (MID == null)
                {
                    result = context.Account.ToList();
                    MID = "";
                }
                else
                {
                    result = context.Account.Where(a => a.Id.Contains(MID)).ToList();
                }
                roles = context.Role.ToList();
                ViewBag.result = result;
                ViewBag.roles = roles;
                if (MID != null)
                {
                    HttpContext.Session.SetString("lastSearchValue", MID);
                }
            }
            return View("GetUserList");
        }

        [HttpPost]
        public IActionResult UserCRUD(string btnAction, Account a)
        {
            //Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            UserErrorObject ueobj = new UserErrorObject();
            using (var context = new OnlineLibraryContext())
            {
                Account account;
                switch (btnAction)
                {
                    case "create":
                        ueobj.setErrorMessageIfIdDuplicated(a.Id);
                        if (!ueobj.hasError)
                        {
                            a.Status = true;
                            context.Account.Add(a);
                            context.SaveChanges();
                        }
                        break;
                    case "update":
                        ueobj.setErrorMessageIfIdNotFound(a.Id);
                        if (!ueobj.hasError)
                        {
                            context.Account.Update(a);
                            context.SaveChanges();
                        }
                        break;
                    case "deactivate":
                        ueobj.setErrorMessageIfIdNotFound(a.Id);
                        if (!ueobj.hasError)
                        {
                            account = context.Account.Where(m => m.Id.Equals(a.Id)).First();
                            account.Status = false;
                            context.SaveChanges();
                            if (a.Id.Equals(curAcc["ID"]))
                            {
                                HttpContext.Session.Clear();
                            }           
                        }
                        break;
                    case "activate":
                        ueobj.setErrorMessageIfIdNotFound(a.Id);
                        if (!ueobj.hasError)
                        {
                            account = context.Account.Where(m => m.Id.Equals(a.Id)).First();
                            account.Status = true;
                            context.SaveChanges();
                        }
                        break;
                }
            }
            if (ueobj.hasError)
            {
                HttpContext.Session.Set<UserErrorObject>("UserErrorObject", ueobj);
            }
            return RedirectToAction("GetUserList");
        }

        [HttpGet]
        public IActionResult GetUserList()
        {
            //Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            List<Account> result = null;
            List<Role> roles = null;
            using (var context = new OnlineLibraryContext())
            {
                result = context.Account.Include(r => r.R).ToList();
                roles = context.Role.ToList();
                ViewBag.result = result;
                ViewBag.roles = roles;
            }
            return View();
        }

        [HttpGet]
        public IActionResult SearchMemberBorrowList(string MID)
        {
            //Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            using (var context = new OnlineLibraryContext())
            {
                if (MID == null)
                {
                    return RedirectToAction("GetMemberBorrowList");
                }
                else
                {
                    var result = context.ActivityHistory.Where(ah => ah.ReturnDate == null && ah.Mid.Contains(MID)).Include(m => m.M.R).GroupBy(m => m.Mid).ToList();
                    ViewBag.result = result;
                }
                if (MID != null)
                {
                    HttpContext.Session.SetString("lastSearchValue", MID);
                }
                return View("GetMemberBorrowList");
            }
        }

        [HttpPost]
        public IActionResult SearchMemberBorrowList(string MID, bool useToOverload)
        {
            //Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            using (var context = new OnlineLibraryContext())
            {
                if (MID == null)
                {
                    return RedirectToAction("GetMemberBorrowList");
                }
                else
                {
                    var result = context.ActivityHistory.Where(ah => ah.ReturnDate == null && ah.Mid.Contains(MID)).Include(m => m.M.R).GroupBy(m => m.Mid).ToList();
                    ViewBag.result = result;
                }
                if (MID != null)
                {
                    HttpContext.Session.SetString("lastSearchValue", MID);
                }
                return View("GetMemberBorrowList");
            }
        }

        [HttpGet]
        public IActionResult GetMemberBorrowList()
        {
            //Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            using (var context = new OnlineLibraryContext())
            {
                var result = context.ActivityHistory.Include(m => m.M).Include(m => m.M.R).GroupBy(m => m.Mid).ToList();
                ViewBag.result = result;
                return View();
            }

        }

        [HttpGet]
        public IActionResult ShowBorrowedBooks(string MID)
        {
            //Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            List<ActivityHistory> result = null;
            Account acc = null;
            using (var context = new OnlineLibraryContext())
            {
                result = context.ActivityHistory.Where(ah => ah.Mid.Equals(MID) && ah.ReturnDate == null).Include(b => b.B.TitleNavigation).Include(b => b.B.TitleNavigation.ReturnTypeNavigation).ToList();
                acc = context.Account.Where(a => a.Id.Equals(MID)).First();
            }
            ViewBag.result = result;
            ViewBag.account = acc;
            return View();
        }

        [HttpGet]
        public IActionResult ShowBorrowedHistory(string MID)
        {
            //Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            List<ActivityHistory> result = null;
            Account acc = null;
            using (var context = new OnlineLibraryContext())
            {
                result = context.ActivityHistory.Where(ah => ah.Mid.Equals(MID) && ah.ReturnDate != null).Include(b => b.B.TitleNavigation).ToList();
                acc = context.Account.Where(a => a.Id.Equals(MID)).First();
            }
            ViewBag.result = result;
            ViewBag.account = acc;
            return View();
        }

        [HttpPost]
        public IActionResult MarkReturnedBooks(string[] returned, string MID)
        {
            //Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login_admin");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
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
            return RedirectToAction("ShowBorrowedBooks", new { MID = MID });
        }
    }
}
