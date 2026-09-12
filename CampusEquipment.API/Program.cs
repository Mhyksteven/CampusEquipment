using CampusEquipment.Core.Repositories;
using CampusEquipment.Core.Services;
using CampusEquipment.Infrastructure.Data;
using CampusEquipment.Infrastructure.Repositories;
using CampusEquipment.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));

// Repositories
builder.Services.AddScoped<
    IEquipmentRepository,
    EquipmentRepository>();

builder.Services.AddScoped<
    IDepartmentRepository,
    DepartmentRepository>();

// Services
builder.Services.AddScoped<
    IEquipmentService,
    EquipmentService>();

builder.Services.AddScoped<
    IDepartmentService,
    DepartmentService>();

var app = builder.Build();

// Swagger only during development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    Console.WriteLine("=================================");
    Console.WriteLine($"API SERVER: {db.Database.GetDbConnection().DataSource}");
    Console.WriteLine($"API DATABASE: {db.Database.GetDbConnection().Database}");
    Console.WriteLine("=================================");
}

app.Run();