using Npgsql;
using YungDev01;

string dbUri = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=YungDev";
await using var _db = NpgsqlDataSource.Create(dbUri);

Console.WriteLine("Hello");



Table table = new Table(_db);
await table.CreateTables();

Server server = new Server(_db);
