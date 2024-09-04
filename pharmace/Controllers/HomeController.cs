using HUc.Filters;
using HUc.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using pharmace.Models;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using pharmace.Constants;
using static NuGet.Packaging.PackagingConstants;
using System.Collections.Generic;
using pharmace.Helper;

namespace pharmace.Controllers
{
    public class HomeController : Controller
    {
        private int GetLoggedInUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }
        PharmacyContext context = new PharmacyContext();

        private readonly ILogger<HomeController> _logger;
        private readonly PharmacyContext _context;


        public HomeController(ILogger<HomeController> logger, PharmacyContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.categories =context.Categories.ToList();
            ViewBag.Proudect = context.Proudects
                                          .OrderBy(p => Guid.NewGuid())
                .Take(10).ToList();
            ViewBag.prodectcat = context.Proudects.Include(c=>c.category)
                .Where(c=>c.category.Name== "منتجات للنظافة الشخصية")
                                          .OrderBy(p => Guid.NewGuid())
                .Take(10).ToList();
            ViewBag.prodectvet = context.Proudects.Include(c=>c.category)
                .Where(c=>c.category.Name== "الفيتامينات" || c.category.Name == "المكملات الغذائية")
                                          .OrderBy(p => Guid.NewGuid())
                .Take(10).ToList();
            var orders = context.Offers
                .Include(c=>c.proudect)
                          .OrderBy(p => Guid.NewGuid()) 
                          .Take(10)                    
                          .ToList();

            if (orders.Count == 0)
            {
                List<Orders> list = new List<Orders>();
                ViewBag.offers = list;
                return View();
            }
            else
            {
                ViewBag.offers = orders;
                return View();
            }
                
        }


        public IActionResult contantUs()
        {
            return View();
        }


        public async Task<IActionResult> accountinfo()
        {
            List<Orders> orders = await context.Orders.Include(c => c.user)
                .Where(c=>c.user.Id == GetLoggedInUserId())
                .OrderByDescending(c => c.Id).ToListAsync();
            List<List<orderDTO>> ordersDTO = new List<List<orderDTO>>();
            foreach (var order in orders)
            {
                var p = context.orderprodects.Include(c => c.proudect)
                    .Where(c => c.orderId == order.Id)
                    .Select(c => new orderDTO
                    {
                        name = c.proudect.Name,
                        count =c.count,
                        price =c.proudect.Price,
                        image =c.proudect.image
                    })
                    .ToList();
                ordersDTO.Add(p);
            }
            ViewBag.ordersDTO = ordersDTO;
            if (orders.Count == 0)
            {
                List<Orders> list = new List<Orders>();
                ViewBag.myorder = list;

                userpermations xx = context.userpermations.Where(c => c.Id == GetLoggedInUserId()).FirstOrDefault();
                return View(xx);
            }
             
            ViewBag.myorder = orders;


            userpermations x = context.userpermations.Where(c=>c.Id == GetLoggedInUserId()).FirstOrDefault();
            return View(x);
        }

        [AuthorizeUser]
        public IActionResult Privacy()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await context.userpermations.FirstOrDefaultAsync(u => u.Email == email);
            //var olduser = CommonMethods.ConvertToDecrypt(user.Password);

            try
            {
                if (password == null)
                    password = "";
                if (user is null)
                    return View(nameof(Create));
                else if (password == CommonMethods.ConvertToDecrypt(user.Password))
                {
                    if (!Enum.GetNames(typeof(AllowedRoles)).ToList().Contains(user?.Role))
                    {
                        return View(nameof(Index));
                    }
                    List<Claim> claims = new List<Claim>{
                    new Claim("userId",  user.Id.ToString()),
                    new Claim("role", user?.Role)
                };
                    var appIdentity = new ClaimsIdentity(claims);

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
                else
                    return View(nameof(Create));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ اثناء التسجيل");
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return View(nameof(Create));
            }

        }


        public IActionResult signup()
        {
            return View();
        }

        [AuthorizeUser]
        public async Task<IActionResult> Logout(string email, string password)
        {

            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNew(string fname, string sname, string Email, string password, string passwordconfirm ,string? role)
        {
            if (password != passwordconfirm)
            {
                ViewBag.Password = "كلمت السر غير متشابه";
                return RedirectToAction(nameof(signup));
            }
            userpermations x = new userpermations()
            {
                Email=Email,
                Password= CommonMethods.ConvertToEncrypt(password),
                fname=fname,
                lname=sname,
                Username=fname+" "+sname,
                Role=role ??"user"
            };
            
            context.userpermations.Add(x);
            context.SaveChanges();
                return RedirectToAction(nameof(Create));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string? firstName, string? lastName, string? email, string? phone, int id,string? address)
        {
            userpermations x = context.userpermations.Where(c => c.Id == GetLoggedInUserId()).FirstOrDefault();
            x.Email=email??x.Email;
            x.Address=address??x.Address;
            x.PhoneNumber=phone ??x.PhoneNumber;
            x.fname= firstName ??x.fname;
            x.lname= lastName ?? x.lname;
            x.Username = firstName ?? x.fname + " " + lastName ?? x.lname;

            context.SaveChanges();
            TempData["Message"] = "تم تحديث المعلومات بنجاح!";

            return RedirectToAction(nameof(accountinfo));

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPassword(int id, string currentPassword, string newPassword, string confirmPassword)
        {
            userpermations x = context.userpermations.Where(c => c.Id == GetLoggedInUserId()).FirstOrDefault();
            if (newPassword != confirmPassword)
            {
                TempData["AlertMessage"] = "كلمة السر غير متطابقة";
                TempData["AlertType"] = "danger";
            }
            else if (CommonMethods.ConvertToEncrypt(currentPassword) == x.Password)
            {
                x.Password = CommonMethods.ConvertToEncrypt(newPassword);
                context.SaveChanges();
                TempData["AlertMessage"] = "تم تحديث كلمة المرور بنجاح";
                TempData["AlertType"] = "success";
            }
            else
            {
                TempData["AlertMessage"] = "كلمة السر الحالية غير صحيحة";
                TempData["AlertType"] = "danger";
            }
            return RedirectToAction("accountinfo");
        }

        //public ActionResult add(int id)
        //{
        //    var userid = GetLoggedInUserId();

        //    var x = context.Carts.Where(c=>c.userId == userid).ToList();
        //    foreach (var c in x) 
        //    {
        //        if(c.proudectId == id)
        //        {
        //            var r = context.Carts.Where(c=>c.userId == userid && c.proudectId==id).FirstOrDefault();
        //            r.count++;
        //            context.SaveChanges();
        //            return RedirectToAction(nameof(Index));
        //        }
        //    }
        //    var cart = new Cart
        //    {
        //        userId = userid,
        //        proudectId = id,
        //        count = 1
        //    };
        //    context.Carts.Add(cart);
        //    context.SaveChanges();
        //    return RedirectToAction(nameof(Index));
        //}

        [HttpPost]
        public IActionResult add(int id)
        {
            var userid = GetLoggedInUserId();

            var existingCartItem = context.Carts.FirstOrDefault(c => c.userId == userid && c.proudectId == id);

            if (existingCartItem != null)
            {
                existingCartItem.count++;
                context.SaveChanges();

               
                return Json(new { success = true, message = "تمت الاضافة" });
            }

            var newCartItem = new Cart
            {
                userId = userid,
                proudectId = id,
                count = 1
            };
            context.Carts.Add(newCartItem);
            context.SaveChanges();


            return Json(new { success = true, message = "تمت اضافة المنتج" });
        }


    }
}
