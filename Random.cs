using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace YungDev01;

public class Random(NpgsqlDataSource db, string playerId)
{
    public string Event()
    {
        int id = Convert.ToInt32(playerId);

        Update update = new(db);
        Check check = new(db);
        System.Random random = new();

        int roll = random.Next(1, 11);

        string eventResponse = string.Empty;

        if (roll == 1)
        {
            update.Stamina(0, id);

            eventResponse = "You are feeling sick today and are gonna have to stay in..\n* -5 Stamina";
        }
        if (roll == 2 || roll == 3)
        {
            update.Stamina(4, id);

            eventResponse = "You didnt have a great night of sleep..\n* -1 Stamina";
        }
        if (roll == 4 || roll == 5)
        {
            update.Stamina(7, id);

            eventResponse = "You had an awesome night of sleep!\n* +2 Stamina";
        }
        if (roll == 6)
        {
            update.Stamina(0, id);
            update.Money(check.Money(id) + 3000, id);

            eventResponse = "On your way to bed, you decided to visit the local bar and got way too drunk, " +
                    "but you went to the casino and won big!\n* -5 Stamina\n* +3000$";
        }
        if (roll == 7 || roll == 8)
        {
            update.Money(check.Money(id) + 300, id);

            eventResponse = "After waking you find that you lost a tooth, you look under your pillow and find some money\n* +300$";
        }
        if (roll == 9 || roll == 10)
        {
            update.Money(check.Money(id) - 300, id);

            eventResponse = "Upon waking you look in your wallet and find a note saying your " +
                            "girlfriend took some money for shopping\n* -300$";
        }

        return "\u001b[91;1m**** EVENT TRIGGERED ****\u001b[0m\n\n" + eventResponse;
    }
}
