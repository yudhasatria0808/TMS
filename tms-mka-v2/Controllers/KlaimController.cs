using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tms_mka_v2.Business_Logic.Abstract;
using tms_mka_v2.Models;
using System.Web.Script.Serialization;
using tms_mka_v2.Infrastructure;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using OfficeOpenXml;
using System.Globalization;
using Newtonsoft.Json;

namespace tms_mka_v2.Controllers
{
    public class KlaimController : BaseController
    {
        private IKlaimRepo RepoKlaim;
        private ISalesOrderRepo RepoSalesOrder;
        private IAtmRepo RepoAtm;
        private IDataBoronganRepo RepoBor;
        private IKlaimDriverRepo RepoKlaimDriver;
        private Iglt_detRepo Repoglt_det;
        private IERPConfigRepo RepoERPConfig;
        private Iac_mstrRepo Repoac_mstr;
        public KlaimController(IUserReferenceRepo repoBase, ILookupCodeRepo repoLookUp, IKlaimRepo repoKlaim, ISalesOrderRepo repoSalesOrder, IAtmRepo repoAtm,
            IDataBoronganRepo repoBor, IKlaimDriverRepo repoKlaimDriver, Iglt_detRepo repoglt_det, IERPConfigRepo repoERPConfig, Iac_mstrRepo repoac_mstr)
            : base(repoBase, repoLookUp)
        {
            RepoKlaim = repoKlaim;
            RepoSalesOrder = repoSalesOrder;
            RepoAtm = repoAtm;
            RepoBor = repoBor;
            RepoKlaimDriver = repoKlaimDriver;
            Repoglt_det = repoglt_det;
            RepoERPConfig = repoERPConfig;
            Repoac_mstr = repoac_mstr;
        }
        public ActionResult Index()
        {
            ViewBag.listKolom = ListKolom.Where(d => d.Action == "Index" && d.Controller == "Klaim").ToList();

            return View();
        }
        public string Binding()
        {
            List<Context.Klaim> items = RepoKlaim.FindAll();

            List<Klaim> ListModel = new List<Klaim>();
            foreach (Context.Klaim item in items)
            {
                ListModel.Add(new Klaim(item));
            }

            return new JavaScriptSerializer().Serialize(new { total = ListModel.Count, data = ListModel });
        }
        public string BindingSalesOrder()
        {
            GridRequestParameters param = GridRequestParameters.Current;

            List<Context.SalesOrder> items = RepoSalesOrder.FindAllKlaim();

            List<ListOrder> ListModel = new List<ListOrder>();
            foreach (Context.SalesOrder item in items)
            {
                if (item.SalesOrderKontrakId.HasValue)
                {
                    if (item.SalesOrderKontrak.SalesOrderKontrakListSo.Any(p => p.IsProses))
                    {
                        foreach (var itemKontrak in item.SalesOrderKontrak.SalesOrderKontrakListSo.Where(s => s.Status == "dispatched" || s.Status == "settlement").GroupBy(d => new { d.IdDataTruck, d.Driver1Id, d.Status, d.Urutan }).Select(grp => grp.ToList()))
                        {
                            foreach (var itemKontrakPerOrder in itemKontrak.OrderBy(t => t.MuatDate).ToList())
                            {
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
        public ActionResult Add()
        {
            Klaim model = new Klaim();

            return View("Form", model);
        }
        [HttpPost]
        public ActionResult Add(Klaim model)
        {
            KlaimProduct[] result = JsonConvert.DeserializeObject<KlaimProduct[]>(model.strProduk);
            model.ListProduct = result.ToList();
            KlaimAttachment[] resultAtt = JsonConvert.DeserializeObject<KlaimAttachment[]>(model.StrAtt);
            model.ListAtt = resultAtt.ToList();
            if (ModelState.IsValid && IsValid(model, ModelState))
            {
                Context.Klaim dbitem = new Context.Klaim();
                int seq = RepoKlaim.getUrutanOnCAll(model.TanggalPengajuan.Value);
                dbitem.NoKlaim = RepoKlaim.GenerateCode(model.TanggalPengajuan.Value, seq);
                bool Isexist = RepoKlaim.IsExist(model.NoKlaim);

                if (Isexist)
                {
                    ModelState.AddModelError("NoKlaim", "No Klaim telah dipakai.");
                    return View("Form", model);
                }
                if (model.BebanClaimDriver + model.BebanClaimKantor != model.BebanClaim)
                {
                    ModelState.AddModelError("BebanClaimDriver", "Beban Klaim Driver + Beban Klaim Kantor harus sama dengan Beban Klaim");
                    return View("Form", model);
                }

                model.setDb(dbitem);
                RepoKlaim.save(dbitem, UserPrincipal.id);
                //tambah beban klaim nya driver
                int idDriver = 0;
                Context.SalesOrder dbso = RepoSalesOrder.FindByPK(model.SOKlaimId.Value);
                if (dbitem.SalesOrderKontrakId == 0)
                {
                    idDriver = dbso.AdminUangJalan.IdDriver1.Value;
                }
                else
                {
                    idDriver = dbso.SalesOrderKontrak.SalesOrderKontrakListSo.Where(
                        d => d.Id == dbitem.SalesOrderKontrakId
                        ).FirstOrDefault().Driver1Id.Value;
                }
                Context.BebanKlaimDriver dbKlaimDriver = new Context.BebanKlaimDriver()
                {
                    IdDriver = idDriver,
                    Klaim = dbitem
                };
                RepoKlaimDriver.save(dbKlaimDriver, UserPrincipal.id);
                //lebah dieu sync ERPna
                Context.ERPConfig erpConfig = RepoERPConfig.FindByFrist();
                Repoglt_det.saveFromAc(1, dbitem.NoKlaim, decimal.Parse(dbitem.TotalPengajuanClaim.Value.ToString()), 0, Repoac_mstr.FindByPk(erpConfig.IdBiayaKlaim));//D
                Repoglt_det.saveFromAc(2, dbitem.NoKlaim, 0, decimal.Parse(dbitem.TotalPengajuanClaim.Value.ToString()), Repoac_mstr.FindByPk(erpConfig.IdKreditKlaim));//K
                return RedirectToAction("Index");
            }
            return View("Form", model);
        }
        public ActionResult Edit(int id)
        {
            Klaim model = new Klaim(RepoKlaim.FindByPK(id));
            ViewBag.name = model.NoKlaim;
            return View("Form", model);
        }
        [HttpPost]
        public ActionResult Edit(Klaim model)
        {
            KlaimProduct[] result = JsonConvert.DeserializeObject<KlaimProduct[]>(model.strProduk);
            model.ListProduct = result.ToList();
            KlaimAttachment[] resultAtt = JsonConvert.DeserializeObject<KlaimAttachment[]>(model.StrAtt);
            model.ListAtt = resultAtt.ToList();
            if (ModelState.IsValid && IsValid(model, ModelState))
            {
                Context.Klaim dbitem = RepoKlaim.FindByPK(model.Id);
                model.setDb(dbitem);
                RepoKlaim.save(dbitem, UserPrincipal.id);

                return RedirectToAction("Index");
            }

            return View("Form", model);
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            ResponeModel response = new ResponeModel(true);
            Context.Klaim dbItem = RepoKlaim.FindByPK(id);

            RepoKlaim.delete(dbItem, UserPrincipal.id);

            return Json(response);
        }
        [HttpGet]
        public PartialViewResult GetPartialSo(int idSo, int IdSoKontrak = 0)
        {
            Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(idSo);

            if (dbitem.SalesOrderOncallId.HasValue)
            {
                AdminUangJalan model = new AdminUangJalan(dbitem.AdminUangJalan, RepoAtm.FindAll(), RepoBor.FindAll());
                model.ModelOncall = new SalesOrderOncall(dbitem);
                return PartialView("SalesOrderOncall/_PartialFormReadOnly", model.ModelOncall);
            }
            else if (dbitem.SalesOrderPickupId.HasValue)
            {
                AdminUangJalan model = new AdminUangJalan(dbitem.AdminUangJalan, RepoAtm.FindAll(), RepoBor.FindAll());
                model.ModelPickup = new SalesOrderPickup(dbitem);
                return PartialView("SalesOrderPickup/_PartialFormReadOnly", model.ModelPickup);
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue)
            {
                AdminUangJalan model = new AdminUangJalan(dbitem.AdminUangJalan, RepoAtm.FindAll(), RepoBor.FindAll());
                model.ModelKonsolidasi = new SalesOrderProsesKonsolidasi(dbitem);
                return PartialView("SalesOrderProsesKonsolidasi/_PartialFormReadOnly", model.ModelKonsolidasi);
            }
            else if (IdSoKontrak != 0)
            {
                //List<int> ListIdDumy = ListIdSo.Split(new string[] { "." }, StringSplitOptions.None).ToList().Select(int.Parse).ToList();
                Context.SalesOrderKontrakListSo dbsoDummy = dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => d.Id == IdSoKontrak).FirstOrDefault();
                dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Clear();
                dbitem.SalesOrderKontrak.SalesOrderKontrakListSo.Add(dbsoDummy);
                AdminUangJalan model = new AdminUangJalan(dbsoDummy.AdminUangJalan, RepoAtm.FindAll(), RepoBor.FindAll());
                model.ModelKontrak = new SalesOrderKontrak(dbitem);
                return PartialView("SalesOrderKontrak/_PartialFormReadOnly", model);
            }
            return PartialView("");
        }
        public string GetKlaim()
        {
            List<Context.Klaim> dbitems = RepoKlaim.FindAll();

            return new JavaScriptSerializer().Serialize(dbitems.Select(d => new { Id = d.Id, Nama = d.NoKlaim }));
        }
        public bool IsValid(Klaim model, ModelStateDictionary modelState)
        {
            bool ispalid = true;

            if (model.Kesalahan == "PihakLain")
            {
                if (string.IsNullOrEmpty(model.KesalahanLain))
                {
                    modelState.AddModelError("KesalahanLain", "Pihak kesalahan wajib diisi.");
                    ispalid = false;
                }
            }

            if (model.AsuransiFlag)
            {
                if (!model.Asuransi.HasValue)
                {
                    modelState.AddModelError("Asuransi", "Asuransi wajib diisi.");
                    ispalid = false;
                }
            }

            if (model.AsuransiFlag)
            {
                if (!model.Asuransi.HasValue)
                {
                    modelState.AddModelError("Asuransi", "Asuransi wajib diisi.");
                    ispalid = false;
                }
            }

            if (model.BebanClaimDriverPercentage.HasValue)
            {
                if (!model.BebanClaimDriver.HasValue)
                {
                    modelState.AddModelError("BebanClaimDriver", "Beban klaim driver wajib diisi.");
                    ispalid = false;
                }
            }

            if (model.BebanClaimKantorPercentage.HasValue)
            {
                if (!model.BebanClaimKantor.HasValue)
                {
                    modelState.AddModelError("BebanClaimKantor", "BebanClaimKantor wajib diisi.");
                    ispalid = false;
                }
            }

            return ispalid;
        }
    }
}