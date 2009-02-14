﻿#region license
//
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using System;

using Mono.Cecil.Cil;

using Cecil.Decompiler.Ast;

namespace Cecil.Decompiler.Steps {

	class SelfAssignement : BaseCodeTransformer, IDecompilationStep {

		public static readonly IDecompilationStep Instance = new SelfAssignement ();

		static readonly Pattern.ICodePattern SelfAssignmentPattern = new Pattern.Assignment {
			Target = new Pattern.VariableReference {
				Bind = var => new Pattern.MatchData ("Variable", var.Variable)
			},
			Expression = new Pattern.Binary {
				Bind = binary => new Pattern.MatchData ("Operator", binary.Operator),
				Left = new Pattern.VariableReference {
					Variable = new Pattern.ContextData { Name = "Variable" }
				},
				Right = new Pattern.Literal {
					Value = 1
				}
			}
		};

		public override ICodeNode VisitAssignExpression (AssignExpression node)
		{
			var result = Pattern.CodePattern.Match (SelfAssignmentPattern, node);
			if (!result.Success)
				return base.VisitAssignExpression (node);

			var variable = (VariableReference) result ["Variable"];

			switch ((BinaryOperator) result ["Operator"]) {
			case BinaryOperator.Add:
				return new UnaryExpression (
					UnaryOperator.PostIncrement,
					new VariableReferenceExpression (variable));
			case BinaryOperator.Subtract:
				return new UnaryExpression (
					UnaryOperator.PostDecrement,
					new VariableReferenceExpression (variable));
			default:
				return base.VisitAssignExpression (node);
			}
		}

		public BlockStatement Process (DecompilationContext context, BlockStatement body)
		{
			return (BlockStatement) VisitBlockStatement (body);
		}
	}
}
