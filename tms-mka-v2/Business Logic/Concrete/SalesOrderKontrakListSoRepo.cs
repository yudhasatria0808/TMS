using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Data.Entity;
using tms_mka_v2.Context;
using tms_mka_v2.Business_Logic.Abstract;
using tms_mka_v2.Infrastructure;
using tms_mka_v2.Linq;


namespace tms_mka_v2.Business_Logic.Concrete
{
    public class SalesOrderKontrakListSoRepo : ISalesOrderKontrakListSoRepo
    {
        private ContextModel context = new ContextModel();
        public void save(SalesOrderKontrakListSo dbitem)
        {
            if (dbitem.Id == 0) //create
            {
                context.SalesOrderKontrakListSo.Add(dbitem);
                var auditrail = new Auditrail { Actionnya = "Add", EventDate = DateTime.Now, Modulenya = "Sales Order Kontrak List", QueryDetail = "Add " + dbitem.Id, RemoteAddress = AppHelper.GetIPAddress(), IdUser = 1 };
                context.Auditrail.Add(auditrail);
            }
            else //edit
            {
                context.SalesOrderKontrakListSo.Attach(dbitem);
                var auditrail = new Auditrail { Actionnya = "Edit", EventDate = DateTime.Now, Modulenya = "Sales Order Kontrak List", QueryDetail = "Edit " + dbitem.Id, RemoteAddress = AppHelper.GetIPAddress(), IdUser = 1 };
                context.Auditrail.Add(auditrail);
                var entry = context.Entry(dbitem);
                entry.State = EntityState.Modified;
            }
            context.SaveChanges();
        }
        public SalesOrderKontrakListSo FindByPK(int id)
        {
            return context.SalesOrderKontrakListSo.Where(d => d.Id == id).FirstOrDefault();
        }
        public SalesOrderKontrakListSo FindByNoSo(string noso)
        {
            return context.SalesOrderKontrakListSo.Where(d => d.NoSo == noso).FirstOrDefault();
        }
        public void OnlyAdd(SalesOrderKontrakListSo dbitem)
        {
            context.SalesOrderKontrakListSo.Add(dbitem);
            context.SaveChanges();
        }

        public void OnlyUpdate(SalesOrderKontrakListSo dbitem)
        {
            context.SalesOrderKontrakListSo.Attach(dbitem);
            var entry = context.Entry(dbitem);
            entry.State = EntityState.Modified;
            context.SaveChanges();
        }

        public void deleteAdd(int? id)
        {
            Context.SalesOrderKontrak dbitem = context.SalesOrderKontrak.Where(d => d.SalesOrderKontrakId == id).FirstOrDefault();
            
            if (dbitem.SalesOrderKontrakListSo != null)
            {
               context.SalesOrderKontrakListSo.RemoveRange(dbitem.SalesOrderKontrakListSo);
            }
            context.SaveChanges();
        }

        public void returnListSo(int? id)
        {
            List<SalesOrderKontrakListSo> dbitem = context.SalesOrderKontrakListSo.Where(d => d.SalesKontrakId == id).ToList();

            foreach (var ListSo in dbitem)
            {
                ListSo.IdDataTruck = null;
                ListSo.Driver1Id = null;
                ListSo.Driver2Id = null;
                ListSo.Urutan = 0;
            }
            context.SaveChanges();
        }

        public List<SalesOrderKontrakListSo> returnListSoBatalTruckOnly()
        {
            return context.SalesOrderKontrakListSo.Where(d => d.IsBatalTruck == true && d.Status == "draft planning").ToList();
        }

        public string generateCodeListSo(string NoKontrak, DateTime valdate, int rit, int urutan, int urutanInduk)
        {
            return "K-" + valdate.ToString("yyMM") + '-' + (urutanInduk).ToString().PadLeft(4, '0') + '-' + valdate.ToString("dd") + '.' + urutan.ToString();
        }
        public int getUrutanProses(int? id)
        {
            List<SalesOrderKontrakListSo> dbkontrakListSo = context.SalesOrderKontrakListSo.Where(d => d.SalesKontrakId == id).ToList();
            return dbkontrakListSo.Max(d => d.Urutan);
        }
    }
}