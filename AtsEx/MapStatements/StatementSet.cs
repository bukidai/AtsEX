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
        private readonly IDictionary<Identifier, IEnumerable<Statement>> Statements;

        public StatementSet(IDictionary<Identifier, IEnumerable<Statement>> statements)
        {
            Statements = statements;
        }

        public IEnumerable<IStatement> GetAll(Identifier identifier) => Statements.TryGetValue(identifier, out IEnumerable<Statement> result) ? result : Enumerable.Empty<Statement>();

        public IEnumerator<Statement> GetEnumerator() => new EnumerableInDictionaryEnumerator<Identifier, Statement>(Statements);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
