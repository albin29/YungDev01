using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace YungDev01;

public class RandomEventGenerator(NpgsqlDataSource db, string body)
{

    public void Event()
    {

        Random random = new();
        int roll = random.Next(1, 6);
        if (roll == 1)
        {
            string qFeelingSick = @"
            update players
            set stamina = 0
            where id = @player_id;";

            using var command = db.CreateCommand(qFeelingSick);
            command.Parameters.AddWithValue("player_id", Convert.ToInt32(body));
        }

        if (roll == 2 || roll == 3)
        {
            string qBadSleep= @"
            update players
            set stamina = 4
            where id = @player_id;";

            using var command = db.CreateCommand(qBadSleep);
            command.Parameters.AddWithValue("player_id", Convert.ToInt32(body));
        }
        if (roll == 4 || roll == 5)
        {
            string qGoodSleep = @"
            update players
            set stamina = 7
            where id = @player_id;";

            using var command = db.CreateCommand(qGoodSleep);
            command.Parameters.AddWithValue("player_id", Convert.ToInt32(body));
        }
        if (roll == 19)
        {
            string qGoodSleep = @"
            update players
            set stamina = 7
            where id = @player_id;";

            using var command = db.CreateCommand(qGoodSleep);
            command.Parameters.AddWithValue("player_id", Convert.ToInt32(body));
        }
        if (roll == 19)
        {
            string qGoodSleep = @"
            update players
            set stamina = 7
            where id = @player_id;";

            using var command = db.CreateCommand(qGoodSleep);
            command.Parameters.AddWithValue("player_id", Convert.ToInt32(body));
        }


    }
}
