using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TSQLHelper;

namespace SqlServerTddHelper_UnitTests
{
    [TestFixture]
    public class DeploymentScriptGeneratorTests
    {

        [Test]
        public void Generate_drop_from_create_proc()
        {
            var result =
                DeploymentScriptGenerator.BuildDeploy(
                    "create procedure test as select 1 from test");

            Assert.AreEqual(@"if exists (select * from sys.procedures where object_id = object_id('test'))
	drop procedure [test];
GO
create procedure test as select 1 from test", result);

        }

        [Test]
        public void Generate_create_from_create_schema()
        {
            var result =
                DeploymentScriptGenerator.BuildDeploy(
                    "create schema [bloo blah blum]");

            Assert.AreEqual("if not exists (select * from sys.schemas where name = 'bloo blah blum')\r\nbegin\r\n\texec sp_executesql N'create schema [bloo blah blum]'\r\nend\r\n", result);

        }

        [Test]
        public void throws_exception_on_non_create_proc_scripts()
        {
            Assert.Throws<TSqlDeploymentException>( () => DeploymentScriptGenerator.BuildDeploy("create table test (a int, b int)"));
        }


        [Test]
        public void escape_identifier_escapes_name()
        {
            const string identifier = "table name";
            const string expected = "[table name]";

            var actual = DeploymentScriptGenerator.EscapeIdentifier(identifier);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void escape_identifier_escapes_name_and_schema()
        {

            const string identifier = "schema.table name";
            const string expected = "[schema].[table name]";

            var actual = DeploymentScriptGenerator.EscapeIdentifier(identifier);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void escape_identifier_escapes_name_and_schema_and_db()
        {

            const string identifier = "database name.schema.table name";
            const string expected = "[database name].[schema].[table name]";

            var actual = DeploymentScriptGenerator.EscapeIdentifier(identifier);

            Assert.AreEqual(expected, actual);
        }


    }
}
