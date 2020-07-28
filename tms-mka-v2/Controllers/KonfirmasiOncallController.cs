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
    public class KonfirmasiOncallController : BaseController
    {
        private ISalesOrderRepo RepoSalesOrder;
        private IHistoryOncallRepo RepoHistoryOncall;
        private IAtmRepo RepoAtm;
        private IAdminUangJalanRepo RepoAdminUangJalan;
        private IAuditrailRepo RepoAuditrail;
        public KonfirmasiOncallController(IUserReferenceRepo repoBase, ILookupCodeRepo repoLookup, ISalesOrderRepo repoSalesOrder, IHistoryOncallRepo repoHistoryOncall, IAtmRepo repoAtm,
            IAdminUangJalanRepo repoAdminUangJalan, IAuditrailRepo repoAuditrail)
            : base(repoBase, repoLookup)
        {
            RepoSalesOrder = repoSalesOrder;
            RepoHistoryOncall = repoHistoryOncall;
            RepoAtm = repoAtm;
            RepoAdminUangJalan = repoAdminUangJalan;
            RepoAuditrail = repoAuditrail;
        }
        [MyAuthorize(Menu = "Konfirmasi Planning Oncall", Action="read")]
        public ActionResult Index()
        {
            ViewBag.listKolom = ListKolom.Where(d => d.Action == "Index" && d.Controller == "KonfirmasiOncall").ToList();
            return View();
        }
        public string Binding()
        {
            List<Context.SalesOrder> items = RepoSalesOrder.FindAll().Where(d =>
                (d.SalesOrderOncallId != null || d.SalesOrderProsesKonsolidasiId != null || d.SalesOrderPickupId != null) &&
                (d.Status == "save planning" || d.Status == "draft konfirmasi")).ToList();
            List<ListOrder> ListModel = new List<ListOrder>();
            foreach (Context.SalesOrder item in items)
            {
                ListModel.Add(new ListOrder(item));
            }

            return new JavaScriptSerializer().Serialize(new { total = ListModel.Count(), data = ListModel });
        }
        [MyAuthorize(Menu = "Konfirmasi Planning Oncall", Action="update")]
        public ActionResult Edit(int id)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            List<Context.Atm> ListAtm = RepoAtm.FindAll();
            if (dbitem.SalesOrderOncallId.HasValue)
            {
                SalesOrderOncall model = new SalesOrderOncall(dbitem);
                //ambil rekening default
                if (!model.AtmId.HasValue)
                {
                    Context.Atm dummAtm = ListAtm.Where(d => d.IdDriver == model.Driver1Id).FirstOrDefault();
                    if (dummAtm != null)
                    {
                        model.AtmId = dummAtm.Id;
                        model.StrRekening = dummAtm.NoRekening;
                        model.AtasNamaRek = dummAtm.AtasNama;
                        model.Bank = dummAtm.LookupCodeBank.Nama;
                    }
                }

                ViewBag.kondisi = "konfirmasi";
                ViewBag.name = model.SONumber;
                ViewBag.Title = "konfirmasi Sales Order Oncall " + model.SONumber;
                ViewBag.PostData = "EditOncall";
                return View("SalesOrderOncall/FormReadOnly", model);
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue)
            {
                SalesOrderProsesKonsolidasi model = new SalesOrderProsesKonsolidasi(dbitem);

                //ambil rekening default
                if (!model.AtmId.HasValue)
                {
                    Context.Atm dummAtm = ListAtm.Where(d => d.IdDriver == model.Driver1Id).FirstOrDefault();
                    if (dummAtm != null)
                    {
                        model.AtmId = dummAtm.Id;
                        model.StrRekening = dummAtm.NoRekening;
                        model.AtasNamaRek = dummAtm.AtasNama;
                        model.Bank = dummAtm.LookupCodeBank.Nama;
                    }
                }

                ViewBag.kondisi = "konfirmasi";
                ViewBag.name = model.SONumber;
                ViewBag.Title = "konfirmasi Sales Proses Konsolidasi " + model.SONumber;
                ViewBag.PostData = "EditKonsolidasi";
                return View("SalesOrderProsesKonsolidasi/FormReadOnly", model);
            }
            else if (dbitem.SalesOrderPickupId.HasValue)
            {
                SalesOrderPickup model = new SalesOrderPickup(dbitem);

                //ambil rekening default
                if (!model.AtmId.HasValue)
                {
                    Context.Atm dummAtm = ListAtm.Where(d => d.IdDriver == model.Driver1Id).FirstOrDefault();
                    if (dummAtm != null)
                    {
                        model.AtmId = dummAtm.Id;
                        model.StrRekening = dummAtm.NoRekening;
                        model.AtasNamaRek = dummAtm.AtasNama;
                        model.Bank = dummAtm.LookupCodeBank.Nama;
                    }
                }

                ViewBag.kondisi = "konfirmasi";
                ViewBag.name = model.SONumber;
                ViewBag.Title = "konfirmasi Sales Order Pickup " + model.SONumber;
                ViewBag.PostData = "EditPickup";
                return View("SalesOrderPickup/FormReadOnly", model);
            }
            else
            {
                return View("");
            }
        }
        [HttpPost]
        public ActionResult EditOncall(SalesOrderOncall model, string btnsave)
        {
            //validasi manual
            bool palid = true;
            if (btnsave != "draft planning")
            {
                if (!model.IdDataTruck.HasValue)
                {
                    ModelState.AddModelError("IdDataTruck", "Truk harus diisi.");
                    palid = false;
                }
                if (!model.Driver1Id.HasValue)
                {
                    ModelState.AddModelError("Driver1Id", "Driver harus diisi.");
                    palid = false;
                }
            }
            if (!model.AtmId.HasValue || model.AtmId.Value == 0)
            {
                ModelState.AddModelError("AtmId", "Rekening harus diisi.");
                palid = false;
            }
            if (palid)
            {
                Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(model.SalesOrderId.Value);
                string act = "";
                if (btnsave != null && btnsave != "")
                {
                    act = "Draft";
                    dbitem.Status = btnsave;
                }
                else
                {
                    if (model.Status.ToLower() == "save")
                    {
                        act = "Return";
                        dbitem.isReturn = true;
                    }
                    else
                    {
                        act = "Submit";
                        dbitem.isReturn = false;
                    }

                    dbitem.Status = model.Status;
                }

                model.setDbOperasional(dbitem.SalesOrderOncall, act, "Operational");
                dbitem.UpdatedBy = UserPrincipal.id;
                RepoSalesOrder.save(dbitem);
                RepoAuditrail.saveKonfirmasiHistory(dbitem);
                if (dbitem.AdminUangJalanId.HasValue){
                    Context.AdminUangJalan dbauj = RepoAdminUangJalan.FindByPK(dbitem.AdminUangJalanId.Value);
                    dbauj.IdDriver1 = dbitem.SalesOrderOncall.Driver1Id;
                    RepoAdminUangJalan.save(dbauj);
                }
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.kondisi = "konfirmasi";
                ViewBag.name = model.SONumber;
                ViewBag.Title = "Konfirmsai Sales Order Oncall " + model.SONumber;
                ViewBag.PostData = "EditOncall";
                return View("SalesOrderOncall/FormReadOnly", model);
            }
        }
        [HttpPost]
        public ActionResult EditPickup(SalesOrderPickup model, string btnsave)
        {
            //validasi manual
            bool palid = true;
            if (btnsave != "draft planning")
            {
                if (!model.IdDataTruck.HasValue)
                {
                    ModelState.AddModelError("IdDataTruck", "Truk harus diisi.");
                    palid = false;
                }
                if (!model.Driver1Id.HasValue)
                {
                    ModelState.AddModelError("Driver1Id", "Driver harus diisi.");
                    palid = false;
                }
            }
            if (!model.AtmId.HasValue || model.AtmId.Value == 0)
            {
                ModelState.AddModelError("AtmId", "Rekening harus diisi.");
                palid = false;
            }
            if (palid)
            {
                Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(model.SalesOrderId.Value);
                string act = "";
                if (btnsave != null && btnsave != "")
                {
                    act = "Draft";
                    dbitem.Status = btnsave;
                }
                else
                {
                    if (model.Status.ToLower() == "save")
                    {
                        act = "Return";
                        dbitem.isReturn = true;
                    }
                    else
                    {
                        act = "Submit";                    
                        dbitem.isReturn = false;
                    }

                    dbitem.Status = model.Status;
                }

                model.setDbOperasional(dbitem.SalesOrderPickup, act, "Operational");
                RepoSalesOrder.save(dbitem);
                var query = "UPDATE dbo.\"SalesOrderPickup\" SET \"SONumber\" = " + dbitem.SalesOrderPickup.SONumber + ", \"Urutan\" = " + dbitem.SalesOrderPickup.Urutan + ", TanggalOrder\" = " +
                    dbitem.SalesOrderPickup.TanggalOrder + ", \"CustomerId\" = " + dbitem.SalesOrderPickup.CustomerId + ", \"JenisTruckId\" = " + dbitem.SalesOrderPickup.JenisTruckId + ", \"ProductId\" = " +
                     dbitem.SalesOrderPickup.ProductId + ", \"TanggalPickup\" = " + dbitem.SalesOrderPickup.TanggalPickup + ", \"JamPickup\" = " + dbitem.SalesOrderPickup.JamPickup + ", \"IsSelect\" = " +
                      dbitem.SalesOrderPickup.IsSelect + ", \"Keterangan\" = " + dbitem.SalesOrderPickup.Keterangan + ", \"KeteranganLoading\" = " + dbitem.SalesOrderPickup.KeteranganLoading + ", \"KeteranganUnloading\" = " +
                       dbitem.SalesOrderPickup.KeteranganUnloading + ", \"JamOrder\" = " + dbitem.SalesOrderPickup.JamOrder + ", \"StrMultidrop\" = " + dbitem.SalesOrderPickup.StrMultidrop + ", \"RuteId\" = " +
                       dbitem.SalesOrderPickup.RuteId + ", \"IdDataTruck\" = " + dbitem.SalesOrderPickup.IdDataTruck + ", \"KeteranganDataTruck\" = " + dbitem.SalesOrderPickup.KeteranganDataTruck + ", \"Driver1Id\" = " +
                       dbitem.SalesOrderPickup.Driver1Id + ", \"KeteranganDriver1\" = " + dbitem.SalesOrderPickup.KeteranganDriver1 + ", \"Driver2Id\" = " + dbitem.SalesOrderPickup.Driver2Id + ", \"KeteranganDriver2\" = " +
                        dbitem.SalesOrderPickup.KeteranganDriver2 + ", \"IsCash\" = " + dbitem.SalesOrderPickup.IsCash + ", \"IdDriverTitip\" = " + dbitem.SalesOrderPickup.IdDriverTitip + ", \"KeteranganRek\" = " +
                         dbitem.SalesOrderPickup.KeteranganRek + ", \"AtmId\" = " + dbitem.SalesOrderPickup.AtmId + " WHERE \"SalesOrderPickupId\" = " + dbitem.SalesOrderPickupId +
                         ";INSERT INTO dbo.\"SalesOrderPickupComment\" (\"SalesOrderPickupId\", \"Tanggal\", \"CommentUser\", \"Action\", \"Username\") VALUES (" + dbitem.SalesOrderPickupId + ", " + DateTime.Now + ", " +
                    model.CommentUser + ", " + act + ", " + UserPrincipal.username + ");";
                RepoAuditrail.SetAuditTrail(query, "Konfirmasi Pickup", "Edit", UserPrincipal.id);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.kondisi = "konfirmasi";
                ViewBag.name = model.SONumber;
                ViewBag.Title = "Konfirmsai Sales Order Pickup " + model.SONumber;
                ViewBag.PostData = "EditPickup";
                return View("SalesOrderPickup/FormReadOnly", model);
            }
        }
        [HttpPost]
        public ActionResult EditKonsolidasi(SalesOrderProsesKonsolidasi model, string btnsave)
        {
            //validasi manual
            bool palid = true;
            if (btnsave != "draft planning")
            {
                if (!model.IdDataTruck.HasValue)
                {
                    ModelState.AddModelError("IdDataTruck", "Truk harus diisi.");
                    palid = false;
                }
                if (!model.Driver1Id.HasValue)
                {
                    ModelState.AddModelError("Driver1Id", "Driver harus diisi.");
                    palid = false;
                }
            }
            if (!model.AtmId.HasValue || model.AtmId.Value == 0)
            {
                ModelState.AddModelError("AtmId", "Rekening harus diisi.");
                palid = false;
            }
            if (palid)
            {
                Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(model.SalesOrderId.Value);
                string act = "";
                if (btnsave != null && btnsave != "")
                {
                    act = "Draft";
                    dbitem.Status = btnsave;
                }
                else
                {
                    if (model.Status.ToLower() == "save")
                    {
                        act = "Return";
                        dbitem.isReturn = true;
                    }
                    else
                    {
                        dbitem.isReturn = false;
                        act = "Submit";
                    }

                    dbitem.Status = model.Status;
                }

                model.setDbOperasional(dbitem.SalesOrderProsesKonsolidasi, act, "Operational");
                RepoSalesOrder.save(dbitem);
                RepoAuditrail.saveUpdSalesOrderProsesKonsolidasiQuery(dbitem.SalesOrderProsesKonsolidasi, UserPrincipal.id);
                RepoAuditrail.saveSalesOrderProsesKonsolidasiCommentQuery(dbitem.SalesOrderProsesKonsolidasi, UserPrincipal.id);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.kondisi = "konfirmasi";
                ViewBag.name = model.SONumber;
                ViewBag.Title = "Konfirmsai Sales Order Oncall " + model.SONumber;
                ViewBag.PostData = "EditKonsolidasi";
                return View("SalesOrderProsesKonsolidasi/FormReadOnly", model);
            }
        }
        [HttpPost]
        public JsonResult Submit(int id)
        {
            ResponeModel response = new ResponeModel(true);
            Context.SalesOrder dbItem = RepoSalesOrder.FindByPK(id);
            dbItem.Status = "save konfirmasi";
            dbItem.isReturn = false;
            dbItem.UpdatedBy = UserPrincipal.id;
            RepoSalesOrder.save(dbItem);
            RepoAuditrail.saveKonfirmasiHistory(dbItem);

            return Json(response);
        }
        [HttpPost]
        public JsonResult Return(int id)
        {
            ResponeModel response = new ResponeModel(true);
            Context.SalesOrder dbItem = RepoSalesOrder.FindByPK(id);
            dbItem.Status = "draft planning";
            dbItem.isReturn = true;
            RepoSalesOrder.save(dbItem);

            return Json(response);
        }
    }
}