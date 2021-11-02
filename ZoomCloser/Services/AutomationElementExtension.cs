using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace ZoomCloser.Services
{
    public static class AutomationElementExtension
    {
        public static bool IsAlive(this AutomationElement element)
        {
            if (element == null)
            {
                return false;
            }
            try
            {
                string t = element.Current.Name;
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
