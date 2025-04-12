using MessengerBackend.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using Microsoft.EntityFrameworkCore.Design;
using MessengerBackend.Repositories;

namespace MessengerBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var walletPath = Path.Combine(Directory.GetCurrentDirectory(), "OracleWallet");
            OracleConfiguration.TnsAdmin = walletPath;
            OracleConfiguration.WalletLocation = walletPath;

            string conString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseOracle(conString)
            );

            builder.Services.AddCors(options => options.AddPolicy("MyCors", builder =>
            {
                builder.WithOrigins("http://localhost:4200").
                AllowAnyMethod().AllowAnyHeader();
            })
);

            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                    c.RoutePrefix = string.Empty;

                });
            }
            app.UseCors("MyCors");
            app.MapControllers();

            app.Run();
        }
    }
}
