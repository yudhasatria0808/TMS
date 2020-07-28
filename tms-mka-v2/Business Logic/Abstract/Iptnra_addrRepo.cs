﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tms_mka_v2.Context;
using tms_mka_v2.Infrastructure;

namespace tms_mka_v2.Business_Logic.Abstract
{
    public interface Iptnra_addrRepo
    {
        ptnra_addr FindByPK(int id);
        void save(CustomerAddress dbitem);
        void updateCustomerAddress(ptnra_addr dbitem);
    }
}