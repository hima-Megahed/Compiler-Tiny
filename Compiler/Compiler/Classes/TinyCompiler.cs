using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Classes
{
    class TinyCompiler
    {
        private Scanner _myScanner;
        public List<Token> list_Tokens;
        public List<string> errors;
        public TinyCompiler()
        {
            _myScanner = new Scanner();
        }

        public void Compile(string sourceCode)
        {
            _myScanner.Scan(sourceCode);
            list_Tokens = _myScanner.list_Tokens;
            errors = _myScanner.errors;
        }
    }
}
