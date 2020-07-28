using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using tms_mka_v2.Context;

namespace tms_mka_v2.Models
{
    public class AdminUangJalan
    {
        #region variable
        public int Id { get; set; }
        public int? IdSalesOrder { get; set; }
        public SalesOrderOncall ModelOncall { get; set; }
        public SalesOrderKontrak ModelKontrak { get; set; }
        public SalesOrderPickup ModelPickup { get; set; }
        public SalesOrderProsesKonsolidasi ModelKonsolidasi { get; set; }
        public List<AdminUangBorongan> ModelListBorongan { get; set; }
        public Decimal? NilaiBorongan { get; set; }
        public Decimal? Kawalan { get; set; }
        public Decimal? Timbangan { get; set; }
        public Decimal? Karantina { get; set; }
        public Decimal? SPSI { get; set; }
        public Decimal? Multidrop { get; set; }
        public List<AdminUangJalanTambahanRute> ModelListTambahanRute { get; set; }
        public List<AdminUangJalanTambahanLain> ModelListTambahanLain { get; set; }
        public Decimal? TotalBorongan { get; set; }
        public string KeteranganAdmin { get; set; }
        public int? IdDriverOld1 { get; set; }
        public string NamaDriverOld1 { get; set; }
        public int? IdDriverOld2 { get; set; }
        public string NamaDriverOld2 { get; set; }
        public int? IdDriver1 { get; set; }
        public string NamaDriver1 { get; set; }
        public string KeteranganGanti1 { get; set; }
        public int? IdDriver2 { get; set; }
        public string NamaDriver2 { get; set; }
        public string KeteranganGanti2 { get; set; }
        public Decimal? TotalKasbon { get; set; }
        public Decimal? KasbonDriver1 { get; set; }
        public Decimal? KasbonDriver2 { get; set; }
        public Decimal? TotalKlaim { get; set; }
        public Decimal? KlaimDriver1 { get; set; }
        public Decimal? KlaimDriver2 { get; set; }
        public List<AdminUangJalanPotonganLain> ModelListPotonganLain { get; set; }
        public Decimal? TotalPotonganDriver { get; set; }
        public List<AdminUangJalanVoucherSpbu> ModelListSpbu { get; set; }
        public List<AdminUangJalanVoucherKapal> ModelListKapal { get; set; }
        public List<AdminUangJalanUangTf> ModelListTf { get; set; }
        public string StrSolar { get; set; }
        public string StrKapal { get; set; }
        public string StrUang { get; set; }
        public Decimal? TotalAlokasi { get; set; }
        public string ListIdSo { get; set; }
        public string SelectedListIdSo { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        #endregion
        
        public AdminUangJalan()
        {
            ModelListTambahanRute = new List<AdminUangJalanTambahanRute>();
            ModelListTambahanLain = new List<AdminUangJalanTambahanLain>();
            ModelListPotonganLain = new List<AdminUangJalanPotonganLain>();
            ModelListBorongan = new List<AdminUangBorongan>();
            ModelListBorongan.Add(new AdminUangBorongan());
            ModelListSpbu = new List<AdminUangJalanVoucherSpbu>();
            ModelListKapal = new List<AdminUangJalanVoucherKapal>();
            ModelListTf = new List<AdminUangJalanUangTf>();
        }
        public AdminUangJalan(Context.AdminUangJalan dbitem, List<Context.Atm> listAtm, List<Context.DataBorongan> listBorongan)
        {
            Id = dbitem.Id;
            ModelListTambahanRute = new List<AdminUangJalanTambahanRute>();
            foreach (var item in dbitem.AdminUangJalanTambahanRute)
            {
                ModelListTambahanRute.Add(new AdminUangJalanTambahanRute(item));
            }
            ModelListTambahanLain = new List<AdminUangJalanTambahanLain>();
            foreach (var item in dbitem.AdminUangJalanTambahanLain)
            {
                ModelListTambahanLain.Add(new AdminUangJalanTambahanLain(item));
            }
            ModelListPotonganLain = new List<AdminUangJalanPotonganLain>();
            foreach (var item in dbitem.AdminUangJalanPotonganDriver)
            {
                ModelListPotonganLain.Add(new AdminUangJalanPotonganLain(item));
            }
            ModelListBorongan = new List<AdminUangBorongan>();
            foreach (var item in dbitem.IdDataBorongan.Split(','))
            {
                ModelListBorongan.Add(new AdminUangBorongan() { IdDataBorongan = int.Parse(item), NamaDataBorongan = listBorongan.Where(d => d.Id == int.Parse(item)).FirstOrDefault().NamaBorongan });
            }
            NilaiBorongan = dbitem.NilaiBorongan;
            Kawalan = dbitem.Kawalan;
            Timbangan = dbitem.Timbangan;
            Karantina = dbitem.Karantina;
            SPSI = dbitem.SPSI;
            Multidrop = dbitem.Multidrop;
            TotalBorongan = dbitem.TotalBorongan;
            KeteranganAdmin = dbitem.KeteranganAdmin;
            IdDriverOld1 = dbitem.IdDriverOld1;
            NamaDriverOld1 = dbitem.IdDriverOld1.HasValue ? dbitem.DriverOld1.KodeDriver + " - " + dbitem.DriverOld1.NamaDriver : ""; 
            IdDriverOld2 = dbitem.IdDriverOld2;
            NamaDriverOld2 = dbitem.IdDriverOld2.HasValue ? dbitem.DriverOld2.KodeDriver + " - " + dbitem.DriverOld2.NamaDriver : "";
            IdDriver1 = dbitem.IdDriver1;
            NamaDriver1 = dbitem.IdDriver1.HasValue ? dbitem.Driver1.KodeDriver + " - " + dbitem.Driver1.NamaDriver : ""; 
            KeteranganGanti1 = dbitem.KeteranganGanti1;
            IdDriver2 = dbitem.IdDriver2;
            NamaDriver2 = dbitem.IdDriver2.HasValue ? dbitem.Driver2.KodeDriver + " - " + dbitem.Driver2.NamaDriver : "";
            KeteranganGanti2 = dbitem.KeteranganGanti2;
            TotalKasbon = dbitem.TotalKasbon;
            KasbonDriver1 = dbitem.KasbonDriver1;
            KasbonDriver2 = dbitem.KasbonDriver2;
            TotalKlaim = dbitem.TotalKlaim;
            KlaimDriver1 = dbitem.KlaimDriver1;
            KlaimDriver2 = dbitem.KlaimDriver2;
            TotalPotonganDriver = dbitem.TotalPotonganDriver;
     
            ModelListSpbu = new List<AdminUangJalanVoucherSpbu>();
            foreach (var item in dbitem.AdminUangJalanVoucherSpbu)
            {
                ModelListSpbu.Add(new AdminUangJalanVoucherSpbu(item));
            }
            ModelListKapal = new List<AdminUangJalanVoucherKapal>();
            foreach (var item in dbitem.AdminUangJalanVoucherKapal)
            {
                ModelListKapal.Add(new AdminUangJalanVoucherKapal(item));
            }
            ModelListTf = new List<AdminUangJalanUangTf>();
            foreach (var item in dbitem.AdminUangJalanUangTf)
            {
                ModelListTf.Add(new AdminUangJalanUangTf(item, listAtm));
            }
            TotalAlokasi = dbitem.TotalAlokasi;
        }
        public void setDb(Context.AdminUangJalan dbitem)
        {
            List<string> listBor = new List<string>();
            foreach (var item in ModelListBorongan.Where(d => d.IsDelete == false))
	        {
		        listBor.Add(item.IdDataBorongan.Value.ToString());
	        }
            dbitem.IdDataBorongan = string.Join(",", listBor);
            dbitem.NilaiBorongan = NilaiBorongan;
            dbitem.Kawalan = Kawalan;
            dbitem.Timbangan = Timbangan;
            dbitem.Karantina = Karantina;
            dbitem.SPSI = SPSI;
            dbitem.Multidrop = Multidrop;
            dbitem.TotalBorongan = TotalBorongan;
            dbitem.KeteranganAdmin = KeteranganAdmin;
            dbitem.IdDriver1 = IdDriver1;
            dbitem.IdDriver2 = IdDriver2;
            dbitem.IdDriverOld1 = IdDriverOld1;
            dbitem.IdDriverOld2 = IdDriverOld2;
            dbitem.KeteranganGanti1 = KeteranganGanti1;
            dbitem.KeteranganGanti2 = KeteranganGanti2;
            dbitem.KasbonDriver1 = KasbonDriver1;
            dbitem.KasbonDriver2 = KasbonDriver2;
            dbitem.TotalKasbon = TotalKasbon;
            dbitem.KlaimDriver1 = KlaimDriver1;
            dbitem.KlaimDriver2 = KlaimDriver2;
            dbitem.TotalKlaim = TotalKlaim;
            dbitem.TotalPotonganDriver = TotalPotonganDriver;

            dbitem.AdminUangJalanTambahanRute.Clear();
            foreach (AdminUangJalanTambahanRute item in ModelListTambahanRute.Where(d=>d.IsDelete == false))
            {
                dbitem.AdminUangJalanTambahanRute.Add(item.setDb(new Context.AdminUangJalanTambahanRute()));    
            }
            dbitem.AdminUangJalanTambahanLain.Clear();
            foreach (AdminUangJalanTambahanLain item in ModelListTambahanLain.Where(d => d.IsDelete == false))
            {
                dbitem.AdminUangJalanTambahanLain.Add(item.setDb(new Context.AdminUangJalanTambahanLain()));
            }
            dbitem.AdminUangJalanPotonganDriver.Clear();
            foreach (AdminUangJalanPotonganLain item in ModelListPotonganLain.Where(d => d.IsDelete == false))
            {
                dbitem.AdminUangJalanPotonganDriver.Add(item.setDb(new Context.AdminUangJalanPotonganDriver()));
            }
            dbitem.AdminUangJalanVoucherSpbu.Clear();
            foreach (AdminUangJalanVoucherSpbu item in ModelListSpbu)
            {
                dbitem.AdminUangJalanVoucherSpbu.Add(item.setDb(new Context.AdminUangJalanVoucherSpbu()));
            }
            dbitem.AdminUangJalanVoucherKapal.Clear();
            foreach (AdminUangJalanVoucherKapal item in ModelListKapal)
            {
                dbitem.AdminUangJalanVoucherKapal.Add(item.setDb(new Context.AdminUangJalanVoucherKapal()));
            }
            dbitem.AdminUangJalanUangTf.Clear();
            foreach (AdminUangJalanUangTf item in ModelListTf)
            {
                dbitem.AdminUangJalanUangTf.Add(item.setDb(new Context.AdminUangJalanUangTf()));
            }
            TotalAlokasi = dbitem.TotalAlokasi;

        }
    }

