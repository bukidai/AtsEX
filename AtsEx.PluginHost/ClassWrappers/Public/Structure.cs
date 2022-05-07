using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public class Structure : LocatableMapObject
    {
        static Structure()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<Structure>();

            Constructor1 = members.GetSourceConstructor(new Type[] { typeof(double), typeof(string), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(int), typeof(double), typeof(Model) });
            Constructor2 = members.GetSourceConstructor(new Type[] { typeof(double), typeof(string), typeof(int), typeof(double), typeof(Model) });

            ModelGetMethod = members.GetSourcePropertyGetterOf(nameof(Model));
            ModelSetMethod = members.GetSourcePropertySetterOf(nameof(Model));
        }

        protected Structure(object src) : base(src)
        {
        }

        public static Structure FromSource(object src)
        {
            if (src is null) return null;
            return new Structure(src);
        }

        private static ConstructorInfo Constructor1;
        public Structure(double location, string trackKey, double x, double y, double z, double dx, double dy, double dz, int tilt, double span, Model model)
            : this(Constructor1.Invoke(new object[] { location, trackKey, x, y, z, dx, dy, dz, tilt, span, model }))
        {
        }

        private static ConstructorInfo Constructor2;
        public Structure(double location, string trackKey, int tilt, double span, Model model)
            : this(Constructor2.Invoke(new object[] { location, trackKey, tilt, span, model }))
        {
        }

        private static MethodInfo ModelGetMethod;
        private static MethodInfo ModelSetMethod;
        public Model Model
        {
            get => ClassWrappers.Model.FromSource(ModelGetMethod.Invoke(Src, null));
            set => ModelSetMethod.Invoke(Src, new object[] { value.Src });
        }
    }
}
