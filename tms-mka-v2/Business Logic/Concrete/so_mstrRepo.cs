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
    public class so_mstrRepo : Iso_mstrRepo
    {
      private ContextModelERP context = new ContextModelERP();
      private ContextModel contextBiasa = new ContextModel();
      public void saveSoMstr(SalesOrder soitem, string username, string guid, int customerId, decimal harga)
      {
            Context.so_mstr model = new Context.so_mstr();
            Context.Customer dbCustomer = soitem.SalesOrderOncallId.HasValue ? soitem.SalesOrderOncall.Customer : soitem.SalesOrderPickupId.HasValue ? soitem.SalesOrderPickup.Customer : soitem.SalesOrderPickup.Customer;
            model.so_oid = guid;
            model.so_dom_id = 1;
            model.so_en_id = 1;
            model.so_add_by = username;
            model.so_add_date = DateTime.Now;
            model.so_upd_by = username;
            model.so_upd_date = DateTime.Now;
            model.so_code = soitem.SalesOrderOncallId.HasValue ? soitem.SalesOrderOncall.SONumber : soitem.SalesOrderProsesKonsolidasiId.HasValue ? soitem.SalesOrderProsesKonsolidasi.SONumber : soitem.SalesOrderPickupId.HasValue ? soitem.SalesOrderPickup.SONumber : soitem.SalesOrderPickup.SONumber;
            model.so_ptnr_id_sold = customerId;
            model.so_ptnr_id_bill = customerId;
            model.so_date = soitem.SalesOrderOncallId.HasValue ? soitem.SalesOrderOncall.TanggalMuat.Value : soitem.SalesOrderPickupId.HasValue ? soitem.SalesOrderPickup.TanggalPickup : soitem.SalesOrderPickup.TanggalPickup;
            model.so_credit_term = 991025;//dbCustomer == null ? 0 : dbCustomer.CustomerCreditStatus.FirstOrDefault().ValueOverdue2.Value;//,   Customer Term of Payment
            model.so_taxable = dbCustomer != null && dbCustomer.CustomerPPN != null && dbCustomer.CustomerPPN.FirstOrDefault().PPN == true ? "Y" : "N";// Checkbox
            model.so_tax_class = 0;//, from code_mstr where code_field ~~* 'taxclass_mstr'
            model.so_si_id = 1;
            model.so_type = "R";
//            model.so_sales_person = soitem.SalesOrderOncallId.HasValue ? soitem.SalesOrderOncall.Driver1Id.Value : soitem.SalesOrderPickupId.HasValue ? soitem.SalesOrderPickup.Driver1Id.Value : soitem.SalesOrderPickup.Driver1Id.Value;//0;//,  from ptnr_mstr where ptnr_is_member ~~* 'Y' 
            model.so_pi_id = 101;//, from pi_mstr
            model.so_pay_type = "9942";//,  from code_mstr where code_field ~~* 'payment_type'
            model.so_pay_method = 99290;//"0";// ,   from code_mstr where code_field ~~* 'payment_methode'
            model.so_ar_ac_id = 0; //, From pla_mstr.pla_ac_id partnumber --> pt_mstr.pt_pl_id =pla_mstr.pl_id)
            /*model.so_ar_sb_id = 0;//,  From pla_mstr.pla_sb_id partnumber --> pt_mstr.pt_pl_id =pla_mstr.pl_id)
            model.so_ar_cc_id = 0;//   From pla_mstr.pla_cc_id partnumber --> pt_mstr.pt_pl_id =pla_mstr.pl_id)
            */model.so_dp = 0;//,    nominal yg didapat dr form InputDP
            model.so_disc_header = 0;
            model.so_total = harga;//, Total SO
            model.so_tran_id = 0;
            model.so_trans_id = "D";
            model.so_dt = DateTime.Now;
            model.so_cu_id = 1;//, from cu_mstr.cu_id currency)
            model.so_total_ppn = 0;//, Total PPN From Detail SO
            model.so_total_pph = 0;//, Total PPH From Detail SO
            model.so_payment = 0;
            model.so_exc_rate = 1;//,  kurs mata uang From exr_rate current Date and Current Currency
            model.so_tax_inc = "N";//,   Y or N termasuk pajak atau belum, checkbox)
            model.so_cons = "N";//,  Y or N konsinyasi atau bukan, checkbox)
            model.so_terbilang = "N";//, Terbilang dalam bahasa indonesia
            model.so_bk_id = 0;//, from bk_mstr.bk_id Nama Bank)
            model.so_interval = 1;//,  from code_mstr where code_field ~~* 'payment_type'  kolom code_user_1
            model.so_ppn_type = "A";//,  E=PPN BEBAS, A = PPN BAYAR, N = NON
            model.so_branch_id = 10001;//, SO Branch from branch_mstr.branch_id)
            model.so_group_id = 991414;//,  from code_mstr where code_field ~~* 'so_group'
            model.so_exc_rate_pi = 0;
            model.so_prct_limit_return = 0;
            model.so_disc_type = 0;
            model.so_departure_date = soitem.SalesOrderOncallId.HasValue ? soitem.SalesOrderOncall.TanggalMuat : soitem.SalesOrderProsesKonsolidasiId.HasValue ? soitem.SalesOrderProsesKonsolidasi.TanggalMuat : soitem.SalesOrderPickupId.HasValue ? soitem.SalesOrderPickup.TanggalPickup : soitem.SalesOrderPickup.TanggalPickup;
            model.so_start_route = "0";//,   Kota Awal Berangkat from code_mstr where code_field ~~* 'route_mstr'
            model.so_destination_route = "0";//  kota Tujuan from code_mstr where code_field ~~* 'route_mstr'*/
            context.so_mstr.Add(model);
            context.SaveChanges();
      }

      public void UpdateSoMstrVehicle(so_mstr dbitem)
      {
            context.so_mstr.Attach(dbitem);
            var entry = context.Entry(dbitem);
            entry.State = EntityState.Modified;
            context.SaveChanges();
      }

      public void saveSoShipMstr(SalesOrder soitem, string username, string guid, string ship_guid)
        {
            Context.soship_mstr model = new Context.soship_mstr();
            model.soship_oid = ship_guid; // uuid NOT NULL, Random uid
            model.soship_dom_id = 1;
            model.soship_en_id = 1;
            model.soship_add_by = username;
            model.soship_add_date = DateTime.Now;
            model.soship_upd_by = username;
            model.soship_upd_date = DateTime.Now;
            model.soship_code = soitem.SalesOrderOncallId.HasValue ? soitem.SalesOrderOncall.SONumber : soitem.SalesOrderProsesKonsolidasiId.HasValue ? soitem.SalesOrderProsesKonsolidasi.SONumber : soitem.SalesOrderPickupId.HasValue ? soitem.SalesOrderPickup.SONumber : soitem.SalesOrderPickup.SONumber;
            model.soship_date = soitem.SalesOrderOncallId.HasValue ? soitem.SalesOrderOncall.TanggalMuat : soitem.SalesOrderProsesKonsolidasiId.HasValue ? soitem.SalesOrderProsesKonsolidasi.TanggalMuat : soitem.SalesOrderPickupId.HasValue ? soitem.SalesOrderPickup.TanggalPickup : soitem.SalesOrderPickup.TanggalPickup;// date,    tgl settlement
            model.soship_so_oid = guid;
            model.soship_si_id = 1;
            model.soship_is_shipment = "Y";
            model.soship_dt = DateTime.Now;
            model.soship_exc_rate = 1;
            model.soship_cu_id = 1;
            model.soship_branch_id = 10001;
            model.soship_expeditur = 0;
            context.soship_mstr.Add(model);
            context.SaveChanges();
      }

      public void saveSoShipDet(SalesOrder soitem, string username, string guid, string ship_guid)
        {
            Context.soshipd_det model = new Context.soshipd_det();
            model.soshipd_oid = Guid.NewGuid().ToString(); // uuid NOT NULL, Random uid
            model.soshipd_soship_oid = guid;//  uuid,     shipmstr
            model.soshipd_sod_oid = ship_guid;//  uuid,  so_det
            model.soshipd_seq = 0;
            model.soshipd_qty = -1;
            model.soshipd_um = 991403;
            model.soshipd_um_conv = 1;
            model.soshipd_qty_real = -1;
            model.soshipd_si_id = 1;
            model.soshipd_loc_id = 10001;;
            model.soshipd_dt = DateTime.Now;
            model.soshipd_qty_inv = 0;
            model.soshipd_close_line = "N";
            model.soshipd_qty_allocated = 0;
            model.soshipd_pieces = 0;
            model.soshipd_remarks = soitem.SalesOrderOncallId.HasValue ? soitem.SalesOrderOncall.Keterangan : soitem.SalesOrderProsesKonsolidasiId.HasValue ? "" : soitem.SalesOrderPickupId.HasValue ? soitem.SalesOrderPickup.Keterangan : soitem.SalesOrderPickup.Keterangan; //  character varying100),   remak biasa we
            model.soshipd_packaging_id = 0;
            model.soshipd_qty_packaging = 0;
            model.soshipd_qty_plus = 0;
            model.soshipd_qty_minus = 0;
            model.soshipd_shipcust = 0;
            context.soshipd_det.Add(model);
            context.SaveChanges();
      }

      public so_mstr FindByPK(string code)
      {
            return context.so_mstr.Where(d => d.so_code == code).FirstOrDefault();
      }

      public so_mstr FindSoDet(string code)
      {
            return context.so_mstr.Where(d => d.so_code == code).FirstOrDefault();
//            return context.so_mstr.Where(d => d.so_code == code).FirstOrDefault().sod_det.FirstOrDefault();
      }

      public void saveSoDet(SalesOrder soitem, string username, string guid, string ship_guid)
      {
            Context.sod_det model = new Context.sod_det();
            model.sod_oid = ship_guid; // uuid NOT NULL, Random uid
            model.sod_dom_id = 1; //  smallint,  1
            model.sod_qty_shipment = 1;
            model.sod_en_id = 1; //  integer,    1
            model.sod_add_by = username; //  character varying25),  current_user.name
            model.sod_add_date = DateTime.Now; //  timestamp without time zone,     DateTime.Now
            model.sod_upd_by = username;//  character varying25),  current_user.name
            model.sod_upd_date = DateTime.Now; //  timestamp without time zone,     DateTime.Now
            model.sod_so_oid = guid; //  uuid,      so_mstr.so_uid
            model.sod_seq = 1; //  smallint,     urutan sod_det dlm 1 so_mstr
            model.sod_is_additional_charge = "N"; //  character1),   0
            model.sod_si_id = 1; //  integer DEFAULT 1,
            model.sod_pt_id = 10100; // ,      9999999
            model.sod_rmks = soitem.SalesOrderOncallId.HasValue ? soitem.SalesOrderOncall.Keterangan : soitem.SalesOrderProsesKonsolidasiId.HasValue ? "" : soitem.SalesOrderPickupId.HasValue ? soitem.SalesOrderPickup.Keterangan : soitem.SalesOrderPickup.Keterangan; //  character varying100),   remak biasa we
            model.sod_qty = 1; //  numeric18,8), banyaknya truk yg disewa
            model.sod_qty_allocated = 0; //  numeric18,8) DEFAULT 0,  0
            model.sod_qty_picked = 0; //  numeric18,8),   0
            model.sod_qty_pending_inv = 0; //  numeric18,8),    0
            model.sod_um = 0; //  integer,    0
            model.sod_cost = 0; //  numeric26,8),   0
            model.sod_price = contextBiasa.DaftarHargaOnCallItem.Where(d => d.Id == soitem.SalesOrderOncall.IdDaftarHargaItem).FirstOrDefault().Harga; //  numeric26,8),  kl dh ada harganya harga masuk k sini
            model.sod_disc = 0;
            model.sod_sales_ac_id = 0;
            model.sod_sales_sb_id = 0;
            model.sod_sales_cc_id = 0;
            model.sod_disc_ac_id = 0;
            model.sod_um_conv = 1; //  numeric18,8),      1
/*            model.sod_qty_real =  //  numeric18,8),       sod_qty numeric18,8),
            model.sod_taxable =  //  character1), Customer.PPN Yes ? 'Y' : 'N'
  */          model.sod_tax_inc = "N"; //  character1), N
    //        model.sod_tax_class =  //  integer,   Customer.PPN Yes ? '991404' : '1034'
            model.sod_dt = DateTime.Now; //  without time zone,      biasa we
            model.sod_payment = 0; //  numeric26,8), -- ini untuk angsuran/bulan  0
            model.sod_dp = 0; //  numeric26,8),     0
            model.sod_sales_unit = 0; //  numeric26,8),   0
            model.sod_loc_id = 10001; //  integer,      0
            model.sod_serial = 0; //  character varying100),    0
            model.sod_qty_return = 0; //  numeric26,8),   0
            model.sod_ppn_type = "A"; //  character1),      A
            model.sod_ppn = 0; //  numeric26,8) DEFAULT 0,      0
            model.sod_pph = 0; //  numeric26,8) DEFAULT 0,      0
            model.sod_price_line =  //  numeric26,8),     sod_qty numeric18,8),
            model.sod_disc1 = 0; //  numeric11,8) DEFAULT 0,    0
            model.sod_disc2 = 0; //  numeric11,8) DEFAULT 0,    0
            model.sod_packaging_id = 0; //  integer NOT NULL DEFAULT 0,     0
            model.sod_qty_packaging = 0; //  numeric26,8) DEFAULT 0,  0
            model.sod_qty_add = 0; //  numeric26,8) DEFAULT 0,  0
            context.sod_det.Add(model);
            context.SaveChanges();
        }
    }
}