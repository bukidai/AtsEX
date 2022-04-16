using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx
{
    internal class ServiceCollection : IDisposable
    {
        protected SortedList<ComparableType, object> RegisteredServices { get; } = new SortedList<ComparableType, object>();
        public bool IsLocked { get; protected set; } = false;

        public virtual void Register<TImplementation>(Func<TImplementation> serviceConstructor)
        {
            Type type = typeof(TImplementation);

            if (IsLocked)
            {
                throw new Exception("サービス コレクションはロック済みです。");
            }
            else if (!type.IsInterface)
            {
                throw new ArgumentException($"{nameof(TImplementation)} '{type}' はインターフェースではありません。");
            }

            ComparableType comparableType = new ComparableType(type);

            if (RegisteredServices.ContainsKey(comparableType))
            {
                throw new ArgumentException($"{nameof(TImplementation)} '{type}' は既に登録されています。");
            }

            RegisteredServices.Add(comparableType, serviceConstructor());
        }

        public virtual void Lock()
        {
            IsLocked = true;
        }

        public TImplementation GetService<TImplementation>()
        {
#if DEBUG
            Type type = typeof(TImplementation);
            if (!type.IsInterface)
            {
                throw new ArgumentException($"{nameof(TImplementation)} '{type}' はインターフェースではありません。");
            }
#endif
            ComparableType comparableType = new ComparableType(typeof(TImplementation));
            return (TImplementation)RegisteredServices[comparableType];
        }

        public void Dispose()
        {
            foreach (KeyValuePair<ComparableType, object> service in RegisteredServices)
            {
                if (service.Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }

    internal struct ComparableType : IComparable
    {
        public Type Type { get; }
        public int HashCode { get; }

        public ComparableType(Type type)
        {
            Type = type;
            HashCode = Type.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ComparableType type)
            {
                return Type == type.Type;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode() => Type.GetHashCode();

        public int CompareTo(object obj)
        {
            if (obj is ComparableType type)
            {
                return HashCode - type.HashCode;
            }
            else
            {
                throw new ArgumentException($"obj が {nameof(ComparableType)} ではありません。");
            }
        }
    }
}
