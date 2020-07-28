using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using tms_mka_v2.Business_Logic.Abstract;
using tms_mka_v2.Infrastructure;
using tms_mka_v2.Models;

namespace tms_mka_v2.Controllers
{
    public class MonitoringController : BaseController
    {
        private ISalesOrderRepo RepoSalesOrder;
        private IDaftarHargaOnCallRepo RepoDaftarHarga;
        private IJenisTruckRepo RepoJnsTruck;
        private IDataTruckRepo RepoDataTruck;
        private IMonitoringVehicleRepo RepoMonitoringVehicle;
        public MonitoringController(IUserReferenceRepo repoBase, ILookupCodeRepo repoLookup, ISalesOrderRepo repoSalesOrder, IDaftarHargaOnCallRepo repoDaftarHarga, IJenisTruckRepo repoJnsTruck,
            IDataTruckRepo repoDataTruck, IMonitoringVehicleRepo repoMonitoringVehicle)
            : base(repoBase, repoLookup)
        {
            RepoSalesOrder = repoSalesOrder;
            RepoDaftarHarga = repoDaftarHarga;
            RepoJnsTruck = repoJnsTruck;
            RepoDataTruck = repoDataTruck;
            RepoMonitoringVehicle = repoMonitoringVehicle;
        }

        // GET: Monitoring
        public ActionResult Index()
        {
            List<MonitoringAll> model = new List<MonitoringAll>();
            RepoMonitoringVehicle.FindAllTruck(model);
            model = model.Where(d => (d.Lat != null && d.Lat != "") && (d.Long != null && d.Long != "")).ToList();
            ViewBag.GPSRealtimeAll = model;
            return View();
        }

        public string BindingGridAllTruck()
        {
            List<MonitoringAll> ListModel = new List<MonitoringAll>();

            RepoMonitoringVehicle.FindAllTruck(ListModel);

            return new JavaScriptSerializer().Serialize(new { total = ListModel.Count, data = ListModel });
        }

        public string BindingGridByStat(string stat)
        {
            List<MonitoringAll> ListModel = new List<MonitoringAll>();

            RepoMonitoringVehicle.FindAllTruck(ListModel);

            ListModel = ListModel.Where(d => d.Status == stat).ToList();

            return new JavaScriptSerializer().Serialize(new { total = ListModel.Count, data = ListModel });
        }

        public string BindingGridService()
        {
            List<MonitoringService> ListModel = new List<MonitoringService>();

            RepoMonitoringVehicle.FindService(ListModel);

            return new JavaScriptSerializer().Serialize(new { total = ListModel.Count, data = ListModel });
        }

        public string BindingGridOntime()
        {
            List<MonitoringOntime> ListModel = new List<MonitoringOntime>();

            RepoMonitoringVehicle.FindOnTime(ListModel);

            ListModel = ListModel.Where(d => (d.Lat != null && d.Lat != "") && (d.Long != null && d.Long != "")).ToList();

            return new JavaScriptSerializer().Serialize(new { total = ListModel, data = ListModel });
        }

        public string BindingGridOntemp()
        {
            List<MonitoringOntemp> ListModel = new List<MonitoringOntemp>();

            RepoMonitoringVehicle.FindOnTemp(ListModel);

            ListModel = ListModel.Where(d => (d.Lat != null && d.Lat != "") && (d.Long != null && d.Long != "")).ToList();

            return new JavaScriptSerializer().Serialize(new { total = ListModel, data = ListModel });
        }

        public string BindingGridOnduty()
        {
            List<MonitoringOnduty> ListModel = new List<MonitoringOnduty>();

            RepoMonitoringVehicle.FindOnDuty(ListModel);

            ListModel = ListModel.Where(d => (d.Lat != null && d.Lat != "") && (d.Long != null && d.Long != "")).ToList();

            return new JavaScriptSerializer().Serialize(new { total = ListModel, data = ListModel });
        }







