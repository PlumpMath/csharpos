using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Translator
{
    public interface ITranslator
    {
        List<MethodReference> CollectMethodReferences(MethodDefinition method);
    }
}
