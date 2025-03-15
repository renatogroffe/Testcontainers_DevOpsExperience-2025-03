using Testcontainers.MsSql;

var msSqlContainer = new MsSqlBuilder()
  .WithImage("mcr.microsoft.com/mssql/server:2022-CU10-ubuntu-22.04")
  .Build();
await msSqlContainer.StartAsync();

var connectionString = msSqlContainer.GetConnectionString();
Console.WriteLine($"Connection String: {connectionString}");

Console.WriteLine("Pressione ENTER para encerrar a aplicacao...");
Console.ReadLine();