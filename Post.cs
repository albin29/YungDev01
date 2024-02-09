﻿using System;
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
            if (path.Contains("moveto"))
            {
                Console.WriteLine($"Registered the following {body}");
                MoveTo(body);
            }

            if (path.Contains("Hack"))
            {
                Hack(body, res);
            }
        }
    }
    public void MoveTo(string body)
    {
        string qGetCurrentStamina = @"
    SELECT locations.stamina_cost, players.stamina
    FROM locations
    JOIN players ON locations.id = @test
    WHERE players.id = @player_id;
    ";
        string[] fields = body.Split(",");
        int id = Convert.ToInt32(fields[0]), location = Convert.ToInt32(fields[1]);

        using var command = db.CreateCommand(qGetCurrentStamina);
        command.Parameters.AddWithValue("player_id", id);
        command.Parameters.AddWithValue("test",location);
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
        SET location_id = @location,
            stamina = @newStamina
        WHERE id = @player_id;";

            using var cmd = db.CreateCommand(updatePlayerLocation);
            cmd.Parameters.AddWithValue("player_id", id);
            cmd.Parameters.AddWithValue("location", location);
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

        int playerStamina = StaminaCheck(hackerId);
        if (playerStamina < 1)
        {
            ErrorResponse(res,"Not enough stamina to execute hack! sleep to recover stamina" );
            return;
        }
        HackResult(hackerId, targetId, res);

        
        ClientResponse(res, $"player {hackerId} succesfully hacked player {targetId}");


    }

    private bool PlayerCheck(int hackerId)
    {

        string qPlayercheck = @"SELECT COUNT(*) FROM players WHERE id = @playerId";

        using var cmd = db.CreateCommand(qPlayercheck);
        cmd.Parameters.AddWithValue("@playerId", hackerId);
        var result = cmd.ExecuteScalar();
        return Convert.ToInt32(result) > 0;

    }

    private void HackResult(int hackerId,int targetId, HttpListenerResponse res)
    {
        Random rnd = new Random();
        int randomskill = rnd.Next(1, 5);
        int randommoney = rnd.Next(0, 11);
        int staminacost = 1;

        string qTargetresult = @"UPDATE players SET
        skills = GREATEST(skills - @randomskill ,0),
        money = GREATEST(skills - @randommoney , 0)
        WHERE id = @targetId";



        string qHackresult = @"
        UPDATE players SET
        skills = skills + @randomskill,
        money = money + @randommoney,
        stamina = stamina - @staminacost
        WHERE id = @hackerId AND stamina >= @staminacost
        ";

        
        using (var cmd = db.CreateCommand(qTargetresult))
        {
            cmd.Parameters.AddWithValue("@randomskill", randomskill);
            cmd.Parameters.AddWithValue("@randommoney", randommoney);
            cmd.Parameters.AddWithValue("@targetId", targetId);

            int rowchang = cmd.ExecuteNonQuery();

            if (rowchang == 0)
            {
                ErrorResponse(res, "Hack did not execute, No update was made.");
            }

        }

        using (var cmd = db.CreateCommand(qHackresult))
        {
            cmd.Parameters.AddWithValue("randomskill", randomskill);
            cmd.Parameters.AddWithValue("randommoney", randommoney);
            cmd.Parameters.AddWithValue("staminacost", staminacost);
            cmd.Parameters.AddWithValue("hackerId", hackerId);

            int rowchanged = cmd.ExecuteNonQuery();

            if (rowchanged == 0)
            {
                ErrorResponse(res, "Hack did not execute, No update was made.");
            }

            ClientResponse(res, $"  You earned {randomskill} Skillpoints and {randommoney} Gold  ");

        }
    }

    private int StaminaCheck(int hackerId)
    {
        string qStaminacheck = @"SELECT stamina FROM players WHERE id = @playerId";
        using (var cmd = db.CreateCommand())
        {
            cmd.CommandText = qStaminacheck;
            cmd.Parameters.AddWithValue("@playerId", hackerId);
            object result = cmd.ExecuteScalar();
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
