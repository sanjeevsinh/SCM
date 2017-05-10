using Microsoft.AspNetCore.Mvc;

namespace SCM.Controllers
{
    public abstract class BaseViewController : Controller
    {
        public ActionResult PageNotFound()
        {
            return NotFound();
        }
    }
}