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
            return result;
        }
        if (path != null && path == "scoreboard")
        {
            string qScoreBoard = @"
            select id, player_name, points 
            from highscore order by points desc;";

            using var command = db.CreateCommand(qScoreBoard);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                result += "[Id: ";
                result += reader.GetInt32(0);
                result += "] ";
                result += "[Name: ";
                result += reader.GetString(1);
                result += "] ";
                result += "[Points: ";
                result += reader.GetInt32(2);
                result += "]";
                result += "\n";
            }
            return result;

        }
        return result;
    }

}
