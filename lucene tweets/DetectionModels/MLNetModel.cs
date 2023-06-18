using Microsoft.Data.Analysis;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.TorchSharp;
using Microsoft.ML.TorchSharp.NasBert;

namespace lucene_tweets.DetectionModels;

public class MlNetModel : DetectionStrategy
{
    private string _datasetPath = "..\\..\\..\\DetectionModels"; 
    public PredictionEngine<SentimentInput, SentimentOutput> Engine { get; }

    public MlNetModel()
    {
        var mlContext = new MLContext();
        var reviews = new[]
        {
            new {Text = "This is a bad steak", Sentiment = "Negative"},
            new {Text = "I really like this restaurant", Sentiment = "Positive"}
        };
        var dataView = mlContext.Data.LoadFromTextFile<SentimentInput>($"{_datasetPath}\\train.csv",
            hasHeader: true,
            separatorChar: ',',
            allowQuoting: true,
            trimWhitespace: true);
        var dataSplit = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
        var trainData = dataSplit.TrainSet;
        var testData = dataSplit.TestSet;
        //Define your training pipeline
        var pipeline =
            mlContext.Transforms.Conversion.MapValueToKey("Label", "Label")
                .Append(mlContext.MulticlassClassification.Trainers.TextClassification(labelColumnName: "Label", sentence1ColumnName: "Sentence"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));

        Console.WriteLine("Training...");
        // Train the model
        var model = pipeline.Fit(trainData);
        Engine = mlContext.Model.CreatePredictionEngine<SentimentInput, SentimentOutput>(model);
        mlContext.Model.Save(model, dataView.Schema, "D:\\model.zip");
        Console.WriteLine("Training DONE");

        Console.WriteLine("Evaluating model performance...");
        // We need to apply the same transformations to our test set so it can be evaluated via the resulting model
        var transformedTest = model.Transform(testData);
        var metrics = mlContext.MulticlassClassification.Evaluate(transformedTest);
        Console.WriteLine($"Macro Accuracy: {metrics.MacroAccuracy}");
        Console.WriteLine($"Micro Accuracy: {metrics.MicroAccuracy}");
        Console.WriteLine($"Log Loss: {metrics.LogLoss}");
        Console.WriteLine();
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
       // t.ForEach(x => x.MlOutput = _engine.Predict(new SentimentInput{InputMessage = Stemmer.StemInput(x.TweetContents.Get("content"))}));
    }
}