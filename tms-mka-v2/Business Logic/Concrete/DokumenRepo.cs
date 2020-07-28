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
    public class DokumenRepo : IDokumenRepo
    {
        private ContextModel context = new ContextModel();
        public void save(Dokumen dbitem, int id, string strQuery=null)
        {
            if (dbitem.Id == 0) //create
            {
                context.Dokumen.Add(dbitem);
            }
            else //edit
            {
                context.Dokumen.Attach(dbitem);
                var entry = context.Entry(dbitem);
                entry.State = EntityState.Modified;
                var query = "UPDATE dbo.\"Dokumen\" SET \"IsComplete\" = " + dbitem.IsComplete + " \"ModifiedDate\" = " + dbitem.ModifiedDate + "\"IsReturn\" = " + dbitem.IsReturn + " WHERE \"Id\" = " + dbitem.Id + ";";
                var auditrail = new Auditrail {
                    Actionnya = "Edit", EventDate = DateTime.Now, Modulenya = "Dokumen", QueryDetail = query + (strQuery == null ? "" : strQuery), RemoteAddress = AppHelper.GetIPAddress(), IdUser = id
                };
                context.Auditrail.Add(auditrail);
            }
            context.SaveChanges();
        }

        public string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        public List<Dokumen> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<Dokumen> list = context.Dokumen;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<Dokumen>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    list = list.OrderBy<Dokumen>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<Dokumen>("Id"); //default, wajib ada atau EF error
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
            List<Dokumen> result = takeList.ToList();
            return result;
        }
        public int Count(FilterInfo filters = null)
        {
            IQueryable<Dokumen> items = context.Dokumen;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<Dokumen>(filters, ref items);
            }

            return items.Count();
        }
        public Dokumen FindByPK(int id)
        {
            return context.Dokumen.Where(d => d.Id == id).FirstOrDefault();
        }
        public Dokumen FindBySO(int idSO)
        {
            return context.Dokumen.Where(d => d.IdSO == idSO && d.IsAdmin == true).FirstOrDefault();
        }
    }
}