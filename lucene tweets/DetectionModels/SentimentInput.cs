using Microsoft.ML.Data;

namespace lucene_tweets.DetectionModels;

public class SentimentInput
{
    [LoadColumn(1), ColumnName("Label")]
    public bool Label { get; set; }
    [LoadColumn(2)]
    public string InputMessage { get; set; }
}