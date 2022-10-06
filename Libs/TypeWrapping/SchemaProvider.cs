using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TypeWrapping
{
    public static class SchemaProvider
    {
        private static readonly Assembly Assembly;

        static SchemaProvider()
        {
            Assembly = Assembly.GetExecutingAssembly();
        }

        public static Stream GetSchemaStream()
        {
            Stream schemaStream = Assembly.GetManifestResourceStream($"{typeof(SchemaProvider).Namespace}.WrapTypesXmlSchema.xsd");
            return schemaStream;
        }
    }
}
