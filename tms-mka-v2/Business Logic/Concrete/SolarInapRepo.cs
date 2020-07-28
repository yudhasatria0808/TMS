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
    public class SolarInapRepo : ISolarInapRepo
    {
        private ContextModel context = new ContextModel();
        public void save(SolarInap dbitem, int id)
        {
            if (dbitem.Id == 0) //create
            {
                context.SolarInap.Add(dbitem);
                var query = "INSERT INTO dbo.\"SolarInap\" (\"IdSO\", \"TanggalDari\", \"TanggalHingga\", \"NilaiYgDiajukan\", \"KeteranganOperation\", \"Nominal\", \"KeteranganMarketing\", " +
                    "\"KeteranganAdmin\", \"StatusTagihan\", \"IdDriver\", \"Cash\", \"Transfer\", \"TglTransfer\", \"TglCash\", \"DititipKe\", \"IdAtm\", \"StepKe\", \"Status\", \"AktualCash\", " +
                    "\"AktualTransfer\", \"KeteranganKasirCash\", \"KeteranganBatal\", \"KeteranganKasirTransfer\", \"TanggalAktualCash\", \"JamAktualCash\", \"TanggalAktualTransfer\", "+
                    "\"JamAktualTransfer\", \"AktualDititipkanKepada\", \"TanggalBatal\", \"AktualIdAtm\", \"CashBack\", \"SalesOrderKontrakListSOId\", \"Code\") VALUES ( " + dbitem.IdSO + ", " + 
                    dbitem.TanggalDari + ", " + dbitem.TanggalHingga + ", " + dbitem.NilaiYgDiajukan + ", " + dbitem.KeteranganOperation + ", " + dbitem.Nominal + ", " + dbitem.KeteranganMarketing + ", " +
                    dbitem.KeteranganAdmin + ", " + dbitem.StatusTagihan + ", " + dbitem.IdDriver + ", " + dbitem.Cash + ", " + dbitem.Transfer + ", " + dbitem.TglTransfer + ", " + dbitem.TglCash + ", " +
                    dbitem.DititipKe + ", " + dbitem.IdAtm + ", " + dbitem.StepKe + ", " + dbitem.Status + ", " + dbitem.AktualCash + ", " + dbitem.AktualTransfer + ", " + dbitem.KeteranganKasirCash +
                    ", " + dbitem.KeteranganBatal + ", " + dbitem.KeteranganKasirTransfer + ", " + dbitem.TanggalAktualCash + ", " + dbitem.JamAktualCash + ", " + dbitem.TanggalAktualTransfer + ", " +
                    dbitem.JamAktualTransfer + ", " + dbitem.AktualDititipkanKepada + ", " + dbitem.TanggalBatal + ", " + dbitem.AktualIdAtm + ", " + dbitem.CashBack + ", " +
                    dbitem.SalesOrderKontrakListSOId + ", " + dbitem.Code + ");";
                var auditrail = new Auditrail { 
                    Actionnya = "Add", EventDate = DateTime.Now, Modulenya = "SolarInap", QueryDetail = query, RemoteAddress = AppHelper.GetIPAddress(), IdUser = id
                };
                context.Auditrail.Add(auditrail);
            }
            else //edit
            {
                context.SolarInap.Attach(dbitem);
                var query = "UPDATE dbo.\"SolarInap\" SET \"TanggalDari\" = " + dbitem.TanggalDari + ", \"TanggalHingga\" = " + dbitem.TanggalHingga + ", \"NilaiYgDiajukan\" = " + dbitem.NilaiYgDiajukan +
                    ", \"KeteranganOperation\" = " + dbitem.KeteranganOperation + ", \"Nominal\" = " + dbitem.Nominal + ", \"KeteranganMarketing\" = " + dbitem.KeteranganMarketing +
                    ", \"KeteranganAdmin\" = " + dbitem.KeteranganAdmin + ", \"StatusTagihan\" = " + dbitem.StatusTagihan + ", \"IdDriver\" = " + dbitem.IdDriver + ", \"Cash\" = " + dbitem.Cash +
                    ", \"Transfer\" = " + dbitem.Transfer + ", \"TglTransfer\" = " + dbitem.TglTransfer + ", \"TglCash\" = " + dbitem.TglCash + ", \"DititipKe\" = " + dbitem.DititipKe +
                    ", \"IdAtm\" = " + dbitem.IdAtm + ", \"StepKe\" = " + dbitem.StepKe + ", \"Status\" = " + dbitem.Status + ", \"AktualCash\" = " + dbitem.AktualCash + ", \"AktualTransfer\" = " +
                    dbitem.AktualTransfer + ", \"KeteranganKasirCash\" = " + dbitem.KeteranganKasirCash + ", \"KeteranganBatal\" = " + dbitem.KeteranganBatal + ", \"KeteranganKasirTransfer\" = " +
                    dbitem.KeteranganKasirTransfer + ", \"TanggalAktualCash\" = " + dbitem.TanggalAktualCash + ", \"JamAktualCash\" = " + dbitem.JamAktualCash + ", \"TanggalAktualTransfer\" = " +
                    dbitem.TanggalAktualTransfer + ", \"JamAktualTransfer\" = " + dbitem.JamAktualTransfer + ", \"AktualDititipkanKepada\" = " + dbitem.AktualDititipkanKepada + ", \"TanggalBatal\" = " +
                    dbitem.TanggalBatal + ", \"AktualIdAtm\" = " + dbitem.AktualIdAtm + ", \"CashBack\" = " + dbitem.CashBack + ", \"SalesOrderKontrakListSOId\" = " + dbitem.SalesOrderKontrakListSOId +
                    ", \"Code\" = " + dbitem.Code + "WHERE \"Id\" = " + dbitem.Id + ";";
                var auditrail = new Auditrail {
                    Actionnya = "Edit", EventDate = DateTime.Now, Modulenya = "SolarInap", QueryDetail = query, RemoteAddress = AppHelper.GetIPAddress(), IdUser = id
                };
                context.Auditrail.Add(auditrail);
                var entry = context.Entry(dbitem);
                entry.State = EntityState.Modified;
            }
            context.SaveChanges();
        }
        public List<SolarInap> FindAll(string Step, int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<SolarInap> list = context.SolarInap.Where(d => (Step.Contains("Kasir") ? d.StepKe == 3 : false) || (Step.Contains("Admin") ? d.StepKe == 2 : false) || (Step.Contains("Marketing") ? d.StepKe == 1 : false));

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SolarInap>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    list = list.OrderBy<SolarInap>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<SolarInap>("Id"); //default, wajib ada atau EF error
            }

            //take & skip
            var takeList = list;
            if (skip != null)
            {
                takeList = takeList.Skip(skip.Value);
            }
            if (take != null)
            {
                takeList = takeList.Take(take.Value);
            }

            //return result
            //var sql = takeList.ToString();
            List<SolarInap> result = takeList.ToList();
            return result;
        }
        public int Count(FilterInfo filters = null)
        {
            IQueryable<SolarInap> items = context.SolarInap;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SolarInap>(filters, ref items);
            }

            return items.Count();
        }
        public int CountTrans(string Step, FilterInfo filters = null)
        {
            IQueryable<SolarInap> items = context.SolarInap.Where(d => (Step.Contains("Kasir") ? d.StepKe == 3 : false) || (Step.Contains("Admin") ? d.StepKe == 2 : false) || (Step.Contains("Marketing") ? d.StepKe == 1 : false));

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SolarInap>(filters, ref items);
            }

            return items.Count();
        }
        public List<SolarInap> FindAllReport(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<SolarInap> list = context.SolarInap;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SolarInap>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    list = list.OrderBy<SolarInap>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<SolarInap>("Id"); //default, wajib ada atau EF error
            }

            //take & skip
            var takeList = list;
            if (skip != null)
            {
                takeList = takeList.Skip(skip.Value);
            }
            if (take != null)
            {
                takeList = takeList.Take(take.Value);
            }

            //return result
            //var sql = takeList.ToString();
            List<SolarInap> result = takeList.ToList();
            return result;
        }
        public SolarInap FindByPK(int id)
        {
            return context.SolarInap.Where(d => d.Id == id).FirstOrDefault();
        }

        public SolarInap FindBySOAndDate(int? so_id, System.DateTime date)
        {
            return context.SolarInap.Where(d => so_id == d.IdSO && d.TanggalDari == date).FirstOrDefault();
        }

        public void delete(SolarInap dbitem, int id)
        {
            context.SolarInap.Remove(dbitem);
            var query = "DELETE FROM dbo.\"SolarInap\" WHERE \"Id\" = " + dbitem.Id +";";
            var auditrail = new Auditrail {
                Actionnya = "Delete", EventDate = DateTime.Now, Modulenya = "SolarInap", QueryDetail = query, RemoteAddress = AppHelper.GetIPAddress(), IdUser = id
            };
            context.Auditrail.Add(auditrail);
            context.SaveChanges();
        }
    }
}