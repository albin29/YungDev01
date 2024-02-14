using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YungDev01;

public class Check(NpgsqlDataSource db)
{
    public int Money(int playerId)
    {
        int currentMoney = 0;
        string qCheckMoney = @"
        select money
        from players
        where id = $1;";

        using var cmd = db.CreateCommand(qCheckMoney);
        cmd.Parameters.AddWithValue(playerId);
        var reader = cmd.ExecuteReader();

        while (reader.Read()) { currentMoney = reader.GetInt32(0); }

        return currentMoney;
    }
    public int Skills(int playerId)
    {
        int currentSkills = 0;
        string qCheckSkills = @"
        select skills 
        from players
        where id = $1;";

        using var cmd = db.CreateCommand(qCheckSkills);
        cmd.Parameters.AddWithValue(playerId);
        var reader = cmd.ExecuteReader();

        while (reader.Read()) { currentSkills = reader.GetInt32(0); }

        return currentSkills;
    }
    public int Stamina(int playerId)
    {
        int currentStamina = 0;
        string qCheckStamina = @"
        select stamina 
        from players
        where id = $1;";

        using var cmd = db.CreateCommand(qCheckStamina);
        cmd.Parameters.AddWithValue(playerId);
        var reader = cmd.ExecuteReader();

        while (reader.Read()) { currentStamina = reader.GetInt32(0); }

        return currentStamina;
    }
    public string Name(int playerId)
    {
        string playerName = string.Empty;
        string qPlayerName = @"
        select name
        from players
        where id = $1;";

        using var cmd = db.CreateCommand(qPlayerName);
        cmd.Parameters.AddWithValue(playerId);
        var reader = cmd.ExecuteReader();

        while (reader.Read()) { playerName = reader.GetString(0); }

        return playerName;
    }
}
