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
    public class KasirController : BaseController
    {
        private ISalesOrderRepo RepoSalesOrder;
        private IAtmRepo Repoatm;
        private IDataBoronganRepo RepoBor;
        private IRemovalRepo RepoRemoval;
        private ISalesOrderKontrakListSoRepo RepoSalesOrderKontrakListSo;
        private Iglt_detRepo Repoglt_det;
        private IERPConfigRepo RepoERPConfig;
        private Iac_mstrRepo Repoac_mstr;
        private Ibk_mstrRepo Repobk_mstr;
        private IAdminUangJalanRepo RepoAdminUangJalan;
        private IAuditrailRepo RepoAuditrail;
        private IDokumenRepo RepoDokumen;
        private IMasterPoolRepo RepoMasterPool;
        private ILookupCodeRepo RepoLookupCode;
        public KasirController(IUserReferenceRepo repoBase, ILookupCodeRepo repoLookup, ISalesOrderRepo repoSalesOrder, IAtmRepo repoatm, IDataBoronganRepo repoBor, IRemovalRepo repoRemoval,
            ISalesOrderKontrakListSoRepo repoSalesOrderKontrakListSo, Iglt_detRepo repoglt_det, IERPConfigRepo repoERPConfig, Iac_mstrRepo repoac_mstr, Ibk_mstrRepo repobk_mstr,
            IAdminUangJalanRepo repoAdminUangJalan, IAuditrailRepo repoAuditrail, IDokumenRepo repoDokumen, IMasterPoolRepo repoMasterPool, ILookupCodeRepo repoLookupCode)
            : base(repoBase, repoLookup)
        {
            RepoDokumen = repoDokumen;
            RepoLookup = repoLookup;
            RepoSalesOrder = repoSalesOrder;
            Repoatm = repoatm;
            RepoBor = repoBor;
            RepoRemoval = repoRemoval;
            RepoSalesOrderKontrakListSo = repoSalesOrderKontrakListSo;
            Repoglt_det = repoglt_det;
            RepoERPConfig = repoERPConfig;
            Repoac_mstr = repoac_mstr;
            Repobk_mstr = repobk_mstr;
            RepoAdminUangJalan = repoAdminUangJalan;
            RepoAuditrail = repoAuditrail;
            RepoMasterPool = repoMasterPool;
            RepoLookupCode = repoLookupCode;
        }
        [MyAuthorize(Menu = "Kasir Transfer", Action="read")]
        public ActionResult IndexTf()
        {
            ViewBag.listKolom = ListKolom.Where(d => d.Action == "Index" && d.Controller == "Kasir").ToList();
            return View("IndexTf");
        }
        [MyAuthorize(Menu = "Kasir Cash", Action="read")]
        public ActionResult IndexKas()
        {
            ViewBag.listKolom = ListKolom.Where(d => d.Action == "Index" && d.Controller == "Kasir").ToList();
            return View("IndexKas");
        }
        public bool Checking(int id){
            Context.SalesOrder so = RepoSalesOrder.FindByPK(id);
            return (so.SalesOrderKontrakId != null && so.SalesOrderKontrakId.Value != null || so.Status.ToLower() == "admin uang jalan" || so.Status.ToLower() == "dispatched");
        }

        public string BindingTf()
        {
            List<Context.SalesOrder> items = RepoSalesOrder.FindAllKasir();

            List<KasirTf> ListModel = new List<KasirTf>();
            foreach (Context.SalesOrder item in items)
            {
                if (item.SalesOrderKontrakId.HasValue)
                {
                    var data = item.SalesOrderKontrak.SalesOrderKontrakListSo.Where(p => p.IsProses && p.IdAdminUangJalan != null && (p.Status == "dispatched" || p.Status == "admin uang jalan") && p.AdminUangJalan.AdminUangJalanUangTf.Where(s => s.Keterangan != "Tunai").Any(n => n.isTf == false))
                        .GroupBy(d => new { d.IdDataTruck, d.Driver1Id, d.Status, d.Urutan }).Select(grp => grp.ToList());
                    foreach (var itemGroup in data.ToList())
                    {
                        ListModel.Add(new KasirTf(item, itemGroup));
                    }
                }
                else if (item.AdminUangJalanId.HasValue && item.AdminUangJalan.AdminUangJalanUangTf.Where(s => s.Keterangan != "Tunai" && s.Value > 0).Any(n => n.isTf == false))
                {
                    ListModel.Add(new KasirTf(item));
                }
            }

            List<Context.Removal> ItemsRemoval = RepoRemoval.FindAll().Where(d => d.Status == "dispatched" || d.Status == "admin uang jalan").ToList();
            foreach (Context.Removal item in ItemsRemoval)
            {
                ListModel.Add(new KasirTf(item));
            }
            ListModel = ListModel.Where(d => d.Jumlah > 0).ToList();
            return new JavaScriptSerializer().Serialize(new { total = items.Count, data = ListModel });
        }

        public string BindingKas()
        {
            List<Context.SalesOrder> items = RepoSalesOrder.FindAllKasir();

            List<Kasirkas> ListModel = new List<Kasirkas>();
            foreach (Context.SalesOrder item in items)
            {
                if (item.SalesOrderKontrakId.HasValue)
                {
                    var data = item.SalesOrderKontrak.SalesOrderKontrakListSo.Where(p => p.IsProses && (p.Status == "dispatched" || p.Status == "admin uang jalan") && p.AdminUangJalan.AdminUangJalanUangTf.Where(s => s.Keterangan == "Tunai").Any(n => n.isTf == false))
                        .GroupBy(d => new { d.IdDataTruck, d.Driver1Id, d.Status, d.Urutan }).Select(grp => grp.ToList());
                    foreach (var itemGroup in data.ToList())
                    {
                        ListModel.Add(new Kasirkas(item, itemGroup));
                    }
                }
                else if (item.AdminUangJalanId.HasValue && item.AdminUangJalan.AdminUangJalanUangTf.Where(s => s.Keterangan == "Tunai").Any(n => n.isTf == false))
                {
                    ListModel.Add(new Kasirkas(item));
                }
            }

            List<Context.Removal> ItemsRemoval = RepoRemoval.FindAll().Where(d => d.Status == "dispatched" || d.Status == "admin uang jalan").ToList();
            foreach (Context.Removal item in ItemsRemoval)
            {
                ListModel.Add(new Kasirkas(item));
            }

            ListModel = ListModel.Where(d => d.Jumlah > 0).ToList();
            return new JavaScriptSerializer().Serialize(new { total = ListModel.Count(), data = ListModel });
        }

        [MyAuthorize(Menu = "Kasir Transfer", Action="update")]
        public ActionResult EditTF(int id)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            //cari
            AdminUangJalan model = new AdminUangJalan();
            if (dbitem.AdminUangJalanId.HasValue)
                model = new AdminUangJalan(dbitem.AdminUangJalan, Repoatm.FindAll(), RepoBor.FindAll());

            model.IdSalesOrder = dbitem.Id;
            if (dbitem.SalesOrderOncallId.HasValue)
            {
                model.ModelOncall = new SalesOrderOncall(dbitem);
                if (model.IdDriver1.HasValue)
                {
                    model.IdDriver1 = model.IdDriver1;
                    model.NamaDriver1 = model.NamaDriver1;
                    model.KeteranganGanti1 = model.KeteranganGanti1;
                }
                else
                {
                    model.IdDriver1 = model.ModelOncall.Driver1Id;
                    model.NamaDriver1 = model.ModelOncall.KodeDriver1 + " - " + model.ModelOncall.NamaDriver1;
                    model.KeteranganGanti1 = model.ModelOncall.KeteranganDriver1;
                }
                if (model.IdDriver2.HasValue)
                {
                    model.IdDriver2 = model.IdDriver2;
                    model.NamaDriver2 = model.NamaDriver2;
                    model.KeteranganGanti2 = model.KeteranganGanti2;
                }
                else
                {
                    model.IdDriver2 = model.ModelOncall.Driver2Id;
                    model.NamaDriver2 = model.ModelOncall.KodeDriver2 + " - " + model.ModelOncall.NamaDriver2;
                    model.KeteranganGanti2 = model.ModelOncall.KeteranganDriver2;
                }
                ViewBag.driverId = dbitem.SalesOrderOncall.Driver1Id;
                ViewBag.driverName = dbitem.SalesOrderOncall.Driver1.KodeDriver + " - " + dbitem.SalesOrderOncall.Driver1.NamaDriver;
                ViewBag.Title = "Kasir Transfer " + dbitem.SalesOrderOncall.SONumber;
                ViewBag.KeteranganAdmin = model.KeteranganAdmin;
                //ViewBag.postData = "EditOncall";
                return View("FormTf", model);
            }
            else if (dbitem.SalesOrderPickupId.HasValue)
            {
                model.ModelPickup = new SalesOrderPickup(dbitem);
                if (model.IdDriver1.HasValue)
                {
                    model.IdDriver1 = model.IdDriver1;
                    model.NamaDriver1 = model.NamaDriver1;
                    model.KeteranganGanti1 = model.KeteranganGanti1;
                }
                else
                {
                    model.IdDriver1 = model.ModelPickup.Driver1Id;
                    model.NamaDriver1 = model.ModelPickup.KodeDriver1 + " - " + model.ModelPickup.NamaDriver1;
                    model.KeteranganGanti1 = model.ModelPickup.KeteranganDriver1;
                }
                if (model.IdDriver2.HasValue)
                {
                    model.IdDriver2 = model.IdDriver2;
                    model.NamaDriver2 = model.NamaDriver2;
                    model.KeteranganGanti2 = model.KeteranganGanti2;
                }
                else
                {
                    model.IdDriver2 = model.ModelPickup.Driver2Id;
                    model.NamaDriver2 = model.ModelPickup.KodeDriver2 + " - " + model.ModelPickup.NamaDriver2;
                    model.KeteranganGanti2 = model.ModelPickup.KeteranganDriver2;
                }
                ViewBag.driverId = dbitem.SalesOrderPickup.Driver1Id;
                ViewBag.driverName = dbitem.SalesOrderPickup.Driver1.KodeDriver + " - " + dbitem.SalesOrderPickup.Driver1.NamaDriver;
                ViewBag.Title = "Kasir Transfer " + dbitem.SalesOrderPickup.SONumber;
                //ViewBag.postData = "EditPickup";
                return View("FormTf", model);
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue)
            {
                model.ModelKonsolidasi = new SalesOrderProsesKonsolidasi(dbitem);
                if (model.IdDriver1.HasValue)
                {
                    model.IdDriver1 = model.IdDriver1;
                    model.NamaDriver1 = model.NamaDriver1;
                    model.KeteranganGanti1 = model.KeteranganGanti1;
                }
                else
                {
                    model.IdDriver1 = model.ModelKonsolidasi.Driver1Id;
                    model.NamaDriver1 = model.ModelKonsolidasi.KodeDriver1 + " - " + model.ModelKonsolidasi.NamaDriver1;
                    model.KeteranganGanti1 = model.ModelKonsolidasi.KeteranganDriver1;
                }
                if (model.IdDriver2.HasValue)
                {
                    model.IdDriver2 = model.IdDriver2;
                    model.NamaDriver2 = model.NamaDriver2;
                    model.KeteranganGanti2 = model.KeteranganGanti2;
                }
                else
                {
                    model.IdDriver2 = model.ModelKonsolidasi.Driver2Id;
                    model.NamaDriver2 = model.ModelKonsolidasi.KodeDriver2 + " - " + model.ModelKonsolidasi.NamaDriver2;
                    model.KeteranganGanti2 = model.ModelKonsolidasi.KeteranganDriver2;
                }
                ViewBag.driverId = dbitem.SalesOrderProsesKonsolidasi.Driver1Id;
                ViewBag.driverName = dbitem.SalesOrderProsesKonsolidasi.Driver1.KodeDriver + " - " + dbitem.SalesOrderProsesKonsolidasi.Driver1.NamaDriver;
                ViewBag.Title = "Kasir Transfer" + dbitem.SalesOrderProsesKonsolidasi.SONumber;
                //ViewBag.postData = "EditPickup";
                return View("FormTf", model);
            }
            else if (dbitem.SalesOrderKontrakId.HasValue)
            {

            }

            return View();
        }
        public ActionResult EditTFRemoval(int id)
        {
            Context.Removal dbitem = RepoRemoval.FindByPK(id);
            RemovalAUJ model = new RemovalAUJ(dbitem, Repoatm.FindAll(), RepoBor.FindAll());

            if (dbitem.SalesOrder.SalesOrderOncallId.HasValue)
            {
                ViewBag.Title = "Kasir Transfer " + dbitem.SalesOrder.SalesOrderOncall.SONumber;
            }
            else if (dbitem.SalesOrder.SalesOrderPickupId.HasValue)
            {
                ViewBag.Title = "Kasir Transfer " + dbitem.SalesOrder.SalesOrderPickup.SONumber;
            }
            else if (dbitem.SalesOrder.SalesOrderProsesKonsolidasiId.HasValue)
            {
                ViewBag.Title = "Kasir Transfer " + dbitem.SalesOrder.SalesOrderProsesKonsolidasi.SONumber;
            }
            return View("FormTfRemoval", model);
        }
        [MyAuthorize(Menu = "Kasir Transfer", Action="update")]
        public ActionResult EditTfKontrak(int id, string listSo)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            //ambil admin uang jalan nya dari listSo yang pertama
            List<int> ListIdDumy = listSo.Split(new string[] { "." }, StringSplitOptions.None).ToList().Select(int.Parse).ToList();
            List<Context.SalesOrderKontrakListSo> dbsoDummy = dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => ListIdDumy.Contains(d.Id)).ToList();
            dbitem.SalesOrderKontrak.SalesOrderKontrakListSo = dbsoDummy;
            AdminUangJalan model = new AdminUangJalan();

            if (dbsoDummy.FirstOrDefault().IdAdminUangJalan.HasValue)
                model = new AdminUangJalan(dbsoDummy.FirstOrDefault().AdminUangJalan, Repoatm.FindAll(), RepoBor.FindAll());

            model.IdSalesOrder = dbitem.Id;
            model.ModelKontrak = new SalesOrderKontrak(dbitem);
            model.ModelKontrak.ListValueModelSOKontrak = new List<SalesOrderKontrakListSo>();
            model.ListIdSo = listSo;
            foreach (var item in dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.ToList())
            {
                model.ModelKontrak.ListValueModelSOKontrak.Add(new SalesOrderKontrakListSo(item));
            }

            return View("FormTf", model);
        }
        [HttpPost]
        public ActionResult EditTf(AdminUangJalan model)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(model.IdSalesOrder.Value);
            Context.AdminUangJalan db = dbitem.AdminUangJalan;
            Context.ERPConfig erpConfig = RepoERPConfig.FindByFrist();
            if (dbitem.Status.ToLower().Contains("dispatched") || dbitem.Status.ToLower().Contains("admin uang jalan"))
            {
                AdminUangJalanUangTf[] resUang = JsonConvert.DeserializeObject<AdminUangJalanUangTf[]>(model.StrUang);
                model.ModelListTf = resUang.ToList();
                int idx = 1;
                if (dbitem.PendapatanDiakui != true){
                    Repoglt_det.saveFromAc(1, "PD-" + dbitem.SalesOrderOncall.SONumber, RepoSalesOrder.Harga(dbitem), 0, Repoac_mstr.FindByPk(erpConfig.IdPiutangDagang));
                    Repoglt_det.saveFromAc(2, "PD-" + dbitem.SalesOrderOncall.SONumber, 0, RepoSalesOrder.Harga(dbitem), Repoac_mstr.FindByPk(erpConfig.IdPendapatanUsahaBlmInv));
                    dbitem.PendapatanDiakui = true;
                    decimal nominalHutangUangJalanDriver = 0;
                    foreach (var item in model.ModelListTf)
                    {
                        if (item.isTf != true){
                            nominalHutangUangJalanDriver += item.JumlahTransfer.Value;
                        }
                    }
                    nominalHutangUangJalanDriver += decimal.Parse(db.AdminUangJalanVoucherSpbu.Select(d => d.Value).Sum().ToString());
                    nominalHutangUangJalanDriver += decimal.Parse(db.AdminUangJalanVoucherKapal.Select(d => d.Value).Sum().ToString());
                    nominalHutangUangJalanDriver += db.KasbonDriver1 == null ? 0 : db.KasbonDriver1.Value;
                    nominalHutangUangJalanDriver += db.KlaimDriver1 == null ? 0 : db.KlaimDriver1.Value;
                    nominalHutangUangJalanDriver += decimal.Parse(db.AdminUangJalanPotonganDriver.Select(d => d.Value).Sum().ToString());

                    Repoglt_det.saveFromAc(1, "KT-" + dbitem.SalesOrderOncall.SONumber, nominalHutangUangJalanDriver, 0, Repoac_mstr.FindByPk(erpConfig.IdAUJCredit));//Hutang Uang Jalan Driver
                    foreach (var item in model.ModelListTf)
                    {
                        if (item.isTf != true){
                            idx++;
                            Repoglt_det.saveFromAc(idx, "KT-" + dbitem.SalesOrderOncall.SONumber, 0, item.JumlahTransfer.Value, Repoac_mstr.FindByPk(item.IdCreditTf));//BCA Audy 386-7957777 atau
                        }
                    }
                    foreach (Context.AdminUangJalanVoucherSpbu aujvs in db.AdminUangJalanVoucherSpbu){
                        idx++;
                        Repoglt_det.saveFromAc(idx, "KT-" + dbitem.SalesOrderOncall.SONumber, 0, aujvs.Value, Repoac_mstr.FindByPk(RepoLookup.FindByName(aujvs.Keterangan).ac_id));//HUTANG SPBU 34.171.04 Pangkalan 2 dan atau
                    }
                    foreach (Context.AdminUangJalanVoucherKapal aujvs in db.AdminUangJalanVoucherKapal){
                        idx++;
                        Repoglt_det.saveFromAc(idx, "KT-" + dbitem.SalesOrderOncall.SONumber, 0, aujvs.Value, Repoac_mstr.FindByPk(RepoLookup.FindByName(aujvs.Keterangan).ac_id));//HUTANG SPBU 34.171.04 Pangkalan 2 dan atau
                    }
                    if (db.PotonganB > 0){
                        idx++;
                        Repoglt_det.saveFromAc(idx, "KT-" + dbitem.SalesOrderOncall.SONumber, 0, db.PotonganB, Repoac_mstr.FindByPk(erpConfig.IdPiutangDriverBatalJalan));//PIUTANG DRIVER BATAL JALAN / SEMENTARA TUNAI
                    }
                    if (db.PotonganP > 0){
                        idx++;
                        Repoglt_det.saveFromAc(idx, "KT-" + dbitem.SalesOrderOncall.SONumber, 0, db.PotonganP, Repoac_mstr.FindByPk(erpConfig.IdPiutangDriverPribadi));//PIUTANG DRIVER BATAL JALAN / SEMENTARA TUNAI
                    }
                    if (db.PotonganK > 0){
                        idx++;
                        Repoglt_det.saveFromAc(idx, "KT-" + dbitem.SalesOrderOncall.SONumber, 0, db.PotonganK, Repoac_mstr.FindByPk(erpConfig.IdPendapatanPengembalianPiutangDriver));
                    }
                    if (db.PotonganT > 0){
                        idx++;
                        Repoglt_det.saveFromAc(idx, "KT-" + dbitem.SalesOrderOncall.SONumber, 0, db.PotonganT, Repoac_mstr.FindByPk(erpConfig.IdTabunganDriver));
                    }
                    dbitem.PendapatanDiakui = true;
                }
                else{
                    foreach (var item in model.ModelListTf)
                    {
                        if (item.JumlahTransfer.Value > 0){
                            if (item.isTf != true){
                                Repoglt_det.saveFromAc(idx, "KT-" + dbitem.SalesOrderOncall.SONumber, 0, item.JumlahTransfer.Value, Repoac_mstr.FindByPk(item.IdCreditTf));//BCA Audy 386-7957777 atau
                                idx++;
                                Repoglt_det.saveFromAc(idx, "KT-" + dbitem.SalesOrderOncall.SONumber, item.JumlahTransfer.Value, 0, Repoac_mstr.FindByPk(erpConfig.IdAUJCredit));//Hutang Uang Jalan Driver
                            }
                        }
                    }
                }
                foreach (var item in model.ModelListTf)
                {
                    if (item.JumlahTransfer.Value > 0){
                        item.isTf = true;
                    }
                    else
                        item.isTf = false;
                    item.setDb(dbitem.AdminUangJalan.AdminUangJalanUangTf.Where(d => d.Id == item.Id).FirstOrDefault());
                }
                dbitem.Status = "dispatched";
                dbitem.UpdatedBy = UserPrincipal.id;
                RepoSalesOrder.save(dbitem);
                string strQuery = "";
                if (RepoDokumen.FindBySO(dbitem.Id) == null){
                    if(dbitem.SalesOrderOncallId.HasValue)
                        strQuery += GenerateDokumen(dbitem.Id, dbitem.SalesOrderOncall.Customer);
                    else if (dbitem.SalesOrderPickupId.HasValue)
                        strQuery += GenerateDokumen(dbitem.Id, dbitem.SalesOrderPickup.Customer);
                    else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue)
                    {
                        foreach (var item in dbitem.SalesOrderProsesKonsolidasi.SalesOrderProsesKonsolidasiItem)
                        {
                            strQuery += GenerateDokumen(dbitem.Id, item.SalesOrderKonsolidasi.Customer, "", item.SalesOrderKonsolidasi.StrDaftarHargaItem);
                        }
                    }
                }
                RepoAuditrail.saveKasirTfHistory(dbitem, strQuery);
                return RedirectToAction("IndexTf");
            }
            if (dbitem.AdminUangJalanId.HasValue)
                model = new AdminUangJalan(dbitem.AdminUangJalan, Repoatm.FindAll(), RepoBor.FindAll());

            ViewBag.Status = "batal";
            model.IdSalesOrder = dbitem.Id;
            if (dbitem.SalesOrderOncallId.HasValue)
            {
                model.ModelOncall = new SalesOrderOncall(dbitem);
                if (model.IdDriver1.HasValue)
                {
                    model.IdDriver1 = model.IdDriver1;
                    model.NamaDriver1 = model.NamaDriver1;
                    model.KeteranganGanti1 = model.KeteranganGanti1;
                }
                else
                {
                    model.IdDriver1 = model.ModelOncall.Driver1Id;
                    model.NamaDriver1 = model.ModelOncall.KodeDriver1 + " - " + model.ModelOncall.NamaDriver1;
                    model.KeteranganGanti1 = model.ModelOncall.KeteranganDriver1;
                }
                if (model.IdDriver2.HasValue)
                {
                    model.IdDriver2 = model.IdDriver2;
                    model.NamaDriver2 = model.NamaDriver2;
                    model.KeteranganGanti2 = model.KeteranganGanti2;
                }
                else
                {
                    model.IdDriver2 = model.ModelOncall.Driver2Id;
                    model.NamaDriver2 = model.ModelOncall.KodeDriver2 + " - " + model.ModelOncall.NamaDriver2;
                    model.KeteranganGanti2 = model.ModelOncall.KeteranganDriver2;
                }
                ViewBag.Title = "Kasir Transfer " + dbitem.SalesOrderOncall.SONumber;
                //ViewBag.postData = "EditOncall";
                return View("FormTf", model);
            }
            else if (dbitem.SalesOrderPickupId.HasValue)
            {
                model.ModelPickup = new SalesOrderPickup(dbitem);
                if (model.IdDriver1.HasValue)
                {
                    model.IdDriver1 = model.IdDriver1;
                    model.NamaDriver1 = model.NamaDriver1;
                    model.KeteranganGanti1 = model.KeteranganGanti1;
                }
                else
                {
                    model.IdDriver1 = model.ModelPickup.Driver1Id;
                    model.NamaDriver1 = model.ModelPickup.KodeDriver1 + " - " + model.ModelPickup.NamaDriver1;
                    model.KeteranganGanti1 = model.ModelPickup.KeteranganDriver1;
                }
                if (model.IdDriver2.HasValue)
                {
                    model.IdDriver2 = model.IdDriver2;
                    model.NamaDriver2 = model.NamaDriver2;
                    model.KeteranganGanti2 = model.KeteranganGanti2;
                }
                else
                {
                    model.IdDriver2 = model.ModelPickup.Driver2Id;
                    model.NamaDriver2 = model.ModelPickup.KodeDriver2 + " - " + model.ModelPickup.NamaDriver2;
                    model.KeteranganGanti2 = model.ModelPickup.KeteranganDriver2;
                }
                ViewBag.Title = "Kasir Transfer " + dbitem.SalesOrderPickup.SONumber;
                //ViewBag.postData = "EditPickup";
                return View("FormTf", model);
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue)
            {
                model.ModelKonsolidasi = new SalesOrderProsesKonsolidasi(dbitem);
                if (model.IdDriver1.HasValue)
                {
                    model.IdDriver1 = model.IdDriver1;
                    model.NamaDriver1 = model.NamaDriver1;
                    model.KeteranganGanti1 = model.KeteranganGanti1;
                }
                else
                {
                    model.IdDriver1 = model.ModelKonsolidasi.Driver1Id;
                    model.NamaDriver1 = model.ModelKonsolidasi.KodeDriver1 + " - " + model.ModelKonsolidasi.NamaDriver1;
                    model.KeteranganGanti1 = model.ModelKonsolidasi.KeteranganDriver1;
                }
                if (model.IdDriver2.HasValue)
                {
                    model.IdDriver2 = model.IdDriver2;
                    model.NamaDriver2 = model.NamaDriver2;
                    model.KeteranganGanti2 = model.KeteranganGanti2;
                }
                else
                {
                    model.IdDriver2 = model.ModelKonsolidasi.Driver2Id;
                    model.NamaDriver2 = model.ModelKonsolidasi.KodeDriver2 + " - " + model.ModelKonsolidasi.NamaDriver2;
                    model.KeteranganGanti2 = model.ModelKonsolidasi.KeteranganDriver2;
                }
                ViewBag.Title = "Kasir Transfer" + dbitem.SalesOrderProsesKonsolidasi.SONumber;
                //ViewBag.postData = "EditPickup";
                return View("FormTf", model);
            }
            return View("FormTf", model);
        }
        [HttpPost]
        public ActionResult EditTfRemoval(RemovalAUJ model)
        {
            Context.Removal dbitem = RepoRemoval.FindByPK(model.Id);
            if (dbitem.Status.ToLower().Contains("dispatched") || dbitem.Status.ToLower().Contains("admin uang jalan"))
            {
                AdminUangJalanUangTf[] resUang = JsonConvert.DeserializeObject<AdminUangJalanUangTf[]>(model.StrUang);
                model.ModelListTf = resUang.ToList();
                foreach (var item in model.ModelListTf)
                {
                    if (item.JumlahTransfer.Value > 0){
                        item.isTf = true;
                        item.Code = "RT" + item.Id.ToString() + "-" + model.Id;

                        //lebah dieu sync ERPna
                        Context.ERPConfig erpConfig = RepoERPConfig.FindByFrist();
                        if (dbitem.StatusTagihan == "Ditagih")
                            Repoglt_det.saveFromAc(1, item.Code, item.JumlahTransfer.Value, 0, Repoac_mstr.FindByPk(erpConfig.IdPiutangCustomer));//D Piutang Customer
                        else
                            Repoglt_det.saveFromAc(1, item.Code, item.JumlahTransfer.Value, 0, Repoac_mstr.FindByPk(erpConfig.IdBiayaPerjalanan));//D Biaya Uang Jalan
                        Repoglt_det.saveFromAc(2, item.Code, 0, item.JumlahTransfer.Value, Repoac_mstr.FindByPk(item.IdCreditTf));//K Transfer Bank
                    }
                    else
                        item.isTf = false;
                    item.setDb(dbitem.RemovalUangTf.Where(d => d.Id == item.Id).FirstOrDefault());
                }
                dbitem.Status = "dispatched";
                RepoRemoval.save(dbitem);
                return RedirectToAction("IndexTf");
            }

            model = new RemovalAUJ(dbitem, Repoatm.FindAll(), RepoBor.FindAll());

            if (dbitem.SalesOrder.SalesOrderOncallId.HasValue)
            {
                ViewBag.Title = "Kasir Transfer " + dbitem.SalesOrder.SalesOrderOncall.SONumber;
            }
            else if (dbitem.SalesOrder.SalesOrderPickupId.HasValue)
            {
                ViewBag.Title = "Kasir Transfer " + dbitem.SalesOrder.SalesOrderPickup.SONumber;
            }
            else if (dbitem.SalesOrder.SalesOrderProsesKonsolidasiId.HasValue)
            {
                ViewBag.Title = "Kasir Transfer " + dbitem.SalesOrder.SalesOrderProsesKonsolidasi.SONumber;
            }
            return View("FormTfRemoval", model);
        }
        [HttpPost]
        public ActionResult EditTfKontrak(AdminUangJalan model)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(model.IdSalesOrder.Value);
            List<int> ListIdDumy = model.ListIdSo.Split(new string[] { "." }, StringSplitOptions.None).ToList().Select(int.Parse).ToList();
            List<Context.SalesOrderKontrakListSo> dbsoDummy = dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => ListIdDumy.Contains(d.Id)).ToList();

            AdminUangJalanUangTf[] resUang = JsonConvert.DeserializeObject<AdminUangJalanUangTf[]>(model.StrUang);
            model.ModelListTf = resUang.ToList();
            foreach (var item in model.ModelListTf)
            {
                if (item.TanggalAktual.HasValue)
                    item.isTf = true;
                else
                    item.isTf = false;
                item.setDb(dbsoDummy.FirstOrDefault().AdminUangJalan.AdminUangJalanUangTf.Where(d => d.Id == item.Id).FirstOrDefault());
            }
            foreach (Context.SalesOrderKontrakListSo sokls in dbsoDummy)
            {
                Context.SalesOrderKontrakListSo sokls1 = RepoSalesOrderKontrakListSo.FindByPK(sokls.Id);
                sokls1.Status = "dispatched";
                RepoSalesOrderKontrakListSo.save(sokls1);
            }

            foreach (var item in dbsoDummy)
            {
                GenerateDokumen(dbitem.Id, dbitem.SalesOrderKontrak.Customer, item.Id.ToString());
            }

            RepoSalesOrder.save(dbitem);
            return RedirectToAction("IndexTf");
        }
        [MyAuthorize(Menu = "Kasir Transfer", Action="read")]
        public ActionResult ViewTF(int id)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            //cari
            AdminUangJalan model = new AdminUangJalan();
            if (dbitem.AdminUangJalanId.HasValue)
                model = new AdminUangJalan(dbitem.AdminUangJalan, Repoatm.FindAll(), RepoBor.FindAll());

            model.IdSalesOrder = dbitem.Id;
            if (dbitem.SalesOrderOncallId.HasValue)
            {
                model.ModelOncall = new SalesOrderOncall(dbitem);
                if (model.IdDriver1.HasValue)
                {
                    model.IdDriver1 = model.IdDriver1;
                    model.NamaDriver1 = model.NamaDriver1;
                    model.KeteranganGanti1 = model.KeteranganGanti1;
                }
                else
                {
                    model.IdDriver1 = model.ModelOncall.Driver1Id;
                    model.NamaDriver1 = model.ModelOncall.KodeDriver1 + " - " + model.ModelOncall.NamaDriver1;
                    model.KeteranganGanti1 = model.ModelOncall.KeteranganDriver1;
                }
                if (model.IdDriver2.HasValue)
                {
                    model.IdDriver2 = model.IdDriver2;
                    model.NamaDriver2 = model.NamaDriver2;
                    model.KeteranganGanti2 = model.KeteranganGanti2;
                }
                else
                {
                    model.IdDriver2 = model.ModelOncall.Driver2Id;
                    model.NamaDriver2 = model.ModelOncall.KodeDriver2 + " - " + model.ModelOncall.NamaDriver2;
                    model.KeteranganGanti2 = model.ModelOncall.KeteranganDriver2;
                }
                ViewBag.Title = "Kasir Transfer " + dbitem.SalesOrderOncall.SONumber;
                //ViewBag.postData = "EditOncall";
                return View("FormTfView", model);
            }
            else if (dbitem.SalesOrderPickupId.HasValue)
            {
                model.ModelPickup = new SalesOrderPickup(dbitem);
                if (model.IdDriver1.HasValue)
                {
                    model.IdDriver1 = model.IdDriver1;
                    model.NamaDriver1 = model.NamaDriver1;
                    model.KeteranganGanti1 = model.KeteranganGanti1;
                }
                else
                {
                    model.IdDriver1 = model.ModelPickup.Driver1Id;
                    model.NamaDriver1 = model.ModelPickup.KodeDriver1 + " - " + model.ModelPickup.NamaDriver1;
                    model.KeteranganGanti1 = model.ModelPickup.KeteranganDriver1;
                }
                if (model.IdDriver2.HasValue)
                {
                    model.IdDriver2 = model.IdDriver2;
                    model.NamaDriver2 = model.NamaDriver2;
                    model.KeteranganGanti2 = model.KeteranganGanti2;
                }
                else
                {
                    model.IdDriver2 = model.ModelPickup.Driver2Id;
                    model.NamaDriver2 = model.ModelPickup.KodeDriver2 + " - " + model.ModelPickup.NamaDriver2;
                    model.KeteranganGanti2 = model.ModelPickup.KeteranganDriver2;
                }
                ViewBag.Title = "Kasir Transfer " + dbitem.SalesOrderPickup.SONumber;
                //ViewBag.postData = "EditPickup";
                return View("FormTfView", model);
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue)
            {
                model.ModelKonsolidasi = new SalesOrderProsesKonsolidasi(dbitem);
                if (model.IdDriver1.HasValue)
                {
                    model.IdDriver1 = model.IdDriver1;
                    model.NamaDriver1 = model.NamaDriver1;
                    model.KeteranganGanti1 = model.KeteranganGanti1;
                }
                else
                {
                    model.IdDriver1 = model.ModelKonsolidasi.Driver1Id;
                    model.NamaDriver1 = model.ModelKonsolidasi.KodeDriver1 + " - " + model.ModelKonsolidasi.NamaDriver1;
                    model.KeteranganGanti1 = model.ModelKonsolidasi.KeteranganDriver1;
                }
                if (model.IdDriver2.HasValue)
                {
                    model.IdDriver2 = model.IdDriver2;
                    model.NamaDriver2 = model.NamaDriver2;
                    model.KeteranganGanti2 = model.KeteranganGanti2;
                }
                else
                {
                    model.IdDriver2 = model.ModelKonsolidasi.Driver2Id;
                    model.NamaDriver2 = model.ModelKonsolidasi.KodeDriver2 + " - " + model.ModelKonsolidasi.NamaDriver2;
                    model.KeteranganGanti2 = model.ModelKonsolidasi.KeteranganDriver2;
                }
                ViewBag.Title = "Kasir Transfer" + dbitem.SalesOrderProsesKonsolidasi.SONumber;
                //ViewBag.postData = "EditPickup";
                return View("FormTfView", model);
            }

            return View("");
        }
        public ActionResult ViewTFRemoval(int id)
        {
            Context.Removal dbitem = RepoRemoval.FindByPK(id);
            RemovalAUJ model = new RemovalAUJ(dbitem, Repoatm.FindAll(), RepoBor.FindAll());

            if (dbitem.SalesOrder.SalesOrderOncallId.HasValue)
            {
                ViewBag.Title = "Kasir Transfer " + dbitem.SalesOrder.SalesOrderOncall.SONumber;
            }
            else if (dbitem.SalesOrder.SalesOrderPickupId.HasValue)
            {
                ViewBag.Title = "Kasir Transfer " + dbitem.SalesOrder.SalesOrderPickup.SONumber;
            }
            else if (dbitem.SalesOrder.SalesOrderProsesKonsolidasiId.HasValue)
            {
                ViewBag.Title = "Kasir Transfer " + dbitem.SalesOrder.SalesOrderProsesKonsolidasi.SONumber;
            }
            return View("FormTfRemovalView", model);
        }
        [MyAuthorize(Menu = "Kasir Transfer", Action="read")]
        public ActionResult ViewTFKontrak(int id, string listSo)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            //ambil admin uang jalan nya dari listSo yang pertama
            List<int> ListIdDumy = listSo.Split(new string[] { "." }, StringSplitOptions.None).ToList().Select(int.Parse).ToList();
            List<Context.SalesOrderKontrakListSo> dbsoDummy = dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => ListIdDumy.Contains(d.Id)).ToList();
            dbitem.SalesOrderKontrak.SalesOrderKontrakListSo = dbsoDummy;
            AdminUangJalan model = new AdminUangJalan();

            if (dbsoDummy.FirstOrDefault().IdAdminUangJalan.HasValue)
                model = new AdminUangJalan(dbsoDummy.FirstOrDefault().AdminUangJalan, Repoatm.FindAll(), RepoBor.FindAll());

            model.IdSalesOrder = dbitem.Id;
            model.ModelKontrak = new SalesOrderKontrak(dbitem);
            model.ModelKontrak.ListValueModelSOKontrak = new List<SalesOrderKontrakListSo>();
            model.ListIdSo = listSo;
            foreach (var item in dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.ToList())
            {
                model.ModelKontrak.ListValueModelSOKontrak.Add(new SalesOrderKontrakListSo(item));
            }

            return View("FormTfView", model);
        }
        [MyAuthorize(Menu = "Kasir Cash", Action="update")]
        public ActionResult EditKas(int id)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            //cari
            AdminUangJalan model = new AdminUangJalan();
            if (dbitem.AdminUangJalanId.HasValue)
                model = new AdminUangJalan(dbitem.AdminUangJalan, Repoatm.FindAll(), RepoBor.FindAll());
            ViewBag.IpAddress = AppHelper.GetIPAddress();

            model.IdSalesOrder = dbitem.Id;
            GenerateModel(dbitem, model);

            return View("FormKas", model);
        }
        [MyAuthorize(Menu = "Kasir Cash", Action="update")]
        public ActionResult EditKasRemoval(int id)
        {
            Context.Removal dbitem = RepoRemoval.FindByPK(id);
            RemovalAUJ model = new RemovalAUJ(dbitem, Repoatm.FindAll(), RepoBor.FindAll());

            return View("FormKasRemoval", model);
        }
        [MyAuthorize(Menu = "Kasir Cash", Action="update")]
        public ActionResult EditKasKontrak(int id, string listSo)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            //cari
            //ambil admin uang jalan nya dari listSo yang pertama
            List<int> ListIdDumy = listSo.Split(new string[] { "." }, StringSplitOptions.None).ToList().Select(int.Parse).ToList();
            List<Context.SalesOrderKontrakListSo> dbsoDummy = dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => ListIdDumy.Contains(d.Id)).ToList();
            dbitem.SalesOrderKontrak.SalesOrderKontrakListSo = dbsoDummy;
            AdminUangJalan model = new AdminUangJalan();
            if (dbsoDummy.FirstOrDefault().AdminUangJalan != null)
                model = new AdminUangJalan(dbsoDummy.FirstOrDefault().AdminUangJalan, Repoatm.FindAll(), RepoBor.FindAll());

            model.IdSalesOrder = dbitem.Id;
            model.ModelKontrak = new SalesOrderKontrak(dbitem);
            model.ModelKontrak.ListValueModelSOKontrak = new List<SalesOrderKontrakListSo>();
            model.ListIdSo = listSo;
            foreach (var item in dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.ToList())
            {
                model.ModelKontrak.ListValueModelSOKontrak.Add(new SalesOrderKontrakListSo(item));
            }
            GenerateModel(dbitem, model);

            return View("FormKas", model);
        }
        [HttpPost]
        public ActionResult EditKas(AdminUangJalan model)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(model.IdSalesOrder.Value);
            Context.AdminUangJalan db = dbitem.AdminUangJalan;
            if (dbitem.Status.ToLower().Contains("dispatched") || dbitem.Status.ToLower().Contains("admin uang jalan")) {
                for (int i = 0; i < model.ModelListTf.Count(); i++) {
                    if (model.ModelListTf[i].Nama == "Tunai") {
                        if (validationManual(model)) {
                            Context.AdminUangJalanUangTf dbtf = dbitem.AdminUangJalan.AdminUangJalanUangTf.Where(d => d.Keterangan == "Tunai").FirstOrDefault();
                            if (dbtf.JumlahTransfer != null && dbtf.JumlahTransfer != 0)
                                dbtf.JumlahTransfer = model.ModelListTf[i].JumlahTransfer;
                            else
                                dbtf.JumlahTransfer = model.ModelListTf[i].Value;
                            dbtf.TanggalAktual = model.ModelListTf[i].TanggalAktual;
                            dbtf.JamAktual = model.ModelListTf[i].JamAktual;
                            dbtf.KeteranganTf = model.ModelListTf[i].KeteranganTf;
                            dbtf.IdDriverPenerima = model.ModelListTf[i].IdDriverPenerima;
                            int? urutan = RepoAdminUangJalan.getUrutanUang(dbtf.TanggalAktual.Value)+1;
                            dbtf.Urutan = urutan;
                            decimal nominalHutangUangJalanDriver = dbtf.JumlahTransfer.Value;
                            int idx = 3;
                            Context.ERPConfig erpConfig = RepoERPConfig.FindByFrist();
                            if (dbitem.PendapatanDiakui != true){
                                Repoglt_det.saveFromAc(1, "PD-" + db.Code, RepoSalesOrder.Harga(dbitem), 0, Repoac_mstr.FindByPk(erpConfig.IdPiutangDagang));
                                Repoglt_det.saveFromAc(2, "PD-" + db.Code, 0, RepoSalesOrder.Harga(dbitem), Repoac_mstr.FindByPk(erpConfig.IdPendapatanUsahaBlmInv));
                                dbitem.PendapatanDiakui = true;
                                nominalHutangUangJalanDriver += decimal.Parse(db.AdminUangJalanVoucherSpbu.Select(d => d.Value).Sum().ToString());
                                nominalHutangUangJalanDriver += decimal.Parse(db.AdminUangJalanVoucherKapal.Select(d => d.Value).Sum().ToString());
                                nominalHutangUangJalanDriver += db.KasbonDriver1 == null ? 0 : db.KasbonDriver1.Value;
                                nominalHutangUangJalanDriver += db.KlaimDriver1 == null ? 0 : db.KlaimDriver1.Value;
                                nominalHutangUangJalanDriver += decimal.Parse(db.AdminUangJalanPotonganDriver.Select(d => d.Value).Sum().ToString());

                                foreach (Context.AdminUangJalanVoucherSpbu aujvs in db.AdminUangJalanVoucherSpbu){
                                    idx++;
                                    Repoglt_det.saveFromAc(idx, "KK-" + dbitem.SalesOrderOncall.SONumber, 0, aujvs.Value, Repoac_mstr.FindByPk(RepoLookup.FindByName(aujvs.Keterangan).ac_id));//HUTANG SPBU 34.171.04 Pangkalan 2 dan atau
                                }
                                foreach (Context.AdminUangJalanVoucherKapal aujvs in db.AdminUangJalanVoucherKapal){
                                    idx++;
                                    Repoglt_det.saveFromAc(idx, "KK-" + dbitem.SalesOrderOncall.SONumber, 0, aujvs.Value, Repoac_mstr.FindByPk(RepoLookup.FindByName(aujvs.Keterangan).ac_id));//HUTANG SPBU 34.171.04 Pangkalan 2 dan atau
                                }
                                if (db.PotonganB > 0){
                                    idx++;
                                    Repoglt_det.saveFromAc(idx, "KK-" + dbitem.SalesOrderOncall.SONumber, 0, db.PotonganB, Repoac_mstr.FindByPk(erpConfig.IdPiutangDriverBatalJalan));//PIUTANG DRIVER BATAL JALAN
                                }
                                if (db.PotonganP > 0){
                                    idx++;
                                    Repoglt_det.saveFromAc(idx, "KK-" + dbitem.SalesOrderOncall.SONumber, 0, db.PotonganP, Repoac_mstr.FindByPk(erpConfig.IdPiutangDriverPribadi));//PIUTANG DRIVER BATAL JALAN / SEMENTARA TUNAI
                                }
                                if (db.PotonganK > 0){
                                    idx++;
                                    Repoglt_det.saveFromAc(idx, "KK-" + dbitem.SalesOrderOncall.SONumber, 0, db.PotonganK, Repoac_mstr.FindByPk(erpConfig.IdPendapatanPengembalianPiutangDriver));
                                }
                                if (db.PotonganT > 0){
                                    idx++;
                                    Repoglt_det.saveFromAc(idx, "KK-" + dbitem.SalesOrderOncall.SONumber, 0, db.PotonganT, Repoac_mstr.FindByPk(erpConfig.IdTabunganDriver));
                                }
                            }
                            Repoglt_det.saveFromAc(1, "KK-" + dbitem.SalesOrderOncall.SONumber, nominalHutangUangJalanDriver, 0, Repoac_mstr.FindByPk(erpConfig.IdAUJCredit));//Hutang Uang Jalan Driver
                            Context.MasterPool mp = RepoMasterPool.FindByIPAddress();
                            Repoglt_det.saveFromAc(2, "KK-" + dbitem.SalesOrderOncall.SONumber, 0, dbtf.JumlahTransfer, Repoac_mstr.FindByPk(RepoMasterPool.FindByIPAddress().IdCreditCash));//BCA Audy 386-7957777 atau
                            if (dbtf.TanggalAktual.HasValue)
                                dbtf.isTf = true;
                        }
                        else
                            return View("FormKas", model);
                    }
                }
                dbitem.Status = "dispatched";
                dbitem.UpdatedBy = UserPrincipal.id;
                RepoSalesOrder.save(dbitem);
                string strQuery = "";
                if (RepoDokumen.FindBySO(dbitem.Id) == null){
                    if(dbitem.SalesOrderOncallId.HasValue)
                        strQuery += GenerateDokumen(dbitem.Id, dbitem.SalesOrderOncall.Customer);
                    else if (dbitem.SalesOrderPickupId.HasValue)
                        strQuery += GenerateDokumen(dbitem.Id, dbitem.SalesOrderPickup.Customer);
                    else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue)
                    {
                        foreach (var item in dbitem.SalesOrderProsesKonsolidasi.SalesOrderProsesKonsolidasiItem)
                        {
                            strQuery += GenerateDokumen(dbitem.Id, item.SalesOrderKonsolidasi.Customer, "", item.SalesOrderKonsolidasi.StrDaftarHargaItem);
                        }
                    }
                }
                RepoAuditrail.saveKasirKasHistory(dbitem, strQuery);
                return RedirectToAction("IndexKas");
            }
            if (dbitem.AdminUangJalanId.HasValue)
                model = new AdminUangJalan(dbitem.AdminUangJalan, Repoatm.FindAll(), RepoBor.FindAll());
            model.IdSalesOrder = dbitem.Id;
            GenerateModel(dbitem, model);
            ViewBag.Status = "batal";
            return View("FormKas", model);
        }
        [HttpPost]
        public ActionResult EditKasRemoval(RemovalAUJ model)
        {
            Context.Removal dbitem = RepoRemoval.FindByPK(model.Id);
            if (dbitem.Status.ToLower().Contains("dispatched") || dbitem.Status.ToLower().Contains("admin uang jalan"))
            {
                for (int i = 0; i < model.ModelListTf.Count(); i++)
                {
                    if (model.ModelListTf[i].Nama == "Tunai")
                    {
                        if (validationManual(model))
                        {
                            Context.RemovalUangTf dbtf = dbitem.RemovalUangTf.Where(d => d.Keterangan == "Tunai").FirstOrDefault();
                            if (dbtf.JumlahTransfer != null && dbtf.JumlahTransfer != 0)
                                dbtf.JumlahTransfer = model.ModelListTf[i].JumlahTransfer;
                            else
                                dbtf.JumlahTransfer = model.ModelListTf[i].Value;
                            dbtf.TanggalAktual = model.ModelListTf[i].TanggalAktual;
                            dbtf.JamAktual = model.ModelListTf[i].JamAktual;
                            dbtf.KeteranganTf = model.ModelListTf[i].KeteranganTf;
                            dbtf.IdDriverPenerima = model.ModelListTf[i].IdDriverPenerima;
                            if (dbtf.TanggalAktual.HasValue)
                                dbtf.isTf = true;
                        }
                        else
                        {
                            return View("FormKasRemoval", model);
                        }
                    }
                }

                dbitem.Status = "dispatched";
                RepoRemoval.save(dbitem);
                return RedirectToAction("IndexKas");
            }
            model = new RemovalAUJ(dbitem, Repoatm.FindAll(), RepoBor.FindAll());

            return View("FormKasRemoval", model);
        }
        [HttpPost]
        public ActionResult EditKasKontrak(AdminUangJalan model)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(model.IdSalesOrder.Value);
            List<int> ListIdDumy = model.ListIdSo.Split(new string[] { "." }, StringSplitOptions.None).ToList().Select(int.Parse).ToList();
            List<Context.SalesOrderKontrakListSo> dbsoDummy = dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => ListIdDumy.Contains(d.Id)).ToList();
            string strQuery = "";

            for (int i = 0; i < model.ModelListTf.Count(); i++)
            {
                if (model.ModelListTf[i].Nama == "Tunai")
                {
                    if (validationManual(model))
                    {
                        Context.AdminUangJalanUangTf dbtf = dbsoDummy.FirstOrDefault().AdminUangJalan.AdminUangJalanUangTf.Where(d => d.Keterangan == "Tunai").FirstOrDefault();
                        if (dbtf.JumlahTransfer != null && dbtf.JumlahTransfer != 0)
                            dbtf.JumlahTransfer = model.ModelListTf[i].JumlahTransfer;
                        else
                            dbtf.JumlahTransfer = model.ModelListTf[i].Value;
                        dbtf.TanggalAktual = model.ModelListTf[i].TanggalAktual;
                        dbtf.JamAktual = model.ModelListTf[i].JamAktual;
                        dbtf.KeteranganTf = model.ModelListTf[i].KeteranganTf;
                        dbtf.IdDriverPenerima = model.ModelListTf[i].IdDriverPenerima;
                        if (dbtf.TanggalAktual.HasValue)
                            dbtf.isTf = true;
                        foreach (var item in dbsoDummy)
                        {
                            GenerateDokumen(dbitem.Id, dbitem.SalesOrderKontrak.Customer, item.Id.ToString());
                        }
                    }
                    else
                    {
                        return View("FormKas", model);
                    }
                }
            }

            RepoSalesOrder.save(dbitem);
            return RedirectToAction("IndexKas");
        }
        [MyAuthorize(Menu = "Kasir Cash", Action="read")]
        public ActionResult ViewKas(int id)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            //cari
            AdminUangJalan model = new AdminUangJalan();
            if (dbitem.AdminUangJalanId.HasValue)
                model = new AdminUangJalan(dbitem.AdminUangJalan, Repoatm.FindAll(), RepoBor.FindAll());

            model.IdSalesOrder = dbitem.Id;
            if (dbitem.SalesOrderOncallId.HasValue)
            {
                model.ModelOncall = new SalesOrderOncall(dbitem);
                if (model.IdDriver1.HasValue)
                {
                    model.IdDriver1 = model.IdDriver1;
                    model.NamaDriver1 = model.NamaDriver1;
                    model.KeteranganGanti1 = model.KeteranganGanti1;
                }
                else
                {
                    model.IdDriver1 = model.ModelOncall.Driver1Id;
                    model.NamaDriver1 = model.ModelOncall.KodeDriver1 + " - " + model.ModelOncall.NamaDriver1;
                    model.KeteranganGanti1 = model.ModelOncall.KeteranganDriver1;
                }
                if (model.IdDriver2.HasValue)
                {
                    model.IdDriver2 = model.IdDriver2;
                    model.NamaDriver2 = model.NamaDriver2;
                    model.KeteranganGanti2 = model.KeteranganGanti2;
                }
                else
                {
                    model.IdDriver2 = model.ModelOncall.Driver2Id;
                    model.NamaDriver2 = model.ModelOncall.KodeDriver2 + " - " + model.ModelOncall.NamaDriver2;
                    model.KeteranganGanti2 = model.ModelOncall.KeteranganDriver2;
                }
                ViewBag.Title = "Kasir Cash " + dbitem.SalesOrderOncall.SONumber;
                //ViewBag.postData = "EditOncall";
                return View("FormKasView", model);
            }
            else if (dbitem.SalesOrderPickupId.HasValue)
            {
                model.ModelPickup = new SalesOrderPickup(dbitem);
                if (model.IdDriver1.HasValue)
                {
                    model.IdDriver1 = model.IdDriver1;
                    model.NamaDriver1 = model.NamaDriver1;
                    model.KeteranganGanti1 = model.KeteranganGanti1;
                }
                else
                {
                    model.IdDriver1 = model.ModelPickup.Driver1Id;
                    model.NamaDriver1 = model.ModelPickup.KodeDriver1 + " - " + model.ModelPickup.NamaDriver1;
                    model.KeteranganGanti1 = model.ModelPickup.KeteranganDriver1;
                }
                if (model.IdDriver2.HasValue)
                {
                    model.IdDriver2 = model.IdDriver2;
                    model.NamaDriver2 = model.NamaDriver2;
                    model.KeteranganGanti2 = model.KeteranganGanti2;
                }
                else
                {
                    model.IdDriver2 = model.ModelPickup.Driver2Id;
                    model.NamaDriver2 = model.ModelPickup.KodeDriver2 + " - " + model.ModelPickup.NamaDriver2;
                    model.KeteranganGanti2 = model.ModelPickup.KeteranganDriver2;
                }
                ViewBag.Title = "Kasir Cash " + dbitem.SalesOrderPickup.SONumber;
                //ViewBag.postData = "EditPickup";
                return View("FormKasView", model);
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue)
            {
                model.ModelKonsolidasi = new SalesOrderProsesKonsolidasi(dbitem);
                if (model.IdDriver1.HasValue)
                {
                    model.IdDriver1 = model.IdDriver1;
                    model.NamaDriver1 = model.NamaDriver1;
                    model.KeteranganGanti1 = model.KeteranganGanti1;
                }
                else
                {
                    model.IdDriver1 = model.ModelKonsolidasi.Driver1Id;
                    model.NamaDriver1 = model.ModelKonsolidasi.KodeDriver1 + " - " + model.ModelKonsolidasi.NamaDriver1;
                    model.KeteranganGanti1 = model.ModelKonsolidasi.KeteranganDriver1;
                }
                if (model.IdDriver2.HasValue)
                {
                    model.IdDriver2 = model.IdDriver2;
                    model.NamaDriver2 = model.NamaDriver2;
                    model.KeteranganGanti2 = model.KeteranganGanti2;
                }
                else
                {
                    model.IdDriver2 = model.ModelKonsolidasi.Driver2Id;
                    model.NamaDriver2 = model.ModelKonsolidasi.KodeDriver2 + " - " + model.ModelKonsolidasi.NamaDriver2;
                    model.KeteranganGanti2 = model.ModelKonsolidasi.KeteranganDriver2;
                }
                ViewBag.Title = "Kasir Cash " + dbitem.SalesOrderProsesKonsolidasi.SONumber;
                //ViewBag.postData = "EditPickup";
                return View("FormKasView", model);
            }

            return View("");
        }
        [MyAuthorize(Menu = "Kasir Cash", Action="read")]
        public ActionResult ViewKasRemoval(int id)
        {
            Context.Removal dbitem = RepoRemoval.FindByPK(id);
            RemovalAUJ model = new RemovalAUJ(dbitem, Repoatm.FindAll(), RepoBor.FindAll());

            return View("FormKasRemovalView", model);
        }
        public void GenerateModel(Context.SalesOrder dbitem, AdminUangJalan model)
        {
            if (dbitem.SalesOrderOncallId.HasValue)
            {
                model.ModelOncall = new SalesOrderOncall(dbitem);
                if (model.IdDriver1.HasValue)
                {
                    model.IdDriver1 = model.IdDriver1;
                    model.NamaDriver1 = model.NamaDriver1;
                    model.KeteranganGanti1 = model.KeteranganGanti1;
                }
                else
                {
                    model.IdDriver1 = model.ModelOncall.Driver1Id;
                    model.NamaDriver1 = model.ModelOncall.KodeDriver1 + " - " + model.ModelOncall.NamaDriver1;
                    model.KeteranganGanti1 = model.ModelOncall.KeteranganDriver1;
                }
                if (model.IdDriver2.HasValue)
                {
                    model.IdDriver2 = model.IdDriver2;
                    model.NamaDriver2 = model.NamaDriver2;
                    model.KeteranganGanti2 = model.KeteranganGanti2;
                }
                else
                {
                    model.IdDriver2 = model.ModelOncall.Driver2Id;
                    model.NamaDriver2 = model.ModelOncall.KodeDriver2 + " - " + model.ModelOncall.NamaDriver2;
                    model.KeteranganGanti2 = model.ModelOncall.KeteranganDriver2;
                }
                ViewBag.Title = "Kasir Kas " + dbitem.SalesOrderOncall.SONumber;
                //ViewBag.postData = "EditOncall";
            }
            else if (dbitem.SalesOrderPickupId.HasValue)
            {
                model.ModelPickup = new SalesOrderPickup(dbitem);
                if (model.IdDriver1.HasValue)
                {
                    model.IdDriver1 = model.IdDriver1;
                    model.NamaDriver1 = model.NamaDriver1;
                    model.KeteranganGanti1 = model.KeteranganGanti1;
                }
                else
                {
                    model.IdDriver1 = model.ModelPickup.Driver1Id;
                    model.NamaDriver1 = model.ModelPickup.KodeDriver1 + " - " + model.ModelPickup.NamaDriver1;
                    model.KeteranganGanti1 = model.ModelPickup.KeteranganDriver1;
                }
                if (model.IdDriver2.HasValue)
                {
                    model.IdDriver2 = model.IdDriver2;
                    model.NamaDriver2 = model.NamaDriver2;
                    model.KeteranganGanti2 = model.KeteranganGanti2;
                }
                else
                {
                    model.IdDriver2 = model.ModelPickup.Driver2Id;
                    model.NamaDriver2 = model.ModelPickup.KodeDriver2 + " - " + model.ModelPickup.NamaDriver2;
                    model.KeteranganGanti2 = model.ModelPickup.KeteranganDriver2;
                }
                ViewBag.Title = "Kasir Kas " + dbitem.SalesOrderPickup.SONumber;
                //ViewBag.postData = "EditPickup";
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue)
            {
                model.ModelKonsolidasi = new SalesOrderProsesKonsolidasi(dbitem);
                if (model.IdDriver1.HasValue)
                {
                    model.IdDriver1 = model.IdDriver1;
                    model.NamaDriver1 = model.NamaDriver1;
                    model.KeteranganGanti1 = model.KeteranganGanti1;
                }
                else
                {
                    model.IdDriver1 = model.ModelKonsolidasi.Driver1Id;
                    model.NamaDriver1 = model.ModelKonsolidasi.KodeDriver1 + " - " + model.ModelKonsolidasi.NamaDriver1;
                    model.KeteranganGanti1 = model.ModelKonsolidasi.KeteranganDriver1;
                }
                if (model.IdDriver2.HasValue)
                {
                    model.IdDriver2 = model.IdDriver2;
                    model.NamaDriver2 = model.NamaDriver2;
                    model.KeteranganGanti2 = model.KeteranganGanti2;
                }
                else
                {
                    model.IdDriver2 = model.ModelKonsolidasi.Driver2Id;
                    model.NamaDriver2 = model.ModelKonsolidasi.KodeDriver2 + " - " + model.ModelKonsolidasi.NamaDriver2;
                    model.KeteranganGanti2 = model.ModelKonsolidasi.KeteranganDriver2;
                }
                ViewBag.Title = "Kasir Kas " + dbitem.SalesOrderProsesKonsolidasi.SONumber;
                //ViewBag.postData = "EditPickup";
            }
        }
        public bool validationManual(AdminUangJalan model)
        {
            //if (dbtf.JumlahTransfer != null && dbtf.JumlahTransfer != 0){

            //}
            //    dbtf.JumlahTransfer = model.ModelListTf[i].JumlahTransfer;
            //else
            //    dbtf.JumlahTransfer = model.ModelListTf[i].Value;


            return true;
        }
        public bool validationManual(RemovalAUJ model)
        {
            //if (dbtf.JumlahTransfer != null && dbtf.JumlahTransfer != 0){

            //}
            //    dbtf.JumlahTransfer = model.ModelListTf[i].JumlahTransfer;
            //else
            //    dbtf.JumlahTransfer = model.ModelListTf[i].Value;


            return true;
        }
        [MyAuthorize(Menu = "Kasir Cash", Action="print")]
        public ActionResult PrintKas(int id, string listSo, string terbilang)
        {
            return new Rotativa.ActionAsPdf("ShowPrintKas", new { id = id, listSo = listSo, terbilang = terbilang })
            { 
                FileName = "Kas_" + new Guid().ToString() + ".pdf", 
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait
            };
        }
        [MyAuthorize(Menu = "Kasir Cash", Action="print")]
        public ActionResult PrintKasRemoval(int id)
        {
            return new Rotativa.ActionAsPdf("ShowPrintKasRemoval", new { id = id})
            {
                FileName = "Kas_" + new Guid().ToString() + ".pdf",
                PageSize = Rotativa.Options.Size.A5,
                PageOrientation = Rotativa.Options.Orientation.Landscape
            };
        }
        public ActionResult ShowPrintKas(int id, string listSo, string terbilang)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            //cari
            AdminUangJalan model = new AdminUangJalan();
            List<Context.SalesOrderKontrakListSo> dbsoDummy = new List<Context.SalesOrderKontrakListSo>();
            if (dbitem.AdminUangJalanId.HasValue)
            {
                if (dbitem.SalesOrderKontrakId.HasValue)
                {
                    List<int> ListIdDumy = listSo.Split(new string[] { "." }, StringSplitOptions.None).ToList().Select(int.Parse).ToList();
                    dbsoDummy = dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => ListIdDumy.Contains(d.Id)).ToList();
                    if (dbsoDummy.FirstOrDefault().AdminUangJalan != null)
                        model = new AdminUangJalan(dbsoDummy.FirstOrDefault().AdminUangJalan, Repoatm.FindAll(), RepoBor.FindAll());
                }
                else
                {
                    model = new AdminUangJalan(dbitem.AdminUangJalan, Repoatm.FindAll(), RepoBor.FindAll());   
                }
            }

            model.IdSalesOrder = dbitem.Id;
            GenerateModel(dbitem, model);

            string NoPol = "";
            var items = model.ModelListTf.Where(d => d.Nama == "Tunai").ToList();
            if (dbitem.SalesOrderOncallId.HasValue)
            {
                ViewBag.noBukti = items.FirstOrDefault().Code;
                ViewBag.tanggal = DateTime.Now.ToShortDateString();
                ViewBag.kontak = dbitem.AdminUangJalan.Driver1.NamaDriver;
                NoPol = dbitem.SalesOrderOncall.DataTruck.VehicleNo;
            }
            else if (dbitem.SalesOrderPickupId.HasValue)
            {
                ViewBag.noBukti = items.FirstOrDefault().Code;
                ViewBag.tanggal = DateTime.Now.ToShortDateString();
                ViewBag.kontak = dbitem.AdminUangJalan.Driver1.NamaDriver;
                NoPol = dbitem.SalesOrderPickup.DataTruck.VehicleNo;
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue)
            {
                ViewBag.noBukti = items.FirstOrDefault().Code;
                ViewBag.tanggal = DateTime.Now.ToShortDateString();
                ViewBag.kontak = dbitem.AdminUangJalan.Driver1.NamaDriver;
                NoPol = dbitem.SalesOrderProsesKonsolidasi.DataTruck.VehicleNo;
            }
            else if (dbitem.SalesOrderKontrakId.HasValue)
            {
                ViewBag.noBukti = items.FirstOrDefault().Code;
                ViewBag.tanggal = DateTime.Now.ToShortDateString();
                ViewBag.kontak = dbsoDummy.FirstOrDefault().Driver1.NamaDriver;
                NoPol = dbsoDummy.FirstOrDefault().DataTruck.VehicleNo;
            }
            decimal? total = 0;
            if(items != null)
            {
                foreach (var item in items)
                {
                    item.Nama = "Uang Jalan " + NoPol + " " + string.Format("{0:#,00}", item.JumlahTransfer);
                    total = total + item.JumlahTransfer;
                }
            }
            ViewBag.item = items;
            ViewBag.total = total;
            ViewBag.terbilang = terbilang;
            return View("PrintKas");
        }
        public ActionResult ShowPrintKasRemoval(int id)
        {
            Context.Removal dbitem = RepoRemoval.FindByPK(id);
            RemovalAUJ model = new RemovalAUJ(dbitem, Repoatm.FindAll(), RepoBor.FindAll());

            string NoPol = "";
            if (dbitem.SalesOrder.SalesOrderOncallId.HasValue)
            {
                ViewBag.noBukti = "[Belum ada format nya]";
                ViewBag.tanggal = DateTime.Now.ToShortDateString();
                ViewBag.kontak = dbitem.AdminUangJalan.Driver1.NamaDriver;
                NoPol = dbitem.SalesOrder.SalesOrderOncall.DataTruck.VehicleNo;
            }
            else if (dbitem.SalesOrder.SalesOrderPickupId.HasValue)
            {
                ViewBag.noBukti = "[Belum ada format nya]";
                ViewBag.tanggal = DateTime.Now.ToShortDateString();
                ViewBag.kontak = dbitem.AdminUangJalan.Driver1.NamaDriver;
                NoPol = dbitem.SalesOrder.SalesOrderPickup.DataTruck.VehicleNo;
            }
            else if (dbitem.SalesOrder.SalesOrderProsesKonsolidasiId.HasValue)
            {
                ViewBag.noBukti = "[Belum ada format nya]";
                ViewBag.tanggal = DateTime.Now.ToShortDateString();
                ViewBag.kontak = dbitem.AdminUangJalan.Driver1.NamaDriver;
                NoPol = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.DataTruck.VehicleNo;
            }
            var items = model.ModelListTf.Where(d => d.Nama == "Tunai").ToList();
            decimal? total = 0;
            if (items != null)
            {
                foreach (var item in items)
                {
                    item.Nama = NoPol + " ( TOE [Format apa] - " + string.Format("{0:#,00}", item.JumlahTransfer) + ") ";
                    total = total + item.JumlahTransfer;
                }
            }
            ViewBag.item = items;
            ViewBag.total = total;
            return View("PrintKas");
        }

        private string GenerateDokumen(int IdSo , Context.Customer dbcust , string ListIdSo = "", string rute = "")
        {
            Context.Dokumen dbdokumen = new Context.Dokumen();
            //ambil dokumen dari customer
            dbdokumen.IdSO = IdSo;
            dbdokumen.ListIdSo = ListIdSo;
            dbdokumen.IsAdmin = true;
            dbdokumen.ModifiedDate = DateTime.Now;
            dbdokumen.IdCustomer = dbcust.Id;
            dbdokumen.RuteSo = rute;
            string strQuery = "INSERT INTO dbo.\"Dokumen\" (\"IdSO\", \"ListIdSo\", \"IsComplete\", \"IsAdmin\", \"ModifiedDate\", \"IdCustomer\", \"IsReturn\", \"RuteSo\") VALUES (" + dbdokumen.IdSO + ", " + dbdokumen.ListIdSo +
                ", " + dbdokumen.IsComplete + ", " + dbdokumen.IsAdmin + ", " + dbdokumen.ModifiedDate + ", " + dbdokumen.IdCustomer + ", " + dbdokumen.IsReturn + ", " + dbdokumen.RuteSo + ");";
            foreach (var item in dbcust.CustomerBilling)
            {
                dbdokumen.DokumenItem.Add(new Context.DokumenItem() { 
                    IdBilling = item.Id,
                    CustomerId = item.CustomerId,
                    ModifiedDate = DateTime.Now,
                });
                strQuery += "INSERT INTO dbo.\"DokumenItem\" (\"IdBilling\", \"CustomerId\", \"ModifiedDate\") VALUES (" + item.Id + ", " + item.CustomerId + ", " + DateTime.Now + ");";
            }
            RepoDokumen.save(dbdokumen, UserPrincipal.id);
            return strQuery;
        }
    }
}