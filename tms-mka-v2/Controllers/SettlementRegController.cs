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
using OfficeOpenXml;

namespace tms_mka_v2.Controllers
{
    public class SettlementRegController : BaseController
    {
        private ISettlementRegRepo RepoSettlementReg;
        private ISalesOrderRepo RepoSalesOrder;
        private IAtmRepo RepoAtm;
        private IDataBoronganRepo RepoBor;
        private ISalesOrderKontrakListSoRepo RepoListSo;
        private Iglt_detRepo Repoglt_det;
        private IERPConfigRepo RepoERPConfig;
        private Iac_mstrRepo Repoac_mstr;
        private Ibk_mstrRepo Repobk_mstr;
        private IAdminUangJalanRepo RepoAdminUangJalan;
        private IAuditrailRepo RepoAuditrail;
        private Iso_mstrRepo Reposo_mstr;
        public SettlementRegController(IUserReferenceRepo repoBase, ILookupCodeRepo repoLookup, ISettlementRegRepo repoSettlementReg, ISalesOrderRepo repoSalesOrder, IAtmRepo repoAtm,
            IDataBoronganRepo repoBor, ISalesOrderKontrakListSoRepo repoListSo, Iglt_detRepo repoglt_det, IERPConfigRepo repoERPConfig, Iac_mstrRepo repoac_mstr,
            Ibk_mstrRepo repobk_mstr, IAdminUangJalanRepo repoAdminUangJalan, IAuditrailRepo repoAuditrail, Iso_mstrRepo reposo_mstr)
            : base(repoBase, repoLookup)
        {
            RepoSettlementReg = repoSettlementReg;
            RepoSalesOrder = repoSalesOrder;
            RepoAtm = repoAtm;
            RepoBor = repoBor;
            RepoListSo = repoListSo;
            Repoglt_det = repoglt_det;
            RepoERPConfig = repoERPConfig;
            Repoac_mstr = repoac_mstr;
            Repobk_mstr = repobk_mstr;
            RepoAdminUangJalan = repoAdminUangJalan;
            RepoAuditrail = repoAuditrail;
            Reposo_mstr = reposo_mstr;
        }
        [MyAuthorize(Menu = "Settlement Reguler", Action="read")]
        public ActionResult Index()
        {
            ViewBag.listKolom = ListKolom.Where(d => d.Action == "Index" && d.Controller == "SettlementReg").ToList();
            return View();
        }
        public string Binding()
        {
            GridRequestParameters param = GridRequestParameters.Current;

            //List<Context.SettlementReguler> items = RepoSettlementReg.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);
            List<Context.SettlementReguler> items = RepoSettlementReg.FindAll();

            List<SettlementRegIndex> ListModel = new List<SettlementRegIndex>();
            foreach (Context.SettlementReguler item in items)
            {
                if (item.SalesOrder.SalesOrderKontrakId.HasValue) 
                {
                    List<int> ListIdDumy = item.LisSoKontrak.Split(new string[] { "." }, StringSplitOptions.None).ToList().Select(int.Parse).ToList();
                    List<Context.SalesOrderKontrakListSo> dbsoDummy = item.SalesOrder.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => ListIdDumy.Contains(d.Id)).ToList();
                    ListModel.Add(new SettlementRegIndex(item, dbsoDummy));
                }
                else
                {
                    ListModel.Add(new SettlementRegIndex(item));
                }
            }

            //int total = RepoSettlementReg.Count(param.Filters);

