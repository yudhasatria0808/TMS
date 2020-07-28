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
    public class DokumenController : BaseController 
    {
        private IDokumenRepo RepoDokumen;
        private ICustomerRepo RepoCustomer;
        private ISalesOrderRepo RepoSalesOrder;
        private Iso_mstrRepo Reposo_mstr;
        private IERPConfigRepo RepoERPConfig;
        public DokumenController(IUserReferenceRepo repoBase, ILookupCodeRepo repoLookup, IDokumenRepo repoDokumen, ICustomerRepo repoCustomer, ISalesOrderRepo repoSalesOrder, Iso_mstrRepo reposo_mstr, IERPConfigRepo repoERPConfig)
            : base(repoBase, repoLookup)
        {   
            RepoDokumen = repoDokumen;
            RepoCustomer = repoCustomer;
            RepoSalesOrder = repoSalesOrder;
            RepoERPConfig = repoERPConfig;
            Reposo_mstr = reposo_mstr;
        }
        public ActionResult Index(string caller)
        {
            ViewBag.listKolom = ListKolom.Where(d=>d.Action == "Index" && d.Controller == "Dokumen").ToList();
            ViewBag.caller = caller;
            if (caller == "admin")
                ViewBag.Title = "Dokumen Admin Surat Jalan";
            else
                ViewBag.Title = "Dokumen Billing";

            return View();
        }
        public string Binding(string caller)
        {
            List<Context.Dokumen> items = RepoDokumen.FindAll().Where(d => d.IsComplete != true).ToList();

            List<DokumenIndex> ListModel = new List<DokumenIndex>();
            if (caller == "admin")
            {
                foreach (var item in items.Where(d => d.IsAdmin))
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

        public ActionResult Edit(int id, string caller)
        {
            Context.Dokumen dbitem = RepoDokumen.FindByPK(id);
            List<int> ListIdDokumen = dbitem.DokumenItem.Select(b => b.IdBilling).ToList();
            string strQuery = "";
            //cek apakah di billing customer ada penambahan data
            //jika ada maka update data jika data dokumen belum close
            if (!dbitem.IsComplete)
            {
                Context.Customer dbcust = dbitem.Customer;
                foreach (var itemBilling in dbitem.Customer.CustomerBilling.Where( i => !ListIdDokumen.Contains(i.Id)).ToList())
                {
                    dbitem.DokumenItem.Add(new Context.DokumenItem()
                    {
                        IdBilling = itemBilling.Id,
                        CustomerId = itemBilling.CustomerId,
                        ModifiedDate = DateTime.Now,
                    });
                    strQuery += "INSERT INTO dbo.\"DokumenItem\" (\"IdBilling\", \"CustomerId\", \"ModifiedDate\") VALUES (" + itemBilling.Id + ", " + itemBilling.CustomerId + ", " + DateTime.Now + ");";
                }
                RepoDokumen.save(dbitem, UserPrincipal.id, strQuery);
            }

            if (dbitem.SalesOrder.SalesOrderOncallId.HasValue)
                ViewBag.TanggalPulang = dbitem.SalesOrder.SalesOrderOncall.TanggalMuat.Value.AddDays(RepoSalesOrder.FindRute(dbitem.SalesOrder.SalesOrderOncall.IdDaftarHargaItem.Value).WaktuKerja);
            Dokumen model = new Dokumen(dbitem);
            ViewBag.caller = caller;
            if (caller == "admin")
                ViewBag.Title = "Dokumen Admin Surat Jalan";
            else
                ViewBag.Title = "Dokumen Billing";

            return View("Form", model);
        }
        [HttpPost]
        public ActionResult Edit(Dokumen model, string btnSubmit,string caller)
        {
            Context.Dokumen dbitem = RepoDokumen.FindByPK(model.Id);
            DokumenItem[] resDok = JsonConvert.DeserializeObject<DokumenItem[]>(model.strDokumen);
            string strQuery = "";
            foreach (var item in resDok.Where(d => d.IsEdit))
            {
                item.SetDb(dbitem.DokumenItem.Where(d => d.Id == item.Id).FirstOrDefault());
                dbitem.DokumenItemHistory.Add(item.SetDbHistory(new Context.DokumenItemHistory()));
                strQuery += "INSERT INTO dbo.\"DokumenItemHistory\" (\"IdDok\", \"Nama\", \"Jml\", \"Warna\", \"Stempel\", \"Lengkap\", \"KeteranganAdmin\", \"KeteranganBilling\", \"ModifiedDate\") VALUES (" + dbitem.Id +
                    ", " + item.Nama + ", " + item.Jml + ", " + item.Warna + ", " + item.Stempel + ", " + item.Lengkap + ", " + item.KeteranganAdmin + ", " + item.KeteranganBilling + ", " + dbitem.ModifiedDate + ");";
            }

            if (btnSubmit == "Kirim")
                dbitem.IsAdmin = false;
            else if (btnSubmit == "Submit")
            {
                dbitem.IsAdmin = true;
                if (caller != "Admin")
                    dbitem.IsReturn = true;
            }
            else if (btnSubmit == "Terima")
            {
                //create soship
                string sod_oid = Guid.NewGuid().ToString();
                Context.SalesOrder dbso = dbitem.SalesOrder;
                string code = (dbso.SalesOrderOncallId.HasValue ? dbso.SalesOrderOncall.SONumber : dbso.SalesOrderProsesKonsolidasiId.HasValue ? dbso.SalesOrderProsesKonsolidasi.SONumber : dbso.SalesOrderPickupId.HasValue ? dbso.SalesOrderPickup.SONumber : dbso.SalesOrderKontrak.SONumber);
                string ship_guid = Guid.NewGuid().ToString();
                string guid;
                string sod_guid;
                dbitem.IsComplete = true;
                SyncToERP(dbso, sod_oid);
                guid = Reposo_mstr.FindByPK(code).so_oid;
                sod_guid = Reposo_mstr.FindSoDet(code).so_oid;
                Reposo_mstr.saveSoShipMstr(dbso, UserPrincipal.username, guid, ship_guid);
                Reposo_mstr.saveSoShipDet(dbso, UserPrincipal.username, ship_guid, sod_oid);
            }

            RepoDokumen.save(dbitem, UserPrincipal.id, strQuery);
            return RedirectToAction("Index", new { caller = caller });
        }

        public void SyncToERP(Context.SalesOrder dbso, string sod_guid)
        {
            decimal harga = RepoSalesOrder.Harga(dbso);
            Context.SalesOrderOncall dbitem = dbso.SalesOrderOncall;
            string guid = Guid.NewGuid().ToString();
            Reposo_mstr.saveSoMstr(dbso, UserPrincipal.username, guid, dbitem.CustomerId.Value, harga);
            Reposo_mstr.saveSoDet(dbso, UserPrincipal.username, guid, sod_guid);
        }

        public ActionResult View(int id, string caller)
        {
            Context.Dokumen dbitem = RepoDokumen.FindByPK(id);
            Dokumen model = new Dokumen(dbitem);
            ViewBag.caller = caller;
            if (caller == "admin")
                ViewBag.Title = "Dokumen Admin Surat Jalan";
            else
                ViewBag.Title = "Dokumen Billing";

            return View("View", model);
        }
      
        public ActionResult Print(int id)
        {
            return new Rotativa.ActionAsPdf("ShowPrint", new { id = id }) { FileName = "Dokumen_" + new Guid().ToString() + ".pdf", PageSize = Rotativa.Options.Size.A4 };
        }
        public ActionResult ShowPrint(int id)
        {
            Context.Dokumen dbitem = RepoDokumen.FindByPK(id);
            DokumenIndex model = new DokumenIndex(dbitem);
            ViewBag.TanggalPulang = dbitem.SalesOrder.SalesOrderOncall.TanggalMuat.Value.AddDays(RepoSalesOrder.FindRute(dbitem.SalesOrder.SalesOrderOncall.IdDaftarHargaItem.Value).WaktuKerja);

            return View("Print", model);
        }

        public FileContentResult Export()
        {
            //bikin file baru
            ExcelPackage pck = new ExcelPackage();
            //sumber data
            List<Context.Dokumen> dbitems = RepoDokumen.FindAll().Where(d => d.IsComplete != true).ToList();
            //bikin worksheet worksheet 
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet 1");

            //bikin header cell[baris,kolom] , nama kolom sesuaikan dengan template
            ws.Cells[1, 1].Value = "Status";
            ws.Cells[1, 2].Value = "SO No";
            ws.Cells[1, 3].Value = "Vehicle No";
            ws.Cells[1, 4].Value = "Customer";
            ws.Cells[1, 5].Value = "Rute";
            ws.Cells[1, 6].Value = "Nama Driver";
            ws.Cells[1, 7].Value = "Tanggal Muat";
            ws.Cells[1, 8].Value = "Delay";
            ws.Cells[1, 9].Value = "Lengkap?";
            ws.Cells[1, 10].Value = "Last Update";

            // Inserts Data
            int idx = 0;
            for (int i = 0; i < dbitems.Count(); i++)
            {
                DokumenIndex item = new DokumenIndex(dbitems[i]);
                ws.Cells[i + 2, 1].Value = item.Status;
                ws.Cells[i + 2, 2].Value = item.NoSo;
                ws.Cells[i + 2, 3].Value = item.VehicleNo;
                ws.Cells[i + 2, 4].Value = item.Customer;
                ws.Cells[i + 2, 5].Value = item.Rute;
                ws.Cells[i + 2, 6].Value = item.NamaDriver;
                ws.Cells[i + 2, 7].Value = item.TanggalMuat.ToString();
                ws.Cells[i + 2, 8].Value = item.Delay;
                ws.Cells[i + 2, 9].Value = item.Lengkap;
                ws.Cells[i + 2, 10].Value = item.LastUpdate.ToString();
            }


            var fsr = new FileContentResult(pck.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fsr.FileDownloadName = "Surat Jalan.xls";

            return fsr;
        }
    }
}