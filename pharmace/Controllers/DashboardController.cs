using HUc.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using pharmace.Helper;
using pharmace.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static NuGet.Packaging.PackagingConstants;

namespace pharmace.Controllers
{
    public class DashboardController : Controller
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
        //Home
        public IActionResult dashHome()
        {
            var user = context.userpermations.Where(c => c.Id == GetLoggedInUserId()).FirstOrDefault();
            ViewBag.User = user;
            var proudects = context.Proudects.Include(c => c.category)
                .OrderByDescending(c => c.Id).Take(5);
            var numberorder = context.Orders.Select(c => c.Id).ToList().Count;
            ViewBag.numberorder = (int)numberorder;
            ViewBag.numberuser = context.userpermations.ToList().Count;
            ViewBag.offer = context.Offers.ToList().Count;
            ViewBag.catnumber = context.Categories.ToList().Count;
            return View(proudects);
        }

        //all products
        public async Task<IActionResult> AllProduct(int? page, string? query, string? category, string? pricecat)
        {
            var proudects = context.Proudects.Include(c => c.category)
                    .OrderByDescending(c => c.Id).ToList();
            if (!string.IsNullOrEmpty(category) /*|| category != "All"*/)
            {
                proudects = proudects.Where(c => c.category.Name == category).ToList();
            }
            if (!string.IsNullOrEmpty(query))
            {
                proudects = proudects.Where(c => c.Name.ToLower().StartsWith(query.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(pricecat))
            {
                if (pricecat == "lowest")
                {
                    proudects = proudects.OrderBy(c => c.Price).ToList();

                }
                else
                {
                    proudects = proudects.OrderByDescending(c => c.Price).ToList();

                }
            }
            ViewBag.category = category;
            ViewBag.query = query;
            ViewBag.pricecat = pricecat;
            ViewBag.catall = context.Categories.ToList();

            if (proudects.Count == 0)
            {
                List<Proudect> list = new List<Proudect>();
                return View(list);
            }
            var pageSize = 10;
            var totalStudents = proudects.Count();

            var totalPages = (int)Math.Ceiling((double)totalStudents / pageSize);

            page = Math.Max(page ?? 1, 1);
            page = Math.Min(page ?? 1, totalPages);

            ViewBag.CurrentPage = page;

            var students = proudects
                .Skip(((int)page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = totalPages;

            return View(students);
        }

        //add products

        public IActionResult AddProductBtn()
        {
            ViewBag.catall = context.Categories.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(string? name, string? dept, string? decription,
            int? count, float? price, IFormFile? image)
        {
            string studentImagePath = null;
            if (image is not null)
            {
                studentImagePath = DocumentSettings.UploadFile(image, "StudentPhoto", name);
            }
            var catid = context.Categories.Where(c => c.Name == dept).Select(c => c.Id).FirstOrDefault();
            var x = new Proudect
            {
                Name = name,
                Description = decription,
                count = count ?? 0,
                Price = price ?? 0,
                categoryId = catid,
                image = studentImagePath

            };
            context.Proudects.Add(x);
            context.SaveChanges();
            return RedirectToAction("AddProductBtn");
        }

        //all categories
        public IActionResult AllCategories(int? page, string? query, string? category)
        {
            var categories = context.Categories.ToList();

            if (!string.IsNullOrEmpty(category) /*|| category != "All"*/)
            {
                categories = categories.Where(c => c.Name == category).ToList();
            }
            if (!string.IsNullOrEmpty(query))
            {
                categories = categories.Where(c => c.Name.ToLower().StartsWith(query.ToLower())).ToList();
            }
            ViewBag.catall = context.Categories.ToList();
            ViewBag.category = category;
            ViewBag.query = query;
            if (categories.Count == 0)
            {
                List<Category> list = new List<Category>();
                return View(list);
            }
            var pageSize = 10;
            var totalStudents = categories.Count();

            var totalPages = (int)Math.Ceiling((double)totalStudents / pageSize);

            page = Math.Max(page ?? 1, 1);
            page = Math.Min(page ?? 1, totalPages);

            ViewBag.CurrentPage = page;

            var students = categories
                .Skip(((int)page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = totalPages;
            return View(students);
        }
        [HttpPost]
        public IActionResult AddCategories(string? name, IFormFile? image)
        {
            string studentImagePath = null;
            if (image is not null)
            {
                studentImagePath = DocumentSettings.UploadFile(image, "StudentPhoto", name);
            }
            var x = new Category
            {
                Name = name,
                Image = studentImagePath

            };
            context.Categories.Add(x);
            context.SaveChanges();
            return RedirectToAction("AllCategories");
        }
        [HttpPost]
        public IActionResult search(string? search, string? cat)
        {

            return RedirectToAction("AllCategories", new { query = search, category = cat });
        }
        [HttpPost]
        public IActionResult searchcat(string? search, string? cat, string? price)
        {

            return RedirectToAction("AllProduct", new { query = search, category = cat, pricecat = price });
        }

        //Orders
        public async Task<IActionResult> allOrders(int? page)
        {
            List<Orders> orders = await context.Orders.Include(c => c.user)
                .OrderByDescending(c => c.Id).ToListAsync();
            if (orders.Count == 0)
            {
                List<Orders> list = new List<Orders>();
                return View(list);

            }
            var pageSize = 10;
            var totalStudents = orders.Count();

            var totalPages = (int)Math.Ceiling((double)totalStudents / pageSize);

            page = Math.Max(page ?? 1, 1);
            page = Math.Min(page ?? 1, totalPages);

            ViewBag.CurrentPage = page;

            var students = orders
                .Skip(((int)page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = totalPages;
            return View(students);
        }
        public async Task<IActionResult> alloffers(int? page)
        {
            List<Offer> Offer = await context.Offers.Include(c => c.proudect)
                .OrderByDescending(c => c.Id).ToListAsync();
            if (Offer.Count == 0)
            {
                List<Offer> list = new List<Offer>();
                return View(list);

            }
            var pageSize = 10;
            var totalStudents = Offer.Count();

            var totalPages = (int)Math.Ceiling((double)totalStudents / pageSize);

            page = Math.Max(page ?? 1, 1);
            page = Math.Min(page ?? 1, totalPages);

            ViewBag.CurrentPage = page;

            var students = Offer
                .Skip(((int)page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = totalPages;
            return View(students);
        }
        public IActionResult refeuse(int id)
        {

            var order = context.orderprodects.Include(c => c.proudect).Where(c => c.orderId == id).ToList();

            foreach (var c in order)
            {
                c.proudect.count++;
                context.orderprodects.Remove(c);
                context.SaveChanges();
            }
            var ordere = context.Orders.Where(c => c.Id == id).FirstOrDefault();

            context.Orders.Remove(ordere);
            context.SaveChanges();

            return RedirectToAction("allOrders");
        }

        //Orders
        public IActionResult profileSetting()
        {
            var user = context.userpermations.Where(c => c.Id == GetLoggedInUserId()).FirstOrDefault();

            return View(user);
        }

        [HttpGet]
        public IActionResult deleteprodect(int id)
        {
            var user = context.Proudects.FirstOrDefault(p => p.Id == id);

            if (user != null)
            {
                //if (user.PersonalPhoto is not null)
                //    DocumentSettings.DeleteFile(user.PersonalPhoto, "student");

                context.Proudects.Remove(user);
                context.SaveChanges();
            }
            return RedirectToAction("AllProduct");
        }
        [HttpGet]
        public IActionResult deleteprodecte(int id)
        {
            var user = context.Proudects.FirstOrDefault(p => p.Id == id);

            if (user != null)
            {
                //if (user.PersonalPhoto is not null)
                //    DocumentSettings.DeleteFile(user.PersonalPhoto, "student");

                context.Proudects.Remove(user);
                context.SaveChanges();
            }
            return RedirectToAction("dashHome");
        }
        [HttpGet]
        public IActionResult deletecat(int id)
        {
            var user = context.Categories.FirstOrDefault(p => p.Id == id);

            if (user != null)
            {
                //if (user.PersonalPhoto is not null)
                //    DocumentSettings.DeleteFile(user.PersonalPhoto, "student");

                context.Categories.Remove(user);
                context.SaveChanges();
            }
            return RedirectToAction("AllCategories");
        }
        [HttpGet]
        public IActionResult deleteoffer(int id)
        {

            var user = context.Offers.FirstOrDefault(p => p.Id == id);

            if (user != null)
            {
                //if (user.PersonalPhoto is not null)
                //    DocumentSettings.DeleteFile(user.PersonalPhoto, "student");

                context.Offers.Remove(user);
                context.SaveChanges();
            }
            return RedirectToAction("alloffers");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editprofile(string? fname, string? lname, string? Email, string? PhoneNumber, int id, string? Address, string? Password)
        {
            userpermations x = context.userpermations.Where(c => c.Id == GetLoggedInUserId()).FirstOrDefault();
            x.Email = Email ?? x.Email;
            x.Address = Address ?? x.Address;
            x.PhoneNumber = PhoneNumber ?? x.PhoneNumber;
            x.fname = fname ?? x.fname;
            x.lname = lname ?? x.lname;
            x.Password = CommonMethods.ConvertToEncrypt(Password) ?? x.Password;

            x.Username = fname ?? x.fname + " " + lname ?? x.lname;

            context.SaveChanges();
            return RedirectToAction(nameof(profileSetting));
        }

        public IActionResult editProduct(int id)
        {
            var pro = context.Proudects.Include(c => c.category).FirstOrDefault(c => c.Id == id);
            ViewBag.cat = context.Categories.ToList();
            return View(pro);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult editProduct(string? Name, string? Description, int? Price, int? count, int id, IFormFile? image, string? category)
        {
            //IFormFile? imagee = image;
            string studentImagePath = null;
            if (image is not null)
            {
                studentImagePath = DocumentSettings.UploadFile(image, "StudentPhoto", Name);
            }
            var pro = context.Proudects.Include(c => c.category).FirstOrDefault(c => c.Id == id);
            var catid = context.Categories.Where(c => c.Name == category).Select(c => c.Id).FirstOrDefault();
            pro.Name = Name ?? pro.Name;
            pro.Description = Description ?? pro.Description;
            pro.Price = Price ?? pro.Price;
            pro.count = count ?? pro.count;
            pro.image = studentImagePath ?? pro.image;
            pro.category.Name = category ?? pro.category.Name;

            context.SaveChanges();
            return RedirectToAction(nameof(AllProduct));
        }

        
        public IActionResult editcat(int id)
        {
            var pro = context.Categories.FirstOrDefault(c => c.Id == id);
            return View(pro);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult editcat(string? Name, int id, IFormFile? image)
        {
            //IFormFile? imagee = image;
            string studentImagePath = null;
            if (image is not null)
            {
                studentImagePath = DocumentSettings.UploadFile(image, "StudentPhoto", Name);
            }
            var pro = context.Categories.FirstOrDefault(c => c.Id == id);
            pro.Name = Name ?? pro.Name;

            pro.Image = studentImagePath ?? pro.Image;

            context.SaveChanges();
            return RedirectToAction(nameof(AllCategories));
        }



        public IActionResult offer(int id)
        {
            var pro = context.Proudects.Include(c => c.category).FirstOrDefault(c => c.Id == id);
            return View(pro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult offer(int rate, int id)
        {
            var pro = context.Proudects.Include(c => c.category).FirstOrDefault(c => c.Id == id);

            var c = new Offer
            {
                presentage = rate,
                proudectId = id,
            };
            context.Offers.Add(c);
            context.SaveChanges();
            return RedirectToAction(nameof(AllProduct));
        }

        [HttpGet]
        public IActionResult states(int id)
        {
            var user = context.Orders.FirstOrDefault(p => p.Id == id);

            user.states = !user.states;
            context.SaveChanges();

            return RedirectToAction("allOrders");
        }

        public IActionResult editoffer(int id)
        {
            var pro = context.Offers.Include(c => c.proudect).FirstOrDefault(c => c.Id == id);
            return View(pro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editoffer(int rate, int id)
        {
            var pro = context.Offers.Include(c => c.proudect).FirstOrDefault(c => c.Id == id);
            pro.presentage = rate;
            pro.proudectId = id;
            context.SaveChanges();
            return RedirectToAction(nameof(alloffers));
        }

        public IActionResult GetImage(string fileName)
        {
            // المسار الكامل للصورة على الخادم
            var filePath = Path.Combine("D:\\MEDIA\\StudentPhoto", fileName);

            // تحقق مما إذا كان الملف موجودًا
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // قراءة الملف كـ byte array
            var image = System.IO.File.ReadAllBytes(filePath);

            // إرجاع الصورة كنتيجة
            return File(image, "image/jpeg"); // أو حسب نوع الصورة "image/png" مثلا
        }
    }
}
