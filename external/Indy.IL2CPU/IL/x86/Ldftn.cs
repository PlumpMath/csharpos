using System;
using System.Collections.Generic;
using System.Linq;


using CPU = Indy.IL2CPU.Assembler;
using CPUx86 = Indy.IL2CPU.Assembler.X86;
using System.Reflection;
using Indy.IL2CPU.Assembler;
using Mono.Cecil;

namespace Indy.IL2CPU.IL.X86
{
    [OpCode(OpCodeEnum.Ldftn)]
    public class Ldftn : Op
    {
        private string mFunctionLabel;

        public static void ScanOp(ILReader aReader, MethodInformation aMethodInfo, SortedList<string, object> aMethodData)
        {
            MethodDefinition xMethodRef = aReader.OperandValueMethod;
            if (xMethodRef == null)
            {
                throw new Exception("Unable to determine Method!");
            }
            Engine.QueueMethod(xMethodRef);
        }

        public Ldftn(Mono.Cecil.Cil.Instruction instruction, MethodInformation aMethodInfo)
            : base(instruction, aMethodInfo)
        {
            var methodRef = aReader.OperandValueMethod;
            if (methodRef == null)
            {
                throw new Exception("Unable to determine Method!");
            }
            mFunctionLabel = CPU.Label.GenerateLabelName(methodRef);
            Engine.QueueMethod(methodRef);
        }

        public override void DoAssemble()
        {
            new CPUx86.Push { DestinationRef = new ElementReference(mFunctionLabel) };
            Assembler.StackContents.Push(new StackContent(4, true, false, false));
        }
    }
}