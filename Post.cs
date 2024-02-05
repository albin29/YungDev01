using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YungDev01;

public class Post(NpgsqlDataSource db)
{
    public void PostCommands(HttpListenerRequest req, HttpListenerResponse res)
    {
        string? path = req.Url?.AbsolutePath;
        string? lastPath = req.Url?.AbsolutePath.Split("/").Last();

        if (path != null && path.Contains("/register"))
        {
            StreamReader reader = new(req.InputStream, req.ContentEncoding);
            string body = reader.ReadToEnd();

            CharacterRegister(body);

            Console.WriteLine($"Created the following in db: {body}");

            res.StatusCode = (int)HttpStatusCode.Created;
            res.Close();
        }
        if (path != null && path.Contains("/moveto/"))
        {
            StreamReader reader = new(req.InputStream, req.ContentEncoding);
            string body = reader.ReadToEnd();

            CharacterRegister(body);

            Console.WriteLine($"Created the following in db: {body}");

            res.StatusCode = (int)HttpStatusCode.Created;
            res.Close();
        }

    }
    public void CharacterRegister(string body)
    {
        string[] parts = body.Split(",");
        string name = parts[0];
        int skills = Convert.ToInt32(parts[1]);
        int stamina = Convert.ToInt32(parts[2]);

        //curl -X POST localhost:3000/register -d Danijel,12,5

        string qRegisterUser = @"
        insert into character (name, skills, stamina) Values
        (@name, @skills, @stamina);";
        var cmd = db.CreateCommand(qRegisterUser);
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("skills", skills);
        cmd.Parameters.AddWithValue("stamina", stamina);
        cmd.ExecuteNonQuery();
    }
    public void MoveTo(string body)
    {
        //curl -X POST localhost:3000/moveto -d {character},{locationNumber}
        string[] parts = body.Split(",");
        string character = parts[0];
        string locationNumber = parts[1];

        string qStaminaSkills = @"
        select stamina_cost, skill_point_award from locations where id = @locationId;
";

        var cmd = db.CreateCommand(qStaminaSkills);
        cmd.Parameters.AddWithValue("locationId", locationNumber);
        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        {
            reader.Read();
            {
                int staminaCost = reader.GetInt32(0);
                int skillPointAward = reader.GetInt32(1);
                Console.WriteLine($"Stamina cost: {staminaCost}, Skill point award: {skillPointAward}");
            }
        }
        
        
        if (int.TryParse(locationNumber, out int result))
        {
            //query för id där man kollar hur mycket stamina och skills den har, sätt det i en annan variabel,
            //gör sen en till query för att se hur mycket der krävs för att gå dit,
            //sen ta det - vad jag har, är det mindre än 0 så går det inte
            if (result == 0)
            {
                Console.WriteLine("Location changed to ");
            }
            else if (result == 1)
            {

            }
            else if (result == 2)
            {

            }
        }







        //curl -X POST localhost:3000/register -d Danijel,12,5






    }
}
