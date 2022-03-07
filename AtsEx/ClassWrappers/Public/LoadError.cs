using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.ClassWrappers
{
    public class LoadError : ClassWrapper, ILoadError
    {
        public LoadError(object src) : base(src)
        {
			IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<ILoadError>();

			TextGetMethod = members.GetSourcePropertyGetterOf(nameof(Text));
			TextSetMethod = members.GetSourcePropertySetterOf(nameof(Text));

			SenderFileNameGetMethod = members.GetSourcePropertyGetterOf(nameof(SenderFileName));
			SenderFileNameSetMethod = members.GetSourcePropertySetterOf(nameof(SenderFileName));

			LineIndexGetMethod = members.GetSourcePropertyGetterOf(nameof(LineIndex));
			LineIndexSetMethod = members.GetSourcePropertySetterOf(nameof(LineIndex));

			CharIndexGetMethod = members.GetSourcePropertyGetterOf(nameof(CharIndex));
			CharIndexSetMethod = members.GetSourcePropertySetterOf(nameof(CharIndex));
		}

		public LoadError(Assembly assembly, string text, string senderFileName, int lineIndex, int charIndex) : this(CreateSource(assembly, text, senderFileName, lineIndex, charIndex))
        {
        }

		protected static object CreateSource(Assembly assembly, string text, string senderFileName, int lineIndex, int charIndex)
		{
			ConstructorInfo constructor = assembly.GetType("ai").GetConstructor(new Type[] { typeof(string), typeof(string), typeof(int), typeof(int) });
			object src = constructor.Invoke(new object[] { text, senderFileName, lineIndex, charIndex });

			return src;
		}

		protected MethodInfo TextGetMethod;
		protected MethodInfo TextSetMethod;
		public string Text
		{
			get => TextGetMethod.Invoke(Src, new object[0]);
			internal set => TextSetMethod.Invoke(Src, new object[] { value });
		}

		protected MethodInfo SenderFileNameGetMethod;
		protected MethodInfo SenderFileNameSetMethod;
		public string SenderFileName
		{
			get => SenderFileNameGetMethod.Invoke(Src, new object[0]);
			internal set => SenderFileNameSetMethod.Invoke(Src, new object[] { value });
		}

		protected MethodInfo LineIndexGetMethod;
		protected MethodInfo LineIndexSetMethod;
		public int LineIndex
		{
			get => LineIndexGetMethod.Invoke(Src, new object[0]);
			internal set => LineIndexSetMethod.Invoke(Src, new object[] { value });
		}

		protected MethodInfo CharIndexGetMethod;
		protected MethodInfo CharIndexSetMethod;
		public int CharIndex
		{
			get => CharIndexGetMethod.Invoke(Src, new object[0]);
			internal set => CharIndexSetMethod.Invoke(Src, new object[] { value });
		}
	}
}
