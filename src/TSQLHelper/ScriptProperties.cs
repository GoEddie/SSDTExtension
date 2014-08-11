using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.IO;


namespace TSQLHelper
{
    public class ScriptProperties
    {

        public string GetProcNameFromSqlFile(string filename)
        {

            TextReader txtRdr = new StreamReader(filename);
            TSql110Parser parser = new TSql110Parser(true);

            IList<ParseError> errors;
            TSqlFragment sqlFragment = parser.Parse(txtRdr, out errors);

            var in_procedure = false;
            var in_name = false;
            string name = "";

            foreach (var a in sqlFragment.ScriptTokenStream)
            {
                Console.WriteLine(a.TokenType + " " + a.Text);

                switch (a.TokenType)
                {
                    case TSqlTokenType.Procedure:
                        in_procedure = true;
                        break;
                    case TSqlTokenType.QuotedIdentifier:
                    case TSqlTokenType.Identifier:
                    case TSqlTokenType.Dot:
                        in_name = true;
                        name += a.Text;


                        break;

                    case TSqlTokenType.WhiteSpace:
                        if (in_name)
                            return name;
                        break;
                    default:
                        in_procedure = false;
                        break;
                }

            }

            var error_message = "could not find proc name, are these errors anything to do with it?: " + get_errors(errors);
            throw new Exception(error_message);
        }


        string get_errors(IList<ParseError> errors)
        {

            string ret = "";
            foreach (var err in errors)
            {
                ret += " + " + err.Message;
            }
            return ret;
        }

    }



}

