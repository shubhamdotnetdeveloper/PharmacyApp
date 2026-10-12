using PharmacyApp.Api.Data;
using PharmacyApp.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<PharmacyJsonStore>();
builder.Services.AddScoped<MedicineService>();
builder.Services.AddScoped<SaleService>();

// Swagger
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthorization();

app.MapControllers();

app.Run();
