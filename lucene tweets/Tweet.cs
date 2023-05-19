namespace lucene_tweets;

public class Tweet
{
    public string User { get; set; }
    public string Content { get; set; }
    public string Date { get; set; }
    public string Class { get; set; }
    
    public Tweet(string user, string content, string date)
    {
        User = user;
        Content = content;
        Date = date;
        Class = "?";
    }

    public Tweet(string user, string content, string date, string @class)
    {
        User = user;
        Content = content;
        Date = date;
        Class = @class;
    }
}