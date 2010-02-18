﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;

namespace Compiler.Tests
{
    [TestFixture]
    public class VariableTests : CompilerTest
    {
        [Test]
        public void VariableReferenceTests()
        {
            Assert.AreEqual("True", CompileAndRunMethod(() =>
            {
                var fieldT = true;
                return fieldT;
            }));

            Assert.AreEqual("False", CompileAndRunMethod(() =>
            {
                var fieldF = false;
                return fieldF;
            }));

            Assert.AreEqual("1", CompileAndRunMethod(() =>
            {
                var fieldF = 1;
                return fieldF;
            }));

            Assert.AreEqual("2", CompileAndRunMethod(() =>
            {
                var fieldF = 2;
                return fieldF;
            }));

            Assert.AreEqual("a", CompileAndRunMethod(() =>
            {
                var fieldF = 'a';
                return fieldF;
            }));

            Assert.AreEqual("5", CompileAndRunMethod(() =>
            {
                var fieldF = 5;
                var field2 = fieldF;
                return field2;
            }));

            Assert.AreEqual("a", CompileAndRunMethod(() =>
            {
                var fieldF = 'a';
                var field2 = fieldF;
                return field2;
            }));

            Assert.AreEqual("-2", CompileAndRunMethod(() =>
            {
                var fieldF = -2;
                return fieldF;
            }));
        }
    }
}
