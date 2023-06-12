using Microsoft.ML.Data;

namespace lucene_tweets.DetectionModels;

public class SentimentOutput
{
    [ColumnName("PredictedLabel")]
    public int Sentiment { get; set; }
    public float Probability { get; set; }
    public float Score { get; set; }
}