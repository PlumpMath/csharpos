﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.Framework
{
    public interface IMethodCompilerStage
    {
        string Name { get; }
        IMethodCompilerContext Run(IMethodCompilerContext context);
    }
}
