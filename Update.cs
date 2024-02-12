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
        string qGoodSleep = @"
        update players
        set money = @updated_money
        where id = @player_id;";

        using var command = db.CreateCommand(qGoodSleep);
        command.Parameters.AddWithValue("updated_money", updatedMoney);
        command.Parameters.AddWithValue("player_id", playerId);
        command.ExecuteNonQuery();
    }
    public void Stamina(int updatedStamina, int playerId)
    {
        string qGoodSleep = @"
        update players
        set stamina = @updated_stamina 
        where id = @player_id;";

        using var command = db.CreateCommand(qGoodSleep);
        command.Parameters.AddWithValue("updated_stamina", updatedStamina);
        command.Parameters.AddWithValue("player_id", playerId);
        command.ExecuteNonQuery();
    }
    public void Skills(int updatedSkills, int playerId)
    {
        string qGoodSleep = @"
        update players
        set stamina = @updated_skills 
        where id = @player_id;";

        using var command = db.CreateCommand(qGoodSleep);
        command.Parameters.AddWithValue("updated_skills", updatedSkills);
        command.Parameters.AddWithValue("player_id", playerId);
        command.ExecuteNonQuery();
    }
}
