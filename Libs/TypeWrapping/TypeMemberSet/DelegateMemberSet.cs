using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeWrapping
{
    internal class DelegateMemberSet : TypeMemberSetBase
    {
        public DelegateMemberSet(Type wrapperType, Type originalType) : base(wrapperType, originalType)
        {
        }
    }
}
