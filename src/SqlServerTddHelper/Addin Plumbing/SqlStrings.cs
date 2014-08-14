namespace GoEddieUk.SqlServerTddHelper
{
    internal static class SqlStrings
    {
        public static string DropExistingProc =
            "\r\nif exists(select * from sys.procedures where name = '{0}')\r\nbegin\r\n\tdrop proc {1}\r\n\r\nend\r\n";

        public static string Script =
            @"

{0}

";

    }
}