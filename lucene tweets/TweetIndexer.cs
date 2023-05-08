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
    public IndexWriter IndexWriter { get; }
    public TweetIndexer(LuceneVersion luceneVersion, string indexName, Analyzer analyzer)
    {
        var indexPath = Path.Combine(Environment.CurrentDirectory, indexName);
        IndexDirectory = FSDirectory.Open(indexPath);
        var indexConfig = new IndexWriterConfig(luceneVersion, analyzer);
        indexConfig.SetOpenMode(OpenMode.CREATE_OR_APPEND);
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
            var date = Helper.GetUntilOrEmpty(row[2].ToString(), " · ");
            var doc = new Document
            {
                new StringField("user", row[0].ToString(), Field.Store.YES),
                new TextField("content", row[1].ToString(), Field.Store.YES),
                new StringField("date", date != null ? DateOnly.Parse(date).ToString() : "-", Field.Store.YES),
            };
            IndexWriter.AddDocument(doc);
        }
        //Flush and commit the index data to the directory
        IndexWriter.Commit();
    }
    
    public void AddTrainingSetToIndex(DataFrame df)
    {
        //Add documents to the index
        foreach(var row in df.Rows)
        {
            var doc = new Document
            {
                new TextField("content", row[5].ToString(), Field.Store.YES),
                new TextField("flag", row[3].ToString(), Field.Store.YES),
                new TextField("target", row[0].ToString(), Field.Store.YES)
            };
            IndexWriter.AddDocument(doc);
        }
        //Flush and commit the index data to the directory
        IndexWriter.Commit();
    }
}

/*
 * Source:
 * https://stackoverflow.com/questions/1857513/get-substring-everything-before-certain-char
 */
static class Helper
{
    public static string GetUntilOrEmpty(this string? text, string stopAt = "-")
    {
        if (!String.IsNullOrWhiteSpace(text))
        {
            int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

            if (charLocation > 0)
            {
                return text.Substring(0, charLocation);
            }
        }

        return String.Empty;
    }
}