using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OnlineLibrary.Models;
using Microsoft.EntityFrameworkCore;



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
        public IActionResult Index()
        {
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

        public IActionResult Error()
        {
            return View();
        }
        public IActionResult Logout_admin()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }


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
                    ["Name"] = empAcc.FirstName + " " + empAcc.LastName,
                    ["ID"] = empAcc.Id,
                    ["Avatar"] = "/images/avatar.jpg"
                };

                HttpContext.Session.Set("CurStaff", accountInfo);
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
                    return context.Account.FirstOrDefault(a => a.Id == acc.Id && a.Rid == "EMP");
                }
                else
                {
                    return context.Account.FirstOrDefault(a => a.Id == acc.Id && a.Password == acc.Password && a.Rid == "EMP");
                }

            }

        }

        public IActionResult Detail_Info(int titleID)
        {
            using (var context = new OnlineLibraryContext())
            {
                var title_info = context.Title.Where(t => t.Seq == titleID)
                    .Include(t => t.TitleCategoryDetail)
                        .ThenInclude(cd => cd.Category)
                    .Include(t => t.Author).ToList();
                ViewBag.Title_Info = title_info;

                var top_view_title = context.Title.OrderByDescending(t => t.Views).Take(5).ToList();
                ViewBag.Top_View_Title = top_view_title;
            }
            return View("Book-detail");
        }

        public IActionResult Add_To_Fav(int titleId)
        {
            //var userId = HttpContext.Session.Get("E00001  ");
            var userSEQ = 1;
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

            }
            return Get_Fav(member);
        }

        public IActionResult Get_Fav(Account member)
        {
            using (var context = new OnlineLibraryContext())
            {
                var resultList = context.FavoriteList.Where(f => f.MemberId == member.Seq)
                    .Include(fav => fav.Title).ToList();
                ViewBag.ResultList = resultList;

                ViewBag.AccountInfo = member;

                var accountInfo = context.Account.Where(acc => acc.Seq == member.Seq).Include(acc => acc.R).ToList()[0];
                ViewBag.AccountInfo = accountInfo;

                var activityInfo = context.ActivityHistory.Where(act => act.Mid == member.Id).ToList();
                ViewBag.ActivityList = accountInfo;
            }
            return View("Book-favorite");
        }

    }
}
