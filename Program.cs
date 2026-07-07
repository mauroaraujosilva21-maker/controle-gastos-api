using Microsoft.EntityFrameworkCore;
using ControleGastos.Data;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. CONFIGURAÇÃO DO CORS (LIBERADO PARA TUDO NO DEPLOY)
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.AllowAnyOrigin() // Permite que o seu React na nuvem consiga acessar aqui
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Configuração do Swagger (Adicionando os serviços necessários)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do Banco de Dados PostgreSQL (Supabase)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// ==========================================
// 2. ATIVAÇÃO DO SWAGGER (SOLTO PARA FUNCIONAR NA RENDER)
// ==========================================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ControleGastos API v1");
    c.RoutePrefix = "swagger"; // Força a rota a ser exatamente /swagger
});

// Ativa a permissão antes dos outros mapeamentos
app.UseCors("AllowReact"); 

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ControleGastos.Data.AppDbContext>();
    dbContext.Database.EnsureCreated(); // Cria um banco 100% novo com Pessoas e Transacoes
}

app.Run();