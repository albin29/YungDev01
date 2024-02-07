using Npgsql;
using YungDev01;
Console.WriteLine("Hello, world!");

string dbUri = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=YungDev";
await using var db = NpgsqlDataSource.Create(dbUri);

Table table = new Table(db);
await table.CreateTable();
