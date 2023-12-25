using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();
        
        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public  Node root;
        
        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            Console.WriteLine(TokenStream.Count);
            root = new Node("root");
            root.Children.Add(Program());
            return root;
        }


        int there_is_funs = 0;
        private Node Program()
        {
            Node program = new Node("Program");

            if (InputPointer >= TokenStream.Count)
            {
               
                return null;
            }


            // there might be more than one function though the main get inside the FunctionDecl();  
            // we should only get in here if the function_name is identifier

            Node functionDecl = null;

            while (InputPointer < TokenStream.Count && TokenStream[InputPointer+1].token_type == Token_Class.Idenifier)
            {

                 functionDecl = FunctionDecl();
                 program.Children.Add(functionDecl);
            }
//            if (functionDecl != null)
//            {
                Console.WriteLine("IM NOT NULL");
                Node mainFunc = MainFunction();
                if (mainFunc != null)
                {
                    program.Children.Add(mainFunc);
                    if (InputPointer != TokenStream.Count)
                    {
                        Errors.Parser_Error_List.Add("Extra content found after main function");
                        return null;
                    }
                    return program;
                }
                Errors.Parser_Error_List.Add("Main Function missing");
                return null;
//            }

            Errors.Parser_Error_List.Add("Invalid program structure");
            return null;
        }

        private Node MainFunction()
        {
            Node mainFunc = new Node("Main Function");
            Node mainFuncHeader = MainFunctionHeader();
            if (mainFuncHeader != null)
            {
                mainFunc.Children.Add(mainFuncHeader);
                Console.WriteLine("function_body");

               // Node mainbody = FunctionBody();
               /* Console.WriteLine("hna");
                mainbody.Children.ForEach(x => Console.WriteLine(x.Name));
                Console.WriteLine("hna");*/
                mainFunc.Children.Add(FunctionBody());
                return mainFunc;
            }
            return null;
        }

        private Node FunctionDecl()
        {
            Node functionDecl = new Node("Function Declaration");
            Node functionHeader = FunctionHeader();
            if (functionHeader != null)
            {
                Console.WriteLine("IM NOT NULL");
                there_is_funs = 1;
                functionDecl.Children.Add(functionHeader);   
                functionDecl.Children.Add(FunctionBody());
                return functionDecl;
            }
            return null;
        }


        private Node MainFunctionHeader()
        {
            Node mainFuncHead = new Node("Main Function Header");
            mainFuncHead.Children.Add(DatatypeF());
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Main)
            {
                mainFuncHead.Children.Add(match(Token_Class.Main));
                mainFuncHead.Children.Add(L_parenthesis());
                mainFuncHead.Children.Add(R_parenthesis());

                return mainFuncHead;
            }

            Errors.Parser_Error_List.Add("Missing Main Function");
            return null;

        }

       /* private Node FunctionDecl()
        {
            Node Function_Declaration = new Node("Function Declaration");
            Function_Declaration.Children.Add(FunctionHeader());
            Function_Declaration.Children.Add(FunctionBody());

            return Function_Declaration;
        }*/

        private Node FunctionHeader()
        {
            Node Function_Header = new Node("Function Header");

            Function_Header.Children.Add(DatatypeF());
            Console.WriteLine("i failed here 1");

            Function_Header.Children.Add(FunctionName());
            Console.WriteLine("i failed here 2");

            Function_Header.Children.Add(L_parenthesis());
            Console.WriteLine("i failed here 3");

        if(TokenStream[InputPointer].token_type !=Token_Class.RParanthesis)
            Function_Header.Children.Add(Params());
            Console.WriteLine("i failed here 4");

            Function_Header.Children.Add(R_parenthesis());
            Console.WriteLine("i failed here 5");


            return Function_Header;

        }
        private Node DatatypeF()
        {

            Node Datatype = new Node("Return type");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Int)
            {
                Datatype.Children.Add(match(Token_Class.T_Int));
                return Datatype;
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Float)        
            {
                Datatype.Children.Add(match(Token_Class.T_Float));
                return Datatype;
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_String)
            {
                Datatype.Children.Add(match(Token_Class.T_String));
                return Datatype;
            }
            Errors.Parser_Error_List.Add("Missing return type in Function Declaration");
            return null;
        }
        private Node FunctionName()
        {
            Node FuncName = new Node("Function Name");
            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier &&
                TokenStream[InputPointer].token_type != Token_Class.Main)
            {
                FuncName.Children.Add(match(Token_Class.Idenifier));
                return FuncName;
            }
            Errors.Parser_Error_List.Add("Missing function name in Function Declaration");
            return null;
        }


        private Node L_parenthesis()
        {
            Node LParenthesis = new Node("(");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {
                LParenthesis.Children.Add(match(Token_Class.LParanthesis));
                return LParenthesis;
            }
            Errors.Parser_Error_List.Add("Missing left paranthesis '('");
            return null;
        }
        private Node R_parenthesis()
        {
            Node RParenthesis = new Node(")");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.RParanthesis)
            {
                RParenthesis.Children.Add(match(Token_Class.RParanthesis));
                return RParenthesis;
            }
            Errors.Parser_Error_List.Add("Missing right paranthesis ')'");
            return null;
        }

        private Node Params()
        {
            Node Parameters = new Node("Parameters");
            Node temp = Param();
            if (temp != null)
            {
                Parameters.Children.Add(temp);
                Parameters.Children.Add(ParamsDash());

                return Parameters;
            }

            return null;
        }

        private Node Param()
        {
            Node Parameter = new Node("Parameter");
            Node temp = DatatypeP();
            if (temp != null)
            {
                Parameter.Children.Add(temp);

                if (InputPointer != TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                {
                    Parameter.Children.Add(match(Token_Class.Idenifier));
                    return Parameter;
                }

                Errors.Parser_Error_List.Add("Missing variable name in Parameter Declaration");
                return null;
            }

            return null;

        }

        private Node DatatypeP()
        {
            Node Datatype = new Node("Parameters type");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Int)
            {
                Datatype.Children.Add(match(Token_Class.T_Int));
                return Datatype;
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Float)
            {
                Datatype.Children.Add(match(Token_Class.T_Float));
                return Datatype;
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_String)
            {
                Datatype.Children.Add(match(Token_Class.T_String));
                return Datatype;
            }
            Errors.Parser_Error_List.Add("Missing datatype in Parameter Declaration");
            return null;
        }

        private Node Datatype()
        {
            Node Datatype = new Node("Variable type");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Int)
            {
                Datatype.Children.Add(match(Token_Class.T_Int));
                return Datatype;
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_Float)
            {
                Datatype.Children.Add(match(Token_Class.T_Float));
                return Datatype;
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_String)
            {
                Datatype.Children.Add(match(Token_Class.T_String));
                return Datatype;
            }
            Errors.Parser_Error_List.Add("Missing datatype in Variable Declaration");
            return null;
        }

        private Node ParamsDash()
        {
            Node paramsdash = new Node("Parameters'");


            Console.WriteLine(TokenStream[InputPointer].token_type);
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
                paramsdash.Children.Add(Comma());

            if (InputPointer != TokenStream.Count && 
                (TokenStream[InputPointer].token_type == Token_Class.T_Int ||
                TokenStream[InputPointer].token_type == Token_Class.T_Float ||
                TokenStream[InputPointer].token_type == Token_Class.T_String))
            {
                paramsdash.Children.Add(Param());
                paramsdash.Children.Add(ParamsDash());
                return paramsdash;
            }


            return null;
   
        }

        private Node Comma()
        {
           Node comma_ = new Node(",");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                comma_.Children.Add(match(Token_Class.Comma));
                return comma_;
            }
            return null;
        }

        private Node FunctionBody()
        {
            Node funcBody = new Node("Function Body");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LCurly)
            {
                Console.WriteLine("in body");
                funcBody.Children.Add(match(Token_Class.LCurly));
                Node statementsNode = Statements();
                //if (statementsNode != null)
               // may be there is no statemnt so cheking it is wrong 
                //{

                    Console.WriteLine("in statement");
                    funcBody.Children.Add(statementsNode);
                    Node returnNode = ReturnStmt();
                    if (returnNode != null)
                    {
                        funcBody.Children.Add(returnNode);
                        funcBody.Children.Add(match(Token_Class.RCurly));
                        return funcBody;
                    }
                    Errors.Parser_Error_List.Add("Missing Return Statement");
                    Node eps = new Node("epsilon");
                    funcBody.Children.Add(eps);
                    funcBody.Children.Add(match(Token_Class.RCurly));
                    return funcBody;
                //}
                //Errors.Parser_Error_List.Add("Invalid Statements");
                return null;
            }
            Errors.Parser_Error_List.Add("Missing function body");
            return null;
        }
        private Node L_Curly()
        {
            Node LCurly = new Node("}");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LCurly)
            {
                LCurly.Children.Add(match(Token_Class.LCurly));
                return LCurly;
            }
            Errors.Parser_Error_List.Add("Missing left curly bracket '}'");
            return null;
        }
        private Node R_Curly()
        {
            Node RCurly = new Node("{");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.RCurly)
            {
                RCurly.Children.Add(match(Token_Class.RCurly));
                return RCurly;
            }
            Errors.Parser_Error_List.Add("Missing right curly bracket '}'");
            return null;
        }
        
        private Node Statements()
        {
            Node statements = new Node("Statements");
            Console.WriteLine("statements");
            Node statementNode = Statement();

            if (statementNode != null)
            {
                statements.Children.Add(statementNode);
                Node statementDashNode = StatementDash();
                if (statementDashNode != null)
                {
                    statements.Children.Add(statementDashNode);
                    return statements;
                }
                Errors.Parser_Error_List.Add("Invalid Statement Dash");
                return null;
            }


            // we do not need to check if it is a wrong statment as Statement() does it ;
            /*           if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type != Token_Class.T_return)
            {
                Errors.Parser_Error_List.Add("Invalid Statement or no statments found ");
            }*/
            // edited was null 
            return statementNode;
        }
        private Node Statement()
        {
            
            /*Node statmenets = new Node("Statement");
            if (InputPointer >= TokenStream.Count)
            {
                return null; // Return null if we've reached the end of the TokenStream
            }

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier &&
                    (InputPointer + 1 < TokenStream.Count &&
                    TokenStream[InputPointer + 1].token_type == Token_Class.AssignOp))
                statmenets.Children.Add(AssignStmt());

            else if (InputPointer < TokenStream.Count && 
                (TokenStream[InputPointer].token_type == Token_Class.T_Int ||
                    TokenStream[InputPointer].token_type == Token_Class.T_Float ||
                    TokenStream[InputPointer].token_type == Token_Class.T_String))
                statmenets.Children.Add(DeclStmt());

            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Write)
                statmenets.Children.Add(WriteStmt());

            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Read)
                statmenets.Children.Add(ReadStmt());

            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.If)
                statmenets.Children.Add(IfStmt());

            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Repeat)
                statmenets.Children.Add(RepeatStmt());
            else return null;

            //statmenets.Children.Add(semicolon());
            return statmenets;*/

            Node statement = new Node("Statement");
            Console.WriteLine("in statement");
            if (InputPointer >= TokenStream.Count)
            {
                return null; // Return null if we've reached the end of the TokenStream
            }

            Token currentToken = TokenStream[InputPointer];

            if (currentToken.token_type == Token_Class.Idenifier &&
                (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.AssignOp))
            {
                Console.WriteLine("in assstatement");
                statement.Children.Add(AssignStmt());
            }
            else if (IsDataType(currentToken))
            {
                Console.WriteLine("in decstatement");
                statement.Children.Add(DeclStmt());
            }
            else if (currentToken.token_type == Token_Class.Write)
            {
                Console.WriteLine("in writestatement");
                statement.Children.Add(WriteStmt());
            }
            else if (currentToken.token_type == Token_Class.Read)
            {
                Console.WriteLine("in readstatement");
                statement.Children.Add(ReadStmt());
            }
            else if (currentToken.token_type == Token_Class.If)
            {
                Console.WriteLine("in ifstatement");
                statement.Children.Add(IfStmt());
            }
            else if (currentToken.token_type == Token_Class.Repeat)
            {
                statement.Children.Add(RepeatStmt());
            }
            else
            {
                // should edit  to check if return then no errors should be printed
                // the question here when there should be no statments anymore (return ,else , else if ,end)

                Console.WriteLine(TokenStream[InputPointer].token_type);
                if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type != Token_Class.T_return &&
                    TokenStream[InputPointer].token_type != Token_Class.Else && TokenStream[InputPointer].token_type != Token_Class.ELSEIF
                    && TokenStream[InputPointer].token_type != Token_Class.Until && TokenStream[InputPointer].token_type != Token_Class.End))
                {
                    Errors.Parser_Error_List.Add("Invalid or Unrecognized Statement or not statments found ");
                }
                return null;
            }

            return statement;
        }

        private bool IsDataType(Token token)
        {
            return token.token_type == Token_Class.T_Int ||
                   token.token_type == Token_Class.T_Float ||
                   token.token_type == Token_Class.T_String;
        }

        private Node StatementDash()
        {
            /*Node statements_ = new Node("Statements'");
            Node temp = Statement();
            if(temp != null)
            {
                statements_.Children.Add(temp);
                statements_.Children.Add(StatementDash());

                return statements_;
            }

            return null;
            */

            Node statementDash = new Node("StatementDash");

            Node temp = Statement();
            if (temp != null)
            {
                statementDash.Children.Add(temp);
                Node statementDashTemp = StatementDash();
                if (statementDashTemp != null)
                {
                    statementDash.Children.Add(statementDashTemp);
                    return statementDash;
                }
                Errors.Parser_Error_List.Add("Invalid Statement Dash");
                return null;
            }

            Node eps = new Node("epsilon");
            return eps;
        }

        private Node AssignStmt()
        {
            Node assignment = new Node("Assignment Statement");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                assignment.Children.Add(match(Token_Class.Idenifier));
                assignment.Children.Add(match(Token_Class.AssignOp));
            }
            assignment.Children.Add(Expression());// <---- NOT IMPLEMENTED YET
            assignment.Children.Add(semicolon());

            return assignment;

        }
        private Node AssignR()
        {
            Node assignment = new Node("Assignment R");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.AssignOp)
            {
                //assignment.Children.Add(match(Token_Class.Idenifier));
                assignment.Children.Add(match(Token_Class.AssignOp));
            }
            assignment.Children.Add(Expression());// <---- NOT IMPLEMENTED YET
            //assignment.Children.Add(semicolon());

            return assignment;

        }
        private Node DeclStmt()
        {
            Node declaration = new Node("Declaration Statement");
            declaration.Children.Add(Datatype());
            declaration.Children.Add(IdList());
// we donot need it as we already checked if there is assigment in IdList()
 //           declaration.Children.Add(AssignR());
            declaration.Children.Add(semicolon());

            return declaration;
        }

        private Node IdList()
        {
            Node ids = new Node("Identifiers List");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                ids.Children.Add(match(Token_Class.Idenifier));

                // if int a:=5,b,  if there is equal after the ID
                if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.AssignOp)
                {
                    ids.Children.Add(AssignR());
                }

                ids.Children.Add(IdListDash());
                return ids;
            }

            Errors.Parser_Error_List.Add("Missing Identifier");
            return null;
        }
        private Node IdListDash()
        {
            Node ids_ = new Node("Identifiers List'");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type != Token_Class.Comma) return null;

            ids_.Children.Add(Comma());

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                ids_.Children.Add(match(Token_Class.Idenifier));

                // if int a,b,c:=5  if there is equal after the ID
                if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.AssignOp)
                {
                    ids_.Children.Add(AssignR());
                }
                    ids_.Children.Add(IdListDash());
                return ids_;
            }
            Errors.Parser_Error_List.Add("Missing Identifier");
            return null;
        }

        private Node AssignsList()
        {
            Node assigns = new Node("Assignments List");
            assigns.Children.Add(AssignStmt());
            assigns.Children.Add(AssignsListDash());

            return assigns;
        }

        private Node AssignsListDash()
        {
            Node assigns = new Node("Assignments List'");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type != Token_Class.Comma) return null;
            assigns.Children.Add(Comma());

            Node temp = AssignStmt();

            if (temp != null)
            {
                assigns.Children.Add(temp);
                assigns.Children.Add(AssignsListDash());

                return assigns;
            }

            return null;
        }

        private Node Expression()
        {
            Node Exp = new Node("Expression");
            Console.WriteLine("in expression");


            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.String)
            {
                Exp.Children.Add(match(Token_Class.String));
                return Exp;
            }

            Node temp = Equation();
            if (temp != null)
            {
                Exp.Children.Add(temp);
                return Exp;
            }
            else if (InputPointer < TokenStream.Count &&
                (TokenStream[InputPointer].token_type == Token_Class.Constant ||
                TokenStream[InputPointer].token_type == Token_Class.Idenifier))
            {
                Console.WriteLine("is_id");
                Exp.Children.Add(Term());
                return Exp;

            }
            Errors.Parser_Error_List.Add("Invalid Expression");
            return null;
        }

        private Node Term()
        {
            Node terms = new Node("Terms");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Constant)
                terms.Children.Add(match(Token_Class.Constant));

            else if ((InputPointer < TokenStream.Count) && (TokenStream[InputPointer].token_type == Token_Class.Idenifier) &&    (InputPointer + 1 < TokenStream.Count) &&TokenStream[InputPointer+1].token_type == Token_Class.LParanthesis)
                terms.Children.Add(FuncCall());

            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier) {

                Console.WriteLine("is_idnt term");
                terms.Children.Add(match(Token_Class.Idenifier));
            
            }
               

            return terms;
        }

        private Node Equation()
        {
            Node equation = new Node("Equation");
            
            if (InputPointer < TokenStream.Count &&
               InputPointer + 1 < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Constant || TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                          && (TokenStream[InputPointer + 1].token_type == Token_Class.PlusOp ||
                     TokenStream[InputPointer + 1].token_type == Token_Class.MinusOp ||
                          TokenStream[InputPointer + 1].token_type == Token_Class.MultiplyOp ||
                         (TokenStream[InputPointer + 1].token_type == Token_Class.DivideOp)))
            {

                equation.Children.Add(Term());
                equation.Children.Add(arithOP());
                equation.Children.Add(Equation());
                return equation;
            }


            if (InputPointer < TokenStream.Count  && (TokenStream[InputPointer].token_type == Token_Class.Constant || TokenStream[InputPointer].token_type == Token_Class.Idenifier))
            {
                equation.Children.Add(Term());
                return equation;
            }

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {
                equation.Children.Add(L_parenthesis());
                Node innerEquation = Equation();
                if (innerEquation != null)
                {
                    equation.Children.Add(innerEquation);
                    equation.Children.Add(R_parenthesis());

                    
                    if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.PlusOp ||
                     TokenStream[InputPointer].token_type == Token_Class.MinusOp ||
                          TokenStream[InputPointer].token_type == Token_Class.MultiplyOp ||
                         (TokenStream[InputPointer].token_type == Token_Class.DivideOp)))
                    {

                        equation.Children.Add(arithOP());
                        equation.Children.Add(Equation());
                    }
                    return equation;
                }
            }
            else
            {

                return null;
            }
          
