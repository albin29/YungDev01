using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace YungDev01;

public class Get(HttpListenerResponse res, HttpListenerRequest req, NpgsqlDataSource db)
{
    public string GetMessage()
    {
        var path = req.Url?.AbsolutePath;
        var lastPath = req.Url?.AbsolutePath.Split("/").Last();

        if (path != null && path == "/users")
        {
            string result = string.Empty;

            string qCharacter = $@"
            SELECT id, name, skills, stamina
            FROM character;";

            using var command = db.CreateCommand(qCharacter);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                result += "[";
                result += reader.GetInt32(0);
                result += ". ";
                result += reader.GetString(1);
                result += "] Skills: ";
                result += reader.GetInt32(2);
                result += " | Stamina: ";
                result += reader.GetInt32(3);
                result += "\n";
            }
            return result;
        }
        else if (path != null && path == "/locations")
        {
            string result = string.Empty;

            string qCharacter = $@"
            SELECT id, name, stamina_cost, skill_point_reward 
            FROM locations;";

            using var command = db.CreateCommand(qCharacter);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                result += "[";
                result += reader.GetInt32(0);
                result += ". ";
                result += reader.GetString(1);
                result += "] Skills: ";
                result += reader.GetInt32(2);
                result += " | Stamina: ";
                result += reader.GetInt32(3);
                result += "\n";
            }
            return result;
        }
        else
        {
            NotFound(res);
        }
        return "";
    }
    public void NotFound(HttpListenerResponse res)
    {
        res.StatusCode = (int)HttpStatusCode.NotFound;
        res.Close();
    }
}
