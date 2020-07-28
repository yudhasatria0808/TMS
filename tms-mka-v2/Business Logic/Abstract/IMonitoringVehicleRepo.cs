using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tms_mka_v2.Context;
using tms_mka_v2.Infrastructure;

namespace tms_mka_v2.Business_Logic.Abstract
{
    public interface IMonitoringVehicleRepo
    {
        List<MonitoringVehicle> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        int Count(FilterInfo filters = null);
        Context.MonitoringDetailSo FindMonitoringDetailSo(string NoSo);
        List<Context.MonitoringDetailSo> FindAllMonitoringSo(int driverId);
        Context.MonitoringVehicle FindByVehicleNo(string vehicleNo);
        string Area(string area);
        void FindAllTruck(List<Models.MonitoringAll> ListModel);
        void FindOnDuty(List<Models.MonitoringOnduty> ListModel);
        void FindOnTime(List<Models.MonitoringOntime> ListModel);
        void FindOnTemp(List<Models.MonitoringOntemp> ListModel);
        void FindService(List<Models.MonitoringService> ListModel);
    }
}