using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{

    //operators
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp, assign, and, or,


    //Reserved
    Integer, Float, String, read, write, repeat, until, If, Elseif, Else, then, Return, Endl, Idenifier, end,
    //others
    Comment_Statement, Number, LeftBraces, RightBraces, 

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
        static String Letter = "[_|a-z|A-Z]";
        static String Digit = "[0-9]";
        static String Nunsigned = Digit + "+";
        static String Nsigned = "(+|-)?" + Nunsigned;
        static String Float = Nsigned + "( \\." + Nunsigned + ")?";
        //static String tmp;
        static bool found;
        static int found2;
        static bool instring;
        static bool ischar = false;
        static bool num = false;

        static bool error = true;
        public Scanner()
        {
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("elseif", Token_Class.Elseif);
            ReservedWords.Add("end", Token_Class.end);
            ReservedWords.Add("int", Token_Class.Integer);
            ReservedWords.Add("then", Token_Class.then);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("read", Token_Class.read);
            ReservedWords.Add("write", Token_Class.write);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("until", Token_Class.until);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("repeat", Token_Class.repeat);

            Operators.Add(".", Token_Class.Dot);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add(":=", Token_Class.assign);

            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("&&", Token_Class.and);
            Operators.Add("||", Token_Class.or);

            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("{", Token_Class.LeftBraces);
            Operators.Add("}", Token_Class.RightBraces);
        }

        public void StartScanning(string SourceCode)
        {
            //Console.WriteLine(SourceCode);

            for (int i = 0; i < SourceCode.Length; i++)
            {
                found2 = 0;
                error = true;
                // Console.WriteLine(SourceCode[i]);
                int j = i + 1;
                found = false;
                char CurrentChar = SourceCode[i];

                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                {

                    continue;
                }
                else if (CurrentChar == '|')
                {
                    if (j < SourceCode.Length)
                    {
                        if (SourceCode[j] == '|')
                        {
                            CurrentLexeme += SourceCode[j];
                            FindTokenClass(CurrentLexeme);
                            j++;
                            found = true;
                        }
                        else
                        {
                            FindTokenClass(CurrentChar.ToString());
                        }
                    }
                }
                else if (CurrentChar == '&')
                {
                    if (j < SourceCode.Length)
                    {
                        if (SourceCode[j] == '&')
                        {
                            CurrentLexeme += SourceCode[j];
                            FindTokenClass(CurrentLexeme);
                            j++;
                            found = true;
                        }
                        else
                        {
                            FindTokenClass(CurrentChar.ToString());
                        }
                    }
                }
                else if (CurrentChar == ':')
                {
                    if (j < SourceCode.Length)
                    {
                        if (SourceCode[j] == '=')
                        {
                            CurrentLexeme += SourceCode[j];
                            FindTokenClass(CurrentLexeme);
                            j++;
                            found = true;
                        }
                        else
                        {
                            FindTokenClass(CurrentChar.ToString());
                            j++;
                            found = true;
                        }
                    }
                    else
                    {
                        FindTokenClass(CurrentChar.ToString());
                        j++;
                        found = true;


                    }

                }
                else if (Operators.ContainsKey(CurrentChar.ToString()) && CurrentChar != '/' && CurrentChar != '.')
                {

                    FindTokenClass(CurrentChar.ToString());
                    //i loop
                    continue;
                }
                //comments
                else if (CurrentChar == '/')
                {
                    if (j < SourceCode.Length)
                    {
                        ischar = false;
                        found2 = 0;
                        while (true)
                        {
                            if (j >= SourceCode.Length)
                            {

                                break;
                            }
                            else if (SourceCode[j] == '/' && found2 == 2)
                            {
                                //Console.WriteLine("helooo");

                                CurrentLexeme += SourceCode[j];
                                FindTokenClass(CurrentLexeme);
                                found = true;
                                j++;
                                break;
                            }
                            //operation
                            else if (char.IsDigit(SourceCode[j]) && found2 == 0)
                            {

                                FindTokenClass(CurrentLexeme);
                                FindTokenClass(SourceCode[j].ToString());
                                j += 2;
                                found = true;
                                break;

                            }
                            else if (SourceCode[j] == '*')
                            {

                                // Console.WriteLine("helooo");

                                found2++;
                            }
                            else if (SourceCode[j] == ' ')
                            {
                                // CurrentLexeme += SourceCode[j];
                                Console.WriteLine("helooo");

                                j++;
                                continue;
                            }
                            else if (char.IsLetter(SourceCode[j]) && found2 == 0)
                            {
                                Console.WriteLine("i'm here " + SourceCode[j + 1]);


                                FindTokenClass(CurrentLexeme);

                                found = true;
                                break;

                            }
                            CurrentLexeme += SourceCode[j];
                            j++;


                        }
                        i = j - 1;

                        if (found == false)
                        {
                            //Console.WriteLine(CurrentLexeme);
                            FindTokenClass(CurrentLexeme);
                            found = true;
                        }

                    }

                    else
                    {

                        FindTokenClass(CurrentChar.ToString());
                        found = true;
                    }
                }

                else if (SourceCode[i] == '"')
                {
                    instring = true;
                    while (j < SourceCode.Length && (SourceCode[j] != '\r' || SourceCode[j] != '\n'))
                    {
                        CurrentLexeme += SourceCode[j];
                        if (SourceCode[j] == '"')
                        {
                            instring = false;
                            j++;
                            found = true;
                            break;
                        }
                        j++;
                    }
                    i = j - 1;
                    found = true;
                    FindTokenClass(CurrentLexeme);

                }
                else
                {

                    while (j < SourceCode.Length && (SourceCode[j] != '\r' || SourceCode[j] != '\n'))// && (char.IsLetter(CurrentChar) || char.IsDigit(CurrentChar) || CurrentChar == '"'))
                    {


                        if (Operators.ContainsKey(SourceCode[j].ToString()) && SourceCode[j] != '.' && SourceCode[j] != ':')
                        {
                            FindTokenClass(CurrentLexeme);
                            FindTokenClass(SourceCode[j].ToString());

                            j++;
                            found = true;
                            break;

                        }
                        else if (SourceCode[j] == ':' && !instring)
                        {
                            if (j + 1 < SourceCode.Length)
                            {
                                if (SourceCode[j + 1] == '=')
                                {
                                    FindTokenClass(CurrentLexeme);
                                    FindTokenClass(SourceCode[j] + SourceCode[j + 1].ToString());
                                    j += 2;
                                    found = true;
                                    break;
                                }
                                else
                                {
                                    FindTokenClass(CurrentLexeme);
                                    Errors.Error_List.Add(SourceCode[j].ToString());
                                    //عشان ميدخلش ف اخر كوندشن
                                    error = false;
                                    j++;
                                    found = true;
                                    break;
                                }
                            }
                            else
                            {
                                FindTokenClass(CurrentLexeme);
                                Errors.Error_List.Add(SourceCode[j].ToString());
                                error = false;
                                j++;
                                found = true;
                                break;

                            }
                        }
                        else
                        {
                            if (SourceCode[j] != ' ' && SourceCode[j] != '\n')
                            {
                                if (SourceCode[j] == '!')
                                {
                                    FindTokenClass(CurrentLexeme);
                                    CurrentLexeme = SourceCode[j].ToString();

                                    FindTokenClass(CurrentLexeme);
                                    j++;
                                    found = true;
                                    break;
                                }
                                /*if (j+1<SourceCode.Length&&SourceCode[j] == ':') {
                               

                                }*/

                                else if (SourceCode[j] == '&' || SourceCode[j] == '|')
                                {
                                    FindTokenClass(CurrentLexeme);

                                    found = true;
                                    break;
                                }
                                CurrentLexeme += SourceCode[j];
                                j++;
                            }
                            //space found
                            else
                            {

                                FindTokenClass(CurrentLexeme);
                                //CurrentLexeme = "";
                                j++;
                                found = true;
                                break;

                            }
                        }


                    }
                }


                i = j - 1;

                if (found == false)
                    FindTokenClass(CurrentLexeme);





                if (error)
                {

                    Errors.Error_List.Add(CurrentLexeme);
                }




            }

            JASON_Compiler.TokenStream = Tokens;
        }

        void FindTokenClass(string Lex)
        {

            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex.Trim()))
            {
                Tok.token_type = ReservedWords[Lex.Trim()];
                error = false;
                Tokens.Add(Tok);
            }
            else if (Operators.ContainsKey(Lex.Trim()) || Lex.Trim() == ":=")
            {
                Tok.token_type = Operators[Lex.Trim()];
                error = false;
                Tokens.Add(Tok);
            }

            //Is it a string?
            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.String;
                Tokens.Add(Tok);
                error = false;

            }

            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
                error = false;

            }



            //Is it an Number?
            else if (isNumber(Lex.Trim()))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);
                error = false;

            }
            //Is it a isComment?
            else if (isComment(Lex))
            {
                Tok.token_type = Token_Class.Comment_Statement;
                Tokens.Add(Tok);
                error = false;

            }

            //Is it an float
            /*else if (isFloat(Lex))
            {
                Tok.token_type = Token_Class.Float;
                Tokens.Add(Tok);
                error = false;

            }*/
            else
            {
                error = true;
            }
        }

        bool isString(string lex)
        {
            bool isValid = true;
            Regex reg = new Regex("^\"(" + Digit + "|.| )*\"$");
            if (!reg.IsMatch(lex))
            {
                isValid = false;
            }
            else
            {
                error = false;
            }
            return isValid;
        }

        bool isNumber(string lex)
        {
            bool isValid = true;
            //Digit *(. Digit)?
            Regex reg = new Regex("^" + Digit + "*(\\.(" + Digit + ")+)?$");
            if (!reg.IsMatch(lex))
            {
                isValid = false;
            }
            else
            {
                error = false;
            }
            return isValid;
        }

        bool isIdentifier(string lex)
        {
            bool isValid = true;
            Regex reg = new Regex("^" + Letter + "(.|" + Digit + ")*$");
            // Check if the lex is an identifier or not. Letter(Letter|Digit)*
            if (!reg.IsMatch(lex))
            {
                isValid = false;
            }
            else
            {
                error = false;
            }
            return isValid;
        }
        bool isComment(string lex)
        {
            //4-	/([a-z]|[0-9]|[A_Z])  */
            bool isValid = true;
            Regex reg = new Regex("^/( |\\n)*" + "\\*(.|" + Digit + "|\\s)*\\*" + "( |\\s)*/ *");            // Check if the lex is a constant (Number) or not.

            if (!reg.IsMatch(lex))
            {
                isValid = false;
            }
            else
            {
                error = false;
            }
            return isValid;
        }

        /*bool isFloat(string lex)
        {
            //4-	/([a-z]|[0-9]|[A_Z])  
            bool isValid = true;
            Regex reg = new Regex("^"+Digit+"\\.("+Digit+")+");
         

            if (!reg.IsMatch(lex))
            {
                isValid = false;
            }
            else
            {
                error = false;
            }
            return isValid;
        }*/
    }
}