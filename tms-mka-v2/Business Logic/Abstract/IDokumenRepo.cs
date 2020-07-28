using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tms_mka_v2.Context;
using tms_mka_v2.Infrastructure;

namespace tms_mka_v2.Business_Logic.Abstract
{
    public interface IDokumenRepo
    {
        void save(Dokumen dbitem, int id, string strQuery=null);
        List<Dokumen> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        int Count(FilterInfo filters = null);
        Dokumen FindByPK(int id);
        Dokumen FindBySO(int idSO);
    }
}