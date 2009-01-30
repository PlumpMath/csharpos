using System;
using System.Collections.Generic;
using System.IO;


using CPU = Indy.IL2CPU.Assembler;
using System.Reflection;
using Mono.Cecil;

namespace Indy.IL2CPU.IL.X86
{
    [OpCode(OpCodeEnum.Ldfld)]
    public class Ldfld : Op
    {
        private readonly TypeInformation.Field mField;
        private readonly TypeInformation mType;
        public static void ScanOp(Mono.Cecil.Cil.Instruction instruction, MethodInformation aMethodInfo, SortedList<string, object> aMethodData)
        {
            var xField = instruction.Operand;
            if (xField == null)
            {
                throw new Exception("Field not found!");
            }
            Engine.RegisterType(xField.DeclaringType);
            Engine.RegisterType(xField.FieldType);
        }

        public Ldfld(TypeInformation.Field aField)
            : base(null, null)
        {
            mField = aField;
        }
        public Ldfld(Mono.Cecil.Cil.Instruction instruction, MethodInformation aMethodInfo)
            : base(instruction, aMethodInfo)
        {
            var field = (FieldDefinition)instruction.Operand;
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
            Ldfld(Assembler, mType, mField);
        }
    }
}