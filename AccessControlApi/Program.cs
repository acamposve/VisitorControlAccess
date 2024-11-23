using AccessControlApi.Application.Services;
using AccessControlApi.Domain.ApplicationInterfaces;
using AccessControlApi.Domain.InfrastructureInterfaces;
using AccessControlApi.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Configurar la conexión a la base de datos
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAccessPointRepository, AccessPointRepository>();
builder.Services.AddScoped<IAccessPointService, AccessPointService>();

builder.Services.AddScoped<IBraceletRepository, BraceletRepository>();
builder.Services.AddScoped<IBraceletService, BraceletService>();

builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IVisitorRepository, VisitorRepository>();
builder.Services.AddScoped<IVisitorService, VisitorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
