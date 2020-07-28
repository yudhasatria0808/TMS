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
using tms_mka_v2.Models;

namespace tms_mka_v2.Business_Logic.Concrete
{
    public class glt_detRepo : Iglt_detRepo
    {
        private ContextModelERP context = new ContextModelERP();
        public void save(int seq, Context.glt_det model, Context.SalesOrder soitem, string code, decimal? nominalDb, decimal? nominalCr, Models.ac_mstr ac_mstr){
            model.glt_oid = Guid.NewGuid().ToString();
            model.glt_dom_id = 1;
            model.glt_en_id = 1;
            model.glt_add_by = "";
            model.glt_add_date = DateTime.Now;
            model.glt_upd_by = "";
            model.glt_upd_date = DateTime.Now;
            model.glt_code = code;
            model.glt_date = DateTime.Now;//soitem.SalesOrderOncallId.HasValue ? soitem.SalesOrderOncall.TanggalMuat : soitem.SalesOrderProsesKonsolidasiId.HasValue ? soitem.SalesOrderProsesKonsolidasi.TanggalMuat : soitem.SalesOrderPickupId.HasValue ? soitem.SalesOrderPickup.TanggalPickup : DateTime.Now;
            model.glt_type = "SO"; //?
            model.glt_cu_id = 1;
            model.glt_exc_rate = 1;
            model.glt_seq = seq;
            model.glt_ac_id = ac_mstr.id;
            model.glt_cc_id = 0;
            model.glt_sb_id = 1;
            model.glt_desc = ac_mstr.ac_name;
            model.glt_debit = nominalDb;
            model.glt_credit = nominalCr;
            model.glt_posted = "N";
            model.glt_dt = DateTime.Now;
            model.glt_branch_id = 10001;
            model.glt_driver_id = soitem.SalesOrderOncallId.HasValue ? (soitem.SalesOrderOncall.Driver1Id.HasValue ? soitem.SalesOrderOncall.Driver1Id.Value + 7000000 : 0) : soitem.SalesOrderProsesKonsolidasiId.HasValue ? (soitem.SalesOrderProsesKonsolidasi.Driver1Id.HasValue ? soitem.SalesOrderProsesKonsolidasi.Driver1Id.Value + 7000000 : 0) : soitem.SalesOrderPickupId.HasValue ? (soitem.SalesOrderPickup.Driver1Id.HasValue ? soitem.SalesOrderPickup.Driver1Id.Value + 7000000 : 0) : 0;
            model.glt_no_pol = soitem.SalesOrderOncallId.HasValue ? (soitem.SalesOrderOncall.DataTruck == null ? null : soitem.SalesOrderOncall.DataTruck.VehicleNo) : soitem.SalesOrderProsesKonsolidasiId.HasValue ? (soitem.SalesOrderProsesKonsolidasi.DataTruck == null ? null : soitem.SalesOrderProsesKonsolidasi.DataTruck.VehicleNo) : soitem.SalesOrderPickupId.HasValue ? (soitem.SalesOrderPickup.DataTruck == null ? null : soitem.SalesOrderPickup.DataTruck.VehicleNo) : null;
            context.glt_det.Add(model);
            context.SaveChanges();
        }

