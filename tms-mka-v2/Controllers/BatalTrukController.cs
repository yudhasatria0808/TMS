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
    public class BatalTrukController : BaseController
    {
        private ISalesOrderRepo RepoSalesOrder;
        private IBatalOrderRepo RepoBatalOrder;
        private ISettlementBatalRepo RepoSettBatal;
        private IAtmRepo Repoatm;
        private IDataBoronganRepo RepoBor;
        private IAdminUangJalanRepo RepoAdminUangJalan;
        private Iglt_detRepo Repoglt_det;
        private IERPConfigRepo RepoERPConfig;
        private Iac_mstrRepo Repoac_mstr;
        private Ibk_mstrRepo Repobk_mstr;
        private IAuditrailRepo RepoAuditrail;
        private Ipbyd_detRepo Repopbyd_det;

        public BatalTrukController(IUserReferenceRepo repoBase, ILookupCodeRepo repoLookup, ISalesOrderRepo repoSalesOrder, IBatalOrderRepo repoBatalOrder, ISettlementBatalRepo repoSettBatal,
            IAtmRepo repoatm, IDataBoronganRepo repoBor, IAdminUangJalanRepo repoAdminUangJalan, Iglt_detRepo repoglt_det, IERPConfigRepo repoERPConfig, Iac_mstrRepo repoac_mstr, Ibk_mstrRepo repobk_mstr, IAuditrailRepo repoAuditrail, Ipbyd_detRepo repopbyd_det)
            : base(repoBase, repoLookup)
        {
            RepoSalesOrder = repoSalesOrder;
            RepoBatalOrder = repoBatalOrder;
            RepoSettBatal = repoSettBatal;
            Repoatm = repoatm;
            RepoBor = repoBor;
            RepoAdminUangJalan = repoAdminUangJalan;
            Repoglt_det = repoglt_det;
            RepoERPConfig = repoERPConfig;
            Repoac_mstr = repoac_mstr;
            Repobk_mstr = repobk_mstr;
            RepoAuditrail = repoAuditrail;
            Repopbyd_det = repopbyd_det;
        }

        [MyAuthorize(Menu = "Batal Truck", Action="create")]
        public ActionResult Edit(int id)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            AdminUangJalan model = new AdminUangJalan();
            if (dbitem.AdminUangJalanId.HasValue)
                model = new AdminUangJalan(dbitem.AdminUangJalan, Repoatm.FindAll(), RepoBor.FindAll());

            model.IdSalesOrder = dbitem.Id;
            ViewBag.Status = dbitem.Status;
            if (dbitem.SalesOrderOncallId.HasValue)
            {
                model.ModelOncall = new SalesOrderOncall(dbitem);
                model.StatusSo = model.ModelOncall.Status;
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
                ViewBag.dbsoPerDriverCount = 1;
                ViewBag.Title = "Batal Truk " + dbitem.SalesOrderOncall.SONumber;
                return View("Form", model);
            }
            else if (dbitem.SalesOrderPickupId.HasValue)
            {
                model.ModelPickup = new SalesOrderPickup(dbitem);
                model.StatusSo = model.ModelPickup.Status;
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
                ViewBag.Title = "Batal Order " + dbitem.SalesOrderPickup.SONumber;
                //ViewBag.postData = "EditPickup";
                return View("Form", model);
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue)
            {
                model.ModelKonsolidasi = new SalesOrderProsesKonsolidasi(dbitem);
                model.StatusSo = model.ModelKonsolidasi.Status;
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
                ViewBag.Title = "Batal Order " + dbitem.SalesOrderProsesKonsolidasi.SONumber;
                //ViewBag.postData = "EditPickup";
                return View("Form", model);
            }
            else if (dbitem.SalesOrderKontrakId.HasValue)
            {
                model.ModelKonsolidasi = new SalesOrderProsesKonsolidasi(dbitem);
                model.StatusSo = model.ModelKonsolidasi.Status;
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
                ViewBag.Title = "Batal Order " + dbitem.SalesOrderProsesKonsolidasi.SONumber;
                //ViewBag.postData = "EditPickup";
                ViewBag.dbsoPerDriverCount = 1;
                return View("Form", model);
            }
            
            return View();            
        }

        [HttpPost]
        public ActionResult Edit(AdminUangJalan model, string Keterangan, string IsTransfer)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(model.IdSalesOrder.Value);
            Context.AdminUangJalan dummyAdminUangJalan = dbitem.AdminUangJalan;
            Context.BatalOrder batalOrder = new Context.BatalOrder();
            string code =  "BT-" + (dbitem.SalesOrderOncallId.HasValue ? dbitem.SalesOrderOncall.SONumber : dbitem.SalesOrderProsesKonsolidasiId.HasValue ? dbitem.SalesOrderProsesKonsolidasi.SONumber : dbitem.SalesOrderPickup.SONumber);
            int? urutanBatal = RepoBatalOrder.FindAll().Where(d => d.Code == code).Count() == 0 ? 1 : RepoBatalOrder.FindAll().Where(d => d.Code == code).Max(d => d.UrutanBatal) + 1;

            //cek status na
            Context.ERPConfig erpConfig = RepoERPConfig.FindByFrist();
            if (dbitem.Status == "dispatched")
            {
                Context.SettlementBatal dbsettlement = new Context.SettlementBatal();
                decimal? tambahanRute = dummyAdminUangJalan.AdminUangJalanTambahanRute.Sum(s => s.values);
                decimal? boronganDasar = dummyAdminUangJalan.TotalAlokasi - dummyAdminUangJalan.Kawalan - dummyAdminUangJalan.Timbangan - dummyAdminUangJalan.Karantina - dummyAdminUangJalan.SPSI - dummyAdminUangJalan.Multidrop - tambahanRute - dummyAdminUangJalan.AdminUangJalanTambahanLain.Sum(s => s.Values);
                //ambil data 
                dbsettlement.IdSalesOrder = model.IdSalesOrder;
                if(dummyAdminUangJalan.AdminUangJalanUangTf.Any(d => d.Keterangan == "Tunai" ))
                {
                    dbsettlement.KasDiterima = dummyAdminUangJalan.AdminUangJalanUangTf.Where(d => d.Keterangan == "Tunai").FirstOrDefault().JumlahTransfer;
                }
                if(dummyAdminUangJalan.AdminUangJalanUangTf.Any(d => d.Keterangan.Contains("Transfer")))
                {
                    dbsettlement.TransferDiterima = dummyAdminUangJalan.AdminUangJalanUangTf.Where(d => d.Keterangan.Contains("Transfer")).Sum(t => t.JumlahTransfer);
                }
                dbsettlement.SolarDiterima = dummyAdminUangJalan.AdminUangJalanVoucherSpbu.Sum(s => s.Value);
                dbsettlement.KapalDiterima = dummyAdminUangJalan.AdminUangJalanVoucherKapal.Sum(s => s.Value);
                dbsettlement.JenisBatal = "Batal Truk";
                dbsettlement.IdDriver = dbitem.SalesOrderOncallId.HasValue ? dbitem.SalesOrderOncall.Driver1Id : dbitem.SalesOrderProsesKonsolidasiId.HasValue ? dbitem.SalesOrderProsesKonsolidasi.Driver1Id : dbitem.SalesOrderPickup.Driver1Id;
                dbsettlement.IdDataTruck = dbitem.SalesOrderOncallId.HasValue ? dbitem.SalesOrderOncall.IdDataTruck : dbitem.SalesOrderProsesKonsolidasiId.HasValue ? dbitem.SalesOrderProsesKonsolidasi.IdDataTruck : dbitem.SalesOrderPickup.IdDataTruck;
                RepoSettBatal.save(dbsettlement, UserPrincipal.id, "Batal Truk");
                decimal? TotalBiaya = (dummyAdminUangJalan.KasbonDriver1 ?? 0) + (dummyAdminUangJalan.KlaimDriver1 ?? 0) + dummyAdminUangJalan.AdminUangJalanPotonganDriver.Select(d => d.Value).Sum() +
                    dummyAdminUangJalan.AdminUangJalanVoucherKapal.Select(d => d.Value).Sum() + dummyAdminUangJalan.AdminUangJalanVoucherSpbu.Select(d => d.Value).Sum() +
                    dummyAdminUangJalan.AdminUangJalanUangTf.Select(d => d.Value).Sum();
                decimal TotalUtangDriver = (dbsettlement.SolarDiterima == null ? 0 : dbsettlement.SolarDiterima.Value) + (dbsettlement.KapalDiterima == null ? 0 : dbsettlement.KapalDiterima.Value) + (dbsettlement.TransferDiterima == null ? 0 : dbsettlement.TransferDiterima.Value) + (dbsettlement.KasDiterima == null ? 0 : dbsettlement.KasDiterima.Value);
                Repoglt_det.saveFromAc(1, code + "-" + urutanBatal, TotalBiaya, 0, Repoac_mstr.FindByPk(erpConfig.IdAUJCredit));
                Repoglt_det.saveFromAc(2, code + "-" + urutanBatal, 0, boronganDasar, Repoac_mstr.FindByPk(erpConfig.IdBoronganDasar));
                Repoglt_det.saveFromAc(3, code + "-" + urutanBatal, 0, dummyAdminUangJalan.Kawalan, Repoac_mstr.FindByPk(erpConfig.IdKawalan));
                Repoglt_det.saveFromAc(4, code + "-" + urutanBatal, 0, dummyAdminUangJalan.Timbangan, Repoac_mstr.FindByPk(erpConfig.IdTimbangan));
                Repoglt_det.saveFromAc(5, code + "-" + urutanBatal, 0, dummyAdminUangJalan.Karantina, Repoac_mstr.FindByPk(erpConfig.IdKarantina));
                Repoglt_det.saveFromAc(6, code + "-" + urutanBatal, 0, dummyAdminUangJalan.SPSI, Repoac_mstr.FindByPk(erpConfig.IdSPSI));
                Repoglt_det.saveFromAc(7, code + "-" + urutanBatal, 0, dummyAdminUangJalan.Multidrop, Repoac_mstr.FindByPk(erpConfig.IdMultidrop));
                Repoglt_det.saveFromAc(8, code + "-" + urutanBatal, 0, tambahanRute + dummyAdminUangJalan.AdminUangJalanTambahanLain.Sum(s => s.Values), Repoac_mstr.FindByPk(erpConfig.IdTambahanRuteMuat));
                Repoglt_det.saveFromAc(9, code + "-" + urutanBatal, RepoSalesOrder.Harga(dbitem), 0, Repoac_mstr.FindByPk(erpConfig.IdPendapatanUsahaBlmInv));
                Repoglt_det.saveFromAc(10, code + "-" + urutanBatal, 0, RepoSalesOrder.Harga(dbitem), Repoac_mstr.FindByPk(erpConfig.IdPiutangDagang));
//                try {//masuklah ke saldo piutang driver, save to pby_mstr&det
                    var glt_oid = Guid.NewGuid().ToString();
                    Repopbyd_det.saveMstr(glt_oid, code + "-" + urutanBatal, erpConfig.IdAUJCredit.Value, "Batal Order " + code, dbsettlement.IdDriver.Value+7000000);
                    Repopbyd_det.save(
                        glt_oid, code + "-" + urutanBatal, erpConfig.IdAUJCredit.Value, "Batal Order " + code, dbsettlement.IdDriver.Value+7000000, erpConfig.IdAUJCredit.Value, Repoac_mstr.FindByPk(erpConfig.IdAUJCredit).ac_name, TotalUtangDriver
                    );
  //              }
    //            catch (Exception e) {}
            }
            else if (dbitem.Status == "admin uang jalan"){
                //lebah dieu sync ERPna
                decimal? tambahanRute = dummyAdminUangJalan.AdminUangJalanTambahanRute.Sum(s => s.values);
                decimal? boronganDasar = dummyAdminUangJalan.TotalAlokasi - dummyAdminUangJalan.Kawalan - dummyAdminUangJalan.Timbangan - dummyAdminUangJalan.Karantina - dummyAdminUangJalan.SPSI - dummyAdminUangJalan.Multidrop - tambahanRute - dummyAdminUangJalan.AdminUangJalanTambahanLain.Sum(s => s.Values);
                Repoglt_det.save(1, new Context.glt_det(), dbitem, code + "-" + urutanBatal, 0, boronganDasar, Repoac_mstr.FindByPk(erpConfig.IdBoronganDasar));
                Repoglt_det.save(2, new Context.glt_det(), dbitem, code + "-" + urutanBatal, 0, dummyAdminUangJalan.Kawalan, Repoac_mstr.FindByPk(erpConfig.IdKawalan));
                Repoglt_det.save(3, new Context.glt_det(), dbitem, code + "-" + urutanBatal, 0, dummyAdminUangJalan.Timbangan, Repoac_mstr.FindByPk(erpConfig.IdTimbangan));
                Repoglt_det.save(4, new Context.glt_det(), dbitem, code + "-" + urutanBatal, 0, dummyAdminUangJalan.Karantina, Repoac_mstr.FindByPk(erpConfig.IdKarantina));
                Repoglt_det.save(5, new Context.glt_det(), dbitem, code + "-" + urutanBatal, 0, dummyAdminUangJalan.SPSI, Repoac_mstr.FindByPk(erpConfig.IdSPSI));
                Repoglt_det.save(6, new Context.glt_det(), dbitem, code + "-" + urutanBatal, 0, dummyAdminUangJalan.Multidrop, Repoac_mstr.FindByPk(erpConfig.IdMultidrop));
                Repoglt_det.save(7, new Context.glt_det(), dbitem, code + "-" + urutanBatal, 0, tambahanRute, Repoac_mstr.FindByPk(erpConfig.IdTambahanRuteMuat));
                Repoglt_det.save(8, new Context.glt_det(), dbitem, code + "-" + urutanBatal, 0, dummyAdminUangJalan.AdminUangJalanTambahanLain.Sum(s => s.Values), Repoac_mstr.FindByPk(erpConfig.IdTambahanRuteLain));
                Repoglt_det.save(9, new Context.glt_det(), dbitem, code + "-" + urutanBatal, dummyAdminUangJalan.KasbonDriver1, 0, Repoac_mstr.FindByPk(erpConfig.IdKasbonAuj));
                Repoglt_det.save(10, new Context.glt_det(), dbitem, code + "-" + urutanBatal, dummyAdminUangJalan.KlaimDriver1, 0, Repoac_mstr.FindByPk(erpConfig.IdKlaimAuj));
                Repoglt_det.save(11, new Context.glt_det(), dbitem, code + "-" + urutanBatal, dummyAdminUangJalan.AdminUangJalanPotonganDriver.Sum(s => s.Value), 0, Repoac_mstr.FindByPk(erpConfig.IdPotonganLainAuj));
                Repoglt_det.save(12, new Context.glt_det(), dbitem, code + "-" + urutanBatal, dummyAdminUangJalan.AdminUangJalanVoucherKapal.Sum(s => s.Value), 0, Repoac_mstr.FindByPk(erpConfig.IdVoucherKapal));
                Repoglt_det.save(13, new Context.glt_det(), dbitem, code + "-" + urutanBatal, dummyAdminUangJalan.AdminUangJalanVoucherSpbu.Sum(s => s.Value), 0, Repoac_mstr.FindByPk(erpConfig.IdVoucherSolar));
                Repoglt_det.save(14, new Context.glt_det(), dbitem, code + "-" + urutanBatal, dummyAdminUangJalan.AdminUangJalanUangTf.Sum(s => s.Value), 0, Repoac_mstr.FindByPk(erpConfig.IdAUJCredit));
            }

            if (dbitem.Status != "batal order"){
                dbitem.Status = "save";
                dbitem.IsBatalTruk = true;
                if (dbitem.SalesOrderOncallId.HasValue){
                    dbitem.KeteranganBatal = "Ganti truck dari " + dbitem.SalesOrderOncall.DataTruck.VehicleNo + " " + Keterangan;//Ganti truck dari <nopol sebelumnya > ke <nopol yang dipilih> [isi dari keterangan batal truck ]
                    dbitem.SalesOrderOncall.Keterangan = "Ganti truck dari " + dbitem.SalesOrderOncall.DataTruck.VehicleNo + " " + Keterangan;//Ganti truck dari <nopol sebelumnya > ke <nopol yang dipilih> [isi dari keterangan batal truck ]
                    dbitem.SalesOrderOncall.Driver1Id = null;
                    dbitem.SalesOrderOncall.Driver2Id = null;
                    dbitem.SalesOrderOncall.IdDataTruck = null;
                    RepoAuditrail.SetAuditTrail(
                        "UPDATE dbo.\"SalesOrderOncall\" SET \"Driver1Id\" = NULL , Driver2Id = NULL, IdDataTruck = NULL WHERE \"SalesOrderOnCallId\" = " + dbitem.SalesOrderOncallId, "List Order", "Batal Truk", UserPrincipal.id
                    );
                }
                else if (dbitem.SalesOrderPickupId.HasValue){
                    dbitem.KeteranganBatal = "Ganti truck dari " + dbitem.SalesOrderPickup.DataTruck.VehicleNo + " " + Keterangan;//Ganti truck dari <nopol sebelumnya > ke <nopol yang dipilih> [isi dari keterangan batal truck ]
                    dbitem.SalesOrderPickup.Keterangan = "Ganti truck dari " + dbitem.SalesOrderPickup.DataTruck.VehicleNo + " " + Keterangan;//Ganti truck dari <nopol sebelumnya > ke <nopol yang dipilih> [isi dari keterangan batal truck ]
                    dbitem.SalesOrderPickup.Driver1Id = null;
                    dbitem.SalesOrderPickup.Driver2Id = null;
                    dbitem.SalesOrderPickup.IdDataTruck = null;
                }
                else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue){
                    dbitem.KeteranganBatal = "Ganti truck dari " + dbitem.SalesOrderProsesKonsolidasi.DataTruck.VehicleNo + " " + Keterangan;//Ganti truck dari <nopol sebelumnya > ke <nopol yang dipilih> [isi dari keterangan batal truck ]
                    dbitem.SalesOrderProsesKonsolidasi.Keterangan = "Ganti truck dari " + dbitem.SalesOrderProsesKonsolidasi.DataTruck.VehicleNo + " " + Keterangan;//Ganti truck dari <nopol sebelumnya > ke <nopol yang dipilih> [isi dari keterangan batal truck ]
                    dbitem.SalesOrderProsesKonsolidasi.Driver1Id = null;
                    dbitem.SalesOrderProsesKonsolidasi.Driver2Id = null;
                    dbitem.SalesOrderProsesKonsolidasi.IdDataTruck = null;
                }
                batalOrder.IdSalesOrder = dbitem.Id;
                batalOrder.Keterangan = Keterangan;
                batalOrder.ModifiedDate = DateTime.Now;
                batalOrder.IsBatalTruk = true;
                batalOrder.Code = code;
                batalOrder.UrutanBatal = urutanBatal;
                batalOrder.IsTransfer = IsTransfer == "1";
                RepoSalesOrder.save(dbitem);
                RepoBatalOrder.save(batalOrder, UserPrincipal.id);

                return RedirectToAction("index","ListOrder");
            }
            ViewBag.status = "batal order";
            return View("Form", model);
        }

        public ActionResult EditKontrak(int id, int idsokontrak)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            Context.SalesOrderKontrakListSo soKontrak = dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => d.Id == idsokontrak).FirstOrDefault();
            //ambil jumlah so yang sama admin uang jalan nya
            int jumSo = dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => d.IdAdminUangJalan == soKontrak.IdAdminUangJalan).Count();
            ViewBag.dbsoPerDriverCount = jumSo;
            dbitem.SalesOrderKontrak.SalesOrderKontrakListSo = new List<Context.SalesOrderKontrakListSo>();
            dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Add(soKontrak);
            AdminUangJalan model = new AdminUangJalan();
            if (soKontrak.IdAdminUangJalan.HasValue)
                model = new AdminUangJalan(soKontrak.AdminUangJalan, Repoatm.FindAll(), RepoBor.FindAll());

            model.IdSalesOrder = dbitem.Id;
            model.ListIdSo = idsokontrak.ToString(); 
            ViewBag.Status = soKontrak.Status;
            model.ModelKontrak = new SalesOrderKontrak(dbitem);
            if (soKontrak.Status.ToLower().Contains("konfirmasi"))
            {
                model.StatusSo = "Konfirmasi";
            }
            if (model.IdDriver1.HasValue)
            {
                model.IdDriver1 = model.IdDriver1;
                model.NamaDriver1 = model.NamaDriver1;
                model.KeteranganGanti1 = model.KeteranganGanti1;
            }
            else
            {
                model.IdDriver1 = soKontrak.Driver1Id;
                model.NamaDriver1 = soKontrak.Driver1.KodeDriver + " - " + soKontrak.Driver1.NamaDriver;
            }
            if (model.IdDriver2.HasValue)
            {
                model.IdDriver2 = model.IdDriver2;
                model.NamaDriver2 = model.NamaDriver2;
                model.KeteranganGanti2 = model.KeteranganGanti2;
            }
            else
            {
                if (soKontrak.Driver2Id.HasValue)
                {
                    model.IdDriver2 = soKontrak.Driver2Id;
                    model.NamaDriver2 = soKontrak.Driver2.KodeDriver + " - " + soKontrak.Driver2.NamaDriver;
                }
            }
            ViewBag.Title = "Batal Truk " + dbitem.SalesOrderKontrak.SONumber;
            return View("Form", model);
        }

        [HttpPost]
        public ActionResult EditKontrak(AdminUangJalan model, string Keterangan)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(model.IdSalesOrder.Value);
            Context.SalesOrderKontrakListSo sokontrak = dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => d.Id == int.Parse(model.ListIdSo)).FirstOrDefault();
            int jumSo = dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => d.IdAdminUangJalan == sokontrak.IdAdminUangJalan && d.IsBatalTruck != true).Count();
            
            Context.BatalOrder batalOrder = new Context.BatalOrder();
            Context.AdminUangJalan dbauj = sokontrak.AdminUangJalan;

            //cek status na
            if (sokontrak.Status == "dispatched")
            {
                Context.SettlementBatal dbsettlement = new Context.SettlementBatal();
                //ambil data 
                dbsettlement.IdSalesOrder = model.IdSalesOrder;
                dbsettlement.IdSoKontrak = model.ListIdSo;
                if (dbauj.AdminUangJalanUangTf.Any(d => d.Keterangan == "Tunai"))
                {
                    dbsettlement.KasDiterima = dbauj.AdminUangJalanUangTf.Where(d => d.Keterangan == "Tunai").FirstOrDefault().JumlahTransfer / jumSo;
                }
                if (dbauj.AdminUangJalanUangTf.Any(d => d.Keterangan.Contains("Transfer")))
                {
                    dbsettlement.TransferDiterima = dbauj.AdminUangJalanUangTf.Where(d => d.Keterangan.Contains("Transfer")).Sum(t => t.JumlahTransfer) / jumSo;
                }
                dbsettlement.SolarDiterima = dbauj.AdminUangJalanVoucherSpbu.Sum(s => s.Value) / jumSo;
                dbsettlement.KapalDiterima = dbauj.AdminUangJalanVoucherKapal.Sum(s => s.Value) / jumSo;
                dbsettlement.JenisBatal = "Batal Truk";
                dbsettlement.IdDriver = sokontrak.Driver1Id;
                RepoSettBatal.save(dbsettlement, UserPrincipal.id, "Batal Truk");
            }
            else if (sokontrak.Status == "admin uang jalan")
            {
                foreach(Context.AdminUangJalanUangTf dbUang in dbauj.AdminUangJalanUangTf){
                    dbUang.Value = dbUang.Value / jumSo * (jumSo-1);
                    RepoSalesOrder.saveUangTf(dbUang);
                }
            }

            foreach (var trukItem in dbitem.SalesOrderKontrak.SalesOrderKontrakTruck.Where(d => d.DataTruckId == sokontrak.IdDataTruck).ToList())
            {
                trukItem.DataTruckId = null;
                trukItem.IdDriver1 = null;
                trukItem.IdDriver2 = null;
            }
            sokontrak.IsProses = false;
            sokontrak.Urutan = 0;
            sokontrak.IsBatalTruck = true;
            sokontrak.IdDataTruck = null;
            sokontrak.Driver1Id = null;
            sokontrak.Driver2Id = null;
            sokontrak.Status = "draft planning";

            batalOrder.IdSalesOrder = dbitem.Id;
            batalOrder.IdSoKontrak = model.ListIdSo;
            batalOrder.Keterangan = Keterangan;
            batalOrder.ModifiedDate = DateTime.Now;
            batalOrder.IsBatalTruk = true;

            RepoSalesOrder.save(dbitem);
            RepoBatalOrder.save(batalOrder, UserPrincipal.id);

            ViewBag.status = "batal order";
            return RedirectToAction("index", "ListOrder");
        }
    }
}