using Microsoft.VisualBasic;
using System;
using System.Net;

ControlC();

int port = 3000;
bool listen = true;

HttpListener listener = new();
listener.Prefixes.Add($"<host>:{port}/");

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




void HandleRequest(IAsyncResult result)
{
    if (result.AsyncState is HttpListener listener)
    {
        HttpListenerContext context = listener.EndGetContext(result);

        listener.BeginGetContext(new AsyncCallback(HandleRequest), listener);
    }
}

void ControlC()
{
    Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
    {
        Console.WriteLine("Interrupting cancel event");
        e.Cancel = true;
        listen = false;
    };
}