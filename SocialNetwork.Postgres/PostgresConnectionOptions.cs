namespace SocialNetwork.Postgres;

public class PostgresConnectionOptions
{
    public string Database { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
}