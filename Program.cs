using System;
using MySql.Data.MySqlClient;

/*
CREATE TABLE utenti (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    ruolo ENUM('admin', 'utente') NOT NULL DEFAULT 'utente'
);

CREATE TABLE luoghi (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    descrizione TEXT
);

CREATE TABLE attrazioni (
    id INT AUTO_INCREMENT PRIMARY KEY,
    luogo_id INT NOT NULL,
    nome VARCHAR(255) NOT NULL,
    descrizione VARCHAR(255),
    FOREIGN KEY (luogo_id) REFERENCES luoghi(id) 
);
*/

// Per entrare da admin  'admin@admin.com', 'admin'
public class Db
{
    private string connStr = "server=localhost;user=Il_TUO_USER;database=ILTuoDB;port=LaTuaPorta;password=LaTuaPassword";
    public MySqlConnection conn;

    public Db()
    {
        conn = new MySqlConnection(connStr);
    }

    public void Open()
    {
        conn.Open();
    }

    public void Close()
    {
        conn.Close();
    }
}

public class Utente
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Ruolo { get; set; }

    public void Registrazione(string nome, string email, string password, string ruolo = "utente")
    {
        try
        {
            Db db = new Db();
            db.Open();
            string sql = "INSERT INTO utenti (username, email, password, ruolo) VALUES (@username, @email, @password, @ruolo)";
            using (MySqlCommand cmd = new MySqlCommand(sql, db.conn))
            {
                cmd.Parameters.AddWithValue("@username", nome);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.Parameters.AddWithValue("@ruolo", ruolo);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Registrazione completata!");
            }
            db.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante la registrazione: " + ex.Message);
        }
    }

    public (bool successo, string ruolo, int id) Login(string email, string password)
    {
        Db db = new Db();
        try
        {
            db.Open();
            string sql = "SELECT id, ruolo, username FROM utenti WHERE email = @email AND password = @password";
            using (MySqlCommand cmd = new MySqlCommand(sql, db.conn))
            {
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@password", password);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int id = reader.GetInt32("id");
                        string ruolo = reader.GetString("ruolo");
                        string nome = reader.GetString("username");
                        Console.WriteLine($"Benvenuto {nome}!");
                        db.Close();
                        return (true, ruolo, id);
                    }
                }
            }
            db.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante il login: " + ex.Message);
        }
        return (false, null, -1);
    }
}

public class Luogo
{
    public void AggiungiLuogo(string nome, string descrizione)
    {
        try
        {
            Db db = new Db();
            db.Open();
            string sql = "INSERT INTO luoghi (nome, descrizione) VALUES (@nome, @descrizione)";
            using (MySqlCommand cmd = new MySqlCommand(sql, db.conn))
            {
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@descrizione", descrizione);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Luogo aggiunto!");
            }
            db.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante l'inserimento del luogo: " + ex.Message);
        }
    }

    public void VisualizzaLuoghi()
    {
        Db db = new Db();
        try
        {
            db.Open();
            string sql = "SELECT id, nome, descrizione FROM luoghi";
            using (MySqlCommand cmd = new MySqlCommand(sql, db.conn))
            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine("Luoghi disponibili:");
                while (reader.Read())
                {
                    Console.WriteLine($"{reader.GetInt32("id")}: {reader.GetString("nome")} - {reader.GetString("descrizione")}");
                }
            }
            db.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante la lettura dei luoghi: " + ex.Message);
        }
    }
}

public class Attrazione
{
    public void AggiungiAttrazione(int luogoId, string nome, string descrizione)
    {
        try
        {
            Db db = new Db();
            db.Open();
            string sql = "INSERT INTO attrazioni (luogo_id, nome, descrizione) VALUES (@luogo_id, @nome, @descrizione)";
            using (MySqlCommand cmd = new MySqlCommand(sql, db.conn))
            {
                cmd.Parameters.AddWithValue("@luogo_id", luogoId);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@descrizione", descrizione);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Attrazione aggiunta!");
            }
            db.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante l'inserimento dell'attrazione: " + ex.Message);
        }
    }

    public void VisualizzaAttrazioniPerLuogo(int luogoId)
    {
        Db db = new Db();
        try
        {
            db.Open();
            string sql = "SELECT nome, descrizione FROM attrazioni WHERE luogo_id = @luogo_id";
            
            using (MySqlCommand cmd = new MySqlCommand(sql, db.conn))
            {
                cmd.Parameters.AddWithValue("@luogo_id", luogoId);
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Attrazioni:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"- {reader.GetString("nome")}: {reader.GetString("descrizione")}");
                    }
                }
            }
            db.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante la lettura delle attrazioni: " + ex.Message);
        }
    }
}

public class AgenziaViaggi
{
    public static void Main()
    {
        Menu();
    }

