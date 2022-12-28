using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MEDIOClinic.Controllers
{
    public class HttpErrorsController : Controller
    {
        public IActionResult Error(int code)
        {
            if (code == 404)
            {
                return View("NotFound");
            }

            return StatusCode(code);
        }
    }
}
