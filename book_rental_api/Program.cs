using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using book_rental_api.Data.book_rental_db;
using book_rental_api.Helpers;
using book_rental_api.Services;
using Hangfire;
using Hangfire.Common;
using System.ComponentModel.DataAnnotations;
using book_rental_api.Exceptions;

var builder = WebApplication.CreateBuilder(args);

var configValue = builder.Configuration.GetValue<string>("my_key");

builder.Services.AddHttpContextAccessor();


builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            // Create a dictionary without null entries
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    e => e.Key,
                    e => e.Value?.Errors.Select(err => err.ErrorMessage).ToArray() ?? []);

            // Throwing a custom exception instead of returning a BadRequest response
            throw new ModelValidationException(errors);
        };
    });

builder.Services.AddControllersWithViews();

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "My Api", Version = "v1" });
    opt.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Description = "Standard authorization header using the bearer scheme, e.g., \"Bearer: {token}\"",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Scheme = "bearer"
    });
    opt.OperationFilter<AuthenticationRequirementsOperationFilter>();
});

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:4200")
                     .AllowAnyHeader()
                     .AllowAnyMethod();
    });
});

// Authentication configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.IncludeErrorDetails = true;
    options.UseSecurityTokenValidators = true;
    options.TokenValidationParameters = new TokenValidationParameters
    { 
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("appSettings:token").Value!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,  
    };
});

// Database context configuration
builder.Services.AddDbContext<BookRentalDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("BRDBConnection")));


// Add Hangfire services
builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("BRDBConnection")));

builder.Services.AddHangfireServer();

builder.Services.AddScoped<EmailService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddScoped<IRentalsService, RentalsService>();

var app = builder.Build();

// Middleware configuration
app.UseCors("AllowOrigin");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard();


RecurringJob.AddOrUpdate<IRentalsService>(
    "MarkOverdueRentalsJob",  
    service => service.ProcessOverdueRentals(),
    Cron.Hourly,
    new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.Local 
    }
);

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


 