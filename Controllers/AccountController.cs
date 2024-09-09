using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using UserManagement.Models;

namespace UserManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Account/Login
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = db.UserDetails.Include(u => u.PasswordDetails)
                                 .FirstOrDefault(u => u.EmailAddress.ToLower() == model.Email.ToLower().Trim()); 

                if (user != null && user.PasswordDetails == model.PasswordDetails)
                {
                    if (user.IsActivated)
                    {
                        FormsAuthentication.SetAuthCookie(user.EmailAddress, model.RememberMe);
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Your account is not activated.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }

            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var temp = db.UserDetails.ToList();
                var user = new UserDetail
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DOB = model.DOB,
                    Gender = model.Gender,
                    EmailAddress = model.Email,
                    IsActivated = true,
                    PasswordDetails = model.PasswordDetails
                };
                db.UserDetails.Add(user);
                
                db.SaveChanges();

                // Send email with activation link
                string token = Guid.NewGuid().ToString();
                string activationLink = Url.Action("ActivateAccount", "Account", new { userId = user.Id, token = token }, Request.Url.Scheme);

                // Optionally send email here
                // SendEmail(user.EmailAddress, "Activate your account", activationLink);

                TempData["Message"] = "Registration successful! Please check your email to activate your account.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        // GET: /Account/ActivateAccount
        [HttpGet]
        public ActionResult ActivateAccount(int userId, string token)
        {
            var user = db.UserDetails.Find(userId);
            if (user != null && !user.IsActivated)
            {
                // Verify the token if required (in production scenarios)
                user.IsActivated = true;
                db.SaveChanges();
                return RedirectToAction("CreatePassword", new { userId = user.Id });
            }

            return HttpNotFound();
        }

        // GET: /Account/CreatePassword
        [HttpGet]
        public ActionResult CreatePassword(int userId)
        {
            ViewBag.UserId = userId;
            return View();
        }

        // POST: /Account/CreatePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePassword(CreatePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.UserDetails.Find(model.UserId);
                if (user != null && user.IsActivated)
                {
                    user.PasswordDetails = new PasswordDetail { UserID = user.Id, Password = model.Password };
                    db.SaveChanges();
                    TempData["Message"] = "Password created successfully! You can now login.";
                    return RedirectToAction("Login");
                }
            }

            return View(model);
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        public ActionResult ResetPassword(int userId, string token)
        {
            var model = new ResetPasswordViewModel
            {
                UserId = userId,
                Token = token
            };
            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.UserDetails.Find(model.UserId);
                if (user != null)
                {
                    // Here you would verify the token and reset the password
                    user.PasswordDetails = model.Password; // Hash this in a real application
                    db.SaveChanges();

                    TempData["Message"] = "Password has been reset successfully!";
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("", "Error resetting password.");
            }
            return View(model);
        }


        // GET: /Account/ForgotPassword
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.UserDetails.FirstOrDefault(u => u.EmailAddress == model.Email);
                if (user != null)
                {
                    // Generate password reset token
                    string resetToken = Guid.NewGuid().ToString();

                    // Create a reset link
                    string resetLink = Url.Action("ResetPassword", "Account", new { userId = user.Id, token = resetToken }, Request.Url.Scheme);

                    // TODO: Store token in DB (create a field for reset tokens) or cache

                    // Send the reset link via email
                    // SendEmail(user.EmailAddress, "Password Reset", $"Please reset your password by clicking on this link: {resetLink}");

                    TempData["Message"] = "Please check your email for the password reset link.";
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", "Email address not found.");
            }

            return View(model);
        }


        // GET: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
