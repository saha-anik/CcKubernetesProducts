﻿using CcKubernetes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CcKubernetes.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var homeviewModel = new HomeViewModel()
            {
                Version = _configuration.GetConnectionString("BuildVersion")
            };
            
            try
            {
                using var client = new HttpClient();
                HttpResponseMessage response = client.GetAsync($"http://{_configuration.GetConnectionString("Seq")}:5341").Result;

                if (response.RequestMessage?.RequestUri != null)
                {
                    homeviewModel.Version = response.RequestMessage.RequestUri.ToString();
                }
            }
            catch (Exception e)
            {
                homeviewModel.Version = e.Message + "|" + $"http://{_configuration.GetConnectionString("Seq")}:5341";
            }
            
            return View(homeviewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
