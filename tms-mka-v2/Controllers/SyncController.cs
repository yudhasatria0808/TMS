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

/*
Berhubung Driver & Customer sama2 Sync ke ptnr_mstr, jadi utk kolom ptnr_idnya memang ambil dari Id masing2 tabel, tapi:
Driver:
 - ptnr_id = Id + 7000000
 - Tambah kolom oid supaya benar2 terhubung

Customer: +0
*/

namespace tms_mka_v2.Controllers
{
    public class SyncController : BaseController 
    {
        private IDriverRepo RepoDriver;
        private IUserRepo RepoUser;
        private Iptnr_mstrRepo Repoptnr_mstr;
        private ICustomerRepo RepoCustomer;
        private IDataTruckRepo RepoDataTruck;
        private Icode_mstrRepo Repocode_mstr;

        public SyncController(IUserReferenceRepo repoBase, IDriverRepo repoDriver, IUserRepo repoUser, Iptnr_mstrRepo repoptnr_mstr, ILookupCodeRepo repoLookup, ICustomerRepo repoCustomer, IDataTruckRepo repoDataTruck,
            Icode_mstrRepo repocode_mstr)
            : base(repoBase, repoLookup)
        {   
            RepoDriver = repoDriver;
            RepoUser = repoUser;
            Repoptnr_mstr = repoptnr_mstr;
            RepoCustomer = repoCustomer;
            RepoDataTruck = repoDataTruck;
            Repocode_mstr = repocode_mstr;
        }

        public ActionResult sync_driver()
        {
            foreach (Context.Driver dbitem in RepoDriver.FindAll())
            {
                if (Repoptnr_mstr.FindByPK(dbitem.Id+7000000) == null)
                {
                    Repoptnr_mstr.saveDriver(dbitem);
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult sync_vehicle()
        {
            foreach (Context.DataTruck dbitem in RepoDataTruck.FindAll())
            {
                if (Repocode_mstr.FindByCodeName(dbitem.VehicleNo) == null)
                {
                    Repocode_mstr.saveVehicle(dbitem);
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult sync_customer()
        {
            foreach (Context.Customer dbitem in RepoCustomer.FindAll())
            {
                if (Repoptnr_mstr.FindByPK(dbitem.Id) == null)
                {
                    Repoptnr_mstr.save(dbitem);
                }
                else
                {
                    Context.ptnr_mstr dbptnr = Repoptnr_mstr.FindByPK(dbitem.Id);
                    dbptnr.ptnr_name = dbitem.CustomerNama;
                    Repoptnr_mstr.updateCustomer(dbptnr);
                }
            }
            return RedirectToAction("Index");
        }
    }
}