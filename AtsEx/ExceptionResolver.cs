using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx
{
    internal class ExceptionResolver
    {
        public ExceptionResolver()
        {
        }

        public void Resolve(string senderName, Exception exception)
        {
            bool isWrapperException = false;
            switch (exception)
            {
                case AggregateException ex:
                    foreach (Exception innerException in ex.InnerExceptions)
                    {
                        Resolve(senderName, innerException);
                    }
                    isWrapperException = true;
                    break;

                case TypeInitializationException ex:
                    Resolve(senderName, ex.InnerException);
                    isWrapperException = true;
                    break;

                case TargetInvocationException ex:
                    Resolve(senderName, ex.InnerException);
                    isWrapperException = true;
                    break;

                case ReflectionTypeLoadException ex:
                    foreach (Exception innerException in ex.LoaderExceptions)
                    {
                        Resolve(senderName, innerException);
                    }
                    isWrapperException = true;
                    break;
            }

            Throw(senderName, exception, isWrapperException);
        }

        protected virtual void Throw(string senderName, Exception exception, bool isWrapperException) => ExceptionDispatchInfo.Capture(exception).Throw();
    }
}
