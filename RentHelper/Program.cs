using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RentHelper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //程式由此處Main開始
            CreateHostBuilder(args).Build().Run();
            //Task.Run(OrderConsumer.GetInstance().StartOrderTask());
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel() //Kestrel 是 ASP.NET Core 的跨平台網頁伺服器
                        .UseContentRoot(Directory.GetCurrentDirectory())  //獲取專案根目錄
                        .UseUrls("http://*:5000")  //表示伺服器應接聽任何 IP 位址或主機名稱上的要求

                    .UseStartup<Startup>();
                    // 讀取Startup.cs
                });
    }
}
