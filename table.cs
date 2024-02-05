using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
namespace YungDev01;


public class Table(NpgsqlDataSource db)
{
    public async Task CreateTables()
    {
        await db.CreateCommand("DROP TABLE IF EXISTS player CASCADE").ExecuteNonQueryAsync();
        await db.CreateCommand("DROP TABLE IF EXISTS player_stats CASCADE").ExecuteNonQueryAsync();
        await db.CreateCommand("DROP TABLE IF EXISTS highscore CASCADE").ExecuteNonQueryAsync();

        //Create tables
        string player = @"create table if not exists player(
                id serial primary key,
                name text,
                password text);";
        await using (var cmd = db.CreateCommand(player))
        {
            await cmd.ExecuteNonQueryAsync();
        }
        string playerStats = @"create table if not exists player_stats(
                       player_id integer references player(id),
                       current_day int,
                       programming_skill int,
                       math_skill int,
                       money int,
                       stamina int,
                       score int);";

        await db.CreateCommand(playerStats).ExecuteNonQueryAsync();

        string highscore = @"CREATE TABLE IF NOT EXISTS highscores AS
                     SELECT
                     p.name AS player_name,
                     ps.current_day,
                     ps.score desc
                     FROM
                     player_stats ps
                     JOIN
                     player p ON ps.player_id = p.id;";

        await db.CreateCommand(highscore).ExecuteNonQueryAsync();

        //Create triggers
        string view = @"CREATE OR REPLACE FUNCTION update_highscores()
                        RETURNS TRIGGER AS $$
                        BEGIN
                        DELETE FROM highscores;
                        INSERT INTO highscores (player_name, current_day, score)
                        SELECT
                        p.name AS player_name,
                        ps.current_day,
                        ps.score
                        FROM
                            player_stats ps
                        JOIN
                            player p ON ps.player_id = p.id;
                        RETURN NULL;
                        END;
                        $$ LANGUAGE plpgsql;
                        
                        CREATE TRIGGER update_highscores_trigger
                        AFTER INSERT OR UPDATE OR DELETE ON player_stats
                        FOR EACH STATEMENT
                        EXECUTE FUNCTION update_highscores();";

        await db.CreateCommand(view).ExecuteNonQueryAsync();


        //Mockdata
        string mockdataPlayer = @"
                        insert into player (id, name, password) values (1, 'Wilhelmina', 'Vasyushkhin');
                        insert into player (id, name, password) values (2, 'Owen', 'Lembcke');
                        insert into player (id, name, password) values (3, 'Carlen', 'Godlee');
                        insert into player (id, name, password) values (4, 'Carmel', 'Melding');
                        insert into player (id, name, password) values (5, 'Wallie', 'Roscow');
                        insert into player (id, name, password) values (6, 'Ody', 'Emmens');
                        insert into player (id, name, password) values (7, 'Immanuel', 'Guerriero');
                        insert into player (id, name, password) values (8, 'Marcile', 'Hyndley');
                        insert into player (id, name, password) values (9, 'Randal', 'Chilles');
                        insert into player (id, name, password) values (10, 'Ichabod', 'Guiden');";
        await db.CreateCommand(mockdataPlayer).ExecuteNonQueryAsync();


        string qCharacterTable = @"
        Create table if not exists character(
        id      serial      primary key,
        name                text,
        skills              int,
        stamina             int,
        location            int);";

        await db.CreateCommand(qCharacterTable).ExecuteNonQueryAsync();

        string qDropLocations = @"drop table if exists locations;";

        await db.CreateCommand(qDropLocations).ExecuteNonQueryAsync();

        string qLocations = @"
        create table if not exists locations(
        id      serial      primary key,
        name                text,
        stamina_cost        int,
        skill_point_award   int)";

        await db.CreateCommand(qLocations).ExecuteNonQueryAsync();

        string qLocationInsertions = @"
        insert into locations (name, stamina_cost, skill_point_award) values
        ('Home', 0, 0), 
        ('Marks House', 2, 1),
        ('Underground Study', 3, 5);";

        await db.CreateCommand(qLocationInsertions).ExecuteNonQueryAsync();

        /*string mockdataPlayer_Stats = @"insert into player_stats (player_id, current_day, programming_skill, math_skill, money, stamina, score) values (1, 19, 5, 2, 710, 1, 663);
                        insert into player_stats (player_id, current_day, programming_skill, math_skill, money, stamina, score) values (2, 25, 14, 5, 973, 5, 71);
                        insert into player_stats (player_id, current_day, programming_skill, math_skill, money, stamina, score) values (3, 20, 4, 1, 181, 3, 376);
                        insert into player_stats (player_id, current_day, programming_skill, math_skill, money, stamina, score) values (4, 27, 5, 4, 541, 3, 250);
                        insert into player_stats (player_id, current_day, programming_skill, math_skill, money, stamina, score) values (5, 7, 15, 13, 282, 5, 183);
                        insert into player_stats (player_id, current_day, programming_skill, math_skill, money, stamina, score) values (6, 25, 1, 4, 346, 0, 381);
                        insert into player_stats (player_id, current_day, programming_skill, math_skill, money, stamina, score) values (7, 7, 8, 12, 118, 3, 744);
                        insert into player_stats (player_id, current_day, programming_skill, math_skill, money, stamina, score) values (8, 13, 15, 13, 828, 3, 285);
                        insert into player_stats (player_id, current_day, programming_skill, math_skill, money, stamina, score) values (9, 19, 19, 3, 42, 4, 296);
                        insert into player_stats (player_id, current_day, programming_skill, math_skill, money, stamina, score) values (10, 23, 16, 11, 289, 3, 789);";
        await db.CreateCommand(mockdataPlayer_Stats).ExecuteNonQueryAsync();
        */
    }
}
