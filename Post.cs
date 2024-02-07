using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace YungDev01;

public class Post(NpgsqlDataSource db, HttpListenerRequest req, HttpListenerResponse res)
{
    // add comands here that will post something to the database


    public void Commands(string body)
    {
        string? path = req.Url?.AbsolutePath;
        string? lastPath = req.Url?.AbsolutePath.Split("/").Last();
        
        if (path != null && path.Contains("register"))
        {
            //add logic

            Console.WriteLine($"Registered the following {body}");
            
        }
        if (path != null && path.Contains("moveto"))
        {
            //add logic

            Console.WriteLine($"Registered the following {body}");
        }
    }
}
