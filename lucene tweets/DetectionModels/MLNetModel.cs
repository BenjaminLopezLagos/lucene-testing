using Lucene.Net.Documents;
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
        mlContext.GpuDeviceId = 0;
        mlContext.FallbackToCpu = true;
        
        var dataView = mlContext.Data.LoadFromTextFile<SentimentInput>($"{_datasetPath}\\Sentiment Analysis Dataset.csv",
            hasHeader: true,
            separatorChar: ',',
            allowQuoting: true,
            trimWhitespace: true);
        var dataSplit = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.3);
        var trainData = dataSplit.TrainSet;
        var testData = dataSplit.TestSet;
        //Define your training pipeline
        var pipeline =
            mlContext.Transforms.Conversion.MapValueToKey("Label", "Label")
                .Append(mlContext.MulticlassClassification.Trainers.TextClassification(
                    labelColumnName: "Label",
                    batchSize: 512,
                    maxEpochs: 3,
                    sentence1ColumnName: "Sentence",
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
        var trainedModel = mlContext.Model.Load($"{modelPath}", out modelSchema);
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
       t.ForEach(x => x.MlOutput = Engine.Predict(new SentimentInput{Sentence = x.Content}).PredictedLabel);
    }
    public void DetectEmotion(IEnumerable<Lucene.Net.Documents.Document> docs, TweetIndexer indexer)
    {
        //Add documents to the index
        foreach(var d in docs)
        {
            var dClass = Engine.Predict(new SentimentInput { Sentence = d.Get("content") }).PredictedLabel;
            var doc = new Document
            {
                new Int64Field("date",  long.Parse(d.Get("date")), Field.Store.YES),
                new StringField("user", d.Get("user"), Field.Store.YES),
                new TextField("content", d.Get("content"), Field.Store.YES),
                new DoubleField("mloutput", dClass, Field.Store.YES),
            };
            indexer.IndexWriter.AddDocument(doc);
        }
        //Flush and commit the index data to the directory
        indexer.IndexWriter.Commit();
    }
}