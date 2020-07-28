using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tms_mka_v2.Context;
using tms_mka_v2.Infrastructure;

namespace tms_mka_v2.Business_Logic.Abstract
{
    public interface ISalesOrderKontrakListSoRepo
    {
        void save(SalesOrderKontrakListSo dbitem);
        SalesOrderKontrakListSo FindByPK(int id);
        void OnlyAdd(SalesOrderKontrakListSo dbitem);
        void deleteAdd(int? id);
        void returnListSo(int? id);
        string generateCodeListSo(string NoKontrak, DateTime valdate, int rit, int urutan, int urutanInduk);
        int getUrutanProses(int? id);
        List<SalesOrderKontrakListSo> returnListSoBatalTruckOnly();
        SalesOrderKontrakListSo FindByNoSo(string noso);
        void OnlyUpdate(SalesOrderKontrakListSo dbitem);
    }
}
