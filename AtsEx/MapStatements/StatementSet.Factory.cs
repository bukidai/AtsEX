using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;
using BveTypes.ClassWrappers.Extensions;

using AtsEx.PluginHost.MapStatements;

namespace AtsEx.MapStatements
{
    internal partial class StatementSet
    {
        public static StatementSet Create(IDictionary<string, MapObjectList> repeatedStructures, IDictionary<Model, string> loadedModels, IDictionary<string, Train> targetTrains)
        {
            List<MonitorableTrain> trains = targetTrains.Select(item => new MonitorableTrain(item.Key, item.Value)).ToList();

            ConcurrentDictionary<Identifier, IReadOnlyList<Statement>> statements = new ConcurrentDictionary<Identifier, IReadOnlyList<Statement>>();
            foreach (KeyValuePair<string, MapObjectList> sameKeyRepeaters in repeatedStructures)
            {
                for (int i = 0; i < sameKeyRepeaters.Value.Count; i++)
                {
                    if (!(sameKeyRepeaters.Value[i] is RepeatedStructure repeater)) continue;
                    if (repeater.Models is null) continue;

                    string argument = sameKeyRepeaters.Key;
                    Identifier[] declaration = GetDeclaration(repeater.Models);
                    if (declaration.Length == 0 || declaration[0] is null) continue;

                    Identifier name = declaration[0];
                    if (name.Namespace is null || !name.Namespace.IsChildOf(Namespace.Root)) continue;
                    Identifier[] additionalDeclaration = declaration.Skip(1).ToArray();

                    double to = sameKeyRepeaters.Value.Count <= i + 1 || sameKeyRepeaters.Value[i + 1] is null ? double.PositiveInfinity : sameKeyRepeaters.Value[i + 1].Location;

                    Statement statement = new Statement(name, additionalDeclaration, argument, repeater, to);

                    List<Statement> list = statements.GetOrAdd(name, new List<Statement>()) as List<Statement>;
                    list.Add(statement);
                }
            }

            return new StatementSet(statements, trains);


            Identifier[] GetDeclaration(WrappedList<Model> models)
            {
                Identifier[] result = new Identifier[models.Count];
                for (int i = 0; i < models.Count; i++)
                {
                    result[i] =
                        models[i] is null ||
                        !loadedModels.TryGetValue(models[i], out string structureKey) ||
                        !Identifier.TryParse(structureKey, out Identifier identifier) ? null : identifier;
                }

                return result;
            }
        }

        public static StatementSet Create(IDictionary<string, MapObjectList> repeatedStructures, IDictionary<string, Model> loadedModels, IDictionary<string, Train> targetTrains)
            => Create(repeatedStructures, loadedModels.Where(item => !(item.Value is null)).ToDictionary(item => item.Value, item => item.Key), targetTrains);
    }
}
