using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiler.Classes
{
    class TinyCompiler
    {
        private readonly Scanner _myScanner;
        private readonly parser _myParser;
        public SemanticAnalyzer SemanticAnalyzer;
        public List<Token> ListTokens;
        public List<string> Errors;
        public TreeNode ParseTree;
        public TreeNode NewParseTree;
        public TinyCompiler()
        {
            _myScanner = new Scanner();
            _myParser = new parser();
            //NewParseTree = new TreeNode();
        }

        public void Compile(string sourceCode)
        {
            // scanner 
            _myScanner.Scan(sourceCode);
            ListTokens = _myScanner.list_Tokens;
            Errors = _myScanner.errors;
            // parser
            _myParser.Set_token_list(ListTokens);
            if(Errors.Count == 0)
            ParseTree = _myParser.Parse();
            Errors.AddRange(_myParser.parseError);
            // Semantic analyzer
            SemanticAnalyzer = new SemanticAnalyzer(ParseTree);
            SemanticAnalyzer.Analyze_function(SemanticAnalyzer.Tree);
            SemanticAnalyzer.Get_allErrors();
            SemanticAnalyzer.Errors.Reverse();
            Errors.AddRange(SemanticAnalyzer.Errors);
            SemanticAnalyzer.Annotate(SemanticAnalyzer.NewTree);
            //NewParseTree = new TreeNode();
            NewParseTree = SemanticAnalyzer.NewTree;
            
            
        }
    }
}
