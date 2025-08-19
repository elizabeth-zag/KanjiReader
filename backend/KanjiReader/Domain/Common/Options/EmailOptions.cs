namespace KanjiReader.Domain.Common.Options;

public class EmailOptions
{ 
    public string Host { get; set; }
    public int Port { get; set; }
    public string FromAddress { get; set; }
    public string Password { get; set; }
}