        public void saveSoMstr(SalesOrder soitem, string username)
        {
            Context.so_mstr model = new Context.so_mstr();
            Context.Customer dbCustomer = soitem.SalesOrderOncallId.HasValue ? soitem.SalesOrderOncall.Customer : soitem.SalesOrderPickupId.HasValue ? soitem.SalesOrderPickup.Customer : soitem.SalesOrderPickup.Customer;
            model.so_oid = Guid.NewGuid().ToString();
            model.so_dom_id = 1;
            model.so_en_id = 1;
            model.so_add_by = username;
            model.so_add_date = DateTime.Now;
            model.so_upd_by = username;
            model.so_upd_date = DateTime.Now;
            model.so_code = soitem.SalesOrderOncallId.HasValue ? soitem.SalesOrderOncall.SONumber : soitem.SalesOrderProsesKonsolidasiId.HasValue ? soitem.SalesOrderProsesKonsolidasi.SONumber : soitem.SalesOrderPickupId.HasValue ? soitem.SalesOrderPickup.SONumber : soitem.SalesOrderPickup.SONumber;
            model.so_ptnr_id_sold = dbCustomer == null ? 0 : dbCustomer.Id;
            model.so_ptnr_id_bill = dbCustomer == null ? 0 : dbCustomer.Id;
            model.so_date = soitem.SalesOrderOncallId.HasValue ? soitem.SalesOrderOncall.TanggalOrder : soitem.SalesOrderPickupId.HasValue ? soitem.SalesOrderPickup.TanggalOrder : soitem.SalesOrderPickup.TanggalOrder;
            //public string so_credit_term { get; set; }//,   Customer Term of Payment
            model.so_taxable = dbCustomer != null && dbCustomer.CustomerPPN != null && dbCustomer.CustomerPPN.FirstOrDefault().PPN == true ? "Y" : "N";// Checkbox
            model.so_tax_class = 0;//, from code_mstr where code_field ~~* 'taxclass_mstr'
            model.so_si_id = 1;
            model.so_type = "R";
            model.so_sales_person = 0;//,  from ptnr_mstr where ptnr_is_member ~~* 'Y' 
            model.so_pi_id = 0;//, from pi_mstr
            /*model.so_pay_type = 0;//,  from code_mstr where code_field ~~* 'payment_type'
            model.so_pay_method = 0;// ,   from code_mstr where code_field ~~* 'payment_methode'
            model.so_ar_ac_id = 0; //, From pla_mstr.pla_ac_id partnumber --> pt_mstr.pt_pl_id =pla_mstr.pl_id)
            model.so_ar_sb_id = 0;//,  From pla_mstr.pla_sb_id partnumber --> pt_mstr.pt_pl_id =pla_mstr.pl_id)
            model.so_ar_cc_id = 0;//   From pla_mstr.pla_cc_id partnumber --> pt_mstr.pt_pl_id =pla_mstr.pl_id)
            model.so_dp = 0;*///,    nominal yg didapat dr form InputDP
            model.so_disc_header = 0;
            model.so_total = 0;//, Total SO
            model.so_tran_id = 0;
            model.so_trans_id = "D";
            model.so_trans_rmks = "0";//,    SO Approval Remarks Null)
/*            model.so_current_route = 0;//, Current City Route
            model.so_next_route = 0;//,    Next City Route
            model.so_dt = DateTime.now;
            model.so_cu_id = 0;//, from cu_mstr.cu_id currency)
            model.so_total_ppn = 0;//, Total PPN From Detail SO
            model.so_total_pph = 0;//, Total PPH From Detail SO
            model.so_payment = 0;
            model.so_exc_rate = 0;//,  kurs mata uang From exr_rate current Date and Current Currency
            model.so_tax_inc = 0;//,   Y or N termasuk pajak atau belum, checkbox)
            model.so_cons = 0;//,  Y or N konsinyasi atau bukan, checkbox)
            model.so_terbilang = 0;//, Terbilang dalam bahasa indonesia
            model.so_bk_id = 0;//, from bk_mstr.bk_id Nama Bank)
            model.so_interval = 0;//,  from code_mstr where code_field ~~* 'payment_type'  kolom code_user_1
            model.so_ppn_type = 0;//,  E=PPN BEBAS, A = PPN BAYAR, N = NON
            model.so_branch_id = 0;//, SO Branch from branch_mstr.branch_id)
            model.so_group_id = 0;//,  from code_mstr where code_field ~~* 'so_group'
            model.so_exc_rate_pi = 0;
            model.so_prct_limit_return = 0;
            model.so_disc_type = 0;
            model.so_vehicle = model.DataTruck.VehicleNo;
            model.so_departure_date = soitem.SalesOrderOncallId.HasValue ? soitem.SalesOrderOncall.TanggalMuat : soitem.SalesOrderProsesKonsolidasiId.HasValue ? soitem.SalesOrderProsesKonsolidasi.TanggalMuat : soitem.SalesOrderPickupId.HasValue ? soitem.SalesOrderPickup.TanggalMuat : soitem.SalesOrderPickup.TanggalMuat;
            model.so_start_route = 0;//,   Kota Awal Berangkat from code_mstr where code_field ~~* 'route_mstr'
            model.so_destination_route = 0;//  kota Tujuan from code_mstr where code_field ~~* 'route_mstr'*/
            context.so_mstr.Add(model);
            context.SaveChanges();
        }

        public void saveFromAc(int seq, string code, decimal? nominalDb, decimal? nominalCr, Models.ac_mstr ac_mstr, string desc=null){
            Context.glt_det model = new Context.glt_det();
            model.glt_oid = Guid.NewGuid().ToString();
            model.glt_dom_id = 1;
            model.glt_en_id = 1;
            model.glt_add_by = "";
            model.glt_add_date = DateTime.Now;
            model.glt_upd_by = "";
            model.glt_upd_date = DateTime.Now;
            model.glt_code = code;
            model.glt_date = DateTime.Now;
            model.glt_type = "SO"; //?
            model.glt_cu_id = 1;
            model.glt_exc_rate = 1;
            model.glt_seq = seq;
            model.glt_ac_id = ac_mstr.id;
            model.glt_cc_id = 0;
            model.glt_sb_id = 1;
            model.glt_desc = desc == null ? ac_mstr.ac_name : desc;
            model.glt_debit = nominalDb;
            model.glt_credit = nominalCr;
            model.glt_posted = "N";
            model.glt_dt = DateTime.Now;
            model.glt_branch_id = 10001;
            try{
                model.glt_daybook = "TMS-" + code.Split('-')[0] + "-" + code.Split('-')[1];//TMS-<TIGAKODEJURNAL><TYPE SO>;
            }
            catch (Exception e)
            {
            }
            context.glt_det.Add(model);
            context.SaveChanges();
        }

        public void saveFromBk(int seq, string code, decimal? nominalDb, decimal? nominalCr, Models.bk_mstr bk_mstr){
            Context.glt_det model = new Context.glt_det();
            model.glt_oid = Guid.NewGuid().ToString();
            model.glt_dom_id = 1;
            model.glt_en_id = 1;
            model.glt_add_by = "";
            model.glt_add_date = DateTime.Now;
            model.glt_upd_by = "";
            model.glt_upd_date = DateTime.Now;
            model.glt_code = code;
            model.glt_date = DateTime.Now;//soitem.SalesOrderOncallId.HasValue ? soitem.SalesOrderOncall.TanggalMuat : soitem.SalesOrderProsesKonsolidasiId.HasValue ? soitem.SalesOrderProsesKonsolidasi.TanggalMuat : soitem.SalesOrderPickupId.HasValue ? soitem.SalesOrderPickup.TanggalPickup : DateTime.Now;
            model.glt_type = "SO"; //?
            model.glt_cu_id = 1;
            model.glt_exc_rate = 1;
            model.glt_seq = seq;
            model.glt_ac_id = bk_mstr.id;
            model.glt_cc_id = 0;
            model.glt_sb_id = 1;
            model.glt_desc = bk_mstr.bk_name;
            model.glt_debit = nominalDb;
            model.glt_credit = nominalCr;
            model.glt_posted = "N";
            model.glt_dt = DateTime.Now;
            model.glt_branch_id = 10001;
            context.glt_det.Add(model);
            context.SaveChanges();
        }
    }
}