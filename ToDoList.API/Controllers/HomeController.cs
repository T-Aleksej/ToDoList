using Microsoft.AspNetCore.Mvc;

namespace ToDoList.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => new RedirectResult("~/swagger");
    }
}