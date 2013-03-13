using System;
using System.Collections.Generic;

namespace GrEmit.StackMutators
{
    internal class LdlenStackMutator : StackMutator
    {
        public override void Mutate(GroboIL il, ILInstructionParameter parameter, ref Stack<Type> stack)
        {
            CheckNotEmpty(il, stack);
            var peek = stack.Pop();
            if (!peek.IsArray && peek != typeof(Array))
                throw new InvalidOperationException("An array expected but was '" + peek + "'");
            stack.Push(typeof(int));
        }
    }
}