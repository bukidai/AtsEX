using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace UnembeddedResources
{
    public partial class ResourceLocalizer
    {
        private class ResourceSet : Dictionary<string, object>
        {
            public static ResourceSet FromResX(string filePath)
            {
                using (ResXResourceSet resources = new ResXResourceSet(filePath)) return new ResourceSet(resources);
            }

            protected ResourceSet(ResXResourceSet source)
                : base(new Dictionary<string, object>(CreateSortedListFromResX(source)))
            {
            }

            private static Dictionary<string, object> CreateSortedListFromResX(IEnumerable items)
            {
                Dictionary<string, object> genericDictionary = items.
                    Cast<DictionaryEntry>().
                    ToDictionary(x => (string)x.Key, x => x.Value);

                return new Dictionary<string, object>(genericDictionary);
            }
        }
    }
}
