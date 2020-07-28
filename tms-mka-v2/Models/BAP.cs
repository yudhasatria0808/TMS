using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using tms_mka_v2.Context;

namespace tms_mka_v2.Models
{
    public class BAP
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public int SOBapId { get; set; }
        public int? SOBapKontrakId { get; set; }
        [Display(Name = "Status")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string StatusBap { get; set; }
        [Display(Name = "No BAP")]
        public string NoBAP { get; set; }
        [Display(Name = "Tanggal Kejadian")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public System.DateTime? TanggalKejadian { get; set; }
        [Display(Name = "Jam Kejadian")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public TimeSpan JamKejadian { get; set; }
        [Display(Name = "Kategori")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public int? KategoriId { get; set; }
        public string StrKategori { get; set; }
        [Display(Name = "Laporan Kejadian")]
        public string LaporanKejadian { get; set; }
        [Display(Name = "Dilaporkan Oleh")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string DilaporkanOleh { get; set; }
        [Display(Name = "Departemen1")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public int? Departemen1Id { get; set; }
        [Display(Name = "Hasil Pemeriksaan")]
        public string HasilPemeriksaan { get; set; }
        [Display(Name = "Penyelesaian")]
        public string Penyelesaian { get; set; }
        [Display(Name = "Diperiksa Oleh")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string DiperiksaOleh { get; set; }
        [Display(Name = "Departemen2")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public int? Departemen2Id { get; set; }
        [Display(Name = "Upload BAP")]
        public string File { get; set; }

        [Display(Name = "Driver 1")]
        public int? Driver1Id { get; set; }
        [Display(Name = "ID Driver")]
        public string KodeDriver1 { get; set; }
        [Display(Name = "Nama Driver")]
        public string NamaDriver1 { get; set; }
        [Display(Name = "ID Driver 2")]
        public int? Driver2Id { get; set; }
        [Display(Name = "Kode Driver 2")]
        public string KodeDriver2 { get; set; }
        [Display(Name = "Nama Driver 2")]
        public string NamaDriver2 { get; set; }
        public int? IdDataTruck { get; set; }
        [Display(Name = "Vehicle No")]
        public string VehicleNo { get; set; }
        [Display(Name = "Jenis Truk")]
        public string JenisTruk { get; set; }
        [Display(Name = "Jenis Pendingin")]
        public string JenisPendingin { get; set; }

        public BAP()
        {

        }
        public BAP(Context.BAP dbitem)
        {
            Id = dbitem.Id;
            SOBapId = dbitem.SalesOrderId.Value;
            SOBapKontrakId = dbitem.SalesOrderKontrakId;
            NoBAP = dbitem.NoBAP;
            TanggalKejadian = dbitem.TanggalKejadian;
            JamKejadian = dbitem.JamKejadian;
            KategoriId = dbitem.KategoriId;
            StrKategori = dbitem.LookUpKategori.Nama;
            LaporanKejadian = dbitem.LaporanKejadian;
            DilaporkanOleh = dbitem.DilaporkanOleh;
            Departemen1Id = dbitem.Departemen1Id;
            HasilPemeriksaan = dbitem.HasilPemeriksaan;
            Penyelesaian = dbitem.Penyelesaian;
            DiperiksaOleh = dbitem.DiperiksaOleh;
            Departemen2Id = dbitem.Departemen2Id;
            StatusBap = dbitem.Status;
            File = dbitem.File;
            Driver1Id = dbitem.Driver1Id;
            KodeDriver1 = dbitem.Driver1.KodeDriver;
            NamaDriver1 = dbitem.Driver1.NamaDriver;
            Driver2Id = dbitem.Driver2Id;
            KodeDriver2 = dbitem.Driver2Id.HasValue ? dbitem.Driver2.KodeDriver : "";
            NamaDriver2 = dbitem.Driver2Id.HasValue ? dbitem.Driver2.NamaDriver : "";
            IdDataTruck = dbitem.IdDataTruck;
            VehicleNo = dbitem.DataTruck.VehicleNo;
            JenisTruk = dbitem.DataTruck.JenisTrucks.StrJenisTruck;
            JenisPendingin = dbitem.DataTruck.DataPendingin.Count > 0 ? dbitem.DataTruck.DataPendingin.OrderBy(p => p.Id).Last().Model : "";
        }
        public void setDb(Context.BAP dbitem)
        {
            dbitem.Id = Id;
            dbitem.SalesOrderId = SOBapId;
            dbitem.SalesOrderKontrakId = SOBapKontrakId;
            dbitem.NoBAP = NoBAP;
            dbitem.TanggalKejadian = TanggalKejadian;
            dbitem.JamKejadian = JamKejadian;
            dbitem.KategoriId = KategoriId;
            dbitem.Departemen1Id = Departemen1Id;
            dbitem.Departemen2Id = Departemen2Id;
            dbitem.LaporanKejadian = LaporanKejadian;
            dbitem.DilaporkanOleh = DilaporkanOleh;
            dbitem.HasilPemeriksaan = HasilPemeriksaan;
            dbitem.Penyelesaian = Penyelesaian;
            dbitem.DiperiksaOleh = DiperiksaOleh;
            dbitem.Status = StatusBap;
            dbitem.File = File;
            dbitem.IdDataTruck = IdDataTruck;
            dbitem.Driver1Id = Driver1Id;
            dbitem.Driver2Id = Driver2Id;
        }
    }
}