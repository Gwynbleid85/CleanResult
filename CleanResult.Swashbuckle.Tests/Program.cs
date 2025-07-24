using CleanResult.Swashbuckle.Tests;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddWolverineHttp();

builder.Host.AddProjects(["CleanResult.Swashbuckle.Tests"]);
builder.Services.AddSwaggerGen();
builder.Services.AddSwagger("title", ["CleanResult.Swashbuckle.Tests"]);


var app = builder.Build();
app.MapWolverineEndpoints();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();