namespace lucene_tweets.DetectionModels;
using VaderSharp2;

public class Vader : DetectionStrategy
{
    public void DetectEmotion(IEnumerable<Tweet> tweets)
    {
        var analyzer = new SentimentIntensityAnalyzer();
        /*
        tweets.AsParallel().ForAll(x => x.VaderValues = new Dictionary<string, double>
        {
            { "Positive", analyzer.PolarityScores(x.Content).Positive },
            { "Negative", analyzer.PolarityScores(x.Content).Negative },
            { "Neutral", analyzer.PolarityScores(x.Content).Neutral }
        });
        */
        tweets.AsParallel().ForAll(x => x.VaderScore = analyzer.PolarityScores(x.Content).Compound);
    }
}