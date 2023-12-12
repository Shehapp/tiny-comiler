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
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        Node Program()
        {
            //Node program = new Node("Program");
            /*program.Children.Add(Header());
            program.Children.Add(DeclSec());
            program.Children.Add(Block());
            program.Children.Add(match(Token_Class.Dot));
            MessageBox.Show("Success");
            */
            /*if (TokenStream.Count == 0) return null;

            if (TokenStream[InputPointer+1].token_type==Token_Class.Main){
                program.Children.Add(MainFunction());
                return program;
            }

            program.Children.Add(FunctionDecl());
            program.Children.Add(MainFunction());

            if (InputPointer != TokenStream.Count)
            { 
                Errors.Parser_Error_List.Add("Nothing should be written after main function");
                return null;
            }
            return program;
            */

            Node program = new Node("Program");

            if (InputPointer >= TokenStream.Count) return null;

            Node temp = FunctionDecl();
            if (temp != null)
            {
                program.Children.Add(temp);
                program.Children.Add(MainFunction());
                if (InputPointer != TokenStream.Count)
                {
                    Errors.Parser_Error_List.Add("Extra content found after main function");
                    return null;
                }
                return program;

            }
            program.Children.Add(MainFunction());
            return program;

        }

        private Node MainFunction()
        {
            Node mainFunc = new Node("Main Function");

            /*mainFunc.Children.Add(MainFunctionHeader());
            mainFunc.Children.Add(DatatypeF());

            if (TokenStream[InputPointer].token_type == Token_Class.Main)
            {
                mainFunc.Children.Add(match(Token_Class.Main));
                return mainFunc;
            }

            Errors.Parser_Error_List.Add("Missing Main function");
            return null;*/
            mainFunc.Children.Add(MainFunctionHeader());
            mainFunc.Children.Add(FunctionBody());

            return mainFunc;
            
        }

        private Node MainFunctionHeader()
        {
            Node mainFuncHead = new Node("Main Function Header");
            mainFuncHead.Children.Add(DatatypeF());
            if (TokenStream[InputPointer].token_type == Token_Class.Main)
            {
                mainFuncHead.Children.Add(match(Token_Class.Main));
                mainFuncHead.Children.Add(L_parenthesis());
                mainFuncHead.Children.Add(R_parenthesis());

                return mainFuncHead;
            }

            Errors.Parser_Error_List.Add("Missing Main Function");
            return null;

        }

        private Node FunctionDecl()
        {
            Node Function_Declaration = new Node("Function Declaration");
            Function_Declaration.Children.Add(FunctionHeader());
            Function_Declaration.Children.Add(FunctionBody());

            return Function_Declaration;
        }

        private Node FunctionHeader()
        {
            Node Function_Header = new Node("Function Header");

            Function_Header.Children.Add(DatatypeF());
            Function_Header.Children.Add(FunctionName());
            Function_Header.Children.Add(L_parenthesis());
            Function_Header.Children.Add(Params());
            Function_Header.Children.Add(R_parenthesis());

            return Function_Header;

        }
        private Node DatatypeF()
        {
            Node Datatype = new Node("Return type");
            if (TokenStream[InputPointer].token_type == Token_Class.T_Int)
            {
                Datatype.Children.Add(match(Token_Class.T_Int));
                return Datatype;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_Float)        
            {
                Datatype.Children.Add(match(Token_Class.T_Float));
                return Datatype;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_String)
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
            if(TokenStream[InputPointer].token_type == Token_Class.Idenifier &&
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
            if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
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
            if (TokenStream[InputPointer].token_type == Token_Class.RParanthesis)
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

                if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
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
            if (TokenStream[InputPointer].token_type == Token_Class.T_Int)
            {
                Datatype.Children.Add(match(Token_Class.T_Int));
                return Datatype;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_Float)
            {
                Datatype.Children.Add(match(Token_Class.T_Float));
                return Datatype;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_String)
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
            if (TokenStream[InputPointer].token_type == Token_Class.T_Int)
            {
                Datatype.Children.Add(match(Token_Class.T_Int));
                return Datatype;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_Float)
            {
                Datatype.Children.Add(match(Token_Class.T_Float));
                return Datatype;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_String)
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

            if (TokenStream[InputPointer].token_type != Token_Class.Comma)
                paramsdash.Children.Add(Comma());

            if (TokenStream[InputPointer].token_type == Token_Class.T_Int ||
                TokenStream[InputPointer].token_type == Token_Class.T_Float ||
                TokenStream[InputPointer].token_type == Token_Class.T_String)
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
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                comma_.Children.Add(match(Token_Class.Comma));
                return comma_;
            }
            return null;
        }

        private Node FunctionBody()
        {
            Node FuncBody = new Node("Funcion Body");
            FuncBody.Children.Add(L_Curly());
            //TODO: [COMPILER'23.MS2 - #3] STATEMENTS - Implement the statements syntax analyzer
            FuncBody.Children.Add(Statements());
            FuncBody.Children.Add(ReturnStmt());
            FuncBody.Children.Add(R_Curly());


            return FuncBody;
        }
        private Node L_Curly()
        {
            Node LCurly = new Node("}");
            if (TokenStream[InputPointer].token_type == Token_Class.LCurly)
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
            if (TokenStream[InputPointer].token_type == Token_Class.RCurly)
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
            statements.Children.Add(Statement());
           
            statements.Children.Add(StatementDash());

            return statements;
        }
        private Node Statement()
        {
            
            Node statmenets = new Node("Statement");
            if (InputPointer >= TokenStream.Count)
            {
                return null; // Return null if we've reached the end of the TokenStream
            }

            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier &&
                    (InputPointer + 1 <= TokenStream.Count &&
                    TokenStream[InputPointer + 1].token_type == Token_Class.AssignOp))
                statmenets.Children.Add(AssignStmt());

            else if (TokenStream[InputPointer].token_type == Token_Class.T_Int ||
                    TokenStream[InputPointer].token_type == Token_Class.T_Float ||
                    TokenStream[InputPointer].token_type == Token_Class.T_String)
                statmenets.Children.Add(DeclStmt());

            else if (TokenStream[InputPointer].token_type == Token_Class.Write)
                statmenets.Children.Add(WriteStmt());

            else if (TokenStream[InputPointer].token_type == Token_Class.Read)
                statmenets.Children.Add(ReadStmt());

            else if (TokenStream[InputPointer].token_type == Token_Class.If)
                statmenets.Children.Add(IfStmt());

            else if (TokenStream[InputPointer].token_type == Token_Class.Repeat)
                statmenets.Children.Add(RepeatStmt());
            else return null;

            //statmenets.Children.Add(semicolon());
            return statmenets;
        }

        private Node StatementDash()
        {
            Node statements_ = new Node("Statements'");
            Node temp = Statement();
            if(temp != null)
            {
                statements_.Children.Add(temp);
                statements_.Children.Add(StatementDash());

                return statements_;
            }

            return null;
        }

        private Node AssignStmt()
        {
            Node assignment = new Node("Assignment Statement");

            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                assignment.Children.Add(match(Token_Class.Idenifier));
                assignment.Children.Add(match(Token_Class.AssignOp));
            }
            assignment.Children.Add(Expression());// <---- NOT IMPLEMENTED YET
            assignment.Children.Add(semicolon());

            return assignment;

        }
        private Node DeclStmt()
        {
            Node declaration = new Node("Declaration Statement");
            declaration.Children.Add(Datatype());
            declaration.Children.Add(IdList());
            declaration.Children.Add(AssignsList());
            declaration.Children.Add(semicolon());

            return declaration;
        }

        private Node IdList()
        {
            Node ids = new Node("Identifiers List");
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                ids.Children.Add(match(Token_Class.Idenifier));
                ids.Children.Add(IdListDash());
                return ids;
            }

            Errors.Parser_Error_List.Add("Missing Identifier");
            return null;
        }
        private Node IdListDash()
        {
            Node ids_ = new Node("Identifiers List'");

            if (TokenStream[InputPointer].token_type != Token_Class.Comma) return null;

            ids_.Children.Add(Comma());

            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                ids.Children.Add(match(Token_Class.Idenifier));
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
            if (TokenStream[InputPointer].token_type != Token_Class.Comma) return null;
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

            if(TokenStream[InputPointer].token_type == Token_Class.T_String)
            {
                Exp.Children.Add(match(Token_Class.T_String));
                return Exp;
            }
            else if(TokenStream[InputPointer].token_type == Token_Class.Constant ||
                TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                Exp.Children.Add(Term());
                return Exp;
            }
            Node temp = Equation();
            if(temp != null)
            {
                Exp.Children.Add(temp);
                return Exp;
            }
            Errors.Parser_Error_List.Add("Invalid Expression");
            return null;
        }

        private Node Term()
        {
            Node terms = new Node("Terms");

            if (TokenStream[InputPointer].token_type == Token_Class.Constant)
                terms.Children.Add(match(Token_Class.Constant));

            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier &&
                (InputPointer+1 <= TokenStream.Count &&
                TokenStream[InputPointer].token_type == Token_Class.LParanthesis))
                terms.Children.Add(FuncCall());

            else if(TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                terms.Children.Add(match(Token_Class.Idenifier));

            return terms;
        }

        private Node Equation()
        {
            Node equation = new Node("Equation");

            if (TokenStream[InputPointer].token_type == Token_Class.Constant ||
                TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                equation.Children.Add(Term());
                return equation;
            }

            equation.Children.Add(SubEquation());
            equation.Children.Add(arithOP());
            equation.Children.Add(Term());


            return equation;
        }

        private Node SubEquation() // ( Equation ) | Equation | Term
        {
            Node subequation = new Node("Sub Equation");

            if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {
                subequation.Children.Add(L_parenthesis());
                subequation.Children.Add(Equation());
                subequation.Children.Add(R_parenthesis());

                return subequation;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Constant ||
                TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                subequation.Children.Add(Term());
                return subequation;
            }

            // missing Equation
            return null;
        }

        private Node arithOP()
        {
            Node operators = new Node("Operator");

            if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
                operators.Children.Add(match(Token_Class.PlusOp));

            else if (TokenStream[InputPointer].token_type == Token_Class.MinusOp)
                operators.Children.Add(match(Token_Class.MinusOp));

            else if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
                operators.Children.Add(match(Token_Class.MultiplyOp));
        
            else if (TokenStream[InputPointer].token_type == Token_Class.DivideOp)
                operators.Children.Add(match(Token_Class.DivideOp));

            return operators;

        }

        private Node ReturnStmt()
        {
            Node returnst = new Node("Return Statement");

            if (TokenStream[InputPointer].token_type == Token_Class.T_return)
                returnst.Children.Add(match(Token_Class.T_return));

            returnst.Children.Add(Expression());
            returnst.Children.Add(semicolon());

            return returnst;
        }

        private Node WriteStmt()
        {
            Node writest = new Node("Write Statement");

            if (TokenStream[InputPointer].token_type == Token_Class.Write)
                writest.Children.Add(match(Token_Class.Write));

            if (TokenStream[InputPointer].token_type == Token_Class.T_endl)
            {
                writest.Children.Add(match(Token_Class.T_endl));
                writest.Children.Add(semicolon());
                return writest;
            }

            Node temp = Expression();
            if(temp != null)
            {
                writest.Children.Add(temp);
                writest.Children.Add(semicolon());
                return writest;
            }

            Errors.Parser_Error_List.Add("Invalid Write Statement");
            return null;
        }

        private Node ReadStmt()
        {
            Node readst = new Node("Read Statement");
            if (TokenStream[InputPointer].token_type == Token_Class.Read)
            {
                readst.Children.Add(match(Token_Class.Read));
                readst.Children.Add(match(Token_Class.Idenifier));
                readst.Children.Add(semicolon());

                return readst;
            }
            Errors.Parser_Error_List.Add("Invalid Read Statement");
            return null;
        }

        private Node FuncCall()
        {
            Node funcall = new Node("Function Call");
            if(TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                funcall.Children.Add(match(Token_Class.Idenifier));

            funcall.Children.Add(L_parenthesis());
            funcall.Children.Add(Params());
            funcall.Children.Add(R_parenthesis());
            funcall.Children.Add(semicolon());

            return funcall;
        }

        private Node semicolon()
        {
            Node semiColon = new Node("Semi Colon");
            if(TokenStream[InputPointer].token_type == Token_Class.Semicolon)
            {
                semiColon.Children.Add(match(Token_Class.Semicolon));
                return semiColon;
            }
            Errors.Parser_Error_List.Add("Missing semicolon");

            return null;
        }
        private Node IfStmt()
        {
            Node ifst = new Node("If Statement");
            ifst.Children.Add(match(Token_Class.If));
            ifst.Children.Add(CondStmt());
            ifst.Children.Add(match(Token_Class.Then));

            Node statementTemp = Statements();
            
            if (statementTemp != null)
            {
                ifst.Children.Add(statementTemp);
                if (TokenStream[InputPointer].token_type == Token_Class.Else)
                {
                    ifst.Children.Add(ElseStmt());
                    return ifst;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.ELSEIF)
                {
                    ifst.Children.Add(ElseIfStmt());
                    return ifst;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.End)
                {
                    ifst.Children.Add(match(Token_Class.End));
                    return ifst;
                }
                Errors.Parser_Error_List.Add("Invalid If statement");
                return null;
            }


            Errors.Parser_Error_List.Add("Invalid If statement");
            return null;
        }

        private Node CondStmt()
        {
            Node condst = new Node("Condition Statement");
            condst.Children.Add(Condition());
            if (TokenStream[InputPointer].token_type == Token_Class.AndOp)
            {
                condst.Children.Add(match(Token_Class.AndOp));
                condst.Children.Add(ConditionList());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.OrOp)
            {
                condst.Children.Add(match(Token_Class.OrOp));
                condst.Children.Add(ConditionList());
            }
            return condst;

        }

        private Node Condition()
        {
            Node cond = new Node("Condition");

            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
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
            if(TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
                cond_op.Children.Add(match(Token_Class.LessThanOp));

            else if (TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
                cond_op.Children.Add(match(Token_Class.GreaterThanOp));

            else if (TokenStream[InputPointer].token_type == Token_Class.EqualOp)
                cond_op.Children.Add(match(Token_Class.EqualOp));

            else if (TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
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
            Node condlist_ = new Node("Condition List'");

            if (TokenStream[InputPointer].token_type == Token_Class.AndOp)
            {
                condlist_.Children.Add(match(Token_Class.AndOp));
                condlist_.Children.Add(Condition());
                condlist_.Children.Add(ConditionListDash());

                return condlist_;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.OrOp)
            {
                condlist_.Children.Add(match(Token_Class.OrOp));
                condlist_.Children.Add(Condition());
                condlist_.Children.Add(ConditionListDash());

                return condlist_;
            }

            return null;
        }

        private Node ElseIfStmt()
        {
            Node elsifst = new Node("Else If Statement");

            elsifst.Children.Add(match(Token_Class.ELSEIF));
            elsifst.Children.Add(CondStmt());
            elsifst.Children.Add(match(Token_Class.Then));

            Node statementTemp = Statements();

            if (statementTemp != null)
            {
                ifst.Children.Add(statementTemp);
                if (TokenStream[InputPointer].token_type == Token_Class.Else)
                {
                    ifst.Children.Add(ElseStmt());
                    return ifst;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.ELSEIF)
                {
                    ifst.Children.Add(ElseIfStmt());
                    return ifst;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.End)
                {
                    ifst.Children.Add(match(Token_Class.End));
                    return ifst;
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
                if (TokenStream[InputPointer].token_type == Token_Class.End)
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

            if(TokenStream[InputPointer].token_type == Token_Class.Repeat)
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
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString()  + "\r\n");
                InputPointer++;
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

        /*
        Node Header()
        {
            Node header = new Node("Header");
            // write your code here to check the header sructure
            return header;
        }
        Node DeclSec()
        {
            Node declsec = new Node("DeclSec");
            // write your code here to check atleast the declare sturcure 
            // without adding procedures
            return declsec;
        }
        Node Block()
        {
            Node block = new Node("block");
            // write your code here to match statements
            return block;
        }

        // Implement your logic here
        */