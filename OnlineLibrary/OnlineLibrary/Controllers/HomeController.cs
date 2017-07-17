using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OnlineLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.Globalization;


namespace OnlineLibrary.Controllers
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) :
                                  JsonConvert.DeserializeObject<T>(value);
        }
    }

    public class HomeController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private MsgObject msg = null;

        public HomeController(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
        }

        private Dictionary<string, string> checkCurAcc()
        {
            Dictionary<string, string> curAcc = HttpContext.Session.Get<Dictionary<string, string>>("CurAcc");
            return curAcc;
        }

        public IActionResult Index()
        {
            var curAcc = checkCurAcc();
            ViewBag.CurAcc = curAcc;
            using (var context = new OnlineLibraryContext())
            {
                var title = context.Title.OrderByDescending(a => a.Rating).ToList();
                if (title != null)
                {
                    ViewBag.Title = title;
                }

                var newestTitle = context.Title.OrderByDescending(a => a.PublishYear).ToList();
                if (title != null)
                {
                    ViewBag.NewestTitle = newestTitle;
                }

                var tag = context.Category.ToList();
                if (tag != null)
                {
                    ViewBag.Tag = tag;
                }
            }
            return View("Main");
        }

        public IActionResult Login_admin()
        {
            //if (HttpContext.Session.Get("CurStaff") != null)
            //{
            //    return RedirectToAction("Check_login_admin", null);
            //}
            //else
            return View();
        }
        public IActionResult Login_member(string oldPage, int title)
        {
            //if (HttpContext.Session.Get("CurStaff") != null)
            //{
            //    return RedirectToAction("Check_login_admin", null);
            //}
            //else
            ViewBag.oldPage = oldPage;
            ViewBag.title = title;
            return View("Login");
        }
        [HttpPost]
        public IActionResult Check_login_member(Account reqAcc, string oldPage, int title)
        {
            ViewBag.oldPage = oldPage;
            ViewBag.title = title;
            using (var context = new OnlineLibraryContext())
            {
                var account = context.Account.Where(acc => acc.Id == reqAcc.Id && acc.Password == reqAcc.Password && acc.Rid == "MEM" && acc.Status == true).SingleOrDefault();
                if (account != null)
                {
                    var curAcc = new Dictionary<string, string>
                    {
                        ["Seq"] = account.Seq.ToString(),
                        ["ID"] = reqAcc.Id,
                        ["Avatar"] = account.Avatar,
                        ["FirstName"] = $"{account.FirstName}",
                        ["LastName"] = $"{account.LastName}"
                    };
                    HttpContext.Session.Set("CurAcc", curAcc);
                    ViewBag.curAcc = curAcc;
                    string hiMsg = null;
                    Random sayHello = new Random();
                    switch (sayHello.Next(1, 3))
                    {
                        case 1:
                            hiMsg = $"It's nice to see you again, {account.FirstName}";
                            break;
                        case 2:
                            hiMsg = $"Yo! Have a nice day, {account.FirstName}";
                            break;
                        case 3:
                            hiMsg = $"‘Ello mate, {account.FirstName}";
                            break;
                        case 4:
                            hiMsg = $"{account.FirstName}! Long time no see!";
                            break;
                    }

                    ViewBag.msg = new MsgObject
                    {
                        msgTitle = "Login success!",
                        msgCode = 1,
                        msg = hiMsg
                    };

                    if (oldPage == null)
                    {
                        return Index();
                    }
                    else if (oldPage == "Book-detail")
                    {
                        return Book_Detail_Info(title);
                    }
                }
            }
            ViewBag.Msg = "Your Member ID or Password is not correct!";
            return View("Login");
        }

        public IActionResult Error()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Index();
        }

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
                ViewBag.curAcc = accountInfo;
                return View("../Dashboard/Index");
            }
            else
            {
                ViewBag.msg = "Login Failed\nYour Staff ID or Password is not correct!";
            }

            return View("~/Views/Shared/Login_admin.cshtml");

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

        public IActionResult Book_Detail_Info(int titleID)
        {
            var curAcc = checkCurAcc();
            ViewBag.CurAcc = curAcc;

            using (var context = new OnlineLibraryContext())
            {
                var title_info = context.Title.Where(t => t.Seq == titleID)
                    .Include(t => t.TitleCategoryDetail)
                        .ThenInclude(cd => cd.Category)
                    .Include(t => t.ReturnTypeNavigation)
                    .Include(t => t.Author).SingleOrDefault();

                ((Title)title_info).Views++;
                context.SaveChanges();
                if (curAcc != null)
                {
                    var userSEQ = Int32.Parse(curAcc["Seq"]);
                    var isAddedToFav = context.FavoriteList.Where(fl => fl.TitleId == titleID && fl.MemberId == userSEQ).SingleOrDefault();
                    var isRated = context.Rating.Where(rt => rt.Mseq == userSEQ && rt.TitleSeq == titleID).SingleOrDefault();
                    ViewBag.IsAdded = isAddedToFav;
                    ViewBag.IsRated = isRated;
                }


                var top_view_title = context.Title.Include(t => t.Author).OrderByDescending(t => t.Views).Take(5).ToList();

                ViewBag.Title_Info = title_info;

                ViewBag.Top_View_Title = top_view_title;
            }

            return View("Book-detail");
        }

        public IActionResult Remove_From_Fav(int titleId)
        {
            try
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
                var userSEQ = Int32.Parse(curAcc["Seq"] ?? "-1");
                Account member;
                using (var context = new OnlineLibraryContext())
                {
                    FavoriteList fav_list = context.FavoriteList.Where(fl => fl.TitleId == titleId && fl.MemberId == userSEQ).SingleOrDefault();
                    context.Remove(fav_list);
                    context.SaveChanges();
                    member = context.Account.Find(userSEQ);
                    ViewBag.msg = new MsgObject
                    {
                        msgTitle = "Favorite list removed successfully!",
                        msgCode = 1,
                        msg = "Title was removed from your list"
                    };
                }
            }
            catch (Exception e)
            {
                ViewBag.msg = new MsgObject
                {
                    msgTitle = "Favorite list removing failed!",
                    msgCode = -1,
                    msg = "Error: " + e
                };
            }

            return Get_Fav();
        }
        public IActionResult Add_To_Fav(int titleId)
        {
            //var userId = HttpContext.Session.Get("E00001  ");
            try
            {
                //Check cur user
                var curAcc = checkCurAcc();
                if (curAcc == null)
                {
                    return View("Login");
                }
                else
                {
                    ViewBag.CurAcc = curAcc;
                }
                //End check
                var userSEQ = Int32.Parse(curAcc["Seq"] ?? "-1");
                Account member;
                using (var context = new OnlineLibraryContext())
                {
                    FavoriteList fav_list = new FavoriteList
                    {
                        TitleId = titleId,
                        MemberId = userSEQ
                    };
                    context.Add(fav_list);
                    context.SaveChanges();
                    member = context.Account.Find(userSEQ);
                    ViewBag.msg = new MsgObject
                    {
                        msgTitle = "Favorite list added successfully!",
                        msgCode = 1,
                        msg = "A new title was added in your list"
                    };
                }
            }
            catch (Exception e)
            {
                ViewBag.msg = new MsgObject
                {
                    msgTitle = "Adding to Favorite failed!",
                    msgCode = -1,
                    msg = "Error: " + e
                };
            }

            return Get_Fav();
        }

        public IActionResult Get_Fav()
        {
            var curAcc = checkCurAcc();
            var userSeq = int.Parse(curAcc["Seq"] ?? "-1");
            using (var context = new OnlineLibraryContext())
            {
                var resultList = context.FavoriteList.Where(f => f.MemberId == userSeq)
                    .Include(fav => fav.Title).ToList();
                ViewBag.ResultList = resultList;

                ViewBag.AccountInfo = context.Account.Find(userSeq);

                var accountInfo = context.Account.Where(acc => acc.Seq == userSeq).Include(acc => acc.R).ToList()[0];
                ViewBag.AccountInfo = accountInfo;

                var activityInfo = context.ActivityHistory.Where(act => act.Mid == accountInfo.Id)
                    .Include(act => act.FineHistory)
                        .ThenInclude(fh => fh.FineType)
                    .Include(act => act.B)
                        .ThenInclude(b => b.TitleNavigation)
                            .ThenInclude(t => t.ReturnTypeNavigation)
                    .ToList();
                ViewBag.ActivityList = activityInfo;


            }
            if (msg != null)
            {
                ViewBag.msg = new MsgObject
                {
                    msgTitle = "Updating successfully!",
                    msgCode = 1,
                    msg = "Your profile was updated"
                };
                msg = null;
            }
            ViewBag.CurAcc = curAcc;
            return View("Book-favorite");
        }


        public IActionResult Rating(int yourRate, int titleSeq)
        {
            //Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            var userSEQ = int.Parse(curAcc["Seq"] ?? "-1");
            if (yourRate > 5)
            {
                return Book_Detail_Info(titleSeq);
            }
            using (var context = new OnlineLibraryContext())
            {
                var updatedTitle = context.Title.Find(titleSeq);

                if (updatedTitle != null)
                {

                    //Add new obj in ratingTbl
                    var isRate = context.Rating.Where(r => r.Mseq == userSEQ && r.TitleSeq == titleSeq).FirstOrDefault();
                    if (isRate != null)
                    {

                        //Add new obj in titleTbl
                        updatedTitle.Rating = Math.Floor((double)(updatedTitle.Rating * updatedTitle.RatingNo - isRate.Rating1 + yourRate) / ((double)updatedTitle.RatingNo));
                        isRate.Rating1 = yourRate;
                    }
                    else
                    {
                        Rating updateRating = new Rating
                        {
                            Mseq = userSEQ,
                            TitleSeq = updatedTitle.Seq,
                            Rating1 = yourRate
                        };
                        context.Rating.Add(updateRating);

                        //Add new obj in titleTbl
                        updatedTitle.Rating = Math.Floor((double)(updatedTitle.Rating * updatedTitle.RatingNo + yourRate) / ((double)++updatedTitle.RatingNo));
                    }
                    context.SaveChanges();
                }
            }
            return Book_Detail_Info(titleSeq);
        }


        public IActionResult SearchByCat()
        {
            using (var context = new OnlineLibraryContext())
            {
                ViewBag.catList = context.Category.ToList();
                var top_view_title = context.Title
                   .Include(t => t.Author)
                   .OrderByDescending(t => t.Views).Take(5).ToList();
                ViewBag.Top_View_Title = top_view_title;
            }


            return View("Book-search-cat");
        }

        public IActionResult AutoSearchByCat(string catString)
        {
            List<string> catList = new List<string>(catString.Split('-'));
            try
            {
                using (var context = new OnlineLibraryContext())
                {
                    if (catList != null)
                    {
                        List<Title> SearchResult = new List<Title>();
                        var titleList = context.TitleCategoryDetail.Include(t=>t.Title).ThenInclude(t=>t.Author).GroupBy(t=> new {title= t.Title, cat = t.CategoryId}).ToList();
                        foreach(var t in titleList)
                        {
                            if (catList.Contains(t.Key.cat.ToString())&&!SearchResult.Contains(t.Key.title))
                            {
                                SearchResult.Add(t.Key.title);
                            }
                        }
                        
                        
                        ViewBag.SearchResult = SearchResult;
                        ViewBag.OldCats = catList;

                        ViewBag.catList = context.Category.ToList();
                        var top_view_title = context.Title
                           .Include(t => t.Author)
                           .OrderByDescending(t => t.Views).Take(5).ToList();
                        ViewBag.Top_View_Title = top_view_title;
                    }
                }
            }
            catch
            {
                return SearchByCat();
            }
            return View("Book-search-cat");
        }
        public IActionResult Search(IFormCollection data)
        {
            using (var context = new OnlineLibraryContext())
            {
                if (data["searchBy"] == "Author")
                {
                    var result = context.Title
                        .Include(t => t.Author)
                        .Where(t => ($"{t.Author.LastName} {t.Author.FirstName}".ToLower().Contains(data["searchVal"].ToString().ToLower()))).ToList();
                    ViewBag.Result = result;
                }
                else if (data["searchBy"] == "Title" || data["searchBy"].Count() == 0)
                {
                    var result = context.Title
                        .Include(t => t.Author)
                        .Where(t => ($"{t.Name} {t.FullName}".ToLower().Contains(data["searchVal"].ToString().ToLower()))).ToList();
                    ViewBag.Result = result;
                }

                var top_view_title = context.Title
                    .Include(t => t.Author)
                    .OrderByDescending(t => t.Views).Take(5).ToList();
                ViewBag.Top_View_Title = top_view_title;

                ViewBag.curAcc = checkCurAcc();
            }
            return View("Book-search-detail");
        }

        public IActionResult User_Modify()
        {
            //Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            using (var context = new OnlineLibraryContext())
            {
                var account = context.Account.Find(Int32.Parse(curAcc["Seq"]));
                return View("User-modify", new UpdateInfo
                {
                    Id = account.Id,
                    FirstName = account.FirstName,
                    LastName = account.LastName,
                    Address = account.Address,
                    Phone = account.Phone,
                    Email = account.Email,
                    Password = account.Password,
                    Gender = account.Sex,
                    Dob = ((DateTime)account.Dob).ToString("dd/MM/yyyy"),
                    Seq = account.Seq,
                    Avatar = account.Avatar
                });
            }
        }



        [HttpPost]
        public async Task<IActionResult> Update_UInfo(IFormFile file, UpdateInfo account, string typeSubmit)
        {
            //Check cur user
            var curAcc = checkCurAcc();
            if (curAcc == null)
            {
                return View("Login");
            }
            else
            {
                ViewBag.CurAcc = curAcc;
            }
            //End check
            if (file != null)
            {
                var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                if (file.Length > 0)
                {
                    try
                    {
                        var filePath = Path.Combine(uploads, file.FileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        ViewBag.TempFile = file.FileName;
                        ViewBag.OldData = account;
                        ViewBag.msg = new MsgObject
                        {
                            msgTitle = "Uploading successfully!",
                            msgCode = 1,
                            msg = "Your avatar was uploaded successfully"
                        };
                    }
                    catch (Exception e)
                    {
                        ViewBag.OldData = account;
                        ViewBag.msg = new MsgObject
                        {
                            msgTitle = "Uploading failed!",
                            msgCode = -1,
                            msg = "Error: " + e.Message
                        };
                    }
                    return View("User-modify", account);
                }
            }

            if (typeSubmit == null)
            {
                try
                {
                    using (var context = new OnlineLibraryContext())
                    {
                        var acc = context.Account.Find(account.Seq);
                        acc.FirstName = account.FirstName;
                        acc.LastName = account.LastName;
                        acc.Password = account.Password;
                        acc.Address = account.Address;
                        try
                        {
                            acc.Dob = (DateTime?)DateTime.ParseExact(account.Dob, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        catch (Exception e)
                        {
                            ViewBag.msg = new MsgObject
                            {
                                msgTitle = "Submit failed!",
                                msgCode = -1,
                                msg = "Date of Birth is in wrong format!\nError: " + e
                            };
                            return View("User-modify", account);
                        }

                        acc.Email = account.Email;
                        acc.Avatar = account.Avatar;
                        acc.Sex = account.Gender;
                        context.Account.Update(acc);
                        context.SaveChanges();

                        //Update info in session scope
                        Dictionary<string, string> accountInfo = new Dictionary<string, string>
                        {
                            ["Seq"] = account.Seq.ToString(),
                            ["ID"] = account.Id,
                            ["Avatar"] = $@"{account.Avatar}",
                            ["FirstName"] = $"{account.FirstName}",
                            ["LastName"] = $"{account.LastName}"
                        };

                        HttpContext.Session.Set("CurAcc", accountInfo);
                        ViewBag.curAcc = accountInfo;
                    }
                }
                catch
                {
                    ViewBag.msg = new MsgObject
                    {
                        msgTitle = "Updating failed!",
                        msgCode = -1,
                        msg = "Account information was not updated!"
                    };
                    return View("User-modify", account);
                }
            }
            msg = ViewBag.msg = new MsgObject
            {
                msgTitle = "Updating successfully!",
                msgCode = 1,
                msg = "Account information updating successfully!"
            };

            return Get_Fav();

        }

    }
}