            return new JavaScriptSerializer().Serialize(new { total = ListModel.Count(), data = ListModel });
        }
        [MyAuthorize(Menu = "Settlement Reguler", Action="print")]
        public ActionResult Print(int id, string listSo, int idSo)
        {
            return new Rotativa.ActionAsPdf("ShowPrint", new { id = id, listSo = listSo, idSo = idSo })
            {
                FileName = new Guid().ToString() + ".pdf",
                PageSize = Rotativa.Options.Size.A5,
                IsJavaScriptDisabled = false,
                PageOrientation = Rotativa.Options.Orientation.Landscape
            };
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
                    }
                }
                else {
                    ListModel.Add(new AdminUangJalanIndex(item));
                }
                
            }

            return new JavaScriptSerializer().Serialize(new { total = items.Count, data = ListModel });
        }
        [MyAuthorize(Menu = "Settlement Reguler", Action="create")]
        public ActionResult Add()
        {
            SettlementReg model = new SettlementReg();
            return View("Form", model);
        }

        [HttpPost]
        public ActionResult Add(SettlementReg model)
        {
            SettlementRegTambahanBiaya[] res = JsonConvert.DeserializeObject<SettlementRegTambahanBiaya[]>(model.StrBiayaTambahan);
            model.ListBiayaTambahan = res.ToList();
            Context.ERPConfig erpConfig = RepoERPConfig.FindByFrist();
            Context.SalesOrder dbso = RepoSalesOrder.FindByPK(model.IdSalesOrder.Value);
            string sod_oid = Guid.NewGuid().ToString();

            if (ModelState.IsValid)
            {
                if (model.listIdSoKontrak != "" && model.listIdSoKontrak != null)
                {
                    List<int> ListIdDumy = model.listIdSoKontrak.Split(new string[] { "." }, StringSplitOptions.None).ToList().Select(int.Parse).ToList();
                    List<Context.SalesOrderKontrakListSo> dbsoDummy = dbso.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => ListIdDumy.Contains(d.Id)).ToList();
                    //dbso.SalesOrderKontrak.SalesOrderKontrakListSo = dbsoDummy;
                    foreach (var item in dbsoDummy)
                    {
                        item.Status = "settlement";
                    }
                    Context.SettlementReguler dbitem = new Context.SettlementReguler();
                    model.SetDb(dbitem);
                    RepoSalesOrder.save(dbso);
                    RepoSettlementReg.save(dbitem, UserPrincipal.id);
                    foreach (Context.SettlementRegulerTambahanBiaya srtb in dbitem.SettlementRegulerTambahanBiaya) {
                        RepoAuditrail.saveSettlementRegulerBiayaTambahanQuery(srtb, UserPrincipal.id);
                    }
                }
                else
                {
                    Context.SettlementReguler dbitem = new Context.SettlementReguler();
                    string code = (dbso.SalesOrderOncallId.HasValue ? dbso.SalesOrderOncall.SONumber : dbso.SalesOrderProsesKonsolidasiId.HasValue ? dbso.SalesOrderProsesKonsolidasi.SONumber : dbso.SalesOrderPickupId.HasValue ? dbso.SalesOrderPickup.SONumber : dbso.SalesOrderKontrak.SONumber);
                    dbitem.Code = "SR-" + code;
                    string unyu = dbitem.Code;
                    model.SetDb(dbitem);
                    Context.AdminUangJalan db = RepoAdminUangJalan.FindByPK(dbso.AdminUangJalanId.Value);
                    dbso.Status = "settlement";
                    dbso.UpdatedBy = UserPrincipal.id;
                    if (dbso.oidErp == null)
                        dbso.oidErp = sod_oid;
                    RepoSalesOrder.save(dbso);
                    RepoSettlementReg.save(dbitem, UserPrincipal.id);
                    RepoAuditrail.saveSettRegHistory(dbso);
                    foreach (Context.SettlementRegulerTambahanBiaya srtb in dbitem.SettlementRegulerTambahanBiaya) {
                        RepoAuditrail.saveSettlementRegulerBiayaTambahanQuery(srtb, UserPrincipal.id);
                    }

                    //lebah dieu sync ERPna
                    //klo ada revisi rute
                    //D
                    Repoglt_det.saveFromAc(1, dbitem.Code, dbitem.KasSelisih + dbitem.TransferSelisih + dbitem.SolarSelisih + (dbitem.KapalSelisih == null ? 0 : dbitem.KapalSelisih), 0, Repoac_mstr.FindByPk(erpConfig.IdKasbonDriver)); //Kasbon Driver (Piutang)
                    Repoglt_det.saveFromBk(2, dbitem.Code, dbitem.KasAktual + dbitem.TransferAktual + dbitem.SolarAktual + (dbitem.KapalAktual == null ? 0 : dbitem.KapalAktual), 0, Repobk_mstr.FindByPk(erpConfig.IdCashCredit)); //Cash
                    Repoglt_det.saveFromAc(3, dbitem.Code, dbitem.KasDiakui + dbitem.TransferDiakui + dbitem.SolarDiakui + (dbitem.KapalDiakui == null ? 0 : dbitem.KapalDiakui), 0, Repoac_mstr.FindByPk(erpConfig.IdBiayaPerjalanan)); //Biaya

                    //K
                    decimal? tambahanRute = db.AdminUangJalanTambahanRute.Sum(s => s.values);
                    decimal? boronganDasar = db.TotalBorongan - db.Kawalan - db.Timbangan - db.Karantina - db.SPSI - db.Multidrop - tambahanRute - db.AdminUangJalanTambahanLain.Sum(s => s.Values);
                    Repoglt_det.saveFromAc(1, dbitem.Code, 0, boronganDasar, Repoac_mstr.FindByPk(erpConfig.IdBoronganDasar));
                    Repoglt_det.saveFromAc(2, dbitem.Code, 0, db.Kawalan, Repoac_mstr.FindByPk(erpConfig.IdKawalan));
                    Repoglt_det.saveFromAc(3, dbitem.Code, 0, db.Timbangan, Repoac_mstr.FindByPk(erpConfig.IdTimbangan));
                    Repoglt_det.saveFromAc(4, dbitem.Code, 0, db.Karantina, Repoac_mstr.FindByPk(erpConfig.IdKarantina));
                    Repoglt_det.saveFromAc(5, dbitem.Code, 0, db.SPSI, Repoac_mstr.FindByPk(erpConfig.IdSPSI));
                    Repoglt_det.saveFromAc(6, dbitem.Code, 0, db.Multidrop, Repoac_mstr.FindByPk(erpConfig.IdMultidrop));
                    Repoglt_det.saveFromAc(7, dbitem.Code, 0, tambahanRute, Repoac_mstr.FindByPk(erpConfig.IdTambahanRuteMuat));
                    Repoglt_det.saveFromAc(8, dbitem.Code, 0, db.AdminUangJalanTambahanLain.Sum(s => s.Values), Repoac_mstr.FindByPk(erpConfig.IdTambahanRuteLain));
                    Repoglt_det.saveFromAc(4, dbitem.Code, 0, db.TotalPotonganDriver, Repoac_mstr.FindByPk(erpConfig.IdKasbonAuj)); //Hutang ke Driver, jurnal balik dr auj

                    //Cash&Transfer pada biaya tambahan
                    Repoglt_det.saveFromAc(3, dbitem.Code, dbitem.SettlementRegulerTambahanBiaya.Sum(d => d.Value), 0, Repoac_mstr.FindByPk(erpConfig.IdBiayaPerjalanan)); //D, Biaya Tambahan
                    Repoglt_det.saveFromAc(2, dbitem.Code, 0, dbitem.TotalCash, Repoac_mstr.FindByPk(erpConfig.IdCashCredit));//K, Cash
                    Repoglt_det.saveFromAc(2, dbitem.Code, 0, dbitem.TotalTf, Repoac_mstr.FindByPk(erpConfig.IdTfCredit));//K, Transfer
                }
                return RedirectToAction("Index");
            }

            return View("Form", model);
        }

        public void SyncToERP(Context.SalesOrder dbso, string sod_guid){
            //if (dbso.Status == "save")//save to so_mstr for invoice processing
            decimal harga = RepoSalesOrder.Harga(dbso);
            Context.SalesOrderOncall dbitem = dbso.SalesOrderOncall;
            string guid = Guid.NewGuid().ToString();
            Reposo_mstr.saveSoMstr(dbso, UserPrincipal.username, guid, dbitem.CustomerId.Value, harga);
            Reposo_mstr.saveSoDet(dbso, UserPrincipal.username, guid, sod_guid);
            //Jurnal Piutang blm inv pada penjualan blm inv
/*            Context.ERPConfig erpConfig = RepoERPConfig.FindByFrist();
            Repoglt_det.saveFromAc(1, dbso.SalesOrderOncall.SONumber, harga, 0, Repoac_mstr.FindByPk(erpConfig.IdPiutangDagang)); //Piutang blm inv
            Repoglt_det.saveFromAc(2, dbso.SalesOrderOncall.SONumber, 0, harga, Repoac_mstr.FindByPk(erpConfig.IdPendapatanUsahaBlmInv)); //Penjualan blm inv
*/        }

        [MyAuthorize(Menu = "Settlement Reguler", Action="update")]
        public ActionResult Edit(int id)
        {
            Context.SettlementReguler dbitem = RepoSettlementReg.FindByPK(id);
            SettlementReg model = new SettlementReg(dbitem, RepoAtm.FindAll());
            if (model.ModelOncall != null)
                ViewBag.name = model.ModelOncall.SONumber;
            if (model.ModelPickup != null)
                ViewBag.name = model.ModelPickup.SONumber;
            if (model.ModelKonsolidasi != null)
                ViewBag.name = model.ModelKonsolidasi.SONumber;
            if (model.ModelKontrak != null)
                ViewBag.name = model.ModelKontrak.SONumber;

            return View("Form", model);
        }
        public ActionResult ShowPrint(int id, string listSo, int idSo)
        {
            Context.SettlementReguler dbitem2 = RepoSettlementReg.FindByPK(id);
            SettlementReg model = new SettlementReg(dbitem2, RepoAtm.FindAll());
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(idSo);
            var items = model.ListBiayaTambahan;
            ViewBag.item = items;

            if (dbitem.SalesOrderOncallId.HasValue)
            {
                model.ModelOncall = new SalesOrderOncall(dbitem);
                ViewBag.NoSO = model.ModelOncall.SONumber;
                ViewBag.Customer = model.ModelOncall.NamaCustomer;
                ViewBag.VNo = model.ModelOncall.VehicleNo;
                ViewBag.Driver = model.ModelOncall.NamaDriver1;
                return View("Print", model);
            }
            else if (dbitem.SalesOrderPickupId.HasValue)
            {
                model.ModelPickup = new SalesOrderPickup(dbitem);
                ViewBag.NoSO = model.ModelPickup.SONumber;
                ViewBag.Customer = model.ModelPickup.NamaCustomer;
                ViewBag.VNo = model.ModelPickup.VehicleNo;
                ViewBag.Driver = model.ModelPickup.NamaDriver1;
                return View("Print", model);
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue)
            {
                model.ModelKonsolidasi = new SalesOrderProsesKonsolidasi(dbitem);
                ViewBag.NoSO = model.ModelKonsolidasi.SONumber;
                ViewBag.Customer = "";
                ViewBag.VNo = model.ModelKonsolidasi.VehicleNo;
                ViewBag.Driver = model.ModelKonsolidasi.NamaDriver1;
                return View("Print", model);
            }
            else if (dbitem.SalesOrderKontrakId.HasValue)
                return View("Print", model);

            return View("Print", model);
        }
        [HttpPost]
        public ActionResult Edit(SettlementReg model)
        {
            SettlementRegTambahanBiaya[] res = JsonConvert.DeserializeObject<SettlementRegTambahanBiaya[]>(model.StrBiayaTambahan);
            model.ListBiayaTambahan = res.ToList();

            if (ModelState.IsValid)
            {
                Context.SettlementReguler dbitem = RepoSettlementReg.FindByPK(model.Id);
                model.SetDb(dbitem);
                RepoSettlementReg.save(dbitem, UserPrincipal.id);
                RepoAuditrail.saveDelAllSettlementRegulerBiayaTambahanQuery(dbitem, UserPrincipal.id);
                foreach (Context.SettlementRegulerTambahanBiaya srtb in dbitem.SettlementRegulerTambahanBiaya) {
                    RepoAuditrail.saveSettlementRegulerBiayaTambahanQuery(srtb, UserPrincipal.id);
                }

                return RedirectToAction("Index");
            }

            return View("Form", model);
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
                model = new AdminUangJalan(dbitem.AdminUangJalan, RepoAtm.FindAll(), RepoBor.FindAll());
            }

            var tempModel = model.ModelListRemoval.Where(d => d.Status == "dispatched");
            if (tempModel.Count() > 0){
                return new JavaScriptSerializer().Serialize(tempModel.OrderBy(d => d.Id).Last());
            }
            else
                return new JavaScriptSerializer().Serialize(model);
        }
    }
}