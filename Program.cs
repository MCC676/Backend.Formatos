using BackendFormatos.Data;
using BackendFormatos.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

string AllowSpecificOrigins = "_AllowSpecificOrigins";

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ? REGISTRA tus servicios ANTES de builder.Build()
builder.Services.AddScoped<IClienteService, ClienteService>();
// builder.Services.AddScoped<IAgenciaService, AgenciaService>();
// builder.Services.AddScoped<IExportadorService, ExportadorService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowSpecificOrigins,
        policy => policy.AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed(_ => true)
                        .AllowCredentials());
});

builder.Services.AddDbContext<DbFormatoContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("ProveedorFormato")));

// ? Ahora puedes construir la aplicación
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(AllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
