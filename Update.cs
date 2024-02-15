using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YungDev01;

public class Update(NpgsqlDataSource db)
{
    public void Money(int updatedMoney, int playerId)
    {
        string qUpdatedMoney = @"
        update players
        set money = $1 
        where id = $2;";

        using var command = db.CreateCommand(qUpdatedMoney);
        command.Parameters.AddWithValue(updatedMoney);
        command.Parameters.AddWithValue(playerId);
        command.ExecuteNonQuery();
    }
    public void Stamina(int updatedStamina, int playerId)
    {
        string qUpdatedStamina = @"
        update players
        set stamina = $1 
        where id = $2;";

        using var command = db.CreateCommand(qUpdatedStamina);
        command.Parameters.AddWithValue(updatedStamina);
        command.Parameters.AddWithValue(playerId);
        command.ExecuteNonQuery();
    }
    public void Skills(int updatedSkills, int playerId)
    {
        string qUpdatedSkills = @"
        update players
        set skills = $1 
        where id = $2;";

        using var command = db.CreateCommand(qUpdatedSkills);
        command.Parameters.AddWithValue(updatedSkills);
        command.Parameters.AddWithValue(playerId);
        command.ExecuteNonQuery();
    }
    public void Day(int updatedDay, int playerId)
    {
        string qUpdatedDay = @"
        update players
        set day = $1 
        where id = $2;";

        using var command = db.CreateCommand(qUpdatedDay);
        command.Parameters.AddWithValue(updatedDay);
        command.Parameters.AddWithValue(playerId);
        command.ExecuteNonQuery();
    }
}
