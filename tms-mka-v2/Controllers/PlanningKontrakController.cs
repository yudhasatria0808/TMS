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
    public class PlanningKontrakController : BaseController
    {
        private ISalesOrderRepo RepoSalesOrder;
        private ISalesOrderKontrakListSoRepo RepoSalesOrderKontrakListSo;
        public PlanningKontrakController(IUserReferenceRepo repoBase, ILookupCodeRepo repoLookup, ISalesOrderRepo repoSalesOrder, ISalesOrderKontrakListSoRepo repoSalesOrderKontrakListSo)
            : base(repoBase, repoLookup)
        {
            RepoSalesOrder = repoSalesOrder;
            RepoSalesOrderKontrakListSo = repoSalesOrderKontrakListSo;
        }
        [MyAuthorize(Menu = "Planning Kontrak", Action="read")]
        public ActionResult Index()
        {
            ViewBag.listKolom = ListKolom.Where(d => d.Action == "Index" && d.Controller == "PlanningKontrak").ToList();
            return View();
        }
        public string Binding()
        {
            GridRequestParameters param = GridRequestParameters.Current;

            List<Context.SalesOrder> items = RepoSalesOrder.FindAllKontrak().Where(d => d.Status == "save" || d.Status == "draft planning").ToList();
            List<SalesOrderKontrak> ListModel = new List<SalesOrderKontrak>();
            foreach (Context.SalesOrder item in items)
            {
                ListModel.Add(new SalesOrderKontrak(item));
            }
            List<Context.SalesOrderKontrakListSo> itemsListSoKontrakBatalTruk = RepoSalesOrderKontrakListSo.returnListSoBatalTruckOnly();
            foreach (Context.SalesOrderKontrakListSo itemBatalTruk in itemsListSoKontrakBatalTruk)
            {
                ListModel.Add(new SalesOrderKontrak(RepoSalesOrder.FindByKontrak(itemBatalTruk.SalesKontrakId.Value), itemBatalTruk));
            }

            int total = RepoSalesOrder.CountKontrak(param.Filters);

            return new JavaScriptSerializer().Serialize(new { total = ListModel.Count(), data = ListModel });
        }
        [MyAuthorize(Menu = "Planning Kontrak", Action="update")]
        public ActionResult Edit(int id)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            SalesOrderKontrak model = new SalesOrderKontrak(dbitem);

            ViewBag.kondisi = "planning";
            ViewBag.name = model.SONumber;
            ViewBag.Title = "Planning Sales Order Kontrak " + model.SONumber;
            ViewBag.PostData = "Edit";

            return View("SalesOrderKontrak/FormReadOnly", model);
        }
        [HttpPost]
        public ActionResult Edit(SalesOrderKontrak model, string btnsave)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(model.SalesOrderId.Value);
            if (btnsave != null && btnsave != "")
            {
                dbitem.Status = btnsave;
            }
            else
            {
                dbitem.Status = model.Status;
            }
            
            model.setDbOpertional(dbitem.SalesOrderKontrak);

            #region list kontrak SO
            dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Clear();

            int idx = 1;
            for (int j = 1; j <= (dbitem.SalesOrderKontrak.Rit); j++)
            {
                foreach (var dateItem in dbitem.SalesOrderKontrak.SalesOrderKontrakDetail)
                {
                    int trukIdx = 1;
                    foreach (var trukItem in dbitem.SalesOrderKontrak.SalesOrderKontrakTruck)
                    {
                        Context.SalesOrderKontrakListSo dblist = new Context.SalesOrderKontrakListSo();
                        dblist.SalesKontrakId = dbitem.SalesOrderKontrakId;
                        dblist.NoSo = RepoSalesOrderKontrakListSo.generateCodeListSo(dbitem.SalesOrderKontrak.SONumber, dateItem.MuatDate, j, trukIdx, dbitem.SalesOrderKontrak.Urutan);
                        dblist.MuatDate = dateItem.MuatDate;
                        dblist.IdDataTruck = trukItem.DataTruckId;
                        dblist.Driver1Id = trukItem.IdDriver1;
                        dblist.Driver2Id = trukItem.IdDriver2;
                        RepoSalesOrderKontrakListSo.OnlyAdd(dblist);
                        idx++;
                        trukIdx++;
                    }
                }
            }
            #endregion list kontrak SO

            RepoSalesOrder.save(dbitem);
            return RedirectToAction("Index");
        }

        [MyAuthorize(Menu = "Planning Kontrak", Action="update")]
        public ActionResult EditBatalTruk(string id)
        {
            Context.SalesOrderKontrakListSo detailsoitem = RepoSalesOrderKontrakListSo.FindByNoSo(id);
            Context.SalesOrder kontrakitem = RepoSalesOrder.FindByKontrak(detailsoitem.SalesKontrakId.Value);
            SalesOrderKontrak model = new SalesOrderKontrak(kontrakitem, detailsoitem);

            ViewBag.kondisi = "planning";
            ViewBag.name = model.SONumber;
            ViewBag.so_detail = detailsoitem;
            ViewBag.Title = "Planning Sales Order Kontrak " + model.SONumber;
            ViewBag.PostData = "EditBatalTruk";

            return View("PlanningAfterBatalTruk", model);
        }

        [HttpPost]
        public ActionResult EditBatalTruk(SalesOrderKontrak model, int DataTruckId, int IdDriver1, int IdDriver2, string btnsave)
        {
            Context.SalesOrderKontrakListSo dblist = RepoSalesOrderKontrakListSo.FindByNoSo(model.SONumber);
            dblist.IdDataTruck = DataTruckId;
            dblist.Driver1Id = IdDriver1;
            if (IdDriver2 > 0)
                dblist.Driver2Id = IdDriver2;
            if (IdDriver2 > 0)
                dblist.Driver2Id = IdDriver2;
            if (btnsave != null && btnsave != "")
            {
                dblist.Status = "draft konfirmasi";
            }
            else
            {
                dblist.Status = "save planning";
            }
            dblist.Status = btnsave;
            model.Status = "draft konfirmasi";
            //RepoSalesOrder.save(model);
            RepoSalesOrderKontrakListSo.OnlyUpdate(dblist);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult Submit(int id)
        {
            ResponeModel response = new ResponeModel(true);
            Context.SalesOrder dbItem = RepoSalesOrder.FindByPK(id);
            if (dbItem.SalesOrderKontrak.SalesOrderKontrakTruck.Count > 0)
            {
                dbItem.Status = "save planning";
                dbItem.isReturn = false;
                
                #region list kontrak SO
                dbItem.SalesOrderKontrak.SalesOrderKontrakListSo.Clear();

                int idx = 1;
                for (int j = 1; j <= (dbItem.SalesOrderKontrak.Rit); j++)
                {
                    foreach (var dateItem in dbItem.SalesOrderKontrak.SalesOrderKontrakDetail)
                    {
                        foreach (var trukItem in dbItem.SalesOrderKontrak.SalesOrderKontrakTruck)
                        {
                            Context.SalesOrderKontrakListSo dblist = new Context.SalesOrderKontrakListSo();
                            dblist.SalesKontrakId = dbItem.SalesOrderKontrakId;
                            dblist.NoSo = RepoSalesOrderKontrakListSo.generateCodeListSo(dbItem.SalesOrderKontrak.SONumber, dateItem.MuatDate, j, idx, dbItem.SalesOrderKontrak.Urutan);
                            dblist.MuatDate = dateItem.MuatDate;
                            dblist.IdDataTruck = trukItem.DataTruckId;
                            dblist.Driver1Id = trukItem.IdDriver1;
                            dblist.Driver2Id = trukItem.IdDriver2;
                            RepoSalesOrderKontrakListSo.OnlyAdd(dblist);
                            idx++;
                        }
                    }
                }
                #endregion list kontrak SO

                RepoSalesOrder.save(dbItem);
            }
            else
            {
                response.SetFail("Truck belum dipilih.");
            }


            return Json(response);
        }
        [HttpPost]
        public JsonResult Return(int id)
        {
            ResponeModel response = new ResponeModel(true);
            Context.SalesOrder dbItem = RepoSalesOrder.FindByPK(id);
            dbItem.Status = "draft";
            dbItem.SalesOrderKontrak.SalesOrderKontrakTruck.Clear();
            dbItem.isReturn = true;
            RepoSalesOrder.save(dbItem);

            return Json(response);
        }
    }
}