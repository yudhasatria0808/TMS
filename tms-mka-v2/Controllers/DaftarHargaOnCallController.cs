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
using MoreLinq;

namespace tms_mka_v2.Controllers
{
    public class DaftarHargaOnCallController : BaseController
    {
        private IDaftarHargaOnCallRepo RepoDHO;
        private IRuteRepo RepoRute;
        private ISalesOrderRepo RepoSO;
        public DaftarHargaOnCallController(IUserReferenceRepo repoBase, ILookupCodeRepo repoLookup, IDaftarHargaOnCallRepo repoDHO,
            IRuteRepo repoRute, ISalesOrderRepo repoSO)
            : base(repoBase, repoLookup)
        {
            RepoDHO = repoDHO;
            RepoRute = repoRute;
            RepoSO = repoSO;
        }

        [MyAuthorize(Menu = "Daftar Harga Oncall", Action="read")]
        public ActionResult Index()
        {
            ViewBag.listKolom = ListKolom.Where(d => d.Action == "Index" && d.Controller == "DaftarHargaOnCall").ToList();
            return View();
        }
        public string Binding()
        {
            GridRequestParameters param = GridRequestParameters.Current;

            List<Context.DaftarHargaOnCall> items = RepoDHO.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);

            List<DaftarHargaOnCall> ListModel = new List<DaftarHargaOnCall>();
            foreach (Context.DaftarHargaOnCall item in items)
            {
                ListModel.Add(new DaftarHargaOnCall(item));
            }

            int total = RepoDHO.Count(param.Filters);

