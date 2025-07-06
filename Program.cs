using BackendFormatos.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string AllowSpecificOrigins = "_AllowSpecificOrigins";
string[] cors = builder.Configuration["Cors"].Split(";");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowSpecificOrigins,
                      builder => builder
                      .WithOrigins(cors)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .SetIsOriginAllowed((x) => true)
                      .AllowCredentials()
                     );
});

builder.Services.AddDbContext<DbFormatoContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("ProveedorFormato")));



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
