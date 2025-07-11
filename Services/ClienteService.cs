﻿using BackendFormatos.Data;
using BackendFormatos.Models;
using BackendFormatos.Models.ContentResponse;
using Microsoft.EntityFrameworkCore;

namespace BackendFormatos.Services
{
    public class ClienteService : _BaseService, IClienteService
    {
        public ClienteService(DbFormatoContext context) : base(context) { }

        public async Task<ClienteDto> GetByIdAsync(int id)
        {
            var entity = await _context.Clientes.FindAsync(id);
            if (entity == null) throw new Exception("Cliente no encontrado");

            return new ClienteDto
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Ruc = entity.Ruc,
            };
        }

        public async Task<IEnumerable<Clientes>> ObtenerClientesAsync()
        {
            return await _context.Clientes
            .Where(c => c.Estado == true)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
        }
    }
}
