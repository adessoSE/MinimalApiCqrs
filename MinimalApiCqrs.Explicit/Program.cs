using System.Reflection;
using MinimalApiCqrs.Core.Endpoints;
using MinimalApiCqrs.Explicit.Core.Mediator;
using MinimalApiCqrs.Simple.Todos;

var assemblies = new Assembly[] { typeof(Program).Assembly };

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.SetupCQRS(assemblies);
builder.Services.AddSingleton<ITodoRepository, TodoInMemoryRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapEndpoints(assemblies);

app.Run();
