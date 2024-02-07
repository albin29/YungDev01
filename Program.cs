using Microsoft.VisualBasic;
using System;
using System.Net;
using System.Text;
using YungDev01;
using Npgsql;

string dbUri = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=YungDev";
await using var db = NpgsqlDataSource.Create(dbUri);

Table table = new Table(db);
await table.CreateTable();

ControlC();

int port = 3000;
bool listen = true;

HttpListener listener = new();
listener.Prefixes.Add($"http://localhost:{port}/");
Console.WriteLine("Server is running...");
try
{
    listener.Start();
    listener.BeginGetContext(new AsyncCallback(HandleRequest), listener);
    while (listen) { }
}
finally
{
    listener.Stop();
}



void Router(HttpListenerContext context)
{
    HttpListenerRequest request = context.Request;
    HttpListenerResponse response = context.Response;
    
    

    switch (request.HttpMethod)
    {
        case ("GET"):
            Get getter = new Get(response, request, db);
            getter.GetMethod();
            break;
        case ("POST"):
            Post poster = new Post(db);
            poster.Commands(request, response);
            break;
        default:
            //NotFound(response);
            break;   
    }
}




void HandleRequest(IAsyncResult result)
{
    if (result.AsyncState is HttpListener listener)
    {
        HttpListenerContext context = listener.EndGetContext(result);

        Router(context);

        listener.BeginGetContext(new AsyncCallback(HandleRequest), listener);
    }
}

/*
void NotFound(HttpListenerResponse res)
{
    res.StatusCode = (int)HttpStatusCode.NotFound;
    res.Close();
}
*/
void ControlC()
{
    Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
    {
        Console.WriteLine("Interrupting cancel event");
        e.Cancel = true;
        listen = false;
    };
}
