using System.Reflection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// MVC: controladores + vistas Razor (también sirve a los controladores REST con [ApiController]).
builder.Services.AddControllersWithViews();

// Inyección de dependencias de los DAO: cada integrante registra aquí sus repositorios.
// Scoped = una instancia por petición HTTP, igual que el DbContext.

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
