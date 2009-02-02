using System;
using System.IO;


using CPU = Indy.IL2CPU.Assembler.X86;

namespace Indy.IL2CPU.IL.X86 {
	[OpCode(Mono.Cecil.Cil.Code.Stind_R8)]
	public class Stind_R8: Op {
		public Stind_R8(Mono.Cecil.Cil.Instruction instruction, MethodInformation aMethodInfo)
			: base(instruction, aMethodInfo) {
		}
		public override void DoAssemble() {
			Stind_I.Assemble(Assembler, 8);
		}
	}
}