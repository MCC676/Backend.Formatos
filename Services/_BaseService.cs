using BackendFormatos.Data;
using Microsoft.AspNetCore.Authorization;

namespace BackendFormatos.Services
{
    [Authorize]
    public class _BaseService
    {
        protected readonly DbFormatoContext _context = null!;
        public _BaseService(DbFormatoContext context) { _context = context; }
    }
}
