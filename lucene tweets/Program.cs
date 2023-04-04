// See https://aka.ms/new-console-template for more information
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Lucene.Net.QueryParsers;
using Microsoft.Data.Analysis;
using System.Diagnostics;
using LuceneDirectory = Lucene.Net.Store.Directory;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.QueryParsers.Flexible.Standard;

Console.WriteLine("Hello, World!");
var DF = DataFrame.LoadCsv("..\\..\\..\\chiletweets.csv");
// Specify the compatibility version we want
const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;

//Open the Directory using a Lucene Directory class
string indexName = "example_index";
string indexPath = Path.Combine(Environment.CurrentDirectory, indexName);

using LuceneDirectory indexDir = FSDirectory.Open(indexPath);

//Create an index writer
IndexWriterConfig indexConfig = new IndexWriterConfig(luceneVersion, new StandardAnalyzer(luceneVersion));
IndexWriter writer = new IndexWriter(indexDir, indexConfig);

//Add documents to the index
foreach(var row in DF.Rows)
{
    Document doc = new Document();
    doc.Add(new TextField("content", row[3].ToString(), Field.Store.YES));
    doc.Add(new TextField("likes", row[4].ToString(), Field.Store.YES));
    doc.Add(new TextField("rts", row[5].ToString(), Field.Store.YES));
    doc.Add(new TextField("views", row[6] != null ? row[6].ToString() : "no views", Field.Store.YES));
    writer.AddDocument(doc);
}
//Flush and commit the index data to the directory
writer.Commit();


using DirectoryReader reader = DirectoryReader.Open(indexDir);
IndexSearcher searcher = new IndexSearcher(reader);

/*
Query query = new TermQuery(new Term("content", "good"));
TopDocs topDocs = searcher.Search(query, n: 2);         //indicate we want the first 2 results
*/

var qParser = new StandardQueryParser();
Query query = qParser.Parse("+partido", "content");
TopDocs topDocs = searcher.Search(query, n: 2);         //indicate we want the first 2 results

Document resultDoc = searcher.Doc(topDocs.ScoreDocs[0].Doc);  //read back first doc from results (ie 0 offset)

Console.WriteLine($"Matching results: {topDocs.TotalHits}");
Console.WriteLine($"Content of first result: {resultDoc.Get("content")}");
Console.WriteLine($"Likes of first result: {resultDoc.Get("likes")}");
Console.ReadLine();