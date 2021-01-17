using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RentHelper.Database;
using RentHelper.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RentHelper.Helpers;
using Microsoft.Extensions.Logging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System.Diagnostics;

namespace RentHelper
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string keypath = Directory.GetCurrentDirectory() + "/key.json";
            Console.WriteLine(keypath);
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(keypath),
            });

            //程式導入 MVC控制器 模式，會將Controllers內的 Controller結尾的檔案進行讀取
            services.AddControllers(setupAction => {
                setupAction.ReturnHttpNotAcceptable = true;
                //若不允許的類別則返回406
            }).AddXmlDataContractSerializerFormatters();
            //設定XML為可接受的類別

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<ICartItemRepository, CartItemRepository>();
            services.AddTransient<INoteRepository, NoteRepository>();
            //資料庫使用倉庫模式

            services.AddTransient<JwtHelpers>();

            services.AddDbContext<AppDbContext>(option => {
                //option.UseMySql(Configuration["DbContext:MySQLConnectionString"]); //Docker MySQL 本地端
                option.UseMySql(Configuration["DbContext:GCPMySQLConnectionString"]); //遠端GCP MySQL Rental資料庫
                //option.UseMySql(Configuration["DbContext:GCPMySQLConnectionString2"]); //遠端GCP MySQL Rent資料庫
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //使用 AutoMapper 套件，進行數據映射
            //使用 Profile 進行對應

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // 當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
                    options.IncludeErrorDetails = true; // 預設值為 true，有時會特別關閉

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // 透過這項宣告，就可以從 "sub" 取值並設定給 User.Identity.Name
                        //NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                        // 透過這項宣告，就可以從 "roles" 取值，並可讓 [Authorize] 判斷角色
                        //RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

                        // 一般我們都會驗證 Issuer
                        ValidateIssuer = true,
                        ValidIssuer = Configuration.GetValue<string>("JwtSettings:Issuer"),

                        // 通常不太需要驗證 Audience
                        ValidateAudience = true,
                        ValidAudience = Configuration.GetValue<string>("JwtSettings:Issuer"), // 不驗證就不需要填寫

                        // 一般我們都會驗證 Token 的有效期間
                        ValidateLifetime = false,//暫時使用永久時效

                        // 如果 Token 中包含 key 才需要驗證，一般都只有簽章而已
                        ValidateIssuerSigningKey = false,

                        // SignKey 從 IConfiguration 取得
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("JwtSettings:SignKey")))
                    };
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            //加入JWT驗證的中間層
            app.UseAuthentication(); //驗證用
            app.UseAuthorization(); //授權用

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //MVC模式，自動對應Controllers資料夾內的Controller內的http響應
            });
            
        }
    }
}
