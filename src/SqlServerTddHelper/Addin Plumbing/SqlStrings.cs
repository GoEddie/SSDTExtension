namespace GoEddieUk.SqlServerTddHelper
{
    internal static class SqlStrings
    {
        public static string DropExistingProc =
            @"
if exists(select * from sys.procedures where name = '{0}')
begin
    drop proc {1}
end 

";

        public static string Script =
            @"

{0}

";

    }
}