using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FallbackController : Controller
    {
        // this method is to tell the api what to do when it do not know what to do with client routes
        // in startup.cs we tell the api to fall back to this controller when it needs to.
        public ActionResult Index() 
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "index.html"),"text/HTML");
        }
    }
}