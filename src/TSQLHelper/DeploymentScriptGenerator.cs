using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TSQLHelper
{
    public class DeploymentScriptGenerator
    {
        public static string BuildDropStatment(string createStatement)
        {
            var scriptType = ScriptProperties.GetScriptDetail(createStatement);

            const string dropStatementFormat =
                "if exists (select * from {0} where name = '{1}')\r\n\t{2} {3}\r\n";

            var name = CleanName(scriptType.Name);
            var viewName = "";
            var dropCommand = "";

            switch (scriptType.Type)
            {
                case TSqlTokenType.Proc:
                case TSqlTokenType.Procedure:

                    viewName = "sys.procedures";
                    dropCommand = "drop procedure";

                    break;
                default:
                    throw new TSqlDeploymentException("Only stored procedure creates are currently supported");
            }

            return string.Format(dropStatementFormat, viewName, GetLastPartOfName(name), dropCommand, EscapeIdentifier(name));
        }

        private static string CleanName(string name)
        {
            return name.Replace("[", "").Replace("]", "").Replace("\"", "");
        }

        public static string GetLastPartOfName(string procName)
        {
            var parts = procName.Replace("\"", "").Replace("[", "").Replace("]", "").Split(new[] { '.' });
            return parts[parts.Length - 1];
        }

        public static string EscapeIdentifier(string identifier)
        {
            var builder = new StringBuilder();
            var parts = identifier.Split(new[] {'.'});
            bool first = true;

            foreach (var part in parts)
            {
                if (first)
                {
                    builder.AppendFormat("[{0}]",part);
                    first = false;
                }
                else
                {
                    builder.AppendFormat(".[{0}]", part);
                }
            }

            return builder.ToString();
        }
    }
}