            return new JavaScriptSerializer().Serialize(new { total = total, data = ListModel });
        }
        [MyAuthorize(Menu = "Daftar Harga Oncall", Action="create")]
        public ActionResult Add()
        {
            DaftarHargaOnCall model = new DaftarHargaOnCall();
            return View("Form", model);
        }
        [HttpPost]
        public ActionResult Add(DaftarHargaOnCall model)
        {
            //validasi kondisi
            int idx = 0;
            foreach (DaftarHargaKondisi item in model.listKondisi.Where(d => d.IsDelete == false))
            {
                if (item.kondisi != "Biaya Multidrop" && item.IsDefault == true && item.IsInclude == true && item.IsBill == false && item.value == null)
                {
                    ModelState.AddModelError("listKondisi[" + idx + "].value", "Nilai harus diisi.");
                }
                if (item.kondisi == "Biaya Multidrop" && item.IsKota == true && item.ValKota == null)
                {
                    ModelState.AddModelError("listKondisi[" + idx + "].ValKota", "Nilai harus diisi.");
                }
                if (item.kondisi == "Biaya Multidrop" && item.IsTitik == true && item.ValTitik == null)
                {
                    ModelState.AddModelError("listKondisi[" + idx + "].ValTitik", "Nilai harus diisi.");
                }
                if (item.IsDefault == false)
                {
                    if (item.kondisi == "" || item.kondisi == null)
                        ModelState.AddModelError("listKondisi[" + idx + "].kondisi", "Nama kondisi harus diisi.");
                    if (item.IsInclude == true && item.IsBill == false && item.value == null)
                        ModelState.AddModelError("listKondisi[" + idx + "].value", "Nilai harus diisi.");
                }

                idx++;
            }

            DaftarHargaOnCallItem[] result = JsonConvert.DeserializeObject<DaftarHargaOnCallItem[]>(model.StrItem);
            model.listItem = result.ToList();

            DaftarHargaOnCallAttachment[] resultAtt = JsonConvert.DeserializeObject<DaftarHargaOnCallAttachment[]>(model.StrAttachment);
            model.listAtt = resultAtt.ToList();

            if (ModelState.IsValid)
            {
                bool palid = true;
                if (RepoDHO.IsPeriodValid(model.PeriodStart.Value, model.PeriodEnd.Value, model.IdCust.Value))
                {
                    ModelState.AddModelError("PeriodStart", "Periode awal tidak boleh overlaping.");
                    ModelState.AddModelError("PeriodEnd", "Periode akhir tidak boleh overlaping.");
                    palid = false;
                }
                if (!palid)
                    return View("Form", model);

                Context.DaftarHargaOnCall dbitem = new Context.DaftarHargaOnCall();
                model.setDb(dbitem);
                RepoDHO.save(dbitem);

                return RedirectToAction("Index");
            }

            return View("Form", model);
        }
        [MyAuthorize(Menu = "Daftar Harga Oncall", Action="update")]
        public ActionResult Edit(int id)
        {
            Context.DaftarHargaOnCall dbitem = RepoDHO.FindByPK(id);
            DaftarHargaOnCall model = new DaftarHargaOnCall(dbitem);
            ViewBag.name = model.NamaCustomer;
            return View("Form", model);
        }
        [HttpPost]
        public ActionResult Edit(DaftarHargaOnCall model)
        {
            //validasi kondisi
            int idx = 0;
            foreach (DaftarHargaKondisi item in model.listKondisi.Where(d => d.IsDelete == false))
            {
                if (item.kondisi != "Biaya Multidrop" && item.IsDefault == true && item.IsInclude == true && item.IsBill == false && item.value == null)
                {
                    ModelState.AddModelError("listKondisi[" + idx + "].value", "Nilai harus diisi.");
                }
                if (item.kondisi == "Biaya Multidrop" && item.IsKota == true && item.ValKota == null)
                {
                    ModelState.AddModelError("listKondisi[" + idx + "].ValKota", "Nilai harus diisi.");
                }
                if (item.kondisi == "Biaya Multidrop" && item.IsTitik == true && item.ValTitik == null)
                {
                    ModelState.AddModelError("listKondisi[" + idx + "].ValTitik", "Nilai harus diisi.");
                }
                if (item.IsDefault == false)
                {
                    if (item.kondisi == "" || item.kondisi == null)
                        ModelState.AddModelError("listKondisi[" + idx + "].kondisi", "Nama kondisi harus diisi.");
                    if (item.IsInclude == true && item.IsBill == false && item.value == null)
                        ModelState.AddModelError("listKondisi[" + idx + "].value", "Nilai harus diisi.");
                }

                idx++;
            }

            if (RepoDHO.IsPeriodValid(model.PeriodStart.Value, model.PeriodEnd.Value, model.IdCust.Value, model.Id))
            {
                ModelState.AddModelError("PeriodStart", "Periode awal tidak boleh overlaping.");
                ModelState.AddModelError("PeriodEnd", "Periode akhir tidak boleh overlaping.");
            }

            //if (RepoDHO.IsUsedPrices(model.Id))
            //{
            //    //ModelState.AddModelError("PeriodStart", "Periode awal tidak boleh overlaping.");
            //    ViewBag.errorMsg = "Daftar Harga sudah digunakan, Proses tidak dapat dilanjutkan";
            //    return View("Form", model);                
            //}

            if (ModelState.IsValid)
            {
                Context.DaftarHargaOnCall dbitem = RepoDHO.FindByPK(model.Id);
                model.setDb(dbitem);
                RepoDHO.save(dbitem);

                return RedirectToAction("Index");
            }
            ViewBag.name = model.NamaCustomer;

            DaftarHargaOnCallItem[] result = JsonConvert.DeserializeObject<DaftarHargaOnCallItem[]>(model.StrItem);
            model.listItem = result.ToList();

            DaftarHargaOnCallAttachment[] resultAtt = JsonConvert.DeserializeObject<DaftarHargaOnCallAttachment[]>(model.StrAttachment);
            model.listAtt = resultAtt.ToList();

            return View("Form", model);
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            bool isExistData = false;
            ResponeModel response = new ResponeModel(true);
            Context.DaftarHargaOnCall dbItem = RepoDHO.FindByPK(id);
            List<int> listId = dbItem.DaftarHargaOnCallItem.Where(i => i.IdDaftarHargaOnCall == dbItem.Id).Select(i => i.Id).ToList();
            isExistData = RepoSO.FindAllOnCall().Where(a => listId.Contains(a.SalesOrderOncall.IdDaftarHargaItem.Value)).Count() > 0;

            if (!isExistData)
                RepoDHO.delete(dbItem);
            else
            {
                response.Success = false;
                response.Message = "Data sudah digunakan, Penghapusan gagal";
            }

            return Json(response);
        }

        #region option

