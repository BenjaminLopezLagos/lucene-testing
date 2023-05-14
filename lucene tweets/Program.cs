using System.Runtime.InteropServices;
using System.Runtime.Serialization;
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

var resultsFromQuery = 5000;
var resultDocs = searcher.CustomQuery(query, numberOfResults: resultsFromQuery);
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
    var count = 0;
    DataFrameColumn[] columns = {
        new StringDataFrameColumn("user"),
        new StringDataFrameColumn("content"),
        new PrimitiveDataFrameColumn<DateTime>("date"),
        new StringDataFrameColumn("class")
    };
    var df = new DataFrame(columns);

    foreach (var d in CollectionsMarshal.AsSpan(resultDocs))
    {
        ClassificationResult<BytesRef> classValue = nbc.AssignClass(d.Get("content"));
        Console.WriteLine(count++);
        df.Append(new[]
        {
            new KeyValuePair<string, object>("user", d.Get("user")),
            new KeyValuePair<string, object>("content",d.Get("content").Replace("\n", " ").Replace("\r", " ")),
            new KeyValuePair<string, object>("date", DateTime.Parse(d.Get("date"))),
            new KeyValuePair<string, object>("class", classValue.AssignedClass.Utf8ToString())
        }, inPlace: true);
        Console.WriteLine($"{d.Get("content")} \n {classValue.AssignedClass.Utf8ToString()}"); 
    }
    Console.WriteLine("writing");
    DataFrame.SaveCsv(df, @"..\\..\\..\\ClassifiedTweets.csv");
}
Console.WriteLine("done");
