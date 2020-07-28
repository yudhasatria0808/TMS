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
    public class ListOrderController : BaseController
    {
        #region initial condition

        private ISalesOrderRepo RepoSalesOrder;
        public ListOrderController(IUserReferenceRepo repoBase, ILookupCodeRepo repoLookup, ISalesOrderRepo repoSalesOrder)
            : base(repoBase, repoLookup)
        {
            RepoSalesOrder = repoSalesOrder;
        }
        [MyAuthorize(Menu = "List Order", Action="read")]
        public ActionResult Index()
        {
            ViewBag.listKolom = ListKolom.Where(d => d.Action == "Index" && d.Controller == "ListOrder").ToList();
            return View();
        }

        #endregion initial condition

        #region initial value for grid main

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

        #endregion initial value for grid main

        #region partial views

        [MyAuthorize(Menu = "List Order", Action="update")]
        public ActionResult InputDp(int id)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            if (dbitem.SalesOrderOncallId != null)
            {
                return RedirectToAction("InputDp", "SalesOrderOncall", new { id = dbitem.Id, status = dbitem.Status });
            }
            else if (dbitem.SalesOrderKontrakId != null)
            {
                return RedirectToAction("InputDp", "SalesOrderKontrak", new { id = dbitem.Id, status = dbitem.Status });
            }
            else if (dbitem.SalesOrderPickupId != null)
            {
                return RedirectToAction("InputDp", "SalesOrderPickup", new { id = dbitem.Id, status = dbitem.Status });
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId != null)
            {
                return RedirectToAction("InputDp", "SalesOrderProsesKonsolidasi", new { id = dbitem.Id, status = dbitem.Status });
            }
            else
                return RedirectToAction("", "");
        }

        [MyAuthorize(Menu = "List Order", Action="update")]
        public ActionResult BatalOrder(int id)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            if (dbitem.SalesOrderOncallId != null)
            {
                return RedirectToAction("BatalOrderOnCall", "BatalOrder", new { id = dbitem.Id });
            }
            else if (dbitem.SalesOrderPickupId != null)
            {
                return RedirectToAction("BatalOrderPickup", "BatalOrder", new { id = dbitem.SalesOrderPickupId });
            }
            //else if (dbitem.SalesOrderPickupId != null)
            //{
            //    return RedirectToAction("InputDp", "SalesOrderPickup", new { id = dbitem.SalesOrderPickupId });
            //}
            //else if (dbitem.SalesOrderProsesKonsolidasiId != null)
            //{
            //    return RedirectToAction("InputDp", "SalesOrderProsesKonsolidasi", new { id = dbitem.SalesOrderProsesKonsolidasiId });
            //}
            else
                return RedirectToAction("", "");
        }

        [MyAuthorize(Menu = "List Order", Action="update")]
        public ActionResult RevisiTanggal(int id)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            if (dbitem.SalesOrderOncallId != null)
            {
                return RedirectToAction("RevisiTanggalOnCall", "RevisiTanggal", new { id = dbitem.Id });
            }
            else if (dbitem.SalesOrderPickupId != null)
            {
                return RedirectToAction("RevisiTanggalPickup", "RevisiTanggal", new { id = dbitem.Id });
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId != null)
            {
                return RedirectToAction("RevisiTanggalKonsolidasi", "RevisiTanggal", new { id = dbitem.Id });
            }
            //else if (dbitem.SalesOrderProsesKonsolidasiId != null)
            //{
            //    return RedirectToAction("InputDp", "SalesOrderProsesKonsolidasi", new { id = dbitem.SalesOrderProsesKonsolidasiId });
            //}
            else
                return RedirectToAction("", "");
        }

        [MyAuthorize(Menu = "List Order", Action="update")]
        public ActionResult RevisiJenisTruk(int id)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            if (dbitem.SalesOrderOncallId != null)
            {
                return RedirectToAction("RevisiKapasitasOnCall", "RevisiJenisTruk", new { id = dbitem.Id });
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId != null)
            {
                return RedirectToAction("RevisiKapasitasKonsolidasi", "RevisiJenisTruk", new { id = dbitem.Id });
            }
            else if (dbitem.SalesOrderPickupId != null)
            {
                return RedirectToAction("RevisiKapasitasPickup", "RevisiJenisTruk", new { id = dbitem.Id });
            }
            //else if (dbitem.SalesOrderProsesKonsolidasiId != null)
            //{
            //    return RedirectToAction("InputDp", "SalesOrderProsesKonsolidasi", new { id = dbitem.SalesOrderProsesKonsolidasiId });
            //}
            else
                return RedirectToAction("", "");
        }
        
        [MyAuthorize(Menu = "List Order", Action="update")]
        public ActionResult RevisiRute(int id)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            if (dbitem.SalesOrderOncallId != null)
            {
                return RedirectToAction("RevisiRuteOnCall", "RevisiRute", new { id = dbitem.Id });
            }
            else if (dbitem.SalesOrderPickupId != null)
            {
                return RedirectToAction("RevisiRutePickup", "RevisiRute", new { id = dbitem.Id });
            }
            else if (dbitem.SalesOrderKonsolidasiId != null)
            {
                return RedirectToAction("RevisiRutekonsolidasi", "RevisiRute", new { id = dbitem.Id });
            }
            //else if (dbitem.SalesOrderProsesKonsolidasiId != null)
            //{
            //    return RedirectToAction("InputDp", "SalesOrderProsesKonsolidasi", new { id = dbitem.SalesOrderProsesKonsolidasiId });
            //}
            else
                return RedirectToAction("", "");
        }

        [MyAuthorize(Menu = "List Order", Action="read")]
        public ActionResult View(int id)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);

            if (dbitem.SalesOrderOncallId != null){
                SalesOrderOncall model = new SalesOrderOncall(dbitem);
                ViewBag.name = model.SONumber;
                return View("SalesOrderOncall/FormReadOnly", model);
            }
            else if (dbitem.SalesOrderPickupId != null){
                SalesOrderPickup model = new SalesOrderPickup(dbitem);
                ViewBag.name = model.SONumber;
                return View("SalesOrderPickup/FormReadOnly", model);
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId != null){
                SalesOrderProsesKonsolidasi model = new SalesOrderProsesKonsolidasi(dbitem);
                ViewBag.name = model.SONumber;
                return View("SalesOrderProsesKonsolidasi/FormReadOnly", model);
            }
            else if (dbitem.SalesOrderKontrakId != null){
                SalesOrderKontrak model = new SalesOrderKontrak(dbitem);
                ViewBag.name = model.SONumber;
                return View("SalesOrderKontrak/FormReadOnly", model);
            }
            return RedirectToAction("", "");
        }
        #endregion partial views
    }
}