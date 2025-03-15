namespace BenchmarkingDapperEFCoreCRMSqlServer;

public static class Configurations
{
    public static string BaseMaster => Environment.GetEnvironmentVariable("BaseMasterConnectionString")!;
    public static string BaseEFCore => Environment.GetEnvironmentVariable("BaseEFCoreConnectionString")!;
    public static string BaseDapper => Environment.GetEnvironmentVariable("BaseDapperConnectionString")!;
    public static string BaseDapperContrib => Environment.GetEnvironmentVariable("BaseDapperContribConnectionString")!;
    public static string BaseADO => Environment.GetEnvironmentVariable("BaseADOConnectionString")!;

    public static void Load(string connectionStringBaseMaster)
    {
        Environment.SetEnvironmentVariable("BaseMasterConnectionString", connectionStringBaseMaster);
        Environment.SetEnvironmentVariable("BaseEFCoreConnectionString",
            connectionStringBaseMaster.Replace(";Database=master;", ";Database=BaseCRMEF;"));
        Environment.SetEnvironmentVariable("BaseDapperConnectionString",
            connectionStringBaseMaster.Replace(";Database=master;", ";Database=BaseCRMDapper;"));
        Environment.SetEnvironmentVariable("BaseDapperContribConnectionString",
            connectionStringBaseMaster.Replace(";Database=master;", ";Database=BaseCRMDapperContrib;"));
        Environment.SetEnvironmentVariable("BaseADOConnectionString",
            connectionStringBaseMaster.Replace(";Database=master;", ";Database=BaseCRMADO;"));
    }
}