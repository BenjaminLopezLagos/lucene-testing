namespace lucene_tweets;

public class Tweet
{
    public string User { get; }
    public string Content { get; }
    public DateTime Date { get; }
    public string Class { get; }
    
    public Tweet(string user, string content, DateTime date)
    {
        User = user;
        Content = content;
        Date = date;
        Class = "?";
    }

    public Tweet(string user, string content, DateTime date, string @class)
    {
        User = user;
        Content = content;
        Date = date;
        Class = @class;
    }
}