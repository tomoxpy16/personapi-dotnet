using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.Repository;

var builder = WebApplication.CreateBuilder(args);

// MVC: controladores + vistas Razor (también sirve a los controladores REST con [ApiController]).
builder.Services.AddControllersWithViews();

// EF Core: el DbContext se registra con la cadena de conexión de appsettings.json.
builder.Services.AddDbContext<PersonaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PersonaDb")));

// Inyección de dependencias de los DAO: cada integrante registra aquí sus repositorios.
// Scoped = una instancia por petición HTTP, igual que el DbContext.
builder.Services.AddScoped<IPersonaRepository, PersonaRepository>();
builder.Services.AddScoped<ITelefonoRepository, TelefonoRepository>();

// Swagger (OpenAPI 3) para documentar y probar los endpoints REST.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "personapi-dotnet",
        Version = "v1",
        Description = "API REST para el CRUD de persona, profesion, estudios y telefono"
    });
    var xml = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    c.IncludeXmlComments(xml);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "personapi-dotnet v1"));

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Ruta convencional de MVC: /{controlador}/{acción}/{id?}
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
