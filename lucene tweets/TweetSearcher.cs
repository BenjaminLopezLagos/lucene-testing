using J2N.Collections.Generic.Extensions;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Flexible.Standard;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Directory = Lucene.Net.Store.Directory;

namespace lucene_tweets;

public class TweetSearcher
{
    private Directory IndexDirectory { get; }
    private DirectoryReader Reader { get; }
    private IndexSearcher Searcher { get; }

    public TweetSearcher(string indexName)
    {
        string indexPath = Path.Combine(Environment.CurrentDirectory, indexName);
        IndexDirectory = FSDirectory.Open(indexPath);
        Reader = DirectoryReader.Open(IndexDirectory);
        Searcher = new IndexSearcher(Reader);
    }

    public IList<Document>? SingleTermQuery(string field, string content, int numberOfResults = 5)
    {
        var query = new TermQuery(new Term(field, content));
        TopDocs topDocs = Searcher.Search(query, n: numberOfResults); //indicate we want the first 2 results
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
            Document resultDoc = Searcher.Doc(scoreDoc.Doc); //read back first doc from results (ie 0 offset)
            docList.Add(resultDoc);
            flag++;
            if (flag > numberOfResults)
            {
                break;
            }
        }

        return docList;
    }

    public void PrintResults(IList<Document>? documents)
    {
        if ((documents == null) || (documents.Count < 1))
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