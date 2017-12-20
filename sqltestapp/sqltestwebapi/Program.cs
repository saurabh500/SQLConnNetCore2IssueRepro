﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace sqltestwebapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

//        public static IWebHost BuildWebHost(string[] args) =>
//            WebHost.CreateDefaultBuilder(args)
//                .UseApplicationInsights()
//                .UseStartup<Startup>()                
//                .Build();

        
        public static IWebHost BuildWebHost(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: true)
                .AddJsonFile("appsettings.json", optional: false)
                .AddCommandLine(args)
                .Build();

            return WebHost.CreateDefaultBuilder(args)
                //.UseUrls("http://*:5000")
                .UseConfiguration(config)
                .UseApplicationInsights()
                .UseStartup<Startup>()
                .Build();
        }
    }
}