/*
            Node subEquation = SubEquation();
            if (subEquation != null)
            {
                equation.Children.Add(subEquation);
                equation.Children.Add(arithOP());
                equation.Children.Add(Term());
                return equation;
            }

            if (InputPointer < TokenStream.Count &&
                (TokenStream[InputPointer].token_type == Token_Class.Constant ||
                 TokenStream[InputPointer].token_type == Token_Class.Idenifier))
            {
                equation.Children.Add(Term());
                return equation;
            }

  */         

            Errors.Parser_Error_List.Add("Invalid Equation");
            return null;
        }

        private Node SubEquation() // ( Equation ) | Equation | Term
        {
            Node subEquation = new Node("Sub Equation");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {
                subEquation.Children.Add(L_parenthesis());
                Node innerEquation = Equation();
                if (innerEquation != null)
                {
                    subEquation.Children.Add(innerEquation);
                    subEquation.Children.Add(R_parenthesis());
                    return subEquation;
                }
            }
            else if (InputPointer < TokenStream.Count &&
                     (TokenStream[InputPointer].token_type == Token_Class.Constant ||
                      TokenStream[InputPointer].token_type == Token_Class.Idenifier))
            {
                subEquation.Children.Add(Term());
                return subEquation;
            }

            return null; // Missing Equation
        }

        private Node arithOP()
        {
            Node operators = new Node("Operator");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.PlusOp)
                operators.Children.Add(match(Token_Class.PlusOp));

            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.MinusOp)
                operators.Children.Add(match(Token_Class.MinusOp));

            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
                operators.Children.Add(match(Token_Class.MultiplyOp));
        
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.DivideOp)
                operators.Children.Add(match(Token_Class.DivideOp));

            return operators;

        }

        private Node ReturnStmt()
        {
            Node returnStatement = new Node("Return Statement");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_return)
            {
                returnStatement.Children.Add(match(Token_Class.T_return));
                Node expressionNode = Expression();
                if (expressionNode != null)
                {
                    returnStatement.Children.Add(expressionNode);
                    returnStatement.Children.Add(semicolon());
                    return returnStatement;
                }
            }

            Errors.Parser_Error_List.Add("Invalid Return Statement");
            return null;
        }

        private Node WriteStmt()
        {
            Node writeStatement = new Node("Write Statement");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Write)
            {
                Console.WriteLine("in write");
                writeStatement.Children.Add(match(Token_Class.Write));

                if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_endl)
                {
                    Console.WriteLine("is write");
                    writeStatement.Children.Add(match(Token_Class.T_endl));
                    writeStatement.Children.Add(semicolon());
                    return writeStatement;
                }

                Node temp = Expression();
                if (temp != null)
                {
                    Console.WriteLine("is write");
                    writeStatement.Children.Add(temp);
                    writeStatement.Children.Add(semicolon());

                    Console.WriteLine("END write");
                    return writeStatement;
                }

                Errors.Parser_Error_List.Add("Invalid Write Statement");
                return null;
            }
            Errors.Parser_Error_List.Add("Invalid Write Statement");
            return null;
        }

        private Node ReadStmt()
        {
            Node readst = new Node("Read Statement");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Read)
            {
                readst.Children.Add(match(Token_Class.Read));
                readst.Children.Add(match(Token_Class.Idenifier));
                readst.Children.Add(semicolon());

                return readst;
            }
            Errors.Parser_Error_List.Add("Invalid Read Statement");
            return null;
        }

        private Node Paramscall()
        {
            Node Parameters = new Node("Parameters");
            Node temp = Paramcall();
            if (temp != null)
            {
                Parameters.Children.Add(temp);
                Parameters.Children.Add(ParamsDashcall());

                return Parameters;
            }

            return null;
        }

        private Node Paramcall()
        {
            Node Parameter = new Node("Parameter");
       

                Console.WriteLine(TokenStream[InputPointer].token_type);
                if (InputPointer <= TokenStream.Count &&( TokenStream[InputPointer].token_type == Token_Class.Idenifier))
                {
                    Parameter.Children.Add(match(Token_Class.Idenifier));
                    return Parameter;
                }
                if (InputPointer <= TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Constant) 
                {

                 Parameter.Children.Add(match(Token_Class.Constant));
                  return Parameter;
                 }

                Errors.Parser_Error_List.Add("Missing variable name in Parameter Declaration");
                return null;
            


        }

        private Node ParamsDashcall()
        {
            Node paramsdash = new Node("Parameters'");


            Console.WriteLine(TokenStream[InputPointer].token_type);
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
                paramsdash.Children.Add(Comma());

            if (InputPointer != TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier || TokenStream[InputPointer].token_type == Token_Class.Constant)
            {
                paramsdash.Children.Add(Paramcall());
                paramsdash.Children.Add(ParamsDashcall());
                return paramsdash;
            }


            return null;

        }


        private Node FuncCall()
        {
            Node funcall = new Node("Function Call");
            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                funcall.Children.Add(match(Token_Class.Idenifier));

            funcall.Children.Add(L_parenthesis());
            funcall.Children.Add(Paramscall());
            funcall.Children.Add(R_parenthesis());
           // funcall.Children.Add(semicolon());

            return funcall;
        }



        private Node semicolon()
        {
            Node semiColon = new Node("Semi Colon");
            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Semicolon)
            {
                semiColon.Children.Add(match(Token_Class.Semicolon));
                return semiColon;
            }
            Errors.Parser_Error_List.Add("Missing semicolon");

            return null;
        }
        private Node IfStmt()
        {
            Node ifStatement = new Node("If Statement");
            ifStatement.Children.Add(match(Token_Class.If));
            ifStatement.Children.Add(CondStmt());
            ifStatement.Children.Add(match(Token_Class.Then));

            Node statementTemp = Statements();
// we could have if statment without statments inside it so checiking it will be useless
//            if (statementTemp != null)
//            {
                ifStatement.Children.Add(statementTemp);
                if (TokenStream[InputPointer].token_type == Token_Class.Else)
                {
                    ifStatement.Children.Add(ElseStmt());
                    return ifStatement;
                }
                else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.ELSEIF)
                {
                    ifStatement.Children.Add(ElseIfStmt());
                    return ifStatement;
                }
                else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.End)
                {
                    ifStatement.Children.Add(match(Token_Class.End));
                    return ifStatement;
                }
                Errors.Parser_Error_List.Add("invalid if statement missing END ");
                return ifStatement;
