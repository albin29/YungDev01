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

    private string GetMenu()
    {
        return @"
----------------------------------------------
Welcome to YungDev: The Game Tutorial
----------------------------------------------

Welcome, Player, to the exciting world of YungDev! 

In this game, you will embark on a thrilling adventure, facing challenges, making decisions, and shaping your destiny.
Let's get started with a quick tutorial on how to play:

1. **Registering as a Player:**
To begin your journey, you must first register as a player. Use the following command to register:
curl -X POST localhost:3000/register -d YourName,YourPassword
----------------------------------------------
2. **Exploring Player Information::**
----------------------------------------------
Once registered, you can explore your player information, including stats and progress.
Use the following command to view player information:

curl -X GET localhost:3000/players/YourPlayerID
----------------------------------------------
3. **Resting and Advancing Time:::**
----------------------------------------------
In YungDev, time passes as you rest. Use the following command to rest and advance to the next day:

curl -X POST localhost:3000/sleep -d YourPlayerID
----------------------------------------------
4. **Moving to Different Locations::::**
----------------------------------------------
Explore various locations in the game world by moving to different areas. Use the following command to move:

curl -X POST localhost:3000/moveto -d YourPlayerID,LocationID

----------------------------------------------
5. **placeholder::::**
----------------------------------------------
";

    }

    public string GetCommand()
    {
        string result = string.Empty;

        if (path != null && path.Contains("/players"))
        {
            return ShowPlayer(result);
        }

        else if (path != null && path.Contains("/menu"))
        {
            return GetMenu();
        }
        return result;

    }

    public string ShowPlayer(string result)
    {
        NpgsqlCommand? command;
        if (path != string.Empty && lastPath!= string.Empty && path.Contains("players/"))
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
