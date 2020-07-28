﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tms_mka_v2.Context;
using tms_mka_v2.Infrastructure;

namespace tms_mka_v2.Business_Logic.Abstract
{
    public interface IAuditrailRepo
    {
        void save(Auditrail dbitem);
        List<Auditrail> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        int Count(FilterInfo filters = null);
        Auditrail FindByPK(int id);
        void delete(Auditrail dbitem);
        void saveOrderHistory(Context.SalesOrder so);
        void savePlanningHistory(Context.SalesOrder so);
        void saveKonfirmasiHistory(Context.SalesOrder so);
        void saveAUJHistory(Context.SalesOrder so, Context.HistoryJalanTruck hjt);
        void saveKasirTfHistory(Context.SalesOrder so, string strQuery);
        void saveKasirKasHistory(Context.SalesOrder so, string strQuery);
        void saveSettRegHistory(Context.SalesOrder so);
        void saveBOHistory(Context.SalesOrder so);
        void saveCustomerQuery(Customer dbitem, int id, string add=null);
        void saveCustomerAddressQuery(CustomerAddress dbitem, int id, string add=null);
        void saveDelCustomerAddressQuery(CustomerAddress dbitem, int id);
        void saveCustomerProductQuery(CustomerProductType dbitem, int id);
        void saveCustomerAttachmentQuery(CustomerAttachment dbitem, int id);
        void saveDelCustomerAttachmentQuery(CustomerAttachment dbitem, int id);
        void saveCustomerBillingQuery(CustomerBilling dbitem, int id);
        void saveCustomerJadwalBillingQuery(CustomerJadwalBilling dbitem, int id);
        void saveDelCustomerJadwalBillingQuery(CustomerBilling dbitem, int id);
        void saveDelCustomerBillingQuery(CustomerBilling dbitem, int id);
        void saveDelAllCustomerBillingQuery(Customer dbitem, int id);
        void saveCustomerCreditStatusQuery(CustomerCreditStatus dbitem, int id);
        void saveUpdCustomerCreditStatusQuery(CustomerCreditStatus dbitem, int id);
        void saveCustomerCreditStatusHistoryQuery(CustomerCreditStatusHistory dbitem, int id);
        void saveDelCustomerLoadingAddressQuery(CustomerLoadingAddress dbitem, int id);
        void saveDelAllCustomerLoadingAddressQuery(Customer dbitem, int id);
        void saveCustomerLoadingAddressQuery(CustomerLoadingAddress dbitem, int id, string add=null);
        void saveDelAllCustomerNotifRuteQuery(CustomerNotification dbitem, int id);
        void saveDelCustomerNotifRuteQuery(CustomerNotifRute dbitem, int id);
        void saveCustomerNotifRuteQuery(CustomerNotifRute dbitem, int id);
        void saveDelAllCustomerNotifTruckQuery(CustomerNotification dbitem, int id);
        void saveDelCustomerNotifTruckQuery(CustomerNotifTruck dbitem, int id);
        void saveCustomerNotifTruckQuery(CustomerNotifTruck dbitem, int id);
        void saveDelCustomerNotificationQuery(CustomerNotification dbitem, int id);
        void saveDelAllCustomerNotificationQuery(Customer dbitem, int id);
        void saveCustomerNotificationQuery(CustomerNotification dbitem, int id);
        void saveCustomerPPNQuery(CustomerPPN dbitem, int id);
        void saveDelCustomerPPNQuery(CustomerPPN dbitem, int id);
        void saveUpdCustomerPPNQuery(CustomerPPN dbitem, int id);
        void saveUpdCustomerPICQuery(CustomerPic dbitem, int id);
        void saveDelCustomerPICQuery(CustomerPic dbitem, int id);
        void saveDelAllCustomerPICQuery(Customer dbitem, int id);
        void saveCustomerPICQuery(CustomerPic dbitem, int id);
        void saveUpdCustomerProductTypeQuery(CustomerProductType dbitem, int id);
        void saveDelCustomerProductTypeQuery(CustomerProductType dbitem, int id);
        void saveDelAllCustomerProductTypeQuery(Customer dbitem, int id);
        void saveCustomerProductTypeQuery(CustomerProductType dbitem, int id);
        void saveSalesOrderPickupUnLoadingAddQuery(SalesOrderPickupUnLoadingAdd dbitem, int id);
        void saveDelAllSalesOrderPickupUnLoadingAddQuery(SalesOrderPickup dbitem, int id);
        void saveUpdSalesOrderProsesKonsolidasiQuery(SalesOrderProsesKonsolidasi dbitem, int id);
        void saveSalesOrderProsesKonsolidasiQuery(SalesOrderProsesKonsolidasi dbitem, int id);
        void saveSalesOrderProsesKonsolidasiCommentQuery(SalesOrderProsesKonsolidasi dbitem, int id);
        void saveSalesOrderProsesKonsolidasiItemQuery(SalesOrderProsesKonsolidasiItem dbitem, int id);
        void saveSalesOrderProsesKonsolidasiLoadingAddQuery(SalesOrderProsesKonsolidasiLoadingAdd dbitem, int id);
        void saveSalesOrderProsesKonsolidasiUnLoadingAddQuery(SalesOrderProsesKonsolidasiUnLoadingAdd dbitem, int id);
        void saveSettlementRegulerBiayaTambahanQuery(SettlementRegulerTambahanBiaya dbitem, int id);
        void saveDelAllSettlementRegulerBiayaTambahanQuery(SettlementReguler dbitem, int id);
        void saveDelAllSpkQuery(Workshop dbitem, int id);
        void saveUpdCustomerSupplierQuery(CustomerSupplier dbitem, int id);
        void saveDelCustomerSupplierQuery(CustomerSupplier dbitem, int id);
        void saveCustomerSupplierQuery(CustomerSupplier dbitem, int id);
        void SetAuditTrail(string query, string modul, string act, int user_id);
    }
}