using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1;
using WebApplication1.API.Dashboard;
using WebApplication1.API.Login;
using WebApplication1.API.Logout;
using WebApplication1.API.Register;
using WebApplication1.API.Splash;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<EmployeeContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
     p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.Configure<EmailcredModel>(
    builder.Configuration.GetSection("EmailCredentials"));

builder.Services.AddScoped<ITokenValidator, TokenValidator>();
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<RegisterApi>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Web";
});
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();
// Redis + JWT Services

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Clean endpoints
app.MapPost("/Register", async ([FromBody] RegisterModel regm, EmployeeContext db, ITokenValidator tokenValidator, IJwtGenerator jwtGenerator, RegisterApi regapi) =>
{
    try
    {
        var principal = tokenValidator.Validate(regm.Token);
        if (principal == null)
            return Results.Unauthorized();
        var result = await regapi.RegisterAsync(regm, db, jwtGenerator);
        if (result.status == "Success") return Results.Ok(result);
        else if (result.status == "UnAuthorized") return Results.Unauthorized();
        else return Results.BadRequest(result);

    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
}).WithTags("UserRegistration");

app.MapGet("/splash", async (EmployeeContext db, IDistributedCache cache) =>
{
    try
    {
        SplashApi splashApi = new SplashApi();
        var resp = await splashApi.SplashAsync(db, cache);
        if (resp.status == "success") return Results.Ok(resp);
        else return Results.BadRequest(resp);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
}).WithTags("Splash");


app.MapPost("/login", async ([FromBody] LoginModel model, EmployeeContext db, ITokenValidator tokenValidator, IJwtGenerator jwtGenerator) =>

{
    try
    {
        var principal = tokenValidator.Validate(model.Token);
        if (principal == null)
            return Results.Unauthorized();
        LoginApi logapi = new LoginApi();
        var result = await logapi.LoginAsync(model, db, jwtGenerator);
        if (result.status == "Success") return Results.Ok(result);
        else if (result.status == "UnAuthorized") return Results.Unauthorized();
        else return Results.BadRequest(result);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
}).WithTags("User");


app.MapPost("/foodlist", async ([FromBody] LoginModel model, EmployeeContext db, ITokenValidator tokenValidator, IJwtGenerator jwtGenerator) =>

{
    try
    {
        var principal = tokenValidator.Validate(model.Token);
        if (principal == null)
            return Results.Unauthorized();
        LoginApi logapi = new LoginApi();
        var result = await logapi.LoginAsync(model, db, jwtGenerator);
        if (result.status == "Success") return Results.Ok(result);
        else if (result.status == "UnAuthorized") return Results.Unauthorized();
        else return Results.BadRequest(result);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
}).WithTags("User");


app.MapGet("/dashboard", async (long userid, string token, EmployeeContext db, ITokenValidator tokenValidator, IJwtGenerator _jwtGenerator) =>
{
    try
    {
        var principal = tokenValidator.Validate(token);
        if (principal == null)
            return Results.Unauthorized();
        DashboardAPI dashboardapi = new DashboardAPI(_jwtGenerator);
        var result = await dashboardapi.DashboardAsync(userid, token, db);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }
    catch (Exception ex)
    {

        return Results.BadRequest(ex.Message);

    }

}).WithTags("dashboard");

app.Run();
