using Microsoft.EntityFrameworkCore;
using ControleGastos.Data;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. CONFIGURAÇÃO DO CORS
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVercel", policy =>
    {
        policy.WithOrigins("https://frontend-gamma-one-11.vercel.app") // URL exata da Vercel
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configuração dos Controllers com as opções de JSON ajustadas (camelCase)
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

// Configuração do Banco de Dados PostgreSQL (Supabase)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// ATENÇÃO: UseCors DEVE vir obrigatoriamente ANTES de UseAuthorization e MapControllers
app.UseCors("AllowVercel");

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated(); 
}
app.Run();