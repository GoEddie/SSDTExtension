using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoEddieUk.SqlServerTddHelper.Config
{
    struct ConfigSettings
    {
        public string ConnectionString;        
    }

    class Configuration
    {
        public string GetConnectionString()
        {
            return "";
        }

        public void ShowConfig()
        {


        }

        private ConfigSettings GetConfig()
        {
            throw new NotImplementedException();
        }
    }
}
