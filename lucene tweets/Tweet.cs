using lucene_tweets.DetectionModels;
using VaderSharp2;

namespace lucene_tweets;

public class Tweet
{
    public string Date { get; set; }
    public SentimentOutput? MlOutput { get; set; }
    public double? VaderScore { get; set; }
    public string User { get; set; }
    public string Content { get; set; }
    
    public Tweet(string user, string content, string date)
    {
        User = user;
        Content = content;
        Date = date;
    }

    public Tweet(string user, string content, string date, SentimentOutput mlOutput)
    {
        User = user;
        Content = content;
        Date = date;
        MlOutput = mlOutput;
    }

    public override string ToString()
    {
        var sentimentOutput = "";
        if (MlOutput != null)
        {
            sentimentOutput = $"{sentimentOutput}MLNet: {MlOutput.Sentiment}. \n";
        }

        if (VaderScore != null)
        {
            sentimentOutput = $"{sentimentOutput}Vader: {VaderScore}. \n";
        }
        return $"USER: {User} \n" +
               $"Contents: {Content} \n" +
               $"Date: {Date} \n" +
               $"{sentimentOutput}";
    }
}