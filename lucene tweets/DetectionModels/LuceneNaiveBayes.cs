using Lucene.Net.Analysis.Standard;
using Lucene.Net.Classification;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace lucene_tweets.DetectionModels;

public class LuceneNaiveBayes : DetectionStrategy
{
    
    private Lucene.Net.Store.Directory IndexDirectory { get; }
    private IndexReader IndexReader { get; }
    private SimpleNaiveBayesClassifier Classifier { get; }

    public LuceneNaiveBayes()
    {
        var indexName = "training_index";
        var indexPath = Path.Combine(Environment.CurrentDirectory, indexName);
        IndexDirectory = FSDirectory.Open(indexPath);
        IndexReader = DirectoryReader.Open(IndexDirectory);
        Classifier = new SimpleNaiveBayesClassifier();
        /*
        Classifier.Train(SlowCompositeReaderWrapper.Wrap(reader: IndexReader)
            , "content"
            , "target"
            , new StandardAnalyzer(LuceneVersion.LUCENE_48));
            */
        Console.WriteLine("training nb");
        foreach (AtomicReaderContext context in IndexReader.Leaves)
        {
            Classifier.Train(context.AtomicReader
                , "content"
                , "target"
                , new StandardAnalyzer(LuceneVersion.LUCENE_48));
        }
        Console.WriteLine("training nb done");
    }
    public void DetectEmotion(IEnumerable<Tweet> tweets)
    {
        tweets.ToList().ForEach(x => 
            x.NaiveBayesLabel = Classifier.AssignClass(x.TweetContents.Get("content")).AssignedClass.Utf8ToString());
    }
}