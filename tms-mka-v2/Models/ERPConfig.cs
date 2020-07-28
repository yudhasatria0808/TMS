﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace tms_mka_v2.Models
{
    public class ERPConfig
    {
        public int IdDP { get; set; }
        public string CodeDP { get; set; }
        public string NamaDP { get; set; }
        public int Id { get; set; }
        public int? IdCreditAuj { get; set; }
        public string CodeCreditAuj { get; set; }
        public string NamaCreditAuj { get; set; }
        public int? IdKasbonAuj { get; set; }
        public string CodeKasbonAuj { get; set; }
        public string NamaKasbonAuj { get; set; }
        public int? IdKlaimAuj { get; set; }
        public string CodeKlaimAuj { get; set; }
        public string NamaKlaimAuj { get; set; }
        public int? IdVoucherSolar { get; set; }
        public string CodeVoucherSolar { get; set; }
        public string NamaVoucherSolar { get; set; }
        public int? IdVoucherKapal { get; set; }
        public string CodeVoucherKapal { get; set; }
        public string NamaVoucherKapal { get; set; }
        public int? IdPotonganLainAuj { get; set; }
        public string CodePotonganLainAuj { get; set; }
        public string NamaPotonganLainAuj { get; set; }
        public int? IdBoronganDasar { get; set; }
        public string CodeBoronganDasar { get; set; }
        public string NamaBoronganDasar { get; set; }
        public int? IdKawalan { get; set; }
        public string CodeKawalan { get; set; }
        public string NamaKawalan { get; set; }
        public int? IdTimbangan { get; set; }
        public string CodeTimbangan { get; set; }
        public string NamaTimbangan { get; set; }
        public int? IdKaarantina { get; set; }
        public string CodeKaarantina { get; set; }
        public string NamaKaarantina { get; set; }
        public int? IdSPSI { get; set; }
        public string CodeSPSI { get; set; }
        public string NamaSPSI { get; set; }
        public int? IdMultidrop { get; set; }
        public string CodeMultidrop { get; set; }
        public string NamaMultidrop { get; set; }
        public int? IdRutemuat { get; set; }
        public string CodeRutemuat { get; set; }
        public string NamaRutemuat { get; set; }
        public int? IdRuteLain { get; set; }
        public string CodeRuteLain { get; set; }
        public string NamaRuteLain { get; set; }
        public int? IdOprCash { get; set; }
        public string CodeOprCash { get; set; }
        public string NamaOprCash { get; set; }
        public int? IdCreditCash { get; set; }
        public string CodeCreditCash { get; set; }
        public string NamaCreditCash { get; set; }
        public int? IdOprTf { get; set; }
        public string CodeOprTf { get; set; }
        public string NamaOprTf { get; set; }
        public int? IdCreditTf { get; set; }
        public string CodeCreditTf { get; set; }
        public string NamaCreditTf { get; set; }
        public int? IdBiayaInap { get; set; }
        public string CodeBiayaInap { get; set; }
        public string NamaBiayaInap { get; set; }
        public int? IdBiayaPerjalanan { get; set; }
        public string CodeBiayaPerjalanan { get; set; }
        public string NamaBiayaPerjalanan { get; set; }
        public int? IdBiayaKlaim { get; set; }
        public string CodeBiayaKlaim { get; set; }
        public string NamaBiayaKlaim { get; set; }
        public int? IdKreditKlaim { get; set; }
        public string CodeKreditKlaim { get; set; }
        public string NamaKreditKlaim { get; set; }
        public int? IdOprInapCash { get; set; }
        public string CodeOprInapCash { get; set; }
        public string NamaOprInapCash { get; set; }
        public int? IdOprInapTf { get; set; }
        public string CodeOprInapTf { get; set; }
        public string NamaOprInapTf { get; set; }
        public int? IdKasbonDriver { get; set; }
        public string CodeKasbonDriver { get; set; }
        public string NamaKasbonDriver { get; set; }
        public int? IdPiutangCustomer { get; set; }
        public string CodePiutangCustomer { get; set; }
        public string NamaPiutangCustomer { get; set; }
        public int? IdPiutangBlmInv { get; set; }
        public string CodePiutangBlmInv { get; set; }
        public string NamaPiutangBlmInv { get; set; }
        public int? IdPenjualanBlmInv { get; set; }
        public string CodePenjualanBlmInv { get; set; }
        public string NamaPenjualanBlmInv { get; set; }
        public int? IdPiutangDailySupir { get; set; }
        public string CodePiutangDailySupir { get; set; }
        public string NamaPiutangDailySupir { get; set; }
        public int? IdKlaimSupir { get; set; }
        public string CodeKlaimSupir { get; set; }
        public string NamaKlaimSupir { get; set; }
        public int? IdPiutangDriverBatalJalan { get; set; }
        public string CodePiutangDriverBatalJalan { get; set; }
        public string NamaPiutangDriverBatalJalan { get; set; }
        public int? IdPiutangDriverPribadi { get; set; }
        public string CodePiutangDriverPribadi { get; set; }
        public string NamaPiutangDriverPribadi { get; set; }
        public int? IdPendapatanPengembalianPiutangDriver { get; set; }
        public string CodePendapatanPengembalianPiutangDriver { get; set; }
        public string NamaPendapatanPengembalianPiutangDriver { get; set; }
        public int? IdTabunganDriver { get; set; }
        public string CodeTabunganDriver { get; set; }
        public string NamaTabunganDriver { get; set; }
        public int? IdPiutangDriverBatalJalanSementaraSolar { get; set; }
        public string CodePiutangDriverBatalJalanSementaraSolar { get; set; }
        public string NamaPiutangDriverBatalJalanSementaraSolar { get; set; }
        public int? IdBiayaBatalJalan { get; set; }
        public string CodeBiayaBatalJalan { get; set; }
        public string NamaBiayaBatalJalan { get; set; }
    }
}