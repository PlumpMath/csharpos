using System;
using System.IO;


using CPU = Indy.IL2CPU.Assembler.X86;

namespace Indy.IL2CPU.IL.X86 {
    [OpCode(Mono.Cecil.Cil.Code.Volatile)]
	public class Volatile: Op {
		public Volatile(Mono.Cecil.Cil.Instruction instruction, MethodInformation aMethodInfo)
			: base(instruction, aMethodInfo) {
		}
		public override void DoAssemble() {
			// todo: implement correct Volatile support
		}
	}
}