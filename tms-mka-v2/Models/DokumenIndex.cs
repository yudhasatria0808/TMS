using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using tms_mka_v2.Context;

namespace tms_mka_v2.Models
{
    public class DokumenIndex
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string NoSo { get; set; }
        public string VehicleNo { get; set; }
        public string JnsTruck { get; set; }
        public string Customer { get; set; }
        public string Rute { get; set; }
        public DateTime? TanggalMuat { get; set; }
        public DateTime LastUpdate { get; set; }
        public int Delay { get; set; }
        public string Lengkap { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsReturn { get; set; }
        public List<DokumenItem> ListDokumen { get; set; }
        public string KodeNama { get; set; }
        public string NamaDriver { get; set; }

        public DokumenIndex() { }
        public DokumenIndex(Context.Dokumen dbitem)
        {
            Id = dbitem.Id; 
            IsAdmin = dbitem.IsAdmin;
            Status = dbitem.IsComplete ? "Close" : "Open";
            if (dbitem.SalesOrder.SalesOrderOncallId.HasValue)
            {
                NoSo = dbitem.SalesOrder.SalesOrderOncall.SONumber;
                VehicleNo = dbitem.SalesOrder.SalesOrderOncall.DataTruck == null ? "" : dbitem.SalesOrder.SalesOrderOncall.DataTruck.VehicleNo;
                JnsTruck = dbitem.SalesOrder.SalesOrderOncall.DataTruck == null ? "" : dbitem.SalesOrder.SalesOrderOncall.DataTruck.JenisTrucks.StrJenisTruck;
                Rute = dbitem.SalesOrder.SalesOrderOncall.StrDaftarHargaItem;
                TanggalMuat = dbitem.SalesOrder.SalesOrderOncall.TanggalMuat;
                NamaDriver = dbitem.SalesOrder.SalesOrderOncall.Driver1 == null ? "" : dbitem.SalesOrder.SalesOrderOncall.Driver1.NamaDriver;
                //Customer = dbitem.SalesOrder.SalesOrderOncall.Customer.CustomerNama;
            }
            else if (dbitem.SalesOrder.SalesOrderPickupId.HasValue)
            {
                NoSo = dbitem.SalesOrder.SalesOrderPickup.SONumber;
                VehicleNo = dbitem.SalesOrder.SalesOrderPickup.DataTruck.VehicleNo;
                JnsTruck = dbitem.SalesOrder.SalesOrderPickup.DataTruck.JenisTrucks.StrJenisTruck;
                Rute = dbitem.SalesOrder.SalesOrderPickup.Rute.Nama;
                TanggalMuat = dbitem.SalesOrder.SalesOrderPickup.TanggalPickup;
                NamaDriver = dbitem.SalesOrder.SalesOrderPickup.Driver1.NamaDriver;
            }
            else if (dbitem.SalesOrder.SalesOrderProsesKonsolidasiId.HasValue)
            {
                NoSo = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.SONumber;
                VehicleNo = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.DataTruck.VehicleNo;
                JnsTruck = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.DataTruck.JenisTrucks.StrJenisTruck;
                Rute = dbitem.RuteSo;
                TanggalMuat = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.TanggalMuat;
                NamaDriver = dbitem.SalesOrder.SalesOrderProsesKonsolidasi.Driver1.NamaDriver;
            }
            else if (dbitem.SalesOrder.SalesOrderKontrakId.HasValue)
            {
                List<int> ListIdDumy = dbitem.ListIdSo.Split(new string[] { "." }, StringSplitOptions.None).ToList().Select(int.Parse).ToList();
                List<Context.SalesOrderKontrakListSo> dbsoDummy = dbitem.SalesOrder.SalesOrderKontrak.SalesOrderKontrakListSo.Where(d => ListIdDumy.Contains(d.Id)).ToList();
                NoSo = string.Join(", ", dbsoDummy.Select(d => d.NoSo).ToList());
                VehicleNo = dbsoDummy.FirstOrDefault().DataTruck.VehicleNo;
                JnsTruck = dbsoDummy.FirstOrDefault().DataTruck.JenisTrucks.StrJenisTruck;
                TanggalMuat = dbsoDummy.FirstOrDefault().MuatDate;
                Customer = dbitem.SalesOrder.SalesOrderKontrak.Customer.CustomerNama;
                KodeNama = dbitem.SalesOrder.SalesOrderKontrak.Customer.CustomerCodeOld;
                NamaDriver = dbsoDummy.FirstOrDefault().Driver1.NamaDriver;
            }

            Customer = dbitem.Customer.CustomerNama;
            KodeNama = dbitem.Customer.CustomerCodeOld;
            Delay = dbitem.DokumenItem.Count() == 0 ? 0 : dbitem.DokumenItem.Where(d => !d.IsLengkap).Count();
            Lengkap = dbitem.DokumenItem.Count() == 0 ? "Ya" : dbitem.DokumenItem.Any(d => !d.IsLengkap) ? "Tidak" : "Ya";
            LastUpdate = dbitem.ModifiedDate;

            ListDokumen = new List<DokumenItem>();
            foreach (var item in dbitem.DokumenItem)
            {
                ListDokumen.Add(new DokumenItem(item));
            }

            IsReturn = dbitem.IsReturn;
        }
    }
}