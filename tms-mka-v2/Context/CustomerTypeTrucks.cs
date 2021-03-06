﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace tms_mka_v2.Context
{
    public class CustomerTypeTrucks
    {
        public CustomerTypeTrucks()
        {

        }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        [ForeignKey("JenisTrucks")]
        public int? JenisTruck { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        

        public virtual Customer Customer { get; set; }
        public virtual JenisTrucks JenisTrucks { get; set; }
        
    }
}