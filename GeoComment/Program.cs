using GeoComment.Data;
using GeoComment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<DatabaseHandler>();

var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<GeoCommentDbContext>(options =>
    options.UseSqlServer(connectionString));

#region Verison

builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(0, 1); //Behövs kanske inte
    o.AssumeDefaultVersionWhenUnspecified = true; //Behövs kanske inte
    o.ReportApiVersions = true; //Behövs förmodligen inte

    o.ApiVersionReader = new QueryStringApiVersionReader("api-version");
});

#endregion


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

using (var scope = app.Services.CreateAsyncScope())
{
    var databaseHandler = scope.ServiceProvider.GetRequiredService<DatabaseHandler>();

    await databaseHandler.RecreateDb();
}

app.Run();
