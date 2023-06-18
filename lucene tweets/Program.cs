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
using Document = Lucene.Net.Documents.Document;

//testing bert
/*
var testSentence = "lmao elon musk fucking sucks";
var bTokenizer = new BertBaseTokenizer();
var bTokens = bTokenizer.Tokenize(testSentence);
var bEncoded = bTokenizer.Encode(bTokens.Count, testSentence);
var bertInput = new BertInput()
{
    InputIds = bEncoded.Select(t => t.InputIds).ToArray(),
    AttentionMask = bEncoded.Select(t => t.AttentionMask).ToArray(),
    TypeIds = bEncoded.Select(t => t.TokenTypeIds).ToArray()
};
Console.WriteLine(string.Join(", ", bertInput.InputIds));
var modelPath = @"D:\roBERTa models\roberta-base-11.onnx";
var input_ids = BertInput.ConvertToTensor(bertInput.InputIds, bertInput.InputIds.Length);
var attention_mask = BertInput.ConvertToTensor(bertInput.AttentionMask, bertInput.AttentionMask.Length);
var token_type_ids = BertInput.ConvertToTensor(bertInput.TypeIds, bertInput.TypeIds.Length);

var input = new List<NamedOnnxValue>
{
    NamedOnnxValue.CreateFromTensor("input_ids", input_ids),
    NamedOnnxValue.CreateFromTensor("input_mask", attention_mask),
    NamedOnnxValue.CreateFromTensor("segment_ids", token_type_ids)
};
var session = new InferenceSession(modelPath);
var sessionOutput = session.Run(input);
List<float> startLogits = (sessionOutput.ToList().First().Value as IEnumerable<float>).ToList();
List<float> endLogits = (sessionOutput.ToList().Last().Value as IEnumerable<float>).ToList();
var startIndex = startLogits.ToList().IndexOf(startLogits.Max());
var endIndex = endLogits.ToList().IndexOf(endLogits.Max());
var predictedTokens = bTokens.Skip(startIndex).Take(endIndex + 1 - startIndex)
    .Select(o => bTokenizer.IdToToken((int)o.VocabularyIndex))
    .ToList();
*/
// testing a different way
var mlnet = new MlNetModel();
var testInput = new SentimentInput()
{
    Sentence = "lol elon fucking sucks"
};
var testOutput = mlnet.Engine.Predict(testInput);
Console.WriteLine(testOutput.Label);
// Specify the compatibility version we want
const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;
Console.Read();
//Open the Directory using a Lucene Directory class
var indexNameTweets = "example_index";
var trainingIndexNb = "training_index";

/****** INDEXER ******/
var indexer = new TweetIndexer(luceneVersion, indexNameTweets, new StandardAnalyzer(luceneVersion));
var filePaths = Directory.GetFiles("D:\\snscrape_tweets\\dask results\\results");
/*
indexer.DeleteCurrentIndex();
Console.WriteLine("indexing");
foreach (var f in filePaths)
{
    var dfForIndex = DataFrame.LoadCsv(f);
    indexer.AddTweetsToIndex(dfForIndex);
}
Console.WriteLine("indexing done");
*/
/****** SEARCHER ******/
var searcher = new TweetSearcher(indexNameTweets);

/****** CLASSIFIER ******/
var vader = new Vader();
var sentimentDetector = new SentimentDetection(vader);

var query = new BooleanQuery();
/*
query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);
query.Add(new TermQuery(new Term("content", "blue")), Occur.SHOULD);
query.Add(new TermQuery(new Term("user", "danspena")), Occur.MUST_NOT);
query.Add(new FuzzyQuery(new Term("content", "via ~")), Occur.MUST_NOT);
query.Add(new WildcardQuery(new Term("content", "#*news")), Occur.MUST_NOT);
*/
query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);
query.Add(new TermQuery(new Term("content", "api")), Occur.MUST);
query.Add(new FuzzyQuery(new Term("content", "pa~3")), Occur.SHOULD);
query.Add(new TermQuery(new Term("user", "danspena")), Occur.MUST_NOT);
query.Add(new FuzzyQuery(new Term("content", "#BREAKING~1")), Occur.MUST_NOT);
query.Add(new WildcardQuery(new Term("content", "#*news")), Occur.MUST_NOT);
query.Add(new WildcardQuery(new Term("content", "?businessinsider")), Occur.MUST_NOT);
query.Add(new WildcardQuery(new Term("content", "#*news")), Occur.MUST_NOT);
query.Add(new TermQuery(new Term("content", "gHacks Tech News ")), Occur.MUST_NOT);


//query.Add(new FuzzyQuery(new Term("content", "fail~")), Occur.MUST);
//query.Add(NumericRangeQuery.NewInt32Range(field:"views",min:0, max:100,true,true), Occur.MUST);
//query.Add(new WildcardQuery(new Term("content", "c*m")), Occur.MUST);

//var query = new MatchAllDocsQuery();
var date1 = "20230506";
var date2 = "20230606";
var strQuery = $"[{date1} TO {date2}]";
var resultDocs = searcher.CustomQuery(query, numberOfResults: 500);
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
    /*
    var docConcurrentBag = new ConcurrentBag<Document>(resultDocs);
    var tweets = new ConcurrentBag<Tweet>();
    docConcurrentBag.AsParallel().ForAll(t =>
        tweets.Add(new Tweet(t.Get("user"),
            $"'{t.Get("content").Replace("\n", " ").Replace("\r", " ")}'",
            DateTime.Parse(t.Get("date")).ToShortDateString()))
    );
    */
    var tweets = new List<Tweet>(resultDocs.Count);
    resultDocs.ToList().ForEach(x => tweets.Add(new Tweet(x)));
    Console.WriteLine("classifying with model 1");
    sentimentDetector.ExecuteDetector(tweets);
    Console.WriteLine("classifying with model 2");
    Console.WriteLine("writing");
    //using var writer = new StreamWriter(@"..\\..\\..\\ClassifiedTweets.csv");
    //using var csv = new CsvWriter(writer,  new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";"});
    //csv.WriteRecords(tweets.ToList().OrderByDescending(x => x.MlOutput.Sentiment));
    //csv.Flush();
    tweets.ForEach(Console.WriteLine);
    Console.WriteLine($"Positive: {tweets.Count(x => x.VaderScore > 0)*100 / tweets.Count}%");
    Console.WriteLine($"Negative: {tweets.Count(x => x.VaderScore < 0)*100 / tweets.Count}%");
    Console.WriteLine($"Neutral: {tweets.Count(x => x.VaderScore == 0)*100 / tweets.Count}%");
}

Console.WriteLine("done");
