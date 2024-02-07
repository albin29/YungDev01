using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace YungDev01;

public class Post(NpgsqlDataSource db)
{
    // add comands here that will post something to the database


    public void Commands(HttpListenerRequest req, HttpListenerResponse res)
    {
        string? path = req.Url?.AbsolutePath;
        string? lastPath = req.Url?.AbsolutePath.Split("/").Last();

        if (path != null && path.Contains("register"))
        {
            //add logic
            StreamReader reader = new(req.InputStream, req.ContentEncoding);
            string body = reader.ReadToEnd();
            Console.WriteLine($"Registered the following {body}");
        }
        if (path != null && path.Contains("moveto"))
        {
            //add logic
            StreamReader reader = new(req.InputStream, req.ContentEncoding);
            string body = reader.ReadToEnd();
            Console.WriteLine($"Registered the following {body}");
        }
    }
}
