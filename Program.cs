using TodoApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddCors(o => o.AddPolicy("MyPolicy", policy =>
        {
            policy.WithOrigins("http://localhost:3000").AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        }));

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ToDo API",
        Description = "An ASP.NET Core Web API for managing ToDo items"


    });
});
builder.Services.AddDbContext<ToDoDbContext>();


var app = builder.Build();
app.UseCors("MyPolicy");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
});


app.MapGet("/", () => "Hello World!");
app.MapGet("/items", async (ToDoDbContext context) =>
 {
     var x =  context.Items.ToList();
     return x;
 });

app.MapPost("/items", async (Item item, ToDoDbContext context) =>
{
    context.Add(item);
    await context.SaveChangesAsync();
    return item;
});

app.MapPut("/items/{id}", async (ToDoDbContext context, [FromBody] Item item, int id) =>
{
    var existItem = await context.Items.FindAsync(id);
    if (existItem is null) return Results.NotFound();

    // existItem.Name = item.Name;
    existItem.Iscomplete = item.Iscomplete;

    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/items/{id}", async (ToDoDbContext context, int id) =>
{
    var existItem = await context.Items.FindAsync(id);
    if (existItem is null) return Results.NotFound();

    context.Items.Remove(existItem);
    await context.SaveChangesAsync();

    return Results.NoContent();
});



app.Run();

