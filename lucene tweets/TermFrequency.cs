using Lucene.Net.Analysis.Hunspell;

namespace lucene_tweets;

public class TermFrequency
{
    public static List<KeyValuePair<string, int>> GetTermFrequency(IEnumerable<Lucene.Net.Documents.Document> docs)
    {
        var tfDict = new Dictionary<string, int>();
        foreach (var doc in docs)
        {
            var tokens = Stemmer.GetStemmedTokens(doc.Get("content"));
            foreach (var token in tokens)
            {
                if (tfDict.ContainsKey(token))
                {
                    tfDict[token] += 1;
                }
                else
                {
                    tfDict.Add(token, 0);
                }
            }
        }
        return tfDict.OrderByDescending(x => x.Value).ToList();
    }
}