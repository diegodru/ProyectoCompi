using Core.Interfaces;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class CompilerEngine
    {
        private readonly IParser parser;

        public CompilerEngine(IParser parser)
        {
            this.parser = parser;
        }

        public void Run()
        {
            var intermediateCode = this.parser.Parse();
            StreamWriter sw = File.CreateText("output.js");
            sw.Write(intermediateCode);
        }
    }
}
