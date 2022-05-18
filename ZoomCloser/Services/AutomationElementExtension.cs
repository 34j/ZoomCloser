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
                // Some values does not throw Exceptions when they are not available. Therefore, we need to check more properties to be sure that the element is alive
                var t = element.Current.IsEnabled;
                var n = element.Current.Name;
                //var p = element.Current.ProcessId;
            }
            catch (ElementNotAvailableException)
            {
                return false;
            }
            catch (ElementNotEnabledException)
            {
                return false;
            }
            return true;
        }
    }
}
