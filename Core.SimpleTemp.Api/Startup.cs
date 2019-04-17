using Core.SimpleTemp.Application.Authorization;
using Core.SimpleTemp.Common;
using Core.SimpleTemp.Common.PagingQuery;
using Core.SimpleTemp.Mvc.Filters;
using Core.SimpleTemp.Repository.RepositoryEntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CS1591
namespace Core.SimpleTemp.Mvc
{
    public class Startup
    {
        private const string WEBAPP_OPTIONS = "WebAppOptions";
        public IConfiguration _configuration { get; }
        private readonly ILogger _logger;
        private readonly WebAppOptions _webAppOptions;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            _configuration = configuration;
            _webAppOptions = _configuration.GetSection(WEBAPP_OPTIONS).Get<WebAppOptions>();
            _logger = logger;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            #region 跨域规则
            //配置跨域规则 [EnableCors("any")]
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    builder.AllowAnyOrigin() //允许任何来源的主机访问
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();//指定处理cookie
                });
            });
            #endregion


            #region 数据仓储链接设置 DbContext
            services.AddDbContext<CoreDBContext>(options =>
               {
                   options.UseRemoveForeignKeyService();
                   options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
               });
            #endregion


            #region 认证相关
            services.AddAuthentication(option =>
          {
              option.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
          }).AddCookie(option =>
          {
              //为了给Api相应401去掉此设置相应302 并且重定向到指定URL
              option.Events.OnRedirectToLogin = (context) =>
              {
                  context.Response.StatusCode = 401;
                  return Task.CompletedTask;
              };
              option.Cookie.SameSite = SameSiteMode.None;
              //设置Cookie过期时间 ,如不设置 默认为14天挺恐怖的 注意如不设置票据过期时间，默认票据采用此时间

              option.ExpireTimeSpan = TimeSpan.FromMinutes(_webAppOptions.TimeOutOfLogin);
              //当Cookie过期时间已达一半时，是否重置ExpireTimeSpan 每次认证确认，handle 在Http响应重写Cookie
              option.SlidingExpiration = true;

          }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, JwtOption =>
          {
              //禁用Https模式
              JwtOption.RequireHttpsMetadata = false;
              JwtOption.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateAudience = true,
                  ValidAudience = _webAppOptions.JwtValidAudience,
                  ValidateIssuer = true,
                  ValidIssuer = _webAppOptions.JwtValidIssuer,
                  //是否验证失效时间
                  ValidateLifetime = true,

                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_webAppOptions.JwtIssuerSigningKey))
              };
          });
            #endregion

            services.Configure<WebAppOptions>(_configuration.GetSection(WEBAPP_OPTIONS));

            //AutoDI
            services.AutoDi(_logger);

            //自定义授权
            services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
            //services.AddAuthorization();
            //采用内存版分布缓存 方便以后切换Redis
            services.AddDistributedMemoryCache(); //services.AddDistributeRedisCache(null);
            services.AddTransient(typeof(IPagingQueryModelBuild<>), typeof(PagingQueryModelBuildFromBody<>));

            #region Swagger
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
               {
                   c.SwaggerDoc("Sys", new Info
                   {
                       Version = "V1",
                       Title = "Core.SimpleTemp-Vue API",
                       Description = "Core.SimpleTemp-Vue ",
                       TermsOfService = "None",

                   });
                   c.SwaggerDoc("Other", new Info
                   {
                       Version = "V2",
                       Title = "Core.SimpleTemp-Vue API",
                       Description = "Core.SimpleTemp-Vue ",
                       TermsOfService = "None",

                   });

                   var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
                   var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                   //... and tell Swagger to use those XML comments.
                   c.IncludeXmlComments(xmlPath);
               });
            #endregion

            services.AddHttpClient();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMvc(options =>
            {
                //自定义全局异常过滤器
                options.Filters.Add<HttpGlobalExceptionFilter>();
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //生产环境异常处理
                app.UseExceptionHandler(new ExceptionHandlerOptions()
                {
                    ExceptionHandler = async context =>
                    {
                        //context.Items.Add()
                        //Ajax处理
                        if (context.Request.IsAjaxRequest() || context.Request.Path.Value.StartsWith("/api/"))
                        {
                            context.Response.StatusCode = 200;
                            context.Response.ContentType = "application/json;charset=utf-8";
                            var err = context.Features.Get<IExceptionHandlerFeature>();
                            var errStr = err.Error.Message;
                            var sb = context.Response.CreateAjaxResponseExceptionJson(errStr);
                            await context.Response.WriteAsync(sb.ToString());
                        }
                        else
                        {
                            context.Response.Redirect("/error.html");
                        }

                    }
                });

            }



            #region 静态文件
            var cachePeriod = env.IsDevelopment() ? "600" : "604800";//七天
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cachePeriod}");
                },
            });
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cachePeriod}");
                },
                FileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory())
            });
            #endregion


            #region UseSwagger
            if (env.IsDevelopment())
            {
                //app.Use(async (context, next) =>
                //{

                //    await next.Invoke();

                //    if (context.Items.Any(i => i.Key.ToString() == "error"))
                //    {
                //        await context.Response.WriteAsync("如果你看到了这句话说明你的程序中有了Error: \r\n" + context.Items["error"].ToString());
                //    }
                //});
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
                // specifying the Swagger JSON endpoint.

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/Sys/swagger.json", "Sys基础信息");
                    c.SwaggerEndpoint("/swagger/Other/swagger.json", "My API V1");
                    c.RoutePrefix = "swagger";

                });
            }
            #endregion
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();

        }
    }
}
