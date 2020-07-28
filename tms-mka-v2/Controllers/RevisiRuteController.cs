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
    public class RevisiRuteController : BaseController
    {
        private ISalesOrderRepo RepoSalesOrder;
        private IRevisiRuteRepo RepoRevisiRute;
        private ISettlementBatalRepo RepoSettBatal;
        private IBatalOrderRepo RepoBatalOrder;
        private Iglt_detRepo Repoglt_det;
        private IERPConfigRepo RepoERPConfig;
        private Iac_mstrRepo Repoac_mstr;
        private Ibk_mstrRepo Repobk_mstr;
        private IAuditrailRepo RepoAuditrail;
        private Iso_mstrRepo Reposo_mstr;
        public RevisiRuteController(IUserReferenceRepo repoBase, ILookupCodeRepo repoLookup, ISalesOrderRepo repoSalesOrder, IRevisiRuteRepo repoRevisiRute,
            ISettlementBatalRepo repoSettBatal, IBatalOrderRepo repoBatalOrder, Iglt_detRepo repoglt_det, IERPConfigRepo repoERPConfig, Iac_mstrRepo repoac_mstr,
            Ibk_mstrRepo repobk_mstr, IAuditrailRepo repoAuditrail, Iso_mstrRepo reposo_mstr)
            : base(repoBase, repoLookup)
        {
            RepoSalesOrder = repoSalesOrder;
            RepoRevisiRute = repoRevisiRute;
            RepoSettBatal = repoSettBatal;
            RepoBatalOrder = repoBatalOrder;
            Repoglt_det = repoglt_det;
            RepoERPConfig = repoERPConfig;
            Repoac_mstr = repoac_mstr;
            Repobk_mstr = repobk_mstr;
            RepoAuditrail = repoAuditrail;
            Reposo_mstr = reposo_mstr;
        }

        [MyAuthorize(Menu = "Revisi Rute", Action="create")]
        public ActionResult Edit(int idSo)
        {
            Context.SalesOrder dbso = RepoSalesOrder.FindByPK(idSo);

            RevisiRute model = new RevisiRute(dbso);
            ViewBag.kondisi = "revisiRute";

            if(dbso.SalesOrderOncallId.HasValue)
                return View("FormOncall", model);
            else if (dbso.SalesOrderPickupId.HasValue)
                return View("FormPickup", model);
            else if (dbso.SalesOrderProsesKonsolidasiId.HasValue)
                return View("FormKonsolidasi", model);

            return View("Form", model);
        }
        [HttpPost]
        public ActionResult Edit(RevisiRute model)
        {
            Context.SalesOrder dbso = RepoSalesOrder.FindByPK(model.IdSalesOrder.Value);
            Context.RevisiRute revisiRute = new Context.RevisiRute();
            revisiRute.Code = "RR" + (dbso.SalesOrderOncallId.HasValue ? dbso.SalesOrderOncall.SONumber : dbso.SalesOrderProsesKonsolidasiId.HasValue ? dbso.SalesOrderProsesKonsolidasi.SONumber : dbso.SalesOrderPickupId.HasValue ? dbso.SalesOrderPickup.SONumber : dbso.SalesOrderKontrak.SONumber);
            Context.SalesOrder dummySo = new Context.SalesOrder();
            
            if (ModelState.IsValid)
            {
                if (dbso.Status == "dispatched" || dbso.Status == "admin uang jalan")
                {
                    Context.AdminUangJalan dummyAdminUangJalan = dbso.AdminUangJalan;
                    Context.BatalOrder batalOrder = new Context.BatalOrder();
                    //batal
                    dbso.Status = "batal order";
                    dbso.KeteranganBatal = "Revisi rute dari " + model.RuteLama + " ke " + model.RuteBaru;//"Revisi RUte" dari <rute sebelumnya> ke <rute yang dipilih > [isi dari keterangan revisi rute ]
                    batalOrder.IdSalesOrder = dbso.Id;
                    batalOrder.Keterangan = "Revisi Rute";
                    batalOrder.ModifiedDate = DateTime.Now;
                    RepoSalesOrder.save(dbso);
                    RepoBatalOrder.save(batalOrder, UserPrincipal.id);
                    if (dbso.Status == "dispatched"){
                        //batalkeun so na
                        Context.SettlementBatal dbsettlement = new Context.SettlementBatal();
                        //settlement batal
                        dbsettlement.IdDriver = dummyAdminUangJalan.IdDriver1;
                        dbsettlement.IdSalesOrder = dbso.Id;
                        if (dummyAdminUangJalan.AdminUangJalanUangTf.Any(d => d.Keterangan == "Tunai"))
                        {
                            dbsettlement.KasDiterima = dummyAdminUangJalan.AdminUangJalanUangTf.Where(d => d.Keterangan == "Tunai").FirstOrDefault().JumlahTransfer;
                        }
                        if (dummyAdminUangJalan.AdminUangJalanUangTf.Any(d => d.Keterangan.Contains("Transfer")))
                        {
                            dbsettlement.TransferDiterima = dummyAdminUangJalan.AdminUangJalanUangTf.Where(d => d.Keterangan.Contains("Transfer")).Sum(t => t.JumlahTransfer);
                        }
                        dbsettlement.SolarDiterima = dummyAdminUangJalan.AdminUangJalanVoucherSpbu.Sum(s => s.Value);
                        dbsettlement.KapalDiterima = dummyAdminUangJalan.AdminUangJalanVoucherKapal.Sum(s => s.Value);
                        dbsettlement.JenisBatal = "Batal Order";
                        RepoSettBatal.save(dbsettlement, UserPrincipal.id, "Batal Order");
                    }
                    else{
                        //jurnal balik cynt
                        Context.ERPConfig erpConfig = RepoERPConfig.FindByFrist();
                        decimal? tambahanRute = dummyAdminUangJalan.AdminUangJalanTambahanRute.Sum(s => s.values);
                        decimal? boronganDasar = dummyAdminUangJalan.TotalAlokasi - dummyAdminUangJalan.Kawalan - dummyAdminUangJalan.Timbangan - dummyAdminUangJalan.Karantina - dummyAdminUangJalan.SPSI - dummyAdminUangJalan.Multidrop - tambahanRute - dummyAdminUangJalan.AdminUangJalanTambahanLain.Sum(s => s.Values);
                        Repoglt_det.saveFromAc(1, revisiRute.Code, 0, boronganDasar, Repoac_mstr.FindByPk(erpConfig.IdBoronganDasar));
                        Repoglt_det.saveFromAc(2, revisiRute.Code, 0, dummyAdminUangJalan.Kawalan, Repoac_mstr.FindByPk(erpConfig.IdKawalan));
                        Repoglt_det.saveFromAc(3, revisiRute.Code, 0, dummyAdminUangJalan.Timbangan, Repoac_mstr.FindByPk(erpConfig.IdTimbangan));
                        Repoglt_det.saveFromAc(4, revisiRute.Code, 0, dummyAdminUangJalan.Karantina, Repoac_mstr.FindByPk(erpConfig.IdKarantina));
                        Repoglt_det.saveFromAc(5, revisiRute.Code, 0, dummyAdminUangJalan.SPSI, Repoac_mstr.FindByPk(erpConfig.IdSPSI));
                        Repoglt_det.saveFromAc(6, revisiRute.Code, 0, dummyAdminUangJalan.Multidrop, Repoac_mstr.FindByPk(erpConfig.IdMultidrop));
                        Repoglt_det.saveFromAc(7, revisiRute.Code, 0, tambahanRute, Repoac_mstr.FindByPk(erpConfig.IdTambahanRuteMuat));
                        Repoglt_det.saveFromAc(8, revisiRute.Code, 0, dummyAdminUangJalan.AdminUangJalanTambahanLain.Sum(s => s.Values), Repoac_mstr.FindByPk(erpConfig.IdTambahanRuteLain));
                        Repoglt_det.saveFromAc(9, revisiRute.Code, dummyAdminUangJalan.KasbonDriver1, 0, Repoac_mstr.FindByPk(erpConfig.IdKasbonAuj));
                        Repoglt_det.saveFromAc(10, revisiRute.Code, dummyAdminUangJalan.KlaimDriver1, 0, Repoac_mstr.FindByPk(erpConfig.IdKlaimAuj));
                        Repoglt_det.saveFromAc(11, revisiRute.Code, dummyAdminUangJalan.AdminUangJalanPotonganDriver.Sum(s => s.Value), 0, Repoac_mstr.FindByPk(erpConfig.IdPotonganLainAuj));
                        Repoglt_det.saveFromAc(12, revisiRute.Code, dummyAdminUangJalan.AdminUangJalanVoucherKapal.Sum(s => s.Value), 0, Repoac_mstr.FindByPk(erpConfig.IdVoucherKapal));
                        Repoglt_det.saveFromAc(13, revisiRute.Code, dummyAdminUangJalan.AdminUangJalanVoucherSpbu.Sum(s => s.Value), 0, Repoac_mstr.FindByPk(erpConfig.IdVoucherSolar));
                        Repoglt_det.saveFromAc(14, revisiRute.Code, dummyAdminUangJalan.AdminUangJalanUangTf.Sum(s => s.Value), 0, Repoac_mstr.FindByPk(erpConfig.IdAUJCredit));
                    }
                    //create so baru
                    dummySo.isReturn = true;
                    dummySo.Status = "Draft";
                    dummySo.AdminUangJalanId = null;
                    dummySo.AdminUangJalan = null;
                    dummySo.DateStatus = DateTime.Now;

                    try{
                        SalesOrderLoadUnload[] resultLoad = JsonConvert.DeserializeObject<SalesOrderLoadUnload[]>(model.StrLoad);
                        model.ListLoad = resultLoad.ToList();
                        SalesOrderLoadUnload[] resultUnload = JsonConvert.DeserializeObject<SalesOrderLoadUnload[]>(model.StrUnload);
                        model.ListUnload = resultUnload.ToList();
                    }
                    catch (Exception e)
                    {
                    }

                    if (dbso.SalesOrderOncallId.HasValue)
                    {
                        //urus anak na
                        Context.SalesOrderOncall dboncall = new Context.SalesOrderOncall();
                        SalesOrderOncall modelOncall = new SalesOrderOncall(dbso);
                        modelOncall.setDb(dboncall);
                        dboncall.SalesOrderOnCallId = 0;
                        dboncall.Urutan = RepoSalesOrder.getUrutanOnCAll(modelOncall.TanggalMuat.Value) + 1;
                        dboncall.SONumber = RepoSalesOrder.generateCodeOnCall(modelOncall.TanggalMuat.Value, dboncall.Urutan);
                        dboncall.DN = "DN" + dboncall.SONumber;
                        dboncall.IdDaftarHargaItem = model.IdRute;
                        dboncall.StrDaftarHargaItem = model.RuteBaru;
                        dboncall.StrMultidrop = model.MultidropBaru;
                        dboncall.SalesOrderOnCallLoadingAdd.Clear();
                        dboncall.Keterangan = "Revisi rute dari " + model.RuteLama + " ke " + model.RuteBaru + " - " + dbso.SalesOrderOncall.Keterangan;
                        foreach (SalesOrderLoadUnload item in model.ListLoad)
                        {
                            dboncall.SalesOrderOnCallLoadingAdd.Add(new Context.SalesOrderOnCallLoadingAdd()
                            {
                                CustomerId = dboncall.CustomerId,
                                CustomerLoadingAddressId = item.Id,
                                urutan = item.urutan,
                                IsSelect = item.IsSelect
                            });
                        }

                        dboncall.SalesOrderOnCallUnLoadingAdd.Clear();
                        foreach (SalesOrderLoadUnload item in model.ListUnload)
                        {
                            dboncall.SalesOrderOnCallUnLoadingAdd.Add(new Context.SalesOrderOnCallUnLoadingAdd()
                            {
                                CustomerId = dboncall.CustomerId,
                                CustomerUnloadingAddressId = item.Id,
                                urutan = item.urutan,
                                IsSelect = item.IsSelect
                            });
                        }

                        dummySo.SalesOrderOncall = dboncall;
                        RepoAuditrail.SetAuditTrail("INSERT INTO dbo.\"SalesOrderOncall\" (\"SONumber\", \"Urutan\", \"TanggalOrder\", \"JamOrder\", \"CustomerId\", \"PrioritasId\", \"JenisTruckId\", \"ProductId\", " +
                            "\"TanggalMuat\", \"JamMuat\", \"Keterangan\", \"KeteranganLoading\", \"KeteranganUnloading\", \"IdDaftarHargaItem\", \"StrDaftarHargaItem\", \"StrMultidrop\", \"IdDataTruck\", \"Driver1Id\", " +
                            "\"KeteranganDriver1\", \"Driver2Id\", \"KeteranganDriver2\", \"IsCash\", \"KeteranganRek\", \"IdDriverTitip\", \"DN\", \"KeteranganDataTruck\", \"AtmId\") VALUES (" + dboncall.SONumber + ", "
                            + dboncall.Urutan + ", " + dboncall.TanggalOrder + ", " + dboncall.JamOrder + ", " + dboncall.CustomerId + ", " + dboncall.PrioritasId + ", " + dboncall.JenisTruckId + ", " + dboncall.ProductId +
                            ", " + dboncall.TanggalMuat + ", " + dboncall.JamMuat + ", " + dboncall.Keterangan + ", " + dboncall.KeteranganLoading + ", " + dboncall.KeteranganUnloading + ", " + dboncall.IdDaftarHargaItem +
                            "," + dboncall.StrDaftarHargaItem + ", " + dboncall.StrMultidrop + ", " + dboncall.IdDataTruck + ", " + dboncall.Driver1Id + ", " + dboncall.KeteranganDriver1 + ", " + dboncall.Driver2Id + ", " + 
                            dboncall.KeteranganDriver2 + ", " + dboncall.IsCash + ", " + dboncall.KeteranganRek + ", " + dboncall.IdDriverTitip + ", " + dboncall.DN + ", " + dboncall.KeteranganDataTruck + ", " +
                            dboncall.AtmId + ");", "List Order", "Revisi Jenis Truk", UserPrincipal.id);
                    }
                    else if (dbso.SalesOrderPickupId.HasValue)
                    {
                        Context.SalesOrderPickup dbpickup = new Context.SalesOrderPickup();
                        SalesOrderPickup modelPickup = new SalesOrderPickup(dbso);
                        modelPickup.setDb(dbpickup);

                        dbpickup.SalesOrderPickupId = 0;
                        dbpickup.Urutan = RepoSalesOrder.getUrutanPickup(modelPickup.TanggalPickup.Value) + 1;
                        dbpickup.SONumber = RepoSalesOrder.generatePickup(modelPickup.TanggalPickup.Value, dbpickup.Urutan);
                        dbpickup.Keterangan = "Revisi rute dari " + model.RuteLama + " ke " + model.RuteBaru + " - " + dbso.SalesOrderPickup.Keterangan;

                        dbpickup.SalesOrderPickupLoadingAdd.Clear();
                        foreach (SalesOrderLoadUnload item in model.ListLoad)
                        {
                            dbpickup.SalesOrderPickupLoadingAdd.Add(new Context.SalesOrderPickupLoadingAdd()
                            {
                                CustomerId = dbpickup.CustomerId,
                                CustomerLoadingAddressId = item.Id,
                                urutan = item.urutan,
                                IsSelect = item.IsSelect
                            });
                        }

                        dbpickup.SalesOrderPickupUnLoadingAdd.Clear();
                        RepoAuditrail.saveDelAllSalesOrderPickupUnLoadingAddQuery(dbpickup, UserPrincipal.id);
                        foreach (SalesOrderLoadUnload item in model.ListUnload)
                        {
                            dbpickup.SalesOrderPickupUnLoadingAdd.Add(new Context.SalesOrderPickupUnLoadingAdd()
                            {
                                CustomerId = dbpickup.CustomerId,
                                CustomerUnloadingAddressId = item.Id,
                                urutan = item.urutan,
                                IsSelect = item.IsSelect
                            });
                        }

                        dummySo.SalesOrderPickup = dbpickup;
                    }
                    else if (dbso.SalesOrderProsesKonsolidasiId.HasValue)
                    {
                        Context.SalesOrderProsesKonsolidasi dbkonsolidasi = new Context.SalesOrderProsesKonsolidasi();
                        SalesOrderProsesKonsolidasi modelKonsolidasi = new SalesOrderProsesKonsolidasi(dbso);
                        modelKonsolidasi.setDb(dbkonsolidasi);

                        dbkonsolidasi.SalesOrderProsesKonsolidasiId = 0;
                        try{
                            dbkonsolidasi.Urutan = RepoSalesOrder.getUrutanProsesKonsolidasi(modelKonsolidasi.TanggalMuat.Value) + 1;
                        }
                        catch (Exception e)
                        {
                            dbkonsolidasi.Urutan = 0;
                        }
                        dbkonsolidasi.SONumber = RepoSalesOrder.generateProsesKonsolidasi(modelKonsolidasi.TanggalMuat.Value, dbkonsolidasi.Urutan);
                        dbkonsolidasi.DN = "DN" + dbkonsolidasi.SONumber;
                        dbkonsolidasi.IdDaftarHargaItem = model.IdRute;
                        dbkonsolidasi.StrDaftarHargaItem = model.RuteBaru;
                        dbkonsolidasi.Multidrop = model.MultidropBaru;
                        dbkonsolidasi.Keterangan = "Revisi rute dari " + model.RuteLama + " ke " + model.RuteBaru + " - " + dbso.SalesOrderProsesKonsolidasi.Keterangan;

                        dummySo.SalesOrderProsesKonsolidasi = dbkonsolidasi;
                        RepoAuditrail.saveSalesOrderProsesKonsolidasiQuery(dummySo.SalesOrderProsesKonsolidasi, UserPrincipal.id);
                    }

                    dummySo.Id = 0;
                    RepoSalesOrder.save(dummySo);
                    if (dbso.SalesOrderPickupId.HasValue){
                        foreach (Context.SalesOrderPickupUnLoadingAdd sopula in dbso.SalesOrderPickup.SalesOrderPickupUnLoadingAdd){
                            RepoAuditrail.saveSalesOrderPickupUnLoadingAddQuery(sopula, UserPrincipal.id);
                        }
                    }
                }
                else
                {
                    if (dbso.SalesOrderOncallId.HasValue)
                    {
                        dbso.SalesOrderOncall.IdDaftarHargaItem = model.IdRute;
                        dbso.SalesOrderOncall.StrDaftarHargaItem = model.RuteBaru;
                        dbso.SalesOrderOncall.StrMultidrop = model.MultidropBaru;
                        RepoAuditrail.SetAuditTrail(
                            "UPDATE dbo.\"SalesOrderOncall\" SET \"IdDaftarHargaItem\" = " + model.IdRute + ", \"StrDaftarHargaItem\" = " + model.RuteBaru + ", \"StrMultidrop\" = " + model.MultidropBaru + 
                            " WHERE \"SalesOrderOnCallId\" = " + dbso.SalesOrderOncallId + ";",
                            "List Order", "Revisi Rute", UserPrincipal.id
                        );
                    }
                    else if (dbso.SalesOrderPickupId.HasValue)
                    {
                        dbso.SalesOrderPickup.RuteId = model.IdRute;
                    }
                    else if (dbso.SalesOrderProsesKonsolidasiId.HasValue)
                    {
                        dbso.SalesOrderProsesKonsolidasi.IdDaftarHargaItem = model.IdRute;
                        dbso.SalesOrderProsesKonsolidasi.StrDaftarHargaItem = model.RuteBaru;
                        dbso.SalesOrderProsesKonsolidasi.Multidrop = model.MultidropBaru;
                        RepoAuditrail.saveUpdSalesOrderProsesKonsolidasiQuery(dbso.SalesOrderProsesKonsolidasi, UserPrincipal.id);
                    }
                    revisiRute.IdSalesOrder = model.IdSalesOrder.Value;
                    dbso.RuteRevised = true;
                    dbso.AdminUangJalanId = null;
                    dbso.AdminUangJalan = null;
                    RepoSalesOrder.save(dbso);
                    string sod_guid = Guid.NewGuid().ToString();
                    SyncToERP(dummySo, sod_guid);
                    dbso.oidErp = sod_guid;
                    RepoRevisiRute.save(revisiRute);
                }

                return RedirectToAction("Index", "ListOrder");
            }

            model = new RevisiRute(dbso);

            if (dbso.SalesOrderOncallId.HasValue)
                return View("FormOncall", model);
            else if (dbso.SalesOrderPickupId.HasValue)
                return View("FormPickup", model);
            else if (dbso.SalesOrderProsesKonsolidasiId.HasValue)
                return View("FormKonsolidasi", model);

            return View("");
        }
        public void SyncToERP(Context.SalesOrder dbso, string sod_guid)
        {
            //if (dbso.Status == "save")//save to so_mstr for invoice processing
            decimal harga = RepoSalesOrder.Harga(dbso);
            Context.SalesOrderOncall dbitem = dbso.SalesOrderOncall;
            string guid = Guid.NewGuid().ToString();
            Reposo_mstr.saveSoMstr(dbso, UserPrincipal.username, guid, dbitem.CustomerId.Value, harga);
            Reposo_mstr.saveSoDet(dbso, UserPrincipal.username, guid, sod_guid);
        }
    }
}