    public class AdminUangBorongan
    {
        public int Id { get; set; }
        public int? IdDataBorongan { get; set; }
        public string NamaDataBorongan { get; set; }
        public bool IsDelete { get; set; }
    }
    public class AdminUangJalanTambahanRute
    {
        public int Id { get; set; }
        public int? IdAdminUangJalan { get; set; }
        public int? IdDataBorongan { get; set; }
        public Decimal? value { get; set; }
        public bool IsDelete { get; set; }
        public AdminUangJalanTambahanRute() { }
        public AdminUangJalanTambahanRute(Context.AdminUangJalanTambahanRute dbitem) { 
            Id = dbitem.Id;
            IdAdminUangJalan = dbitem.IdAdminUangJalan;
            IdDataBorongan = dbitem.IdDataBorongan;
            value = dbitem.values;
        }
        public Context.AdminUangJalanTambahanRute setDb(Context.AdminUangJalanTambahanRute dbitem)
        {
            dbitem.Id = Id;
            dbitem.IdAdminUangJalan = IdAdminUangJalan;
            dbitem.IdDataBorongan = IdDataBorongan;
            dbitem.values = value;
            return dbitem;
        }
    }
    public class AdminUangJalanTambahanLain
    {
        public int Id { get; set; }
        public int? IdAdminUangJalan { get; set; }
        public string Keterangan { get; set; }
        public Decimal? Value { get; set; }
        public bool IsDelete { get; set; }
        public AdminUangJalanTambahanLain() { }
        public AdminUangJalanTambahanLain(Context.AdminUangJalanTambahanLain dbitem) { 
            Id = dbitem.Id;
            IdAdminUangJalan = dbitem.IdAdminUangJalan;
            Keterangan = dbitem.Keterangan;
            Value = dbitem.Values;
        }
        public Context.AdminUangJalanTambahanLain setDb(Context.AdminUangJalanTambahanLain dbitem)
        {
            dbitem.Id = Id;
            dbitem.IdAdminUangJalan = IdAdminUangJalan;
            dbitem.Keterangan = Keterangan;
            dbitem.Values = Value;
            return dbitem;
        }
    }
    public class AdminUangJalanPotonganLain
    {
        public int Id { get; set; }
        public int? IdAdminUangJalan { get; set; }
        public string Keterangan { get; set; }
        public int TypeDriver { get; set; }
        public int? Value { get; set; }
        public bool IsDelete { get; set; }
        public AdminUangJalanPotonganLain() { }
        public AdminUangJalanPotonganLain(Context.AdminUangJalanPotonganDriver dbitem) { 
            Id = dbitem.Id;
            IdAdminUangJalan = dbitem.IdAdminUangJalan;
            Keterangan = dbitem.Keterangan;
            Value = dbitem.Value;
            TypeDriver = dbitem.TypeDriver;
        }
        public Context.AdminUangJalanPotonganDriver setDb(Context.AdminUangJalanPotonganDriver dbitem)
        {
            dbitem.Id = Id;
            dbitem.IdAdminUangJalan = IdAdminUangJalan;
            dbitem.Keterangan = Keterangan;
            dbitem.Value = Value;
            dbitem.TypeDriver = TypeDriver;
            return dbitem;
        }
    }
    public class AdminUangJalanVoucherSpbu
    {
        public int Id { get; set; }
        public string NamaSpbu { get; set; }
        public int? Value { get; set; }
        public AdminUangJalanVoucherSpbu() { }
        public AdminUangJalanVoucherSpbu(Context.AdminUangJalanVoucherSpbu dbitem) {
            Id = dbitem.Id;
            NamaSpbu = dbitem.Keterangan;
            Value = dbitem.Value;
        }
        public Context.AdminUangJalanVoucherSpbu setDb(Context.AdminUangJalanVoucherSpbu dbitem)
        {
            dbitem.Id = Id;
            dbitem.Keterangan = NamaSpbu;
            dbitem.Value = Value;
            return dbitem;
        }
    }
    public class AdminUangJalanVoucherKapal
    {
        public int Id { get; set; }
        public string NamaPenyebrangan { get; set; }
        public int? Value { get; set; }
        public AdminUangJalanVoucherKapal() { }
        public AdminUangJalanVoucherKapal(Context.AdminUangJalanVoucherKapal dbitem)
        {
            Id = dbitem.Id;
            NamaPenyebrangan = dbitem.Keterangan;
            Value = dbitem.Value;
        }
        public Context.AdminUangJalanVoucherKapal setDb(Context.AdminUangJalanVoucherKapal dbitem)
        {
            dbitem.Id = Id;
            dbitem.Keterangan = NamaPenyebrangan;
            dbitem.Value = Value;
            return dbitem;
        }
    }
    public class AdminUangJalanUangTf
    {
        public int Id { get; set; }
        public string Nama { get; set; }
        public int? Value { get; set; }
        public DateTime? Tanggal { get; set; }
        public decimal? JumlahTransfer { get; set; }
        public int? idRekening { get; set; }
        public string NoRekening { get; set; }
        public string AtasNama { get; set; }
        public string NamaBank { get; set; }
        public DateTime? TanggalAktual { get; set; }
        public TimeSpan? JamAktual { get; set; }
        public string KeteranganTf { get; set; }
        public string KeteranganAdmin { get; set; }
        public int? IdDriverPenerima { get; set; }
        public string DriverPenerima { get; set; }
        public bool isTf { get; set; }

