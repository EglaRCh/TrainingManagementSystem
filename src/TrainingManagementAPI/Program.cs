using Microsoft.EntityFrameworkCore;
using TrainingManagementAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// --- Configuración de servicios ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Configurar DbContext ---
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? "Server=localhost,1433;Database=TMSDB;User Id=sa;Password=Your_password123;TrustServerCertificate=True;";

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(conn));

var app = builder.Build();
Console.WriteLine($"🌍 Entorno activo: {app.Environment.EnvironmentName}");
Console.WriteLine($"🔗 Cadena de conexión: {conn}");


// --- Configuración del pipeline ---
if (app.Environment.IsDevelopment())
{
    // Entorno local
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Entorno Docker / Producción
    app.UseSwagger();       // Habilitar Swagger también en contenedor
    app.UseSwaggerUI();
    // app.UseHttpsRedirection(); // 🔸 Desactivado para evitar cierre sin certificado
}
app.MapControllers();

// --- Aplicar migraciones automáticamente ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();
        Console.WriteLine("✅ Migraciones aplicadas correctamente.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Error al aplicar migraciones: {ex.Message}");
    }
}

// --- Ejecutar aplicación ---
app.Run();
// Para pruebas locales, usar: dotnet run --launch-profile "Local"