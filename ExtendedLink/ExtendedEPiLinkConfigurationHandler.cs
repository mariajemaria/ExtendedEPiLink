using System.Collections.Generic;

namespace MarijasPlayground.ExtendedLink
{
    public class ExtendedEPiLinkConfigurationHandler
    {
        public IDictionary<string, object> GetConfigurationOptions()
        {
            var dictionary = new Dictionary<string, object>();
            dictionary["extendedepilinkmodel_type"] = typeof(ExtendedEPiLinkModel).FullName;
            return dictionary;
        }
    }
}
