﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace tms_mka_v2.Context
{
    public class SalesOrderPickupLoadingAdd
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Key]
        [Column(Order = 1)]
        [ForeignKey("SalesOrderPickup")]
        public int SalesOrderPickupid { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("CustomerLoadingAddress")]
        public int? CustomerLoadingAddressId { get; set; }
        [Key]
        [Column(Order = 3)]
        [ForeignKey("CustomerLoadingAddress")]
        public int? CustomerId { get; set; }
        public int urutan { get; set; }
        public bool IsSelect { get; set; }

        public virtual SalesOrderPickup SalesOrderPickup { get; set; }
        public virtual CustomerLoadingAddress CustomerLoadingAddress { get; set; }
    }
}