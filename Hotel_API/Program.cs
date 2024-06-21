using Application;
using Data;
using Domain.Entites;
using Domain.ViewModels;
using Hotel_API.Services;
using Hotel_Management;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(option =>
{
    option.ReturnHttpNotAcceptable = true;
})
    .AddXmlDataContractSerializerFormatters()
    .AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Registor AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


// Register Serilog
Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Error()
                 .WriteTo.Console()
                 .WriteTo.File("C:\\Users\\User\\OneDrive\\Desktop\\1000 Programming\\ASP.NET\\API\\Homework_API", rollingInterval: RollingInterval.Day)
                 .CreateLogger();


builder.Services.AddDbContext<HotelContext>(
                option => option.UseSqlServer(builder.Configuration["ConnectionStrings:HotelDbConnectionStrings"]));


//Registor Services
builder.Services.AddScoped<IRepository<Hotel, HotelForUpdate>, RepositoryHotel>();
builder.Services.AddScoped<IRepository<Employee, EmployeeForUpdate>, RepositoryEmployee>();
builder.Services.AddScoped<IRepository<RoomType, RoomTypeForUpdate>, RepositoryRoomType>();
builder.Services.AddScoped<IRepository<Room, RoomForUpdate>, RepositoryRoom>();
builder.Services.AddScoped<IRepository<Guest, GuestForUpdate>, RepositoryGuest>();
builder.Services.AddScoped<IRepository<Booking, BookingForUpdate>, RepositoryBooking>();
builder.Services.AddScoped<IRepository<Payment, PaymentForUpdate>, RepositoryPayment>();



// إضافة Policy
builder.Services.AddAuthorization(option => {
    option.AddPolicy("userShouldeBeUserAndFromCoures11", policy => {
        policy.RequireRole("User");
        policy.RequireClaim("course", "Midad-11");
    });
});


//Decrypt the token
builder.Services.AddAuthentication().AddJwtBearer(options =>
            options.TokenValidationParameters = new()
            {
                ValidIssuer = builder.Configuration["Authentication:Issuer"],
                ValidAudience = builder.Configuration["Authentication:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretKey"])),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
            });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
