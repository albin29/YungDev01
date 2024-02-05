using Npgsql;
using System.Net;
using System.Text;
using YungDev01;

string dbUri = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=YungDev";
await using var db = NpgsqlDataSource.Create(dbUri);

Table table = new Table(db);
await table.CreateTables();

bool listen = true;
int port = 3000;
HttpListener listener = new();
listener.Prefixes.Add($"http://localhost:{port}/");

ControlC();
Console.WriteLine("Server is up and running..");

try
{
    listener.Start();
    listener.BeginGetContext(new AsyncCallback(HandleRequest), listener);
    while (listen) { };

}
finally
{
    listener.Stop();
}

void ControlC() // Handle ctrl + c interup event, and gracefully shut down server
{
    Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
    {

        Console.WriteLine("Interupting cancel event");
        e.Cancel = true;
        listen = false;
    };
}
void HandleRequest(IAsyncResult result)
{
    if (result.AsyncState is HttpListener listener)
    {
        HttpListenerContext context = listener.EndGetContext(result);

        Router(context);

        // metod eller kod här som hanterar request och response från context

        listener.BeginGetContext(new AsyncCallback(HandleRequest), listener);
    }
}
void Router(HttpListenerContext context)
{
    HttpListenerRequest req = context.Request;
    HttpListenerResponse res = context.Response;
    Get get = new Get(res, req, db);
    Post post = new Post(db);

    switch (req.HttpMethod)
    {

        case ("GET"):

            byte[] buffer = Encoding.UTF8.GetBytes(get.GetMessage());
            res.ContentType = "text/plain";
            res.StatusCode = (int)HttpStatusCode.OK;

            res.OutputStream.Write(buffer, 0, buffer.Length);
            res.OutputStream.Close();
            break;
        case ("POST"):
            post.PostCommands(req, res);
            break;
        default:
            get.NotFound(res);
            break;
    }
}
