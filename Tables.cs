using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Npgsql;


namespace YungDev01;
public class Table(NpgsqlDataSource db)
{
    public async Task CreateTable()
    {
        await db.CreateCommand("DROP TABLE IF EXISTS study_spot cascade").ExecuteNonQueryAsync();
        await db.CreateCommand("DROP TABLE IF EXISTS shop CASCADE").ExecuteNonQueryAsync();

        string qShop = @"
                create table if not exists shop(
                id              serial      primary key,
                name            text,
                skills_given    int,
                stamina_given   int,
                price           int);";

        string qStudySpot = @"
                create table if not exists study_spot(
                id              serial      primary key,
                name            text,
                stamina_cost    int,
                skill_award     int);";

        string qPlayers = @"
                create table if not exists players(
                id              serial      primary key,
                name            text        unique,
                password        text,
                stamina         int,
                skills          int,
                money           int,
                points          int,
                day             int);";

        string qHighscore = @"
                CREATE TABLE IF NOT EXISTS highscore (
                id              serial      primary key,
                player_name     text,
                points          int);";

        await db.CreateCommand(qStudySpot).ExecuteNonQueryAsync();
        await db.CreateCommand(qPlayers).ExecuteNonQueryAsync();
        await db.CreateCommand(qHighscore).ExecuteNonQueryAsync();
        await db.CreateCommand(qShop).ExecuteNonQueryAsync();

        string qStudySpotInsertions = @"
                insert into study_spot (name, stamina_cost, skill_award) values
                ('Neoschool', 2, 2),
                ('Underground Study', 3, 3);";

        string qShopInsertions= @"
                insert into shop(name, stamina_given, skills_given, price) values
                ('AMD Ryzen Threadripper PRO 5995WX', 0, 20, 1000),
                ('SAMSUNG Odyssey ARK 55¨', 0, 9, 500),
                ('Razer Death Adder', 0, 2, 65),
                ('Redbull ULTRA', 3, 0, 150),
                ('Elias Snus', 1, 0, 60),
                ('Manuels NVIM Addons', 0, 4, 150),
                ('Project McFly', 0, 5, 125),
                ('NVIDIA RTX 4090', 0, 60, 2500);";


        await db.CreateCommand(qShopInsertions).ExecuteNonQueryAsync();
        await db.CreateCommand(qStudySpotInsertions).ExecuteNonQueryAsync();


        string triggerSQL = @"
                CREATE OR REPLACE FUNCTION calculate_points()
                RETURNS TRIGGER AS $$
                BEGIN
                    NEW.points := NEW.skills * 3 + NEW.money * 2;
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;

                CREATE OR REPLACE TRIGGER update_points_trigger
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

        CREATE OR REPLACE TRIGGER highscore_trigger
        AFTER INSERT OR UPDATE ON players
        FOR EACH ROW EXECUTE FUNCTION update_highscore();";

        await db.CreateCommand(sql).ExecuteNonQueryAsync();

    }
}
