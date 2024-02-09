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

                RandomEventGenerator randomEvent = new(db, body);
                Random random = new();
                int result = random.Next(1, 3);

                if (result == 1)
                {
                    ClientResponse(res, randomEvent.Event());
                }
                else
                {
                    ClientResponse(res, "No event was triggered..\n");
                }
            }
            if (path.Contains("study"))
            {
                Console.WriteLine($"Registered the following {body}");
                Study(body);
            }
            if (path.Contains("shop"))
            {
                Console.WriteLine($"Registered the following {body}");
                Shop(body);
            }

        }
    }
    public void Shop(string body)
    {
        string qCheckValues = @"
        SELECT players.money, shop.price, players.stamina, shop.stamina_given, players.skills, shop.skills_given, shop.name, players.name
        FROM players
        CROSS JOIN shop
        WHERE shop.id = @shop_id AND players.id = @player_id;";

        string[] fields = body.Split(",");
        int playerId = Convert.ToInt32(fields[0]), shopId = Convert.ToInt32(fields[1]);

        using var command = db.CreateCommand(qCheckValues);
        command.Parameters.AddWithValue("player_id", playerId);
        command.Parameters.AddWithValue("shop_id", shopId);

        int currentStamina = 0, givenStamina = 0, currentCash = 0, priceCash = 0, currentSkills = 0, givenSkills = 0;
        string product = string.Empty, playerName = string.Empty;

        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            currentCash = reader.GetInt32(0);
            priceCash = reader.GetInt32(1);
            currentStamina = reader.GetInt32(2);
            givenStamina = reader.GetInt32(3);
            currentSkills = reader.GetInt32(4);
            givenSkills = reader.GetInt32(5);
            product = reader.GetString(6);
            playerName = reader.GetString(7);
        }
        if (currentCash >= priceCash)
        {
            int newCash = currentCash - priceCash;
            int newStamina = currentStamina + givenStamina;
            int newSkills = currentSkills + givenSkills;

            string qNewValues = @"
            UPDATE players
            set stamina = @new_stamina, skills = @new_skills, money = @new_cash
            WHERE id = @player_id;";

            using var cmd = db.CreateCommand(qNewValues);
            cmd.Parameters.AddWithValue("new_stamina", newStamina);
            cmd.Parameters.AddWithValue("new_cash", newCash);
            cmd.Parameters.AddWithValue("new_skills", newSkills);
            cmd.Parameters.AddWithValue("player_id", playerId);
            cmd.ExecuteNonQuery();

            ClientResponse(res, $"{playerName} has purchased {product} for the price of {priceCash}$..");
        }
        else
        {
            ClientResponse(res, "Not enough money..");
        }
    }
    public void Study(string body)
    {
        string qGetCurrentStamina = @"
        SELECT study_spot.stamina_cost, players.stamina
        FROM study_spot
        CROSS JOIN players
        WHERE study_spot.id = @study_spot_id AND players.id = @player_id;";

        string[] fields = body.Split(",");
        int id = Convert.ToInt32(fields[0]), studySpot = Convert.ToInt32(fields[1]);

        using var command = db.CreateCommand(qGetCurrentStamina);
        command.Parameters.AddWithValue("player_id", id);
        command.Parameters.AddWithValue("study_spot_id", studySpot);
        var reader = command.ExecuteReader();
        int currentStamina = 0;
        int staminaCost = 0;
        while (reader.Read())
        {
            staminaCost = reader.GetInt32(0);
            currentStamina = reader.GetInt32(1);
        }
        if (currentStamina >= staminaCost)
        {
            int newStamina = currentStamina - staminaCost;
            string updatePlayerLocation = @"
            UPDATE players
            set stamina = @newStamina
            WHERE id = @player_id;";

            using var cmd = db.CreateCommand(updatePlayerLocation);
            cmd.Parameters.AddWithValue("player_id", id);
            cmd.Parameters.AddWithValue("newStamina", newStamina);
            cmd.ExecuteNonQuery();
        }
        else
        {
            Console.WriteLine("Not enough stamina");
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
        insert into players (name,password,stamina,skills,money,day) Values
        (@name,@password,@stamina,@skills,@money,@day);";

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


    private void ErrorResponse(HttpListenerResponse res, string errorMessage)
    {
        res.StatusCode = 400; // Bad Request
        byte[] buffer = Encoding.UTF8.GetBytes(errorMessage);
        res.OutputStream.Write(buffer, 0, buffer.Length);
    }

    private void ClientResponse(HttpListenerResponse res, string successMessage)
    {
        res.StatusCode = 200; // OK
        byte[] buffer = Encoding.UTF8.GetBytes(successMessage);
        res.OutputStream.Write(buffer, 0, buffer.Length);
    }
}
