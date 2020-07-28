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
    public class ReportController : BaseController
    {
        private ISalesOrderRepo RepoSalesOrder;
        private IRevisiTanggalRepo RepoRevisiTanggal;
        private ISettlementBatalRepo RepoSettBatal;
        private IBatalOrderRepo RepoBatalOrder;
        private IUserRepo RepoUser;
        private IDaftarHargaOnCallRepo RepoDHO;
        private IDaftarHargaKonsolidasiRepo RepoDHK;
        private IRuteRepo RepoRute;
        private IAtmRepo RepoAtm;
        private IDataBoronganRepo RepoBor;
        private ISalesOrderKontrakListSoRepo RepoSalesOrderKontrakListSo;
        private IHistoryJalanTruckRepo RepoHistoryJalanTruck;
        private IRemovalRepo RepoRemovalRepo;
        private IDokumenRepo RepoDokumen;
        private IRemovalRepo RepoRemoval;
        private IAtmRepo Repoatm;
        private ISolarInapRepo RepoSolarInap;

        public ReportController(IUserReferenceRepo repoBase, ILookupCodeRepo repoLookup, ISalesOrderRepo repoSalesOrder, IRevisiTanggalRepo repoRevisiTanggal, ISettlementBatalRepo repoSettBatal,
            IBatalOrderRepo repoBatalOrder, IUserRepo repoUser, IDaftarHargaKonsolidasiRepo repoDHK, IRuteRepo repoRute, IAtmRepo repoAtm, IDataBoronganRepo repoBor, IAtmRepo repoatm,
            ISalesOrderKontrakListSoRepo repoSalesOrderKontrakListSo, IHistoryJalanTruckRepo repoHistoryJalanTruck, IRemovalRepo repoRemovalRepo, IDaftarHargaOnCallRepo repoDHO, IDokumenRepo repoDokumen,
            IRemovalRepo repoRemoval, ISolarInapRepo repoSolarInap)
            : base(repoBase, repoLookup)
        {
            RepoDHO = repoDHO;
            RepoSalesOrder = repoSalesOrder;
            RepoRevisiTanggal = repoRevisiTanggal;
            RepoSettBatal = repoSettBatal;
            RepoBatalOrder = repoBatalOrder;
            RepoUser = repoUser;
            RepoDHK = repoDHK;
            RepoRute = repoRute;
            RepoAtm = repoAtm;
            RepoBor = repoBor;
            RepoSalesOrderKontrakListSo = repoSalesOrderKontrakListSo;
            RepoHistoryJalanTruck = repoHistoryJalanTruck;
            RepoRemovalRepo = repoRemovalRepo;
            RepoDokumen = repoDokumen;
            RepoRemoval = repoRemoval;
            Repoatm = repoatm;
            RepoSolarInap = repoSolarInap;
        }

        public ActionResult SolarInap()
        {
            ViewBag.listKolom = ListKolom.Where(d => d.Action == "Index" && d.Controller == "SolarInap").ToList();
            return View();
        }

        public string BindingSolarInap(string user_types)
        {
            GridRequestParameters param = GridRequestParameters.Current;
            Context.User user = RepoUser.FindByPK(UserPrincipal.id);
            List<SolarInap> ListModel = new List<SolarInap>();

            List<Context.SolarInap> items = RepoSolarInap.FindAllReport();

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
            int total = RepoSolarInap.Count(param.Filters);
            return new JavaScriptSerializer().Serialize(new { total = total, data = ListModel });
        }

        [MyAuthorize(Menu = "Order", Action = "read")]
        public ActionResult Detail(int idSo)
        {
            Context.SalesOrder dbso = RepoSalesOrder.FindByPK(idSo);
            Context.RevisiTanggal dbitem = RepoRevisiTanggal.FindBySo(idSo);
            
            RevisiTanggal model = new RevisiTanggal(dbso);

            if (RepoRevisiTanggal.FindBySo(idSo) != null)
            {
                model = new RevisiTanggal(dbitem);
            }

            return View("Detail", model);
        }

        [MyAuthorize(Menu = "Surat Jalan", Action="read")]
        public ActionResult SuratJalan()
        {
            ViewBag.listKolom = ListKolom.Where(d=>d.Action == "Index" && d.Controller == "Dokumen").ToList();
            ViewBag.caller = "admin";
            ViewBag.Title = "Dokumen Admin Surat Jalan";
            return View();
        }

        public string BindingHistoryDetail(int idSo){
            List<Context.OrderHistory> items = RepoSalesOrder.FindAllHistory(idSo);
            List<OrderHistory> ListModel = new List<OrderHistory>();
            foreach (Context.OrderHistory item in items)
            {
                ListModel.Add(new OrderHistory(item, (RepoUser.FindByPK(item.PIC.Value).Fristname + " " + RepoUser.FindByPK(item.PIC.Value).Lastname)));
            }

            return new JavaScriptSerializer().Serialize(ListModel);
        }

        [MyAuthorize(Menu = "Order", Action="read")]
        public ActionResult Order()
        {
            ViewBag.listKolom = ListKolom.Where(d => d.Action == "Index" && d.Controller == "ListOrder").ToList();
            return View();
        }

        public string BindingSJ(string caller)
        {
            List<Context.Dokumen> items = RepoDokumen.FindAll().Where(d => d.IsAdmin == false).ToList();

            List<DokumenIndex> ListModel = new List<DokumenIndex>();
            if (caller == "admin")
            {
                foreach (var item in items)
                {
                    ListModel.Add(new DokumenIndex(item));
                }
            }
            else
            {
                foreach (var item in items.Where(d => !d.IsAdmin))
                {
                    ListModel.Add(new DokumenIndex(item));
                }
            }

            return new JavaScriptSerializer().Serialize(new { total = ListModel.Count, data = ListModel });
        }

        public string Binding()
        {
            GridRequestParameters param = GridRequestParameters.Current;
            List<Context.SalesOrder> items = RepoSalesOrder.FindAll().Where(d=> d.SalesOrderKonsolidasiId == null).ToList();

            List<ListOrder> ListModel = new List<ListOrder>();
            foreach (Context.SalesOrder item in items)
            {
                if (item.SalesOrderKontrakId.HasValue)
                {
                    if (item.SalesOrderKontrak.SalesOrderKontrakListSo.Any(p => p.IsProses))
                    {
                        foreach (var itemKontrak in item.SalesOrderKontrak.SalesOrderKontrakListSo.Where(s => s.Status != null && s.Status != "").GroupBy(d => new { d.IdDataTruck, d.Driver1Id, d.Status, d.Urutan }).Select(grp => grp.ToList()))
                        {
                            foreach (var itemKontrakPerOrder in itemKontrak.OrderBy(t => t.MuatDate).ToList()){
                                ListModel.Add(new ListOrder(item, itemKontrakPerOrder));
                            }
                        }
                    }
                    else
                    {
                        ListModel.Add(new ListOrder(item));
                    }
                }
                else
                {
                    ListModel.Add(new ListOrder(item));
                }
            }

            return new JavaScriptSerializer().Serialize(new { total = ListModel.Count, data = ListModel });
        }

        [MyAuthorize(Menu = "Admin Uang Jalan", Action="read")]
        public ActionResult AdminUangJalan()
        {
            ViewBag.listKolom = ListKolom.Where(d => d.Action == "Index" && d.Controller == "AdminUangJalan").ToList();
            return View();
        }

        [HttpPost]
        public ActionResult View(AdminUangJalan model, string btnsave)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(model.IdSalesOrder.Value);
            Context.AdminUangJalan db = dbitem.AdminUangJalan;
            db.KeteranganAdmin = model.KeteranganAdmin;
            dbitem.UpdatedBy = UserPrincipal.id;
            RepoSalesOrder.save(dbitem);
            return RedirectToAction("View", new {Id = dbitem.Id});
        }

        public string BindingAUJ()
        {
            List<Context.SalesOrder> items = RepoSalesOrder.FindAllAUJReport();

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
                            _model.Rute = string.Join(", ", ListRute);
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
                    _model.Status = "Sudah";
                    ListModel.Add(_model);
                }
                else {
                    var data = item.SalesOrderKontrak.SalesOrderKontrakListSo.Where(p => p.IsProses && (p.Status == "save konfirmasi" || p.Status == "dispatched")).GroupBy(d => new { d.IdDataTruck, d.Driver1Id, d.Status, d.Urutan }).Select(grp => grp.ToList());
                    foreach (var itemGroup in data.ToList())
                    {
                        ListModel.Add(new AdminUangJalanIndex(item, itemGroup));

                        //foreach (var itemSo in itemGroup.ToList())
                        //{
                            //List<Context.SalesOrderKontrakListSo> listItem = item.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => d.Urutan == i && (d.Status == "save konfirmasi" || d.Status == "dispatched")).ToList();
                            //ListModel.Add(new AdminUangJalanIndex(item, listItem));
                        //}    
                    }
                }
            }

            return new JavaScriptSerializer().Serialize(new { total = items.Count, data = ListModel });
        }

        [MyAuthorize(Menu = "Kasir Transfer", Action="read")]
        public ActionResult KasirTf()
        {
            ViewBag.listKolom = ListKolom.Where(d => d.Action == "Index" && d.Controller == "Kasir").ToList();
            return View("IndexTf");
        }
        [MyAuthorize(Menu = "Kasir Cash", Action="read")]
        public ActionResult KasirKas()
        {
            ViewBag.listKolom = ListKolom.Where(d => d.Action == "Index" && d.Controller == "Kasir").ToList();
            return View("IndexKas");
        }

        public string BindingTf()
        {
            List<Context.SalesOrder> items = RepoSalesOrder.FindAllKasirReport();

            List<KasirTf> ListModel = new List<KasirTf>();
            foreach (Context.SalesOrder item in items)
            {
                if (item.SalesOrderKontrakId.HasValue)
                {
                    var data = item.SalesOrderKontrak.SalesOrderKontrakListSo.Where(p => p.IsProses && p.IdAdminUangJalan != null && (p.Status == "dispatched" || p.Status == "settlement" || p.Status == "batal order") && p.AdminUangJalan.AdminUangJalanUangTf.Where(s => s.Keterangan != "Tunai").Any(n => n.isTf == true))
                        .GroupBy(d => new { d.IdDataTruck, d.Driver1Id, d.Status, d.Urutan }).Select(grp => grp.ToList());
                    foreach (var itemGroup in data.ToList())
                    {
                        ListModel.Add(new KasirTf(item, itemGroup));
                    }
                }
                else if (item.AdminUangJalanId.HasValue && item.AdminUangJalan.AdminUangJalanUangTf.Where(s => s.Keterangan != "Tunai").Any(n => n.isTf == true))
                {
                    ListModel.Add(new KasirTf(item));
                }
            }

            List<Context.Removal> ItemsRemoval = RepoRemoval.FindAll().Where(d => d.Status == "dispatched").ToList();
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
                    var data = item.SalesOrderKontrak.SalesOrderKontrakListSo.Where(p => p.IsProses && (p.Status == "dispatched") && p.AdminUangJalan.AdminUangJalanUangTf.Where(s => s.Keterangan == "Tunai").Any(n => n.isTf == true))
                        .GroupBy(d => new { d.IdDataTruck, d.Driver1Id, d.Status, d.Urutan }).Select(grp => grp.ToList());
                    foreach (var itemGroup in data.ToList())
                    {
                        ListModel.Add(new Kasirkas(item, itemGroup));
                    }
                }
                else if (item.AdminUangJalanId.HasValue && item.AdminUangJalan.AdminUangJalanUangTf.Where(s => s.Keterangan == "Tunai").Any(n => n.isTf == true))
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
            return new JavaScriptSerializer().Serialize(new { total = items.Count, data = ListModel });
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
        public ActionResult ShowPrint(int id)
        {
            Context.Dokumen dbitem = RepoDokumen.FindByPK(id);
            DokumenIndex model = new DokumenIndex(dbitem);

            return View("Print", model);
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

        [MyAuthorize(Menu = "Admin Uang Jalan", Action="print")]
        public ActionResult PrintBon(int idSo, string listSo = "")
        {
            return new Rotativa.ActionAsPdf("ShowPrint", new { idSo = idSo, listSo = listSo })
            {
                FileName = "adminuangjalan.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = { Left = 0, Right = 0 },
                PageOrientation = Rotativa.Options.Orientation.Portrait
            };
        }

        public ActionResult PrintSuratJalan(int id)
        {
            return new Rotativa.ActionAsPdf("ShowPrint", new { id = id }) { FileName = "Dokumen_" + new Guid().ToString() + ".pdf", PageSize = Rotativa.Options.Size.A4 };
        }

        public ActionResult ViewSuratJalan(int id, string caller)
        {
            Context.Dokumen dbitem = RepoDokumen.FindByPK(id);
            Dokumen model = new Dokumen(dbitem);
            ViewBag.caller = caller;
            if (caller == "admin")
                ViewBag.Title = "Dokumen Admin Surat Jalan";
            else
                ViewBag.Title = "Dokumen Billing";

            return View("ViewSuratJalan", model);
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
            GenerateModelAUJ(model, dbitem);
            model.ModelKontrak.ListValueModelSOKontrak = model.ModelKontrak.ListModelSOKontrak;

            return View("ViewKontrak", model);
        }

        public void GenerateModelAUJ(AdminUangJalan model, Context.SalesOrder dbitem)
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
    }
}