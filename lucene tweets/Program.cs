using Lucene.Net.Util;
using Microsoft.Data.Analysis;
using lucene_tweets;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Classification;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using LuceneDirectory = Lucene.Net.Store.Directory;

// Specify the compatibility version we want
const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;
/****** DATA ******/
var df1 = DataFrame.LoadCsv("..\\..\\..\\output.csv");
//var df2 = DataFrame.LoadCsv("..\\..\\..\\chiletweets.csv");

//var trainingDf = DataFrame.LoadCsv("..\\..\\..\\dataset\\tweet_sentiment_dataset.csv");
//Open the Directory using a Lucene Directory class
var indexNameTweets = "example_index";
var indexNameTraining = "training_index";

/****** INDEXER ******/
var indexer = new TweetIndexer(luceneVersion, indexNameTweets, new StandardAnalyzer(luceneVersion));
//var indexerTraining = new TweetIndexer(luceneVersion, indexNameTraining, new StandardAnalyzer(luceneVersion));
indexer.DeleteCurrentIndex();
indexer.AddTweetsToIndex(df1);
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

var resultDocs = searcher.CustomQuery(query);
TweetSearcher.PrintResults(resultDocs);

if (resultDocs != null)
{
    foreach (var d in resultDocs)
    {
        ClassificationResult<BytesRef> classValue = nbc.AssignClass(d.Get("content"));
        var docClass = classValue.AssignedClass.Utf8ToString();
        Console.WriteLine($"{d.Get("content")} \n {docClass}"); 
    } 
}
Console.WriteLine("done");
