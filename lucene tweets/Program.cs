using System.Collections.Concurrent;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using CsvHelper;
using Lucene.Net.Util;
using Microsoft.Data.Analysis;
using lucene_tweets;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Classification;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using LuceneDirectory = Lucene.Net.Store.Directory;
using Document = Lucene.Net.Documents.Document;

// Specify the compatibility version we want
const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;
/****** DATA ******/
//var df1 = DataFrame.LoadCsv("..\\..\\..\\output.csv");
//var df2 = DataFrame.LoadCsv("..\\..\\..\\chiletweets.csv");

//var trainingDf = DataFrame.LoadCsv("..\\..\\..\\dataset\\tweet_sentiment_dataset.csv");
//Open the Directory using a Lucene Directory class
var indexNameTweets = "example_index";
var indexNameTraining = "training_index";

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
//var indexerTraining = new TweetIndexer(luceneVersion, indexNameTraining, new StandardAnalyzer(luceneVersion));
//indexer.AddTweetsToIndex(df2);
//indexerTraining.AddTrainingSetToIndex(df: trainingDf);

/****** SEARCHER ******/
var searcher = new TweetSearcher(indexNameTweets);
var searcherTrainingSet = new TweetSearcher(indexNameTraining);

/****** CLASSIFIER ******/
var nbc = new SimpleNaiveBayesClassifier();

nbc.Train(
    SlowCompositeReaderWrapper.Wrap(searcherTrainingSet.IndexReader), textFieldName:"content",
    classFieldName:"target",
    new StandardAnalyzer(luceneVersion)
    );
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
            DateTime.Parse(t.Get("date")).ToShortDateString(),
            "?"))
        );
    tweets.AsParallel().ForAll(t => t.Content = nbc.AssignClass(t.Content).AssignedClass.Utf8ToString());
    Console.WriteLine("writing");
    using var writer = new StreamWriter(@"..\\..\\..\\ClassifiedTweets.csv");
    using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
    csv.WriteRecords(tweets.ToList());
    csv.Flush();
}
Console.WriteLine("done");
