using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PriceMonitorAPI.Dados;
using PriceMonitorAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Carregar configurações do appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Adicionar Entity Framework
builder.Services.AddDbContext<PriceMonitorContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<NotificationService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
app.UseDeveloperExceptionPage();
