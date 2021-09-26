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

        public async void Run()
        {
            var intermediateCode = this.parser.Parse();
            await File.WriteAllTextAsync("codigo.js", intermediateCode);
        }
    }
}