    public static void Menu()
    {
        Utente utente = new Utente();
        bool continua = true;

        while (continua)
        {
            Console.WriteLine("\n=== Agenzia Viaggi ===");
            Console.WriteLine("1. Registrazione");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Esci");
            Console.Write("Scelta: ");
            string scelta = Console.ReadLine();

            switch (scelta)
            {
                case "1":
                    Console.Write("Nome: ");
                    string nome = Console.ReadLine();
                    Console.Write("Email: ");
                    string email = Console.ReadLine();
                    Console.Write("Password: ");
                    string password = Console.ReadLine();
                    utente.Registrazione(nome, email, password);
                    break;
                case "2":
                    Console.Write("Email: ");
                    string emailLogin = Console.ReadLine();
                    Console.Write("Password: ");
                    string passwordLogin = Console.ReadLine();
                    var risultato = utente.Login(emailLogin, passwordLogin);
                    if (risultato.successo)
                    {
                        if (risultato.ruolo == "admin")
                            MenuAdmin();
                        else
                            MenuUtente();
                    }
                    else
                    {
                        Console.WriteLine("Login fallito.");
                    }
                    break;
                case "3":
                    continua = false;
                    break;
                default:
                    Console.WriteLine("Scelta non valida.");
                    break;
            }
        }
    }

    public static void MenuAdmin()
    {
        Luogo luogo = new Luogo();
        Attrazione attrazione = new Attrazione();
        bool continua = true;

        while (continua)
        {
            Console.WriteLine("\n--- Menu Admin ---");
            Console.WriteLine("1. Aggiungi luogo");
            Console.WriteLine("2. Aggiungi attrazione");
            Console.WriteLine("3. Visualizza luoghi e attrazioni");
            Console.WriteLine("4. Logout");
            Console.Write("Scelta: ");
            string scelta = Console.ReadLine();

            switch (scelta)
            {
                case "1":
                    Console.Write("Nome luogo: ");
                    string nomeLuogo = Console.ReadLine();
                    Console.Write("Descrizione: ");
                    string descLuogo = Console.ReadLine();
                    luogo.AggiungiLuogo(nomeLuogo, descLuogo);
                    break;
                case "2":
                    luogo.VisualizzaLuoghi();
                    int luogoId;
                    Console.Write("ID luogo: ");
                    while (!int.TryParse(Console.ReadLine(), out luogoId))
                    {
                        Console.Write("Inserisci un numero valido per l'ID luogo: ");
                    }
                    Console.Write("Nome attrazione: ");
                    string nomeAttr = Console.ReadLine();
                    Console.Write("Descrizione: ");
                    string descAttr = Console.ReadLine();
                    attrazione.AggiungiAttrazione(luogoId, nomeAttr, descAttr);
                    break;
                case "3":
                    luogo.VisualizzaLuoghi();
                    int idLuogo;
                    Console.Write("ID luogo per vedere le attrazioni (0 per tutti): ");
                    while (!int.TryParse(Console.ReadLine(), out idLuogo))
                    {
                        Console.Write("Inserisci un numero valido per l'ID luogo: ");
                    }
                    if (idLuogo > 0)
                        attrazione.VisualizzaAttrazioniPerLuogo(idLuogo);
                    else
                    {
                        // Visualizza tutte le attrazioni per ogni luogo
                        Db db = new Db();
                        db.Open();
                        string sql = "SELECT id, nome FROM luoghi";
                        using (MySqlCommand cmd = new MySqlCommand(sql, db.conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int lid = reader.GetInt32("id");
                                string lnome = reader.GetString("nome");
                                Console.WriteLine($"\nLuogo: {lnome}");
                                attrazione.VisualizzaAttrazioniPerLuogo(lid);
                            }
                        }
                        db.Close();
                    }
                    break;
                case "4":
                    continua = false;
                    break;
                default:
                    Console.WriteLine("Scelta non valida.");
                    break;
            }
        }
    }

    public static void MenuUtente()
    {
        Luogo luogo = new Luogo();
        Attrazione attrazione = new Attrazione();
        bool continua = true;

        while (continua)
        {
            Console.WriteLine("\n--- Menu Utente ---");
            Console.WriteLine("1. Visualizza luoghi");
            Console.WriteLine("2. Visualizza attrazioni di un luogo");
            Console.WriteLine("3. Logout");
            Console.Write("Scelta: ");
            string scelta = Console.ReadLine();

            switch (scelta)
            {
                case "1":
                    luogo.VisualizzaLuoghi();
                    break;
                case "2":
                    luogo.VisualizzaLuoghi();
                    int luogoId;
                    Console.Write("ID luogo: ");
                    while (!int.TryParse(Console.ReadLine(), out luogoId))
                    {
                        Console.Write("Inserisci un numero valido per l'ID luogo: ");
                    }
                    attrazione.VisualizzaAttrazioniPerLuogo(luogoId);
                    break;
                case "3":
                    continua = false;
                    break;
                default:
                    Console.WriteLine("Scelta non valida.");
                    break;
            }
        }
    }
}