using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using BERTTokenizers;
using Lucene.Net.Util;
using Microsoft.Data.Analysis;
using lucene_tweets;
using lucene_tweets.DetectionModels;
using Lucene.Net.Analysis.En;
using Lucene.Net.Analysis.Hunspell;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Classification;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Queries.Function;
using Lucene.Net.Search;
using Microsoft.ML;
using Microsoft.ML.TorchSharp;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.TorchSharp.NasBert;
using LuceneDirectory = Lucene.Net.Store.Directory;
using Lucene.Net.Documents;

var mlnet = new MlNetModel($@"..\..\..\DetectionModels\model.zip");
var testInput = new SentimentInput()
{
    Sentence = "Juuuuuuuuuuuuuuuuussssst Chillin!!"
};
var testOutput = mlnet.Engine.Predict(testInput);
Console.WriteLine($"label: {testOutput.Label} | predicted: {testOutput.PredictedLabel}");

// Specify the compatibility version we want
const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;
//Open the Directory using a Lucene Directory class
var indexNameTweets = "example_index";
var trainingIndexNb = "training_index";

/****** INDEXER ******/
var tweetIndexer = new TweetIndexer(luceneVersion, $@"..\..\..\{indexNameTweets}", new StandardAnalyzer(luceneVersion));
//var filePaths = Directory.GetFiles("D:\\snscrape_tweets\\dask results\\results");
/*
tweetIndexer.DeleteCurrentIndex();
Console.WriteLine("indexing");
foreach (var f in filePaths)
{
    var dfForIndex = DataFrame.LoadCsv(f);
    indexer.AddTweetsToIndex(dfForIndex);
}
Console.WriteLine("indexing done");
*/
/****** SEARCHER ******/
var searcher = new TweetSearcher($@"..\..\..\{indexNameTweets}");

/****** CLASSIFIER ******/
var vader = new Vader();
var sentimentDetector = new SentimentDetection(mlnet);

var query = new BooleanQuery();
query.Add(new TermQuery(new Term("content", "elon")), Occur.MUST);

// elon offers to buy Twitter at $54.20
//query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20220414, max:20220416,true,true), Occur.MUST);

// The social media platform accepts Musk's offer and announces the deal valuation at $44 billion.
//query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20220425, max:20220427,true,true), Occur.MUST);

// Musk says that the Twitter deal is ‘temporarily on hold’. He cites the prevalence of bots and spam accounts on the microblogging platform.
//query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20220513, max:20220515,true,true), Occur.MUST);

// Twitter shareholders bring class-action lawsuit against Musk over alleged stock manipulation tied to the acquisition process.
//query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20220526, max:20220529,true,true), Occur.MUST);

// Musk threatens to withdraw from the deal if the social media giant does not disclose information about bots and spams on the platform.
//query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20220606, max:20220608,true,true), Occur.MUST);
//query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);

// Musk moves to terminate his deal on issue of fake accounts
//query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20220708, max:20220710,true,true), Occur.MUST);
//query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);

// Twitter sues Musk in Delaware court to force him to complete the deal.
//query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20220712, max:20220714,true,true), Occur.MUST);
//query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);

// Musk challenges former Twitter CEO Parag Agrawal to a public debate about spam accounts
// and polls followers on whether they believe less than 5% of Twitter’s daily active users are fake.
//query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20220806, max:20220810,true,true), Occur.MUST);
//query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);

// Musk submits a proposal to move forward with the acquisition at the originally
// agreed-upon price of $44 billion ($54.20 per share) on the condition that Twitter
// drops its lawsuit. Elon's larger goal is to create X.
//query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20221004, max:20221006,true,true), Occur.MUST);
//query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);

// According to a report in The Washington Post, Musk is telling investors he plans
// to terminate nearly 75% of Twitter’s staff.
//query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20221020, max:20221023,true,true), Occur.MUST);
//query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);
//query.Add(new TermQuery(new Term("content", "crypto")), Occur.MUST_NOT);
//query.Add(new TermQuery(new Term("content", "doge")), Occur.MUST_NOT);

