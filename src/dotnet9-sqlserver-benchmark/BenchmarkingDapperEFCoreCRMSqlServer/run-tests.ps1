$env:NumeroContatosPorCompanhia = "2"
$env:ExecucaoManual = "true"
dotnet run --filter BenchmarkingDapperEFCoreCRMSqlServer.Tests.* -c Release