using EnterpriseAppLINQ.Data;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();

// Configuración de la base de datos con MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("LinqExampleConnection"), 
        new MySqlServerVersion(new Version(8, 0, 21))));  // Cambia la versión a la que estés usando

// Agregar servicios necesarios para Razor Pages y controladores
builder.Services.AddRazorPages();

// Habilitar Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations(); // Habilitar anotaciones si usas SwaggerOperation en los controladores
});

// Si necesitas CORS, puedes habilitarlo de la siguiente manera:
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // El valor predeterminado de HSTS es 30 días. Puedes querer cambiar esto para escenarios de producción.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Aplicar la política de CORS si es necesario
app.UseCors("AllowAll");

app.UseAuthorization();

// Habilitar Swagger en la aplicación
app.UseSwagger(); // Activa Swagger
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    c.RoutePrefix = string.Empty;  // Para que Swagger UI esté disponible en la raíz
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Agregar esta línea si estás usando Razor Pages

app.Run();