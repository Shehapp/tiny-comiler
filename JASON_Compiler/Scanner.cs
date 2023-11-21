using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    Begin, Call, Declare, End, Do, Else, EndIf, EndUntil, EndWhile, If, T_Int, T_String, T_Float,
    Parameters, Procedure, Program, Read, Real, Set, Then, Until, While, Write,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    Idenifier, Constant , Int, Float, String
}
namespace JASON_Compiler
{
    

    public class Token
    {
       public string lex;
       public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("IF", Token_Class.If);
            ReservedWords.Add("BEGIN", Token_Class.Begin);
            ReservedWords.Add("CALL", Token_Class.Call);
            ReservedWords.Add("DECLARE", Token_Class.Declare);
            ReservedWords.Add("END", Token_Class.End);
            ReservedWords.Add("DO", Token_Class.Do);
            ReservedWords.Add("ELSE", Token_Class.Else);
            ReservedWords.Add("ENDIF", Token_Class.EndIf);
            ReservedWords.Add("ENDUNTIL", Token_Class.EndUntil);
            ReservedWords.Add("ENDWHILE", Token_Class.EndWhile);
            ReservedWords.Add("INT", Token_Class.T_Int);
            ReservedWords.Add("STRING", Token_Class.T_String);
            ReservedWords.Add("FLOAT", Token_Class.T_Float);
            ReservedWords.Add("PARAMETERS", Token_Class.Parameters);
            ReservedWords.Add("PROCEDURE", Token_Class.Procedure);
            ReservedWords.Add("PROGRAM", Token_Class.Program);
            ReservedWords.Add("READ", Token_Class.Read);
            ReservedWords.Add("REAL", Token_Class.Real);
            ReservedWords.Add("SET", Token_Class.Set);
            ReservedWords.Add("THEN", Token_Class.Then);
            ReservedWords.Add("UNTIL", Token_Class.Until);
            ReservedWords.Add("WHILE", Token_Class.While);
            ReservedWords.Add("WRITE", Token_Class.Write);

            Operators.Add(".", Token_Class.Dot);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("!", Token_Class.NotEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);



        }

    public void StartScanning(string SourceCode)
        {
            for(int i=0; i<SourceCode.Length;i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;


                if (i<SourceCode.Length-1 && CurrentChar =='/' && SourceCode[i+1]=='*') //if you read a character "dkfngkj" /**/
                { //  /**/
                    CurrentLexeme+='*';
                    for (j = i + 2; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];

                        CurrentLexeme += CurrentChar.ToString();
                        if ((CurrentChar == '*') && (SourceCode[j + 1] == '/'))
                        {
                            j++;
                            CurrentLexeme += SourceCode[j].ToString();
                            break;
                        }
                    }

                    i=j;
                    Console.WriteLine(CurrentLexeme);
                    FindTokenClass(CurrentLexeme);
                }
                else if (CurrentChar >= 'A' && CurrentChar <= 'z') //if you read a character "dkfngkj"
                {
                   for(j=i+1;j<SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];

                        if ((CurrentChar >= 'A' && CurrentChar <= 'z') || (CurrentChar >= '0' && CurrentChar <= '9'))
                            CurrentLexeme += CurrentChar.ToString();
                        
                        else 
                                break;
                    }

                   i=j-1;
                   FindTokenClass(CurrentLexeme);
                }
                else if(CurrentChar >= '0' && CurrentChar <= '9')
                {
                    for (j = i+1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];

                        if ((CurrentChar >= '0' && CurrentChar <= '9') || (CurrentChar == '.'))
                            CurrentLexeme += CurrentChar.ToString();

                        else
                            break;
                    }
                    i=j-1;
                    FindTokenClass(CurrentLexeme);
                }else if (CurrentChar == '"')
                {
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];

                        CurrentLexeme += CurrentChar.ToString();
                        if ((CurrentChar == '"'))
                            break;
                    }

                    i=j;
                    FindTokenClass(CurrentLexeme);
                }
                else if(CurrentChar == '{')
                {
                    continue;
                }
                else
                {
                    FindTokenClass(CurrentChar.ToString());
                }
            }
            
            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Lex = Lex.ToUpper();
            Token Tok = new Token();
            Tok.lex = Lex;
            Console.WriteLine(Lex);
            
            
           // Console.WriteLine("mm");
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex)){
               // Console.WriteLine("2ss");
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
                return;

            }
            //Is it an identifier?
            if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
                return;
            }

            //Is it a Constant?
            if (isConstant(Lex))
            {
                Tok.token_type = Token_Class.Constant;
                Tokens.Add(Tok);
                return;
            }
            //Is it an operator?
            if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
                return;
            }

            //Is is a String?
            if (isString(Lex))
            {
                Tok.token_type = Token_Class.String;
                Tokens.Add(Tok);
                return;
            }

            if (isComment(Lex))
            {
                return;
            }


            //Is it an undefined?
            Errors.Error_List.Add("Undefined Token: " + Lex);
        }

    

        bool isIdentifier(string lex)
        {
            // Check if the lex is an identifier or not.
            string pattern = @"^[a-zA-Z_][a-zA-Z0-9_]*$";
            return new Regex(pattern).IsMatch(lex);
        }
        bool isConstant(string lex)
        {
            // Check if the lex is an identifier or not. int dg=12;
            string pattern = @"^[0-9]+(\.[0-9]+)?$";
            return new Regex(pattern).IsMatch(lex);
        }

        bool isString(string lex) // "fgf"
        {
            // Check if the lex is an identifier or not. 
            string pattern = @"^"".*""$";
            return new Regex(pattern).IsMatch(lex);
        }

        bool isComment(string lex) // 
        {
            // comment
            string pattern = @"^\/\*.*\*\/$";
            return new Regex(pattern).IsMatch(lex);
        }

    }
}
