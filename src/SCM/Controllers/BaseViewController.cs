using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace SCM.Controllers
{
    public abstract class BaseViewController : Controller
    {
        public ActionResult PageNotFound()
        {
            return NotFound();
        }

        internal string FormatAsHtmlList(string str, char[] separator)
        {
            if (str.Length > 0)
            {
                var splitMessage = str.Trim(separator).Split(separator).ToList();
                var list = string.Concat(splitMessage.Select(q => $"<li>{q}</li>"));

                return $"<ul>{list}</ul>";
            }
            else
            {
                return null;
            }
        }

        internal string FormatAsHtmlList(string str)
        {
            char[] separator;
            if (str.StartsWith("\\"))
            {
                str.TrimStart(new char[] { '\\', '\\' });
                separator = new char[] { '\\', '\\' };
            }
            else {
                separator = new char[] { '.' };
            }

            return FormatAsHtmlList(str, separator);
        }
    }
}