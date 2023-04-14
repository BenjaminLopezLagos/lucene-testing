// See https://aka.ms/new-console-template for more information
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;
using Microsoft.Data.Analysis;
using lucene_tweets;
using LuceneDirectory = Lucene.Net.Store.Directory;

// Specify the compatibility version we want
const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;
var df = DataFrame.LoadCsv("..\\..\\..\\chiletweets.csv");
//Open the Directory using a Lucene Directory class
string indexName = "example_index";

/****** INDEXER ******/
//var indexer = new TweetIndexer(luceneVersion, indexName, new StandardAnalyzer(luceneVersion));
//indexer.DeleteCurrentIndex();
//indexer.AddTweetsToIndex(df);

/****** SEARCHER ******/
var searcher = new TweetSearcher(indexName);
var resultDocs = searcher.SingleTermQuery("content", "csm");
searcher.PrintResults(resultDocs);
/*
var qParser = new StandardQueryParser();
Query query = qParser.Parse("+partido", "content");
TopDocs topDocs = searcher.Search(query, n: 2);         //indicate we want the first 2 results
*/