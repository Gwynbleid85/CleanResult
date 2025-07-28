using CleanResult.Swashbuckle.Tests;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddWolverineHttp();

builder.Host.AddProjects(["CleanResult.Swashbuckle.Tests"]);
builder.Services.AddSwagger("CleanResult Swagger Test", ["CleanResult.Swashbuckle.Tests"]);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapWolverineEndpoints();

app.Run();