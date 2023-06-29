using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.En;
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
        tokenStream = new PorterStemFilter(tokenStream);
        tokenStream = new StopFilter(LuceneVersion.LUCENE_48, tokenStream, StopAnalyzer.ENGLISH_STOP_WORDS_SET);
        var attr = tokenStream.GetAttribute<ICharTermAttribute>();
        tokenStream.Reset();
        while (tokenStream.IncrementToken())
        {
            tokens.Add(attr.ToString());
        }

        return string.Join(' ', tokens);
    }
    
    public static IEnumerable<string> GetStemmedTokens(string text)
    {
        var tokens = new List<string>();
        var englishAnalyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
        var tokenStream = englishAnalyzer.GetTokenStream(null, text);
        tokenStream = new PorterStemFilter(tokenStream);
        tokenStream = new StopFilter(LuceneVersion.LUCENE_48, tokenStream, StopAnalyzer.ENGLISH_STOP_WORDS_SET);
        var attr = tokenStream.GetAttribute<ICharTermAttribute>();
        tokenStream.Reset();
        while (tokenStream.IncrementToken())
        {
            tokens.Add(attr.ToString());
        }

        return tokens;
    }
}