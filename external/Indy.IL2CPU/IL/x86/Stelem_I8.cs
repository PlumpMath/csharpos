using System;
using System.IO;


using CPU = Indy.IL2CPU.Assembler;
using CPUx86 = Indy.IL2CPU.Assembler.X86;

namespace Indy.IL2CPU.IL.X86 {
	[OpCode(Mono.Cecil.Cil.Code.Stelem_I8)]
	public class Stelem_I8: Op {
		public Stelem_I8(Mono.Cecil.Cil.Instruction instruction, MethodInformation aMethodInfo)
			: base(instruction, aMethodInfo) {
		}
		
		public override void DoAssemble() {
			Stelem_Ref.Assemble(Assembler, 8);
		}
	}
}