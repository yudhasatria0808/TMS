﻿using System;
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
    public class KonfirmasiKontrakController : BaseController
    {
        private ISalesOrderRepo RepoSalesOrder;
        private ISalesOrderKontrakListSoRepo RepoSalesOrderKontrakListSo;
        public KonfirmasiKontrakController(IUserReferenceRepo repoBase, ILookupCodeRepo repoLookup, ISalesOrderRepo repoSalesOrder, ISalesOrderKontrakListSoRepo repoSalesOrderKontrakListSo)
            : base(repoBase, repoLookup)
        {
            RepoSalesOrder = repoSalesOrder;
            RepoSalesOrderKontrakListSo = repoSalesOrderKontrakListSo;
        }
        [MyAuthorize(Menu = "Konfirmasi Planning kontrak", Action="read")]
        public ActionResult Index()
        {
            ViewBag.listKolom = ListKolom.Where(d => d.Action == "Index" && d.Controller == "PlanningKontrak").ToList();
            return View();
        }
        public string Binding()
        {
            GridRequestParameters param = GridRequestParameters.Current;

            List<Context.SalesOrder> items = RepoSalesOrder.FindAllKontrak().Where(d => d.Status == "save planning" || d.Status == "draft konfirmasi").ToList();
            List<SalesOrderKontrak> ListModel = new List<SalesOrderKontrak>();
            foreach (Context.SalesOrder item in items)
            {
                ListModel.Add(new SalesOrderKontrak(item));
            }

            int total = RepoSalesOrder.CountKontrak(param.Filters);

            return new JavaScriptSerializer().Serialize(new { total = total, data = ListModel });
        }
        [MyAuthorize(Menu = "Konfirmasi Planning kontrak", Action="update")]
        public ActionResult Edit(int id)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            SalesOrderKontrak model = new SalesOrderKontrak(dbitem);

            ViewBag.kondisi = "konfirmasi";
            ViewBag.name = model.SONumber;
            ViewBag.Title = "konfirmasi Sales Order Kontrak " + model.SONumber;
            ViewBag.PostData = "Edit";

            return View("SalesOrderKontrak/FormReadOnly", model);
        }
        public class ListTruckSo
        {
            public int Id { get; set; }
            public int? IdTruk { get; set; }
            public int? IdDriver1 { get; set; }
            public int? IdDriver2 { get; set; }
        }
        [HttpPost]
        public JsonResult Proses(string listSo, int idSo)
        {
            ResponeModel response = new ResponeModel(true);

            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(idSo);
            List<ListTruckSo> res = JsonConvert.DeserializeObject<ListTruckSo[]>(listSo).ToList();
            //int urutListSo = (RepoSalesOrderKontrakListSo.getUrutanProses(dbitem.SalesOrderKontrakId)) + 1;
            foreach (ListTruckSo item in res)
            {
                    Context.SalesOrderKontrakListSo dblist = RepoSalesOrderKontrakListSo.FindByPK(item.Id);
                    dblist.IdDataTruck = item.IdTruk == null ? null : item.IdTruk;
                    dblist.Driver1Id = item.IdDriver1 == null ? null : item.IdDriver1;
                    dblist.Driver2Id = item.IdDriver2 == null ? null : item.IdDriver2;
                    dblist.IsProses = true;
                    dblist.Status = "save konfirmasi";
                    try
                    {
                        RepoSalesOrderKontrakListSo.save(dblist);
                        if (dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Any(s => s.IsProses == false))
                        {
                            dbitem.Status = "draft konfirmasi";
                        }
                        else
                        {
                            dbitem.Status = "save konfirmasi";
                        }
                        RepoSalesOrder.save(dbitem);
                    }
                    catch (Exception e)
                    {
                        response.Success = false;
                        response.Message = e.Message;
                    }
            }

            var data = res.GroupBy(d => d.IdTruk).Select(grp => grp.ToList());
            foreach (var item in data)
            {
                foreach (var item2 in item)
                {
                    Context.SalesOrderKontrakListSo dblist = RepoSalesOrderKontrakListSo.FindByPK(item2.Id);
                    dblist.IdDataTruck = item2.IdTruk == null ? null : item2.IdTruk;
                    dblist.Driver1Id = item2.IdDriver1 == null ? null : item2.IdDriver1;
                    dblist.Driver2Id = item2.IdDriver2 == null ? null : item2.IdDriver2;
                    dblist.IsProses = true;
                    //dblist.Urutan = urutListSo;
                    dblist.Status = "save konfirmasi";

                    try
                    {
                        RepoSalesOrderKontrakListSo.save(dblist);
                        if (dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Any(s => s.IsProses == false))
                        {
                            dbitem.Status = "draft konfirmasi";
                        }
                        else
                        {
                            dbitem.Status = "save konfirmasi";
                        }
                        RepoSalesOrder.save(dbitem);
                    }
                    catch (Exception e)
                    {
                        response.Success = false;
                        response.Message = e.Message;
                    }
                }
                //urutListSo++;
            }
            
            return Json(response);
        }
        [HttpPost]
        public JsonResult Submit(int id)
        {
            ResponeModel response = new ResponeModel(true);
            Context.SalesOrder dbItem = RepoSalesOrder.FindByPK(id);
            dbItem.Status = "save konfirmasi";
            RepoSalesOrder.save(dbItem);

            return Json(response);
        }
        [HttpPost]
        public JsonResult Return(int id)
        {
            ResponeModel response = new ResponeModel(true);
            Context.SalesOrder dbItem = RepoSalesOrder.FindByPK(id);
            dbItem.Status = "draft planning";
            RepoSalesOrder.save(dbItem);
            RepoSalesOrderKontrakListSo.returnListSo(dbItem.SalesOrderKontrakId);

            return Json(response);
        }
    }
}