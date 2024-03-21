using AngularAuthAPI.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(option => //3.make a service of DB connection.
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnStr"));//4. to make coonection with DB and add it to 
                                                                                       //appsetting.json file
});
builder.Services.AddCors(option => //13.This used when the CORS policy error is shown when two local Host are different
{
    option.AddPolicy("AllowPolicy", builder =>
    {
        builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); //This will allow any local host or headers or origin 
    });
});

builder.Services.AddAuthentication(x => ////--Token -3 Add Authentication for token
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false; // for not allowing aggresive checking
    x.SaveToken = true; // Save Token
    x.TokenValidationParameters = new TokenValidationParameters //Security key and audience any other things should match
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_with_at_least_256_bits")),
        ValidateAudience = false,
        ValidateIssuer = false,
        ClockSkew = TimeSpan.Zero //to match the exact expiring time 
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowPolicy");     //14. This should always be added above app.UseAuthorization(); pipeline

app.UseAuthentication();        //15. Use to give Authentication allow
app.UseAuthorization();

app.MapControllers();

app.Run();
