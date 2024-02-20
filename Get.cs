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
        string menu = @"
        ----------------------------------------------                                                              ----------------------------------------------
        Welcome to YungDev: The Game Tutorial                                                                       You can buy items to advance your skills or stamina                                                                       
        ----------------------------------------------                                                              ----------------------------------------------
                                                                                                                    To see the store
        Welcome, Player, to the exciting world of YungDev!";                                                        menu += "\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\u001b[91;1m    curl -X GET localhost:3000/shop \u001b[0m\n";

        menu += @"                                                                                                                    ----------------------------------------------
        In this game, you will embark on a thrilling adventure, facing challenges,                                  To buy something
        making decisions, and shaping your destiny.";                                                               menu += "\n\t\t\t\t\t\t\t\t\t\t\t\t\t\u001b[91;1m            curl -X POST localhost:3000/shop -d playerid,itemid \u001b[0m\n";
        menu += @"
        Let's get started with a quick tutorial on how to play:                                                     ----------------------------------------------
                                                                                                                    You can hack other players to steal their stats
                                                                                                                    ----------------------------------------------
        Registering as a Player:                                                                                    If you manage to hack the legend Arvid,
        To begin your journey, you must first register as a player.                                                 you will be awarded with bonus stats
        Use the following command to register:                                                                      ----------------------------------------------
        ";                                                                                                          menu += "\t\t\t\t\t\t\t\t\t\t\t\t\t\u001b[91;1m    curl -x POST localhost:3000/hack -d yourid,victimid \u001b[0m\n";

                                                                                                                    menu +="\t\t\t\t\t\t\t\t\t\t\t\t\t\t    ----------------------------------------------\n";
        menu += "\u001b[91;1m        curl -X POST localhost:3000/register -d YourName,YourPassword \u001b[0m\n";
                menu += @"
        ----------------------------------------------                                                              Studying at different locations 
        Exploring Player Information:                                                                               ----------------------------------------------
        Once registered, you can explore your player information,                                                   Explore various locations in the game world by moving to different areas
        including stats and progress.                                                                               To show studyspots
        Use the following command to view player information:";                                                     menu += "\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\u001b[91;1m    curl -X GET localhost:3000/studyspots \u001b[0m\n\n";
                                                                                                                    menu += "\t\t\t\t\t\t\t\t\t\t\t\t\t\t    Use the following command to study:\n";
        menu += "\t\u001b[91;1mcurl -X GET localhost:3000/players/YourPlayerID \u001b[0m\n";                        menu += "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\u001b[91;1m    curl -X POST localhost:3000/study -d YourPlayerID,LocationID \u001b[0m\n";

        menu += "\tOR\n\n";
                menu += "\t\u001b[91;1mcurl -X GET localhost:3000/players/ \u001b[0m\n";
        menu += @"
        ----------------------------------------------
        Resting and Advancing Time:
        In YungDev, time passes as you rest. 
        Use the following command to rest and advance to the next day:
        ";

                menu += "\u001b[91;1mcurl -X POST localhost:3000/sleep -d YourPlayerID \u001b[0m\n";
                menu += "\t----------------------------------------------";
                
        menu += @"
        
        ";
        return menu;
    }
    public string GetCommand()
    {
        if (path != null)
        {
            if (path.Contains("/players"))
            {
                return ShowPlayer();
            }
            if (path.Contains("/shop"))
            {
                return Shop();
            }
            if (path.Contains("/menu"))
            {
                return GetMenu();
            }
            if (path.Contains("/scoreboard"))
            {
                return Scoreboard();
            }
            if (path.Contains("/jobs"))
            {
                return ShowWork();
            }
            if (path.Contains("/studyspots"))
            {
                return ShowStudy();
            }
        }
        return "Not Found";
    }

    public string Scoreboard()
    {
        string result = "\u001b[91;1m****** SCOREBOARD ******\u001b[0m\n";

        string qCharacter = @"
    SELECT player_name, points 
    FROM highscore
    ORDER BY points DESC;";


        using var command = db.CreateCommand(qCharacter);
        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            result += reader.GetString(0).PadRight(9);
            result += " | Score: ";
            result += reader.GetInt32(1).ToString().PadLeft(5);
            result += "\n";

        }
        return result;

    }

    public string ShowWork()
    {
        string qJobs = @"
            SELECT id, name, skill_req, stamina_req, money_gain, skill_gain
            FROM workplace
            ";
        using var command = db.CreateCommand(qJobs);
        var reader = command.ExecuteReader();

        string result = string.Empty;
        while (reader.Read())
        {
            result += "ID[";
            result += reader.GetInt32(0).ToString();
            result += "] ";
            result += reader.GetString(1).PadRight(20);
            result += " | Skill Requirement: ";
            result += reader.GetInt32(2).ToString().PadRight(5);
            result += " | Stamina Requirement: ";
            result += reader.GetInt32(3).ToString().PadRight(5);
            result += " | Money Gain: ";
            result += reader.GetInt32(4).ToString().PadRight(5);
            result += " | Skill Gain: ";
            result += reader.GetInt32(5).ToString().PadRight(5);
            result += "\n";




        }
        return result;
    }
    public string Shop()
    {
        string qShop = @"
            SELECT id, name, skills_given, stamina_given, price
            FROM shop
            ";
        using var command = db.CreateCommand(qShop);
        var reader = command.ExecuteReader();

        string result = string.Empty;
        while (reader.Read())
        {
            result += "ID[";
            result += reader.GetInt32(0).ToString(); // Adjust padding width as needed
            result += "] ";
            result += reader.GetString(1).PadRight(35); // Adjust padding width as needed
            result += " | Skills Given: ";
            result += reader.GetInt32(2).ToString().PadRight(8); // Adjust padding width as needed
            result += " | Stamina Given: ";
            result += reader.GetInt32(3).ToString().PadRight(8); // Adjust padding width as needed
            result += " | Price: ";
            result += reader.GetInt32(4).ToString().PadRight(8); // Adjust padding width as needed
            result += "\n";

        }
        return result;
    }
    public string ShowPlayer()
    {
        NpgsqlCommand? command;

        if (path != null && lastPath != string.Empty && path.Contains("players/"))
        {
            string qPlayer = @"
                SELECT id, name, password, stamina, skills, money, day
                FROM players
                WHERE id = @player_id;
                ";
            command = db.CreateCommand(qPlayer);
            command.Parameters.AddWithValue("player_id", Convert.ToInt32(lastPath));
        }
        else
        {
            string qAllPlayers = @"
                SELECT id, name, password, stamina, skills, money, day
                FROM players
                ORDER BY id ASC;
                ";
            command = db.CreateCommand(qAllPlayers);
        }
        var reader = command.ExecuteReader();

        string result = string.Empty;
        while (reader.Read())
        {
            result += "ID[";
            result += reader.GetInt32(0).ToString();
            result += "] ";
            result += reader.GetString(1).PadRight(10);
            result += " | Skills: ";
            result += reader.GetInt32(4).ToString().PadRight(8);
            result += " | Stamina: ";
            result += reader.GetInt32(3).ToString().PadRight(8);
            result += " | Money: ";
            result += reader.GetInt32(5).ToString().PadRight(8);
            result += " | Day: ";
            result += reader.GetInt32(6).ToString().PadRight(8);
            result += "\n";

        }
        return result;
    }

    public string ShowStudy()
    {
        string qStudy = @"
            SELECT id, name, stamina_cost, skill_award
            FROM study_spot
            ";
        using var command = db.CreateCommand(qStudy);
        var reader = command.ExecuteReader();

        string result = string.Empty;
        while (reader.Read())
        {
              result += "ID[";
        result += reader.GetInt32(0).ToString();
        result += "] ";
        result += reader.GetString(1).PadRight(21);
        result += " | Stamina Requirement: ";
        result += reader.GetInt32(2).ToString().PadRight(5);
        result += " | Skill Gain: ";
        result += reader.GetInt32(3).ToString().PadRight(5);
        result += "\n";
        }
        return result;
    }
}
