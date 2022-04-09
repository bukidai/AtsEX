using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.ClassWrappers
{
    internal class AssistantText : AssistantTextBase, IAssistantText
    {
        static AssistantText()
        {
            IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<IAssistantText>();

            ColorGetMethod = members.GetSourcePropertyGetterOf(nameof(Color));
            ColorSetMethod = members.GetSourcePropertySetterOf(nameof(Color));

            TextGetMethod = members.GetSourcePropertyGetterOf(nameof(Text));
            TextSetMethod = members.GetSourcePropertySetterOf(nameof(Text));
        }

        public AssistantText(object src) : base(src)
        {
        }

        protected static MethodInfo ColorGetMethod;
        protected static MethodInfo ColorSetMethod;
        public Color Color
        {
            get => ColorGetMethod.Invoke(Src, null);
            set => ColorSetMethod.Invoke(Src, new object[] { value });
        }

        protected static MethodInfo TextGetMethod;
        protected static MethodInfo TextSetMethod;
        public string Text
        {
            get => TextGetMethod.Invoke(Src, null);
            set => TextSetMethod.Invoke(Src, new object[] { value });
        }
    }
}
