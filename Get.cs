using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Npgsql;
using System.Reflection.Metadata.Ecma335;


namespace YungDev01;

public class Get (HttpListenerResponse res, HttpListenerRequest req, NpgsqlDataSource db)
{

    public string GetCommand()
    {
        var path = req.Url?.AbsolutePath;
        var lastPath = req.Url?.AbsolutePath.Split("/").Last();
        string result = string.Empty;

        if (path != null && path == "/users")
        {
            //add logic
        }
        return result;
    }
    public void GetMethod()
    {
        string message = @"
                        Get command list: 
                        "; //add list for commands for the player to use
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        res.ContentType = "text/plain";
        res.StatusCode = (int)HttpStatusCode.OK;

        res.OutputStream.Write(buffer, 0, buffer.Length);
        res.OutputStream.Close();
    }
}
