﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Cecil;

namespace Indy.IL2CPU.CustomImplementation.System {
	public static class ArrayImplRefs {
		static ArrayImplRefs() {
			Type xType = typeof(ArrayImpl);
			foreach (FieldInfo xField in typeof(ArrayImplRefs).GetFields()) {
				if (xField.Name.EndsWith("Ref")) {
					MethodDefinition xTempMethod = xType.GetMethod(xField.Name.Substring(0, xField.Name.Length - "Ref".Length));
					if (xTempMethod == null) {
						throw new Exception("Method '" + xField.Name.Substring(0, xField.Name.Length - "Ref".Length) + "' not found on ArrayImpl!");
					}
					xField.SetValue(null, xTempMethod);
				}
			}
		}

		//public static readonly MethodDefinition InitArrayWithReferenceTypesRef;
	}
}
