using ImageUpload.Data;
using ImageUpload.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ImageUpload.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var Db = new ImageUploadDB(Properties.Settings.Default.ConStr);
            var IVM = new IndexViewModel
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                User=Db.GetByEmail(User.Identity.Name)
            };
            return View(IVM);
        }
        public ActionResult CreateAccount()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateAccount(User user, string password)
        {
            var Db = new ImageUploadDB(Properties.Settings.Default.ConStr);
            Db.AddUser(user, password);
            return RedirectToAction("Index");
        }
        public ActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Upload(Image image, HttpPostedFileBase imageFile)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            imageFile.SaveAs(Server.MapPath("~/UploadedImages/") + fileName);
            image.FileName = fileName;
            var Db = new ImageUploadDB(Properties.Settings.Default.ConStr);
            string email = User.Identity.Name;
            User user = Db.GetByEmail(email);
            image.UserId = user.Id;
            Db.Add(image);
            return View(image);
        }

        public ActionResult ViewImage(int id)
        {
            var IVM = new ImageViewModel();

            if (TempData["message"] != null)
            {
                IVM.Message = (string)TempData["message"];
            }
            if (!HasPermission(id))
            {
                IVM.HasPermission = false;
                IVM.Image = new Image { Id = id };
            }
            else
            {
                IVM.HasPermission = true;
                var Db = new ImageUploadDB(Properties.Settings.Default.ConStr);
                Db.UpdateViewCount(id);
                var image = Db.GetImagebyId(id);
                if (image == null)
                {
                    return RedirectToAction("Index");
                }
                IVM.Image = image;
            }

            return View(IVM);
        }
        private bool HasPermission(int id)
        {
            if (Session["allowedids"] == null)
            {
                return false;
            }

            var allowedIds = (List<int>)Session["allowedids"];
            return allowedIds.Contains(id);
        }

        [HttpPost]
        public ActionResult ViewImage(int id, string password)
        {
            var Db = new ImageUploadDB(Properties.Settings.Default.ConStr);
            var uploadedImages = Db.GetImagebyId(id);
            if (uploadedImages == null)
            {
                return RedirectToAction("Index");
            }

            if (password != uploadedImages.Password)
            {
                TempData["message"] = "Invalid password";
            }
            else
            {
                List<int> allowedIds;
                if (Session["allowedids"] == null)
                {
                    allowedIds = new List<int>();
                    Session["allowedids"] = allowedIds;
                }
                else
                {
                    allowedIds = (List<int>)Session["allowedids"];
                }

                allowedIds.Add(id);
            }

            return Redirect($"/home/ViewImage?id={id}");
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var Db = new ImageUploadDB(Properties.Settings.Default.ConStr);
            var user = Db.Login(email, password);
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            FormsAuthentication.SetAuthCookie(email, true);
            return RedirectToAction("Index");
        }
        public ActionResult ViewAllImages()
        {
            var Db = new ImageUploadDB(Properties.Settings.Default.ConStr);
            return View(Db.GetAllImages());
        }
        [Authorize]
        public ActionResult MyAccount()
        {     
            var Db = new ImageUploadDB(Properties.Settings.Default.ConStr);
            var user = Db.GetByEmail(User.Identity.Name);
            return View(Db.GetImagesByUserId(user.Id));
        }
        public ActionResult UploadImage()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            var Db = new ImageUploadDB(Properties.Settings.Default.ConStr);
            Db.DeleteImage(id);
            return Redirect("/Home/MyAccount");
        }
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/Home/Index");
        }
    }
}