        public static DataTable GetDataTruck()
        {
            NpgsqlConnection con = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["tmsDefault"].ConnectionString);
            con.Open();
            using (DataTable dt = new DataTable())
            {

                var query = " SELECT \"VehicleNo\", \"Type\", \"Status\", \"LastSo\", \"Gps\", \"Engine\", \"Speed\", \"Ac\", \"Suhu\", \"Km\", \"Hm\", \"LatNew\", \"LongNew\", \"LatOld\", \"LongOld\", ";
                query += " \"Zone\", \"Provinsi\", \"Kabupaten\", \"Alamat\", \"LastUpdate\" FROM dbo.\"MonitoringVehicle\" WHERE \"LatNew\" is not null and \"LongNew\" is not null and ";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                da.Fill(dt);


                cmd.Dispose();
                con.Close();

                return dt;
            }
        }

        public static DataTable GetDataTruckAll(string state=null)
        {
            NpgsqlConnection con = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["tmsDefault"].ConnectionString);
            con.Open();
            using (DataTable dt = new DataTable())
            {
                var query = " SELECT \"VehicleNo\", \"Type\", \"Status\", \"LastSo\", \"Gps\", \"Engine\", \"Speed\", \"Ac\", \"Suhu\", \"Km\", \"Hm\", \"LatNew\", \"LongNew\", \"LatOld\", \"LongOld\", ";
                query += "\"Zone\", \"Provinsi\", \"Kabupaten\", \"Alamat\", \"LastUpdate\" FROM  dbo.\"MonitoringVehicle\" WHERE \"LatNew\" is not null and \"LongNew\" is not null ";
                if (state == "ready"){
                    query += "AND \"VehicleNo\" IN (SELECT \"VehicleNo\" FROM \"dbo\".\"DataTruck\" WHERE \"DataTruck\".\"Id\" IN (SELECT \"IdDataTruck\" FROM \"dbo\".\"SalesOrderOncall\" ";
                    query += "WHERE \"SalesOrderOnCallId\" IN (SELECT \"SalesOrderOncallId\" FROM \"dbo\".\"SalesOrder\" WHERE \"SalesOrderOncallId\" IS NOT NULL AND \"Status\" = 'save konfirmasi')))";
                }
                else if (state == "available"){
                    query += "AND \"VehicleNo\" IN (SELECT \"VehicleNo\" FROM \"dbo\".\"DataTruck\" WHERE \"DataTruck\".\"Id\" NOT IN (SELECT \"IdDataTruck\" FROM \"dbo\".\"SalesOrderOncall\"  WHERE ";
                    query += "\"SalesOrderOnCallId\" IN (SELECT \"SalesOrderOncallId\" FROM \"dbo\".\"SalesOrder\" WHERE \"SalesOrderOncallId\" IS NOT NULL AND \"Status\" IN ";
                    query += "('draft planning', 'save planning', 'draft konfirmasi', 'save konfirmasi', 'admin uang jalan', 'dispatched'))))";
                }
                query += " LIMIT 350";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                da.Fill(dt);
                cmd.Dispose();
                con.Close();
                return dt;
            }
        }

        public static DataTable GetDataTruckOnDuty()
        {
            NpgsqlConnection con = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["tmsDefault"].ConnectionString);
            con.Open();
            using (DataTable dt = new DataTable())
            {
                var query = " SELECT \"MonitoringVehicle\".\"VehicleNo\", \"Type\", \"MonitoringVehicle\".\"Status\", \"MonitoringVehicle\".\"LastSo\", \"MonitoringVehicle\".\"Gps\", \"NoSo\", ";
                query += " \"MonitoringVehicle\".\"Engine\", \"MonitoringVehicle\".\"Speed\", \"MonitoringVehicle\".\"Ac\", \"Suhu\", \"Km\", \"Hm\", \"LatNew\", \"LongNew\", \"LatOld\", \"LongOld\", ";
                query += "\"Zone\", \"Provinsi\", \"Kabupaten\", \"Alamat\", \"LastUpdate\", \"CustomerNama\", \"Dari\", \"Tujuan\", \"TglMuat\", \"TglBerangkat\", \"TargetTiba\", \"RangeSuhu\", ";
                query += "\"TglTiba\",\"SuhuAvg\" FROM dbo.\"MonitoringDetailSo\" INNER JOIN dbo.\"MonitoringVehicle\" ON \"MonitoringDetailSo\".\"VehicleNo\" = \"MonitoringVehicle\".\"VehicleNo\" ";
                query += "WHERE \"LatNew\" is not null and \"LongNew\" is not null AND LOWER(\"Ac\") = 'on'";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                da.Fill(dt);
                cmd.Dispose();
                con.Close();
                return dt;
            }
        }

        public static DataTable GetDataTruckService(string state=null)
        {
            NpgsqlConnection con = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["tmsDefault"].ConnectionString);
            con.Open();
            using (DataTable dt = new DataTable())
            {
                var query = "SELECT \"Workshop\".\"Status\", \"DataTruck\".\"VehicleNo\", \"Type\", \"LastSo\", \"Gps\", \"Engine\", \"Speed\", \"Ac\", \"Suhu\", \"Km\", \"Hm\", \"LatNew\",";
                query += " \"LongNew\", \"LatOld\", \"LongOld\", ";
                query += "\"Zone\", \"Provinsi\", \"Kabupaten\", \"Alamat\", \"LastUpdate\" FROM  dbo.\"MonitoringVehicle\" ";
                query += "INNER JOIN dbo.\"DataTruck\" ON \"DataTruck\".\"VehicleNo\" = \"MonitoringVehicle\".\"VehicleNo\" INNER JOIN dbo.\"Workshop\" ON \"Workshop\".\"IdVehicle\"=\"DataTruck\".\"Id\"";
                query += "WHERE \"LatNew\" is not null and \"LongNew\" is not null AND \"Workshop\".\"Status\" != 'SPK' GROUP BY \"MonitoringVehicle\".\"VehicleNo\", \"Workshop\".\"Status\", \"DataTruck\".\"VehicleNo\"";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                da.Fill(dt);
                cmd.Dispose();
                con.Close();
                return dt;
            }
        }

        //public static DataTable GetDataHistoryTruckDriver(string TrukDriver, string Search)
        //public static DataTable GetDataHistoryTruckDriver()
        //{
        //    NpgsqlConnection con = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["tmsDefault"].ConnectionString);
        //    con.Open();

        //    using (DataTable dt = new DataTable())
        //    {
        //        // fill DataTable logic
        //        var query = " SELECT DISTINCT h.\"VehicleNo\", h.\"soId\", h.\"AdminUangJalanId\", c.\"CustomerNama\", ";
        //        query += " CASE  	 ";
        //        query += "  WHEN so.\"SalesOrderOncallId\" is not null THEN sc.\"SONumber\"       ";
        //        query += "  WHEN so.\"SalesOrderKontrakId\" is not null THEN skd.\"NoSo\"       ";
        //        query += "  WHEN so.\"SalesOrderPickupId\" is not null THEN sp.\"SONumber\"      ";
        //        query += "  WHEN so.\"SalesOrderProsesKonsolidasiId\" is not null THEN sl.\"SONumber\"       ";
        //        query += " END as SoNumber, ";
        //        query += " CASE  	 ";
        //        query += "  WHEN so.\"SalesOrderOncallId\" is not null THEN 'Oncall'       ";
        //        query += "  WHEN so.\"SalesOrderKontrakId\" is not null THEN 'Kontrak'       ";
        //        query += "  WHEN so.\"SalesOrderPickupId\" is not null THEN 'Pickup'      ";
        //        query += "  WHEN so.\"SalesOrderProsesKonsolidasiId\" is not null THEN 'Konsolidasi'       ";
        //        query += " END as JenisOrder, ";
        //        query += " CASE  	 ";
        //        query += "  WHEN so.\"SalesOrderOncallId\" is not null THEN dc.\"ListNamaRute\"       ";
        //        query += "  WHEN so.\"SalesOrderKonsolidasiId\" is not null THEN dk.\"ListNamaRute\"  ";
        //        query += " END AS rute, ";
        //        query += " CASE  	 ";
        //        query += "  WHEN so.\"SalesOrderOncallId\" is not null THEN sc.\"TanggalMuat\"       ";
        //        query += "  WHEN so.\"SalesOrderKontrakId\" is not null THEN skd.\"MuatDate\"       ";
        //        query += "  WHEN so.\"SalesOrderPickupId\" is not null THEN sp.\"TanggalPickup\"       ";
        //        query += "  WHEN so.\"SalesOrderProsesKonsolidasiId\" is not null THEN sl.\"TanggalMuat\"       ";
        //        query += " END as TanggalMuat, ";
        //        query += " dr.\"Id\", ";
        //        query += " CASE  	 ";
        //        query += "  WHEN sc.\"Driver1Id\" is not null THEN dr.\"NamaDriver\"      ";
        //        query += "  WHEN skt.\"IdDriver1\" is not null THEN dr.\"NamaDriver\"       ";
        //        query += "  WHEN sp.\"Driver1Id\" is not null THEN dr.\"NamaDriver\"       ";
        //        query += "  WHEN sl.\"Driver1Id\" is not null THEN dr.\"NamaDriver\"       ";
        //        query += " END as Driver1, ";
        //        query += " dr2.\"Id\" as Id2, ";
        //        query += " CASE  	 ";
        //        query += "  WHEN sc.\"Driver2Id\" is not null THEN dr2.\"NamaDriver\"      ";
        //        query += "  WHEN skt.\"IdDriver2\" is not null THEN dr2.\"NamaDriver\"       ";
        //        query += "  WHEN sp.\"Driver2Id\" is not null THEN dr2.\"NamaDriver\"       ";
        //        query += "  WHEN sl.\"Driver2Id\" is not null THEN dr2.\"NamaDriver\"       ";
        //        query += " END as Driver2 ";
        //        query += " from dbo.\"HistoryGps\" h ";
        //        query += " LEFT JOIN dbo.\"SalesOrder\" so ON h.\"soId\" = so.\"Id\"   ";
        //        query += " LEFT JOIN dbo.\"SalesOrderOncall\" sc ON so.\"SalesOrderOncallId\" = sc.\"SalesOrderOnCallId\"   ";
        //        query += " LEFT JOIN dbo.\"SalesOrderKontrak\" sk ON so.\"SalesOrderKontrakId\" = sk.\"SalesOrderKontrakId\" ";
        //        query += " LEFT JOIN dbo.\"SalesOrderKontrakTruck\" skt ON sk.\"SalesOrderKontrakId\" = skt.\"SalesKontrakId\"    ";
        //        query += " LEFT JOIN dbo.\"SalesOrderKontrakDetail\" skd ON sk.\"SalesOrderKontrakId\" = skd.\"SalesKontrakId\"   ";
        //        query += " LEFT JOIN dbo.\"SalesOrderPickup\" sp ON so.\"SalesOrderPickupId\" = sp.\"SalesOrderPickupId\"   ";
        //        query += " LEFT JOIN dbo.\"SalesOrderProsesKonsolidasi\" sl ON so.\"SalesOrderProsesKonsolidasiId\" = sl.\"SalesOrderProsesKonsolidasiId\" ";
        //        query += " LEFT JOIN dbo.\"DaftarHargaOnCallItem\" dc on sc.\"IdDaftarHargaItem\" = dc.\"Id\" ";
        //        query += " LEFT JOIN dbo.\"DaftarHargaKonsolidasiItem\" dk on sl.\"IdDaftarHargaItem\" = dk.\"Id\" ";
        //        query += " LEFT JOIN dbo.\"Customer\" c ON sc.\"CustomerId\" = c.\"Id\" or sk.\"CustomerId\" = c.\"Id\" or sp.\"CustomerId\" = c.\"Id\" ";
        //        query += " LEFT JOIN dbo.\"Driver\" dr on sc.\"Driver1Id\" = dr.\"Id\" ";
        //        query += " or skt.\"IdDriver1\" = dr.\"Id\" ";
        //        query += " or sp.\"Driver1Id\" = dr.\"Id\" ";
        //        query += " or sl.\"Driver1Id\" = dr.\"Id\" ";
        //        query += " LEFT JOIN dbo.\"Driver\" dr2 on sc.\"Driver2Id\" = dr2.\"Id\" ";
        //        query += " or skt.\"IdDriver2\" = dr2.\"Id\"  ";
        //        query += " or sp.\"Driver2Id\" = dr2.\"Id\"  ";
        //        query += " or sl.\"Driver2Id\" = dr2.\"Id\"  ";
        //        query += " where h.\"soId\" != 0 and h.\"AdminUangJalanId\" != 0  ";
        //        //if (TrukDriver == "'truck'")
        //        //{
        //        //    query += " and h.\"VehicleNo\" like " + Search + " ";
        //        //}
        //        //else if (TrukDriver == "'driver1'")
        //        //{
        //        //    query += " and dr.\"Id\" = " + Search + " ";
        //        //}
        //        //else if (TrukDriver == "'driver2'")
        //        //{
        //        //    query += " and dr2.\"Id\" = " + Search + " ";
        //        //}

        //        NpgsqlCommand cmd = new NpgsqlCommand(query, con);

        //        NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
        //        da.Fill(dt);


        //        cmd.Dispose();
        //        con.Close();

        //        return dt;
        //    }
        //}

        

        //public string BindingGridReady()
        //{
        //    //var dataSo = RepoMonitoringVehicle.FindAll();
        //    //var query = from ds in dataSo


        //    //DataTable dt = GetDataTruckAll("ready");
        //    //List<MonitoringPosition> ListModel = dt.DataTableToList<MonitoringPosition>();

        //    //return new JavaScriptSerializer().Serialize(new { total = dt.Rows.Count, data = ListModel });
        //    return "";
        //}

        public string BindingGridAvailable()
        {
            DataTable dt = GetDataTruckAll("available");
            List<MonitoringPosition> ListModel = dt.DataTableToList<MonitoringPosition>();

            return new JavaScriptSerializer().Serialize(new { total = dt.Rows.Count, data = ListModel });
        }

        public string BindingGridHistroyTruckDriver()
        {
            //DataTable dt = GetDataHistoryTruckDriver(TruckDriver, Search);
            //DataTable dt = GetDataHistoryTruckDriver();
            //List<HistoryTruckDriver> ListModel = dt.DataTableToList<HistoryTruckDriver>();

            //return new JavaScriptSerializer().Serialize(new { total = dt.Rows.Count, data = ListModel });
            return "";
        }

        public List<MonitoringPosition> GPSRealtimeAll()
        {
            List<MonitoringPosition> gisModels = new List<MonitoringPosition>();
            DataTable dt = GetDataTruckAll();

            gisModels = (from DataRow dr in dt.Rows
                         select new MonitoringPosition()
                         {
                             VehicleNo = dr["VehicleNo"].ToString(),
                             Type = dr["Type"].ToString(),
                             Status = dr["Status"].ToString(),
                             //LastSo = dr["LastSo"].ToString(),
                             Gps = dr["Gps"].ToString(),
                             Engine = dr["Engine"].ToString(),
                             Speed = dr["Speed"].ToString(),
                             Ac = dr["Ac"].ToString(),
                             Suhu = dr["Suhu"].ToString(),
                             Km = dr["Km"].ToString(),
                             Hm = dr["Hm"].ToString(),
                             LatNew = dr["LatNew"].ToString(),
                             LongNew = dr["LongNew"].ToString(),
                             LatOld = dr["LatOld"].ToString() == "" ? dr["LatNew"].ToString() : dr["LatOld"].ToString(),
                             LongOld = dr["LongOld"].ToString() == "" ? dr["LongNew"].ToString() : dr["LongOld"].ToString(),
                             Zone = dr["Zone"].ToString(),
                             Provinsi = dr["Provinsi"].ToString(),
                             Kabupaten = dr["Kabupaten"].ToString(),
                             Alamat = dr["Alamat"].ToString(),
                             LastUpdate = dr["LastUpdate"].ToString()
                         }).ToList();

            return gisModels;
        }

        public string getCoordinate()
        {
            //List<MonitoringOnduty> gisModels = new List<MonitoringOnduty>();
            //DataTable dt = GetDataTruck();

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    var newGISModels = new MonitoringOnduty();

            //    newGISModels.VehicleNo = dt.Rows[i]["VehicleNo"].ToString();
            //    newGISModels.Kecepatan = dt.Rows[i]["Kecepatan"].ToString();
            //    newGISModels.Alamat = dt.Rows[i]["Alamat"].ToString();
            //    newGISModels.Long = dt.Rows[i]["Long"].ToString();
            //    newGISModels.Lat = dt.Rows[i]["Lat"].ToString();
            //    newGISModels.LongFirst = dt.Rows[i]["LongFirst"].ToString();
            //    newGISModels.LatFisrt = dt.Rows[i]["LatFisrt"].ToString();
            //    newGISModels.StatusOrder = dt.Rows[i]["StatusOrder"].ToString();
            //    newGISModels.CreatedDate = dt.Rows[i]["CreatedDate"].ToString();
            //    newGISModels.Status = dt.Rows[i]["Status"].ToString();
            //    newGISModels.Suhu = dt.Rows[i]["Suhu"].ToString();

            //    gisModels.Add(newGISModels);
            //}

            //return new JavaScriptSerializer().Serialize(gisModels.Select(d => new
            //{
            //    VehicleNo = d.VehicleNo,
            //    Kecepatan = d.Kecepatan,
            //    Alamat = d.Alamat,
            //    Long = d.Long,
            //    Lat = d.Lat,
            //    LongFirst = d.LongFirst,
            //    LatFisrt = d.LatFisrt,
            //    StatusOrder = d.StatusOrder,
            //    CreatedDate = d.CreatedDate,
            //    Status = d.Status,
            //    Suhu = d.Suhu,
            //}));

            return "";
        }

        public string OnDutyDetail(string VehicleNo, string NoSo){
            Context.DataTruck item = RepoDataTruck.FindByName(VehicleNo);
            UnitList unit = new UnitList(item);
            Context.MonitoringDetailSo mon_so = RepoMonitoringVehicle.FindMonitoringDetailSo(NoSo);

            return new JavaScriptSerializer().Serialize(new {unit = unit, Delay = mon_so.TglTiba - mon_so.TargetTiba});
        }

        public string getCoordinateAll()
        {
            List<MonitoringOnduty> gisModels = new List<MonitoringOnduty>();
            DataTable dt = GetDataTruckAll();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var newGISModels = new MonitoringOnduty();

                //newGISModels.VehicleNo = dt.Rows[i]["VehicleNo"].ToString();
                //newGISModels.Kecepatan = dt.Rows[i]["Kecepatan"].ToString();
                //newGISModels.Alamat = dt.Rows[i]["Alamat"].ToString();
                //newGISModels.Long = dt.Rows[i]["Long"].ToString();
                //newGISModels.Lat = dt.Rows[i]["Lat"].ToString();
                //newGISModels.LongFirst = dt.Rows[i]["LongFirst"].ToString();
                //newGISModels.LatFisrt = dt.Rows[i]["LatFisrt"].ToString();
                //newGISModels.StatusOrder = dt.Rows[i]["StatusOrder"].ToString();
                //newGISModels.CreatedDate = dt.Rows[i]["CreatedDate"].ToString();
                //newGISModels.Status = dt.Rows[i]["Status"].ToString();
                //newGISModels.Suhu = dt.Rows[i]["Suhu"].ToString();

                gisModels.Add(newGISModels);
            }

            return new JavaScriptSerializer().Serialize(gisModels.Select(d => new
            {
                //VehicleNo = d.VehicleNo,
                //Kecepatan = d.Kecepatan,
                //Alamat = d.Alamat,
                //Long = d.Long,
                //Lat = d.Lat,
                //LongFirst = d.LongFirst,
                //LatFisrt = d.LatFisrt,
                //StatusOrder = d.StatusOrder,
                //CreatedDate = d.CreatedDate,
                //Status = d.Status,
                //Suhu = d.Suhu,
            }));
        }

        //public ActionResult Detail(string id)
        public ActionResult Detail(string VehicleNo)
        {
            //TempData["VehicleNoMonitoring"] = id;

            //Context.SalesOrder dbitem = RepoSalesOrder.FindByPK(id);
            //SalesOrderOncall model = new SalesOrderOncall(dbitem);

            //ViewBag.name = model.SONumber;
            //return View("Detail", model);
            ViewBag.MonitoringPosition = RepoMonitoringVehicle.FindByVehicleNo(VehicleNo);
            return View("Detail");
        }
    }
}