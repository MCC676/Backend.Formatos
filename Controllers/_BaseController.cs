using BackendFormatos.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendFormatos.Controllers
{
    [Authorize]
    public class _BaseController : ControllerBase
    {
        protected readonly DbFormatoContext _context = null!;
        public _BaseController(DbFormatoContext context)
        {
            _context = context;
        }
    }
}
