using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace YungDev01;

public class Post(NpgsqlDataSource db, HttpListenerRequest req, HttpListenerResponse res)
{
    public void Commands(string body)
    {
        string? path = req.Url?.AbsolutePath;
        string? lastPath = req.Url?.AbsolutePath.Split("/").Last();
        if (path != null)
        {
            if (path.Contains("register"))
            {
                PlayerRegister(body);
                Console.WriteLine($"Registered the following {body}");
            }
            if (path.Contains("sleep"))
            {
                Sleep(body);
                Console.WriteLine($"Registered the following {body}");
            }
            if (path.Contains("moveto"))
            {
                Console.WriteLine($"Registered the following {body}");
            }

            if (path.Contains("Hack"))
            {
                Hack(body, res);
            }
        }

    }

    public void Sleep(string body)
    {
        string qGetCurrentDay = @"
        SELECT day
        FROM players
        WHERE id = @player_id;
        ";

        using var command2 = db.CreateCommand(qGetCurrentDay);
        command2.Parameters.AddWithValue("player_id", Convert.ToInt32(body));
        var reader = command2.ExecuteReader();
        int stamina = 5, day = 0;

        while (reader.Read()) { day = reader.GetInt32(0); }

        string qUpdatePlayer = @"
        UPDATE players
        SET day = @day, stamina = @stamina
        WHERE id = @player_id;
        ";

        using var command = db.CreateCommand(qUpdatePlayer);
        command.Parameters.AddWithValue("player_id", Convert.ToInt32(body));
        command.Parameters.AddWithValue("day", day + 1);
        command.Parameters.AddWithValue("stamina", stamina);
        command.ExecuteNonQuery();
    }
    public void PlayerRegister(string body)
    {
        int stamina = 5, skills = 0, money = 100, day = 1, location_id = 1;
        string[] fields = body.Split(',');
        string name = fields[0], password = fields[1];
        string qRegisterPlayer = @"
        insert into players (name,password,stamina,skills,money,day,location_id) Values
        (@name,@password,@stamina,@skills,@money,@day,@location_id);";

        var cmd = db.CreateCommand(qRegisterPlayer);
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("password", password);
        cmd.Parameters.AddWithValue("stamina", stamina);
        cmd.Parameters.AddWithValue("skills", skills);
        cmd.Parameters.AddWithValue("money", money);
        cmd.Parameters.AddWithValue("day", day);
        cmd.Parameters.AddWithValue("location_id", location_id);
        cmd.ExecuteNonQuery();
    }

    private void Hack(string body, HttpListenerResponse res)
    {
        var parts = body.Split(',');

        if (parts.Length < 2 || !int.TryParse(parts[0].Trim(), out int hackerId) ||
            !int.TryParse(parts[1].Trim(), out int targetId))
        {
            ErrorResponse(res, "Enter only integers starting from 1");
            return;
        }

        if (!PlayerCheck(hackerId) || !PlayerCheck(targetId))
        {
            ErrorResponse(res, "enter a number from 1 and up , try a lower number if the target number dont exist");
            return;
        }
        
        HackResult(hackerId, res);

        string output = $"player {hackerId} succesfully hacked player {targetId}";
        ClientResponse(res, output);


    }

    private bool PlayerCheck(int playerid)
    {

        string qPlayercheck = @"SELECT COUNT(*) FROM players WHERE id = @playerId";

        using var cmd = db.CreateCommand(qPlayercheck);
        cmd.Parameters.AddWithValue("@playerId", playerid);
        var result = cmd.ExecuteScalar();
        return Convert.ToInt32(result) > 0;

    }

    private void HackResult(int playerid, HttpListenerResponse res)
    {
        Random rnd = new Random();
        int randomskill = rnd.Next(3, 11);
        int randommoney = rnd.Next(0, 101);
        int staminacost = 1;

        string qHackresult = @"
        UPDATE players SET
        skills = skills + @randomskill,
        money = money + @randommoney,
        stamina = stamina - @staminacost
        WHERE id = @playerid AND stamina >= @staminacost
        ";

        using (var cmd = db.CreateCommand(qHackresult))
        {
            cmd.Parameters.AddWithValue("randomskill", randomskill);
            cmd.Parameters.AddWithValue("randommoney", randommoney);
            cmd.Parameters.AddWithValue("staminacost", staminacost);
            cmd.Parameters.AddWithValue("playerid", playerid);

            int rowchanged = cmd.ExecuteNonQuery();

            if (rowchanged == 0)
            {
                ErrorResponse(res, "To low on stamina to execute hack! Go sleep, See you another day!");
            }

        }
    }

    private void ErrorResponse(HttpListenerResponse res, string errorMessage)
    {
        res.StatusCode = 400; // Bad Request
        byte[] buffer = Encoding.UTF8.GetBytes(errorMessage);
        res.OutputStream.Write(buffer, 0, buffer.Length);
        res.Close();
    }

    private void ClientResponse(HttpListenerResponse res, string successMessage)
    {
        res.StatusCode = 200; // OK
        byte[] buffer = Encoding.UTF8.GetBytes(successMessage);
        res.OutputStream.Write(buffer, 0, buffer.Length);
        res.Close();
    }

    private void GoToSchool(int playerId, HttpListenerResponse res)
    {
        Random rnd = new Random();
        int staminacost = 2;
        int skillreward = rnd.Next(1, 11);


    }
    
}
