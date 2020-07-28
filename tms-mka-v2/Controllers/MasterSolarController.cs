using System;
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

namespace tms_mka_v2.Controllers
{
    public class MasterSolarController : BaseController
    {
        private IMasterSolarRepo RepoMasterSolar;
        public MasterSolarController(IUserReferenceRepo repoBase, ILookupCodeRepo repoLookup, IMasterSolarRepo repoMasterSolar)
            : base(repoBase, repoLookup)
        {
            RepoMasterSolar = repoMasterSolar;
        }
        public ActionResult Index()
        {
            ViewBag.listKolom = ListKolom.Where(d => d.Action == "Index" && d.Controller == "MasterSolar").ToList();
            return View();
        }
        public string Binding()
        {
            GridRequestParameters param = GridRequestParameters.Current;

            List<Context.MasterSolar> items = RepoMasterSolar.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);

            List<MasterSolar> ListModel = new List<MasterSolar>();
            foreach (Context.MasterSolar item in items)
            {
                ListModel.Add(new MasterSolar(item));
            }

            int total = RepoMasterSolar.Count(param.Filters);

            return new JavaScriptSerializer().Serialize(new { total = total, data = ListModel });
        }
        [HttpPost]
        public JsonResult Add(List<MasterSolar> models)
        {
            Context.MasterSolar dbitem = new Context.MasterSolar();
            models[0].setDb(dbitem);
            RepoMasterSolar.save(dbitem);
            ResponeModel response = new ResponeModel(true);

            return Json(response);
        }
        [HttpPost]
        public JsonResult Delete(List<MasterSolar> models)
        {
            Context.MasterSolar dbitem = RepoMasterSolar.FindByPK(models[0].Id);
            RepoMasterSolar.delete(dbitem);
            ResponeModel response = new ResponeModel(true);
            return Json(response);
        }
        public string GetCurrentSolar()
        {
            ResponeModel response = new ResponeModel();
            DateTime currDate = DateTime.Now.Date;
            Context.MasterSolar dbItem = RepoMasterSolar.FindAll().Where(d => currDate >= d.Start && currDate <= d.End).FirstOrDefault();

            if (dbItem != null)
            {
                response.Success = true;
                response.Data = dbItem.Harga.ToString();

            }
            else
            {
                response.Success = false;
            }
 
            return new JavaScriptSerializer().Serialize(response);
        }
    }
}