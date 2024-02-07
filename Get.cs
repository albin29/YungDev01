using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Npgsql;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using System.IO;


namespace YungDev01;

public class Get(HttpListenerRequest req, NpgsqlDataSource db)
{
    public string? path = req.Url?.AbsolutePath;
    public string? lastPath = req.Url?.AbsolutePath.Split("/").Last();
    public string GetCommand()
    {
        string result = string.Empty;

        if (path != null && path.Contains("/players"))
        {
            return ShowPlayer(result);
        }
        return result;

    }
    public string ShowPlayer(string result)
    {
        NpgsqlCommand? command;
        if (path != null && path.Contains("players/"))
        {
            string qPlayer = @"
                SELECT id, name, password, stamina, skills, money, day, location_id
                FROM players
                WHERE id = @player_id;
                ";
            command = db.CreateCommand(qPlayer);
            command.Parameters.AddWithValue("player_id", Convert.ToInt32(lastPath));
        }
        else
        {
            string qAllPlayers = @"
                SELECT id, name, password, stamina, skills, money, day, location_id
                FROM players
                ORDER BY id ASC;
                ";
            command = db.CreateCommand(qAllPlayers);
        }
        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            result += "[";
            result += reader.GetInt32(0);
            result += ". ";
            result += reader.GetString(1);
            result += "] Skills: ";
            result += reader.GetInt32(4);
            result += " | Stamina: ";
            result += reader.GetInt32(3);
            result += " | Money: ";
            result += reader.GetInt32(5);
            result += " | Day: ";
            result += reader.GetInt32(6);
            result += " | Location ID: ";
            result += reader.GetInt32(7);
            result += "\n";
        }
        return result;
    }
}
