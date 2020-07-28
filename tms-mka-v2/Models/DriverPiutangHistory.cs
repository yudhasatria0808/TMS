using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using tms_mka_v2.Context;

namespace tms_mka_v2.Models
{
    public class DriverPiutangHistory
    {
        #region field
        public string Keterangan { get; set; }
        public decimal? Jumlah { get; set; }
        public System.DateTime? Tanggal { get; set; }
        public decimal? Saldo { get; set; }
        public int? Id { get; set; }
        #endregion

        public DriverPiutangHistory()
        {

        }
        public DriverPiutangHistory(Context.pbyd_det dbitem, decimal saldo)
        {
            //dv
            Tanggal = dbitem.pbyd_dt;
            Jumlah = dbitem.pbyd_amount_pay;
            Keterangan = dbitem.pby_mstr.pby_remarks == null ? "Pencairan " + dbitem.pby_mstr.pby_code : dbitem.pby_mstr.pby_remarks;
            Saldo = Jumlah + saldo;
        }

        public DriverPiutangHistory(string penghibur, Context.cashd_det dbitem, decimal saldo)
        {
            //dv
            Tanggal = dbitem.cashd_dt;
            Jumlah = (dbitem.cashd_amount+dbitem.cashd_refund_amount)*-1;
            Keterangan = "Realisasi " + dbitem.pbyd_det.pby_mstr.pby_code;
            Saldo = Jumlah + saldo;
        }

        public DriverPiutangHistory(string penghibur, string fromKlaim, Context.Klaim dbitem, decimal saldo)
        {
            //klaim
            Tanggal = dbitem.TanggalPengajuan;
            Jumlah = decimal.Parse(dbitem.BebanClaimDriver.ToString());
            Keterangan = "Klaim " + dbitem.NoKlaim;
            Saldo = Jumlah + saldo;
            Id = dbitem.Id;
        }

        public void setDb(Context.pbyd_det dbitem)
        {
//            dbitem.pbyd_dt = pbyd_dt;
        }
    }
}