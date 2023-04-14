using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Index.Extensions;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Data.Analysis;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace lucene_tweets;

public class TweetIndexer
{
    private LuceneDirectory IndexDirectory { get; }
    private IndexWriter IndexWriter { get; }
    public TweetIndexer(LuceneVersion luceneVersion, string indexName, Analyzer analyzer)
    {
        string indexPath = Path.Combine(Environment.CurrentDirectory, indexName);
        IndexDirectory = FSDirectory.Open(indexPath);
        var indexConfig = new IndexWriterConfig(luceneVersion, analyzer);
        indexConfig.SetOpenMode(OpenMode.CREATE);
        IndexWriter = new IndexWriter(IndexDirectory, indexConfig);
    }

    public void DeleteCurrentIndex()
    {
        IndexWriter.DeleteAll();
    }
    
    public void AddTweetsToIndex(DataFrame df)
    {
        //Add documents to the index
        foreach(var row in df.Rows)
        {
            var doc = new Document
            {
                new TextField("content", row[3].ToString(), Field.Store.YES),
                new TextField("likes", row[4].ToString(), Field.Store.YES),
                new TextField("rts", row[5].ToString(), Field.Store.YES),
                new TextField("views", row[6] != null ? row[6].ToString() : "no views", Field.Store.YES)
            };
            IndexWriter.AddDocument(doc);
        }
        //Flush and commit the index data to the directory
        IndexWriter.Commit();
    }
}