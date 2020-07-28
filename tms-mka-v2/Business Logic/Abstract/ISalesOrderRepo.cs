using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tms_mka_v2.Context;
using tms_mka_v2.Infrastructure;

namespace tms_mka_v2.Business_Logic.Abstract
{
    public interface ISalesOrderRepo
    {
        void save(SalesOrder dbitem);
        void saveUangTf(AdminUangJalanUangTf dbitem);
        List<SalesOrder> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        List<SalesOrder> FindAllOnCall(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        List<SalesOrder> FindAllKontrak(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        List<SalesOrder> FindAllPickUp(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        List<SalesOrder> FindAllKonsolidasi(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        List<SalesOrder> FindAllProsesKonsolidasi(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        List<SalesOrder> FindAllAdminUangJalan(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        List<SalesOrder> FindAllAdminDispatched(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        List<SalesOrder> FindAllKasir(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        List<SalesOrder> FindAllKlaim(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        List<SalesOrder> FindAllAUJReport(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        List<SalesOrder> FindAllKasirReport(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        int Count(FilterInfo filters = null);
        int CountKontrak(FilterInfo filters = null);
        int CountOncall(FilterInfo filters = null);
        int CountPickup(FilterInfo filters = null);
        int CountKonsolidasi(FilterInfo filters = null);
        int CountProsesKonsolidasi(FilterInfo filters = null);
        SalesOrder FindByPK(int id);
        SalesOrder FindByOnCall(int id);
        SalesOrder FindByKontrak(int id);
        SalesOrder FindByPickup(int id);
        SalesOrder FindByKonsolidasi(int id);
        SalesOrder FindByProsesKonsolidasi(int id);
        void delete(SalesOrder dbitem, int id);
        string generateCodeOnCall(DateTime valdate,int urutan);
        int getUrutanOnCAll(DateTime valdate);
        string generateCodeKontrak(int urutan);
        int getUrutanKontrak();
        string generatePickup(DateTime valdate,int urutan);
        int getUrutanPickup(DateTime valdate);
        string generateKonsolidasi(DateTime valdate,int urutan);
        int getUrutanKonsolidasi(DateTime valdate);
        string generateProsesKonsolidasi(DateTime valdate,int urutan);
        int getUrutanProsesKonsolidasi(DateTime valdate);
        string IsMuatDateExist(List<string> muatDate, int custId, int id = 0);
        string FindArea(int idSO);
        SalesOrder FindByOnCallCode(string number);
        List<OrderHistory> FindAllHistory(int idSo);
        Context.Rute FindRute(int id);
        decimal Harga(Context.SalesOrder dbso);
        double GroupPerMonth(DateTime valdate);
        List<Context.OrderHistory> FindAllPlanningHistory();
        Context.OrderHistory FindMarketingHistory(int idSO);
        Context.OrderHistory FindPlanningHistory(int idSO);
        List<Context.OrderHistory> FindAllKonfirmasiHistory();
        List<Context.OrderHistory> FindAllAUJHistory();
        Context.OrderHistory FindKonfirmasiHistory(int idSO);
        string TanggalTiba(Context.SalesOrderOncall so);
        Context.DaftarHargaOnCallItem FindDH(int id);
    }
}