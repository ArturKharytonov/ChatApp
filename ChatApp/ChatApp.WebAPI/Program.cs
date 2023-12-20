using System.Configuration;
using System.Security.Claims;
using System.Text;
using ChatApp.Application.Services.FileService;
using ChatApp.Application.Services.FileService.Interface;
using ChatApp.Application.Services.ISqlService;
using ChatApp.Application.Services.ISqlService.Interfaces;
using ChatApp.Application.Services.JwtHandler;
using ChatApp.Application.Services.JwtHandler.Interfaces;
using ChatApp.Application.Services.MessageService;
using ChatApp.Application.Services.MessageService.Interfaces;
using ChatApp.Application.Services.QueryBuilder;
using ChatApp.Application.Services.QueryBuilder.Interfaces;
using ChatApp.Application.Services.RoomService;
using ChatApp.Application.Services.RoomService.Interfaces;
using ChatApp.Application.Services.UserContext;
using ChatApp.Application.Services.UserContext.Interfaces;
using ChatApp.Application.Services.UserService;
using ChatApp.Application.Services.UserService.Interfaces;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Mapping;
using ChatApp.Domain.Users;
using ChatApp.Persistence.Context;
using ChatApp.Persistence.UnitOfWork;
using ChatApp.Persistence.UnitOfWork.Interfaces;
using ChatApp.UI.Services.OpenAiService;
using ChatApp.UI.Services.OpenAiService.Interfaces;
using ChatApp.WebAPI.Hubs.Call;
using ChatApp.WebAPI.Hubs.Chat;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ChatApp.WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ChatDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<User, IdentityRole<int>>()
            .AddEntityFrameworkStores<ChatDbContext>()
            .AddDefaultTokenProviders();

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/chatHub") ||
                             path.StartsWithSegments("/callHub")))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer"),
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration.GetValue<string>("Jwt:Audience"),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Jwt:Key")))
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("https://localhost:7000")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddTransient<ClaimsPrincipal>(s =>
            s.GetService<IHttpContextAccessor>().HttpContext.User);

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<IUserContext, UserContext>();
        builder.Services.AddScoped<IOpenAiService, OpenAiService>();

        builder.Services.AddScoped<IFileService, FileService>();
        builder.Services.AddScoped<IMessageService, MessageService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IRoomService, RoomService>();

        builder.Services.AddScoped<IQueryBuilder<User>, QueryBuilder<User>>();
        builder.Services.AddScoped<IQueryBuilder<RoomDto>, QueryBuilder<RoomDto>>();
        builder.Services.AddScoped<IQueryBuilder<MessageDto>, QueryBuilder<MessageDto>>();

        builder.Services.AddAutoMapper(typeof(MappingProfile));

        builder.Services.AddSignalR();


        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter your JWT Token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();
        app.UseCors();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapHub<ChatHub>("/chatHub");
        app.MapHub<CallHub>("/callHub");

        app.Run();
    }
}