//            }

//            Errors.Parser_Error_List.Add("Invalid If statement");
//            return ifStatement;
        }

        private Node CondStmt()
        {
            Node condst = new Node("Condition Statement");
            condst.Children.Add(Condition());
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.AndOp)
            {
                condst.Children.Add(match(Token_Class.AndOp));
                condst.Children.Add(ConditionList());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.OrOp)
            {
                condst.Children.Add(match(Token_Class.OrOp));
                condst.Children.Add(ConditionList());
            }
            return condst;

        }

        private Node Condition()
        {
            Node cond = new Node("Condition");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                cond.Children.Add(match(Token_Class.Idenifier));
                cond.Children.Add(ConditionOP());
                cond.Children.Add(Term());

                return cond;
            }
            Errors.Parser_Error_List.Add("Invalid Condition");
            return null;

        }
        private Node ConditionOP()
        {
            Node cond_op = new Node("Condition Operator");
            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
                cond_op.Children.Add(match(Token_Class.LessThanOp));

            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
                cond_op.Children.Add(match(Token_Class.GreaterThanOp));

            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.EqualOp)
                cond_op.Children.Add(match(Token_Class.EqualOp));

            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
                cond_op.Children.Add(match(Token_Class.NotEqualOp));

            else
                Errors.Parser_Error_List.Add("Invalid Condition Operator");

            return cond_op;

        }

        private Node ConditionList()
        {
            Node condlist = new Node("Condition List");
            condlist.Children.Add(Condition());
            condlist.Children.Add(ConditionListDash());

            return condlist;



        }

        private Node ConditionListDash()
        {
            Node conditionListDash = new Node("Condition List'");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.AndOp)
            {
                conditionListDash.Children.Add(match(Token_Class.AndOp));
                conditionListDash.Children.Add(Condition());
                conditionListDash.Children.Add(ConditionListDash());

                return conditionListDash;
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.OrOp)
            {
                conditionListDash.Children.Add(match(Token_Class.OrOp));
                conditionListDash.Children.Add(Condition());
                conditionListDash.Children.Add(ConditionListDash());

                return conditionListDash;
            }

            return null;
        }

        private Node ElseIfStmt()
        {
            Node elseIfStatement = new Node("Else If Statement");

            elseIfStatement.Children.Add(match(Token_Class.ELSEIF));
            elseIfStatement.Children.Add(CondStmt());
            elseIfStatement.Children.Add(match(Token_Class.Then));

            Node statementTemp = Statements();

            if (statementTemp != null)
            {
                elseIfStatement.Children.Add(statementTemp);
                if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Else)
                {
                    elseIfStatement.Children.Add(ElseStmt());
                    return elseIfStatement;
                }
                else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.ELSEIF)
                {
                    elseIfStatement.Children.Add(ElseIfStmt());
                    return elseIfStatement;
                }
                else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.End)
                {
                    elseIfStatement.Children.Add(match(Token_Class.End));
                    return elseIfStatement;
                }
                Errors.Parser_Error_List.Add("Invalid If statement");
                return null;
            }

            Errors.Parser_Error_List.Add("Invalid Else If statement");
            return null;
        }

        private Node ElseStmt()
        {
            Node elsest = new Node("Else Statement");
            elsest.Children.Add(match(Token_Class.Else));

            Node statementTemp = Statements();

            if (statementTemp != null)
            {
                elsest.Children.Add(statementTemp);
                if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.End)
                {
                    elsest.Children.Add(match(Token_Class.End));
                    return elsest;

                }
                Errors.Parser_Error_List.Add("Invalid Else statement");
                return null;
            }

            Errors.Parser_Error_List.Add("Invalid Else statement");
            return null;
        }
        private Node RepeatStmt()
        {
            Node repeatst = new Node("Repeat Statement");

            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Repeat)
            {
                repeatst.Children.Add(match(Token_Class.Repeat));
                repeatst.Children.Add(Statements());
                repeatst.Children.Add(match(Token_Class.Until));
                repeatst.Children.Add(CondStmt());

                return repeatst;
            }

            Errors.Parser_Error_List.Add("Invalid Repeat statement");
            return null;
        }

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                  //  InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString()  + "\r\n");
                //InputPointer++;
                return null;
            }
        }


        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
