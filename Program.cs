using Npgsql;
using YungDev01;

string dbUri = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=YungDev";

await using var _db = NpgsqlDataSource.Create(dbUri);

Table table = new Table(_db);
table.CreateTables();