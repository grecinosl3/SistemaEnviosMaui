using Microsoft.EntityFrameworkCore;
using Struct_1_proyec.Data;
using Struct_1_proyec.Services;
using Struct_1_proyec.Components;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar componentes de Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 2. Configurar la conexión a SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Inyección de servicios logísticos
builder.Services.AddScoped<TrackingService>();
builder.Services.AddScoped<CotizadorService>();
//builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EnvioService>();

var app = builder.Build();

// 4. Inicializar Base de Datos y Datos de Prueba automáticos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
   // await Struct_1_proyec.Data.SeedData.InitializeAsync(services);
}

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
// creador de tablas DB
// dotnet ef migrations add CambioAPersistenciaSQL
// dotnet ef database update

//dotnet ef migrations add InitialCreate