using System.Collections.Concurrent;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using CsvHelper;
using Lucene.Net.Util;
using Microsoft.Data.Analysis;
using lucene_tweets;
using lucene_tweets.ML_Models;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Classification;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Microsoft.ML;
using LuceneDirectory = Lucene.Net.Store.Directory;
using Document = Lucene.Net.Documents.Document;

// Specify the compatibility version we want
const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;

//Open the Directory using a Lucene Directory class
var indexNameTweets = "example_index";

/****** INDEXER ******/
var indexer = new TweetIndexer(luceneVersion, indexNameTweets, new StandardAnalyzer(luceneVersion));
var filePaths = Directory.GetFiles("..\\..\\..\\elon_tweets");
/*
indexer.DeleteCurrentIndex();
foreach (var f in filePaths)
{
    var df = DataFrame.LoadCsv(f);
    indexer.AddTweetsToIndex(df);
}
*/

/****** SEARCHER ******/
var searcher = new TweetSearcher(indexNameTweets);

/****** CLASSIFIER ******/
var ctx = new MLContext();

//Step 2. Read in the input data from a text file for model training
var dataView = ctx.Data.LoadFromTextFile<SentimentInput>("..\\..\\..\\ML_Models\\train.csv", hasHeader: true, separatorChar: ',', allowQuoting: true, trimWhitespace: true);
var trainTestSplit = ctx.Data.TrainTestSplit(dataView, testFraction: 0.2);
var trainingData = trainTestSplit.TrainSet;
var testData = trainTestSplit.TestSet;

var estimator = ctx.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentInput.Message))
    .Append(ctx.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));

var trainedModel = estimator.Fit(trainingData);
var predEngine = ctx.Model.CreatePredictionEngine<SentimentInput, SentimentOutput>(trainedModel);

/*
var query = new BooleanQuery();
//query.Add(new TermQuery(new Term("content", "csm")), Occur.MUST);
//query.Add(NumericRangeQuery.NewInt32Range(field:"views",min:0, max:100,true,true), Occur.MUST);
query.Add(new WildcardQuery(new Term("content", "c*m")), Occur.MUST);
*/
var query = new MatchAllDocsQuery();

var resultDocs = searcher.CustomQuery(query, numberOfResults: 15000);
//TweetSearcher.PrintResults(resultDocs);
/*
var termFreqDf = new DataFrame(columns: new DataFrameColumn[]
{
    new StringDataFrameColumn("term"),
    new PrimitiveDataFrameColumn<long>("frequency"),
});
var fields = MultiFields.GetFields(searcher.IndexReader);
var terms = fields.GetTerms("content");
var iterator = terms.GetEnumerator(null);
while (iterator.MoveNext())
{
    var currentTerm = iterator.Term;
    termFreqDf.Append(new[]
    {
        new KeyValuePair<string, object>("term", currentTerm.Utf8ToString()),
    }, inPlace: true);
    Console.WriteLine($"{currentTerm.Utf8ToString()}");
}
DataFrame.SaveCsv(termFreqDf, @"..\\..\\..\\TFID.csv");
*/
if (resultDocs != null)
{
    var concurrentTweetBag = new ConcurrentBag<Document>(resultDocs);
    var tweets = new ConcurrentBag<Tweet>();
    concurrentTweetBag.AsParallel().ForAll( t=>
        tweets.Add(new Tweet(t.Get("user"),
            $"{ '"' } {t.Get("content").Replace("\n", " ").Replace("\r", " ")} { '"' }",
            DateTime.Parse(t.Get("date")).ToShortDateString()))
        );
    tweets.ToList().ForEach(t => t.Class = predEngine.Predict(new SentimentInput{Message = t.Content}));
    Console.WriteLine("writing");
    using var writer = new StreamWriter(@"..\\..\\..\\ClassifiedTweets.csv");
    using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
    csv.WriteRecords(tweets.ToList());
    csv.Flush();
}
Console.WriteLine("done");
