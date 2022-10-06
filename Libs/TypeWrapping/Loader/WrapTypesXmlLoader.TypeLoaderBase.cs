using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TypeWrapping
{
    public static partial class WrapTypesXmlLoader
    {
        private abstract class TypeLoaderBase
        {
            protected readonly XElement Root;
            protected readonly string TargetNamespace;

            public bool IsLoaded { get; private set; } = false;

            public TypeLoaderBase(XElement root, string targetNamespace)
            {
                Root = root;
                TargetNamespace = targetNamespace;
            }

            public virtual void LoadAll()
            {
                Load(Root, Enumerable.Empty<XElement>());
                IsLoaded = true;

                void Load(XElement parent, IEnumerable<XElement> parentClassElements)
                {
                    IEnumerable<XElement> enumElements = parent.Elements(TargetNamespace + "Enum");
                    IEnumerable<XElement> classElements = parent.Elements(TargetNamespace + "Class");

#if DEBUG
                    IEnumerable<XElement> allElements = parent.Elements();
                    if (allElements.Any(element => !enumElements.Contains(element) && !classElements.Contains(element)))
                    {
                        throw new NotImplementedException();
                    }
#endif

                    LoadEnums(enumElements, parentClassElements);
                    LoadClasses(classElements, parentClassElements);

                    foreach (XElement classElement in classElements)
                    {
                        XElement childrenElement = classElement.Element(TargetNamespace + "Children");
                        if (!(childrenElement is null)) Load(childrenElement, parentClassElements.Concat(new XElement[] { classElement }));
                    }
                }
            }

            protected abstract void LoadEnums(IEnumerable<XElement> enumElements, IEnumerable<XElement> parentClassElements);

            protected abstract void LoadClasses(IEnumerable<XElement> classElements, IEnumerable<XElement> parentClassElements);
        }
    }
}
