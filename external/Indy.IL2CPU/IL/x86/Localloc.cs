using System;
using System.Collections.Generic;
using System.IO;
using Indy.IL2CPU.Assembler.X86;
using CPU = Indy.IL2CPU.Assembler;
using CPUx86 = Indy.IL2CPU.Assembler.X86;

namespace Indy.IL2CPU.IL.X86 {
	[OpCode(Mono.Cecil.Cil.Code.Localloc)]
	public class Localloc: Op {
        public const string LocAllocCountMethodDataEntry = "LocAllocCount";
        public const string LocAllicItemMethodDataEntryTemplate = "LocAllocItem_L{0}";
        public static void ScanOp(Mono.Cecil.Cil.Instruction instruction, MethodInformation aMethodInfo, SortedList<string, object> aMethodData) {
            // xCurrentMethodLocallocCount contains the number of LocAlloc occurrences
            int xCurrentMethodLocallocCount = 0;
            if (aMethodData.ContainsKey(LocAllocCountMethodDataEntry)) {
                xCurrentMethodLocallocCount = (int)aMethodData[LocAllocCountMethodDataEntry];
            }
            xCurrentMethodLocallocCount++;
            aMethodData[LocAllocCountMethodDataEntry] = xCurrentMethodLocallocCount;
            string xCurrentItem = String.Format(LocAllicItemMethodDataEntryTemplate,
                                                instruction.Offset);
#if DEBUG
            if (aMethodData.ContainsKey(xCurrentItem)) {
                throw new Exception("Localloc item already exists in MethodData!");
            }
#endif
            aMethodData.Add(xCurrentItem, xCurrentMethodLocallocCount);
        }

	    private readonly int mLocallocOffset = 0;
		public Localloc(Mono.Cecil.Cil.Instruction instruction, MethodInformation aMethodInfo)
			: base(instruction, aMethodInfo) {
		    mLocallocOffset = (int)aMethodInfo.MethodData[String.Format(LocAllicItemMethodDataEntryTemplate,
		                                                                instruction.Offset)];
		    mLocallocOffset *= 4;
		    mLocallocOffset += aMethodInfo.LocalsSize;

		}
        public override void DoAssemble() {
            new CPUx86.Call { DestinationLabel = new CPU.Label(RuntimeEngineRefs.Heap_AllocNewObjectRef).Name };
            new CPUx86.Move {
                DestinationReg = CPUx86.Registers.EBP,
                DestinationIsIndirect = true,
                DestinationDisplacement = mLocallocOffset,
                SourceReg = Registers.ESP,
                SourceIsIndirect = true
            };
        }
	}
}