// Elon enters twitter with a sink.
// Then closes deal with twitter.
// to terminate nearly 75% of Twitter’s staff.
//query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20221026, max:20221029,true,true), Occur.MUST);
//query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);
//query.Add(new TermQuery(new Term("content", "crypto")), Occur.MUST_NOT);
//query.Add(new TermQuery(new Term("content", "doge")), Occur.MUST_NOT);

// Elon plans to add a new verification system.
// Then confirms that it will cost 8 bucks.
//query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20221030, max:20221103,true,true), Occur.MUST);
//query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);
//query.Add(new TermQuery(new Term("content", "crypto")), Occur.MUST_NOT);
//query.Add(new TermQuery(new Term("content", "doge")), Occur.MUST_NOT);

// Elon fires half of his staff.
// He did it in order to cut costs due to massive drop in revenue.
//query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20221104, max:20221108,true,true), Occur.MUST);
//query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);
//query.Add(new TermQuery(new Term("content", "crypto")), Occur.MUST_NOT);
//query.Add(new TermQuery(new Term("content", "doge")), Occur.MUST_NOT);

// Twitter BLUE BEGINS.
// ... and then people start impersonating brands.
// During this time, elon talks about possible bankruptcy.
//query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20221109, max:20221112,true,true), Occur.MUST);
//query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);
//query.Add(new TermQuery(new Term("content", "crypto")), Occur.MUST_NOT);
//query.Add(new TermQuery(new Term("content", "doge")), Occur.MUST_NOT);

// Musk fires more twitter staff; supossedly 80%.
// Might delete this one later (((( REMINDER )))).
//query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20221112, max:20221114,true,true), Occur.MUST);
//query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);
//query.Add(new TermQuery(new Term("content", "staff")), Occur.SHOULD);
//query.Add(new FuzzyQuery(new Term("content", "employee")), Occur.SHOULD);
//query.Add(new TermQuery(new Term("content", "crypto")), Occur.MUST_NOT);
//query.Add(new TermQuery(new Term("content", "doge")), Occur.MUST_NOT);

// Musk send an ultimatum to staff, asking to work long hours or leave.
//query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20221116, max:20221118,true,true), Occur.MUST);
//query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);
//query.Add(new TermQuery(new Term("content", "staff")), Occur.SHOULD);

// NOW ADDING TWITTER API STUFF


// TERMS TO ALWAYS AVOID
query.Add(new TermQuery(new Term("user", "ElonAnnounces")), Occur.MUST_NOT);
query.Add(new FuzzyQuery(new Term("content", "Breaking")), Occur.MUST_NOT);
query.Add(new FuzzyQuery(new Term("content", "BreakingNews")), Occur.MUST_NOT);
query.Add(new FuzzyQuery(new Term("content", "In a recent interview")), Occur.MUST_NOT);
query.Add(new FuzzyQuery(new Term("user", "breaking"), 2, 4), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("content", "BREAKING:")), Occur.MUST_NOT);
query.Add(new FuzzyQuery(new Term("user", "news"),  2, 2), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("content", "LATEST:")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "DefiCinema")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "movierecite")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "kosmatos")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "africansinnews")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "SobhitSinha")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "CryptonewZi")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "StockandmoreCom")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "themetav3rse")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "MorningBrew")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "VisitoryNews")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "TraderMarcoCost")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "marketexplainor")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "KMJNOW")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "theblaze")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "UrbanRecorderPk")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "Raw_News1st")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "Andrey_Daniel_B")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "macronewswire")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "BoricuaEnMaui")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "BradHound")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "PBandJaimie")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "breakingmkts")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "faststocknewss")), Occur.MUST_NOT);
query.Add(new FuzzyQuery(new Term("content", "What You Need to Know - CNET"),  2, 2), Occur.MUST_NOT);
query.Add(new FuzzyQuery(new Term("user", "cgtnafrica"),  2, 2), Occur.MUST_NOT);
query.Add(new FuzzyQuery(new Term("user", "Taipan57602002"),  2, 2), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "CoinDiscoveryy")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "Claire__James")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("content", "@realDailyWire")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "StockMKTNewz")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "W3B_news")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("content", "SCOOP")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "0xthefutureis")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "delete75522330")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("content", "bloomberg")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("content", "Cryptocurrency Price")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("user", "EmpiricalNooz")), Occur.MUST_NOT);