        public string GetRuteByCustomer(int idCust, DateTime tanggalMuat)
        {
            GridRequestParameters param = GridRequestParameters.Current;
            if (param.Filters.Filters == null)
                param.Filters.Filters = new List<tms_mka_v2.Infrastructure.FilterInfo>();
            param.Filters.Filters.Add(new tms_mka_v2.Infrastructure.FilterInfo
            {
                Field = "IdCust",
                Operator = "eq",
                Value = idCust.ToString(),
            });

            List<Rute> listRute = new List<Rute>();
            List<Context.DaftarHargaOnCall> dho = RepoDHO.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);
            List<DaftarHargaOnCallItem> model = new List<DaftarHargaOnCallItem>();
            Context.DaftarHargaOnCall dhoitem = dho.Where(d => tanggalMuat >= d.PeriodStart && tanggalMuat <= d.PeriodEnd).FirstOrDefault();

            if (dho != null)
            {
                foreach (Context.DaftarHargaOnCallItem item in dhoitem.DaftarHargaOnCallItem.ToList())
                {
                    model.Add(new DaftarHargaOnCallItem(item));
                }
            }

            return new JavaScriptSerializer().Serialize(new { data = model });
        }
        //[HttpPost]
        public JsonResult GetPeriodeByCust(int id)
        {
            List<Context.DaftarHargaOnCall> dho = RepoDHO.FindAll().Where(d => d.IdCust == id && d.DaftarHargaOnCallItem.Count > 0).ToList();
            var listPeriode = dho.Select(c => new
            {
                Id = c.Id,
                PeriodeHarga = c.PeriodStart.ToString("dd/MM/yyyy") + " - " + c.PeriodEnd.ToString("dd/MM/yyyy")

            });

            return this.Json(listPeriode, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public String GetItem(int id, string date)
        {
            DateTime dtrDate = DateTime.Parse(date.Split(new string[] { " - " }, StringSplitOptions.None)[0]);
            DateTime dtrDate2 = DateTime.Parse(date.Split(new string[] { " - " }, StringSplitOptions.None)[1]);

            List<DaftarHargaOnCallItem> listItem = new List<DaftarHargaOnCallItem>();
            Context.DaftarHargaOnCall DHO = RepoDHO.FindAll().Where(d => d.Id == id && d.PeriodStart == dtrDate && d.PeriodEnd == dtrDate2).FirstOrDefault();
            if (DHO != null)
            {
                foreach (Context.DaftarHargaOnCallItem item in DHO.DaftarHargaOnCallItem)
                {
                    listItem.Add(new DaftarHargaOnCallItem(item));
                }
            }

            return new JavaScriptSerializer().Serialize(
                listItem.Select(i => new
                {
                    Id = i.Id,
                    NamaRuteDaftarHarga = i.NamaRuteDaftarHarga,
                    ListIdRute = i.ListIdRute,
                    ListNamaRute = i.ListNamaRute,
                    IdJenisTruck = i.IdJenisTruck,
                    NamaJenisTruck = i.NamaJenisTruck,
                    MinKg = i.MinKg,
                    Harga = i.Harga,
                    IdSatuanHarga = i.IdSatuanHarga,
                    SatuanHarga = i.SatuanHarga,
                    IsAsuransi = i.IsAsuransi,
                    IsAdhoc = i.IsAdHoc,
                    PihakPenanggung = i.PihakPenanggung,
                    TipeNilaiTanggungan = i.TipeNilaiTanggungan,
                    Premi = i.Premi,
                    NilaiTanggungan = i.NilaiTanggungan,
                    Keterangan = i.Keterangan
                }));
        }

        public JsonResult IsUsed(int id, bool isImported)
        {
            bool isExistData = false;

            // fitur Import Daftar Harga
            if (isImported)
            {
                Context.DaftarHargaOnCall dbItem = RepoDHO.FindByPK(id);
                List<int> listId = dbItem.DaftarHargaOnCallItem.Where(i => i.IdDaftarHargaOnCall == dbItem.Id).Select(i => i.Id).ToList();
                isExistData = RepoSO.FindAllOnCall().Where(a => listId.Contains(a.SalesOrderOncall.IdDaftarHargaItem.Value)).Count() > 0;
            }
            // edit & delete grid item Daftar Harga
            else
            {
                isExistData = RepoSO.FindAllOnCall().Where(a => a.SalesOrderOncall.IdDaftarHargaItem == id).Count() > 0;
            }

            return this.Json(isExistData, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}