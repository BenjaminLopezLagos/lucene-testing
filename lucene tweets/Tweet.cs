using lucene_tweets.ML_Models;

namespace lucene_tweets;

public class Tweet
{
    public SentimentOutput? Class { get; set; }
    public string User { get; set; }
    public string Content { get; set; }
    public string Date { get; set; }
    
    public Tweet(string user, string content, string date)
    {
        User = user;
        Content = content;
        Date = date;
    }

    public Tweet(string user, string content, string date, SentimentOutput @class)
    {
        User = user;
        Content = content;
        Date = date;
        Class = @class;
    }
}