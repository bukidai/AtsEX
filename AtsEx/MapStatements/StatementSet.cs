using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.Utilities;
using AtsEx.PluginHost.MapStatements;

namespace AtsEx.MapStatements
{
    internal sealed partial class StatementSet : IStatementSet, IEnumerable<Statement>
    {
        private readonly IDictionary<Identifier, IReadOnlyList<Statement>> Statements;

        public StatementSet(IDictionary<Identifier, IReadOnlyList<Statement>> statements)
        {
            Statements = statements;
        }

        public IReadOnlyList<IStatement> GetAll(Identifier identifier) => Statements.TryGetValue(identifier, out IReadOnlyList<Statement> result) ? result : new List<Statement>();

        public IEnumerator<Statement> GetEnumerator() => new EnumerableInDictionaryEnumerator<Identifier, Statement>(Statements);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
