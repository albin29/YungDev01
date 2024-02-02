using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YungDev01;

public class Server
{
    private bool _listen = true;
    private int _port = 3000;
    public Server()
    {

        HttpListener listener = new();
        listener.Prefixes.Add($"http://localhost:{_port}/");

        ControlC();

        try
        {
            listener.Start();
            listener.BeginGetContext(new AsyncCallback(HandleRequest), listener);
            while (_listen) { };

        }
        finally
        {
            listener.Stop();
        }
    }

    private void ControlC() // Handle ctrl + c interup event, and gracefully shut down server
    {
        Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
        {

            Console.WriteLine("Interupting cancel event");
            e.Cancel = true;
            _listen = false;
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
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;

        var path = request.Url?.AbsolutePath;
        string? lastPath = request.Url?.AbsolutePath.Split("/").Last();

        /*if (request.HttpMethod == "GET")
        {
            
            if (path != null && path.Contains("/users/"))
            {
                string result = string.Empty;

                Console.WriteLine(lastPath);
                string qUser =
                    $@"SELECT name, stamina, day, money
                       FROM users
                       WHERE users.id = @userId;";
                using var command = db.CreateCommand(qUser);
                command.Parameters.AddWithValue("userId", Convert.ToInt32(lastPath));


                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result += "[";
                    result += reader.GetString(0);
                    result += "] Stamina: ";
                    result += reader.GetInt32(1);
                    result += " | Day: ";
                    result += reader.GetInt32(2);
                    result += " | Money: ";
                    result += reader.GetInt32(3);

                }
                HelloGet(response, result);
            }
        }*/

        switch (request.HttpMethod, request.Url?.AbsolutePath)
        {
            case ("GET", "/users"):
                RootGet(response);
                break;
            case ("GET", "/users/"):
                break;
            case ("POST", "/"):
                RootPost(request, response);
                break;
            default:
                NotFound(response);
                break;
        }
    }

    void RootGet(HttpListenerResponse response)
    {
        string message = "You have arrived to inital get"; // byt ut till vilken text som ska skickas tillbaka
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        response.ContentType = "text/plain";
        response.StatusCode = (int)HttpStatusCode.OK;

        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }
    void HelloGet(HttpListenerResponse response, string message)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        response.ContentType = "text/plain";
        response.StatusCode = (int)HttpStatusCode.OK;

        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }
    void RootPost(HttpListenerRequest req, HttpListenerResponse res)
    {
        StreamReader reader = new(req.InputStream, req.ContentEncoding);
        string body = reader.ReadToEnd();

        // metod här för att hantera request body 
        Console.WriteLine($"Created the following in db: {body}");

        res.StatusCode = (int)HttpStatusCode.Created;
        res.Close();
    }
    void NotFound(HttpListenerResponse response)
    {
        string message = "Your input has not been recognized";
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        response.ContentType = "text/plain";
        response.StatusCode = (int)HttpStatusCode.OK;

        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }




}
