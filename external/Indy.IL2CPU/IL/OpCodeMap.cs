﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Indy.IL2CPU.Assembler;
using Indy.IL2CPU.IL;
using Indy.IL2CPU.Plugs;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Indy.IL2CPU.IL
{
    public abstract class OpCodeMap
    {
        protected readonly SortedList<Code, Type> mMap = new SortedList<Code, Type>();
        protected readonly SortedList<Code, Action<Mono.Cecil.Cil.Instruction, MethodInformation, SortedList<string, object>>> _scanMethods = new SortedList<Code, Action<Mono.Cecil.Cil.Instruction, MethodInformation, SortedList<string, object>>>();

        protected OpCodeMap()
        {
            MethodHeaderOp = GetMethodHeaderOp();
            MethodFooterOp = GetMethodFooterOp();
            CustomMethodImplementationProxyOp = GetCustomMethodImplementationProxyOp();
            CustomMethodImplementationOp = GetCustomMethodImplementationOp();
            InitVmtImplementationOp = GetInitVmtImplementationOp();
            MainEntryPointOp = GetMainEntryPointOp();
        }

        protected abstract Assembly ImplementationAssembly
        {
            get;
        }

        protected abstract Type GetMethodHeaderOp();
        protected abstract Type GetMethodFooterOp();
        protected abstract Type GetCustomMethodImplementationProxyOp();
        protected abstract Type GetCustomMethodImplementationOp();
        protected abstract Type GetInitVmtImplementationOp();
        protected abstract Type GetMainEntryPointOp();

        public void ScanILCode(Mono.Cecil.Cil.Instruction instruction, MethodInformation aMethod, SortedList<string, object> aMethodData)
        {
            if (_scanMethods.ContainsKey(instruction.OpCode.Code))
                _scanMethods[instruction.OpCode.Code](instruction, aMethod, aMethodData);
        }

        public virtual void Initialize(Assembler.Assembler aAssembler, IEnumerable<AssemblyDefinition> aApplicationAssemblies)
        {
            foreach (var xItem in (from item in ImplementationAssembly.GetTypes()
                                   let xAttrib = item.GetCustomAttributes(typeof(OpCodeAttribute), true).FirstOrDefault() as OpCodeAttribute
                                   where item.IsSubclassOf(typeof(Op)) && xAttrib != null
                                   select new
                                   {
                                       OpCode = xAttrib.OpCode,
                                       Type = item
                                   }))
            {
                try
                {
                    mMap.Add(xItem.OpCode, xItem.Type);
                    var xMethod = xItem.Type.GetMethod("ScanOp",
                                         new Type[] { typeof(Mono.Cecil.Cil.Instruction), typeof(MethodInformation), typeof(SortedList<string, object>) });
                    if (xMethod != null)
                    {
                        _scanMethods.Add(xItem.OpCode,
                                         (Action<Mono.Cecil.Cil.Instruction, MethodInformation, SortedList<string, object>>)Delegate.CreateDelegate(typeof(Action<Mono.Cecil.Cil.Instruction, MethodInformation, SortedList<string, object>>),
                                                                                                                                  xMethod));
                    }
                }
                catch
                {
                    Console.WriteLine("Was adding op " + xItem.OpCode);
                    throw;
                }
            }
        }

        public Type GetOpForOpCode(Code code)
        {
            if (!mMap.ContainsKey(code))
            {
                throw new NotSupportedException("OpCode '" + code + "' not supported!");
            }
            return mMap[code];
        }

        public readonly Type MethodHeaderOp;
        public readonly Type MethodFooterOp;
        public readonly Type CustomMethodImplementationProxyOp;
        public readonly Type CustomMethodImplementationOp;
        public readonly Type InitVmtImplementationOp;
        public readonly Type MainEntryPointOp;

        public virtual Type GetOpForCustomMethodImplementation(string aName)
        {
            return null;
        }

        public virtual IList<AssemblyDefinition> GetPlugAssemblies()
        {
            var xResult = new List<AssemblyDefinition> {
			                                     TypeResolver.Resolve<OpCodeMap>().Module.Assembly,
			                                     AssemblyFactory.GetAssembly("Indy.IL2CPU")
			                                 };
            return xResult;
        }

        public MethodDefinition GetCustomMethodImplementation(string aOrigMethodName)
        {
            return null;
        }

        public virtual bool HasCustomAssembleImplementation(MethodInformation aMethod)
        {
            PlugMethodAttribute xResult = (from attrib in aMethod.Method.CustomAttributes.Cast<Attribute>()
                                           where attrib.GetType() == typeof(PlugMethodAttribute)
                                           select attrib).Cast<PlugMethodAttribute>().FirstOrDefault();
            if (xResult != null)
            {
                return xResult.Assembler != null;
            }
            return false;
        }

        public virtual void ScanCustomAssembleImplementation(MethodInformation aMethod)
        {
        }

        public virtual void DoCustomAssembleImplementation(Assembler.Assembler aAssembler, MethodInformation aMethodInfo)
        {

            PlugMethodAttribute xAttrib = (from attrib in aMethodInfo.Method.CustomAttributes.Cast<CustomAttribute>()
                                           where attrib.GetType() == typeof(PlugMethodAttribute)
                                           select attrib).Cast<PlugMethodAttribute>().FirstOrDefault();
            if (xAttrib != null)
            {
                Type xAssemblerType = xAttrib.Assembler;
                if (xAssemblerType != null)
                {
                    var xAssembler = (AssemblerMethod)Activator.CreateInstance(xAssemblerType);
                    var xNeedsMethodInfo = xAssembler as INeedsMethodInfo;
                    if (xNeedsMethodInfo != null)
                    {
                        xNeedsMethodInfo.MethodInfo = aMethodInfo;
                    }
                    xAssembler.Assemble(aAssembler);
                }
            }
        }

        public virtual void PostProcess(Assembler.Assembler aAssembler)
        {
        }

        public abstract void EmitOpDebugHeader(Assembler.Assembler aAssembler, uint aOpId, string aOpLabel);

        protected virtual void RegisterAllUtilityMethods(Action<MethodDefinition> aRegister)
        {
        }

        public virtual void PreProcess(Indy.IL2CPU.Assembler.Assembler mAssembler)
        {

        }
    }
}