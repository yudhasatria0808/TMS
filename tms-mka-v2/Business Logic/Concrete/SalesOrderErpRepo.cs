using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Data.Entity;
using tms_mka_v2.Business_Logic.Abstract;
using tms_mka_v2.Infrastructure;
using tms_mka_v2.Linq;
using tms_mka_v2.DataserErp;

namespace tms_mka_v2.Business_Logic.Concrete
{
    public class SalesOrderErpRepo : ISalesOrderErpRepo
    {
        private DataserErp.DsErpTableAdapters.ac_mstrTableAdapter dta = new DataserErp.DsErpTableAdapters.ac_mstrTableAdapter();

        //public void save(string user)
        //{ 
        //    Guid so_oid = new Guid();

        //    //dta.Insert(so_oid,1,user,DateTime.Now,user,DateTime.Now);
        //}
    }
}