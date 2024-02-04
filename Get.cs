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

        if (path != null && path.Contains("/users"))
        {
            string result = string.Empty;

            string qCharacter = $@"
            SELECT name, skills, stamina
            FROM character;";

            using var command = db.CreateCommand(qCharacter);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                result += "[";
                result += reader.GetString(0);
                result += "] Skills: ";
                result += reader.GetInt32(1);
                result += " | Stamina: ";
                result += reader.GetInt32(2);
            }
            return result;
        }
        else
        {
            res.StatusCode = (int)HttpStatusCode.NotFound;
            res.Close();
        }
        return "";
    }
}
