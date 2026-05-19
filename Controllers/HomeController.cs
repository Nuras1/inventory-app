using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_itransition.Models;
using System.Diagnostics;
using System.Globalization;

namespace project_itransition.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult ChangeLanguage(string lang)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                lang = "en";
            }
            Response.Cookies.Append("Language", lang);
            return Redirect(Request.GetTypedHeaders().Referer.ToString());
        }
    }
}
