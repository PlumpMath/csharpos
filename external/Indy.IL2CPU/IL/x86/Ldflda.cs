using System;
using System.Collections.Generic;
using System.IO;


using CPUx86 = Indy.IL2CPU.Assembler.X86;	    
using System.Reflection;
using Mono.Cecil;

namespace Indy.IL2CPU.IL.X86
{
    [OpCode(Mono.Cecil.Cil.Code.Ldflda)]
    public class Ldflda : Op
    {
        private TypeInformation mType;
        private TypeInformation.Field mField;
        public static void ScanOp(Mono.Cecil.Cil.Instruction instruction, MethodInformation aMethodInfo, SortedList<string, object> aMethodData)
        {
            var xField = instruction.Operand as FieldDefinition;
            if (xField == null)
            {
                throw new Exception("Field not found!");
            }
            Engine.RegisterType(xField.DeclaringType);
            Engine.RegisterType(xField.FieldType);
        }

        public Ldflda(Mono.Cecil.Cil.Instruction instruction, MethodInformation aMethodInfo)
            : base(instruction, aMethodInfo)
        {
            var field = instruction.Operand as FieldReference;
            if (field == null)
            {
                throw new Exception("Field not found!");
            }
            string xFieldId = field.Name;
            mType = Engine.GetTypeInfo(field.DeclaringType);
            mField = mType.Fields[xFieldId];
        }

        public override void DoAssemble()
        {
            Ldflda(Assembler, mType, mField);
        }
    }
}