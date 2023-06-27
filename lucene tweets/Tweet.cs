using CsvHelper.Configuration.Attributes;
using lucene_tweets.DetectionModels;
using Lucene.Net.Documents;
using VaderSharp2;

namespace lucene_tweets;

public class Tweet
{
    [Name("mloutput")]
    public float? MlOutput { get; set; }
    public double? VaderScore { get; set; }
    public string? NaiveBayesLabel { get; set; }

    public DateTime Date { get; set; }
    public string User { get; set; }
    public string Content { get; set; }

    public Lucene.Net.Documents.Document TweetContents { get; set; }

    public Tweet(Lucene.Net.Documents.Document doc)
    {
        TweetContents = doc;
        Date = DateTools.StringToDate(doc.Get("date"));
        User = doc.Get("user");
        Content = doc.Get("content");
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
            sentimentOutput = $"{sentimentOutput}BERT: {MlOutput}. \n";
        }

        var a = Date;
        return $"USER: {User} \n" +
               $"Contents: {Content} \n" +
               $"Date: {Date.ToShortDateString()} \n" +
               $"{sentimentOutput}";
    }
}