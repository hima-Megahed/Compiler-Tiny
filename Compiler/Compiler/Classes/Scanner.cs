using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Classes
{
    class Scanner
    {
        private Dictionary<string, TokenType> Preserved_Words;
        private Dictionary<string, TokenType> Operators;
        public List<Token> list_Tokens;
        public List<string> errors;
        public Scanner()
        {
            //defining possible preserved words and operators
            Preserved_Words = new Dictionary<string, TokenType>()
            {
                {"int", TokenType.Int },
                {"float", TokenType.Float },
                {"string",TokenType.String },
                {"read", TokenType.read },
                {"write", TokenType.write },
                {"repeat", TokenType.repeat },
                {"until", TokenType.until },
                {"if", TokenType.If },
                {"elseif", TokenType.elseif },
                {"else", TokenType.Else },
                {"then", TokenType.then },
                {"return", TokenType.Return },
                {"endl", TokenType.endl },
                {"identifier", TokenType.identifier }
            };
            Operators = new Dictionary<string, TokenType>()
            {
                {"+", TokenType.plus },
                {"-", TokenType.minus },
                {"/", TokenType.division },
                {"*", TokenType.multiply },
                {":=", TokenType.assignment },
                {"<", TokenType.lessThan },
                {">", TokenType.greaterThan },
                {"=", TokenType.isEqual },
                {"<>", TokenType.notEqual },
                {"&&", TokenType.And },
                {"||", TokenType.Or },
                {",", TokenType.comma },
                {";", TokenType.semicolon }
            };
            list_Tokens = new List<Token>();
            errors = new List<string>();
        }

        /// <summary>
        /// this function scans the source code and get list of Tokens
        /// </summary>
        /// <param name="SourceCode">string containing the source code</param>
        public void Scan(string SourceCode)
        {
            string temp = "";
            for (int i = 0; i < SourceCode.Length; i++)
            {
                // If current char is letter continue : Identifier - preserved word
                if(char.IsLetter(SourceCode[i]))
                {
                    temp += SourceCode[i];
                    i++;
                    while (i < SourceCode.Length && (char.IsLetter(SourceCode[i]) || char.IsDigit(SourceCode[i])) )
                    {
                        temp += SourceCode[i];
                        i++;
                    }
                    // current char is "{" - "}"  or white space or any operator or undefined char
                    //    temp is preserved word
                    if (Preserved_Words.ContainsKey(temp))
                    {
                        list_Tokens.Add(new Token { lexeme = temp, tokenType = Preserved_Words[temp] });
                        temp = "";
                        i--;
                    }
                    //    temp is identifier
                    else if(SourceCode[i] == ';' || SourceCode[i] == ' ' || SourceCode[i] == ':') // To know if this a valid identifier e.g. var< is NOT allowed
                    {
                        list_Tokens.Add(new Token { lexeme = temp, tokenType = Preserved_Words["identifier"] });
                        temp = "";
                        i--;
                    }
                    //    current char is operator or error
                    else
                    {
                        i--; // to get out on the same charachter 
                    }
                }
                // current char is number
                else if(char.IsDigit(SourceCode[i]))
                {
                    temp += SourceCode[i];
                    i++;
                    while(i < SourceCode.Length && (SourceCode[i] == '.' || char.IsDigit(SourceCode[i])))
                    {
                        temp += SourceCode[i];
                        i++;
                    }
                    // save the number in tokens
                    if(char.IsWhiteSpace(SourceCode[i]) || SourceCode[i] == ',' || SourceCode[i] == ';')
                    {
                        list_Tokens.Add(new Token { lexeme = temp, tokenType = TokenType.number });
                        temp = "";
                        i--; // to get out to the same charchter
                    }
                    //error 
                    else
                    {
                        while (i < SourceCode.Length && (!char.IsWhiteSpace(SourceCode[i]) && SourceCode[i] != ';' && SourceCode[i] != ','))
                        {
                            temp += SourceCode[i];
                            i++;
                        }
                        errors.Add(temp);
                        temp = "";
                        i--;
                    }
                    // another char or end of string
                     
                }
                // special operators contains double similar letters or comment
                else if(i+1 < SourceCode.Length && ((SourceCode[i] == ':' && SourceCode[i+1] == '=') || (SourceCode[i] == '<' && SourceCode[i+1] == '>') || (SourceCode[i] == '&' && SourceCode[i+1] == '&') || (SourceCode[i] == '|' && SourceCode[i+1] == '|') || (SourceCode[i] == '/' && SourceCode[i + 1] == '*')))
                {
                    switch (SourceCode[++i])
                    {
                        case '=':
                            list_Tokens.Add(new Token { lexeme = ":=", tokenType = TokenType.assignment });
                            break;
                        case '>':
                            list_Tokens.Add(new Token { lexeme = "<>", tokenType = TokenType.notEqual });
                            break;
                        case '&':
                            list_Tokens.Add(new Token { lexeme = "&&", tokenType = TokenType.And });
                            break;
                        case '|':
                            list_Tokens.Add(new Token { lexeme = "||", tokenType = TokenType.Or });
                            break;
                        // comment case
                        case '*':
                            i++;
                            while(i+1 < SourceCode.Length && (SourceCode[i] != '*' && SourceCode[i+1] != '/'))
                            {
                                temp += SourceCode[i];
                                i++;
                            }
                            // valid comment
                            if(i+1 < SourceCode.Length && (SourceCode[i] == '*' && SourceCode[i+1] == '/')) { i++; }
                            else
                            {
                                errors.Add(temp);
                            }
                            break;
                    }
                }
                // current char is operator 
                else if(Operators.ContainsKey(SourceCode[i].ToString()) && (SourceCode[i] != ':' /*&& SourceCode[i] != '<' && SourceCode[i] != '>'*/ && SourceCode[i] != '&' && SourceCode[i] != '|'))
                {
                    list_Tokens.Add(new Token { lexeme = SourceCode[i].ToString(), tokenType = Operators[SourceCode[i].ToString()] });
                }
                // Current char is " and this is string
                else if(SourceCode[i] == '"')
                {
                    i++;
                    while (i < SourceCode.Length && SourceCode[i] != '"')
                    {
                        temp += SourceCode[i];
                        i++;
                    }

                    // check if valid string
                    if(i < SourceCode.Length && SourceCode[i] == '"')
                    {
                        list_Tokens.Add(new Token { lexeme = temp, tokenType = TokenType.String });
                        temp = "";
                        i++;
                    }
                    // error Not Valid String
                    else
                    {
                        errors.Add(temp);
                        temp = "";
                    }
                }
                // Error now
                else if(SourceCode[i] != ' ')
                {
                    while (i < SourceCode.Length && (!char.IsWhiteSpace(SourceCode[i]) && SourceCode[i] != ';' && SourceCode[i] != ','))
                    {
                        temp += SourceCode[i];
                        i++;
                    }
                    
                }
            }
        }
    }
}
