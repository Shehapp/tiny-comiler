using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    Begin, Call, Declare, End, Do, Else,Repeat, EndIf,ELSEIF, EndUntil, EndWhile, If, T_Int, T_String, T_Float,
    Parameters, Procedure, Program, Read, Real, Set, Then, Until, While, Write,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, RCurly, LCurly, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    Idenifier, Constant , Int, Float, String,AssignOp, AndOp, OrOp , T_return ,T_endl,Main
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
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("begin", Token_Class.Begin);
            ReservedWords.Add("call", Token_Class.Call);
            ReservedWords.Add("declare", Token_Class.Declare);
            ReservedWords.Add("end", Token_Class.End);
            ReservedWords.Add("do", Token_Class.Do);
            ReservedWords.Add("endl", Token_Class.T_endl);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("elseif", Token_Class.ELSEIF);
            ReservedWords.Add("endif", Token_Class.EndIf);
            ReservedWords.Add("enduntil", Token_Class.EndUntil);
            ReservedWords.Add("endwhile", Token_Class.EndWhile);
            ReservedWords.Add("int", Token_Class.T_Int);
            ReservedWords.Add("string", Token_Class.T_String);
            ReservedWords.Add("float", Token_Class.T_Float);
            ReservedWords.Add("parameters", Token_Class.Parameters);
            ReservedWords.Add("procedure", Token_Class.Procedure);
            ReservedWords.Add("program", Token_Class.Program);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("real", Token_Class.Real);
            ReservedWords.Add("set", Token_Class.Set);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("while", Token_Class.While);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("return", Token_Class.T_return);
            ReservedWords.Add("main", Token_Class.Main);

            Operators.Add(".", Token_Class.Dot);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add(":=", Token_Class.AssignOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("!", Token_Class.NotEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOp);
            Operators.Add("{", Token_Class.LCurly);
            Operators.Add("}", Token_Class.RCurly);


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
                else if(CurrentChar == '{' || CurrentChar =='}')
                {
                    continue;
                    
                }
                else if((i<SourceCode.Length-1 && CurrentChar ==':' && SourceCode[i+1]=='='))
                {
                    FindTokenClass(":=");
                    i++;
                }
                else if((i<SourceCode.Length-1 && CurrentChar =='|' && SourceCode[i+1]=='|'))
                {
                    FindTokenClass("||");
                    i++;
                }
                else if((i<SourceCode.Length-1 && CurrentChar =='&' && SourceCode[i+1]=='&'))
                {
                    FindTokenClass("&&");
                    i++;
                }
                else
                {
                    FindTokenClass(CurrentLexeme);
                }
            }
            
            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            // delete spaces from begining and end of the string
            Lex = Lex.Trim();
            if(Lex.Length==0)
            {
                return;
            }
           // Lex = Lex.ToUpper();
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
