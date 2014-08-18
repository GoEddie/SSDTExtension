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
                DeploymentScriptGenerator.BuildDropStatment(
                    "create procedure test as select 1 from test");

            Assert.AreEqual("if exists (select * from sys.procedures where name = 'test')\r\n\tdrop procedure test\r\n", result);

        }

        [Test]
        public void throws_exception_on_non_create_proc_scripts()
        {
            Assert.Throws<TSqlDeploymentException>( () => DeploymentScriptGenerator.BuildDropStatment("create table test (a int, b int)"));
        }

    }
}
