using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using tms_mka_v2.Context;

namespace tms_mka_v2.Models
{
    public class KasirTf
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public int? IdSalesOrder { get; set; }
        public int? IdRemoval { get; set; }
        public string DnNo {get;set;}
        public string SoNo {get;set;}
        public int IdChild { get; set; }
        public string IdDriver { get; set; }
        public string Driver { get; set; }
        public string VehicleNo { get; set; }
        public string KodeNama { get; set; }
        public string Customer { get; set; }
        public DateTime? TanggalJalan { get; set; }
        public string Keterangan { get; set; }
        public DateTime? Tanggal { get; set; }
        public decimal? Jumlah { get; set; }
        public decimal? Realisasi { get; set; }
        public DateTime? Waktu { get; set; }
        public string strGrid { get; set; }
        public string JenisOrder { get; set; }
        public string ListIdSo { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public KasirTf()
        {

        }
        public KasirTf(Context.SalesOrder dbitem)
        {
            IdSalesOrder = dbitem.Id;
            Jumlah = 0; Realisasi = 0;
            if (dbitem.Status == "settlement")
                Status = "Close";
            else if ((dbitem.AdminUangJalan.AdminUangJalanUangTf.Where(s => s.Keterangan != "Tunai").Any(n => n.isTf == false)) && (dbitem.AdminUangJalan.AdminUangJalanUangTf.Where(s => s.Keterangan != "Tunai").Any(n => n.isTf == true)))
                Status =  "On Progress";
            else if (dbitem.AdminUangJalan.AdminUangJalanUangTf.Where(s => s.Keterangan != "Tunai").Any(n => n.isTf == false))
                Status =  "Belum";
            else
                Status =  "Sudah";

            Waktu = dbitem.AdminUangJalan.AdminUangJalanUangTf.Where(d => d.TanggalAktual != null).OrderByDescending(t => t.TanggalAktual).Select(t => t.TanggalAktual).FirstOrDefault();
            Tanggal = dbitem.AdminUangJalan.AdminUangJalanUangTf.Where(d => d.TanggalAktual != null).OrderByDescending(t => t.TanggalAktual).Select(t => t.Tanggal).FirstOrDefault();
            ModifiedDate = dbitem.DateStatus;
            if (dbitem.SalesOrderOncallId.HasValue)
            {
                DnNo = dbitem.SalesOrderOncall.DN;
                SoNo = dbitem.SalesOrderOncall.SONumber;
                IdChild = dbitem.SalesOrderOncallId.Value;
                IdDriver = dbitem.SalesOrderOncall.Driver1 == null ? "" : dbitem.SalesOrderOncall.Driver1.KodeDriver;
                Driver = dbitem.SalesOrderOncall.Driver1 == null ? "" : dbitem.SalesOrderOncall.Driver1.NamaDriver;
                VehicleNo = dbitem.SalesOrderOncall.DataTruck == null ? "" : dbitem.SalesOrderOncall.DataTruck.VehicleNo;
                KodeNama = dbitem.SalesOrderOncall.Customer.CustomerCodeOld;
                Customer = dbitem.SalesOrderOncall.Customer.CustomerNama;
                foreach (var item in dbitem.AdminUangJalan.AdminUangJalanUangTf.Where(n => n.Keterangan != "Tunai"))
	            {
                    Jumlah = Jumlah + (item.Value > 0 ? item.Value : 0);
                    Realisasi = Realisasi + item.JumlahTransfer;
	            }
                TanggalJalan = dbitem.SalesOrderOncall.TanggalMuat;
            }
            else if (dbitem.SalesOrderPickupId.HasValue)
            {
                DnNo = dbitem.SalesOrderPickup.SONumber;
                IdChild = dbitem.SalesOrderPickupId.Value;
                IdDriver = dbitem.SalesOrderPickup.Driver1.KodeDriver;
                Driver = dbitem.SalesOrderPickup.Driver1.NamaDriver;
                VehicleNo = dbitem.SalesOrderPickup.DataTruck.VehicleNo;
                KodeNama = dbitem.SalesOrderPickup.Customer.CustomerCodeOld;
                Customer = dbitem.SalesOrderPickup.Customer.CustomerNama;
                foreach (var item in dbitem.AdminUangJalan.AdminUangJalanUangTf.Where(n => n.Keterangan != "Tunai"))
                {
                    Jumlah = Jumlah + item.Value;
                    Realisasi = Realisasi + item.JumlahTransfer;
                }
                TanggalJalan = dbitem.SalesOrderPickup.TanggalPickup;
            }
            else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue)
            {
                DnNo = dbitem.SalesOrderProsesKonsolidasi.DN;
                SoNo = dbitem.SalesOrderProsesKonsolidasi.SONumber;
                IdChild = dbitem.SalesOrderProsesKonsolidasiId.Value;
                IdDriver = dbitem.SalesOrderProsesKonsolidasi.Driver1.KodeDriver;
                Driver = dbitem.SalesOrderProsesKonsolidasi.Driver1.NamaDriver;
                VehicleNo = dbitem.SalesOrderProsesKonsolidasi.DataTruck.VehicleNo;
                foreach (var item in dbitem.AdminUangJalan.AdminUangJalanUangTf.Where(n => n.Keterangan != "Tunai"))
                {
                    Jumlah = Jumlah + item.Value;
                    Realisasi = Realisasi + item.JumlahTransfer;
                }
                TanggalJalan = dbitem.SalesOrderProsesKonsolidasi.TanggalMuat;
            }
            else if (dbitem.SalesOrderKontrakId.HasValue)
            {

            }
        }
        public KasirTf(Context.SalesOrder dbso, List<Context.SalesOrderKontrakListSo> dbitem)
        {
            IdSalesOrder = dbso.Id;
            Jumlah = 0; Realisasi = 0;
            if (dbitem.FirstOrDefault().Status == "settlement")
                Status = "Close";
            else
                Status = dbitem.FirstOrDefault().AdminUangJalan.AdminUangJalanUangTf.Where(s => s.Keterangan != "Tunai").Any(n => n.isTf == false) ? "Belum" : "Sudah";
            JenisOrder = "Kontrak";
            Waktu = dbitem.FirstOrDefault().AdminUangJalan.AdminUangJalanUangTf.Where(d => d.TanggalAktual != null).OrderByDescending(t => t.TanggalAktual).Select(t => t.TanggalAktual).FirstOrDefault();
            Tanggal = dbitem.FirstOrDefault().AdminUangJalan.AdminUangJalanUangTf.Where(d => d.TanggalAktual != null).OrderByDescending(t => t.TanggalAktual).Select(t => t.Tanggal).FirstOrDefault();

            SoNo = string.Join(", ", dbitem.Select(s => s.NoSo).ToList());
            IdDriver = dbitem.FirstOrDefault().Driver1.KodeDriver;
            Driver = dbitem.FirstOrDefault().Driver1.NamaDriver;
            VehicleNo = dbitem.FirstOrDefault().DataTruck.VehicleNo;
            Customer = dbso.SalesOrderKontrak.Customer.CustomerNama;
            foreach (var item in dbitem.FirstOrDefault().AdminUangJalan.AdminUangJalanUangTf.Where(n => n.Keterangan != "Tunai"))
            {
                Jumlah = Jumlah + item.Value;
                Realisasi = Realisasi + item.JumlahTransfer;
            }
            ListIdSo = string.Join(".", dbitem.Select(d => d.Id.ToString()).ToList());
            ModifiedDate = dbso.DateStatus;
        }
        public KasirTf(Context.Removal dbitem)
        {
            IdSalesOrder = dbitem.IdSO;
            IdRemoval = dbitem.Id;
            Jumlah = 0; Realisasi = 0;
            if (dbitem.Status == "settlement")
                Status = "Close";
            else if ((dbitem.RemovalUangTf.Where(s => s.Keterangan != "Tunai").Any(n => n.isTf == false)) && (dbitem.RemovalUangTf.Where(s => s.Keterangan != "Tunai").Any(n => n.isTf == true)))
                Status = "On Progress";
            else if (dbitem.RemovalUangTf.Where(s => s.Keterangan != "Tunai").Any(n => n.isTf == false))
                Status = "Belum";
            else
                Status = "Sudah";

            Waktu = dbitem.RemovalUangTf.Where(d => d.TanggalAktual != null).OrderByDescending(t => t.TanggalAktual).Select(t => t.TanggalAktual).FirstOrDefault();
            Tanggal = dbitem.RemovalUangTf.Where(d => d.TanggalAktual != null).OrderByDescending(t => t.TanggalAktual).Select(t => t.Tanggal).FirstOrDefault();
            ModifiedDate = dbitem.ModifiedDate;
            if (dbitem.SalesOrder.SalesOrderOncallId.HasValue)
            {
                DnNo = dbitem.SalesOrder.SalesOrderOncall.DN;
                SoNo = dbitem.SalesOrder.SalesOrderOncall.SONumber;
                IdChild = dbitem.SalesOrder.SalesOrderOncallId.Value;
                IdDriver = dbitem.SalesOrder.SalesOrderOncall.Driver1.KodeDriver;
                Driver = dbitem.SalesOrder.SalesOrderOncall.Driver1.NamaDriver;
                VehicleNo = dbitem.SalesOrder.SalesOrderOncall.DataTruck.VehicleNo;
                Customer = dbitem.SalesOrder.SalesOrderOncall.Customer.CustomerNama;
                foreach (var item in dbitem.RemovalUangTf.Where(n => n.Keterangan != "Tunai"))
                {
                    Jumlah = Jumlah + item.Value;
                    Realisasi = Realisasi + item.JumlahTransfer;
                }
                TanggalJalan = dbitem.SalesOrder.SalesOrderOncall.TanggalMuat;
            }
            else if (dbitem.SalesOrder.SalesOrderPickupId.HasValue)
            {
                DnNo = dbitem.SalesOrder.SalesOrderPickup.SONumber;
                IdChild = dbitem.SalesOrder.SalesOrderPickupId.Value;
                IdDriver = dbitem.SalesOrder.SalesOrderPickup.Driver1.KodeDriver;
                Driver = dbitem.SalesOrder.SalesOrderPickup.Driver1.NamaDriver;
                VehicleNo = dbitem.SalesOrder.SalesOrderPickup.DataTruck.VehicleNo;
                Customer = dbitem.SalesOrder.SalesOrderPickup.Customer.CustomerNama;
                foreach (var item in dbitem.RemovalUangTf.Where(n => n.Keterangan != "Tunai"))
                {
                    Jumlah = Jumlah + item.Value;
                    Realisasi = Realisasi + item.JumlahTransfer;
                }
                TanggalJalan = dbitem.SalesOrder.SalesOrderPickup.TanggalPickup;
            }
            else if (dbitem.SalesOrder.SalesOrderProsesKonsolidasiId.HasValue)
            {
                DnNo = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.DN;
                SoNo = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.SONumber;
                IdChild = dbitem.SalesOrder.SalesOrderProsesKonsolidasiId.Value;
                IdDriver = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.Driver1.KodeDriver;
                Driver = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.Driver1.NamaDriver;
                VehicleNo = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.DataTruck.VehicleNo;
                foreach (var item in dbitem.RemovalUangTf.Where(n => n.Keterangan != "Tunai"))
                {
                    Jumlah = Jumlah + item.Value;
                    Realisasi = Realisasi + item.JumlahTransfer;
                }
                TanggalJalan = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.TanggalMuat;
            }
        }
    }
}