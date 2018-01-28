using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiler.Classes
{
    public enum DataType
    {
        Int, Float, String, Undefined
    }
    public struct FunctionValue
    {
        public List<DataType> Parameters;
        public DataType ReturnType;
    }

    public struct SymbolValue
    {
        public DataType DataType;
        public object Value;
        public string Scope;
    }
    class SemanticAnalyzer
    {
        // Represents Funtion Name and its parameters and return value
        public Dictionary<string, FunctionValue> FunctionTable;

        public List<KeyValuePair<string, SymbolValue>> SymbolTable;

        private List<DataType> _tmpParameters;
        private List<DataType> _argumentsDataTypes;
        private List<KeyValuePair<string, string>> _allSymbols;

        public readonly TreeNode Tree;
        public  TreeNode NewTree;

        public List<string> Errors;

        public SemanticAnalyzer()
        {
            FunctionTable = new Dictionary<string, FunctionValue>();
            SymbolTable = new List<KeyValuePair<string, SymbolValue>>();
        }

        public SemanticAnalyzer(TreeNode tree)
        {
            FunctionTable = new Dictionary<string, FunctionValue>();
            SymbolTable = new List<KeyValuePair<string, SymbolValue>>();
            Errors = new List<string>();
            _argumentsDataTypes = new List<DataType>();
            _allSymbols = new List<KeyValuePair<string, string>>();
            this.Tree = tree;
            NewTree = tree;
        }

        // Function Table
        public void Analyze_function(TreeNode tree)
        {
            if (tree is null)
                return;

            foreach (TreeNode treeNode in tree.Nodes)
            {
                if (treeNode.Text == @"FunctionHeader")
                {
                    DataType returntype = GetType(treeNode.Nodes[0].Nodes[0].Text);
                    string functionName = treeNode.Nodes[1].Text;
                    FunctionValue functionValue;
                    _tmpParameters = new List<DataType>();
                    GetParameters(treeNode.Nodes[3], functionName);
                    functionValue.Parameters = _tmpParameters;
                    functionValue.ReturnType = returntype;
                    FunctionTable.Add(functionName, functionValue);

                    // Getting variables in Function Bodies
                    Get_SymbolsInFunctions(functionName, treeNode.NextNode);
                }
                if (treeNode.Text == @"MainFunctionHeader")
                {
                    DataType returntype = GetType(treeNode.Nodes[0].Nodes[0].Text);
                    string functionName = "main";
                    _tmpParameters = new List<DataType>();
                    FunctionValue functionValue = new FunctionValue();
                    functionValue.Parameters = _tmpParameters;
                    functionValue.ReturnType = returntype;
                    FunctionTable.Add(functionName, functionValue);

                    Get_SymbolsInFunctions(functionName, treeNode.NextNode);
                }
                Analyze_function(treeNode);
            }
        }

        private DataType GetType(string data)
        {
            switch (data)
            {
                case "int":
                    return DataType.Int;
                case "float":
                    return DataType.Float;
                case "string":
                    return DataType.String;
                default:
                    return DataType.Undefined;
            }
        }

        private void GetParameters(TreeNode parametersList, string functioName)
        {
            if (parametersList is null)
                return;

            foreach (TreeNode parameter in parametersList.Nodes)
            {
                if (parameter.Text == @"ParamDecl")
                {
                    _tmpParameters.Add(GetType(parameter.Nodes[0].Nodes[0].Text));
                    SymbolValue symbolValue = new SymbolValue
                    {
                        DataType = GetType(parameter.Nodes[0].Nodes[0].Text),
                        Value = null,
                        Scope = functioName
                    };
                    SymbolTable.Add(new KeyValuePair<string, SymbolValue>(parameter.Nodes[1].Text, symbolValue));
                    _allSymbols.Add(new KeyValuePair<string, string>(parameter.Nodes[1].Text, functioName));
                    
                    
                }

                GetParameters(parameter, functioName);
            }

        }

        // Symbol Table
        private void Get_SymbolsInFunctions(string functionName, TreeNode tree)
        {
            if (tree is null)
                return;

            foreach (TreeNode treeNode in tree.Nodes)
            {
                if (treeNode.Text == @"DeclarationStatement")
                {
                    DataType dataType = GetType(treeNode.Nodes[0].Nodes[0].Text);

                    Get_AllVariables(dataType, functionName, treeNode);
                }
                // releasing Temporal values If Statements
                else if (treeNode.Text == @"end")
                {
                    for (int i = SymbolTable.Count - 1; i >= 0; i--)
                    {
                        if (SymbolTable[i].Value.Scope == "IfStatement")
                        {
                            SymbolTable.RemoveAt(i);
                        }
                    }
                }
                // releasing Temporal values If Statements
                else if (treeNode.Text == @"until")
                {
                    for (int i = SymbolTable.Count - 1; i >= 0; i--)
                    {
                        if (SymbolTable[i].Value.Scope == "RepeatStatement")
                        {
                            SymbolTable.RemoveAt(i);
                        }
                    }
                }
                Get_SymbolsInFunctions(functionName, treeNode);
            }
        }

        private void Get_AllVariables(DataType dataType, string functionName, TreeNode treeNode)
        {
            if (treeNode is null)
                return;

            foreach (TreeNode node in treeNode.Nodes)
            {
                if (node.Text == @"Id")
                {
                    string varName;
                    object varValue = null;
                    if (node.Nodes[0].Text == @"AssignmentStatement")
                    {
                        varName = node.Nodes[0].Nodes[0].Text;
                        varValue = node.Nodes[0].Nodes[2].Nodes[0].Nodes[0].Nodes[0].Text;
                    }
                    else
                    {
                        varName = node.Nodes[0].Text;
                    }
                    SymbolValue symbolValue;
                    symbolValue.DataType = dataType;
                    string scope = Get_Scope(treeNode, functionName);
                    symbolValue.Scope = scope;
                    symbolValue.Value = varValue;

                    // 2nd Error Type Redeclaring Variable
                    if (Get_VarDeclaredSymbolTable(varName, scope))
                    {
                        if (!Errors.Contains("Error ReDeclaration of Variable " + varName))
                            Errors.Add("Error ReDeclaration of Variable " + varName);
                    }
                    else
                    {
                        SymbolTable.Add(new KeyValuePair<string, SymbolValue>(varName, symbolValue));
                        _allSymbols.Add(new KeyValuePair<string, string>(varName, scope));
                    }
                        

                }
                Get_AllVariables(dataType, functionName, node);
            }
        }

        private bool Get_VarDeclaredSymbolTable(string varName, string scope)
        {
            foreach (var item in SymbolTable)
            {
                if (item.Key == varName && item.Value.Scope == scope)
                    return true;
            }
            return false;
        }

        private string Get_Scope(TreeNode treeNode, string functionName)
        {
            while (treeNode.Text != @"RepeatStatement" &&
                treeNode.Text != @"IfStatement" &&
                treeNode.Text != @"FunctionBody")
            {
                //MessageBox.Show(treeNode.Parent.Text);
                treeNode = treeNode.Parent;
            }

            if (treeNode.Text == @"RepeatStatement")
                return "RepeatStatement";

            if (treeNode.Text == @"IfStatement")
                return "IfStatement";

            return functionName;
        }

        // Error Detection 
        public void Get_allErrors()
        {
            Get_1stTypeError(Tree);
            Get_3rdTypeError(Tree);
            Get_4thTypeError(Tree);
            Get_5thErrorType(Tree);
            eval_Expressions(Tree);
        }
        private bool Get_VarFromSymbolTable(string varName)
        {
            foreach (var item in SymbolTable)
            {
                if (item.Key == varName)
                    return true;
            }
            return false;
        }
        // 1st Error Type using undeclaerd variable
        private void Get_1stTypeError(TreeNode tree)
        {
            if (tree is null)
                return;

            foreach (TreeNode node in tree.Nodes)
            {
                if (node.Text == @"AssignmentStatement")
                {
                    string varName = node.Nodes[0].Text;
                    if (!Get_VarFromSymbolTable(varName) && !FindSymbol_InAll_Symbols(varName))
                    {
                        if (!Errors.Contains("Error Undeclared Variable " + varName))
                            Errors.Add("Error Undeclared Variable " + varName);
                    }
                }
                else if (node.Text == @"Condition")
                {
                    string varName = node.Nodes[0].Text;
                    if (!Get_VarFromSymbolTable(varName))
                    {
                        if (!Errors.Contains("Error Undeclared Variable " + varName))
                            Errors.Add("Error Undeclared Variable " + varName);
                    }
                }

                Get_1stTypeError(node);
            }
        }

        private bool FindSymbol_InAll_Symbols(string varName)
        {
            foreach (var symbol in _allSymbols)
            {
                if (symbol.Key == varName)
                    return true;
            }
            return false;
        }

        // 3rd Error Type using undeclared function
        private void Get_3rdTypeError(TreeNode tree)
        {
            if (tree is null)
                return;

            foreach (TreeNode node in tree.Nodes)
            {
                if (node.Text == @"FunctionCall")
                {
                    string functionName = node.Nodes[0].Text;
                    if (!FunctionTable.ContainsKey(functionName))
                    {
                        if (!Errors.Contains("Error Using Undeclared Function " + functionName))
                            Errors.Add("Error Using Undeclared Function " + functionName);
                    }
                }
                Get_3rdTypeError(node);
            }
        }
        // 4th Error Type Arguments type missmatch & Arguments Number missmatch
        private void Get_4thTypeError(TreeNode tree)
        {
            if (tree is null)
                return;

            foreach (TreeNode node in tree.Nodes)
            {
                if (node.Text == @"FunctionCall")
                {
                    string functionName = node.Nodes[0].Text;
                    TreeNode arguments = node.Nodes[2];
                    _argumentsDataTypes.Clear();
                    Get_AllArguments(arguments);
                    if (FunctionTable.ContainsKey(functionName) && _argumentsDataTypes.Count != FunctionTable[functionName].Parameters.Count)
                    {
                        if (!Errors.Contains("Error Parameters Number Missmatch in Function " + functionName))
                            Errors.Add("Error Parameters Number Missmatch in Function " + functionName);
                    }
                    else
                    {
                        for (int i = 0; FunctionTable.ContainsKey(functionName) && i < FunctionTable[functionName].Parameters.Count; i++)
                        {
                            if (FunctionTable[functionName].Parameters[i] != _argumentsDataTypes[i])
                            {
                                if (!Errors.Contains("Error Parameter DataType Missmatch in Function " + functionName + " Expected " +
                                    FunctionTable[functionName].Parameters[i]))
                                    Errors.Add("Error Parameter DataType Missmatch in Function " + functionName + " Expected " +
                                        FunctionTable[functionName].Parameters[i] + " but Found " + _argumentsDataTypes[i]);
                            }
                        }
                    }
                }
                Get_4thTypeError(node);
            }
        }

        private void Get_AllArguments(TreeNode treeNode)
        {
            if (treeNode is null)
                return;

            foreach (TreeNode node in treeNode.Nodes)
            {
                if (node.Text == @"Args")
                {

                    if (node.Nodes[0].Text == @"Equation")
                    {
                        TreeNode equationNode = node.Nodes[0];
                        object value = equationNode.Nodes[0].Nodes[0].Nodes[0].Text;

                        int valInt;
                        bool ResultInt = Int32.TryParse(value.ToString(), out valInt);
                        if (ResultInt)
                        {
                            _argumentsDataTypes.Add(DataType.Int);
                        }
                        else
                        {
                            float valFloat;
                            bool ResultFloat = float.TryParse(value.ToString(), out valFloat);
                            if (ResultFloat)
                            {
                                _argumentsDataTypes.Add(DataType.Float);
                            }
                            else
                            {
                                _argumentsDataTypes.Add(DataType.String);
                            }
                        }

                    }
                    else
                    {
                        string varOrVal = node.Nodes[0].Text;
                        if (Get_VarFromSymbolTable(varOrVal))
                        {
                            DataType varDataType = Get_VariableDataType(varOrVal);
                            _argumentsDataTypes.Add(varDataType);
                        }
                        else
                        {
                            object value = varOrVal;

                            int valInt;
                            bool ResultInt = Int32.TryParse(value.ToString(), out valInt);
                            if (ResultInt)
                            {
                                _argumentsDataTypes.Add(DataType.Int);
                            }
                            else
                            {
                                float valFloat;
                                bool ResultFloat = float.TryParse(value.ToString(), out valFloat);
                                if (ResultFloat)
                                {
                                    _argumentsDataTypes.Add(DataType.Float);
                                }
                                else
                                {
                                    _argumentsDataTypes.Add(DataType.String);
                                }
                            }
                        }

                    }
                }
                Get_AllArguments(node);
            }

        }

        private DataType Get_VariableDataType(string variableName)
        {
            foreach (var symbol in SymbolTable)
            {
                if (symbol.Key == variableName)
                {
                    return symbol.Value.DataType;
                }
            }
            return DataType.Undefined;
        }

        // 5th Error Type return type miss match
        private void Get_5thErrorType(TreeNode tree)
        {
            if (tree is null)
                return;

            foreach (TreeNode node in tree.Nodes)
            {
                if (node.Text == @"FunctionDecl")
                {
                    string functionName = node.Nodes[0].Nodes[1].Text;
                    Get_functionReturnType(tree, functionName);
                }
                Get_5thErrorType(node);
            }
        }

        private void Get_functionReturnType(TreeNode tree, string functionName)
        {
            if (tree is null)
                return;
            foreach (TreeNode node in tree.Nodes)
            {
                if (node.Text == @"ReturnStatement" && node.Parent.Parent.Nodes[0].Nodes[1].Text == functionName)
                {
                    string varOrVal = node.Nodes[1].Nodes[0].Nodes[0].Nodes[0].Text;
                    DataType varDataType;
                    if (varOrVal == @"FunctionCall")
                    {
                        varOrVal = node.Nodes[1].Nodes[0].Nodes[0].Nodes[0].Nodes[0].Text;
                    }

                    if (Get_VarFromSymbolTable(varOrVal))
                    {
                        varDataType = Get_VariableDataType(varOrVal);
                    }
                    else if (FunctionTable.ContainsKey(varOrVal))
                    {
                        varDataType = FunctionTable[varOrVal].ReturnType;
                    }
                    else
                    {
                        object value = varOrVal;

                        int valInt;
                        bool ResultInt = Int32.TryParse(value.ToString(), out valInt);
                        if (ResultInt)
                        {
                            varDataType = DataType.Int;
                        }
                        else
                        {
                            float valFloat;
                            bool ResultFloat = float.TryParse(value.ToString(), out valFloat);
                            if (ResultFloat)
                            {
                                varDataType = DataType.Float;
                            }
                            else
                            {
                                varDataType = DataType.String;
                            }
                        }
                    }



                    if (varDataType != FunctionTable[functionName].ReturnType)
                    {
                        if (!Errors.Contains(("Error Return Type Missmatch In function " + functionName + " Expected " + FunctionTable[functionName].ReturnType +
                                            " but found " + varDataType)))
                            Errors.Add("Error Return Type Missmatch In function " + functionName + " Expected " + FunctionTable[functionName].ReturnType +
                            " but found " + varDataType);
                    }
                }
                Get_functionReturnType(node, functionName);
            }
        }

        private void eval_Expressions(TreeNode tree)
        {
            if (tree is null)
                return;

            foreach (TreeNode node in tree.Nodes)
            {
                if (node.Text == @"AssignmentStatement")
                {
                    string varName = node.Nodes[0].Text;
                    TreeNode exp = node.Nodes[2];
                    Get_Expression(exp, varName);

                }
                eval_Expressions(node);
            }
        }

        private void Get_Expression(TreeNode exp, string varName)
        {
            if (exp is null)
                return;

            foreach (TreeNode treeNode in exp.Nodes)
            {
                if (treeNode.Text == @"Factor")
                {
                    string val = treeNode.Nodes[0].Text;
                    DataType dataType;
                    if (val == "FunctionCall")
                        val = treeNode.Nodes[0].Nodes[0].Text;

                    if (Get_VarFromSymbolTable(val)) // Identifier
                    {
                        dataType = Get_VariableDataType(val);
                    }
                    else if (FunctionTable.ContainsKey(val))
                    {
                        dataType = FunctionTable[val].ReturnType;
                    }
                    else
                    {
                        dataType = Get_valType(val);
                    }
                    
                    if (Get_VariableDataType(varName) != dataType)
                    {
                        if (!Errors.Contains("Error Variable DataType miss match in " + varName + " expected " +
                            Get_VariableDataType(varName) + " Found " + dataType) && Get_VariableDataType(varName)!= DataType.Undefined)
                            Errors.Add("Error Variable DataType miss match in " + varName + " expected " + Get_VariableDataType(varName) + " Found " + dataType);
                    }
                }
                Get_Expression(treeNode, varName);
            }
        }

        private DataType Get_valType(string val)
        {
            object value = val;

            int valInt;
            bool ResultInt = Int32.TryParse(value.ToString(), out valInt);
            if (ResultInt)
            {
                return DataType.Int;
            }

            float valFloat;
            bool ResultFloat = float.TryParse(value.ToString(), out valFloat);
            if (ResultFloat)
            {
                return DataType.Float;
            }

            return DataType.String;
        }

        public void Annotate(TreeNode tree)
        {
            if(tree is null)
                return;
            foreach (TreeNode node in tree.Nodes)
            {
                if (node.Text == @"DeclarationStatement")
                {
                    string varName;

                    if (node.Nodes[1].Nodes[0].Nodes[0].Text == @"AssignmentStatement")
                    {
                        varName = node.Nodes[1].Nodes[0].Nodes[0].Nodes[0].Text;
                    }
                    else
                    {
                        varName = node.Nodes[1].Nodes[0].Nodes[0].Text;
                    }

                    if (Get_VarFromSymbolTable(varName))
                    {
                        SymbolValue val = Get_SymbolValue(varName);
                        if (val.Scope != "NN")
                        {
                            node.Nodes.Add("DateType").Nodes.Add(val.DataType.ToString());
                            if(val.Value != null)
                                node.Nodes.Add("Value").Nodes.Add(val.Value.ToString());
                            else
                                node.Nodes.Add("Value").Nodes.Add("null");
                            node.Nodes.Add("Scope").Nodes.Add(val.Scope);
                        }
                    }
                }
                Annotate(node);
            }
        }

        private SymbolValue Get_SymbolValue(string varName)
        {
            foreach (var symbol in SymbolTable)
            {
                if (symbol.Key == varName)
                    return symbol.Value;
            }
            return new SymbolValue(){Scope = "NN"};
        }

        /*
        public TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        private TreeNode PrintTree(Node root)
        {
            if (root?.Token == null)
                return null;
            TreeNode tree = new TreeNode(root.Token.lexeme + " " + root.DataType + " " + root.Scope + " " + root.Value);
            if (root.childs.Count == 0)
                return tree;
            foreach (Node child in root.childs)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }*/
    }
}

/*
 int main (){
int x1,z1,d2:=5;
return 0;
}
     */
//Todo: 