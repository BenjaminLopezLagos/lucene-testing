using Microsoft.ML.Data;

namespace lucene_tweets.ML_Models;

public class SentimentOutput
{
    [ColumnName("PredictedLabel")]
    public bool Sentiment { get; set; }
    public float Probability { get; set; }
    public float Score { get; set; }
}