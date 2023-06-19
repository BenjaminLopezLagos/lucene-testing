using lucene_tweets.DetectionModels;
using Lucene.Net.Documents;
using VaderSharp2;

namespace lucene_tweets;

public class Tweet
{
    public SentimentOutput? MlOutput { get; set; }
    public double? VaderScore { get; set; }
    public string? NaiveBayesLabel { get; set; }

    public Lucene.Net.Documents.Document TweetContents { get; set; }

    public Tweet(Lucene.Net.Documents.Document doc)
    {
        TweetContents = doc;
    }
    /*
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
    */
    public override string ToString()
    {
        var sentimentOutput = "";
        if (NaiveBayesLabel != null)
        {
            sentimentOutput = $"{sentimentOutput}Naive Bayes: {NaiveBayesLabel}. \n";
        }

        if (VaderScore != null)
        {
            sentimentOutput = $"{sentimentOutput}Vader: {VaderScore}. \n";
        }
        
        if (MlOutput != null)
        {
            sentimentOutput = $"{sentimentOutput}BERT: {MlOutput.PredictedLabel}. \n";
        }

        var a = TweetContents.Get("date");
        return $"USER: {TweetContents.Get("user")} \n" +
               $"Contents: {TweetContents.Get("content")} \n" +
               $"Date: {DateTools.StringToDate(TweetContents.Get("date")).ToShortDateString()} \n" +
               $"{sentimentOutput}";
    }
}