using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tms_mka_v2.Context;
using tms_mka_v2.Infrastructure;
using tms_mka_v2.Models;

namespace tms_mka_v2.Business_Logic.Abstract
{
    public interface Iglt_detRepo
    {
		void save(int seq, Context.glt_det model, Context.SalesOrder soitem, string code, decimal? nominalCr, decimal? nominalDb, Models.ac_mstr ac_mstr);
		void saveFromBk(int seq, string code, decimal? nominalDb, decimal? nominalCr, Models.bk_mstr bk_mstr);
		void saveFromAc(int seq, string code, decimal? nominalCr, decimal? nominalDb, Models.ac_mstr ac_mstr, string desc=null);
		void saveSoMstr(SalesOrder soitem, string username);
    }
}