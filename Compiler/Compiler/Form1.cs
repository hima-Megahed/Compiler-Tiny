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
        private Classes.TinyCompiler _myCompiler;
        public Form1()
        {
            InitializeComponent();
            _myCompiler = new Classes.TinyCompiler();
        }

        private void btnCompiler_Click(object sender, EventArgs e)
        {
            string sourceCode = txtbxSource.Text.ToLower();
            sourceCode = sourceCode.Replace("\t", " ");
            sourceCode = sourceCode.Replace("\n", " ");
            sourceCode = sourceCode.Replace("\r", " ");
            _myCompiler.Compile(sourceCode);
            Result();
        }

        private void Result()
        {
            for(int i=0; i < _myCompiler.list_Tokens.Count; i++)
            {
                dgvTokens.Rows.Add(_myCompiler.list_Tokens[i].lexeme, _myCompiler.list_Tokens[i].tokenType.ToString());
            }

            for (int i = 0; i < _myCompiler.errors.Count; i++)
            {
                dgverrors.Rows.Add(_myCompiler.errors[i], "Undefined");
            }
        }
    }
}
