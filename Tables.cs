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
                points          int,
                day             int,
                location_id     int         references locations(id));";

        string highscore = @"
                CREATE TABLE IF NOT EXISTS highscore (
                id              SERIAL      PRIMARY KEY,
                player_name     TEXT,
                points          INT);";




        await db.CreateCommand(locations).ExecuteNonQueryAsync();
        await db.CreateCommand(players).ExecuteNonQueryAsync();
        await db.CreateCommand(highscore).ExecuteNonQueryAsync();

        string locationsInsertions = @"
                insert into locations (name, stamina_cost, skill_award) values
                ('Home', 0, 0),
                ('Neoschool', 2, 2),
                ('Underground Study', 3, 3);";

        await db.CreateCommand(locationsInsertions).ExecuteNonQueryAsync();


        string triggerSQL = @"
                CREATE OR REPLACE FUNCTION calculate_points()
                RETURNS TRIGGER AS $$
                BEGIN
                    NEW.points := NEW.skills * 3 + NEW.money * 2;
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;

                CREATE TRIGGER update_points_trigger
                BEFORE INSERT OR UPDATE ON players
                FOR EACH ROW
                EXECUTE FUNCTION calculate_points();
            ";

        await db.CreateCommand(triggerSQL).ExecuteNonQueryAsync();

        string sql = @"
                        CREATE OR REPLACE FUNCTION update_highscore()
            RETURNS TRIGGER AS $$
            BEGIN
                -- Check if the player already exists in the highscore table
                IF NOT EXISTS (
                    SELECT 1 FROM highscore WHERE player_name = NEW.name
                ) THEN
                    -- Insert the new player into the highscore table
                    INSERT INTO highscore (player_name, points) VALUES (NEW.name, NEW.points);
                ELSE
                    -- If the player already exists, update the score if the new score is higher
                    UPDATE highscore
                    SET points = NEW.points
                    WHERE player_name = NEW.name AND points < NEW.points;
                END IF;
                RETURN NULL;
            END;
            $$ LANGUAGE plpgsql;

        CREATE TRIGGER highscore_trigger
        AFTER INSERT OR UPDATE ON players
        FOR EACH ROW EXECUTE FUNCTION update_highscore();";

        await db.CreateCommand(sql).ExecuteNonQueryAsync();

    }
}
