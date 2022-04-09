using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Mackoy.Bvets;

using Automatic9045.AtsEx.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.ClassWrappers
{
    internal class AssistantTextBase : ClassWrapper, IAssistantTextBase
    {
        static AssistantTextBase()
        {
            IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<IAssistantTextBase>();

            AssistantSettingsGetMethod = members.GetSourcePropertyGetterOf(nameof(AssistantSettings));

            BackgroundColorGetMethod = members.GetSourcePropertyGetterOf(nameof(BackgroundColor));
            BackgroundColorSetMethod = members.GetSourcePropertySetterOf(nameof(BackgroundColor));

            TextAreaGetMethod = members.GetSourcePropertyGetterOf(nameof(TextArea));

            DrawMethod = members.GetSourceMethodOf(nameof(Draw));
        }

        public AssistantTextBase(object src) : base(src)
        {
        }

        protected static MethodInfo AssistantSettingsGetMethod;
        public AssistantSettings AssistantSettings
        {
            get => AssistantSettingsGetMethod.Invoke(Src, null);
        }

        protected static MethodInfo BackgroundColorGetMethod;
        protected static MethodInfo BackgroundColorSetMethod;
        public Color BackgroundColor
        {
            get => BackgroundColorGetMethod.Invoke(Src, null);
            set => BackgroundColorSetMethod.Invoke(Src, new object[] { value });
        }

        protected static MethodInfo TextAreaGetMethod;
        public Rectangle TextArea
        {
            get => TextAreaGetMethod.Invoke(Src, null);
        }

        protected static MethodInfo DrawMethod;
        public void Draw()
        {
            DrawMethod.Invoke(Src, null);
        }
    }
}
