using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace NguyenMinhNguyen_Assignment2.Extensions;

public static class IdentityServiceExtension
{

    const string ADMIN_ID = "0";
    const string STAFF_ID = "1";
    const string LECTURER_ID = "2";
    public static IServiceCollection IdentityServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"])),
                ValidateIssuer = false,
                ValidateAudience = false
                
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy =>
                      policy.RequireClaim("AccountRole", ADMIN_ID));
            options.AddPolicy("Staff", policy =>
                      policy.RequireClaim("AccountRole", STAFF_ID));
            options.AddPolicy("Lecturer", policy =>
                      policy.RequireClaim("AccountRole", LECTURER_ID));
        });
        return services;
    }
}