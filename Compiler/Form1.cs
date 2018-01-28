using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiler
{
    public partial class Form1 : Form
    {
        private readonly Classes.TinyCompiler _myCompiler;
        public Form1()
        {
            InitializeComponent();
            _myCompiler = new Classes.TinyCompiler();
           
        }

        private void Result()
        {
            for(int i=0; i < _myCompiler.ListTokens.Count; i++)
            {
                dgvTokens.Rows.Add(i+1, _myCompiler.ListTokens[i].lexeme, _myCompiler.ListTokens[i].tokenType.ToString());
            }

            for (int i = 0; i < _myCompiler.Errors.Count; i++)
            {
                dgverrors.Rows.Add(i+1, _myCompiler.Errors[i]);
            }


            /////////////////////////////////////////////////////////////////
            foreach (var functionValue in _myCompiler.SemanticAnalyzer.FunctionTable)
            {
                string parameters="";
                foreach (var paremeter in functionValue.Value.Parameters)
                {
                    parameters += paremeter + " - ";
                }
                FuncTble_DGV.Rows.Add(functionValue.Key, functionValue.Value.ReturnType, parameters, functionValue.Value.Parameters.Count);
            }

            foreach (var Symbol in _myCompiler.SemanticAnalyzer.SymbolTable)
            {
                SymbolTble_DGV.Rows.Add(Symbol.Key, Symbol.Value.DataType, Symbol.Value.Value, Symbol.Value.Scope);
            }
        }

        private void btnCompiler_Click_1(object sender, EventArgs e)
        {
            string sourceCode = txtbxSource.Text.ToLower();
            sourceCode = sourceCode.Replace("\t", " ");
            sourceCode = sourceCode.Replace("\n", " ");
            sourceCode = sourceCode.Replace("\r", " ");
            _myCompiler.Compile(sourceCode);
            Result();
            if (_myCompiler.ParseTree != null)
                ParseTreeViewer.Nodes.Add(_myCompiler.ParseTree);
            ParseTreeViewer.ExpandAll();

            if (_myCompiler.NewParseTree != null)
            {
                for (int i = 0; i < _myCompiler.NewParseTree.Nodes.Count; i++)
                {
                    TreeNode nd = (TreeNode) _myCompiler.NewParseTree.Nodes[i].Clone();
                    treeView1.Nodes.Add(nd);
                }
            }
                
            treeView1.ExpandAll();

            
        }
    }
}
