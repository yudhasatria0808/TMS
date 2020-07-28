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
    public class gr_mstrRepo : Igr_mstrRepo
    {
        private ContextModelERP context = new ContextModelERP();

        public void save(int? total, Customer customer, string username, string code)
        {
            gr_mstr model = new gr_mstr();
            model.gr_oid = Guid.NewGuid().ToString();
            model.gr_dom_id = 1;
            model.gr_en_id = 1;
            model.gr_add_by = username;
            model.gr_add_date = DateTime.Now;
            model.gr_ptnr_id = customer.Id;
            model.gr_code = code;
            model.gr_date = DateTime.Now;
            model.gr_tax_basis_amount = total;
            model.gr_tax_amount = total;
            model.gr_total = total;
            model.gr_ptnr_code = customer.CustomerCode;
            model.gr_branch_id = 10001;
            context.gr_mstr.Add(model);
            context.SaveChanges();
        }
    }
}