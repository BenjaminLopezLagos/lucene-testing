﻿using Microsoft.Data.Analysis;
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
        
        var dataView = mlContext.Data.LoadFromTextFile<SentimentInput>($"{_datasetPath}\\Twitter_Dataset.csv",
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
                .Append(mlContext.MulticlassClassification.Trainers.TextClassification(
                    labelColumnName: "Label",
                    sentence1ColumnName: "Sentence",
                    maxEpochs: 30,
                    architecture: BertArchitecture.Roberta))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));

        Console.WriteLine("Training...");
        // Train the model
        var model = pipeline.Fit(trainData);
        Engine = mlContext.Model.CreatePredictionEngine<SentimentInput, SentimentOutput>(model);
        mlContext.Model.Save(model, trainData.Schema, $"{_datasetPath}\\model_roberta.zip");
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

    public MlNetModel(string modelPath)
    {
        var mlContext = new MLContext();
        DataViewSchema modelSchema;
        var trainedModel = mlContext.Model.Load($"{_datasetPath}\\{modelPath}", out modelSchema);
        Engine = mlContext.Model.CreatePredictionEngine<SentimentInput, SentimentOutput>(trainedModel);
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
       t.ForEach(x => x.MlOutput = Engine.Predict(new SentimentInput{Sentence = x.TweetContents.Get("content")}));
    }
}