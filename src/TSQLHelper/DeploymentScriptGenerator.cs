using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TSQLHelper
{
    public class DeploymentScriptGenerator
    {
        public static string BuildDeploy(string createStatement)
        {
            var scriptType = ScriptProperties.GetScriptDetail(createStatement);


            var name = CleanName(scriptType.Name);


            switch (scriptType.Type)
            {
                case TSqlTokenType.Proc:
                case TSqlTokenType.Procedure:

                    return string.Format("if exists (select * from sys.procedures where object_id = object_id('{0}'))\r\n\tdrop procedure {1};\r\nGO\r\n{2}", name, EscapeIdentifier(name), createStatement);

                case TSqlTokenType.Schema:
                    
                    return string.Format("if not exists (select * from sys.schemas where name = '{0}')\r\nbegin\r\n\texec sp_executesql N'{1}'\r\nend\r\n", name, createStatement);
                    
                default:
                    throw new TSqlDeploymentException("Only stored procedure creates are currently supported");
            }

            
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
