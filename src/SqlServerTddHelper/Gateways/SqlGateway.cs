using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoEddieUk.SqlServerTddHelper.Gateways
{
    class SqlGateway
    {
        private readonly string _connectionString;

        public SqlGateway(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Execute(string command)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = command;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
