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
    public class SalesOrderRepo : ISalesOrderRepo
    {
        private ContextModel context = new ContextModel();
        public void save(SalesOrder dbitem)
        {
            dbitem.DateStatus = DateTime.Now;
            var order_history = new OrderHistory();
            if (dbitem.Id == 0) //create
            {
                context.SalesOrder.Add(dbitem);
            }
            else
            {
                context.SalesOrder.Attach(dbitem);
                if (dbitem.Status == "save"){
                    var last_oh = context.OrderHistory.Where(d => d.SalesOrderId == dbitem.Id && d.StatusFlow == 1 ).FirstOrDefault();
                    if (last_oh == null)
                        order_history = new OrderHistory {SalesOrderId = dbitem.Id, StatusFlow = 1, FlowDate = DateTime.Now, SavedAt = DateTime.Now, ProcessedAt = DateTime.Now, PIC = dbitem.UpdatedBy};
                    else
                        order_history = new OrderHistory {SalesOrderId = dbitem.Id, StatusFlow = 1, FlowDate = DateTime.Now, SavedAt = last_oh.SavedAt, ProcessedAt = DateTime.Now, PIC = dbitem.UpdatedBy};
                }
                context.OrderHistory.Add(order_history);
                var entry = context.Entry(dbitem);
                entry.State = EntityState.Modified;
            }
            dbitem.DateStatus = DateTime.Now;
            context.SaveChanges();
        }

        public void saveUangTf(AdminUangJalanUangTf dbitem)
        {
            if (dbitem.Id == 0) //create
            {
                context.AdminUangJalanUangTf.Add(dbitem);
                var auditrail = new Auditrail { Actionnya = "Add", EventDate = DateTime.Now, Modulenya = "Sales Order List", QueryDetail = "Add " + dbitem.Id, RemoteAddress = AppHelper.GetIPAddress(), IdUser = 1 };
                context.Auditrail.Add(auditrail);
            }
            else //edit
            {
                context.AdminUangJalanUangTf.Attach(dbitem);
                var auditrail = new Auditrail { Actionnya = "Edit", EventDate = DateTime.Now, Modulenya = "Sales Order List", QueryDetail = "Edit " + dbitem.Id, RemoteAddress = AppHelper.GetIPAddress(), IdUser = 1 };
                context.Auditrail.Add(auditrail);
                var entry = context.Entry(dbitem);
                entry.State = EntityState.Modified;
            }
            context.SaveChanges();
        }

        public List<SalesOrder> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<SalesOrder> list = context.SalesOrder;
            
            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SalesOrder>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    list = list.OrderBy<SalesOrder>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<SalesOrder>("Id"); //default, wajib ada atau EF error
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
            List<SalesOrder> result = takeList.ToList();
            return result;
        }
        public List<Context.OrderHistory> FindAllHistory(int idSo){
            IQueryable<OrderHistory> list = context.OrderHistory.Where(d => d.SalesOrderId == idSo);
            List<Context.OrderHistory> result = list.ToList();
            return result;
        }
        public List<SalesOrder> FindAllOnCall(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<SalesOrder> list = context.SalesOrder.Where(d => d.SalesOrderOncallId.HasValue);

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SalesOrder>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    list = list.OrderBy<SalesOrder>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<SalesOrder>("Id"); //default, wajib ada atau EF error
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
            List<SalesOrder> result = takeList.ToList();
            return result;
        }
        public List<SalesOrder> FindAllKontrak(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<SalesOrder> list = context.SalesOrder.Where(d => d.SalesOrderKontrakId.HasValue);

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SalesOrder>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    list = list.OrderBy<SalesOrder>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<SalesOrder>("Id"); //default, wajib ada atau EF error
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
            List<SalesOrder> result = takeList.ToList();
            return result;
        }
        public List<SalesOrder> FindAllPickUp(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<SalesOrder> list = context.SalesOrder.Where(d => d.SalesOrderPickupId.HasValue);

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SalesOrder>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    list = list.OrderBy<SalesOrder>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<SalesOrder>("Id"); //default, wajib ada atau EF error
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
            List<SalesOrder> result = takeList.ToList();
            return result;
        }
        public List<SalesOrder> FindAllKonsolidasi(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<SalesOrder> list = context.SalesOrder.Where(d => d.SalesOrderKonsolidasiId.HasValue);

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SalesOrder>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    list = list.OrderBy<SalesOrder>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<SalesOrder>("Id"); //default, wajib ada atau EF error
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
            List<SalesOrder> result = takeList.ToList();
            return result;
        }
        public List<SalesOrder> FindAllProsesKonsolidasi(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<SalesOrder> list = context.SalesOrder.Where(d => d.SalesOrderProsesKonsolidasiId.HasValue);

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SalesOrder>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    list = list.OrderBy<SalesOrder>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<SalesOrder>("Id"); //default, wajib ada atau EF error
            }

            //take & skip
            var takeList = list;
            if (skip != null && skip != 0)
            {
                takeList = takeList.Skip(skip.Value);
            }
            if (take != null && take != 0)
            {
                takeList = takeList.Take(take.Value);
            }

            //return result
            List<SalesOrder> result = takeList.ToList();
            return result;
        }
        public List<SalesOrder> FindAllAdminUangJalan(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<SalesOrder> list = context.SalesOrder.Where(d => (d.Status == "save konfirmasi" || (d.Status == "dispatched" && d.AdminUangJalan.Removal.Count > 0)) || (d.SalesOrderKontrakId.HasValue && d.Status == "draft konfirmasi"));

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SalesOrder>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    list = list.OrderBy<SalesOrder>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<SalesOrder>("Id"); //default, wajib ada atau EF error
            }

            //take & skip
            var takeList = list;
            if (skip != null && skip != 0)
            {
                takeList = takeList.Skip(skip.Value);
            }
            if (take != null && take != 0)
            {
                takeList = takeList.Take(take.Value);
            }

            //return result
            List<SalesOrder> result = takeList.ToList();
            return result;
        }
        public List<SalesOrder> FindAllAdminDispatched(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<SalesOrder> list = context.SalesOrder.Where(d => d.Status == "dispatched" || (d.SalesOrderKontrakId.HasValue && d.SalesOrderKontrak.SalesOrderKontrakListSo.Any(i => i.Status == "dispatched")));

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SalesOrder>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    list = list.OrderBy<SalesOrder>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<SalesOrder>("Id"); //default, wajib ada atau EF error
            }

            //take & skip
            var takeList = list;
            if (skip != null && skip != 0)
            {
                takeList = takeList.Skip(skip.Value);
            }
            if (take != null && take != 0)
            {
                takeList = takeList.Take(take.Value);
            }

            //return result
            List<SalesOrder> result = takeList.ToList();
            return result;
        }
        public List<SalesOrder> FindAllKasir(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<SalesOrder> list = context.SalesOrder.Where(d => d.Status == "admin uang jalan" || d.Status == "dispatched" || (d.SalesOrderKontrakId.HasValue && (d.Status == "draft konfirmasi" || d.Status == "save konfirmasi")));

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SalesOrder>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    list = list.OrderBy<SalesOrder>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<SalesOrder>("Id"); //default, wajib ada atau EF error
            }

            //take & skip
            var takeList = list;
            if (skip != null && skip != 0)
            {
                takeList = takeList.Skip(skip.Value);
            }
            if (take != null && take != 0)
            {
                takeList = takeList.Take(take.Value);
            }

            //return result
            List<SalesOrder> result = takeList.ToList();
            return result;
        }
        public List<SalesOrder> FindAllKasirReport(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<SalesOrder> list = context.SalesOrder.Where(d => d.Status == "batal order" || d.Status == "settlement" || d.Status == "dispatched" || (d.SalesOrderKontrakId.HasValue && (d.Status == "draft konfirmasi" || d.Status == "save konfirmasi")));

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SalesOrder>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    list = list.OrderBy<SalesOrder>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<SalesOrder>("Id"); //default, wajib ada atau EF error
            }

            //take & skip
            var takeList = list;
            if (skip != null && skip != 0)
            {
                takeList = takeList.Skip(skip.Value);
            }
            if (take != null && take != 0)
            {
                takeList = takeList.Take(take.Value);
            }

            //return result
            List<SalesOrder> result = takeList.ToList();
            return result;
        }
        public List<SalesOrder> FindAllAUJReport(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<SalesOrder> list = context.SalesOrder.Where(d => d.IsBatalTruk == true || d.Status == "batal order" || d.Status == "settlement" || d.Status == "admin uang jalan" || d.Status == "dispatched" || (d.SalesOrderKontrakId.HasValue && (d.Status == "draft konfirmasi" || d.Status == "save konfirmasi")));

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SalesOrder>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    list = list.OrderBy<SalesOrder>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<SalesOrder>("Id"); //default, wajib ada atau EF error
            }

            //take & skip
            var takeList = list;
            if (skip != null && skip != 0)
            {
                takeList = takeList.Skip(skip.Value);
            }
            if (take != null && take != 0)
            {
                takeList = takeList.Take(take.Value);
            }

            //return result
            List<SalesOrder> result = takeList.ToList();
            return result;
        }
        public List<SalesOrder> FindAllKlaim(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<SalesOrder> list = context.SalesOrder.Where(d => !d.SalesOrderKonsolidasiId.HasValue && (d.Status == "dispatched" || d.Status == "settlement" || (d.SalesOrderKontrakId.HasValue && d.Status == "draft konfirmasi")));

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SalesOrder>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    list = list.OrderBy<SalesOrder>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<SalesOrder>("Id"); //default, wajib ada atau EF error
            }

            //take & skip
            var takeList = list;
            if (skip != null && skip != 0)
            {
                takeList = takeList.Skip(skip.Value);
            }
            if (take != null && take != 0)
            {
                takeList = takeList.Take(take.Value);
            }

            //return result
            List<SalesOrder> result = takeList.ToList();
            return result;
        }
        public int Count(FilterInfo filters = null)
        {
            IQueryable<SalesOrder> items = context.SalesOrder;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SalesOrder>(filters, ref items);
            }

            return items.Count();
        }
        public int CountOncall(FilterInfo filters = null)
        {
            IQueryable<SalesOrderKontrak> items = context.SalesOrderKontrak;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SalesOrderKontrak>(filters, ref items);
            }

            return items.Count();
        }
        public int CountKontrak(FilterInfo filters = null)
        {
            IQueryable<SalesOrderKontrak> items = context.SalesOrderKontrak;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SalesOrderKontrak>(filters, ref items);
            }

            return items.Count();
        }
        public int CountPickup(FilterInfo filters = null)
        {
            IQueryable<SalesOrderPickup> items = context.SalesOrderPickup;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SalesOrderPickup>(filters, ref items);
            }

            return items.Count();
        }
        public int CountKonsolidasi(FilterInfo filters = null)
        {
            IQueryable<SalesOrderKonsolidasi> items = context.SalesOrderKonsolidasi;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SalesOrderKonsolidasi>(filters, ref items);
            }

            return items.Count();
        }
        public int CountProsesKonsolidasi(FilterInfo filters = null)
        {
            IQueryable<SalesOrderProsesKonsolidasi> items = context.SalesOrderProsesKonsolidasi;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<SalesOrderProsesKonsolidasi>(filters, ref items);
            }

            return items.Count();
        }
        public SalesOrder FindByPK(int id)
        {
            return context.SalesOrder.Where(d => d.Id == id).FirstOrDefault();
        }
        public SalesOrder FindByOnCall(int id)
        {
            return context.SalesOrder.Where(d => d.SalesOrderOncallId == id).FirstOrDefault();
        }
        public SalesOrder FindByOnCallCode(string number)
        {
            return context.SalesOrder.Where(d => d.SalesOrderOncallId.HasValue && d.SalesOrderOncall.SONumber == number).FirstOrDefault();
        }
        public SalesOrder FindByKontrak(int id)
        {
            return context.SalesOrder.Where(d => d.SalesOrderKontrakId == id).FirstOrDefault();
        }
        public SalesOrder FindByPickup(int id)
        {
            return context.SalesOrder.Where(d => d.SalesOrderPickupId == id).FirstOrDefault();
        }
        public SalesOrder FindByKonsolidasi(int id)
        {
            return context.SalesOrder.Where(d => d.SalesOrderKonsolidasiId == id).FirstOrDefault();
        }
        public SalesOrder FindByProsesKonsolidasi(int id)
        {
            return context.SalesOrder.Where(d => d.SalesOrderProsesKonsolidasiId == id).FirstOrDefault();
        }
        public void delete(SalesOrder dbitem, int id)
        {
            var auditrail = new Auditrail();
            if (dbitem.SalesOrderOncall != null)
            {
                dbitem.SalesOrderOncall.SalesOrderOnCallLoadingAdd.Clear();
                dbitem.SalesOrderOncall.SalesOrderOnCallUnLoadingAdd.Clear();
                dbitem.SalesOrderOncall.SalesOrderOnCallComment.Clear();
                context.SalesOrderOncall.Remove(dbitem.SalesOrderOncall);
            }
            else if (dbitem.SalesOrderKontrak != null)
            {
                dbitem.SalesOrderKontrak.SalesOrderKontrakDetail.Clear();
                context.SalesOrderKontrak.Remove(dbitem.SalesOrderKontrak);
            }
            else if (dbitem.SalesOrderPickup != null)
            {
                dbitem.SalesOrderPickup.SalesOrderPickupLoadingAdd.Clear();
                dbitem.SalesOrderPickup.SalesOrderPickupUnLoadingAdd.Clear();
                context.SalesOrderPickup.Remove(dbitem.SalesOrderPickup);
                var query = "DELETE FROM dbo.\"SalesOrderPickupLoadingAdd\" WHERE \"SalesOrderPickupId\" = " + dbitem.SalesOrderPickupId + ";";
                auditrail = new Auditrail {
                    Actionnya="Delete SO Pickup", EventDate=DateTime.Now, Modulenya = "Sales Order", QueryDetail = query, RemoteAddress = AppHelper.GetIPAddress(), IdUser = id
                };
                context.Auditrail.Add(auditrail);
            }
            else if (dbitem.SalesOrderKonsolidasi != null)
            {
                context.SalesOrderKonsolidasi.Remove(dbitem.SalesOrderKonsolidasi);
            }
            else if (dbitem.SalesOrderProsesKonsolidasi != null)
            {
                foreach (SalesOrderProsesKonsolidasiItem item in dbitem.SalesOrderProsesKonsolidasi.SalesOrderProsesKonsolidasiItem)
                {
                    item.SalesOrderKonsolidasi.IsSelect = false;
                    item.SalesOrderKonsolidasi.Harga = 0;
                    item.SalesOrderKonsolidasi.TotalHarga = 0;
                    context.SalesOrderKonsolidasi.Attach(item.SalesOrderKonsolidasi);

                    var entry = context.Entry(item.SalesOrderKonsolidasi);
                    entry.State = EntityState.Modified;
                    context.SaveChanges();
                }
                dbitem.SalesOrderProsesKonsolidasi.SalesOrderProsesKonsolidasiItem.Clear();
                dbitem.SalesOrderProsesKonsolidasi.SalesOrderProsesKonsolidasiLoadingAdd.Clear();
                dbitem.SalesOrderProsesKonsolidasi.SalesOrderProsesKonsolidasiUnLoadingAdd.Clear();
                context.SalesOrderProsesKonsolidasi.Remove(dbitem.SalesOrderProsesKonsolidasi);
                var query = "DELETE FROM dbo.\"SalesOrderProsesKonsolidasi\" WHERE \"SalesOrderProsesKonsolidasiId\" = " + dbitem.SalesOrderProsesKonsolidasiId + ";";
                query += "DELETE FROM dbo.\"SalesOrderProsesKonsolidasiItem\" WHERE \"SalesOrderProsesKonsolidasiId\" = " + dbitem.SalesOrderProsesKonsolidasiId + ";";
                query += "DELETE FROM dbo.\"SalesOrderProsesKonsolidasiLoadingAdd\" WHERE \"SalesOrderProsesKonsolidasiId\" = " + dbitem.SalesOrderProsesKonsolidasiId + ";";
                query += "DELETE FROM dbo.\"SalesOrderProsesKonsolidasiUnLoadingAdd\" WHERE \"SalesOrderProsesKonsolidasiId\" = " + dbitem.SalesOrderProsesKonsolidasiId + ";";
                auditrail = new Auditrail {
                    Actionnya="Delete SO Konsolidasi", EventDate=DateTime.Now, Modulenya = "Sales Order", QueryDetail = query, RemoteAddress = AppHelper.GetIPAddress(), 
                    IdUser = id
                };
                context.Auditrail.Add(auditrail);
            }

            context.SalesOrder.Remove(dbitem);
            auditrail = new Auditrail {
                Actionnya = "Delete", EventDate = DateTime.Now, Modulenya = "Sales Order List", QueryDetail = "Delete " + dbitem.Id, RemoteAddress = AppHelper.GetIPAddress(),
                IdUser = 1 
            };
            context.Auditrail.Add(auditrail);
            context.SaveChanges();
        }
        public string generateCodeOnCall(DateTime valdate,int urutan)
        {
            return "OC-" + valdate.ToString("yy") + valdate.Month.ToString("00").PadLeft(2, '0') + '-' + (urutan).ToString().PadLeft(4, '0');
        }
        public int getUrutanOnCAll(DateTime valdate)
        {
            DateTime firstDayOfMonth = new DateTime(valdate.Year, valdate.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            List<SalesOrderOncall> dboncall = context.SalesOrder.Where(d => d.SalesOrderOncallId != null).Select(d=>d.SalesOrderOncall).ToList();
            return dboncall.Where(d => d.TanggalMuat >= firstDayOfMonth && d.TanggalMuat <= lastDayOfMonth).Count() == 0 ? 0 : dboncall.Where(d => d.TanggalMuat >= firstDayOfMonth && d.TanggalMuat <= lastDayOfMonth).Max(d => d.Urutan);
        }
        public string generateCodeKontrak(int urutan)
        {
            return "K-" + DateTime.Now.ToString("yyMM") + '-' + (urutan).ToString().PadLeft(4, '0');
        }
        public int getUrutanKontrak()
        {
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            List<SalesOrderKontrak> dbkontrak = context.SalesOrder.Where(d => d.SalesOrderKontrakId != null).Select(d => d.SalesOrderKontrak).ToList();
            return dbkontrak.Where(d => d.DocDate >= firstDayOfMonth && d.DocDate <= lastDayOfMonth).Count() == 0 ? 0 : dbkontrak.Where(d => d.DocDate >= firstDayOfMonth && d.DocDate <= lastDayOfMonth).Max(d => d.Urutan);
        }
        public string generatePickup(DateTime valdate,int urutan)
        {
            return "DD-" + valdate.Year.ToString("00").PadLeft(2, '0') + valdate.Month.ToString("00").PadLeft(2, '0') + '-' + (urutan).ToString().PadLeft(4, '0');
        }
        public int getUrutanPickup(DateTime valdate)
        {
            DateTime firstDayOfMonth = new DateTime(valdate.Year, valdate.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            List<SalesOrderPickup> dbpickup = context.SalesOrder.Where(d => d.SalesOrderPickupId != null).Select(d => d.SalesOrderPickup).ToList();
            return dbpickup.Where(d => d.TanggalPickup >= firstDayOfMonth && d.TanggalPickup <= lastDayOfMonth).Count() == 0 ? 0 : dbpickup.Where(d => d.TanggalPickup >= firstDayOfMonth && d.TanggalPickup <= lastDayOfMonth).Max(d => d.Urutan);
        }
        public string generateKonsolidasi(DateTime valdate,int urutan)
        {
            return "KSO-" + valdate.Year.ToString("00").PadLeft(2, '0') + valdate.Month.ToString("00").PadLeft(2, '0') + '-' + (urutan).ToString().PadLeft(4, '0');
        }
        public int getUrutanKonsolidasi(DateTime valdate)
        {
            DateTime firstDayOfMonth = new DateTime(valdate.Year, valdate.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            List<SalesOrderKonsolidasi> dbkonsolidasi = context.SalesOrder.Where(d => d.SalesOrderKonsolidasiId != null).Select(d => d.SalesOrderKonsolidasi).ToList();
            return dbkonsolidasi.Where(d => d.TanggalMasuk >= firstDayOfMonth && d.TanggalMasuk <= lastDayOfMonth).Count() == 0 ? 0 : dbkonsolidasi.Where(d => d.TanggalMasuk >= firstDayOfMonth && d.TanggalMasuk <= lastDayOfMonth).Max(d => d.Urutan);
        }
        public string generateProsesKonsolidasi(DateTime valdate,int urutan)
        {
            return "IKS-" + valdate.Year.ToString("00").PadLeft(2, '0') + valdate.Month.ToString("00").PadLeft(2, '0') + '-' + (urutan).ToString().PadLeft(4, '0');
        }
        public int getUrutanProsesKonsolidasi(DateTime valdate)
        {
            DateTime firstDayOfMonth = new DateTime(valdate.Year, valdate.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            List<SalesOrderProsesKonsolidasi> dbproses = context.SalesOrder.Where(d => d.SalesOrderProsesKonsolidasiId != null).Select(d => d.SalesOrderProsesKonsolidasi).ToList();
            return dbproses.Where(d => d.TanggalMuat >= firstDayOfMonth && d.TanggalMuat <= lastDayOfMonth).Count() == 0 ? 0 : dbproses.Where(d => d.TanggalMuat >= firstDayOfMonth && d.TanggalMuat <= lastDayOfMonth).Max(d => d.Urutan);
        }
        public string FindArea(int id)
        {
/*
Sales Order Oncall - IdDaftarHargaItem
Daftar Harga Oncall Item - Id
Daftar Harga Oncall Item - ListIdRute
Rute - Id
Rute - IdAreaAsal = 36 && IdAreaTujuan = 36
*/
            string idRutes = context.DaftarHargaOnCallItem.Where(d => d.Id == id).FirstOrDefault().ListIdRute;
            string[] rutes = idRutes.Split(',');
            if (rutes.Count() < 2)
            {
                int idRute = int.Parse(context.DaftarHargaOnCallItem.Where(d => d.Id == id).FirstOrDefault().ListIdRute);
                if (context.Rute.Where(d => d.Id == idRute).FirstOrDefault().IsKotaKota == true)
                    return "YES";
            }
            
            return null;
        }

        public decimal Harga(Context.SalesOrder dbso){
            Context.DaftarHargaOnCallItem dhocItem = context.DaftarHargaOnCallItem.Where(d => d.Id == dbso.SalesOrderOncall.IdDaftarHargaItem).FirstOrDefault();
            Context.DaftarHargaOnCall dhoc = context.DaftarHargaOnCall.Where(d => d.Id == dhocItem.IdDaftarHargaOnCall).FirstOrDefault();
            return dhocItem.Harga + dhoc.DaftarHargaOnCallKondisi.Where(d => d.IsInclude == true && d.IsBill == false && d.value > 0).Sum(d => d.value.Value);
        }

        public Context.Rute FindRute(int id)
        {
            string idRutes = context.DaftarHargaOnCallItem.Where(d => d.Id == id).FirstOrDefault().ListIdRute;
            string[] rutes = idRutes.Split(',');
            int rute = int.Parse(rutes[0]);
            return context.Rute.Where(d => d.Id == rute).FirstOrDefault();
        }

        public Context.DaftarHargaOnCallItem FindDH(int id)
        {
            return context.DaftarHargaOnCallItem.Where(d => d.Id == id).FirstOrDefault();
        }

        public double GroupPerMonth(DateTime valdate){
            DateTime firstDayOfMonth = new DateTime(valdate.Year, valdate.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            List<SalesOrderOncall> dboncall = context.SalesOrder.Where(d => d.SalesOrderOncallId != null).Select(d=>d.SalesOrderOncall).ToList();
            double totalHarga = 0;
            foreach (SalesOrderOncall dbso in dboncall.Where(d => d.TanggalMuat >= firstDayOfMonth && d.TanggalMuat <= lastDayOfMonth)) {
                double harga = double.Parse(context.DaftarHargaOnCallItem.Where(d => d.Id == dbso.IdDaftarHargaItem).FirstOrDefault().Harga.ToString());
                totalHarga = totalHarga + harga;
            };
            return totalHarga;
        }

        public string IsMuatDateExist(List<String> muatDate, int custId, int id = 0)
        {
            
            List<string> wadah = new List<string>();
                        
            foreach (string kituweh in muatDate)
            {
                int t = 0;
                DateTime dateNaon = DateTime.Parse(kituweh);
                if (id == 0)
                    t = context.SalesOrderKontrak.Where(k => k.CustomerId == custId && k.SalesOrderKontrakDetail.Any(v => v.MuatDate == dateNaon)).Count();
                else
                    t = context.SalesOrderKontrak.Where(k => k.CustomerId == custId && k.SalesOrderKontrakId == id && k.SalesOrderKontrakDetail.Any(v => v.MuatDate == dateNaon)).Count();
                    

                if (t != 0)
                    wadah.Add(kituweh);
            }

            return string.Join(",  ", wadah);            
        } 

        public List<Context.OrderHistory> FindAllAUJHistory(){
            return context.OrderHistory.Where(d => d.StatusFlow == 4).ToList();
        }

        public List<Context.OrderHistory> FindAllKonfirmasiHistory(){
            return context.OrderHistory.Where(d => d.StatusFlow == 3).ToList();
        }

        public List<Context.OrderHistory> FindAllPlanningHistory(){
            return context.OrderHistory.Where(d => d.StatusFlow == 2).ToList();
        }

        public Context.OrderHistory FindMarketingHistory(int idSO){
            return context.OrderHistory.Where(d => d.StatusFlow == 1 && d.SalesOrderId == idSO && d.ProcessedAt.ToString() != "01/01/0001 00.00.00").FirstOrDefault();
        }

        public Context.OrderHistory FindPlanningHistory(int idSO){
            return context.OrderHistory.Where(d => d.StatusFlow == 2 && d.SalesOrderId == idSO && d.ProcessedAt.ToString() != "01/01/0001 00.00.00").FirstOrDefault();
        }

        public Context.OrderHistory FindKonfirmasiHistory(int idSO){
            return context.OrderHistory.Where(d => d.StatusFlow == 3 && d.SalesOrderId == idSO && d.ProcessedAt.ToString() != "01/01/0001 00.00.00").FirstOrDefault();
        }

        public string TanggalTiba(Context.SalesOrderOncall so){
            try{
                return context.MonitoringDetailSo.Where(d => d.NoSo == so.SONumber).FirstOrDefault().TglTiba.ToString();
            }
            catch (Exception e){
                return "-";
            }
        }
    }
}