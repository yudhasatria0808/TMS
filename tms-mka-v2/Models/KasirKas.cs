using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using tms_mka_v2.Context;

namespace tms_mka_v2.Models
{
    public class Kasirkas
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public int? IdSalesOrder { get; set; }
        public int? IdRemoval { get; set; }
        public string DnNo { get; set; }
        public string SoNo { get; set; }
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
        public string Penerima { get; set; }
        public string strGrid { get; set; }
        public string JenisOrder { get; set; }
        public string ListIdSo { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Kasirkas()
        {

        }
        public Kasirkas(Context.SalesOrder dbitem)
        {
            if (dbitem.AdminUangJalanId.HasValue){
                Context.AdminUangJalanUangTf dbkas = dbitem.AdminUangJalan.AdminUangJalanUangTf.Where(n => n.Keterangan == "Tunai").FirstOrDefault();
                if (dbkas != null)
                {
                    IdSalesOrder = dbitem.Id;
                    if (dbitem.Status == "settlement")
                        Status = "Close";
                    else
                        Status = dbkas.isTf ? "Sudah" : "Belum";
                    Jumlah = dbkas.Value;
                    Realisasi = dbkas.JumlahTransfer;
                    Waktu = dbkas.TanggalAktual + dbkas.JamAktual;
                    Tanggal = dbkas.Tanggal;
                    Penerima = dbkas.IdDriverPenerima.HasValue ? dbkas.Driver.NamaDriver : "";
                    ModifiedDate = dbitem.DateStatus;
                    if (dbitem.SalesOrderOncallId.HasValue)
                    {
                        DnNo = dbitem.SalesOrderOncall.DN;
                        SoNo = dbitem.SalesOrderOncall.SONumber;
                        IdChild = dbitem.SalesOrderOncallId.Value;
                        IdDriver = dbitem.SalesOrderOncall.Driver1.KodeDriver;
                        Driver = dbitem.SalesOrderOncall.Driver1.NamaDriver;
                        VehicleNo = dbitem.SalesOrderOncall.DataTruck.VehicleNo;
                        KodeNama = dbitem.SalesOrderOncall.Customer.CustomerCodeOld;
                        Customer = dbitem.SalesOrderOncall.Customer.CustomerNama;
                        TanggalJalan = dbitem.SalesOrderOncall.TanggalMuat;
                    }
                    else if (dbitem.SalesOrderPickupId.HasValue)
                    {
                        DnNo = dbitem.SalesOrderPickup.SONumber;
                        IdChild = dbitem.SalesOrderPickupId.Value;
                        IdDriver = dbitem.SalesOrderPickup.Driver1.KodeDriver;
                        Driver = dbitem.SalesOrderPickup.Driver1.NamaDriver;
                        VehicleNo = dbitem.SalesOrderPickup.DataTruck.VehicleNo;
                        KodeNama = dbitem.SalesOrderOncall.Customer.CustomerCodeOld;
                        Customer = dbitem.SalesOrderPickup.Customer.CustomerNama;
                        TanggalJalan = dbitem.SalesOrderPickup.TanggalOrder;
                    }
                    else if (dbitem.SalesOrderProsesKonsolidasiId.HasValue)
                    {
                        DnNo = dbitem.SalesOrderProsesKonsolidasi.DN;
                        SoNo = dbitem.SalesOrderProsesKonsolidasi.SONumber;
                        IdChild = dbitem.SalesOrderProsesKonsolidasiId.Value;
                        IdDriver = dbitem.SalesOrderProsesKonsolidasi.Driver1.KodeDriver;
                        Driver = dbitem.SalesOrderProsesKonsolidasi.Driver1.NamaDriver;
                        VehicleNo = dbitem.SalesOrderProsesKonsolidasi.DataTruck.VehicleNo;
                        TanggalJalan = dbitem.SalesOrderProsesKonsolidasi.TanggalMuat;
                    }
                    else if (dbitem.SalesOrderKontrakId.HasValue)
                    {

                    }
                }
            }
        }
        public Kasirkas(Context.SalesOrder dbso, List<Context.SalesOrderKontrakListSo> dbitem)
        {
            Context.AdminUangJalanUangTf dbkas = dbitem.FirstOrDefault().AdminUangJalan.AdminUangJalanUangTf.Where(n => n.Keterangan == "Tunai").FirstOrDefault();
            IdSalesOrder = dbso.Id;
            if (dbitem.FirstOrDefault().Status == "settlement")
                Status = "Close";
            else
                Status = dbkas.isTf ? "Sudah" : "Belum";
            Jumlah = dbkas.Value;
            Realisasi = dbkas.JumlahTransfer;
            Waktu = dbkas.TanggalAktual + dbkas.JamAktual;
            Tanggal = dbkas.Tanggal;
            Penerima = dbkas.IdDriverPenerima.HasValue ? dbkas.Driver.NamaDriver : "";

            DnNo = "";
            SoNo = string.Join(", ", dbitem.Select(s => s.NoSo).ToList());
            IdDriver = dbitem.FirstOrDefault().Driver1.KodeDriver;
            Driver = dbitem.FirstOrDefault().Driver1.NamaDriver;
            VehicleNo = dbitem.FirstOrDefault().DataTruck.VehicleNo;
            KodeNama = dbso.SalesOrderKontrak.Customer.CustomerNama;
            Customer = dbso.SalesOrderKontrak.Customer.CustomerNama;
            //TanggalJalan = dbitem.SalesOrderOncall.TanggalMuat;
            JenisOrder = "Kontrak";
            ListIdSo = string.Join(".", dbitem.Select(d => d.Id.ToString()).ToList());
            ModifiedDate = dbso.DateStatus;
        }
        public Kasirkas(Context.Removal dbitem)
        {
            Context.RemovalUangTf dbkas = dbitem.RemovalUangTf.Where(n => n.Keterangan == "Tunai").FirstOrDefault();
            if (dbkas != null)
            {
                IdRemoval = dbitem.Id;
                IdSalesOrder = dbitem.IdSO;
                if (dbitem.Status == "settlement")
                    Status = "Close";
                else
                    Status = dbkas.isTf ? "Sudah" : "Belum";
                Jumlah = dbkas.Value;
                Realisasi = dbkas.JumlahTransfer;
                Waktu = dbkas.TanggalAktual + dbkas.JamAktual;
                Tanggal = dbkas.Tanggal;
                Penerima = dbkas.IdDriverPenerima.HasValue ? dbkas.Driver.NamaDriver : "";
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
                    TanggalJalan = dbitem.SalesOrder.SalesOrderPickup.TanggalOrder;
                }
                else if (dbitem.SalesOrder.SalesOrderProsesKonsolidasiId.HasValue)
                {
                    DnNo = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.DN;
                    SoNo = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.SONumber;
                    IdChild = dbitem.SalesOrder.SalesOrderProsesKonsolidasiId.Value;
                    IdDriver = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.Driver1.KodeDriver;
                    Driver = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.Driver1.NamaDriver;
                    VehicleNo = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.DataTruck.VehicleNo;
                    TanggalJalan = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.TanggalMuat;
                }
            }
        }
    }
}