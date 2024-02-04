using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YungDev01;

public class Post(NpgsqlDataSource db)
{
    public void PostCommands(HttpListenerRequest req, HttpListenerResponse res)
    {
        var path = req.Url?.AbsolutePath;
        string? lastPath = req.Url?.AbsolutePath.Split("/").Last();

        if (path != null && path.Contains("/register"))
        {
            StreamReader reader = new(req.InputStream, req.ContentEncoding);
            string body = reader.ReadToEnd();

            CharacterRegister(body);

            Console.WriteLine($"Created the following in db: {body}");

            res.StatusCode = (int)HttpStatusCode.Created;
            res.Close();
        }
        if (path != null && path.Contains("/moveto/"))
        {



        }
    }
    public Character CharacterRegister(string body)
    {
        string[] parts = body.Split(",");
        string name = parts[0];
        int skills = Convert.ToInt32(parts[1]);
        int stamina = Convert.ToInt32(parts[2]);

        Character character = new Character(name, skills, stamina, null);

        //curl -X POST localhost:3000/register -d Danijel,12,5

        string qRegisterUser = @"
        insert into character (name, skills, stamina) Values
        (@name, @skills, @stamina);";
        var cmd = db.CreateCommand(qRegisterUser);
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("skills", skills);
        cmd.Parameters.AddWithValue("stamina", stamina);
        cmd.ExecuteNonQuery();

        return character;

    }
}
