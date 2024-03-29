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
    Check check = new(db);
    Update update = new(db);
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
                Console.WriteLine("Player went to sleep");

                
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

            if (path.Contains("work"))
            {
                Work(body);
            }
        }
    }
    public void Shop(string body)
    {
        string[] fields = body.Split(",");
        int playerId = Convert.ToInt32(fields[0]), shopId = Convert.ToInt32(fields[1]);

        string qCheckValues = @"
        SELECT price, skills_given, stamina_given, name
        FROM shop
        WHERE shop.id = $1;";

        using var command = db.CreateCommand(qCheckValues);
        command.Parameters.AddWithValue(shopId);

        int staminaGiven = 0, price = 0, skillsGiven = 0;
        string productName = string.Empty;

        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            price = reader.GetInt32(0);
            skillsGiven = reader.GetInt32(1);
            staminaGiven = reader.GetInt32(2);
            productName = reader.GetString(3);
        }

        if (check.Money(playerId) >= price)
        {
            update.Money(check.Money(playerId) - price, playerId);
            update.Stamina(check.Stamina(playerId) + staminaGiven, playerId);
            update.Skills(check.Skills(playerId) + skillsGiven, playerId);

            ClientResponse(res, $"{check.Name(playerId)} has purchased {productName} for the price of {price}$..");
        }
        else
        {
            ClientResponse(res, "Insufficient funds..");
        }
    }


    public void Work(string body)
    {
        string[] fields = body.Split(",");
        int playerId = Convert.ToInt32(fields[0]), shopId = Convert.ToInt32(fields[1]);

       string qCheckValues = @"
            SELECT name, skill_req, stamina_req, money_gain, skill_gain
            FROM workplace
            WHERE id = $1;";

        using var command = db.CreateCommand(qCheckValues);
        command.Parameters.AddWithValue(shopId);

        int staminaRequired = 0, moneyGain = 0, skillGain = 0, skillRequired = 0;
        string jobName = string.Empty;

        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            jobName = reader.GetString(0);
            skillRequired = reader.GetInt32(1);
            staminaRequired = reader.GetInt32(2);
            moneyGain = reader.GetInt32(3);
            skillGain = reader.GetInt32(4);
        }

        if (check.Stamina(playerId) >= staminaRequired && check.Skills(playerId) >= skillRequired)
        {
            update.Money(check.Money(playerId) + moneyGain, playerId);
            update.Skills(check.Skills(playerId) + skillGain, playerId);
            update.Stamina(check.Stamina(playerId) - staminaRequired, playerId);

            ClientResponse(res, $"{check.Name(playerId)} has worked as a {jobName} and gained {moneyGain}$ and {skillGain} skill points.");
        }
        else
        {
            ClientResponse(res, $"{check.Name(playerId)} does not meet the requirements to work as {jobName}.");
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

        using var command = db.CreateCommand(qGetCurrentDay);
        command.Parameters.AddWithValue(Convert.ToInt32(body));
        var reader = command.ExecuteReader();
        int day = 0;

        while (reader.Read()) { day = reader.GetInt32(0); }

        if (day <= 10)
        {
            ClientResponse(res, @"
            
            ");
            ClientResponse(res, @$"Current day: {day}

            ");
            Random randomEvent = new(db, body);
            System.Random random = new();
            int result = random.Next(1, 3);

            if (result == 1)
            {
                ClientResponse(res, randomEvent.Event());
            }
            else
            {
                ClientResponse(res, "No event was triggered..\n");
            }
            int playerId = Convert.ToInt32(body);

            update.Stamina(5, playerId);
            update.Day(day + 1, playerId);
            if (day == 10)
            {
                ClientResponse(res, "Day 10 reached! Last day, when you run out of stamina, it's game over. Make the most of it!");
            }
        }
        else if (day >= 11)
        {
            ClientResponse(res, @"

            This character has already reached day 10!");
        }

    }
    public void PlayerRegister(string body)
    {
        int stamina = 5, skills = 0, money = 100, day = 1;
        string[] fields = body.Split(',');
        string name = fields[0];

        string qRegisterPlayer = @"
        insert into players (name,stamina,skills,money,day) Values
        ($1, $2, $3, $4, $5);";

        var cmd = db.CreateCommand(qRegisterPlayer);
        cmd.Parameters.AddWithValue(name);
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
            ErrorResponse(res, "*  Not enough stamina to execute hack! sleep to recover stamina");
            return;
        }
        HackResult(hackerId, targetId, res);

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
    private void HackResult(int hackerId, int targetId, HttpListenerResponse res)
    {
        Check check = new(db);
        Update update = new(db);
        System.Random rnd = new System.Random();
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