        public AdminUangJalanUangTf() { }
        public AdminUangJalanUangTf(Context.AdminUangJalanUangTf dbitem, List<Context.Atm> listAtm)
        {
            Id = dbitem.Id;
            Nama = dbitem.Keterangan;
            Value = dbitem.Value;
            Tanggal = dbitem.Tanggal;
            JumlahTransfer = dbitem.JumlahTransfer.HasValue ? dbitem.JumlahTransfer : 0; 
            idRekening = dbitem.idRekenings;
            if (dbitem.idRekenings.HasValue)
            {
                NoRekening = dbitem.Atm.NoRekening;
                AtasNama = dbitem.Atm.Driver.NamaDriver;
                NamaBank = dbitem.Atm.LookupCodeBank.Nama;
            }
            else {
                Context.Atm dbattm = listAtm.Where(d => d.IdDriver == dbitem.AdminUangJalan.IdDriver1).FirstOrDefault();
                if (dbattm != null)
                {
                    idRekening = dbattm.Id;
                    NoRekening = dbattm.NoRekening;
                    AtasNama = dbattm.AtasNama;
                    NamaBank = dbattm.LookupCodeBank.Nama;
                }
            }
            TanggalAktual = dbitem.TanggalAktual;
            JamAktual = dbitem.JamAktual;
            KeteranganTf = dbitem.KeteranganTf;
            KeteranganAdmin = dbitem.AdminUangJalan.KeteranganAdmin;
            if (dbitem.IdDriverPenerima.HasValue)
            {
                IdDriverPenerima = dbitem.IdDriverPenerima;
                DriverPenerima = dbitem.Driver.NamaDriver;
            }
            isTf = dbitem.isTf;
        }
        public Context.AdminUangJalanUangTf setDb(Context.AdminUangJalanUangTf dbitem)
        {
            dbitem.Id = Id;
            dbitem.Keterangan = Nama;
            dbitem.Value = Value;
            dbitem.Tanggal = Tanggal.Value.AddDays(1);
            dbitem.JumlahTransfer = JumlahTransfer;
            dbitem.idRekenings = idRekening;
            dbitem.TanggalAktual = TanggalAktual.HasValue ? TanggalAktual.Value.AddDays(1) : TanggalAktual;
            dbitem.JamAktual = JamAktual;            
            dbitem.KeteranganTf = KeteranganTf;
            dbitem.isTf = isTf;
            dbitem.IdDriverPenerima = IdDriverPenerima;
            return dbitem;
        }
    }
}