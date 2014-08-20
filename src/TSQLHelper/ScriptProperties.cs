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

        public static ScriptDetail GetScriptDetail(string script)
        {
            var reader = new StringReader(script);
            var parser = new TSql110Parser(true);

            IList<ParseError> errors;
            var sqlFragment = parser.Parse(reader, out errors);

            var inName = false;
            var name = "";

            var result = new ScriptDetail();

            foreach (var a in sqlFragment.ScriptTokenStream)
            {

                switch (a.TokenType)
                {
                    case TSqlTokenType.Procedure:
                    case TSqlTokenType.Schema:
                        result.Type = a.TokenType;
                        break;
                    case TSqlTokenType.AsciiStringOrQuotedIdentifier:
                    case TSqlTokenType.QuotedIdentifier:
                    case TSqlTokenType.Identifier:
                    case TSqlTokenType.Dot:
                        inName = true;
                        name += a.Text;


                        break;
                    case TSqlTokenType.EndOfFile:
                    case TSqlTokenType.WhiteSpace:
                        if (inName)
                        {
                            result.Name = name;
                            return result;
                        }
                        break;
                }
            }

            if (result.Name == null)
            {
                throw new Exception(string.Format("could not get name from script. parsing error count: {0}, detail: {1}", errors.Count, get_errors(errors)));
            }

            return new ScriptDetail();
        }

        

        static string get_errors(IEnumerable<ParseError> errors)
        {
            return errors.Aggregate("", (current, err) => current + (" + " + err.Message));
        }
    }



}

