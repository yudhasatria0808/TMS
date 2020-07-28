using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
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
    public class SolarInapController : BaseController 
    {
        private ISolarInapRepo RepoSolarInap;
        private IDriverRepo RepoDriver;
        private ISalesOrderRepo RepoSalesOrder;
        private IAtmRepo RepoAtm;
        private IDataBoronganRepo RepoBor;
        private Iglt_detRepo Repoglt_det;
        private IERPConfigRepo RepoERPConfig;
        private Iac_mstrRepo Repoac_mstr;
        private Ibk_mstrRepo Repobk_mstr;
        private Ipbyd_detRepo Repopbyd_det;
        private IUserRepo RepoUser;

        public SolarInapController(IUserReferenceRepo repoBase, ILookupCodeRepo repoLookup, ISolarInapRepo repoSolarInap, IDriverRepo repoDriver, ISalesOrderRepo repoSalesOrder, IAtmRepo repoAtm,
            IDataBoronganRepo repoBor, Iglt_detRepo repoglt_det, IERPConfigRepo repoERPConfig, Iac_mstrRepo repoac_mstr, Ibk_mstrRepo repobk_mstr, Ipbyd_detRepo repopbyd_det, IUserRepo repoUser)
            : base(repoBase, repoLookup)
        {   
            RepoSolarInap = repoSolarInap;
            RepoDriver = repoDriver;
            RepoSalesOrder = repoSalesOrder;
            RepoAtm = repoAtm;
            RepoBor = repoBor;
            Repoglt_det = repoglt_det;
            RepoERPConfig = repoERPConfig;
            Repoac_mstr = repoac_mstr;
            Repobk_mstr = repobk_mstr;
            Repopbyd_det = repopbyd_det;
            RepoUser = repoUser;
        }
        public ActionResult Index()
        {
            ViewBag.listKolom = ListKolom.Where(d=>d.Action == "Index" && d.Controller == "SolarInap").ToList();
            return View();
        }
        [MyAuthorize(Menu = "Batal Inap", Action="create")]
        public ActionResult batalInap(int id)
        {
            Context.SolarInap dbitem = RepoSolarInap.FindByPK(id);
            SolarInap model = new SolarInap(dbitem);
            ViewBag.name = model.Id;
            return View("batalInap", model);
        }

        [HttpPost]
        [MyAuthorize(Menu = "Batal Inap", Action="create")]
        public ActionResult batalInap(SolarInap model)
        {
            if (ModelState.IsValid)
            {
                Context.SolarInap dbitem = RepoSolarInap.FindByPK(model.Id);
                Context.SalesOrder dbso = RepoSalesOrder.FindByPK(model.IdSO.Value);
                model.setDbSolarInap(dbitem);
                RepoSolarInap.save(dbitem, UserPrincipal.id);
                if (dbitem.Transfer > 0){
                    //#Jurnal Kasbon Driver pada Biaya
                    string so_code = (dbso.SalesOrderOncallId.HasValue ? dbso.SalesOrderOncall.SONumber : dbso.SalesOrderProsesKonsolidasiId.HasValue ? dbso.SalesOrderProsesKonsolidasi.SONumber : dbso.SalesOrderPickupId.HasValue ? dbso.SalesOrderPickup.SONumber : dbso.SalesOrderKontrak.SONumber);
                    string code = "SIBI-" + dbitem.TanggalDari.ToString() + so_code;
                    Context.ERPConfig erpConfig = RepoERPConfig.FindByFrist();
                    Repoglt_det.saveFromAc(1, code, model.Transfer, 0, Repoac_mstr.FindByPk(erpConfig.IdKasbonDriver)); //D, Kasbon Driver
                    Repoglt_det.saveFromAc(2, code, 0, model.Transfer, Repoac_mstr.FindByPk(erpConfig.IdBiayaInap)); //K, Biaya Inap
                    //masuklah ke saldo piutang driver, save to pby_mstr&det
                    try{
                    var glt_oid = Guid.NewGuid().ToString();
                    Repopbyd_det.saveMstr(glt_oid, dbitem.Code, erpConfig.IdTfCredit.Value, "Batal Inap " + code, dbitem.IdDriver.Value);
                    Repopbyd_det.save(glt_oid, dbitem.Code, erpConfig.IdTfCredit.Value, "Batal Inap " + code, dbitem.IdDriver.Value, erpConfig.IdKasbonDriver.Value, Repoac_mstr.FindByPk(erpConfig.IdKasbonDriver).ac_name, model.Transfer);
                    }
                    catch (Exception e)
                    {
                    }
                }
                return RedirectToAction("Index");
            }
            return View("batalInap", model);
        }

        public ActionResult pengembalianUang(int id)
        {
            Context.SolarInap dbitem = RepoSolarInap.FindByPK(id);
            SolarInap model = new SolarInap(dbitem);
            ViewBag.name = model.Id;
            return View("pengembalianUang", model);
        }
        [HttpPost]
        public ActionResult pengembalianUang(SolarInap model)
        {
            if (ModelState.IsValid)
            {
                Context.SolarInap dbitem = RepoSolarInap.FindByPK(model.Id);
                model.setPengembalianUangSolarInap(dbitem);
                RepoSolarInap.save(dbitem, UserPrincipal.id);
                return RedirectToAction("Index");
            }
            return View("pengembalianUang", model);
        }
        public ActionResult kasirCash(int id)
        {
            Context.SolarInap dbitem = RepoSolarInap.FindByPK(id);
            SolarInap model = new SolarInap(dbitem);
            ViewBag.name = model.Id;
            return View("kasirCash", model);
        }
        [HttpPost]
        public ActionResult kasirCash(SolarInap model)
        {
            if (ModelState.IsValid)
            {
                Context.SolarInap dbitem = RepoSolarInap.FindByPK(model.Id);
                model.setDbKasirCash(dbitem);
                RepoSolarInap.save(dbitem, UserPrincipal.id);
                return RedirectToAction("Index");
            }
            return View("kasirCash", model);
        }
        public ActionResult kasirTransfer(int id)
        {
            Context.SolarInap dbitem = RepoSolarInap.FindByPK(id);
            SolarInap model = new SolarInap(dbitem);
            ViewBag.name = model.Id;
            return View("kasirTransfer", model);
        }
        [HttpPost]
        public ActionResult kasirTransfer(SolarInap model)
        {
            if (ModelState.IsValid)
            {
                Context.SolarInap dbitem = RepoSolarInap.FindByPK(model.Id);
                Context.SalesOrder dbso = RepoSalesOrder.FindByPK(model.IdSO.Value);
                if (dbitem.Cash <= 0)
                    dbitem.Cash = 0;
                model.setDbKasirTransfer(dbitem);
                dbitem.IdCreditTf = model.IdCreditTf;
                //#Marketing, Status Ditagih, Piutang Customer pada Biaya Inapdbitem
                string code = "SIKT-" + dbitem.TanggalDari.ToString() + (dbso.SalesOrderOncallId.HasValue ? dbso.SalesOrderOncall.SONumber : dbso.SalesOrderProsesKonsolidasiId.HasValue ? dbso.SalesOrderProsesKonsolidasi.SONumber : dbso.SalesOrderPickupId.HasValue ? dbso.SalesOrderPickup.SONumber : dbso.SalesOrderKontrak.SONumber);
                Context.ERPConfig erpConfig = RepoERPConfig.FindByFrist();
                Repoglt_det.saveFromAc(1, code, model.Transfer, 0, Repoac_mstr.FindByPk(erpConfig.IdAUJCredit)); //D, Biaya Inap
                Repoglt_det.saveFromAc(2, code, 0, model.Transfer, Repoac_mstr.FindByPk(dbitem.IdCreditTf)); //K, Transfer / Bank
                RepoSolarInap.save(dbitem, UserPrincipal.id);
                return RedirectToAction("Index");
            }
            return View("kasirCash", model);
        }
        public string Binding(string user_types)
        {
            GridRequestParameters param = GridRequestParameters.Current;
            Context.User user = RepoUser.FindByPK(UserPrincipal.id);
            List<SolarInap> ListModel = new List<SolarInap>();

            List<Context.SolarInap> items = RepoSolarInap.FindAll(user_types);

            foreach (Context.SolarInap item in items)
            {
                if (item.SalesOrderKontrakListSOId.HasValue)
                {
                    var soKontrak = item.SO.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => d.Id == item.SalesOrderKontrakListSOId).FirstOrDefault();
                    ListModel.Add(new SolarInap(item, soKontrak));
                }
                else
                { ListModel.Add(new SolarInap(item)); }
            }
            int total = RepoSolarInap.CountTrans(user_types, param.Filters);
            return new JavaScriptSerializer().Serialize(new { total = total, data = ListModel });
        }

        public ActionResult Add()
        {
            SolarInap model = new SolarInap();
            model.StepKe = 1;
            ViewBag.IdSO = 0;
            return View("Form",model);
        }
        [HttpPost]
        public ActionResult Add(SolarInap model)
        {
            bool palid = true;
            if (ModelState.IsValid)
            {
                Context.SolarInap dbitem = new Context.SolarInap();
                if (RepoSolarInap.FindBySOAndDate(model.IdSO, DateTime.Parse(model.TanggalDari)) != null)
                {
                    ModelState.AddModelError("IdSO", "Order terpilih sudah diset di tanggal "+model.TanggalDari);
                    palid = false;
                }
                if (palid){
                    model.setDb(dbitem);
                    RepoSolarInap.save(dbitem, UserPrincipal.id);
                }
                else{
                    ViewBag.IdSO = model.IdSO;
                    return View("Form", model);
                }

                return RedirectToAction("Index");
            }
            return View("Form", model);
        }
        public ActionResult Edit(int id)
        {
            Context.SolarInap dbitem = RepoSolarInap.FindByPK(id);
            SolarInap model = new SolarInap(dbitem);
            model.StepKe += 1;
            ViewBag.name = model.Id;
            ViewBag.IdSO = 0;
            return View("Form", model);
        }
        [HttpPost]
        public ActionResult Edit(SolarInap model)
        {
            bool palid = true;
            if (model.Cash == null || model.Cash <= 0)
                model.Cash = 0;
            if (ModelState.IsValid)
            {
                Context.SolarInap dbitem = RepoSolarInap.FindByPK(model.Id);
                Context.SalesOrder dbso = RepoSalesOrder.FindByPK(model.IdSO.Value);
                if (dbitem.StepKe == 2 && model.Transfer <=0)
                {
                    ModelState.AddModelError("Transfer", "Harus lebih besar dari 0");
                    palid = false;
                }
                if (palid){
                    if (dbitem.StepKe == 1 && model.StatusTagihan == "Ditagih"){
                        //#Jurnal Piutang Customer pada Biaya
                        string so_code = (dbso.SalesOrderOncallId.HasValue ? dbso.SalesOrderOncall.SONumber : dbso.SalesOrderProsesKonsolidasiId.HasValue ? dbso.SalesOrderProsesKonsolidasi.SONumber : dbso.SalesOrderPickupId.HasValue ? dbso.SalesOrderPickup.SONumber : dbso.SalesOrderKontrak.SONumber);
                        string code = "SIM-" + dbitem.TanggalDari.ToString() + so_code;
                        Context.ERPConfig erpConfig = RepoERPConfig.FindByFrist();
                        Repoglt_det.saveFromAc(1, code, model.Nominal, 0, Repoac_mstr.FindByPk(erpConfig.IdPiutangCustomer)); //D, Piutang Customer
                        Repoglt_det.saveFromAc(2, code, 0, model.Nominal, Repoac_mstr.FindByPk(erpConfig.IdBiayaInap)); //K, Biaya Inap
                    }
                    else if (dbitem.StepKe == 2){
                        //#Jurnal Piutang Customer pada Biaya
                        string so_code = (dbso.SalesOrderOncallId.HasValue ? dbso.SalesOrderOncall.SONumber : dbso.SalesOrderProsesKonsolidasiId.HasValue ? dbso.SalesOrderProsesKonsolidasi.SONumber : dbso.SalesOrderPickupId.HasValue ? dbso.SalesOrderPickup.SONumber : dbso.SalesOrderKontrak.SONumber);
                        string code = "SIAUJ-" + dbitem.TanggalDari.ToString() + so_code;
                        Context.ERPConfig erpConfig = RepoERPConfig.FindByFrist();
                        Repoglt_det.saveFromAc(1, code, dbitem.NilaiYgDiajukan, 0, Repoac_mstr.FindByPk(erpConfig.IdTambahanRuteMuat)); //D, Piutang Customer
                        Repoglt_det.saveFromAc(2, code, 0, dbitem.NilaiYgDiajukan, Repoac_mstr.FindByPk(erpConfig.IdAUJCredit)); //K, Biaya Inap
                    }
                    model.setDb(dbitem);
                    RepoSolarInap.save(dbitem, UserPrincipal.id);
                }
                else{
                    return View("Form", model);
                }
                return RedirectToAction("Index");
            }
            ViewBag.IdSO = 0;
            return View("Form", model);
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            ResponeModel response = new ResponeModel(true);
            Context.SolarInap dbItem = RepoSolarInap.FindByPK(id);

            RepoSolarInap.delete(dbItem, UserPrincipal.id);

            return Json(response);
        }
        public string BindingSo()
        {
            List<Context.SalesOrder> items = RepoSalesOrder.FindAllAdminDispatched();

            List<AdminUangJalanIndex> ListModel = new List<AdminUangJalanIndex>();
            foreach (Context.SalesOrder item in items)
            {
                if (item.SalesOrderKontrakId.HasValue)
                {
                    var data = item.SalesOrderKontrak.SalesOrderKontrakListSo.Where(p => p.IsProses && p.Status == "dispatched").GroupBy(d => new { d.IdDataTruck, d.Driver1Id, d.Status, d.Urutan }).Select(grp => grp.ToList());
                    foreach (var itemGroup in data.ToList())
                    {
                        ListModel.Add(new AdminUangJalanIndex(item, itemGroup));
                        foreach (var itemKontrakPerOrder in itemGroup.OrderBy(t => t.MuatDate).ToList()){
                            ListModel.Add(new AdminUangJalanIndex(item, itemGroup.Where(d => d.Id == itemKontrakPerOrder.Id).ToList()));
                        }
                    }
                }
                else
                {
                    ListModel.Add(new AdminUangJalanIndex(item));
                }

            }

            return new JavaScriptSerializer().Serialize(new { total = items.Count, data = ListModel });
        }
        public string GetDetailSo(int idSo, string ListIdSo)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(idSo);
            AdminUangJalan model = new AdminUangJalan();

            if (dbitem.SalesOrderKontrakId.HasValue)
            {
                List<int> ListIdDumy = ListIdSo.Split(new string[] { "." }, StringSplitOptions.None).ToList().Select(int.Parse).ToList();
                List<Context.SalesOrderKontrakListSo> dbsoDummy = dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => ListIdDumy.Contains(d.Id)).ToList();
                dbitem.SalesOrderKontrak.SalesOrderKontrakListSo = dbsoDummy;

                if (dbsoDummy.FirstOrDefault().IdAdminUangJalan.HasValue)
                    model = new AdminUangJalan(dbsoDummy.FirstOrDefault().AdminUangJalan, RepoAtm.FindAll(), RepoBor.FindAll());
            }
            else
            {
                try
                {
                    model = new AdminUangJalan(dbitem.AdminUangJalan, RepoAtm.FindAll(), RepoBor.FindAll());
                }
                catch { }
            }

            return new JavaScriptSerializer().Serialize(model);
        }
        [HttpGet]
        public PartialViewResult GetPartialSo(int idSo, string ListIdSo)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(idSo);

            if (dbitem.SalesOrderOncallId.HasValue)
            {
                AdminUangJalan model = new AdminUangJalan(dbitem.AdminUangJalan, RepoAtm.FindAll(), RepoBor.FindAll());
                model.ModelOncall = new SalesOrderOncall(dbitem);
                return PartialView("SalesOrderOncall/_PartialFormReadOnly", model.ModelOncall);
            }
            else if (dbitem.SalesOrderPickupId.HasValue)
            {
                AdminUangJalan model = new AdminUangJalan(dbitem.AdminUangJalan, RepoAtm.FindAll(), RepoBor.FindAll());
                model.ModelPickup = new SalesOrderPickup(dbitem);
                return PartialView("SalesOrderPickup/_PartialFormReadOnly", model.ModelPickup);
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue)
            {
                AdminUangJalan model = new AdminUangJalan(dbitem.AdminUangJalan, RepoAtm.FindAll(), RepoBor.FindAll());
                model.ModelKonsolidasi = new SalesOrderProsesKonsolidasi(dbitem);
                return PartialView("SalesOrderProsesKonsolidasi/_PartialFormReadOnly", model.ModelKonsolidasi);
            }
            else if (dbitem.SalesOrderKontrakId.HasValue)
            {
                List<int> ListIdDumy = ListIdSo.Split(new string[] { "." }, StringSplitOptions.None).ToList().Select(int.Parse).ToList();
                List<Context.SalesOrderKontrakListSo> dbsoDummy = dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => ListIdDumy.Contains(d.Id)).ToList();
                dbitem.SalesOrderKontrak.SalesOrderKontrakListSo = dbsoDummy;
                AdminUangJalan model = new AdminUangJalan(dbsoDummy.FirstOrDefault().AdminUangJalan, RepoAtm.FindAll(), RepoBor.FindAll());
                model.ModelKontrak = new SalesOrderKontrak(dbitem);
                return PartialView("SalesOrderKontrak/_PartialFormReadOnly", model);
            }
            return PartialView("");
        }
    }
}