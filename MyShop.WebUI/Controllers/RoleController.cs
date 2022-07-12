using MyShop.WebUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.Models;
using MyShop.Core.Supports;

namespace MyShop.WebUI.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class RoleController : Controller
    {

        ApplicationDbContext context;
        public RoleController()
        {
            context = new ApplicationDbContext();
        }

        // GET: Roles
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!isAdminUser())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            var roles = context.Roles.ToList();
            return View(roles);
        }


        /// Create  a New role
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!isAdminUser())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            var Role = new IdentityRole();
            return View(Role);
        }

        /// <summary>
        /// Create a New Role
        /// </summary>
        /// <param name="Role"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(IdentityRole Role)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!isAdminUser())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            context.Roles.Add(Role);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Users
        public ActionResult UserRoles()
        {


            if (User.Identity.IsAuthenticated)
            {

                var userRolesModel = new List<UserRoleModel>();

                var lstUsers = context.Users.Where(x => x.EmailConfirmed == true).ToList();

                foreach (var user in lstUsers)
                {
                    foreach (var role in user.Roles)
                    {
                        var rol = context.Roles.Where(x => x.Id == role.RoleId).FirstOrDefault();
                        if (rol != null)
                        {
                            userRolesModel.Add(new UserRoleModel
                            {
                                User = user.UserName,
                                Role = rol.Name,
                                UserId = user.Id
                            });
                        }

                    }
                }
                return View(userRolesModel);


            }
            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        public ActionResult RoleAssignment()
        {
            var lstRole = context.Roles.ToList();
            ViewBag.RoleId = new SelectList(lstRole, "Name", "Name");

            var lstUsers = context.Users.ToList();
            ViewBag.UserId = new SelectList(lstUsers, "Id", "Email");



            return View();
        }


        [HttpPost]
        public ActionResult RoleAssignment(string userId, string role)
        {
            if (userId == null || role == null)
            {
                ViewBag.RoleError = "Invalid role assignment!";
                return View();
            }
            else
            {
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                var resiltX = UserManager.AddToRole(userId, role);
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public ActionResult Delete(string ID)
        {

            string msg = "";


            if (ID != null)
            {
                var objDel = context.Users.SingleOrDefault(o => o.Id == ID);
                if (objDel != null)
                {
                    context.Users.Remove(objDel);
                    context.SaveChanges();


                    msg = "1";
                }
                else
                {
                    msg = "0";
                }
            }
            return RedirectToAction("UserRoles");
        }

        public static IdentityResult AddToRole(string userId, string role)
        {

            return null;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult RoleRequest()
        {
            var rol = context.Roles.ToList();
            if (rol != null)
            {
                ViewBag.Email = User.Identity.Name;
                ViewBag.Roles = rol;
                return View();
            }
            TempData["Msg"] = "EMPTY ROLES";
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult RoleRequest(string Email, string Name)
        {
            var _user = context.Users.Where(x => x.Email == Email).FirstOrDefault();

            //GET SUPERADMIN EMAIL
            var role = context.Roles.Where(x => x.Name == "SuperAdmin").FirstOrDefault();
            if (role != null)
            {
                string superAdminEmailAddress = "";
                var users = context.Users.ToList();
                foreach (var user in users)
                {
                    foreach (var rol in user.Roles)
                    {
                        if (rol.RoleId == role.Id)
                        {
                            superAdminEmailAddress = user.Email;
                        }
                    }
                }

                if (superAdminEmailAddress != null && superAdminEmailAddress != "")
                {
                    //PROCESS EMAIL
                    //SEND EMAIL TO CUSTOMER

                    IdentityMessage custidentyMessage = new IdentityMessage()
                    {
                        Destination = superAdminEmailAddress,
                        Subject = "Role Request"
                    };

                    string html = "Please Admin, I am requesting to be added to " + Name + " role\b, Email: " + Email +
                        " with \bUserID: " + _user.Id + ".";

                    MailHelper sendMail = new MailHelper();
                    ConfirmEmailSend sendMsg = sendMail.SendMail(custidentyMessage, html);
                    TempData["Msg"] = "Your request was successful!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Msg"] = "NO SUPER ADMIN ACCOUNT AVAILABLE!";
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["Msg"] = "NO SUPER ADMIN ROLE!";
                return RedirectToAction("Index", "Home");
            }
        }




        public bool isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                // ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                var roles = UserManager.GetRoles(user.GetUserId());
                foreach (var role in roles)
                {
                    if (role.ToString() == "SuperAdmin")
                    {
                        return true;
                    }
                }

            }
            return false;
        }
    }
}