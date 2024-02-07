using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace YungDev01;

public class Post(NpgsqlDataSource db, HttpListenerRequest req, HttpListenerResponse res)
{
    public void CharacterRegister(string body)
    {
        int stamina = 5;
        int skills = 0;
        int money = 100;
        int day = 1;
        int location_id = 1;
        string[] fields = body.Split(',');
        string name = fields[0];
        string password = fields[1];

        string qRegisterUser = @"
        insert into players (name,password,stamina,skills,money,day,location_id) Values
        (@name,@password,@stamina,@skills,@money,@day,@location_id);";

        var cmd = db.CreateCommand(qRegisterUser);
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("password", password);
        cmd.Parameters.AddWithValue("stamina", stamina);
        cmd.Parameters.AddWithValue("skills", skills);
        cmd.Parameters.AddWithValue("money", money);
        cmd.Parameters.AddWithValue("day", day);
        cmd.Parameters.AddWithValue("location_id", location_id);
        cmd.ExecuteNonQuery();
    }

    public void Commands(string body)
    {
        string? path = req.Url?.AbsolutePath;
        string? lastPath = req.Url?.AbsolutePath.Split("/").Last();
        
        if (path != null && path.Contains("register"))
        {

            CharacterRegister(body);
            Console.WriteLine($"Registered the following {body}");
            
        }
        if (path != null && path.Contains("moveto"))
        {
            //add logic

            Console.WriteLine($"Registered the following {body}");
        }
    }
}
