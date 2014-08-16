using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace GoEddieUk.SqlServerTddHelper.DirtyVSApi
{
    //I can't forthe life of me work out how to GetConfig a variable in the format ($var decoded so i will manually parse the ssdt proj file to GetConfig it - this must be simpler!!

    internal class SsdtVariableProvider
    {
        public List<SqlCmdVariable> GetVariables(string projectFile)
        {
            string xml = ReadFile(projectFile);
            var variables = new List<SqlCmdVariable>();

            var doc = new XmlDocument();
            doc.LoadXml(xml.Replace("xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\"", ""));

            foreach (XmlNode node in doc.SelectNodes(".//SqlCmdVariable"))
            {
                variables.Add(new SqlCmdVariable
                {
                    Name = string.Format("$({0})", node.Attributes["Include"].Value),
                    Value = node.SelectSingleNode(".//DefaultValue").InnerText
                });
            }

            return variables;
        }

        private string ReadFile(string path)
        {
            using (var sr = new StreamReader(path))
            {
                return sr.ReadToEnd();
            }
        }
    }

    public struct SqlCmdVariable
    {
        public string Name;
        public string Value;
    }
}
