using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Threading.Tasks;

namespace tms_mka_v2.Infrastructure
{
    public static class NotificationHelper
    {
        //{ Nama: "Sales Order", Id: "S" },
        //{ Nama: "Planning Order", Id: "P" },
        //{ Nama: "Konfirmasi Planning", Id: "KP" },
        //{ Nama: "Admin Uang Jalan", Id: "A" },
        //{ Nama: "Kasir", Id: "K" }
        
        public static void SendNotif(List<Context.TimeAlert> Listdbalert, string type)
        {
            foreach (var item in Listdbalert)
            {
                
            }
        }
    }
}