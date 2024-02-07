using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Npgsql;


namespace YungDev01;
public class Table(NpgsqlDataSource db)
{
    public async Task CreateTable()
    {

        await db.CreateCommand("DROP TABLE IF EXISTS locations cascade").ExecuteNonQueryAsync();
        await db.CreateCommand("DROP TABLE IF EXISTS highscore CASCADE").ExecuteNonQueryAsync();

        string locations = @"
                create table if not exists locations(
                id              serial      primary key,
                name            text,
                stamina_cost    int,
                skill_award     int);";

        string players = @"
                create table if not exists players(
                id              serial      primary key,
                name            text        unique,
                password        text,
                stamina         int,
                skills          int,
                money           int,
                day             int,
                location_id     int         references locations(id));";

        string highscore = @"
                 create table if not exists highscore (
                id              serial      primary key,
                player_name     text        references players(name),
                points          int);";


        await db.CreateCommand(locations).ExecuteNonQueryAsync();
        await db.CreateCommand(players).ExecuteNonQueryAsync();
        await db.CreateCommand(highscore).ExecuteNonQueryAsync();

        string locationsInsertions = @"
                insert into locations (name, stamina_cost, skill_award) values
                ('Home', 0, 0),
                ('Neoschool', 2, 2),
                ('Underground Study', 3, 3);";

        await db.CreateCommand(locationsInsertions).ExecuteNonQueryAsync();

        /*
        string view = @"CREATE OR REPLACE FUNCTION insert_into_highscore()
            RETURNS TRIGGER AS $$
            BEGIN
                -- Calculate points based on skills and money
                DECLARE
                    total_points INTEGER;
                    existing_points INTEGER;
                BEGIN
                    -- Calculate total points
                    total_points := (NEW.skills + NEW.money) * 3;
        
                    -- Check if player already exists in highscore table
                    SELECT points INTO existing_points FROM highscore WHERE player_name = NEW.name;
        
                    -- If player exists and new score is higher, update highscore
                    IF existing_points IS NOT NULL AND total_points > existing_points THEN
                        UPDATE highscore SET points = total_points WHERE player_name = NEW.name;
                    -- If player doesn't exist or new score is higher, insert into highscore
                    ELSE
                        -- Remove existing highscore
                        DELETE FROM highscore WHERE player_name = NEW.name;
                        -- Insert new highscore
                        INSERT INTO highscore (player_name, points)
                        VALUES (NEW.name, total_points);
                    END IF;
        
                    RETURN NULL;
                END;
            END;
            $$ LANGUAGE plpgsql;

            CREATE TRIGGER players_insert_trigger
            AFTER INSERT ON players
            FOR EACH ROW
            EXECUTE FUNCTION insert_into_highscore();";

        await db.CreateCommand(view).ExecuteNonQueryAsync();
        */
    }
}
