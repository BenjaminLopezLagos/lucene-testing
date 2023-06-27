using System.Collections.Concurrent;
using Lucene.Net.Documents;

namespace lucene_tweets.DetectionModels;
using VaderSharp2;

public class Vader : DetectionStrategy
{
    public void DetectEmotion(IEnumerable<Tweet> tweets)
    {
        var analyzer = new SentimentIntensityAnalyzer();
        //var concurrentTweets = new ConcurrentBag<Tweet>(tweets);
        /*
        tweets.AsParallel().ForAll(x => x.VaderValues = new Dictionary<string, double>
        {
            { "Positive", analyzer.PolarityScores(x.Content).Positive },
            { "Negative", analyzer.PolarityScores(x.Content).Negative },
            { "Neutral", analyzer.PolarityScores(x.Content).Neutral }
        });
        */
        tweets.ToList().ForEach(x => x.VaderScore = analyzer.PolarityScores(x.TweetContents.Get("content")).Compound);
    }

    public void DetectEmotion(IEnumerable<Document> docs, TweetIndexer indexer)
    {
        throw new NotImplementedException();
    }
}