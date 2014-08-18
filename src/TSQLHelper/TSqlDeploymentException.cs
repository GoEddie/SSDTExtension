using System;

namespace TSQLHelper
{
    public class TSqlDeploymentException : Exception
    {
        public TSqlDeploymentException(string message) : base(message)
        {
            
        }
    }
}