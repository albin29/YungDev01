using Microsoft.VisualBasic;
using System;
using System.Net;
using System.Text;
using YungDev01;

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

    switch (request.HttpMethod, request.Url?.AbsolutePath)
    {
        case ("GET", "/"):
            Get getter = new Get(response);
            getter.GetMethod();
            break;
        case ("POST", "/"):
            RootPost(request, response);
            break;
        default:
            //NotFound(response);
            break;

            
    }
}


void RootPost(HttpListenerRequest req, HttpListenerResponse res)
{
    StreamReader reader = new(req.InputStream, req.ContentEncoding);
    string body = reader.ReadToEnd();

    Console.WriteLine($"Created the following in db: {body}");

    res.StatusCode = (int)HttpStatusCode.Created;
    res.Close();
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