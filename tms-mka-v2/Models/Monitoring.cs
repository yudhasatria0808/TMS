﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using tms_mka_v2.Context;
using System.Data;

namespace tms_mka_v2.Models
{

    public class MonitoringOnduty
    {
        public string StatusFlow { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string NoSo { get; set; }
        public string Vehicle { get; set; }
        public string TypeTruck { get; set; }
        public double Speed { get; set; }
        public string Engine { get; set; }
        public string Gps { get; set; }
        public string Lokasi { get; set; }
        public string KotaKab { get; set; }
        public string Provinsi { get; set; }
        public string Zone { get; set; }
        public string Customer { get; set; }
        public string Rute { get; set; }
        public DateTime? TglMuat { get; set; }
        public DateTime? TglBrkt { get; set; }
        public DateTime? TglTiba { get; set; }
        public DateTime? TglEstimasi { get; set; }
        public string Delay { get; set; }
        public string Suhu { get; set; }
        public string RangeSuhu { get; set; }
        public string SuhuAvg { get; set; }
        public string Deviasi { get; set; }
        public string Muat { get; set; }
        public string Perjalanan { get; set; }
        public string Bongkar { get; set; }
        public string Precooling { get; set; }
        public string Ac { get; set; }
        public string AcMati { get; set; }
        public string SuhuSesuai { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }

        public MonitoringOnduty() { }
        public MonitoringOnduty(Context.MonitoringVehicle dbitem)
        {
            Vehicle = dbitem.VehicleNo;
            TypeTruck = dbitem.Type;
            Speed = dbitem.Kecepatan;
            Gps = dbitem.Gps;
            Engine = dbitem.Engine;
            Ac = dbitem.Ac;
            Suhu = dbitem.Suhu;
            RangeSuhu = dbitem.RangeAc;
            SuhuAvg = dbitem.AvgSuhu.ToString();
            Lokasi = dbitem.Alamat;
            KotaKab = dbitem.Kabupaten;
            Provinsi = dbitem.Provinsi;
            Zone = dbitem.Zone;
            Lat = dbitem.LatNew;
            Long = dbitem.LongNew;
            LastUpdate = dbitem.LastUpdate;
        }
    }

    public class MonitoringOntime
    {
        public string StatusFlow { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string NoSo { get; set; }
        public string Vehicle { get; set; }
        public string TypeTruck { get; set; }
        public double Speed { get; set; }
        public string Engine { get; set; }
        public string Gps { get; set; }
        public string Lokasi { get; set; }
        public string KotaKab { get; set; }
        public string Provinsi { get; set; }
        public string Zone { get; set; }
        public string Customer { get; set; }
        public string Rute { get; set; }
        public DateTime? TglMuat { get; set; }
        public DateTime? TglBrkt { get; set; }
        public DateTime? TglTiba { get; set; }
        public DateTime? TglEstimasi { get; set; }
        public string Delay { get; set; }
        public int TotalMoving { get; set; }
        public int TotalStop { get; set; }
        public int MaxStop { get; set; }
        public string MaxStopPosition { get; set; }
        public string StopTime { get; set; }
        public string Muat { get; set; }
        public string Perjalanan { get; set; }
        public string Bongkar { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }

        public MonitoringOntime() { }
        public MonitoringOntime(Context.MonitoringVehicle dbitem)
        {
            Vehicle = dbitem.VehicleNo;
            TypeTruck = dbitem.Type;
            Speed = dbitem.Kecepatan;
            Gps = dbitem.Gps;
            Engine = dbitem.Engine;
            Lokasi = dbitem.Alamat;
            KotaKab = dbitem.Kabupaten;
            Provinsi = dbitem.Provinsi;
            Zone = dbitem.Zone;
            Lat = dbitem.LatNew;
            Long = dbitem.LongNew;
            LastUpdate = dbitem.LastUpdate;
        }
    }

    public class MonitoringOntemp
    {
        public string StatusFlow { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string NoSo { get; set; }
        public string Vehicle { get; set; }
        public string TypeTruck { get; set; }
        public double Speed { get; set; }
        public string Engine { get; set; }
        public string Gps { get; set; }
        public string Lokasi { get; set; }
        public string KotaKab { get; set; }
        public string Provinsi { get; set; }
        public string Zone { get; set; }
        public string Customer { get; set; }
        public string Rute { get; set; }
        public DateTime? TglMuat { get; set; }
        public DateTime? TglBrkt { get; set; }
        public string JenisProduct { get; set; }
        public string RangeSuhu { get; set; }
        public float AvgSuhu { get; set; }
        public string Deviasi { get; set; }
        public string AcOn { get; set; }
        public string AcOff { get; set; }
        public string MaxOff { get; set; }
        public string MaxOffPosition { get; set; }
        public DateTime? MaxOffTime { get; set; }
        public string Precolling { get; set; }
        public string AcMati { get; set; }
        public string SuhuSesuai { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }


        public MonitoringOntemp() { }
        public MonitoringOntemp(Context.MonitoringVehicle dbitem)
        {
            Vehicle = dbitem.VehicleNo;
            TypeTruck = dbitem.Type;
            Speed = dbitem.Kecepatan;
            Gps = dbitem.Gps;
            Engine = dbitem.Engine;
            Lokasi = dbitem.Alamat;
            KotaKab = dbitem.Kabupaten;
            Provinsi = dbitem.Provinsi;
            Zone = dbitem.Zone;
            Lat = dbitem.LatNew;
            Long = dbitem.LongNew;
            RangeSuhu = dbitem.RangeAc;
            AvgSuhu = AvgSuhu;
            LastUpdate = dbitem.LastUpdate;
        }
    }

