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
    public class SettlementBatalController : BaseController
    {
        private ISettlementBatalRepo RepoSettlementBatal;
        private ISalesOrderRepo RepoSalesOrder;
        private IAtmRepo RepoAtm;
        private IDataBoronganRepo RepoBor;
        private Iglt_detRepo Repoglt_det;
        private IERPConfigRepo RepoERPConfig;
        private Iac_mstrRepo Repoac_mstr;
        private Ibk_mstrRepo Repobk_mstr;
        private IAdminUangJalanRepo RepoAdminUangJalan;
        private Ipbyd_detRepo Repopbyd_det;
        private IMasterPoolRepo RepoMasterPool;
        private ILookupCodeRepo RepoLookupCode;
        private ICustomerRepo RepoCustomer;
        private Igr_mstrRepo Repogr_mstr;

        public SettlementBatalController(IUserReferenceRepo repoBase, ILookupCodeRepo repoLookup, ISettlementBatalRepo repoSettlementBatal, ISalesOrderRepo repoSalesOrder, IAtmRepo repoAtm,
            IDataBoronganRepo repoBor, Iglt_detRepo repoglt_det, IERPConfigRepo repoERPConfig, Iac_mstrRepo repoac_mstr, Ibk_mstrRepo repobk_mstr, IAdminUangJalanRepo repoAdminUangJalan,
            Ipbyd_detRepo repopbyd_det, IMasterPoolRepo repoIMasterPool, ILookupCodeRepo repoLookupCode, ICustomerRepo repoCustomer, Igr_mstrRepo repogr_mstr)
            : base(repoBase, repoLookup)
        {
            RepoAdminUangJalan = repoAdminUangJalan;
            RepoSettlementBatal = repoSettlementBatal;
            RepoSalesOrder = repoSalesOrder;
            RepoAtm = repoAtm;
            RepoBor = repoBor;
            Repoglt_det = repoglt_det;
            RepoERPConfig = repoERPConfig;
            Repoac_mstr = repoac_mstr;
            Repobk_mstr = repobk_mstr;
            Repopbyd_det = repopbyd_det;
            RepoMasterPool = repoIMasterPool;
            RepoLookupCode = repoLookupCode;
            RepoCustomer = repoCustomer;
            Repogr_mstr = repogr_mstr;
        }
        [MyAuthorize(Menu = "Settlement Batal", Action="read")]
        public ActionResult Index()
        {
            ViewBag.listKolom = ListKolom.Where(d => d.Action == "Index" && d.Controller == "SettlementBatal").ToList();
            return View();
        }
        public string Binding()
        {
            GridRequestParameters param = GridRequestParameters.Current;

            List<Context.SettlementBatal> items = RepoSettlementBatal.FindAll();

            List<SettlementBatalIndex> ListModel = new List<SettlementBatalIndex>();
            foreach (Context.SettlementBatal item in items)
            {
                if (item.IdSoKontrak != "" && item.IdSoKontrak != null)
                {
                    var data = item.SalesOrder.SalesOrderKontrak.SalesOrderKontrakListSo.Where(p => p.IsBatalTruck && p.Id == int.Parse(item.IdSoKontrak)).ToList();
                    foreach (var itemGroup in data.ToList())
                    {
                        ListModel.Add(new SettlementBatalIndex(item, itemGroup));
                    }
                }
                else
                {
                    ListModel.Add(new SettlementBatalIndex(item));
                }
            }
            return new JavaScriptSerializer().Serialize(new { total = ListModel.Count(), data = ListModel });
        }
        [MyAuthorize(Menu = "Settlement Batal", Action="update")]
        public ActionResult Edit(int id)
        {
            Context.SettlementBatal dbitem = RepoSettlementBatal.FindByPK(id);
            if (dbitem.IdSoKontrak == "" || dbitem.IdSoKontrak == null)
            {
                SettlementBatal model = new SettlementBatal(dbitem);
                Context.SalesOrder dbso = RepoSalesOrder.FindByPK(dbitem.IdSalesOrder.Value);
                ViewBag.SPBU = dbso.AdminUangJalan.AdminUangJalanVoucherSpbu;
                ViewBag.KeteranganBatal = dbso.KeteranganBatal;
                if (model.ModelOncall != null){
                    ViewBag.name = model.ModelOncall.SONumber;
                    if (dbitem.Driver != null && dbitem.DataTruck != null){
                        ViewBag.KodeDriver = dbitem.Driver.KodeDriver;
                        ViewBag.KodeDriverOld = dbitem.Driver.KodeDriverOld;
                        ViewBag.DriverName = dbitem.Driver.NamaDriver;
                        ViewBag.VehicleNo = dbitem.DataTruck.VehicleNo;
                    }
                }
                if (model.ModelPickup != null)
                    ViewBag.name = model.ModelPickup.SONumber;
                if (model.ModelKonsolidasi != null)
                    ViewBag.name = model.ModelKonsolidasi.SONumber;

                return View("Form", model);
            }
            else{
                SettlementBatal model = new SettlementBatal(dbitem);
                Context.SalesOrder dbso = RepoSalesOrder.FindByPK(dbitem.IdSalesOrder.Value);
                ViewBag.SPBU = dbso.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => d.Id == int.Parse(dbitem.IdSoKontrak)).FirstOrDefault().AdminUangJalan.AdminUangJalanVoucherSpbu;
                return View("Form", model);
            }
        }
        [HttpPost]
        public ActionResult Edit(SettlementBatal model)
        {
            if (ModelState.IsValid)
            {
                Context.SettlementBatal dbitem = RepoSettlementBatal.FindByPK(model.Id);
            if (dbitem.IdSoKontrak == "" || dbitem.IdSoKontrak == null)
            {
                Context.SalesOrder dbso = RepoSalesOrder.FindByPK(model.IdSalesOrder.Value);
                Context.AdminUangJalan db = RepoAdminUangJalan.FindByPK(dbso.AdminUangJalanId.Value);
                Context.ERPConfig erpConfig = RepoERPConfig.FindByFrist();
                dbitem.Code = "SB-" + (dbso.SalesOrderOncallId.HasValue ? dbso.SalesOrderOncall.SONumber : dbso.SalesOrderProsesKonsolidasiId.HasValue ? 
                    dbso.SalesOrderProsesKonsolidasi.SONumber : dbso.SalesOrderPickupId.HasValue ? dbso.SalesOrderPickup.SONumber : dbso.SalesOrderKontrak.SONumber);
                model.SetDb(dbitem);

                //Jurnal Pengembalian Uang Jalan //(uang, dll dikembalikan utuh), mau utuh mau ngga, nominal cash yg dikembalikan mah ambil dari form :p
                decimal nominalHutangUangJalanDriver = (model.KasAktual == null ? 0 : model.KasAktual.Value) + (model.TransferAktual == null ? 0 : model.TransferAktual.Value);
                decimal nomDiakui = (model.KasDiakui == null ? 0 : model.KasDiakui.Value) + (model.TransferDiakui == null ? 0 : model.TransferDiakui.Value) + (model.SolarDiakui == null ? 0 : model.SolarDiakui.Value) + (model.KapalDiakui == null ? 0 : model.KapalDiakui.Value);
                decimal nomSelisih = (dbitem.KasSelisih == null ? 0 : dbitem.KasSelisih.Value) + (dbitem.TransferSelisih == null ? 0 : dbitem.TransferSelisih.Value) + (dbitem.KapalSelisih == null ? 0 : dbitem.KapalSelisih.Value);

                nominalHutangUangJalanDriver += decimal.Parse(db.AdminUangJalanVoucherSpbu.Select(d => d.Value).Sum().ToString());
                nominalHutangUangJalanDriver += decimal.Parse(db.AdminUangJalanVoucherKapal.Select(d => d.Value).Sum().ToString());
                nominalHutangUangJalanDriver += db.KasbonDriver1 == null ? 0 : db.KasbonDriver1.Value;
                nominalHutangUangJalanDriver += db.KlaimDriver1 == null ? 0 : db.KlaimDriver1.Value;
                nominalHutangUangJalanDriver += decimal.Parse(db.AdminUangJalanPotonganDriver.Select(d => d.Value).Sum().ToString());
                decimal? TotalBiaya = nomDiakui;
                int idx = 2;
                if (nomDiakui > 0)
                    Repoglt_det.saveFromAc(2, "SB-" + dbso.SalesOrderOncall.SONumber, nomDiakui, 0, Repoac_mstr.FindByPk(erpConfig.IdBiayaBatalJalan));//BIAYA BATAL JALAN
                foreach (Context.AdminUangJalanVoucherSpbu aujvs in db.AdminUangJalanVoucherSpbu){
                    idx++;
                    if (model.SPBUKembali != null && model.SPBUKembali.Contains(aujvs.Keterangan)){//jurnal balik, ga jadi hutangnya
                        Repoglt_det.saveFromAc(idx, "SB-" + dbso.SalesOrderOncall.SONumber, aujvs.Value, 0, Repoac_mstr.FindByPk(RepoLookup.FindByName(aujvs.Keterangan).ac_id));//HUTANG SPBU 34.171.04 Pangkalan 2 dan atau
                        TotalBiaya += aujvs.Value;
                    }
                    else //jadi hutang SPBU, masuk ke AP Inv
                    {
                        string a = aujvs.Keterangan;
                        string username = UserPrincipal.username;
                        int? vend = RepoLookup.FindByName(aujvs.Keterangan).VendorId;
                        int vendorId = RepoLookup.FindByName(aujvs.Keterangan).VendorId.Value;
                        Context.Customer cust = RepoCustomer.FindByPK(RepoLookup.FindByName(aujvs.Keterangan).VendorId.Value);
                        Repogr_mstr.save(aujvs.Value, cust, username, dbitem.Code);
                    }
                }
                Repoglt_det.saveFromAc(idx, "SB-" + dbso.SalesOrderOncall.SONumber, dbitem.SolarSelisih, 0, Repoac_mstr.FindByPk(erpConfig.IdPiutangDriverBatalJalanSementaraSolar));
                TotalBiaya += dbitem.SolarSelisih;
                if (dbitem.KapalAktual > 0){
                    foreach (Context.AdminUangJalanVoucherKapal aujvs in db.AdminUangJalanVoucherKapal){
                        idx++;
                        Repoglt_det.saveFromAc(idx, "SB-" + dbso.SalesOrderOncall.SONumber, aujvs.Value, 0, Repoac_mstr.FindByPk(RepoLookup.FindByName(aujvs.Keterangan).ac_id));//HUTANG SPBU 34.171.04 Pangkalan 2 dan atau
                        TotalBiaya += aujvs.Value;
                    }
                }
//                if (nomSelisih > 0){
                    idx++;
                    Repoglt_det.saveFromAc(idx, "SB-" + dbso.SalesOrderOncall.SONumber, nomSelisih, 0, Repoac_mstr.FindByPk(erpConfig.IdPiutangDriverBatalJalan), "Selisih Kas/Transfer/Kapal");//PIUTANG DRIVER BATAL JALAN
                    TotalBiaya += nomSelisih;
  //              }
                //mau utuh mau ngga, nominal cash yg dikembalikan mah ambil dari form :p
                Repoglt_det.saveFromAc(2, "SB-" + dbso.SalesOrderOncall.SONumber, model.KasAktual, 0, Repoac_mstr.FindByPk(RepoMasterPool.FindByIPAddress().IdCreditCash));//BCA Audy 386-7957777 atau
                Repoglt_det.saveFromAc(2, "SB-" + dbso.SalesOrderOncall.SONumber, model.TransferAktual, 0, Repoac_mstr.FindByPk(dbitem.IdCreditTf));//BCA Audy 386-7957777 atau
                TotalBiaya += (model.KasAktual + model.TransferAktual);
                Repoglt_det.saveFromAc(1, "SB-" + dbso.SalesOrderOncall.SONumber, 0, TotalBiaya, Repoac_mstr.FindByPk(erpConfig.IdAUJCredit));//Hutang Uang Jalan Driver

                RepoSettlementBatal.save(dbitem, UserPrincipal.id, "Settlement Batal");
                try {//masuklah ke saldo piutang driver, save to pby_mstr&det
                    var glt_oid = Guid.NewGuid().ToString();
                    Repopbyd_det.saveMstr(glt_oid, dbitem.Code, erpConfig.IdAUJCredit.Value, "Settlement Batal " + dbitem.Code, db.IdDriver1.Value+7000000);
                    Repopbyd_det.save(
                        glt_oid, dbitem.Code, erpConfig.IdAUJCredit.Value, "Settlement Batal " + dbitem.Code, db.IdDriver1.Value+7000000,
                        erpConfig.IdAUJCredit.Value, Repoac_mstr.FindByPk(erpConfig.IdAUJCredit).ac_name, 0
                    );
                }
                catch (Exception e) {}
            }
            else{
                Context.SalesOrder dbso = RepoSalesOrder.FindByPK(model.IdSalesOrder.Value);
                Context.SalesOrderKontrakListSo dbKontrak = dbso.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => d.Id == int.Parse(dbitem.IdSoKontrak)).FirstOrDefault();
                Context.AdminUangJalan db = RepoAdminUangJalan.FindByPK(dbKontrak.IdAdminUangJalan.Value);
                Context.ERPConfig erpConfig = RepoERPConfig.FindByFrist();
                dbitem.Code = "SB-" + (dbso.SalesOrderOncallId.HasValue ? dbso.SalesOrderOncall.SONumber : dbso.SalesOrderProsesKonsolidasiId.HasValue ? 
                    dbso.SalesOrderProsesKonsolidasi.SONumber : dbso.SalesOrderPickupId.HasValue ? dbso.SalesOrderPickup.SONumber : dbso.SalesOrderKontrak.SONumber);
                model.SetDb(dbitem);

                //Jurnal Pengembalian Uang Jalan //(uang, dll dikembalikan utuh), mau utuh mau ngga, nominal cash yg dikembalikan mah ambil dari form :p
                decimal nominalHutangUangJalanDriver = model.KasAktual ?? model.KasAktual.Value + model.TransferAktual ?? model.TransferAktual.Value;
                decimal nomDiakui = model.KasDiakui ?? model.KasDiakui.Value + model.TransferDiakui ?? model.TransferDiakui.Value + model.SolarDiakui ?? model.SolarDiakui.Value + model.KapalDiakui ?? model.KapalDiakui.Value;
                decimal nomSelisih = dbitem.KasSelisih.Value + dbitem.TransferSelisih.Value + dbitem.KapalSelisih.Value;

                nominalHutangUangJalanDriver += decimal.Parse(db.AdminUangJalanVoucherSpbu.Select(d => d.Value).Sum().ToString());
                nominalHutangUangJalanDriver += decimal.Parse(db.AdminUangJalanVoucherKapal.Select(d => d.Value).Sum().ToString());
                nominalHutangUangJalanDriver += db.KasbonDriver1 == null ? 0 : db.KasbonDriver1.Value;
                nominalHutangUangJalanDriver += db.KlaimDriver1 == null ? 0 : db.KlaimDriver1.Value;
                nominalHutangUangJalanDriver += decimal.Parse(db.AdminUangJalanPotonganDriver.Select(d => d.Value).Sum().ToString());
                int idx = 2;
                Repoglt_det.saveFromAc(1, "SB-" + dbKontrak.NoSo, 0, nominalHutangUangJalanDriver, Repoac_mstr.FindByPk(erpConfig.IdAUJCredit));//Hutang Uang Jalan Driver
                if (nomDiakui > 0)
                    Repoglt_det.saveFromAc(2, "SB-" + dbKontrak.NoSo, nomDiakui, 0, Repoac_mstr.FindByPk(erpConfig.IdBiayaBatalJalan));//BIAYA BATAL JALAN
                foreach (Context.AdminUangJalanVoucherSpbu aujvs in db.AdminUangJalanVoucherSpbu){
                    idx++;
                    if (model.SPBUKembali.Contains(aujvs.Keterangan))
                        Repoglt_det.saveFromAc(idx, "SB-" + dbKontrak.NoSo, aujvs.Value, 0, Repoac_mstr.FindByPk(RepoLookup.FindByName(aujvs.Keterangan).ac_id));//HUTANG SPBU 34.171.04 Pangkalan 2 dan atau
                    else
                        Repoglt_det.saveFromAc(idx, "SB-" + dbKontrak.NoSo, aujvs.Value, 0, Repoac_mstr.FindByPk(erpConfig.IdPiutangDriverBatalJalanSementaraSolar), "Voucher SPBU " + aujvs.Keterangan);
                }
                foreach (Context.AdminUangJalanVoucherKapal aujvs in db.AdminUangJalanVoucherKapal){
                    idx++;
                    Repoglt_det.saveFromAc(idx, "SB-" + dbKontrak.NoSo, aujvs.Value, 0, Repoac_mstr.FindByPk(RepoLookup.FindByName(aujvs.Keterangan).ac_id));//HUTANG SPBU 34.171.04 Pangkalan 2 dan atau
                }
//                if (nomSelisih > 0){
                    idx++;
                    Repoglt_det.saveFromAc(idx, "SB-" + dbKontrak.NoSo, nomSelisih, 0, Repoac_mstr.FindByPk(erpConfig.IdPiutangDriverBatalJalan), "Selisih Kas/Transfer/Kapal");//PIUTANG DRIVER BATAL JALAN
  //              }
                if (db.PotonganB > 0){
                    idx++;
                    Repoglt_det.saveFromAc(idx, "SB-" + dbKontrak.NoSo, db.PotonganB, 0, Repoac_mstr.FindByPk(erpConfig.IdPiutangDriverBatalJalan));//PIUTANG DRIVER BATAL JALAN
                }
                if (db.PotonganP > 0){
                    idx++;
                    Repoglt_det.saveFromAc(idx, "SB-" + dbKontrak.NoSo, db.PotonganP, 0, Repoac_mstr.FindByPk(erpConfig.IdPiutangDriverPribadi));//PIUTANG DRIVER BATAL JALAN / SEMENTARA TUNAI
                }
                if (db.PotonganK > 0){
                    idx++;
                    Repoglt_det.saveFromAc(idx, "SB-" + dbKontrak.NoSo, db.PotonganK, 0, Repoac_mstr.FindByPk(erpConfig.IdPendapatanPengembalianPiutangDriver));
                }
                if (db.PotonganT > 0){
                    idx++;
                    Repoglt_det.saveFromAc(idx, "SB-" + dbKontrak.NoSo, db.PotonganT, 0, Repoac_mstr.FindByPk(erpConfig.IdTabunganDriver));
                }

                //mau utuh mau ngga, nominal cash yg dikembalikan mah ambil dari form :p
                Repoglt_det.saveFromAc(2, "SB-" + dbKontrak.NoSo, model.KasAktual, 0, Repoac_mstr.FindByPk(RepoMasterPool.FindByIPAddress().IdCreditCash));//BCA Audy 386-7957777 atau
                Repoglt_det.saveFromAc(2, "SB-" + dbKontrak.NoSo, model.TransferAktual, 0, Repoac_mstr.FindByPk(db.AdminUangJalanUangTf.FirstOrDefault().IdCreditTf));//BCA Audy 386-7957777 atau

                RepoSettlementBatal.save(dbitem, UserPrincipal.id, "Settlement Batal");
                try {//masuklah ke saldo piutang driver, save to pby_mstr&det
                    var glt_oid = Guid.NewGuid().ToString();
                    Repopbyd_det.saveMstr(glt_oid, dbitem.Code, erpConfig.IdAUJCredit.Value, "Settlement Batal " + dbitem.Code, db.IdDriver1.Value+7000000);
                    Repopbyd_det.save(
                        glt_oid, dbitem.Code, erpConfig.IdAUJCredit.Value, "Settlement Batal " + dbitem.Code, db.IdDriver1.Value+7000000,
                        erpConfig.IdAUJCredit.Value, Repoac_mstr.FindByPk(erpConfig.IdAUJCredit).ac_name, 0
                    );
                }
                catch (Exception e) {}
            }
                return RedirectToAction("Index");
            }

            return View("Form", model);
        }

        [MyAuthorize(Menu = "Settlement Batal", Action="read")]
        public ActionResult View(int id)
        {
            Context.SettlementBatal dbitem = RepoSettlementBatal.FindByPK(id);
            SettlementBatal model = new SettlementBatal(dbitem);
            if (model.ModelOncall != null)
                ViewBag.name = model.ModelOncall.SONumber;
            if (model.ModelPickup != null)
                ViewBag.name = model.ModelPickup.SONumber;
            if (model.ModelKonsolidasi != null)
                ViewBag.name = model.ModelKonsolidasi.SONumber;
            return View(model);
        }
    }
}