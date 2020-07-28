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
    public class AdminUangJalanController : BaseController
    {
        private IAdminUangJalanRepo RepoAdminUangJalan;
        private ISalesOrderRepo RepoSalesOrder;
        private IDaftarHargaOnCallRepo RepoDHO;
        private IDaftarHargaKonsolidasiRepo RepoDHK;
        private IRuteRepo RepoRute;
        private IAtmRepo RepoAtm;
        private IDataBoronganRepo RepoBor;
        private ISalesOrderKontrakListSoRepo RepoSalesOrderKontrakListSo;
        private IHistoryJalanTruckRepo RepoHistoryJalanTruck;
        private IRemovalRepo RepoRemovalRepo;
        private ISettlementBatalRepo RepoSettlementBatal;
        private Iglt_detRepo Repoglt_det;
        private IERPConfigRepo RepoERPConfig;
        private Iac_mstrRepo Repoac_mstr;
        private Ibk_mstrRepo Repobk_mstr;
        private Ipbyd_detRepo Repopbyd_det;
        private ILookupCodeRepo RepoLookupCode;
        private Icashd_detRepo Repocashd_det;
        private IDriverRepo RepoDriver;
        private IAuditrailRepo RepoAuditrail;
        public AdminUangJalanController(IUserReferenceRepo repoBase, ILookupCodeRepo repoLookup, IAdminUangJalanRepo repoAdminUangJalan, ISalesOrderRepo repoSalesOrder, IDaftarHargaOnCallRepo repoDHO,
            IDaftarHargaKonsolidasiRepo repoDHK, IRuteRepo repoRute, IAtmRepo repoAtm, IDataBoronganRepo repoBor, ISalesOrderKontrakListSoRepo repoSalesOrderKontrakListSo, IAuditrailRepo repoAuditrail,
            IHistoryJalanTruckRepo repoHistoryJalanTruck, IRemovalRepo repoRemovalRepo, ISettlementBatalRepo repoSettlementBatal, Iglt_detRepo repoglt_det, IERPConfigRepo repoERPConfig,
            Iac_mstrRepo repoac_mstr, Ibk_mstrRepo repobk_mstr, Ipbyd_detRepo repopbyd_det, ILookupCodeRepo repoLookupCode, Icashd_detRepo repocashd_det, IDriverRepo repoDriver)
            : base(repoBase, repoLookup)
        {
            RepoAdminUangJalan = repoAdminUangJalan;
            RepoSalesOrder = repoSalesOrder;
            RepoDHO = repoDHO;
            RepoDHK = repoDHK;
            RepoRute = repoRute;
            RepoAtm = repoAtm;
            RepoBor = repoBor;
            RepoSalesOrderKontrakListSo = repoSalesOrderKontrakListSo;
            RepoHistoryJalanTruck = repoHistoryJalanTruck;
            RepoRemovalRepo = repoRemovalRepo;
            RepoSettlementBatal = repoSettlementBatal;
            Repoglt_det = repoglt_det;
            RepoERPConfig = repoERPConfig;
            Repoac_mstr = repoac_mstr;
            Repobk_mstr = repobk_mstr;
            Repopbyd_det = repopbyd_det;
            RepoLookupCode = repoLookupCode;
            Repocashd_det = repocashd_det;
            RepoDriver = repoDriver;
            RepoAuditrail = repoAuditrail;
        }
        [MyAuthorize(Menu = "Admin Uang Jalan", Action="read")]
        public ActionResult Index()
        {
            ViewBag.listKolom = ListKolom.Where(d => d.Action == "Index" && d.Controller == "AdminUangJalan").ToList();
            return View();
        }
        public ActionResult IndexKontrak()
        {
            ViewBag.listKolom = ListKolom.Where(d => d.Action == "Index" && d.Controller == "AdminUangJalan").ToList();
            return View();
        }
        public string Binding()
        {
            List<Context.SalesOrder> items = RepoSalesOrder.FindAllAdminUangJalan();

            List<AdminUangJalanIndex> ListModel = new List<AdminUangJalanIndex>();
            foreach (Context.SalesOrder item in items)
            {
                if (!item.SalesOrderKontrakId.HasValue) {
                    var _model = new AdminUangJalanIndex(item);
                    //rute oncall dan konsolidasi
                    int IdJenisTruck;
                    string IdRute;
                    if (item.SalesOrderOncallId.HasValue)
                    {
                        RepoDHO.FindRuteTruk(item.SalesOrderOncall.IdDaftarHargaItem.Value, out IdRute, out IdJenisTruck);
                        if (IdRute != null && IdRute != "")
                        {
                            List<string> ListRute = new List<string>();
                            List<string> ListIdRute = IdRute.Split(',').ToList();
                            foreach (string idTruck in ListIdRute)
                            {
                                ListRute.Add(RepoRute.FindByPK(int.Parse(idTruck)).Nama);
                            }
//                            _model.Rute = string.Join(", ", ListRute);
                        }
                    }
                    else if (item.SalesOrderProsesKonsolidasiId.HasValue)
                    {
                        RepoDHK.FindRuteTruk(item.SalesOrderProsesKonsolidasi.IdDaftarHargaItem.Value, out IdRute, out IdJenisTruck);
                        {
                            if (IdRute != "")
                            {
                                List<string> ListRute = new List<string>();
                                List<string> ListIdRute = IdRute.Split(',').ToList();
                                foreach (string idTruck in ListIdRute)
                                {
                                    ListRute.Add(RepoRute.FindByPK(int.Parse(idTruck)).Nama);
                                }
                                _model.Rute = string.Join(", ", ListRute);
                            }
                        }
                    }

                    ListModel.Add(_model);
                }
            }

            return new JavaScriptSerializer().Serialize(new { total = items.Count, data = ListModel });
        }

        public string BindingKontrak()
        {
            List<Context.SalesOrder> items = RepoSalesOrder.FindAllAdminUangJalan();

            List<AdminUangJalanIndex> ListModel = new List<AdminUangJalanIndex>();
            foreach (Context.SalesOrder item in items)
            {
                if (item.SalesOrderKontrakId.HasValue){
                    var data = item.SalesOrderKontrak.SalesOrderKontrakListSo.Where(p => p.IsProses && (p.Status == "save konfirmasi"))
                        .GroupBy(d => new { d.IdDataTruck, d.Driver1Id, d.Status, d.Urutan }).Select(grp => grp.ToList());
                    foreach (var itemGroup in data.ToList())
                    {
                        ListModel.Add(new AdminUangJalanIndex(item, itemGroup));
                    }
                }
            }

            return new JavaScriptSerializer().Serialize(new { total = ListModel.Count, data = ListModel });
        }

        public void GenerateModel(AdminUangJalan model, Context.SalesOrder dbitem)
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
                    if (model.ModelOncall.Driver2Id.HasValue)
                    {
                        model.IdDriver2 = model.ModelOncall.Driver2Id;
                        model.NamaDriver2 = model.ModelOncall.KodeDriver2 + " - " + model.ModelOncall.NamaDriver2;
                        model.KeteranganGanti2 = model.ModelOncall.KeteranganDriver2;
                    }
                }
                ViewBag.Title = "Admin Uang Jalan " + dbitem.SalesOrderOncall.SONumber;
                //ViewBag.postData = "EditOncall";
                //return View("Form", model);
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
                ViewBag.Title = "Admin Uang Jalan " + dbitem.SalesOrderPickup.SONumber;
                //ViewBag.postData = "EditPickup";
                //return View("Form", model);
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
                ViewBag.Title = "Admin Uang Jalan " + dbitem.SalesOrderProsesKonsolidasi.SONumber;
                //ViewBag.postData = "EditPickup";
                //return View("Form", model);
            }
            else if (dbitem.SalesOrderKontrakId.HasValue)
            {
                model.ModelKontrak = new SalesOrderKontrak(dbitem);
                Context.SalesOrderKontrakListSo dummySo = dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.FirstOrDefault();
                if (model.IdDriver1.HasValue)
                {
                    model.IdDriver1 = model.IdDriver1;
                    model.NamaDriver1 = model.NamaDriver1;
                    model.KeteranganGanti1 = model.KeteranganGanti1;
                }
                else
                {
                    model.IdDriver1 = dummySo.Driver1Id;
                    model.NamaDriver1 = dummySo.Driver1.KodeDriver + " - " + dummySo.Driver1.NamaDriver;
                }
                if (model.IdDriver2.HasValue)
                {
                    model.IdDriver2 = model.IdDriver2;
                    model.NamaDriver2 = model.NamaDriver2;
                    model.KeteranganGanti2 = model.KeteranganGanti2;
                }
                else
                {
                    if (dummySo.Driver2Id.HasValue)
                    {
                        model.IdDriver2 = dummySo.Driver2Id;
                        model.NamaDriver2 = dummySo.Driver2.KodeDriver + " - " + dummySo.Driver2.NamaDriver;
                    }
                }
                ViewBag.Title = "Admin Uang Jalan " + dbitem.SalesOrderKontrak.SONumber;
            }
        }
        public bool validation(AdminUangJalan model)
        {
            bool valid = true;
            int idx = 0;
            foreach (AdminUangBorongan item in model.ModelListBorongan.Where(d => !d.IsDelete))
            {
                if (!item.IdDataBorongan.HasValue)
                {
                    ModelState.AddModelError("ModelListBorongan[" + idx.ToString() + "].IdDataBorongan", "Borongan harus diisi.");
                    valid = false;
                }
                idx++;
            }

            idx = 0;
            foreach (AdminUangJalanTambahanRute item in model.ModelListTambahanRute.Where(d => !d.IsDelete))
            {
                if (!item.IdDataBorongan.HasValue)
                {
                    ModelState.AddModelError("ModelListTambahanRute[" + idx.ToString() + "].IdDataBorongan", "Borongan harus diisi.");
                    valid = false;
                }
                if (!item.value.HasValue)
                {
                    ModelState.AddModelError("ModelListTambahanRute[" + idx.ToString() + "].value", "Nilai harus diisi.");
                    valid = false;
                }
                idx++;
            }

            idx = 0;
            foreach (AdminUangJalanTambahanLain item in model.ModelListTambahanLain.Where(d => !d.IsDelete))
            {
                if (item.Keterangan == "")
                {
                    ModelState.AddModelError("ModelListTambahanLain[" + idx.ToString() + "].IdDataBorongan", "Borongan harus diisi.");
                    valid = false;
                }
                if (!item.Value.HasValue)
                {
                    ModelState.AddModelError("ModelListTambahanLain[" + idx.ToString() + "].Value", "Nilai harus diisi.");
                    valid = false;
                }
                idx++;
            }

            return valid;
        }
        public bool validationRemoval(RemovalAUJ model)
        {
            bool valid = true;
            int idx = 0;
            foreach (AdminUangBorongan item in model.ModelListBorongan.Where(d => !d.IsDelete))
            {
                if (!item.IdDataBorongan.HasValue)
                {
                    ModelState.AddModelError("ModelListBorongan[" + idx.ToString() + "].IdDataBorongan", "Borongan harus diisi.");
                    valid = false;
                }
                idx++;
            }

            idx = 0;
            foreach (AdminUangJalanTambahanRute item in model.ModelListTambahanRute.Where(d => !d.IsDelete))
            {
                if (!item.IdDataBorongan.HasValue)
                {
                    ModelState.AddModelError("ModelListTambahanRute[" + idx.ToString() + "].IdDataBorongan", "Borongan harus diisi.");
                    valid = false;
                }
                if (!item.value.HasValue)
                {
                    ModelState.AddModelError("ModelListTambahanRute[" + idx.ToString() + "].value", "Nilai harus diisi.");
                    valid = false;
                }
                idx++;
            }

            idx = 0;
            foreach (AdminUangJalanTambahanLain item in model.ModelListTambahanLain.Where(d => !d.IsDelete))
            {
                if (item.Keterangan == "")
                {
                    ModelState.AddModelError("ModelListTambahanLain[" + idx.ToString() + "].IdDataBorongan", "Borongan harus diisi.");
                    valid = false;
                }
                if (!item.Value.HasValue)
                {
                    ModelState.AddModelError("ModelListTambahanLain[" + idx.ToString() + "].Value", "Nilai harus diisi.");
                    valid = false;
                }
                idx++;
            }

            return valid;
        }

        public decimal GetSaldoKlaim(int id, string piutang_type)
        {
            Context.Driver dbdriver = RepoDriver.FindByPK(id);
            List<DriverPiutangHistory> ListModel = new List<DriverPiutangHistory>();
            List<Context.Klaim> dbklaim = dbdriver.BebanKlaimDriver.Select(d => d.Klaim).ToList();
            decimal? saldo = 0;

            List<Klaim> modelklaim = new List<Klaim>();
            foreach (var item in dbklaim)
            {
                ListModel.Add(new DriverPiutangHistory("a", "b", item, saldo.Value));
                saldo += decimal.Parse(item.BebanClaimDriver.Value.ToString());
            }
            return saldo.Value;
        }

        public decimal GetSaldoPiutangBatalJalan(int id, string piutang_type)
        {
            List<DriverPiutangHistory> ListModel = new List<DriverPiutangHistory>();
            decimal? saldo = 0;
            if (piutang_type != "Klaim")
            {
                List<Context.pbyd_det> pbyd_dets = Repopbyd_det.FindAll().Where(d => d.pby_mstr.pby_driver == id && d.pbyd_amount_pay != 0 && d.pbyd_tms_type == piutang_type).ToList();

                foreach (Context.pbyd_det item in pbyd_dets)
                {
                    DriverPiutangHistory dph = new DriverPiutangHistory(item, saldo.Value);
                    ListModel.Add(dph);
                    saldo += item.pbyd_amount_pay;
                }
            }
            return saldo.Value;
        }

        public decimal GetSaldoPiutang(int id, string piutang_type)
        {
            List<DriverPiutangHistory> ListModel = new List<DriverPiutangHistory>();
            decimal? saldo = 0;
            if (piutang_type != "Klaim")
            {
                List<Context.pbyd_det> pbyd_dets = Repopbyd_det.FindAll().Where(d => d.pby_mstr.pby_driver == id && d.pbyd_amount_pay != 0).ToList();
                List<Context.cashd_det> cashd_dets = Repocashd_det.FindAll().Where(d => d != null).ToList();

                foreach (Context.pbyd_det item in pbyd_dets)
                {
                    DriverPiutangHistory dph = new DriverPiutangHistory(item, saldo.Value);
                    ListModel.Add(dph);
                    saldo += item.pbyd_amount_pay;
                }
            }
            return saldo.Value;
        }

        [MyAuthorize(Menu = "Admin Uang Jalan", Action="update")]
        public ActionResult Edit(int id)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            if (dbitem.SalesOrderOncallId.HasValue){
                ViewBag.driverId = dbitem.SalesOrderOncall.Driver1Id;
                ViewBag.driverName = dbitem.SalesOrderOncall.Driver1.KodeDriver + " - " + dbitem.SalesOrderOncall.Driver1.NamaDriver;
                ViewBag.KodeDriver1Old = dbitem.SalesOrderOncall.Driver1.KodeDriverOld;
                ViewBag.TotalKasbon = GetSaldoPiutang(dbitem.SalesOrderOncall.Driver1Id.Value+7000000, "Kasbon");
                ViewBag.TotalKlaim = GetSaldoKlaim(dbitem.SalesOrderOncall.Driver1Id.Value, "Klaim");
            }
            else if (dbitem.SalesOrderPickupId.HasValue){
                ViewBag.driverId = dbitem.SalesOrderPickup.Driver1Id;
                ViewBag.driverName = dbitem.SalesOrderPickup.Driver1.KodeDriver + " - " + dbitem.SalesOrderPickup.Driver1.NamaDriver;
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue){
                ViewBag.driverId = dbitem.SalesOrderProsesKonsolidasi.Driver1Id;
                ViewBag.driverName = dbitem.SalesOrderProsesKonsolidasi.Driver1.KodeDriver + " - " + dbitem.SalesOrderProsesKonsolidasi.Driver1.NamaDriver;
            }
            Context.SettlementBatal sb = RepoSettlementBatal.FindAll().Where(
                d => d.IsProses == true && d.IdAdminUangJalan == null && d.SalesOrder != null && d.IdDriver == ViewBag.driverId
            ).FirstOrDefault();
            ViewBag.SisaPerjalananSblmny = 0;
            ViewBag.PiutangBatalJalan = GetSaldoPiutangBatalJalan(dbitem.SalesOrderOncall.Driver1Id.Value+7000000, "B");
            ViewBag.PiutangPribadi = GetSaldoPiutangBatalJalan(dbitem.SalesOrderOncall.Driver1Id.Value+7000000, "P");
            ViewBag.PiutangTabungan = GetSaldoPiutangBatalJalan(dbitem.SalesOrderOncall.Driver1Id.Value+7000000, "T");
            if (sb != null)
            {
/*                ViewBag.SisaPerjalananSblmny += sb.TransferSelisih == null ? 0 : sb.TransferSelisih;
                ViewBag.SisaPerjalananSblmny += sb.KasSelisih == null ? 0 : sb.KasSelisih;
                ViewBag.SisaPerjalananSblmny += sb.SolarSelisih == null ? 0 : sb.SolarSelisih;
                ViewBag.SisaPerjalananSblmny += sb.KapalSelisih == null ? 0 : sb.KapalSelisih;*/
                if (sb.SalesOrder.SalesOrderOncallId.HasValue)
                    ViewBag.SOSblmny = sb.SalesOrder.SalesOrderOncall.SONumber;
                else if (sb.SalesOrder.SalesOrderPickupId.HasValue)
                    ViewBag.SOSblmny = sb.SalesOrder.SalesOrderPickup.SONumber;
                else if (sb.SalesOrder.SalesOrderProsesKonsolidasiId.HasValue)
                    ViewBag.SOSblmny = sb.SalesOrder.SalesOrderProsesKonsolidasi.SONumber;
                else if (sb.SalesOrder.SalesOrderKontrakId.HasValue)
                    ViewBag.SOSblmny = sb.SalesOrder.SalesOrderKontrak.SONumber;
            }
            AdminUangJalan model = new AdminUangJalan();
            if (dbitem.AdminUangJalanId.HasValue)
                model = new AdminUangJalan(dbitem.AdminUangJalan, RepoAtm.FindAll(), RepoBor.FindAll());

            model.IdSalesOrder = dbitem.Id;
            GenerateModel(model, dbitem);

            if (model.ModelListRemoval.Count > 0)
            {
                model.ModelListRemoval = model.ModelListRemoval.OrderBy(d => d.Id).ToList();
                return View("~/Views/Removal/FormRemoval.cshtml", model);
            }
            else
                return View("Form", model);
        }
        [MyAuthorize(Menu = "Admin Uang Jalan", Action="update")]
        public ActionResult EditKontrak(int id, string listSo)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            //ambil admin uang jalan nya dari listSo yang pertama
            List<int> ListIdDumy = listSo.Split(new string[] { "." }, StringSplitOptions.None).ToList().Select(int.Parse).ToList();
            List<Context.SalesOrderKontrakListSo> dbsoDummy = dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => ListIdDumy.Contains(d.Id)).ToList();
            dbitem.SalesOrderKontrak.SalesOrderKontrakListSo = dbsoDummy;
            AdminUangJalan model = new AdminUangJalan();
            ViewBag.KodeDriver1Old = dbsoDummy.FirstOrDefault().Driver1.KodeDriverOld;

            if (dbsoDummy.FirstOrDefault().IdAdminUangJalan.HasValue)
                model = new AdminUangJalan(dbsoDummy.FirstOrDefault().AdminUangJalan, RepoAtm.FindAll(), RepoBor.FindAll());
            
            model.IdSalesOrder = dbitem.Id;
            GenerateModel(model, dbitem);
            model.ModelKontrak.ListValueModelSOKontrak = model.ModelKontrak.ListModelSOKontrak; 
            ViewBag.PiutangBatalJalan = GetSaldoPiutangBatalJalan(dbsoDummy.FirstOrDefault().Driver1Id.Value+7000000, "B");
            ViewBag.PiutangPribadi = GetSaldoPiutangBatalJalan(dbsoDummy.FirstOrDefault().Driver1Id.Value+7000000, "P");
            ViewBag.PiutangTabungan = GetSaldoPiutangBatalJalan(dbsoDummy.FirstOrDefault().Driver1Id.Value+7000000, "T");

            return View("FormKontrak", model);
        }
        [MyAuthorize(Menu = "Admin Uang Jalan", Action="read")]
        public ActionResult View(int id)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            //cari
            AdminUangJalan model = new AdminUangJalan();
            if (dbitem.AdminUangJalanId.HasValue)
                model = new AdminUangJalan(dbitem.AdminUangJalan, RepoAtm.FindAll(), RepoBor.FindAll());

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
                ViewBag.Title = "Admin Uang Jalan " + dbitem.SalesOrderOncall.SONumber;
                //ViewBag.postData = "EditOncall";
                return View("View", model);
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
                ViewBag.Title = "Admin Uang Jalan " + dbitem.SalesOrderPickup.SONumber;
                //ViewBag.postData = "EditPickup";
                return View("View", model);
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue)
            {
                model.ModelKonsolidasi = new SalesOrderProsesKonsolidasi(dbitem);
                return View("Form", model);
            }

            return View("");
        }
        [MyAuthorize(Menu = "Admin Uang Jalan", Action="read")]
        public ActionResult ViewKontrak(int id, string listSo)
        {

            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            //ambil admin uang jalan nya dari listSo yang pertama
            List<int> ListIdDumy = listSo.Split(new string[] { "." }, StringSplitOptions.None).ToList().Select(int.Parse).ToList();
            List<Context.SalesOrderKontrakListSo> dbsoDummy = dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => ListIdDumy.Contains(d.Id)).ToList();
            dbitem.SalesOrderKontrak.SalesOrderKontrakListSo = dbsoDummy;
            AdminUangJalan model = new AdminUangJalan();

            if (dbsoDummy.FirstOrDefault().IdAdminUangJalan.HasValue)
                model = new AdminUangJalan(dbsoDummy.FirstOrDefault().AdminUangJalan, RepoAtm.FindAll(), RepoBor.FindAll());

            model.IdSalesOrder = dbitem.Id;
            GenerateModel(model, dbitem);
            model.ModelKontrak.ListValueModelSOKontrak = model.ModelKontrak.ListModelSOKontrak;

            return View("ViewKontrak", model);
        }

        [HttpPost]
        public ActionResult Edit(AdminUangJalan model, string btnsave)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(model.IdSalesOrder.Value);
            Context.AdminUangJalan db = new Context.AdminUangJalan();
            AdminUangJalanVoucherSpbu[] resSpbu = JsonConvert.DeserializeObject<AdminUangJalanVoucherSpbu[]>(model.StrSolar);
            model.ModelListSpbu = resSpbu.ToList();
            AdminUangJalanVoucherKapal[] resKapal = JsonConvert.DeserializeObject<AdminUangJalanVoucherKapal[]>(model.StrKapal);
            model.ModelListKapal = resKapal.ToList();
            AdminUangJalanUangTf[] resUang = JsonConvert.DeserializeObject<AdminUangJalanUangTf[]>(model.StrUang);
            model.ModelListTf = resUang.ToList();
            if (ModelState.IsValid && (dbitem.Status.ToLower().Contains("konfirmasi") || dbitem.Status == "save planning"))
            {
                if (validation(model))
                {
                    int idAdm = 0;
                    if (dbitem.AdminUangJalanId.HasValue)
                    {
                        idAdm = dbitem.AdminUangJalanId.Value;
                        model.setDb(dbitem.AdminUangJalan);
                    }
                    else
                    {
                        model.setDb(db);
                        db.Code =  "AUJ-" + (dbitem.SalesOrderOncallId.HasValue ? dbitem.SalesOrderOncall.SONumber : dbitem.SalesOrderProsesKonsolidasiId.HasValue ? 
                            dbitem.SalesOrderProsesKonsolidasi.SONumber : dbitem.SalesOrderPickupId.HasValue ? dbitem.SalesOrderPickup.SONumber : dbitem.SalesOrderKontrak.SONumber);
                        dbitem.AdminUangJalan = db;
                    }
                    if (btnsave == "Submit"){
                        Context.ERPConfig erpConfig = RepoERPConfig.FindByFrist();
                        decimal? tambahanRute = db.AdminUangJalanTambahanRute.Sum(s => s.values);
                        decimal? boronganDasar = db.TotalBorongan - (db.Kawalan ?? 0) - (db.Timbangan ?? 0) - (db.Karantina ?? 0)-(db.SPSI ?? 0)-(db.Multidrop ?? 0)-tambahanRute-db.AdminUangJalanTambahanLain.Sum(s => s.Values);
                        decimal? total_potongan_driver = (db.AdminUangJalanPotonganDriver.Sum(s => s.Value) + db.KasbonDriver1);
                        Repoglt_det.saveFromAc(1, db.Code, boronganDasar, 0, Repoac_mstr.FindByPk(erpConfig.IdBoronganDasar));
                        Repoglt_det.saveFromAc(2, db.Code, db.Kawalan, 0, Repoac_mstr.FindByPk(erpConfig.IdKawalan));
                        Repoglt_det.saveFromAc(3, db.Code, db.Timbangan, 0, Repoac_mstr.FindByPk(erpConfig.IdTimbangan));
                        Repoglt_det.saveFromAc(4, db.Code, db.Karantina, 0, Repoac_mstr.FindByPk(erpConfig.IdKarantina));
                        Repoglt_det.saveFromAc(5, db.Code, db.SPSI, 0, Repoac_mstr.FindByPk(erpConfig.IdSPSI));
                        Repoglt_det.saveFromAc(6, db.Code, db.Multidrop, 0, Repoac_mstr.FindByPk(erpConfig.IdMultidrop));
                        Repoglt_det.saveFromAc(7, db.Code, tambahanRute + db.AdminUangJalanTambahanLain.Sum(s => s.Values), 0, Repoac_mstr.FindByPk(erpConfig.IdTambahanRuteMuat));
                        Repoglt_det.saveFromAc(
                            8, db.Code, 0,
                            (db.KasbonDriver1 ?? 0) + (db.KlaimDriver1 ?? 0) + db.AdminUangJalanPotonganDriver.Select(d => d.Value).Sum()+db.AdminUangJalanVoucherKapal.Select(d => d.Value).Sum() +
                            db.AdminUangJalanVoucherSpbu.Select(d => d.Value).Sum() + db.AdminUangJalanUangTf.Select(d => d.Value).Sum(),
                            Repoac_mstr.FindByPk(erpConfig.IdAUJCredit)
                        );

                        try{
                            var glt_oid = Guid.NewGuid().ToString();
                            if (total_potongan_driver > 0){
                                Repopbyd_det.saveMstr(glt_oid, db.Code, erpConfig.IdTfCredit.Value, "Tabungan " + db.Code, db.IdDriver1.Value);
                                Repopbyd_det.save(
                                    glt_oid, db.Code, erpConfig.IdTfCredit.Value, "Tabungan " + db.Code, db.IdDriver1.Value, erpConfig.IdKasbonDriver.Value,
                                    Repoac_mstr.FindByPk(erpConfig.IdKasbonDriver).ac_name, total_potongan_driver.Value*-1
                                );
                            }
                        }
                        catch (Exception e)
                        {
                        }

                        dbitem.Status = "admin uang jalan";
                    }
                    dbitem.UpdatedBy = UserPrincipal.id;
                    RepoSalesOrder.save(dbitem);
                    //simpan history driver dan truck
                    Context.HistoryJalanTruck dbhistruck = RepoHistoryJalanTruck.FindByAdm(dbitem.AdminUangJalanId.Value);
                    if (btnsave == "Submit"){
                        if (dbhistruck == null)
                        {
                            dbhistruck = new Context.HistoryJalanTruck();
                        }
                        dbhistruck.IdAdminUangJalan = dbitem.AdminUangJalanId.Value;
                        dbhistruck.IdDriver1 = dbitem.AdminUangJalan.IdDriver1.Value;
                        dbhistruck.IdDriver2 = dbitem.AdminUangJalan.IdDriver2;
                        if (dbitem.SalesOrderOncallId.HasValue)
                        {
                            dbhistruck.IdTruck = dbitem.SalesOrderOncall.IdDataTruck.Value;
                            dbhistruck.ShipmentId = dbitem.SalesOrderOncall.DN;
                            dbhistruck.NoSo = dbitem.SalesOrderOncall.SONumber;
                            dbhistruck.TanggalMuat = dbitem.SalesOrderOncall.TanggalMuat.Value;
                            dbhistruck.JenisOrder = dbitem.SalesOrderOncall.PrioritasId.HasValue ? dbitem.SalesOrderOncall.LookUpPrioritas.Nama : "Oncall";
                            dbhistruck.Rute = dbitem.SalesOrderOncall.StrDaftarHargaItem;
                        }
                        else if (dbitem.SalesOrderPickupId.HasValue)
                        {
                            dbhistruck.IdTruck = dbitem.SalesOrderPickup.IdDataTruck.Value;
                            dbhistruck.ShipmentId = dbitem.SalesOrderPickup.SONumber;
                            dbhistruck.TanggalMuat = dbitem.SalesOrderPickup.TanggalPickup;
                            dbhistruck.JenisOrder = "Pickup";
                            dbhistruck.Rute = dbitem.SalesOrderPickup.Rute.Nama;
                        }
                        else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue)
                        {
                            dbhistruck.IdTruck = dbitem.SalesOrderProsesKonsolidasi.IdDataTruck.Value;
                            dbhistruck.ShipmentId = dbitem.SalesOrderProsesKonsolidasi.DN;
                            dbhistruck.NoSo = dbitem.SalesOrderProsesKonsolidasi.SONumber;
                            dbhistruck.TanggalMuat = dbitem.SalesOrderProsesKonsolidasi.TanggalMuat.Value;
                            dbhistruck.JenisOrder = "Konsolidasi";
                            dbhistruck.Rute = dbitem.SalesOrderProsesKonsolidasi.StrDaftarHargaItem;
                        }
                        Context.SettlementBatal sb = RepoSettlementBatal.FindAll().Where(
                            d => d.IsProses == true && d.IdAdminUangJalan == null && dbitem.AdminUangJalan.IdDriver1.Value == d.IdDriver
                        ).FirstOrDefault();
                        if (sb != null){
                            sb.IdAdminUangJalan = dbitem.AdminUangJalanId;
                            RepoSettlementBatal.save(sb, UserPrincipal.id, "Admin Uang Jalan");
                        }
                        RepoHistoryJalanTruck.save(dbhistruck);
                    }
                    RepoAuditrail.saveAUJHistory(dbitem, dbhistruck);
                    if (btnsave == "Submit")
                        return RedirectToAction("Index");
                    else
                        return RedirectToAction("Edit", new {Id = dbitem.Id});
                }
            }

            if (model.ModelOncall != null)
                ViewBag.Title = "Admin Uang Jalan " + model.ModelOncall.SONumber;
            if (model.ModelPickup != null)
                ViewBag.Title = "Admin Uang Jalan " + model.ModelPickup.SONumber;
            if (model.ModelKonsolidasi != null)
                ViewBag.Title = "Admin Uang Jalan " + model.ModelKonsolidasi.SONumber;
            ViewBag.Status = dbitem.Status.ToLower().Contains("konfirmasi") || dbitem.Status == "save planning" ? "data borongan belum diset" : dbitem.Status.ToLower().Contains("admin uang jalan") ?
                "sudah diproses" : "batal";
            GenerateModel(model, dbitem);
            return View("Form", model);
        }
        [HttpPost]
        public ActionResult EditRemoval(RemovalAUJ model, int idRev)
        {
            AdminUangJalanVoucherSpbu[] resSpbu = JsonConvert.DeserializeObject<AdminUangJalanVoucherSpbu[]>(model.StrSolar);
            model.ModelListSpbu = resSpbu.ToList();
            AdminUangJalanVoucherKapal[] resKapal = JsonConvert.DeserializeObject<AdminUangJalanVoucherKapal[]>(model.StrKapal);
            model.ModelListKapal = resKapal.ToList();
            AdminUangJalanUangTf[] resUang = JsonConvert.DeserializeObject<AdminUangJalanUangTf[]>(model.StrUang);
            model.ModelListTf = resUang.ToList();
            if (ModelState.IsValid)
            {
                if (validationRemoval(model)) 
                {
                    Context.Removal dbitem = RepoRemovalRepo.FindByPK(idRev);
                    model.setDb(dbitem);
                    dbitem.Status = "admin uang jalan";
                    RepoRemovalRepo.save(dbitem);
                    return RedirectToAction("Index", "AdminUangJalan");
                }
            }

            Context.SalesOrder dbso = RepoSalesOrder.FindByPK(model.IdSO.Value);
            if (dbso.SalesOrderOncallId.HasValue)
            {
                ViewBag.driverId = dbso.SalesOrderOncall.Driver1Id;
                ViewBag.driverName = dbso.SalesOrderOncall.Driver1.KodeDriver + " - " + dbso.SalesOrderOncall.Driver1.NamaDriver;
            }
            else if (dbso.SalesOrderPickupId.HasValue)
            {
                ViewBag.driverId = dbso.SalesOrderPickup.Driver1Id;
                ViewBag.driverName = dbso.SalesOrderPickup.Driver1.KodeDriver + " - " + dbso.SalesOrderPickup.Driver1.NamaDriver;
            }
            else if (dbso.SalesOrderProsesKonsolidasiId.HasValue)
            {
                ViewBag.driverId = dbso.SalesOrderProsesKonsolidasi.Driver1Id;
                ViewBag.driverName = dbso.SalesOrderProsesKonsolidasi.Driver1.KodeDriver + " - " + dbso.SalesOrderProsesKonsolidasi.Driver1.NamaDriver;
            }
            Context.SettlementBatal sb = RepoSettlementBatal.FindAll().Where(
                d => d.IsProses == true && d.IdAdminUangJalan == null && d.SalesOrder != null &&
                (
                    d.SalesOrder.SalesOrderOncallId.HasValue && (d.SalesOrder.SalesOrderOncall.Driver1Id == ViewBag.driverId || d.SalesOrder.SalesOrderOncall.Driver2Id == ViewBag.driverId ||
                    d.SalesOrder.SalesOrderOncall.Driver2Id == ViewBag.driverId || d.SalesOrder.SalesOrderOncall.Driver1Id == ViewBag.driverId) ||
                    d.SalesOrder.SalesOrderPickupId.HasValue && (d.SalesOrder.SalesOrderPickup.Driver1Id == ViewBag.driverId || d.SalesOrder.SalesOrderPickup.Driver2Id == ViewBag.driverId ||
                    d.SalesOrder.SalesOrderPickup.Driver2Id == ViewBag.driverId || d.SalesOrder.SalesOrderPickup.Driver1Id == ViewBag.driverId) ||
                    d.SalesOrder.SalesOrderProsesKonsolidasiId.HasValue && (d.SalesOrder.SalesOrderProsesKonsolidasi.Driver1Id == ViewBag.driverId ||
                    d.SalesOrder.SalesOrderProsesKonsolidasi.Driver2Id == ViewBag.driverId || d.SalesOrder.SalesOrderProsesKonsolidasi.Driver2Id == ViewBag.driverId ||
                    d.SalesOrder.SalesOrderProsesKonsolidasi.Driver1Id == ViewBag.driverId)
                )
            ).FirstOrDefault();
            ViewBag.SisaPerjalananSblmny = 0;
            if (sb != null)
            {
                ViewBag.SisaPerjalananSblmny += sb.TransferSelisih == null ? 0 : sb.TransferSelisih;
                ViewBag.SisaPerjalananSblmny += sb.KasSelisih == null ? 0 : sb.KasSelisih;
                ViewBag.SisaPerjalananSblmny += sb.SolarSelisih == null ? 0 : sb.SolarSelisih;
                if (sb.SalesOrder.SalesOrderOncallId.HasValue)
                    ViewBag.SOSblmny = sb.SalesOrder.SalesOrderOncall.SONumber;
                else if (sb.SalesOrder.SalesOrderPickupId.HasValue)
                    ViewBag.SOSblmny = sb.SalesOrder.SalesOrderPickup.SONumber;
                else if (sb.SalesOrder.SalesOrderProsesKonsolidasiId.HasValue)
                    ViewBag.SOSblmny = sb.SalesOrder.SalesOrderProsesKonsolidasi.SONumber;
            }
            AdminUangJalan modelauj = new AdminUangJalan();
            if (dbso.AdminUangJalanId.HasValue)
                modelauj = new AdminUangJalan(dbso.AdminUangJalan, RepoAtm.FindAll(), RepoBor.FindAll());

            modelauj.IdSalesOrder = dbso.Id;
            GenerateModel(modelauj, dbso);

            var dummyModel = modelauj.ModelListRemoval.Where(d => d.Id == model.Id).FirstOrDefault();
            var idx = modelauj.ModelListRemoval.IndexOf(dummyModel);
            modelauj.ModelListRemoval[idx] = model;

            return View("~/Views/Removal/FormRemoval.cshtml", model);
        }

        [HttpPost]
        public ActionResult EditKontrak(AdminUangJalan model, string btnsave)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(model.IdSalesOrder.Value);
            Context.AdminUangJalan db = new Context.AdminUangJalan();
            string bbtnsave = btnsave;
            string bbbtnsave = bbtnsave;

            SalesOrderKontrakListSo[] resSo = JsonConvert.DeserializeObject<SalesOrderKontrakListSo[]>(model.SelectedListIdSo);
            SalesOrderKontrakListSo[] resSelSo = JsonConvert.DeserializeObject<SalesOrderKontrakListSo[]>(model.SelectedListIdSo);
            AdminUangJalanVoucherSpbu[] resSpbu = JsonConvert.DeserializeObject<AdminUangJalanVoucherSpbu[]>(model.StrSolar);
            model.ModelListSpbu = resSpbu.ToList();
            AdminUangJalanVoucherKapal[] resKapal = JsonConvert.DeserializeObject<AdminUangJalanVoucherKapal[]>(model.StrKapal);
            model.ModelListKapal = resKapal.ToList();
            AdminUangJalanUangTf[] resUang = JsonConvert.DeserializeObject<AdminUangJalanUangTf[]>(model.StrUang);
            model.ModelListTf = resUang.ToList();

            List<int> SelectedSo = resSelSo.Select(d => d.Id).ToList();
            List<int> UnSelectedSo = resSo.Where(s => !SelectedSo.Contains(s.Id)).Select(d => d.Id).ToList();
            int urutListSo = (RepoSalesOrderKontrakListSo.getUrutanProses(dbitem.SalesOrderKontrakId)) + 1;

            List<int> ListIdDumy = resSo.Select(d => d.Id).ToList();
            List<Context.SalesOrderKontrakListSo> dbsoDummy = dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => ListIdDumy.Contains(d.Id)).ToList();

            if (validation(model))
            {
                //bikin dulu data admin uang jalan
                model.setDb(db);
                int idAdm = 0;
                if (dbitem.AdminUangJalanId.HasValue)
                {
                    idAdm = dbitem.AdminUangJalanId.Value;
                    model.setDb(dbitem.AdminUangJalan);
                }
                else
                {
                    model.setDb(db);
                    db.Code =  "AUJ-" + (dbitem.SalesOrderOncallId.HasValue ? dbitem.SalesOrderOncall.SONumber : dbitem.SalesOrderProsesKonsolidasiId.HasValue ? 
                        dbitem.SalesOrderProsesKonsolidasi.SONumber : dbitem.SalesOrderPickupId.HasValue ? dbitem.SalesOrderPickup.SONumber : dbitem.SalesOrderKontrak.SONumber);
                    dbitem.AdminUangJalan = db;
                }
                //setiap so isi admin uang jalannya
                if (btnsave == "Submit"){
                    Context.ERPConfig erpConfig = RepoERPConfig.FindByFrist();
                    decimal? tambahanRute = db.AdminUangJalanTambahanRute.Sum(s => s.values);
                    decimal? boronganDasar = db.TotalBorongan - (db.Kawalan ?? 0) - (db.Timbangan ?? 0) - (db.Karantina ?? 0)-(db.SPSI ?? 0)-(db.Multidrop ?? 0)-tambahanRute-db.AdminUangJalanTambahanLain.Sum(s => s.Values);
                    decimal? total_potongan_driver = (db.AdminUangJalanPotonganDriver.Sum(s => s.Value) + db.KasbonDriver1);
                    Repoglt_det.saveFromAc(1, db.Code, boronganDasar, 0, Repoac_mstr.FindByPk(erpConfig.IdBoronganDasar));
                    Repoglt_det.saveFromAc(2, db.Code, db.Kawalan, 0, Repoac_mstr.FindByPk(erpConfig.IdKawalan));
                    Repoglt_det.saveFromAc(3, db.Code, db.Timbangan, 0, Repoac_mstr.FindByPk(erpConfig.IdTimbangan));
                    Repoglt_det.saveFromAc(4, db.Code, db.Karantina, 0, Repoac_mstr.FindByPk(erpConfig.IdKarantina));
                    Repoglt_det.saveFromAc(5, db.Code, db.SPSI, 0, Repoac_mstr.FindByPk(erpConfig.IdSPSI));
                    Repoglt_det.saveFromAc(6, db.Code, db.Multidrop, 0, Repoac_mstr.FindByPk(erpConfig.IdMultidrop));
                    Repoglt_det.saveFromAc(7, db.Code, tambahanRute + db.AdminUangJalanTambahanLain.Sum(s => s.Values), 0, Repoac_mstr.FindByPk(erpConfig.IdTambahanRuteMuat));
                    Repoglt_det.saveFromAc(
                        8, db.Code, 0,
                        (db.KasbonDriver1 ?? 0) + (db.KlaimDriver1 ?? 0) + db.AdminUangJalanPotonganDriver.Select(d => d.Value).Sum()+db.AdminUangJalanVoucherKapal.Select(d => d.Value).Sum() +
                        db.AdminUangJalanVoucherSpbu.Select(d => d.Value).Sum() + db.AdminUangJalanUangTf.Select(d => d.Value).Sum(),
                        Repoac_mstr.FindByPk(erpConfig.IdAUJCredit)
                    );

                    try{
                        var glt_oid = Guid.NewGuid().ToString();
                        if (total_potongan_driver > 0){
                            Repopbyd_det.saveMstr(glt_oid, db.Code, erpConfig.IdTfCredit.Value, "Tabungan " + db.Code, db.IdDriver1.Value);
                            Repopbyd_det.save(
                                glt_oid, db.Code, erpConfig.IdTfCredit.Value, "Tabungan " + db.Code, db.IdDriver1.Value, erpConfig.IdKasbonDriver.Value,
                                Repoac_mstr.FindByPk(erpConfig.IdKasbonDriver).ac_name, total_potongan_driver.Value*-1
                            );
                        }
                    }
                    catch (Exception e)
                    {
                    }

                    dbitem.Status = "admin uang jalan";
                    foreach (Context.SalesOrderKontrakListSo item in dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => SelectedSo.Contains(d.Id)))
                    {
                        item.Status = "admin uang jalan";
                        item.AdminUangJalan = db;
                        item.Urutan = urutListSo;
                    }
                    //rubah status yang tidak dipilih
                    foreach (Context.SalesOrderKontrakListSo item in dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => UnSelectedSo.Contains(d.Id)))
                    {
                        item.Status = "save konfirmasi";
                        item.IdAdminUangJalan = null;
                        item.Urutan = 0;
                    }
                }
                RepoSalesOrder.save(dbitem);
                //simpan history dirver
                Context.HistoryJalanTruck dbhisdriver = RepoHistoryJalanTruck.FindByAdm(db.Id);
                if (dbhisdriver == null)
                {
                    dbhisdriver = new Context.HistoryJalanTruck();
                }
                dbhisdriver.IdAdminUangJalan = db.Id;
                dbhisdriver.IdDriver1 = db.IdDriver1.Value;
                dbhisdriver.IdDriver2 = db.IdDriver2;
                dbhisdriver.IdTruck = dbsoDummy.FirstOrDefault().IdDataTruck.Value;
                dbhisdriver.NoSo = string.Join(", ", dbsoDummy.Select(s => s.NoSo).ToList());
                dbhisdriver.TanggalMuat = dbsoDummy.FirstOrDefault().MuatDate;
                dbhisdriver.JenisOrder = "Kontrak";
                RepoHistoryJalanTruck.save(dbhisdriver);
                RepoAuditrail.saveAUJHistory(dbitem, dbhisdriver);
                return RedirectToAction("IndexKontrak");
            }

            ViewBag.Title = "Admin Uang Jalan " + model.ModelKontrak.SONumber;
            dbitem.SalesOrderKontrak.SalesOrderKontrakListSo = dbsoDummy;
            GenerateModel(model, dbitem);
            model.ModelKontrak.ListValueModelSOKontrak = resSo.ToList();
            model.ModelKontrak.ListValueModelSOKontrak = resSelSo.ToList(); 
            return View("FormKontrak", model);
        }

        [MyAuthorize(Menu = "Admin Uang Jalan", Action="print")]
        public ActionResult Print(int idSo, string listSo = "")
        {
            return new Rotativa.ActionAsPdf("ShowPrint", new { idSo = idSo, listSo = listSo })
            {
                FileName = "adminuangjalan.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = { Left = 0, Right = 0 },
                PageOrientation = Rotativa.Options.Orientation.Portrait
            };
        }
        [MyAuthorize(Menu = "Admin Uang Jalan", Action="print")]
        public ActionResult PrintSuratJalan(int idSo, string listSo = "")
        {
            return new Rotativa.ActionAsPdf("ShowPrintSuratJalan", new { idSo = idSo, listSo = listSo })
            {
                FileName = "suratjalan.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = { Left = 0, Right = 0 },
                PageOrientation = Rotativa.Options.Orientation.Portrait
            };
        }
        [MyAuthorize(Menu = "Admin Uang Jalan", Action="print")]
        public ActionResult PrintVoucher(int idSo, string listSo = "")
        {
            return new Rotativa.ActionAsPdf("ShowPrintVoucher", new { idSo = idSo, listSo = listSo })
            {
                FileName = "voucher.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = { Left = 0, Right = 0 , Top = 9 },
                PageOrientation = Rotativa.Options.Orientation.Portrait
            };
        }
        public ActionResult ShowPrint(int idSo, string listSo)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(idSo);
            AdminUangJalan model = new AdminUangJalan();

            if (!dbitem.SalesOrderKontrakId.HasValue)
            {
                if (dbitem.AdminUangJalanId.HasValue)
                    model = new AdminUangJalan(dbitem.AdminUangJalan, RepoAtm.FindAll(), RepoBor.FindAll());

                model.IdSalesOrder = dbitem.Id;
                GenerateModel(model, dbitem);

                ViewBag.kodedr = dbitem.AdminUangJalan.Driver1.KodeDriver;
                ViewBag.namadr = dbitem.AdminUangJalan.Driver1.NamaDriver;
            }


            if(dbitem.SalesOrderOncallId.HasValue)
            {
                ViewBag.nopol = dbitem.SalesOrderOncall.DataTruck.VehicleNo;
                ViewBag.tgljalan = dbitem.SalesOrderOncall.TanggalMuat;
                ViewBag.customer = dbitem.SalesOrderOncall.Customer.CustomerNama;
                ViewBag.rute = dbitem.SalesOrderOncall.StrDaftarHargaItem;
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue)
            {
                ViewBag.nopol = dbitem.SalesOrderProsesKonsolidasi.DataTruck.VehicleNo;
                ViewBag.tgljalan = dbitem.SalesOrderProsesKonsolidasi.TanggalMuat;
                ViewBag.customer = "";
                ViewBag.rute = dbitem.SalesOrderProsesKonsolidasi.StrDaftarHargaItem;
            }
            else if (dbitem.SalesOrderPickupId.HasValue)
            {
                ViewBag.nopol = dbitem.SalesOrderPickup.DataTruck.VehicleNo;
                ViewBag.tgljalan = dbitem.SalesOrderPickup.TanggalPickup;
                ViewBag.customer = dbitem.SalesOrderPickup.Customer.CustomerNama;
                ViewBag.rute = dbitem.SalesOrderPickup.Rute.Nama;
            }
            else if (dbitem.SalesOrderKontrakId.HasValue)
            {
                List<int> ListIdDumy = listSo.Split(new string[] { "." }, StringSplitOptions.None).ToList().Select(int.Parse).ToList();
                List<Context.SalesOrderKontrakListSo> dbsoDummy = dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => ListIdDumy.Contains(d.Id)).ToList();
                dbitem.SalesOrderKontrak.SalesOrderKontrakListSo = dbsoDummy;

                if (dbsoDummy.FirstOrDefault().IdAdminUangJalan.HasValue)
                    model = new AdminUangJalan(dbsoDummy.FirstOrDefault().AdminUangJalan, RepoAtm.FindAll(), RepoBor.FindAll());

                model.IdSalesOrder = dbitem.Id;
                GenerateModel(model, dbitem);

                ViewBag.kodedr = dbsoDummy.FirstOrDefault().AdminUangJalan.Driver1.KodeDriver;
                ViewBag.namadr = dbsoDummy.FirstOrDefault().AdminUangJalan.Driver1.NamaDriver;

                ViewBag.nopol = dbsoDummy.FirstOrDefault().DataTruck.VehicleNo;
                ViewBag.tgljalan = dbsoDummy.FirstOrDefault().MuatDate;
                ViewBag.customer = dbitem.SalesOrderKontrak.Customer.CustomerNama;
                //ViewBag.rute = dbitem.SalesOrderPickup.Rute.Nama;
            }
            
            if(model.ModelListRemoval.Count > 0)
            {
                model.ModelListRemoval = model.ModelListRemoval.Where(r => r.Status != "" && r.Status != null).OrderBy(d => d.Id).ToList();
                if (model.ModelListRemoval.Count > 0)
                    return View("~/Views/Removal/Print.cshtml", model.ModelListRemoval.Last());
                else
                    return View("Print", model);
            }
            else
                return View("Print",model);
        }
        public ActionResult ShowPrintSuratJalan(int idSo, string listSo)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(idSo);

            if(dbitem.SalesOrderOncallId.HasValue)
            {
                ViewBag.cust = dbitem.SalesOrderOncall.Customer.CustomerNama;
                int IdJenisTruck;
                string IdRute;

                RepoDHO.FindRuteTruk(dbitem.SalesOrderOncall.IdDaftarHargaItem.Value, out IdRute, out IdJenisTruck);
                if (IdRute != null && IdRute != "")
                {
                    var dummyRute = RepoRute.FindByPK(int.Parse(IdRute.Split(',')[0]));
                    ViewBag.dari = dummyRute.LocationAsal.Nama;
                    ViewBag.tujuan = dummyRute.LocationTujuan.Nama;
                }
                ViewBag.nopol = dbitem.SalesOrderOncall.DataTruck.VehicleNo;
                ViewBag.type = dbitem.SalesOrderOncall.DataTruck.JenisTrucks.StrJenisTruck;
                ViewBag.barang = dbitem.SalesOrderOncall.MasterProduct.NamaProduk;
                ViewBag.tgl = dbitem.SalesOrderOncall.TanggalMuat.Value.ToShortDateString();
                ViewBag.namadr = dbitem.AdminUangJalan.Driver1.NamaDriver;
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue)
            {
                SalesOrderProsesKonsolidasi modelKonsolidasi = new SalesOrderProsesKonsolidasi(dbitem);
                var dummyRute = RepoRute.FindByPK(modelKonsolidasi.RuteId.Value);
                ViewBag.dari = dummyRute.LocationAsal.Nama;
                ViewBag.tujuan = dummyRute.LocationTujuan.Nama;
                ViewBag.tgl = modelKonsolidasi.TanggalMuat.Value.ToShortDateString();
                ViewBag.namadr = dbitem.AdminUangJalan.Driver1.NamaDriver;
                ViewBag.nopol = modelKonsolidasi.VehicleNo;

                return View("PrintSJKonsolidasi", modelKonsolidasi);
            }
            else if (dbitem.SalesOrderPickupId.HasValue)
            {
                ViewBag.cust = dbitem.SalesOrderPickup.Customer.CustomerNama;
                ViewBag.dari = dbitem.SalesOrderPickup.Rute.LocationAsal.Nama;
                ViewBag.tujuan = dbitem.SalesOrderPickup.Rute.LocationTujuan.Nama;
                ViewBag.nopol = dbitem.SalesOrderPickup.DataTruck.VehicleNo;
                ViewBag.type = dbitem.SalesOrderPickup.DataTruck.JenisTrucks.StrJenisTruck;
                ViewBag.barang = dbitem.SalesOrderPickup.MasterProduct.NamaProduk;
                ViewBag.tgl = dbitem.SalesOrderPickup.TanggalPickup.ToShortDateString();
                ViewBag.namadr = dbitem.AdminUangJalan.Driver1.NamaDriver;
            }
            else if (dbitem.SalesOrderKontrakId.HasValue)
            {
                List<int> ListIdDumy = listSo.Split(new string[] { "." }, StringSplitOptions.None).ToList().Select(int.Parse).ToList();
                List<Context.SalesOrderKontrakListSo> dbsoDummy = dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => ListIdDumy.Contains(d.Id)).ToList();
                ViewBag.cust = dbitem.SalesOrderKontrak.Customer.CustomerNama;
                ViewBag.dari = "";
                ViewBag.tujuan = "";
                ViewBag.nopol = dbsoDummy.FirstOrDefault().DataTruck.VehicleNo;
                ViewBag.type = dbsoDummy.FirstOrDefault().DataTruck.JenisTrucks.StrJenisTruck;
                ViewBag.barang = "";
                ViewBag.tgl = dbsoDummy.FirstOrDefault().MuatDate.ToShortDateString();
                ViewBag.namadr = dbsoDummy.FirstOrDefault().AdminUangJalan.Driver1.NamaDriver;
            }

            return View("PrintSuratJalan");
        }
        public ActionResult ShowPrintVoucher(int idSo, string listSo)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(idSo);
            if (dbitem.SalesOrderOncallId.HasValue){
                ViewBag.driverId = dbitem.SalesOrderOncall.Driver1Id;
                ViewBag.driverName = dbitem.SalesOrderOncall.Driver1.KodeDriver + " - " + dbitem.SalesOrderOncall.Driver1.NamaDriver;
                ViewBag.TanggalMuat = dbitem.SalesOrderOncall.TanggalMuat.Value.ToShortDateString();
                ViewBag.VehicleNo = dbitem.SalesOrderOncall.DataTruck.VehicleNo;
                ViewBag.Jenis = RepoLookupCode.FindByPK(dbitem.SalesOrderOncall.DataTruck.IdMerk).Nama;
                Context.DataBorongan bor = RepoBor.FindByPK(int.Parse(dbitem.AdminUangJalan.IdDataBorongan));
                ViewBag.Liter = bor.LiterSolar;
            }
            else if (dbitem.SalesOrderPickupId.HasValue){
                ViewBag.driverId = dbitem.SalesOrderPickup.Driver1Id;
                ViewBag.driverName = dbitem.SalesOrderPickup.Driver1.KodeDriver + " - " + dbitem.SalesOrderPickup.Driver1.NamaDriver;
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue){
                ViewBag.driverId = dbitem.SalesOrderProsesKonsolidasi.Driver1Id;
                ViewBag.driverName = dbitem.SalesOrderProsesKonsolidasi.Driver1.KodeDriver + " - " + dbitem.SalesOrderProsesKonsolidasi.Driver1.NamaDriver;
            }
            Context.SettlementBatal sb = RepoSettlementBatal.FindAll().Where(
                d => d.IsProses == true && d.IdAdminUangJalan == null && d.SalesOrder != null && d.IdDriver == ViewBag.driverId
            ).FirstOrDefault();
            ViewBag.SisaPerjalananSblmny = 0;
            if (sb != null)
            {
                ViewBag.SisaPerjalananSblmny += sb.TransferSelisih == null ? 0 : sb.TransferSelisih;
                ViewBag.SisaPerjalananSblmny += sb.KasSelisih == null ? 0 : sb.KasSelisih;
                ViewBag.SisaPerjalananSblmny += sb.SolarSelisih == null ? 0 : sb.SolarSelisih;
                if (sb.SalesOrder.SalesOrderOncallId.HasValue)
                    ViewBag.SOSblmny = sb.SalesOrder.SalesOrderOncall.SONumber;
                else if (sb.SalesOrder.SalesOrderPickupId.HasValue)
                    ViewBag.SOSblmny = sb.SalesOrder.SalesOrderPickup.SONumber;
                else if (sb.SalesOrder.SalesOrderProsesKonsolidasiId.HasValue)
                    ViewBag.SOSblmny = sb.SalesOrder.SalesOrderProsesKonsolidasi.SONumber;
                else if (sb.SalesOrder.SalesOrderKontrakId.HasValue)
                    ViewBag.SOSblmny = sb.SalesOrder.SalesOrderKontrak.SONumber;
            }
            AdminUangJalan model = new AdminUangJalan();
            if (dbitem.AdminUangJalanId.HasValue)
                model = new AdminUangJalan(dbitem.AdminUangJalan, RepoAtm.FindAll(), RepoBor.FindAll());
            model.IdSalesOrder = dbitem.Id;
            GenerateModel(model, dbitem);

            if (model.ModelListRemoval.Count > 0)
            {
                model.ModelListRemoval = model.ModelListRemoval.OrderBy(d => d.Id).ToList();
                return View("~/Views/Removal/FormRemoval.cshtml", model);
            }
            else
                return View("PrintVoucher", model);
        }
        public ActionResult ShowPrintSJKonsolidasi(int idSo)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(idSo);
            SalesOrderProsesKonsolidasi model = new SalesOrderProsesKonsolidasi(dbitem);
            var dummyRute = RepoRute.FindByPK(model.RuteId.Value);
            ViewBag.dari = dummyRute.LocationAsal.Nama;
            ViewBag.tujuan = dummyRute.LocationTujuan.Nama;
            ViewBag.tgl = model.TanggalMuat.Value.ToShortDateString();
            ViewBag.namadr = dbitem.AdminUangJalan.Driver1.NamaDriver;
            ViewBag.nopol = model.VehicleNo;            
            return View("PrintSJKonsolidasi", model);
        }
    }
}