using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Npgsql;


namespace YungDev01;

public class Get (HttpListenerResponse res, HttpListenerRequest req, NpgsqlDataSource db)
{
    
    public void GetMethod()
    {
        string message = @"
                        connection successful
                        ";
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        res.ContentType = "text/plain";
        res.StatusCode = (int)HttpStatusCode.OK;

        res.OutputStream.Write(buffer, 0, buffer.Length);
        res.OutputStream.Close();
    }
}