    public class MonitoringAll
    {
        public string Status { get; set; }
        public string StatusFlow { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string Vehicle { get; set; }
        public string TypeTruck { get; set; }
        public double Speed { get; set; }
        public string Engine { get; set; }
        public string Gps { get; set; }
        public string Lokasi { get; set; }
        public string KotaKab { get; set; }
        public string Provinsi { get; set; }
        public string Zone { get; set; }
        public string Suhu { get; set; }
        public string Km { get; set; }
        public string Hm { get; set; }
        public string Ac { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string LastSo { get; set; }

        public MonitoringAll() { }
        public MonitoringAll(Context.MonitoringVehicle dbitem)
        {
            Vehicle = dbitem.VehicleNo;
            TypeTruck = dbitem.Type;
            Speed = dbitem.Kecepatan;
            Gps = dbitem.Gps;
            Engine = dbitem.Engine;
            Ac = dbitem.Ac;
            Suhu = dbitem.Suhu;
            Lokasi = dbitem.Alamat;
            KotaKab = dbitem.Kabupaten;
            Provinsi = dbitem.Provinsi;
            Zone = dbitem.Zone;
            Lat = dbitem.LatNew;
            Long = dbitem.LongNew;
            LastUpdate = dbitem.LastUpdate;
            Hm = dbitem.Hm;
            Km = dbitem.Km;
        }
    }

    public class MonitoringService
    {
        public string Status { get; set; }
        public string StatusFlow { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string Vehicle { get; set; }
        public string TypeTruck { get; set; }
        public double Speed { get; set; }
        public string Engine { get; set; }
        public string Gps { get; set; }
        public string Lokasi { get; set; }
        public string KotaKab { get; set; }
        public string Provinsi { get; set; }
        public string Zone { get; set; }
        public string Suhu { get; set; }
        public string Km { get; set; }
        public string Hm { get; set; }
        public string Ac { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string NoSo { get; set; }
        public DateTime? EstimasiService { get; set; }
        public int? RevEstimasi { get; set; }
        public string KeteranganService { get; set; }
        public string Customer { get; set; }
        public string Rute { get; set; }

        public MonitoringService() { }
        public MonitoringService(Context.MonitoringVehicle dbitem)
        {
            Vehicle = dbitem.VehicleNo;
            TypeTruck = dbitem.Type;
            Speed = dbitem.Kecepatan;
            Gps = dbitem.Gps;
            Engine = dbitem.Engine;
            Ac = dbitem.Ac;
            Suhu = dbitem.Suhu;
            Lokasi = dbitem.Alamat;
            KotaKab = dbitem.Kabupaten;
            Provinsi = dbitem.Provinsi;
            Zone = dbitem.Zone;
            Lat = dbitem.LatNew;
            Long = dbitem.LongNew;
            LastUpdate = dbitem.LastUpdate;
            Hm = dbitem.Hm;
            Km = dbitem.Km;
        }
    }

    public class MonitoringPosition
    {
        public string VehicleNo { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string NoSo { get; set; }
        public string Gps { get; set; }
        public string Engine { get; set; }
        public string Speed { get; set; }
        public string Ac { get; set; }
        public string Suhu { get; set; }
        public string Km { get; set; }
        public string Hm { get; set; }
        public string LatNew { get; set; }
        public string LongNew { get; set; }
        public string LatOld { get; set; }
        public string LongOld { get; set; }
        public string Zone { get; set; }
        public string Provinsi { get; set; }
        public string Kabupaten { get; set; }
        public string Alamat { get; set; }
        public string LastUpdate { get; set; }
    }

    public class HistoryTruckDriver
    {
        public string VehicleNo { get; set; }
        public string soId { get; set; }
        public string AdminUangJalanId { get; set; }
        public string CustomerNama { get; set; }
        public string SoNumber { get; set; }
        public string JenisOrder { get; set; }
        public string rute { get; set; }
        public string TanggalMuat { get; set; }
        public string Id { get; set; }
        public string Driver1 { get; set; }
        public string Id2 { get; set; }
        public string Driver2 { get; set; }
    }
}