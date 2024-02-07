using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Npgsql;
using System.Reflection.Metadata.Ecma335;


namespace YungDev01;

public class Get(HttpListenerRequest req, NpgsqlDataSource db)
{

    public string GetCommand()
    {
        var path = req.Url?.AbsolutePath;
        var lastPath = req.Url?.AbsolutePath.Split("/").Last();
        string result = string.Empty;

        if (path != null && path == "users")
        {
            //add logic
        }
        if (path != null && path == "scoreboard")
        {


            string qScoreBoard = "";
        }
        return result;
    }

}
