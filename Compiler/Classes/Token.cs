using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Classes
{
    public enum TokenType
    {
        // Identifier - Reserved words
        Int, Float, String, read, write, repeat, until,
        If, elseif, Else, then, Return, endl, identifier,
        number,end,
        WHILE,
        DO,
        //main function
        main,
        //Arithmatic Operators
        plus, // +
        minus, // -
        division, // /
        multiply, // *
        
        // Assignment Operator
        assignment, // :=
        
        //Conditions Operators
        lessThan, // <
        greaterThan, // >
        isEqual, // =
        notEqual, // <>

        //Boolean Operators
        And, // &&
        Or, // ||

        //seperators
        comma, // ,
        semicolon, // ;

        //function cll parantheses
        Right_parantheses,
        Left_parantheses,
        Right_CurlyPrakets,
        Left_CurlyPrakets,

        //string data type
        StringDatatype
    }
    class Token
    {
        public string lexeme;
        public TokenType tokenType;
    }
}
