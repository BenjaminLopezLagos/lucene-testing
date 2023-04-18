using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Analyzing;
using Lucene.Net.QueryParsers.Flexible.Standard;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Directory = Lucene.Net.Store.Directory;

namespace lucene_tweets;

public class TweetSearcher
{
    private Directory IndexDirectory { get; }
    public IndexReader IndexReader { get; }
    private IndexSearcher Searcher { get; }

    public TweetSearcher(string indexName)
    {
        var indexPath = Path.Combine(Environment.CurrentDirectory, indexName);
        IndexDirectory = FSDirectory.Open(indexPath);
        IndexReader = DirectoryReader.Open(IndexDirectory);
        Searcher = new IndexSearcher(IndexReader);
    }

    public IList<Document>? SingleTermQuery(string field, string content, int numberOfResults = 5)
    {
        var query = new TermQuery(new Term(field, content));
        var topDocs = Searcher.Search(query, n: numberOfResults); //indicate we want the first n results
        Console.WriteLine($"Matching results: {topDocs.TotalHits}");
        if (topDocs.ScoreDocs.Length < 1)
        {
            Console.WriteLine("No results");
            return null;
        }

        var flag = 0;
        var docList = new List<Document>(numberOfResults);
        foreach (var scoreDoc in topDocs.ScoreDocs)
        {
            var resultDoc = Searcher.Doc(scoreDoc.Doc);
            docList.Add(resultDoc);
            flag++;
            if (flag > numberOfResults)
            {
                break;
            }
        }

        return docList;
    }
    
    public IList<Document>? CustomQuery(string field, string? userQuery, int numberOfResults = 5)
    {
        var queryParser = new AnalyzingQueryParser(LuceneVersion.LUCENE_48, field, new StandardAnalyzer(LuceneVersion.LUCENE_48));
        var query = queryParser.Parse(query: userQuery);
        var topDocs = Searcher.Search(query, n: numberOfResults); //indicate we want the first n results
        Console.WriteLine($"Matching results: {topDocs.TotalHits}");
        if (topDocs.ScoreDocs.Length < 1)
        {
            Console.WriteLine("No results");
            return null;
        }

        var flag = 0;
        var docList = new List<Document>(numberOfResults);
        foreach (var scoreDoc in topDocs.ScoreDocs)
        {
            var resultDoc = Searcher.Doc(scoreDoc.Doc);
            docList.Add(resultDoc);
            flag++;
            if (flag > numberOfResults)
            {
                break;
            }
        }

        return docList;
    }

    public static void PrintResults(IList<Document>? documents)
    {
        if (documents == null || documents.Count < 1)
        {
            return;
        }
        foreach (var doc in documents)
        {
            Console.WriteLine($"Content of tweet: {doc.Get("content")}");
            Console.WriteLine($"Likes of tweet: {doc.Get("likes")}");
            Console.WriteLine($"RTS of tweet: {doc.Get("rts")}");
            Console.WriteLine($"Views of tweet: {doc.Get("views")}");
            Console.WriteLine();
        }
    }
}