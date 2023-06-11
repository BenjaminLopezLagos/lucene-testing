using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Util;

namespace lucene_tweets;

public class Stemmer
{
    public static string StemInput(string text)
    {
        var tokens = new List<string>();
        var englishAnalyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
        var tokenStream = englishAnalyzer.GetTokenStream(null, text);
        var attr = tokenStream.GetAttribute<ICharTermAttribute>();
        tokenStream.Reset();
        while (tokenStream.IncrementToken())
        {
            tokens.Add(attr.ToString());
        }

        return string.Join(' ', tokens);
    }
}