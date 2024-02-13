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
            if (path.Contains("hack"))
            {
                Hack(body, res);
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
        WHERE shop.id = $1 AND players.id = $2;";

        string[] fields = body.Split(",");
        int playerId = Convert.ToInt32(fields[0]), shopId = Convert.ToInt32(fields[1]);

        using var command = db.CreateCommand(qCheckValues);
        command.Parameters.AddWithValue(playerId);
        command.Parameters.AddWithValue(shopId);

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
            set stamina = $1, skills = $2, money = $3 
            WHERE id = $4;";

            using var cmd = db.CreateCommand(qNewValues);
            cmd.Parameters.AddWithValue(newStamina);
            cmd.Parameters.AddWithValue(newSkills);
            cmd.Parameters.AddWithValue(newCash);
            cmd.Parameters.AddWithValue(playerId);
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
        SELECT study_spot.name, study_spot.stamina_cost, study_spot.skill_award, players.stamina , players.skills
        FROM study_spot
        CROSS JOIN players
        WHERE study_spot.id = $1 AND players.id = $2;";

        string[] fields = body.Split(",");
        int id = Convert.ToInt32(fields[0]), studySpot = Convert.ToInt32(fields[1]);

        using var command = db.CreateCommand(qGetCurrentStamina);
        command.Parameters.AddWithValue(studySpot);
        command.Parameters.AddWithValue(id);
        var reader = command.ExecuteReader();
        string spotName = string.Empty;
        int currentStamina = 0, skillAward = 0, staminaCost = 0, currentSkill = 0;
        while (reader.Read())
        {
            spotName = reader.GetString(0);
            staminaCost = reader.GetInt32(1);
            skillAward = reader.GetInt32(2);
            currentStamina = reader.GetInt32(3);
            currentSkill = reader.GetInt32(4);
        }

        if (currentStamina >= staminaCost)
        {
            int newStamina = currentStamina - staminaCost;
            int newSkill = skillAward + currentSkill;
            string updatePlayerLocation = @"
            UPDATE players
            set stamina = $1, skills = $2 
            WHERE id = $3;";

            using var cmd = db.CreateCommand(updatePlayerLocation);
            cmd.Parameters.AddWithValue(newStamina);
            cmd.Parameters.AddWithValue(newSkill);
            cmd.Parameters.AddWithValue(id);
            cmd.ExecuteNonQuery();
            ClientResponse(res, $"You studied at {spotName}\n* Current Stamina = {newStamina}\n* Current skills = {newSkill}");
        }
        else
        {
            ErrorResponse(res, "Insufficient stamina..");
        }
    }
    public void Sleep(string body)
    {
        string qGetCurrentDay = @"
        SELECT day
        FROM players
        WHERE id = $1;
        ";

        using var command2 = db.CreateCommand(qGetCurrentDay);
        command2.Parameters.AddWithValue(Convert.ToInt32(body));
        var reader = command2.ExecuteReader();
        int stamina = 5, day = 0;

        while (reader.Read()) { day = reader.GetInt32(0); }

        string qUpdatePlayer = @"
        UPDATE players
        SET day = $1, stamina = $2
        WHERE id = $3;
        ";

        using var command = db.CreateCommand(qUpdatePlayer);
        command.Parameters.AddWithValue(day + 1);
        command.Parameters.AddWithValue(stamina);
        command.Parameters.AddWithValue(Convert.ToInt32(body));
        command.ExecuteNonQuery();

    }
    public void PlayerRegister(string body)
    {
        int stamina = 5, skills = 0, money = 100, day = 1;
        string[] fields = body.Split(',');
        string name = fields[0], password = fields[1];

        string qRegisterPlayer = @"
        insert into players (name,password,stamina,skills,money,day) Values
        ($1, $2, $3, $4, $5, $6);";

        var cmd = db.CreateCommand(qRegisterPlayer);
        cmd.Parameters.AddWithValue(name);
        cmd.Parameters.AddWithValue(password);
        cmd.Parameters.AddWithValue(stamina);
        cmd.Parameters.AddWithValue(skills);
        cmd.Parameters.AddWithValue(money);
        cmd.Parameters.AddWithValue(day);
        cmd.ExecuteNonQuery();

        ClientResponse(res, $"Player Created: {name}\nCurrent Stats\n* Stamina: {stamina}\n* Skills: {skills}\n* Money: {money}\n* Day: {day}");

    }
    private void Hack(string body, HttpListenerResponse res)
    {
        var parts = body.Split(',');

        if (parts.Length < 2 || !int.TryParse(parts[0].Trim(), out int hackerId) ||
            !int.TryParse(parts[1].Trim(), out int targetId))
        {
            ErrorResponse(res, "*  Enter only integers starting from 1");
            return;
        }

        if (!PlayerCheck(hackerId) || !PlayerCheck(targetId))
        {
            ErrorResponse(res, "*  enter a number from 1 and up , try a lower number if the target number dont exist");
            return;
        }

        int playerStamina = StaminaCheck(hackerId);
        if (playerStamina < 1)
        {
            ErrorResponse(res, "*  Not enough stamina to execute hack! sleep to recover stamina" );
            return;
        }
        HackResult(hackerId,targetId, res);

        ClientResponse(res, $"player {hackerId} succesfully hacked player {targetId}");
    }
    private bool PlayerCheck(int hackerId)
    {
        string qPlayercheck = @"SELECT COUNT(*) FROM players WHERE id = @hackerId";

        using var cmd = db.CreateCommand(qPlayercheck);
        cmd.Parameters.AddWithValue("@hackerId", hackerId);

        var result = cmd.ExecuteScalar();
        return Convert.ToInt32(result) > 0;
    }
    private void HackResult(int hackerId, int targetId,  HttpListenerResponse res)
    {
        Check check = new(db);
        Update update = new(db);
        Random rnd = new Random();
        int randomskill = rnd.Next(3, 11);
        int randommoney = rnd.Next(0, 101);
        int staminacost = 1;

        string qTargetresult = @"UPDATE players SET
        skills = GREATEST(skills - $1, 0),
        money = GREATEST(money - $2, 0)
        WHERE id = $3";


        using (var cmd = db.CreateCommand(qTargetresult))
        {
            cmd.Parameters.AddWithValue(randomskill);
            cmd.Parameters.AddWithValue(randommoney);
            cmd.Parameters.AddWithValue(targetId);
            int rowchang = cmd.ExecuteNonQuery();

            if (rowchang == 0)
            {
                ErrorResponse(res, "*  Hack did not execute, No update was made.");
            }
        }

        string qHackresult = @"
        UPDATE players SET
        skills = skills + $1,
        money = money + $2,
        stamina = stamina - $3 
        WHERE id = $4 AND stamina >= $3
        ";

        using (var cmd = db.CreateCommand(qHackresult))
        {
            cmd.Parameters.AddWithValue(randomskill);
            cmd.Parameters.AddWithValue(randommoney);
            cmd.Parameters.AddWithValue(staminacost);
            cmd.Parameters.AddWithValue(hackerId);

            int rowchanged = cmd.ExecuteNonQuery();

            if (rowchanged == 0)
            {
                ErrorResponse(res, "*  Hack did not execute, No update was made.");
            }

            ClientResponse(res, $"*  You earned {randomskill} Skillpoints and {randommoney} Gold  ");

        }
    }
    private int StaminaCheck(int hackerId)
    {
        string qStaminacheck = @"SELECT stamina FROM players WHERE id = $1";
        using (var cmd = db.CreateCommand())
        {
            cmd.CommandText = qStaminacheck;
            cmd.Parameters.AddWithValue(hackerId);
            object? result = cmd.ExecuteScalar();
            return result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }
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
