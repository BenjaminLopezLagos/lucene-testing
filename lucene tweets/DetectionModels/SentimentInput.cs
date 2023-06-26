using Microsoft.ML.Data;

namespace lucene_tweets.DetectionModels;

public class SentimentInput
{
    [LoadColumn(1), ColumnName("Label")]
    public float Label { get; set; }
    [LoadColumn(3), ColumnName("Sentence")]
    public string Sentence { get; set; }
}