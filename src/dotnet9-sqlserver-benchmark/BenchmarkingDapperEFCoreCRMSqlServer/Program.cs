using BenchmarkDotNet.Running;
using BenchmarkingDapperEFCoreCRMSqlServer;
using BenchmarkingDapperEFCoreCRMSqlServer.Tests;
using BenchmarkingDapperEFCoreCRMSqlServer.Utils;
using DbUp;
using System.Reflection;
using Testcontainers.MsSql;


CommandLineHelper.Execute("docker images",
    "Imagens antes da execucao do Testcontainers...");
CommandLineHelper.Execute("docker container ls",
    "Containers antes da execucao do Testcontainers...");

Console.WriteLine("Criando container para uso do SQL Server 2022...");
var msSqlContainer = new MsSqlBuilder()
  .WithImage("mcr.microsoft.com/mssql/server:2022-CU17-ubuntu-22.04")
  .Build();
await msSqlContainer.StartAsync();
// Lembrando que o SQL Server ja tem uma estrategia de Wait embutida:
// https://github.com/testcontainers/testcontainers-dotnet/discussions/1167#discussioncomment-9270050

CommandLineHelper.Execute("docker images",
    "Imagens apos execucao do Testcontainers...");
CommandLineHelper.Execute("docker container ls",
    "Containers apos execucao do Testcontainers...");

var connectionString = msSqlContainer.GetConnectionString();
Console.WriteLine($"Connection String da base de dados master: {connectionString}");
Configurations.Load(connectionString);

Console.WriteLine("Executando Migrations com DbUp...");

var upgrader = DeployChanges.To.SqlDatabase(Configurations.BaseMaster)
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogToConsole()
    .Build();
var result = upgrader.PerformUpgrade();

if (result.Successful)
{
    Console.WriteLine("Migrations do DbUp executadas com sucesso!");
    new BenchmarkSwitcher([ typeof(CRMTests) ]).Run(args);
}
else
{
    Environment.ExitCode = 3;
    Console.WriteLine($"Falha na execucao das Migrations do DbUp: {result.Error.Message}");
}

string[] databases = ["BaseCRMEF", "BaseCRMDapper", "BaseCRMDapperContrib", "BaseCRMADO"];
foreach (string database in databases)
{
    Console.WriteLine();
    Console.WriteLine($"# Amostragem de registros criados na base {database}");

    Console.WriteLine();
    Console.WriteLine("*** Empresas ***");
    var resultSelectEmpresas = await msSqlContainer.ExecScriptAsync(
        $"USE {database}; SELECT TOP 10 * FROM dbo.Empresas;");
    Console.WriteLine(resultSelectEmpresas.Stdout);

    Console.WriteLine();
    Console.WriteLine("*** Contatos ***");
    var resultSelect = await msSqlContainer.ExecScriptAsync(
        $"USE {database}; SELECT TOP 30 * FROM dbo.Contatos;");
    Console.WriteLine(resultSelect.Stdout);
}

if (Environment.GetEnvironmentVariable("ExecucaoManual") == "true")
{

    Console.WriteLine();
    Console.WriteLine($"Connection String da base de dados master: {connectionString}");

    Console.WriteLine();
    Console.WriteLine("Pressione ENTER para interromper a execucao do container...");
    Console.ReadLine();

    await msSqlContainer.StopAsync();
    Console.WriteLine("Pressione ENTER para encerrar a aplicacao...");
    Console.ReadLine();
}

CommandLineHelper.Execute("docker ps -a",
    "Containers ao finalizar a aplicacao...");

return Environment.ExitCode;