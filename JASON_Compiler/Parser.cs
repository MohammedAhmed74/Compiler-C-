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
            Node program = new Node("Program");
            program.Children.Add(Function());
            program.Children.Add(ProgramDash());
            return program;
        }

        private Node ProgramDash()
        {
            if (InputPointer > TokenStream.Count - 1)
                return null;
            Node programDash = new Node("programDash");
            if(TokenStream[InputPointer].token_type == Token_Class.Integer || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.String)
            {
                programDash.Children.Add(Function());
                programDash.Children.Add(ProgramDash());
                return programDash;
            }
            return null;
        }

        private Node Function()
        {
            Node function = new Node("function");
            function.Children.Add(FunctionHead());
            function.Children.Add(FunctionBody());
            return function;
        }

        private Node FunctionBody()
        {
            Node functionBody = new Node("functionBody");
            functionBody.Children.Add(match(Token_Class.LeftBraces));
            functionBody.Children.Add(Statments());
            functionBody.Children.Add(match(Token_Class.RightBraces));
            return functionBody;
        }

        private Node Statments()
        {
            Node statments = new Node("statments");

            switch (TokenStream[InputPointer].token_type)
            {
                case Token_Class.Comment_Statement:
                    statments.Children.Add(match(Token_Class.Comment_Statement));
                    statments.Children.Add(StatmentsDash());
                    break;

                case Token_Class.repeat:
                    statments.Children.Add(Rebeat());
                    statments.Children.Add(StatmentsDash());
                    break;

                case Token_Class.Integer:
                case Token_Class.Float:
                case Token_Class.String:
                    statments.Children.Add(Declaration_Statement());
                    statments.Children.Add(StatmentsDash());;
                    break;

                case Token_Class.Idenifier:
                    int saveIndex = InputPointer;
                    Node check = new Node("check");
                    check.Children.Add(match(Token_Class.Idenifier));

                    if(TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
                    {
                        InputPointer = saveIndex;
                        statments.Children.Add(FunctionCall());
                        statments.Children.Add(StatmentsDash());
                        break;
                    }
                    InputPointer = saveIndex;
                    statments.Children.Add(VariableDeclare());
                    statments.Children.Add(StatmentsDash());
                    break;

                case Token_Class.If:
                    statments.Children.Add(If_condition());
                    statments.Children.Add(StatmentsDash());
                    break;

                case Token_Class.write:
                    statments.Children.Add(Write_statment());
                    statments.Children.Add(StatmentsDash());
                    break;

                case Token_Class.read:
                    statments.Children.Add(Read_statment());
                    statments.Children.Add(StatmentsDash());
                    break;

                case Token_Class.Return:
                    statments.Children.Add(Return_statment());
                    statments.Children.Add(StatmentsDash());;
                    break;
            }
            return statments;
                
        }

        private Node Return_statment()
        {
            Node return_statment = new Node("return_statment");
            return_statment.Children.Add(match(Token_Class.Return));
            return_statment.Children.Add(Expression());
            return_statment.Children.Add(match(Token_Class.Semicolon));
            return return_statment;
        }

        private Node Read_statment()
        {
            Node read_statment = new Node("read_statment");
            read_statment.Children.Add(match(Token_Class.read));
            read_statment.Children.Add(match(Token_Class.Idenifier));
            read_statment.Children.Add(match(Token_Class.Semicolon));
            return read_statment;
        }

        private Node Write_statment()
        {
            Node write_statment = new Node("write_statment");
            write_statment.Children.Add(match(Token_Class.write));
            write_statment.Children.Add(Output());
            write_statment.Children.Add(match(Token_Class.Semicolon));
            return write_statment;
        }

        private Node Output()
        {
            Node output = new Node("output");
            if(TokenStream[InputPointer].token_type == Token_Class.Endl)
            {
                output.Children.Add(match(Token_Class.Endl));
                return output;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.String || TokenStream[InputPointer].token_type == Token_Class.Idenifier || TokenStream[InputPointer].token_type == Token_Class.Number)
            {
                output.Children.Add(Expression());
                return output;
            }
            return null; // error handling
        }

        private Node If_condition()
        {
            Node if_condition = new Node("if_conidtion");
            if_condition.Children.Add(match(Token_Class.If));
            if_condition.Children.Add(Condition());
            if_condition.Children.Add(match(Token_Class.then));
            if_condition.Children.Add(Statments());
            if_condition.Children.Add(If_conditionDash());
            if_condition.Children.Add(match(Token_Class.end)); //                                           
            return if_condition;
        }

        private Node If_conditionDash()
        {
            Node if_conditionDash = new Node("if_conditionDash");
            if(TokenStream[InputPointer].token_type == Token_Class.Else)
            {
                if_conditionDash.Children.Add(match(Token_Class.Else));
                if_conditionDash.Children.Add(Statments());
                return if_conditionDash;
            }
            else if(TokenStream[InputPointer].token_type == Token_Class.Elseif)
            {
                if_conditionDash.Children.Add(match(Token_Class.Elseif));
                if_conditionDash.Children.Add(Condition());
                if_conditionDash.Children.Add(match(Token_Class.then));
                if_conditionDash.Children.Add(Statments());
                if_conditionDash.Children.Add(If_conditionDash());
                return if_conditionDash;
            }
            return null;
        }

        private Node Declaration_Statement()
        {
            Node declarition_statment = new Node("declaration_statment");
            declarition_statment.Children.Add(DataType());
            declarition_statment.Children.Add(match(Token_Class.Idenifier));
            declarition_statment.Children.Add(Assignment());
            return declarition_statment;
        }

        private Node Assignment()
        {
            Node assignment = new Node("assignment");
            if (TokenStream[InputPointer].token_type == Token_Class.Semicolon)
            {
                assignment.Children.Add(match(Token_Class.Semicolon));
                return assignment;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {

                assignment.Children.Add(match(Token_Class.Comma));
                assignment.Children.Add(VariableDeclare());
                return assignment;
            }
            else
                assignment.Children.Add(VariableDeclare());
            return assignment;
        }

        private Node VariableDeclare()
        {
            Node variableDeclare = new Node("variableDeclare");
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                int saveIndex = InputPointer;
                Node check = new Node("check");
                check.Children.Add(match(Token_Class.Idenifier));
                if(TokenStream[InputPointer].token_type == Token_Class.assign)
                {
                    InputPointer = saveIndex;
                    variableDeclare.Children.Add(match(Token_Class.Idenifier));
                    variableDeclare.Children.Add(match(Token_Class.assign));
                    variableDeclare.Children.Add(Expression());
                    variableDeclare.Children.Add(Assignment());
                    return variableDeclare;
                }
                else
                {
                    InputPointer = saveIndex;
                    variableDeclare.Children.Add(match(Token_Class.Idenifier));
                    variableDeclare.Children.Add(Assignment());
                    return variableDeclare;
                }
            }
            else
            if (TokenStream[InputPointer].token_type == Token_Class.assign)
            {

                variableDeclare.Children.Add(match(Token_Class.assign));
                variableDeclare.Children.Add(Expression());
                variableDeclare.Children.Add(Assignment());
                return variableDeclare;
            }

            return null; // error handling
        }

        private Node Expression()
        {
            Node expression = new Node("expression");
            if (TokenStream[InputPointer].token_type == Token_Class.String)
            {
                expression.Children.Add(match(Token_Class.String));
                return expression;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier || TokenStream[InputPointer].token_type == Token_Class.Number)
            {
                int saveIndex = InputPointer;
                Node check = new Node("check");
                check.Children.Add(Term());
                if (TokenStream[InputPointer].token_type == Token_Class.PlusOp || TokenStream[InputPointer].token_type == Token_Class.MinusOp || TokenStream[InputPointer].token_type == Token_Class.MultiplyOp || TokenStream[InputPointer].token_type == Token_Class.DivideOp)
                {
                    InputPointer = saveIndex;
                    expression.Children.Add(Equation());
                    return expression;
                }
                InputPointer = saveIndex;
                expression.Children.Add(Term());
                return expression;
            }
            return null;//error handling
        }

        private Node Equation()
        {
            Node equation = new Node("equation");
            if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {
                equation.Children.Add(EquationDash());
                return equation;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier || TokenStream[InputPointer].token_type == Token_Class.Number)
            {
                equation.Children.Add(Term());
                equation.Children.Add(EquationDash());
                return equation;
            }
            return null; //  error handling
        }

        private Node EquationDash() //left factoring
        {
            Node equationDash = new Node("equationDash");
            if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {
                equationDash.Children.Add(match(Token_Class.LParanthesis));
                equationDash.Children.Add(Term());
                equationDash.Children.Add(ArithmaticOp());
                equationDash.Children.Add(Term());
                equationDash.Children.Add(match(Token_Class.RParanthesis));
                equationDash.Children.Add(EquationDash());
                return equationDash;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier || TokenStream[InputPointer].token_type == Token_Class.Number)
            {
                equationDash.Children.Add(Term());
                equationDash.Children.Add(EquationDash());
                return equationDash;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.PlusOp || TokenStream[InputPointer].token_type == Token_Class.MinusOp || TokenStream[InputPointer].token_type == Token_Class.MultiplyOp || TokenStream[InputPointer].token_type == Token_Class.DivideOp)
            {
                equationDash.Children.Add(ArithmaticOp());
                equationDash.Children.Add(EquationDash());
                return equationDash;
            }
            else 
                return null;
        }

        private Node ArithmaticOp()
        {
            Node arithmaticOp = new Node("arithmaticOp");
            if(TokenStream[InputPointer].token_type == Token_Class.PlusOp)
            {
                arithmaticOp.Children.Add(match(Token_Class.PlusOp));
                return arithmaticOp;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.MinusOp)
            {
                arithmaticOp.Children.Add(match(Token_Class.MinusOp));
                return arithmaticOp;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
            {
                arithmaticOp.Children.Add(match(Token_Class.MultiplyOp));
                return arithmaticOp;
            }
            else if(TokenStream[InputPointer].token_type == Token_Class.DivideOp)
            {
                arithmaticOp.Children.Add(match(Token_Class.DivideOp));
                return arithmaticOp;
            }
            return null; // error handling
        }

        private Node Rebeat()
        {
            Node rebeat = new Node("rebeat");
            rebeat.Children.Add(match(Token_Class.repeat));
            rebeat.Children.Add(Statments());
            rebeat.Children.Add(match(Token_Class.until));
            rebeat.Children.Add(Condition());
            return rebeat;
        }

        private Node Condition()
        {
            Node condition = new Node("condition");
            condition.Children.Add(match(Token_Class.Idenifier));
            condition.Children.Add(ConditionOperators());
            condition.Children.Add(Term());
            condition.Children.Add(ConditionDash());
            return condition;
        }

        private Node ConditionDash()//                                       lsa ashraf hy3ml el && ||
        {
            Node conditionDash = new Node("conditionDash");
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                conditionDash.Children.Add(match(Token_Class.Idenifier));
                conditionDash.Children.Add(ConditionOperators());
                conditionDash.Children.Add(Term());
                conditionDash.Children.Add(ConditionDash());
                return conditionDash;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.or || TokenStream[InputPointer].token_type == Token_Class.and)
            {
                conditionDash.Children.Add(BoolOperators());
                conditionDash.Children.Add(ConditionDash());
                return conditionDash;
            }
            return null;
               
        }

        private Node BoolOperators()
        {
            Node boolOperators = new Node("conditionDash");
            if (TokenStream[InputPointer].token_type == Token_Class.and)
            {
                boolOperators.Children.Add(match(Token_Class.and));
                return boolOperators;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.or)
            {
                boolOperators.Children.Add(match(Token_Class.or));
                return boolOperators;
            }
            return null;

        }

        private Node Term()
        {
            Node term = new Node("term");
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                int saveIndex = InputPointer;
                Node check = new Node("check");
                check.Children.Add(match(Token_Class.Idenifier));
                if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
                {
                    InputPointer = saveIndex;
                    term.Children.Add(FunctionCall());
                    return term;
                }
                InputPointer = saveIndex;
                term.Children.Add(match(Token_Class.Idenifier));
                return term;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Number)
            {
                term.Children.Add(match(Token_Class.Number));
                return term;
            }

            return term; // error handeling

        }

        private Node FunctionCall()
        {
            Node functionCall = new Node("functionCall");
            functionCall.Children.Add(match(Token_Class.Idenifier));
            functionCall.Children.Add(match(Token_Class.LParanthesis));
            functionCall.Children.Add(Parameters());
            functionCall.Children.Add(match(Token_Class.RParanthesis));
            return functionCall;
        }

        private Node ConditionOperators()
        {
            Node conditionOperators = new Node("conditionOperators");
            if (TokenStream[InputPointer].token_type == Token_Class.EqualOp)
            {
                conditionOperators.Children.Add(match(Token_Class.EqualOp));
                return conditionOperators;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
            {
                conditionOperators.Children.Add(match(Token_Class.NotEqualOp));
                return conditionOperators;
            }
            else if(TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
            {
                conditionOperators.Children.Add(match(Token_Class.GreaterThanOp));
                return conditionOperators;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
            {
                conditionOperators.Children.Add(match(Token_Class.LessThanOp));
                return conditionOperators;
            }

            return conditionOperators; // error handeling
        }

        private Node StatmentsDash()
        {

            Node statmentsDash = new Node("statmentsDash");

            switch (TokenStream[InputPointer].token_type)
            {
                case Token_Class.Comment_Statement:
                    statmentsDash.Children.Add(match(Token_Class.Comment_Statement));
                    statmentsDash.Children.Add(StatmentsDash());
                    return statmentsDash;
                    break;

                case Token_Class.repeat:
                    statmentsDash.Children.Add(Rebeat());
                    statmentsDash.Children.Add(StatmentsDash());
                    return statmentsDash;
                    break;

                case Token_Class.Integer:
                case Token_Class.Float:
                case Token_Class.String:
                    statmentsDash.Children.Add(Declaration_Statement());
                    statmentsDash.Children.Add(StatmentsDash());
                    return statmentsDash;
                    break;

                case Token_Class.Idenifier:
                    int saveIndex = InputPointer;
                    Node check = new Node("check");
                    check.Children.Add(match(Token_Class.Idenifier));

                    if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
                    {
                        InputPointer = saveIndex;
                        statmentsDash.Children.Add(FunctionCall());
                        statmentsDash.Children.Add(StatmentsDash());
                        return statmentsDash;
                    }
                    InputPointer = saveIndex;
                    statmentsDash.Children.Add(VariableDeclare());
                    statmentsDash.Children.Add(StatmentsDash());
                    return statmentsDash;
                    break;

                case Token_Class.If:
                    statmentsDash.Children.Add(If_condition());
                    statmentsDash.Children.Add(StatmentsDash());
                    return statmentsDash;
                    break;

                case Token_Class.write:
                    statmentsDash.Children.Add(Write_statment());
                    statmentsDash.Children.Add(StatmentsDash());
                    return statmentsDash;
                    break;

                case Token_Class.read:
                    statmentsDash.Children.Add(Read_statment());
                    statmentsDash.Children.Add(StatmentsDash());
                    return statmentsDash;
                    break;

                case Token_Class.Return:
                    statmentsDash.Children.Add(Return_statment());
                    statmentsDash.Children.Add(StatmentsDash());
                    return statmentsDash;
                    break;
            }
            return null;

        }

        private Node FunctionHead()
        {
            Node functionHead = new Node("functionHead");
            functionHead.Children.Add(DataType());
            functionHead.Children.Add(match(Token_Class.Idenifier));
            functionHead.Children.Add(ParametersList());
            return functionHead;
        }

        private Node ParametersList()
        {
            Node parametersList = new Node("parametersList");
            parametersList.Children.Add(match(Token_Class.LParanthesis));

            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier || TokenStream[InputPointer].token_type == Token_Class.RParanthesis)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.RParanthesis)
                {
                    parametersList.Children.Add(match(Token_Class.RParanthesis));
                    return parametersList;
                }
                parametersList.Children.Add(Parameters());
                parametersList.Children.Add(match(Token_Class.RParanthesis));
                return parametersList;
            }
            
            parametersList.Children.Add(ParametersDeclartion());
            parametersList.Children.Add(match(Token_Class.RParanthesis));
            return parametersList;

        }

        private Node ParametersDeclartion()
        {
            Node parametersDeclartion = new Node("parametersDeclartion");
            parametersDeclartion.Children.Add(DataType());
            parametersDeclartion.Children.Add(match(Token_Class.Idenifier));
            parametersDeclartion.Children.Add(ParametersDeclartionDash());
            return parametersDeclartion;
        }

        private Node ParametersDeclartionDash()
        {
            Node parametersDeclartionDash = new Node("parametersDeclartionDash");
            if(TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                parametersDeclartionDash.Children.Add(match(Token_Class.Comma));
                parametersDeclartionDash.Children.Add(DataType());
                parametersDeclartionDash.Children.Add(match(Token_Class.Idenifier));
                parametersDeclartionDash.Children.Add(ParametersDeclartionDash());
                return parametersDeclartionDash;
            }
            return null;
        }

        private Node Parameters()
        {
            Node parameters = new Node("parameters");
            parameters.Children.Add(match(Token_Class.Idenifier));
            parameters.Children.Add(ParametersDash());
            return parameters;
        }

        private Node ParametersDash()
        {
            Node parametersDash = new Node("parameters");
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                parametersDash.Children.Add(match(Token_Class.Comma));
                parametersDash.Children.Add(match(Token_Class.Idenifier));
                parametersDash.Children.Add(ParametersDash());
                return parametersDash;
            }
            return null;
        }

        private Node DataType()
        {
            Node dataType = new Node("dataType");
            if(TokenStream[InputPointer].token_type == Token_Class.Integer)
            {
                dataType.Children.Add(match(Token_Class.Integer));
                return dataType;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Float)
            {
                dataType.Children.Add(match(Token_Class.Float));
                return dataType;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.String)
            {
                dataType.Children.Add(match(Token_Class.String));
                return dataType;
            }

            return null;
        }


        // Implement your logic here

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
                    Errors.ParserError_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.ParserError_List.Add("Parsing Error: Expected "
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
