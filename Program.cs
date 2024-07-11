
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PharmacyApi.Data;
using PharmacyApi.Services;

namespace PharmacyApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

             var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection", "DefaultConnection is not configured");

            var userConnection = builder.Configuration.GetConnectionString("UserConnection")
                ?? throw new ArgumentNullException("UserConnection", "UserConnection is not configured");

            builder.Services.AddDbContext<DataBaseContext>(options => 
                    options.UseSqlServer(connectionString));

            builder.Services.AddDbContext<UserDataBaseContext>(options =>
                    options.UseSqlServer(userConnection));

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddScoped<JwtService>();

            var jwtkey = builder.Configuration["jwt:key"]
                ?? throw new ArgumentNullException("Jwt:Key", "Jwt:Key is not configured");

            if(jwtkey.Length < 32) {
                throw new ArgumentException("Jwt:Key must be at least 32 characters long.");
            }

            var jwtissuer = builder.Configuration["jwt:Issuer"]
                ?? throw new ArgumentNullException("Jwt:Issuer", "Jwt:Issuer is not configured");
            
            var jwtAudience = builder.Configuration["jwt:Audience"]
                ?? throw new ArgumentNullException("Jwt:Audience", "Jwt:Audience is not configured");

            var key = Encoding.UTF8.GetBytes(jwtkey);

            builder.Services.AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtissuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization(options => {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("User", policy => policy.RequireRole("User"));
            });

            builder.Services.AddCors(options => {
                options.AddPolicy("AllowSpecificOrigin",
                builder => builder.WithOrigins("http://localhost:5173")
                                  .AllowAnyHeader()
                                  .AllowAnyMethod()
                                  .AllowCredentials());
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseCors("AllowSpecificOrigin");

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
        }

        public static void CreationDefaultAdmin() {

        }
    }
}
