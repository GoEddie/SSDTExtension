using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.IO;


namespace TSQLHelper
{
    public struct ScriptDetail
    {
        public string Name;
        public TSqlTokenType Type;
    }

    public class ScriptProperties
    {

        public ScriptDetail GetScriptDetail(string filename)
        {
            var txtRdr = new StreamReader(filename);
            var parser = new TSql110Parser(true);

            IList<ParseError> errors;
            var sqlFragment = parser.Parse(txtRdr, out errors);

            var inName = false;
            var name = "";

            var result = new ScriptDetail();

            foreach (var a in sqlFragment.ScriptTokenStream)
            {

                switch (a.TokenType)
                {
                    case TSqlTokenType.Procedure:
                        result.Type = a.TokenType;
                        break;
                    case TSqlTokenType.QuotedIdentifier:
                    case TSqlTokenType.Identifier:
                    case TSqlTokenType.Dot:
                        inName = true;
                        name += a.Text;


                        break;

                    case TSqlTokenType.WhiteSpace:
                        if (inName)
                        {
                            result.Name = name;
                            return result;
                        }
                        break;
                }
            }

            return new ScriptDetail();
        }

        public string GetProcNameFromSqlFile(string filename)
        {
            var txtRdr = new StreamReader(filename);
            var parser = new TSql110Parser(true);

            IList<ParseError> errors;
            var sqlFragment = parser.Parse(txtRdr, out errors);
            
            var inName = false;
            var name = "";

            foreach (var a in sqlFragment.ScriptTokenStream)
            {
                
                switch (a.TokenType)
                {
                    case TSqlTokenType.Procedure:
                      
                        break;
                    case TSqlTokenType.QuotedIdentifier:
                    case TSqlTokenType.Identifier:
                    case TSqlTokenType.Dot:
                        inName = true;
                        name += a.Text;


                        break;

                    case TSqlTokenType.WhiteSpace:
                        if (inName)
                            return name;
                        break;
                    default:
                      
                        break;
                }

            }

            var errorMessage = "could not find proc name, are these errors anything to do with it?: " + get_errors(errors);
            throw new Exception(errorMessage);
        }


        string get_errors(IEnumerable<ParseError> errors)
        {
            return errors.Aggregate("", (current, err) => current + (" + " + err.Message));
        }
    }



}

