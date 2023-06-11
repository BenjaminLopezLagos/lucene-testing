namespace lucene_tweets.DetectionModels;

public class SentimentDetection
{
    private DetectionStrategy _strategy;

    public SentimentDetection(DetectionStrategy strategy)
    {
        _strategy = strategy;
    }

    public void ChangeStrategy(DetectionStrategy strategy)
    {
        _strategy = strategy;
    }

    public void ExecuteDetector(IEnumerable<Tweet> t)
    {
        _strategy.DetectEmotion(t);
    }
}