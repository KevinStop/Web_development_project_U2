﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_development_project_U2.Models.ViewModels;

namespace Web_development_project_U2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Contact()
        {
            int userId = (int)Session["UserId"];
            return View();
        }

        public ActionResult Admin()
        {
            return View();
        }
    }
}