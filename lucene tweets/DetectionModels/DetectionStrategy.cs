using System.Collections;

namespace lucene_tweets.DetectionModels;

public interface DetectionStrategy
{
    void DetectEmotion(IEnumerable<Tweet> tweets);
    void DetectEmotion(IEnumerable<Lucene.Net.Documents.Document> docs, TweetIndexer indexer);
}