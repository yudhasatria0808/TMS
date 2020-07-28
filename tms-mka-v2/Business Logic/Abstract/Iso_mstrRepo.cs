using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tms_mka_v2.Context;
using tms_mka_v2.Infrastructure;
using tms_mka_v2.Models;

namespace tms_mka_v2.Business_Logic.Abstract
{
    public interface Iso_mstrRepo
    {
		void saveSoMstr(SalesOrder soitem, string username, string guid, int customerId, decimal harga);
		void saveSoDet(SalesOrder soitem, string username, string guid, string ship_guid);
		void saveSoShipMstr(SalesOrder soitem, string username, string guid, string ship_guid);
		void saveSoShipDet(SalesOrder soitem, string username, string guid, string ship_guid);
		so_mstr FindByPK(string code);
		so_mstr FindSoDet(string code);
		void UpdateSoMstrVehicle(so_mstr dbitem);
    }
}