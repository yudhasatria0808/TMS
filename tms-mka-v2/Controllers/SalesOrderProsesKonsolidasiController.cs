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
    public class SalesOrderProsesKonsolidasiController : BaseController
    {
        private ISalesOrderRepo RepoSalesOrder;
        private IDaftarHargaKonsolidasiRepo RepoDaftarHarga;
        private IJenisTruckRepo RepoTruk;
        private IAuditrailRepo RepoAuditrail;
        public SalesOrderProsesKonsolidasiController(IUserReferenceRepo repoBase, ILookupCodeRepo repoLookup, ISalesOrderRepo repoSalesOrder, IDaftarHargaKonsolidasiRepo repoDaftarHarga,
            IJenisTruckRepo repoTruk, IAuditrailRepo repoAuditrail)
            : base(repoBase, repoLookup)
        {
            RepoSalesOrder = repoSalesOrder;
            RepoDaftarHarga = repoDaftarHarga;
            RepoTruk = repoTruk;
            RepoAuditrail = repoAuditrail;
        }
        [MyAuthorize(Menu = "Sales Order Proses Konsolidasi", Action="read")]
        public ActionResult Index()
        {
            ViewBag.listKolom = ListKolom.Where(d => d.Action == "Index" && d.Controller == "SalesOrderProsesKonsolidasi").ToList();
            return View();
        }

        public string Binding()
        {
            GridRequestParameters param = GridRequestParameters.Current;

            List<Context.SalesOrder> items = RepoSalesOrder.FindAllProsesKonsolidasi();
            List<SalesOrderProsesKonsolidasi> ListModel = new List<SalesOrderProsesKonsolidasi>();
            foreach (Context.SalesOrder item in items)
            {
                ListModel.Add(new SalesOrderProsesKonsolidasi(item));
            }

            int total = RepoSalesOrder.CountProsesKonsolidasi(param.Filters);

            return new JavaScriptSerializer().Serialize(new { total = total, data = ListModel });
        }
        public string BindingComment(int id)
        {
            Context.SalesOrder items = RepoSalesOrder.FindByProsesKonsolidasi(id);

            return new JavaScriptSerializer().Serialize(new
            {
                data = items.SalesOrderProsesKonsolidasi.SalesOrderProsesKonsolidasiComment.ToList().Select(d => new
                {
                    Id = d.Id,
                    Tanggal = d.Tanggal,
                    CommentUser = d.CommentUser,
                    Username = d.Username,
                    Action = d.Action
                })
            });
        }
        [MyAuthorize(Menu = "Sales Order Proses Konsolidasi", Action="create")]
        public ActionResult Add()
        {
            SalesOrderProsesKonsolidasi model = new SalesOrderProsesKonsolidasi();
            return View("Form", model);
        }
        [HttpPost]
        public ActionResult Add(SalesOrderProsesKonsolidasi model, string btnsave)
        {
            //cek validasi so
            List<string> MsgError = new List<string>();
            //if (model.IdJnsTruck.HasValue)
            //{
            //    if (model.StrListSo != null && model.StrListSo != "")
            //    {
            //        foreach (string item in model.StrListSo.Split(','))
            //        {
            //            IsValidSo(int.Parse(item), model.IdJnsTruck.Value, MsgError);
            //        }
            //    }
            //}

            SalesOrderLoadUnload[] resultLoad = JsonConvert.DeserializeObject<SalesOrderLoadUnload[]>(model.StrLoad);
            model.ListLoad = resultLoad.ToList();
            SalesOrderLoadUnload[] resultUnload = JsonConvert.DeserializeObject<SalesOrderLoadUnload[]>(model.StrUnload);
            model.ListUnload = resultUnload.ToList();

            if (ModelState.IsValid && MsgError.Count < 1)
            {
                Context.SalesOrder dbso = new Context.SalesOrder();
                Context.SalesOrderProsesKonsolidasi dbitem = new Context.SalesOrderProsesKonsolidasi();
                SetIsselect(model.StrListSo, null);
                model.setDb(dbitem);
                dbitem.Urutan = RepoSalesOrder.getUrutanProsesKonsolidasi(model.TanggalMuat.Value) + 1;
                dbitem.SONumber = RepoSalesOrder.generateProsesKonsolidasi(model.TanggalMuat.Value, dbitem.Urutan);
                dbitem.DN = "DN" + dbitem.SONumber;
                dbso.SalesOrderProsesKonsolidasi = dbitem;

                if (btnsave == "save")
                    dbso.Status = "save";
                else if (btnsave == "draft")
                    dbso.Status = "draft";
                else
                    dbso.Status = model.Status;

                RepoSalesOrder.save(dbso);
                RepoAuditrail.saveSalesOrderProsesKonsolidasiQuery(dbitem, UserPrincipal.id);
                foreach (Context.SalesOrderProsesKonsolidasiItem item in dbitem.SalesOrderProsesKonsolidasiItem)
                {
                    RepoAuditrail.saveSalesOrderProsesKonsolidasiItemQuery(item, UserPrincipal.id);
                }
                foreach (Context.SalesOrderProsesKonsolidasiLoadingAdd item in dbitem.SalesOrderProsesKonsolidasiLoadingAdd)
                {
                    RepoAuditrail.saveSalesOrderProsesKonsolidasiLoadingAddQuery(item, UserPrincipal.id);
                }
                foreach (Context.SalesOrderProsesKonsolidasiUnLoadingAdd item in dbitem.SalesOrderProsesKonsolidasiUnLoadingAdd)
                {
                    RepoAuditrail.saveSalesOrderProsesKonsolidasiUnLoadingAddQuery(item, UserPrincipal.id);
                }

                return RedirectToAction("Index");
            }


            ViewBag.ErrorSo = MsgError;

            return View("Form", model);
        }
        [MyAuthorize(Menu = "Sales Order Proses Konsolidasi", Action="update")]
        public ActionResult Edit(int id)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            SalesOrderProsesKonsolidasi model = new SalesOrderProsesKonsolidasi(dbitem);

            ViewBag.name = model.SONumber;
            return View("Form", model);
        }
        [HttpPost]
        public ActionResult Edit(SalesOrderProsesKonsolidasi model, string btnsave)
        {
            //cek validasi so
            List<string> MsgError = new List<string>();

            SalesOrderLoadUnload[] resultLoad = JsonConvert.DeserializeObject<SalesOrderLoadUnload[]>(model.StrLoad);
            model.ListLoad = resultLoad.ToList();
            SalesOrderLoadUnload[] resultUnload = JsonConvert.DeserializeObject<SalesOrderLoadUnload[]>(model.StrUnload);
            model.ListUnload = resultUnload.ToList();
            if (ModelState.IsValid && MsgError.Count < 1)
            {
                Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(model.SalesOrderId.Value);
                var idremove = dbitem.SalesOrderProsesKonsolidasi.SalesOrderProsesKonsolidasiItem.Select(d => d.SalesOrderKonsolidasiId).ToList();
                List<string> listRemove = idremove.ConvertAll(i => i.ToString());
                SetIsselect(model.StrListSo, listRemove);

                model.setDb(dbitem.SalesOrderProsesKonsolidasi);

                if (btnsave == "save")
                    dbitem.Status = "save";
                else if (btnsave == "draft")
                    dbitem.Status = "draft";
                else
                    dbitem.Status = model.Status;

                RepoSalesOrder.save(dbitem);
                RepoAuditrail.saveUpdSalesOrderProsesKonsolidasiQuery(dbitem.SalesOrderProsesKonsolidasi, UserPrincipal.id);
                foreach (Context.SalesOrderProsesKonsolidasiItem item in dbitem.SalesOrderProsesKonsolidasi.SalesOrderProsesKonsolidasiItem)
                {
                    RepoAuditrail.saveSalesOrderProsesKonsolidasiItemQuery(item, UserPrincipal.id);
                }
                foreach (Context.SalesOrderProsesKonsolidasiLoadingAdd item in dbitem.SalesOrderProsesKonsolidasi.SalesOrderProsesKonsolidasiLoadingAdd)
                {
                    RepoAuditrail.saveSalesOrderProsesKonsolidasiLoadingAddQuery(item, UserPrincipal.id);
                }
                foreach (Context.SalesOrderProsesKonsolidasiUnLoadingAdd item in dbitem.SalesOrderProsesKonsolidasi.SalesOrderProsesKonsolidasiUnLoadingAdd)
                {
                    RepoAuditrail.saveSalesOrderProsesKonsolidasiUnLoadingAddQuery(item, UserPrincipal.id);
                }
                return RedirectToAction("Index");
            }

            ViewBag.ErrorSo = MsgError;

            return View("Form", model);
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            ResponeModel response = new ResponeModel(true);
            Context.SalesOrder dbItem = RepoSalesOrder.FindByPK(id);

            RepoSalesOrder.delete(dbItem, UserPrincipal.id);

            return Json(response);
        }
        [HttpPost]
        public JsonResult Submit(int id)
        {
            ResponeModel response = new ResponeModel(true);
            Context.SalesOrder dbItem = RepoSalesOrder.FindByPK(id);
            dbItem.Status = "save";
            RepoSalesOrder.save(dbItem);

            return Json(response);
        }
        private void SetIsselect(string listAdd, List<string> listRemove)
        {
            //isselect = false
            if (listRemove != null)
            {
                foreach (string StrId in listRemove)
                {
                    Context.SalesOrder dbRemove = RepoSalesOrder.FindByKonsolidasi(int.Parse(StrId));
                    dbRemove.SalesOrderKonsolidasi.IsSelect = false;
                    RepoSalesOrder.save(dbRemove);
                }
            }

            //isselect = true
            if (listAdd != "")
            {
                foreach (string StrId in listAdd.Split(','))
                {
                    Context.SalesOrder dbAdd = RepoSalesOrder.FindByKonsolidasi(int.Parse(StrId));
                    dbAdd.SalesOrderKonsolidasi.IsSelect = true;
                    RepoSalesOrder.save(dbAdd);
                }
            }


        }
        private void IsValidSo(int idSo, int idTruck, List<string> MsgError)
        {
            //cokot satuan anu tina so  berdasarkan id kon truk, 
            Context.SalesOrder dbso = RepoSalesOrder.FindByKonsolidasi(idSo);
            List<Context.DaftarHargaKonsolidasiItem> dbItems = RepoDaftarHarga.FindByItemId(dbso.SalesOrderKonsolidasi.IdDaftarHargaItem.Value).DaftarHargaKonsolidasiItem.ToList();
            if (dbItems != null)
            {
                //cokot item daftar harga sesuai anu dipilih tina so
                Context.DaftarHargaKonsolidasiItem dbItemHarga = dbItems.Where(d => d.Id == dbso.SalesOrderKonsolidasi.IdDaftarHargaItem.Value).FirstOrDefault();
                if (dbso.SalesOrderKonsolidasi.PerhitunganDasar != "Manual")
                {
                    //cek aya teu data na berdasar kan truk jeung satuana na

                    //khusus tonase
                    if (dbso.SalesOrderKonsolidasi.PerhitunganDasar == "Tonase")
                    {
                        if (dbItemHarga.IdJenisKendaraan != idTruck || (dbItemHarga.LookupCodeSatuan.Nama.ToLower() != "kg" && dbItemHarga.LookupCodeSatuan.Nama.ToLower() != "ton"))
                        {
                            MsgError.Add("Tidak terdapat Daftar harga pada " + dbso.SalesOrderKonsolidasi.SONumber + " untuk satuan " + dbso.SalesOrderKonsolidasi.PerhitunganDasar);
                        }
                    }
                    else
                    {
                        if (dbItemHarga.IdJenisKendaraan != idTruck || dbItemHarga.LookupCodeSatuan.Nama.ToLower() != dbso.SalesOrderKonsolidasi.PerhitunganDasar.ToLower())
                        {
                            MsgError.Add("Tidak terdapat Daftar harga pada " + dbso.SalesOrderKonsolidasi.SONumber + " untuk satuan " + dbso.SalesOrderKonsolidasi.PerhitunganDasar);
                        }
                    }
                }
            }
        }
        public string FindDetailSo(int id)
        {
            Context.SalesOrder item = RepoSalesOrder.FindByProsesKonsolidasi(id);
            List<SalesOrderKonsolidasi> listModel = new List<SalesOrderKonsolidasi>();
            foreach (Context.SalesOrderProsesKonsolidasiItem Detail in item.SalesOrderProsesKonsolidasi.SalesOrderProsesKonsolidasiItem)
            {
                listModel.Add(new SalesOrderKonsolidasi(Detail.SalesOrderKonsolidasi));
            }

            return new JavaScriptSerializer().Serialize(new { total = listModel.Count(), data = listModel });
        }
        [MyAuthorize(Menu = "List Order", Action="update")]
        public ActionResult InputDp(int id)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            SalesOrderProsesKonsolidasi model = new SalesOrderProsesKonsolidasi(dbitem);

            ViewBag.name = model.SONumber;
            ViewBag.kondisi = "inputdp";
            return View("SalesOrderProsesKonsolidasi/FormReadOnly", model);
        }
    }
}