// Guids.cs
// MUST match guids.h
using System;

namespace GoEddieUk.SqlServerTddHelper
{
    static class GuidList
    {
        public const string guidSqlServerTddHelperPkgString = "9f7c9d51-7c17-4f58-bd49-8ee02b5e91c1";
        public const string guidSqlServerTddHelperCmdSetString = "7daef23e-ad50-4e0f-b2c9-7d171d3ed0f5";

        public static readonly Guid guidSqlServerTddHelperCmdSet = new Guid(guidSqlServerTddHelperCmdSetString);


        public const string guidToolsOptionsCmdSetString = "7daef23e-ad50-4e0f-b2c9-7d171d3ed0f6";
        public static readonly Guid guidToolsOptionsCmdSet = new Guid(guidToolsOptionsCmdSetString);

        public const string guidGenScriptCmdSetString = "7daef23e-ad50-4e0f-b2c9-7d171d3ed0f7";
        public static readonly Guid guidGenScriptCmdSet = new Guid(guidGenScriptCmdSetString); 
    };
}