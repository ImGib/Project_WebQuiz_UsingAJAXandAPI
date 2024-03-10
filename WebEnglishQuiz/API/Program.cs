using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<QuizAPIContext>(option =>
            option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddCors();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            app.UseCors
                (policy =>
                policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}