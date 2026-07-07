using Microsoft.EntityFrameworkCore;
using ControleGastos.Data;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. CONFIGURAÇÃO DO CORS (LIBERAÇÃO PARA O REACT)
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // URL padrão do Vite/React
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Configuração do Banco de Dados PostgreSQL (Supabase)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// ==========================================
// 2. ATIVAÇÃO DO CORS
// ==========================================
app.UseCors("AllowReact"); // Ativa a permissão antes dos outros mapeamentos

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ControleGastos.Data.AppDbContext>();
    
    // ATENÇÃO: Adicione esta linha logo acima do EnsureCreated

    dbContext.Database.EnsureCreated(); // Cria um banco 100% novo com Pessoas e Transacoes
}

app.Run();