/*
query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);
query.Add(new TermQuery(new Term("content", "blue")), Occur.SHOULD);
query.Add(new TermQuery(new Term("user", "danspena")), Occur.MUST_NOT);
query.Add(new FuzzyQuery(new Term("content", "via ~")), Occur.MUST_NOT);
query.Add(new WildcardQuery(new Term("content", "#*news")), Occur.MUST_NOT);
*/

/*
query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);
query.Add(new TermQuery(new Term("content", "api")), Occur.MUST);
query.Add(new FuzzyQuery(new Term("content", "pa~3")), Occur.SHOULD);
query.Add(new TermQuery(new Term("user", "danspena")), Occur.MUST_NOT);
query.Add(new FuzzyQuery(new Term("content", "#BREAKING~1")), Occur.MUST_NOT);
query.Add(new WildcardQuery(new Term("content", "#*news")), Occur.MUST_NOT);
query.Add(new WildcardQuery(new Term("content", "?businessinsider")), Occur.MUST_NOT);
query.Add(new WildcardQuery(new Term("content", "#*news")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("content", "gHacks Tech News ")), Occur.MUST_NOT);
*/

//query.Add(new FuzzyQuery(new Term("content", "fail~")), Occur.MUST);
//query.Add(NumericRangeQuery.NewInt32Range(field:"views",min:0, max:100,true,true), Occur.MUST);
//query.Add(new WildcardQuery(new Term("content", "c*m")), Occur.MUST);

//var query = new MatchAllDocsQuery();
var resultDocs = searcher.CustomQuery(query, numberOfResults: 1);
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
    var classifiedTweetsPath = "ClassifiedTweetsIndex";
    var classifiedTweetsIndexer  = new TweetIndexer(
        luceneVersion,
        $@"..\..\..\{classifiedTweetsPath}",
        new StandardAnalyzer(luceneVersion));
    //classifiedTweetsIndexer.DeleteCurrentIndex();
    //sentimentDetector.ExecuteDetector(resultDocs, classifiedTweetsIndexer);
    var classifiedTweetsSearcher = new TweetSearcher($@"..\..\..\{classifiedTweetsPath}");
    var newResults = classifiedTweetsSearcher.CustomQuery(query, 5000);
    newResults = newResults.OrderByDescending(x => x.Get("date")).ToList();
    newResults.ToList().ForEach(x => Console.WriteLine($"{x.Get("user")}" + "\n" +
                                                       $"{x.Get("content")}" + "\n" +
                                                       $"{DateTools.StringToDate(x.Get("date")).ToShortDateString()}" + "\n" +
                                                       $"{x.Get("mloutput")}"));

    /*
    resultDocs.ToList().ForEach(x => tweets.Add(new Tweet(x)));
    Console.WriteLine("classifying with model 1");
    sentimentDetector.ExecuteDetector(tweets);
    Console.WriteLine("classifying with model 2");
    //sentimentDetector.ChangeStrategy(mlnet);
    //sentimentDetector.ExecuteDetector(tweets);
    //Console.WriteLine("writing");
    //using var writer = new StreamWriter(@"..\\..\\..\\ClassifiedTweets.csv");
    //using var csv = new CsvWriter(writer,  new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";"});
    //csv.WriteRecords(tweets.ToList().OrderByDescending(x => x.MlOutput));
    //csv.Flush();
    tweets.OrderBy(o=>o.TweetContents.Get("date")).ToList().ForEach(Console.WriteLine);
    */
    Console.WriteLine("ML.Net Results");
    var pos = newResults.Count(x => 
        double.Parse(x.Get("mloutput").Replace(',', '.')) > 0.5) * 100 / newResults.Count;
    var neg = newResults.Count(x => 
        double.Parse(x.Get("mloutput").Replace(',', '.')) < 0.5) * 100 / newResults.Count;
    Console.WriteLine($"Positive: {pos}%");
    Console.WriteLine($"Negative: {neg}%");
    Console.WriteLine();
}

Console.WriteLine("done");
