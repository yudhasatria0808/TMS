using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using tms_mka_v2.Context;

namespace tms_mka_v2.Models
{
    public class SettlementRegIndex
    {
        public int Id { get; set; }
        public int? IdSalesOrder { get; set; }
        public string JenisOrder { get; set; }
        public string NoSo { get; set; }
        public string Customer { get; set; }
        public string VehicleNo { get; set; }
        public string Driver { get; set; }
        public DateTime TanggalMuat { get; set; }
        public Decimal? TotalTambahan { get; set; }
        public string ListIdSo { get; set; }

        public SettlementRegIndex()
        {

        }
        public SettlementRegIndex(Context.SettlementReguler dbitem)
        {
            Id = dbitem.Id;
            IdSalesOrder = dbitem.IdSalesOrder;
            if (dbitem.SalesOrder.SalesOrderOncallId.HasValue) {
                JenisOrder = "Oncall";
                NoSo = dbitem.SalesOrder.SalesOrderOncall.SONumber;
                Customer = dbitem.SalesOrder.SalesOrderOncall.Customer.CustomerNama;
                VehicleNo = dbitem.SalesOrder.SalesOrderOncall.DataTruck.VehicleNo;
                Driver = dbitem.SalesOrder.AdminUangJalan.Driver1.NamaDriver;
                TanggalMuat = dbitem.SalesOrder.SalesOrderOncall.TanggalMuat.Value;
            }
            else if (dbitem.SalesOrder.SalesOrderPickupId.HasValue) {
                JenisOrder = "Pickup";
                NoSo = dbitem.SalesOrder.SalesOrderPickup.SONumber;
                Customer = dbitem.SalesOrder.SalesOrderPickup.Customer.CustomerNama;
                VehicleNo = dbitem.SalesOrder.SalesOrderPickup.DataTruck.VehicleNo;
                Driver = dbitem.SalesOrder.AdminUangJalan.Driver1.NamaDriver;
                TanggalMuat = dbitem.SalesOrder.SalesOrderPickup.TanggalPickup;
            }
            else if (dbitem.SalesOrder.SalesOrderProsesKonsolidasiId.HasValue)
            {
                JenisOrder = "Konsolidasi";
                NoSo = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.SONumber;
                Customer = "";
                VehicleNo = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.DataTruck.VehicleNo;
                Driver = dbitem.SalesOrder.AdminUangJalan.Driver1.NamaDriver;
                TanggalMuat = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.TanggalMuat.Value;
            }
            ListIdSo = "0";
            TotalTambahan = dbitem.SettlementRegulerTambahanBiaya.Sum(d => d.Value);
        }
        public SettlementRegIndex(Context.SettlementReguler dbitem, List<Context.SalesOrderKontrakListSo> dbso)
        {
            Id = dbitem.Id;
            IdSalesOrder = dbitem.SalesOrder.Id;
            JenisOrder = "Kontrak";
            NoSo = string.Join(", ", dbso.Select(s => s.NoSo).ToList());
            Customer = dbitem.SalesOrder.SalesOrderKontrak.Customer.CustomerNama;
            VehicleNo = dbso.FirstOrDefault().DataTruck.VehicleNo;
            Driver = dbso.FirstOrDefault().Driver1.NamaDriver;
            TotalTambahan = dbitem.SettlementRegulerTambahanBiaya.Sum(d => d.Value);
            ListIdSo = dbitem.LisSoKontrak;
            TanggalMuat = dbso.OrderBy(d => d.MuatDate).FirstOrDefault().MuatDate;
        }
    }
}