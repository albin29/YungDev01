﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace YungDev01;

public class Post(NpgsqlDataSource db, HttpListenerRequest req)
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
                MoveTo(body);
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

}