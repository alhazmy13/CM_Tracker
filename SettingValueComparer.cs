using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace falcon.cmtracker
{
    public class SettingValueComparer : IEqualityComparer<SettingValue>
    {
        public bool Equals(SettingValue x, SettingValue y)
        {
            return x.Account.Equals(y.Account) && x.Boss.Equals(y.Boss); 
        }

        public int GetHashCode(SettingValue obj)
        {
            return (obj.Account.GetHashCode() * 397) + obj.Boss.GetHashCode();
        }
    }
}
