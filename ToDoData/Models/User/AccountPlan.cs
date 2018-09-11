using System;
using System.Collections.Generic;
using System.Text;

namespace TodoData.Models.User
{
    public class AccountPlan
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual bool IsChargeable { get; set; }

        public virtual double? Amount { get; set; }

        public virtual int CurrencyId { get; set; }

        public virtual DateTime LastUpdate { get; set; }
    }
}
