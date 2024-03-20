using BaiTapLonDuAnMau.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace BaiTapLonDuAnMau.Controllers
{
    public class BaseController:Controller
    {
  
        public string CurrentUser
        {
            get
            {
                return HttpContext.Session.GetString("USER_NAME");
            }
            set
            {
                HttpContext.Session.SetString("USER_NAME", value);
            }
        }
      
        public bool IsLogin
        {

            get
            {
                return !string.IsNullOrEmpty(CurrentUser);
            }
 
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var isAdmin = HttpContext.Session.GetString("IsAdmin");
            var isManager = HttpContext.Session.GetString("IsManager");
            var isEmployee = HttpContext.Session.GetString("IsEmployee");
            ViewBag.IsAdmin = isAdmin;
            ViewBag.IsManager = isManager;
            ViewBag.IsEmployee = isEmployee;
            ViewBag.UserName = CurrentUser;
            
        }

    }
}
