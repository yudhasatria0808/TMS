using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using tms_mka_v2.Context;

namespace tms_mka_v2.Models
{
    public class OrderHistory
    {
        public int Id { get; set; }
        public string StatusFlow { get; set; }
        public DateTime FlowDate { get; set; }
        public DateTime SavedAt { get; set; }
        public DateTime ProcessedAt { get; set; }
        public string PIC { get; set; }
        
        public OrderHistory()
        {

        }
        public OrderHistory(Context.OrderHistory dbitem, string pic)
        {
            StatusFlow = dbitem.StatusFlow == 1 ? "MARKETING" : dbitem.StatusFlow == 2 ? "PLANNING" : dbitem.StatusFlow == 3 ? "KONFIRMASI" : dbitem.StatusFlow == 4 ? "ADMIN UANG JALAN" : dbitem.StatusFlow == 5 ? "TRANSFER" : dbitem.StatusFlow == 6 ? "KAS" : dbitem.StatusFlow == 7 ? "SETTLEMENT" : "BATAL ORDER";
            FlowDate = dbitem.FlowDate;
            SavedAt = dbitem.SavedAt;
            ProcessedAt = dbitem.ProcessedAt;
            PIC = pic;
        }
    }
}