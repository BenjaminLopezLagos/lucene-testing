using Lucene.Net.Util;
using Microsoft.Data.Analysis;
using lucene_tweets;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Classification;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using LuceneDirectory = Lucene.Net.Store.Directory;

// Specify the compatibility version we want
const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;
/****** DATA ******/
var df = DataFrame.LoadCsv("..\\..\\..\\chiletweets.csv");

//Open the Directory using a Lucene Directory class
var indexName = "example_index";

/****** INDEXER ******/
//var indexer = new TweetIndexer(luceneVersion, indexName, new StandardAnalyzer(luceneVersion));
//indexer.DeleteCurrentIndex();
//indexer.AddTweetsToIndex(df);

/****** SEARCHER ******/
var searcher = new TweetSearcher(indexName);
var resultDocs = searcher.SingleTermQuery(field: "content", content:"csm", numberOfResults: 3);
TweetSearcher.PrintResults(resultDocs);


/****** CLASSIFIER ******/
var NBC = new SimpleNaiveBayesClassifier();
NBC.Train(SlowCompositeReaderWrapper.Wrap(searcher.Reader), textFieldName:"", classFieldName:"", new StandardAnalyzer(luceneVersion));
ClassificationResult<BytesRef> classValue = NBC.AssignClass(resultDocs?[0].Get("content"));