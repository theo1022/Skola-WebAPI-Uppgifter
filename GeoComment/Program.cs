using GeoComment.Data;
using GeoComment.Models;
using GeoComment.Services;
using GeoComment.Swagger;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<DatabaseHandler>();

#region Database connection
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<GeoCommentDbContext>(options =>
    options.UseSqlServer(connectionString));
#endregion

#region Authentication
builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(
        "BasicAuthentication", null);
#endregion

#region Identity
builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<GeoCommentDbContext>();
#endregion

#region Versioning
builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(0, 1);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;

    o.ApiVersionReader = new QueryStringApiVersionReader("api-version");
});
#endregion

#region Swagger Versioning Automatic
builder.Services.AddTransient<SwaggerGenOptions>();
builder.Services.AddVersionedApiExplorer(o =>
{
    o.GroupNameFormat = "'v'VVV";
});

builder.Services
    .AddTransient<IConfigureOptions<SwaggerGenOptions>,
        ConfigureSwaggerGenOptions>();


builder.Services.AddSwaggerGen(o =>
{
    o.OperationFilter<AddApiVersionExampleValueOperationFilter>();
    o.OperationFilter<SecurityRequirementsOperationFilter>();

    o.AddSecurityDefinition("BasicAuth", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Authorization header using the Basic scheme."
    });
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint($"/swagger/v0.1/swagger.json", "v0.1");
        o.SwaggerEndpoint("/swagger/v0.2/swagger.json", "v0.2");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateAsyncScope())
{
    var databaseHandler = scope.ServiceProvider.GetRequiredService<DatabaseHandler>();

    await databaseHandler.RecreateDb();
}

app.Run();
