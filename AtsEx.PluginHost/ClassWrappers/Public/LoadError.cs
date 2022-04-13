using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public sealed class LoadError : ClassWrapper
    {
		static LoadError()
		{
			BveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<LoadError>();

			Constructor = members.GetSourceConstructorOf(new Type[] { typeof(string), typeof(string), typeof(int), typeof(int) });

			TextGetMethod = members.GetSourcePropertyGetterOf(nameof(Text));
			TextSetMethod = members.GetSourcePropertySetterOf(nameof(Text));

			SenderFileNameGetMethod = members.GetSourcePropertyGetterOf(nameof(SenderFileName));
			SenderFileNameSetMethod = members.GetSourcePropertySetterOf(nameof(SenderFileName));

			LineIndexGetMethod = members.GetSourcePropertyGetterOf(nameof(LineIndex));
			LineIndexSetMethod = members.GetSourcePropertySetterOf(nameof(LineIndex));

			CharIndexGetMethod = members.GetSourcePropertyGetterOf(nameof(CharIndex));
			CharIndexSetMethod = members.GetSourcePropertySetterOf(nameof(CharIndex));
		}

        public LoadError(object src) : base(src)
        {
		}

		private static ConstructorInfo Constructor;
		public LoadError(string text, string senderFileName, int lineIndex, int charIndex) : this(Constructor.Invoke(new object[] { text, senderFileName, lineIndex, charIndex }))
        {
        }

		private static MethodInfo TextGetMethod;
		private static MethodInfo TextSetMethod;
		public string Text
		{
			get => TextGetMethod.Invoke(Src, new object[0]);
			internal set => TextSetMethod.Invoke(Src, new object[] { value });
		}

		private static MethodInfo SenderFileNameGetMethod;
		private static MethodInfo SenderFileNameSetMethod;
		public string SenderFileName
		{
			get => SenderFileNameGetMethod.Invoke(Src, new object[0]);
			internal set => SenderFileNameSetMethod.Invoke(Src, new object[] { value });
		}

		private static MethodInfo LineIndexGetMethod;
		private static MethodInfo LineIndexSetMethod;
		public int LineIndex
		{
			get => LineIndexGetMethod.Invoke(Src, new object[0]);
			internal set => LineIndexSetMethod.Invoke(Src, new object[] { value });
		}

		private static MethodInfo CharIndexGetMethod;
		private static MethodInfo CharIndexSetMethod;
		public int CharIndex
		{
			get => CharIndexGetMethod.Invoke(Src, new object[0]);
			internal set => CharIndexSetMethod.Invoke(Src, new object[] { value });
		}
	}
}
