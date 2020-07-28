﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security;
using Newtonsoft.Json;
using tms_mka_v2.Business_Logic.Abstract;
using tms_mka_v2.Models;
using tms_mka_v2.Security;
using tms_mka_v2.Infrastructure;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using OfficeOpenXml;

namespace tms_mka_v2.Controllers
{
    public class AuditrailController : BaseController 
    {
        private IAuditrailRepo RepoAuditrail;
        private IUserRepo RepoUser;
        public AuditrailController(IUserReferenceRepo repoBase, ILookupCodeRepo repoLookup,
            IAuditrailRepo repoAuditrail, IUserRepo repoUser)
            : base(repoBase, repoLookup)
        {
            RepoAuditrail = repoAuditrail;
            RepoUser = repoUser;
        }
        [MyAuthorize(Menu = "Audit Trail", Action="read")]
        public ActionResult Index()
        {
            ViewBag.listKolom = ListKolom.Where(d=>d.Action == "Index" && d.Controller == "Auditrail").ToList();
            return View();
        }
        public string Binding()
        {
            GridRequestParameters param = GridRequestParameters.Current;

            List<Context.Auditrail> items = RepoAuditrail.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);
            List<Auditrail> ListModel = new List<Auditrail>();
            foreach (Context.Auditrail item in items)
            {
                ListModel.Add(new Auditrail(item));
            }

            int total = RepoAuditrail.Count(param.Filters);

            return new JavaScriptSerializer().Serialize(new { total = total, data = ListModel });
        }
        [HttpPost]
        public ActionResult Add(Auditrail model)
        {
            if (ModelState.IsValid)
            {
                Context.Auditrail dbitem = new Context.Auditrail();
                model.setDb(dbitem);
                RepoAuditrail.save(dbitem);

                return RedirectToAction("Index");
            }
            return View("Form", model);
        }
    }
}