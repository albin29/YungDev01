using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Npgsql;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;


namespace YungDev01;

public class Get(HttpListenerRequest req, NpgsqlDataSource db)
{
    public string GetCommand()
    {
        var path = req.Url?.AbsolutePath;
        var lastPath = req.Url?.AbsolutePath.Split("/").Last();
        string result = string.Empty;

        if (path != null && path.Contains("/users"))
        {
            NpgsqlCommand? command;
            if (path.Contains("users/"))
            {
                string qCharacter = @"
                SELECT id, name, password, stamina, skills, money, day, location_id
                FROM players
                WHERE id = @user_id;
                ";
                command = db.CreateCommand(qCharacter);
                command.Parameters.AddWithValue("user_id", Convert.ToInt32(lastPath));
            }
            else
            {
                string qAllCharacters = @"
                SELECT id, name, password, stamina, skills, money, day, location_id
                FROM players
                ORDER BY id ASC;
                ";
                command = db.CreateCommand(qAllCharacters);
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
        }
        return result;

    }

}
