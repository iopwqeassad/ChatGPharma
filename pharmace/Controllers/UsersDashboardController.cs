using HUc.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using pharmace.Models;

namespace pharmace.Controllers
{
    public class UsersDashboardController : Controller
    {
        PharmacyContext context = new PharmacyContext();

        public IActionResult AllUsers(int? page)
        {
            var users = context.userpermations.ToList();
            if (users.Count ==0)
            {
                List<userpermations> list = new List<userpermations>();
                return View(users);

            }
            var pageSize = 10;
            var totalStudents = users.Count();

            var totalPages = (int)Math.Ceiling((double)totalStudents / pageSize);

            page = Math.Max(page ?? 1, 1);
            page = Math.Min(page ?? 1, totalPages);

            ViewBag.CurrentPage = page;

            var students = users
                .Skip(((int)page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = totalPages;
            return View(students);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult add(string fname, string sname, string Emaile, string passworde, string passwordconfirme, string? role,string? address)
        {
            if (passworde != passwordconfirme)
            {
                ViewBag.Password = "كلمت السر غير متشابه";
                return RedirectToAction(nameof(AllUsers));
            }
            userpermations x = new userpermations()
            {
                Email = Emaile,
                Password = CommonMethods.ConvertToEncrypt(passworde),
                fname = fname,
                lname = sname,
                Address=address,
                Username = fname + " " + sname,
                Role = role 
            };

            context.userpermations.Add(x);
            context.SaveChanges();
            return RedirectToAction(nameof(AllUsers));
        }

        public async Task<IActionResult> notification()
        {
            List<Orders> orders = await context.Orders.Include(c => c.user)
               .OrderByDescending(c => c.Id).ToListAsync();
            if (orders.Count == 0)
            {
                List<Orders> list = new List<Orders>();
                return View(list);

            }
            return View(orders);

        }

        [HttpGet]
        [HttpGet]
        public IActionResult GetLatestNotifications()
        {
            var latestOrders = context.Orders
                .OrderByDescending(o => o.Date)
                .Take(5)
                .Include(o => o.user)
                .Select(o => new {
                    OrderId = o.Id,
                    UserName = o.user.fname + " " + o.user.lname,
                    OrderTime = o.Date
                })
                .ToList();

            return Json(latestOrders);
        }



        public async Task<IActionResult> View_OrderDetailsAsync(int id)
        {
            var userid =context.Orders.Where(c=>c.Id == id).Select(c=>c.userId).FirstOrDefault();
            ViewBag.user=context.userpermations.Where(c=>c.Id==userid).FirstOrDefault();
            ViewBag.order = await context.Orders.Include(c => c.user)
                        .Where(c => c.Id==id).FirstOrDefaultAsync();
            var proudect =context.orderprodects.Include(c=>c.proudect)
                .Where(c=>c.orderId==id).ToList();
            float sum = 0;
            foreach (var p in proudect)
            {
                sum += p.count * p.proudect.Price;
            }
            ViewBag.sum = sum;
            return View(proudect);
        }
        [HttpGet]
        public IActionResult deleteuser(int id)
        {
            var user = context.userpermations.FirstOrDefault(p => p.Id == id);

            if (user != null)
            {
                //if (user.PersonalPhoto is not null)
                //    DocumentSettings.DeleteFile(user.PersonalPhoto, "student");

                context.userpermations.Remove(user);
                context.SaveChanges();
            }
            return RedirectToAction("AllUsers");
        }


    }
}
