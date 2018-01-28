using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiler.Classes
{

    class Node {
        public List<Node> childs = new List<Node>();
        public Token Token;
        public object Value;
        public string Scope;
        public DataType DataType;
        public Node()
        {
            Token = new Token();
        }
        public Node(String lex)
        {
            Token = new Token();
            Token.lexeme = lex;
        }
    }

    class parser
    {
        public List<string> parseError;
        public List<Token> Tokens;
        int crt = 0;

        public parser()
        {
            parseError = new List<string>();
            Tokens = new List<Token>();
            
        }
        public TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
         TreeNode PrintTree(Node root)
        {
            if (root?.Token == null)
                return null;
            TreeNode tree = new TreeNode(root.Token.lexeme);
            if (root.childs.Count == 0)
                return tree;
            foreach (Node child in root.childs)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
        public TreeNode Parse()
        {
            Node Root = new Node();
            Root.Token.lexeme = "ParseTree";
            try
            {
                Root.childs.Add(Program());
            }
            catch (Exception e)
            {
                return null;
            }
            if (parseError.Count != 0)
                return null;
            return PrintParseTree(Root);
        }

        public void Set_token_list(List<Token> T)
        {
            this.Tokens = T;
        }

        public void Set_error_list(List<string> E)
        {
            this.parseError = E;
        }

        //matchs the curent token with a terminal
        private bool match(TokenType token)
        {
            if (Tokens[crt].tokenType != token)
            {

                string error = "Expected Token " + token + " But " + Tokens[crt].tokenType + " Found!! " + crt;
                parseError.Add(error);
              
            } 
            crt++;
            return true;
        }

        private Node Program()
        {
            Node program = new Node();
            program.Token.lexeme = "Program";
            Node c1 = FunctionDecls();
            if(c1 != null)
            program.childs.Add(c1);
            program.childs.Add(MainFunction());
            return program;
        }

        private Node FunctionDecls()
        {
            Node functionDecls = new Node();
            functionDecls.Token.lexeme = "FunctionDecls";
            DataType();
            if (Tokens[crt].tokenType != TokenType.main)
            {
                crt--;
                functionDecls.childs.Add(FunctionDecl());
                Node c1 = _FunctionDecls();
                if (c1 != null)
                    functionDecls.childs.Add(c1);
                return functionDecls;
            }
            else 
                crt--;
            
            return null;
        }

        private Node _FunctionDecls()
        {
            DataType();
            if (Tokens[crt].tokenType != TokenType.main)
            {
                crt--;
                Node _functionDecls = new Node();
                _functionDecls.Token.lexeme = "_FunctionDecls";
                _functionDecls.childs.Add(FunctionDecl());
                Node c1 = _FunctionDecls();
                if (c1 != null)
                    _functionDecls.childs.Add(c1);
                return _functionDecls;
            }
            else crt--;
            return null;
        }

        private Node FunctionDecl()
        {
            Node functionDecl = new Node();
            functionDecl.Token.lexeme = "FunctionDecl";
            functionDecl.childs.Add(FunctionHeader());
            functionDecl.childs.Add(FunctionBody());
            return functionDecl;
        }

        private Node FunctionHeader()
        {
            Node functionHeader = new Node("FunctionHeader");
            
            functionHeader.childs.Add(DataType());

            functionHeader.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.identifier);

            functionHeader.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.Left_parantheses);

            Node c1 = ParamList();
            if(c1 != null)
            functionHeader.childs.Add(c1); 

            functionHeader.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.Right_parantheses);

            return functionHeader;
        }

        private Node DataType()
        {
            Node dataType = new Node();
            dataType.Token.lexeme = "DataType";
            if (Tokens[crt].tokenType == TokenType.Int)
            {
                dataType.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.Int);
            }
            else if (Tokens[crt].tokenType ==TokenType.Float)
            {
                dataType.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.Float);
            }
            else if (Tokens[crt].tokenType == TokenType.String)
            {
                dataType.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.String);
            }
            else
            {
                string s = "Missing data type";
                parseError.Add(s);
            }
            return dataType;
        }

        private Node ParamList()
        {
            Node paramList = new Node("ParamList");

            if (Tokens[crt].tokenType != TokenType.Right_parantheses)
            {
                paramList.childs.Add(ParamDecl());
                Node c1 = _ParamList();
                if(c1 != null)
                paramList.childs.Add(c1);
                return paramList;
            }
            return null;
        }

        private Node _ParamList()
        {
            if (Tokens[crt].tokenType == TokenType.comma)
            {
                Node _paramList = new Node("_ParamList");
                _paramList.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.comma);

                _paramList.childs.Add(ParamDecl());
               Node c1 =  _ParamList();
               if (c1 != null)
                   _paramList.childs.Add(c1);
               return _paramList;
            }
            return null;
        }

        private Node ParamDecl()
        {
            Node paramDecl = new Node("ParamDecl");

            paramDecl.childs.Add(DataType());

            paramDecl.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.identifier);

            return paramDecl;
        }

        private Node FunctionBody()
        {
            Node functionBody = new Node("FunctionBody");

            functionBody.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.Left_CurlyPrakets);
            
            Node c1 = Statements();
            if (c1 != null)
                functionBody.childs.Add(c1);

            functionBody.childs.Add(ReturnStatement());

            functionBody.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.Right_CurlyPrakets);

            return functionBody;
        }

        private Node Statements()
        {
            if (Tokens[crt].tokenType != TokenType.Return &&  Tokens[crt].tokenType != TokenType.Right_CurlyPrakets && Tokens[crt].tokenType != TokenType.until && Tokens[crt].tokenType != TokenType.end && Tokens[crt].tokenType != TokenType.elseif && Tokens[crt].tokenType != TokenType.Else)
            {
                Node statements = new Node("Statements");
                int crtbef = crt;
                statements.childs.Add(Statement());


                if (Tokens[crtbef].tokenType == TokenType.DO && Tokens[crtbef].tokenType != TokenType.repeat && Tokens[crtbef].tokenType != TokenType.If)
                {
                    statements.childs.Add(new Node(Tokens[crt].lexeme));
                    match(TokenType.semicolon);
                }
                if ((Tokens[crtbef].tokenType == TokenType.DO || Tokens[crtbef].tokenType == TokenType.repeat || Tokens[crtbef].tokenType == TokenType.If) && Tokens[crt].tokenType == TokenType.semicolon)
                {
                    statements.childs.Add(new Node(Tokens[crt].lexeme));
                    match(TokenType.semicolon);
                    parseError.Add("Not Expected semicolon");
                }

                Node c1 = _Statements();
                if (c1 != null)
                    statements.childs.Add(c1);
                return statements;
            }
            return null;
        }

        private Node _Statements()
        {
            if (Tokens[crt].tokenType != TokenType.Return && Tokens[crt].tokenType != TokenType.Right_CurlyPrakets && Tokens[crt].tokenType != TokenType.until && Tokens[crt].tokenType != TokenType.end && Tokens[crt].tokenType != TokenType.elseif && Tokens[crt].tokenType != TokenType.Else)
            {
                Node _statements = new Node("_Statements");
                int crtbef = crt;
                _statements.childs.Add(Statement());
                
                if (Tokens[crtbef].tokenType != TokenType.repeat && Tokens[crtbef].tokenType != TokenType.If)
                {
                    _statements.childs.Add(new Node(Tokens[crt].lexeme));
                    match(TokenType.semicolon);
                }
                if ((Tokens[crtbef].tokenType == TokenType.repeat || Tokens[crtbef].tokenType == TokenType.If) && Tokens[crt].tokenType == TokenType.semicolon)
                {
                    _statements.childs.Add(new Node(Tokens[crt].lexeme));
                    match(TokenType.semicolon);
                    parseError.Add("Not Expected semicolon");
                }
               Node c1 = _Statements();
               if (c1 != null)
                   _statements.childs.Add(c1);
               return _statements;
            }
            return null;
        }

        private Node Statement()
        {
            Node statement = new Node("Statement");
            if (Tokens[crt].tokenType == TokenType.Int || Tokens[crt].tokenType == TokenType.Float || Tokens[crt].tokenType == TokenType.String)
            {
                statement.childs.Add(DeclarationStatement());
            }
            else if (Tokens[crt].tokenType == TokenType.identifier && Tokens[crt + 1].tokenType == TokenType.Left_parantheses)
            {
                statement.childs.Add(FunctionCall());
            }
            else if (Tokens[crt].tokenType == TokenType.identifier)
            {
                statement.childs.Add(AssignmentStatement());
            }
            else if(Tokens[crt].tokenType == TokenType.DO)
            {
                statement.childs.Add(DoWhileLoop());
            }
            else if (Tokens[crt].tokenType == TokenType.read)
            {
                statement.childs.Add(ReadStatement());
            }
            else if (Tokens[crt].tokenType == TokenType.write)
            {
                statement.childs.Add(WriteStatement());
            }
            else if (Tokens[crt].tokenType == TokenType.repeat)
            {
                statement.childs.Add(RepeatStatement());
            }
            else if (Tokens[crt].tokenType == TokenType.If)
            {
                statement.childs.Add(IfStatement());
            }
            else return null;

            return statement;
        }
        private Node DoWhileLoop()
        {
            Node doWhileLoop = new Node("DoWhileLoop");
            doWhileLoop.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.DO);

            doWhileLoop.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.Left_CurlyPrakets);

            Node c1 = Statements();
            if (c1 != null)
                doWhileLoop.childs.Add(c1);

            doWhileLoop.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.Right_CurlyPrakets);

            doWhileLoop.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.WHILE);

            doWhileLoop.childs.Add(ConditionStatement());
            return doWhileLoop;
        }
        private Node Expression()
        {
            Node expression = new Node("Expression");
            expression.childs.Add(Term());
            Node c1 = _Expression();
            if (c1 != null)
                expression.childs.Add(c1);
            return expression;

        }
        private Node _Expression()
        {

            if (Tokens[crt].tokenType == TokenType.plus || Tokens[crt].tokenType == TokenType.minus)
            {
                Node _expression = new Node("_Expression");
                _expression.childs.Add(AddOp());
                _expression.childs.Add(Term());
                return _expression;
            }
            return null;
        }

        private Node ReturnStatement()
        {
            Node returnStatement = new Node("ReturnStatement");

            returnStatement.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.Return);

            returnStatement.childs.Add(Expression());

            returnStatement.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.semicolon);

            return returnStatement;
        }

        private Node MainFunction()
        {
            Node mainFunction = new Node("MainFunction");
            mainFunction.childs.Add(MainFunctionHeader());
            mainFunction.childs.Add(FunctionBody());
            return mainFunction;
        }


        private Node MainFunctionHeader()
        {
            Node mainFunctionHeader = new Node("MainFunctionHeader");

            mainFunctionHeader.childs.Add(DataType());

            mainFunctionHeader.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.main);

            mainFunctionHeader.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.Left_parantheses);

            mainFunctionHeader.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.Right_parantheses);
            return mainFunctionHeader;
        }

        private Node DeclarationStatement()
        {
            Node declarationStatement = new Node("DeclarationStatement");

            declarationStatement.childs.Add(DataType());
            declarationStatement.childs.Add(IdList());

          
            return declarationStatement;
        }

        private Node IdList()
        {
            Node idList = new Node("IdList");
            idList.childs.Add(Id());
            Node c1 = _IdList();
            if(c1 != null)
                idList.childs.Add(c1);
            return idList;
        }

        private Node Id()
        {
            Node id = new Node("Id");
            match(TokenType.identifier);
            if(Tokens[crt].tokenType == TokenType.assignment)
            {
                crt--;
                id.childs.Add(AssignmentStatement());
            }
            else
            {
                id.childs.Add(new Node(Tokens[crt - 1].lexeme));
            }
            return id;

        }

        private Node _IdList()
        {
            if (Tokens[crt].tokenType == TokenType.comma)
            {
                Node _idList = new Node("_IdList");
                _idList.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.comma);

                _idList.childs.Add(Id());
                Node c1 = _IdList();
                if (c1 != null)
                    _idList.childs.Add(c1);
                return _idList;
                    
            }
            return null;

        }

        private Node AssignmentStatement()
        {
            Node assignmentStatement = new Node("AssignmentStatement");

            assignmentStatement.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.identifier);

            assignmentStatement.childs.Add(AssignmentOperator());
            assignmentStatement.childs.Add(Expression());

            return assignmentStatement;
        }

        private Node AssignmentOperator()
        {
            Node assignmentOperator = new Node("AssignmentOperator");
            assignmentOperator.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.assignment);
            return assignmentOperator;
        }

        private Node ReadStatement()
        {
            Node readStatement = new Node("ReadStatement");
            readStatement.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.read);

            readStatement.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.identifier);
            return readStatement;
        }

        private Node WriteStatement()
        {
            Node writeStatement = new Node("WriteStatement");
            writeStatement.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.write);
            writeStatement.childs.Add(Exp());
            return writeStatement;
        }

        private Node Exp()
        {
            Node exp = new Node("Exp");

            if (Tokens[crt].tokenType == TokenType.endl)
            {
                exp.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.endl);
            }
            else
            {
                exp.childs.Add(Expression());
            }
            return exp;
        }

        private Node RepeatStatement()
        {
            Node repeatStatement = new Node("RepeatStatement");
            repeatStatement.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.repeat);

            Node c1 = Statements();
            if(c1 != null)
                repeatStatement.childs.Add(c1);

            repeatStatement.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.until);


            repeatStatement.childs.Add(ConditionStatement());
            return repeatStatement;
        }

        private Node ConditionStatement()
        {
            Node conditionStatement = new Node("ConditionStatement");
            int brefCRT = crt;
            Node c1 = Condition();
            if(Tokens[crt].tokenType == TokenType.Or)
            {
                conditionStatement.childs.Add(c1);
                conditionStatement.childs.Add(OrOp());
                conditionStatement.childs.Add(ConditionTerm());
            }
            else
            {
                crt = brefCRT;
                conditionStatement.childs.Add(ConditionTerm());
            }
            return conditionStatement;
        }


        private Node ConditionTerm()
        {
            Node conditionTerm = new Node("ConditionTerm");
            conditionTerm.childs.Add(Condition());
            Node c1 = _ConditionTerm();
            if (c1 != null)
                conditionTerm.childs.Add(c1);
            return conditionTerm;
        }
        private Node _ConditionTerm()
        {
            
            if(Tokens[crt].tokenType == TokenType.And)
            {
                Node _conditionTerm = new Node("_ConditionTerm");
                _conditionTerm.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.And);
                _conditionTerm.childs.Add(Condition());
                Node c1 = _ConditionTerm();
                if (c1 != null)
                    _conditionTerm.childs.Add(c1);
                return _conditionTerm;
            }
            return null;
        }

        

        private Node Condition()
        {
            Node condition = new Node("Condition");

            condition.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.identifier);


            condition.childs.Add(ConditionOperator());
            condition.childs.Add(Term());

            return condition;
        }

        private Node OrOp()
        {
            Node booleanOperator = new Node("OrOp");
            
            if (Tokens[crt].tokenType == TokenType.Or)
            {
                booleanOperator.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.Or);
            }
            else
            {
                string s = "Missing bool operator";
                parseError.Add(s);
            }
            return booleanOperator;
        }


        private Node AndOp()
        {
            Node booleanOperator = new Node("AndOp");
            if (Tokens[crt].tokenType == TokenType.And)
            {
                booleanOperator.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.And);
            }
            else
            {
                string s = "Missing And Operator";
                parseError.Add(s);
            }
            return booleanOperator;
        }
        private Node ConditionOperator()
        {
            Node conditionOperator = new Node("ConditionOperator");
            if (Tokens[crt].tokenType == TokenType.greaterThan)
            {
                conditionOperator.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.greaterThan);
            }
            else if (Tokens[crt].tokenType == TokenType.lessThan)
            {

                conditionOperator.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.lessThan);
            }
            else if (Tokens[crt].tokenType == TokenType.isEqual)
            {
                conditionOperator.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.isEqual);
            }
            else if (Tokens[crt].tokenType == TokenType.notEqual)
            {
                conditionOperator.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.notEqual);
            }
            else
            {
                string s = "Missing conditional operator";
                parseError.Add(s);
            }
            return conditionOperator;
        }

        private Node Term()
        {
            Node term = new Node("Term");
            term.childs.Add(Factor());
            Node c1 = _Term();
            if (c1 != null)
                term.childs.Add(c1);
            return term;
        }

        private Node _Term()
        {
            Node _term = new Node();
            if(Tokens[crt].tokenType == TokenType.division || Tokens[crt].tokenType == TokenType.multiply)
            {
                _term.childs.Add(MulOp());
                _term.childs.Add(Factor());

                Node c1 = _Term();
                if (c1 != null)
                    _term.childs.Add(c1);
                return _term;
            }
            
            return null;
        }

        private Node Factor()
        {
            Node factor = new Node("Factor");
            if(Tokens[crt].tokenType == TokenType.Left_parantheses)
            {
                factor.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.Left_parantheses);

                factor.childs.Add(Expression());

                factor.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.Right_parantheses);

            }else if(Tokens[crt].tokenType == TokenType.number)
            {
                factor.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.number);

            }else if(Tokens[crt].tokenType == TokenType.String)
            {
                factor.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.String);
            }
            else if(Tokens[crt].tokenType == TokenType.StringDatatype)
            {
                factor.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.StringDatatype);
            }
            else if (Tokens[crt].tokenType == TokenType.identifier && Tokens[crt+1].tokenType != TokenType.Left_parantheses)
            {
                factor.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.identifier);
            }else
            {
                factor.childs.Add(FunctionCall());
            }
            return factor;
        }

        private Node FunctionCall()
        {
            Node functionCall = new Node("FunctionCall");

            functionCall.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.identifier);

            functionCall.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.Left_parantheses);

            Node c1 = Arguments();
            if (c1 != null)
                functionCall.childs.Add(c1);

            functionCall.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.Right_parantheses);
            return functionCall;
        }

        private Node Arguments()
        {
            if (Tokens[crt].tokenType != TokenType.Right_parantheses)
            {
                Node arguments = new Node("Arguments");
                arguments.childs.Add(Args());
                
                Node c1 = _Arguments();
                if (c1 != null)
                    arguments.childs.Add(c1);
                return arguments;
            }
            return null;
        }

        private Node Args()
        {
            Node args = new Node("Args");
            if (Tokens[crt].tokenType == TokenType.identifier)
            {
                args.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.identifier);
            }
            else if (Tokens[crt].tokenType == TokenType.String)
            {
                args.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.String);
            }
            else if(Tokens[crt].tokenType == TokenType.number)
            {
                args.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.number);
            }
            else
            {
                args.childs.Add(Equation());
            }
            return args;
        }

        private Node _Arguments()
        {
            if (Tokens[crt].tokenType == TokenType.comma)
            {
                Node _arguments = new Node("_Arguments");

                _arguments.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.comma);


                _arguments.childs.Add(Args());
                
                Node c1 = _Arguments();
                if (c1 != null)
                    _arguments.childs.Add(c1);
                return _arguments;
            }
            return null;
        }

        private Node Equation()
        {
            Node equation = new Node("Equation");
            if (Tokens[crt].tokenType == TokenType.Left_parantheses)
            {
                equation.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.Left_parantheses);

                equation.childs.Add(Equation());

                equation.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.Right_parantheses);
            }
            else
            {
                equation.childs.Add(Term());
                Node c1 = _Equation();
                if (c1 != null)
                    equation.childs.Add(c1);
            }

            return equation;
        }


        private Node _Equation()
        {
            Node _equation = new Node("_Equation");
            if (Tokens[crt].tokenType == TokenType.plus || Tokens[crt].tokenType == TokenType.minus)
            {
                _equation.childs.Add(AddOp());
            }
            else if(Tokens[crt].tokenType == TokenType.multiply || Tokens[crt].tokenType == TokenType.division)
            {
                _equation.childs.Add(MulOp());
            }else return null;
                _equation.childs.Add(Term());
                Node c1 = _Equation();
                if(c1 != null)
                    _equation.childs.Add(c1);
                return _equation;
            
         
        }

        private Node AddOp()
        {
            Node addOp = new Node("AddOp");
            if (Tokens[crt].tokenType == TokenType.plus)
            {
                addOp.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.plus);
            }
            else if (Tokens[crt].tokenType == TokenType.minus)
            {
                addOp.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.minus);
            }
            else
            {
                
                string s = "Missing arithmatical operator";
                parseError.Add(s);
            }
            return addOp;
        }
        private Node MulOp()
        {
            Node mulOp = new Node("MulOp");
            if (Tokens[crt].tokenType == TokenType.multiply)
            {
                mulOp.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.multiply);
            }
            else if (Tokens[crt].tokenType == TokenType.division)
            {
                mulOp.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.division);
            }
            else
            {
                string s = "Missing arithmatical operator";
                parseError.Add(s);
                return null;
            }
            return mulOp;
        }

        private Node IfStatement()
        {
            Node ifStatement = new Node("IfStatement");

            ifStatement.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.If);

            ifStatement.childs.Add(ConditionStatement());

            ifStatement.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.then);

            Node c1 = Statements();
            if(c1 != null)
                ifStatement.childs.Add(c1);

            c1 = ElseIfPart();
            if (c1 != null)
                ifStatement.childs.Add(c1);

            c1 = ElsePart();
            if (c1 != null)
                ifStatement.childs.Add(c1);


            ifStatement.childs.Add(new Node(Tokens[crt].lexeme));
            match(TokenType.end);


            return ifStatement;
        }

        private Node ElseIfPart()
        {
            if (Tokens[crt].tokenType == TokenType.elseif)
            {
                Node elseIfPart = new Node("ElseIfPart");

                elseIfPart.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.elseif);


                elseIfPart.childs.Add(ConditionStatement());

                elseIfPart.childs.Add(new Node(Tokens[crt].lexeme));
                match(TokenType.then);

                Node c1 = Statements();
                if (c1 != null)
                    elseIfPart.childs.Add(c1);
                return elseIfPart;
            }
            return null;
        }

        private Node ElsePart()
        {
            if (Tokens[crt].tokenType == TokenType.Else)
            {
                Node elsePart = new Node("ElsePart");
                match(TokenType.Else);

                Node c1 = Statements();
                if (c1 != null)
                    elsePart.childs.Add(c1);
                return elsePart;
            }
            return null;
        }

    }
}
