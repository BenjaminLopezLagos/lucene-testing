using Microsoft.Data.Analysis;
using Microsoft.ML;

namespace lucene_tweets.DetectionModels;

public class MlNetModel : DetectionStrategy
{
    private string _datasetPath = "..\\..\\..\\DetectionModels"; 
    private readonly PredictionEngine<SentimentInput, SentimentOutput> _engine;

    public MlNetModel()
    {
        var ctx = new MLContext();
        var dataView = ctx.Data.LoadFromTextFile<SentimentInput>($"{_datasetPath}\\train.csv",
            hasHeader: true,
            separatorChar: ',',
            allowQuoting: true,
            trimWhitespace: true);
        var trainTestSplit = ctx.Data.TrainTestSplit(dataView, testFraction: 0.2);
        var trainingData = trainTestSplit.TrainSet;
        var testData = trainTestSplit.TestSet;

        var estimator = ctx.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentInput.InputMessage))
            .Append(ctx.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));

        var trainedModel = estimator.Fit(trainingData);
        _engine = ctx.Model.CreatePredictionEngine<SentimentInput, SentimentOutput>(trainedModel);
    }

    private void ProcessTrainData()
    {
        var dfForTraining = DataFrame.LoadCsv(_datasetPath);
        for (long i = 0; i < dfForTraining.Rows.Count; i++)
        {
            var row = dfForTraining.Rows[i];
            row[2] = Stemmer.StemInput(row[2].ToString());
        }
        DataFrame.SaveCsv(dfForTraining, path: $"{_datasetPath}\\stemmed train\\processed_train.csv");
    }
    public void DetectEmotion(IEnumerable<Tweet> tweets)
    {
        var t = tweets.ToList();
        t.ForEach(x => x.MlOutput = _engine.Predict(new SentimentInput{InputMessage = Stemmer.StemInput(x.Content)}));
    }
}