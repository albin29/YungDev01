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
        string? path = req.Url?.AbsolutePath;
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
            StreamReader reader = new(req.InputStream, req.ContentEncoding);
            string body = reader.ReadToEnd();

            CharacterRegister(body);

            Console.WriteLine($"Created the following in db: {body}");

            res.StatusCode = (int)HttpStatusCode.Created;
            res.Close();   
        }

    }
    public void CharacterRegister(string body)
    {
        string name = body;
        int location = 1;
        int skills = 0;
        int stamina = 5;

        //curl -X POST localhost:3000/register -d Danijel,12,5

        string qRegisterUser = @"
        insert into character (name, skills, stamina,location) Values
        (@name, @skills, @stamina, @location);";
        var cmd = db.CreateCommand(qRegisterUser);
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("skills", skills);
        cmd.Parameters.AddWithValue("stamina", stamina);
        cmd.Parameters.AddWithValue("location", location);


        cmd.ExecuteNonQuery();
    }
    public void MoveTo(string body)
    {
        string[] parts = body.Split(",");
        string name = parts[0];
        int skills = Convert.ToInt32(parts[1]);
        int stamina = Convert.ToInt32(parts[2]);

        //curl -X POST localhost:3000/register -d Danijel,12,5

        string qRegisterUser = @"
        insert into character (name, skills, stamina) Values
        (@name, @skills, @stamina);";
        var cmd = db.CreateCommand(qRegisterUser);
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("skills", skills);
        cmd.Parameters.AddWithValue("stamina", stamina);
        cmd.ExecuteNonQuery();


    }

}
