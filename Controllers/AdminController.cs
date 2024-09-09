// Controllers/AdminController.cs
using System.Linq;
using System.Web.Mvc;
using UserManagement.Models;

namespace UserManagement.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Admin/
        public ActionResult Index()
        {
            var users = db.UserDetails.ToList();
            return View(users);
        }

        // GET: /Admin/Edit/5
        public ActionResult Edit(int id)
        {
            var user = db.UserDetails.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: /Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserDetail userDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userDetail).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userDetail);
        }

        // GET: /Admin/Details/5
        public ActionResult Details(int id)
        {
            var user = db.UserDetails.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
    }
}
