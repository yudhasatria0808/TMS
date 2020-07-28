using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace tms_mka_v2.Context
{
    public class AdminUangJalan
    {
        public AdminUangJalan()
        {
            this.AdminUangJalanTambahanLain = new HashSet<AdminUangJalanTambahanLain>();
            this.AdminUangJalanPotonganDriver = new HashSet<AdminUangJalanPotonganDriver>();
            this.AdminUangJalanTambahanRute = new HashSet<AdminUangJalanTambahanRute>();
            this.AdminUangJalanVoucherSpbu = new HashSet<AdminUangJalanVoucherSpbu>();
            this.AdminUangJalanVoucherKapal = new HashSet<AdminUangJalanVoucherKapal>();
            this.AdminUangJalanUangTf = new HashSet<AdminUangJalanUangTf>();
            this.Removal = new HashSet<Removal>();
        }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Code { get; set; }
        public string IdDataBorongan { get; set; }
        public Decimal? NilaiBorongan { get; set; }
        public Decimal? Kawalan { get; set; }
        public Decimal? Timbangan { get; set; }
        public Decimal? Karantina { get; set; }
        public Decimal? SPSI { get; set; }
        public Decimal? Multidrop { get; set; }
        public Decimal? TotalBorongan { get; set; }
        public string KeteranganAdmin { get; set; }
        [ForeignKey("DriverOld1")]
        public int? IdDriverOld1 { get; set; }
        [ForeignKey("DriverOld2")]
        public int? IdDriverOld2 { get; set; }
        [ForeignKey("Driver1")]
        public int? IdDriver1 { get; set; }
        public string KeteranganGanti1 { get; set; }
        [ForeignKey("Driver2")]
        public int? IdDriver2 { get; set; }
        public string KeteranganGanti2 { get; set; }
        public Decimal? TotalKasbon { get; set; }
        public Decimal? KasbonDriver1 { get; set; }
        public Decimal? KasbonDriver2 { get; set; }
        public Decimal? TotalKlaim { get; set; }
        public Decimal? KlaimDriver1 { get; set; }
        public Decimal? KlaimDriver2 { get; set; }
        public Decimal? TotalPotonganDriver { get; set; }
        public Decimal? TotalAlokasi { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
        public decimal? PotonganB { get; set; }
        public decimal? PotonganP { get; set; }
        public decimal? PotonganK { get; set; }
        public decimal? PotonganT { get; set; }

        public virtual Driver DriverOld1 { get; set; }
        public virtual Driver DriverOld2 { get; set; }
        public virtual Driver Driver1 { get; set; }
        public virtual Driver Driver2 { get; set; }

        public virtual ICollection<AdminUangJalanTambahanLain> AdminUangJalanTambahanLain { get; set; }
        public virtual ICollection<AdminUangJalanPotonganDriver> AdminUangJalanPotonganDriver { get; set; }
        public virtual ICollection<AdminUangJalanTambahanRute> AdminUangJalanTambahanRute { get; set; }
        public virtual ICollection<AdminUangJalanVoucherSpbu> AdminUangJalanVoucherSpbu { get; set; }
        public virtual ICollection<AdminUangJalanVoucherKapal> AdminUangJalanVoucherKapal { get; set; }
        public virtual ICollection<AdminUangJalanUangTf> AdminUangJalanUangTf { get; set; }
        public virtual ICollection<Removal> Removal { get; set; }
    }
}