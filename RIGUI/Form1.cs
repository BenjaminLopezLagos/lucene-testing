using System.Security;
using lucene_tweets;
using lucene_tweets.DetectionModels;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.Util;

namespace RIGUI
{
    public partial class Form1 : Form
    {
        private double[] pValues = { 1, 1, 1, 1, 1, 1, 1 };
        private double[] nValues = { 0, 0, 0, 0, 0, 0, 0 };
        private IList<Tweet> _tweets;

        public Form1()
        {
            InitializeComponent();
            var mlnet = new MlNetModel(@"..\..\..\..\lucene tweets\DetectionModels\model.zip");
            var sentimentDetector = new SentimentDetection(mlnet);
            const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;
            var indexPath = @"..\..\..\..\lucene tweets\example_index";
            var searcher = new TweetSearcher(indexPath);
            var query = new BooleanQuery();
            query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20220420, max:20220427,true,true), Occur.MUST);
            var resultDocs = searcher.CustomQuery(query, numberOfResults: 10000);
            if (resultDocs != null)
            {
                label3.Text = resultDocs.Count.ToString();
                _tweets = new List<Tweet>(resultDocs.Count);
                resultDocs.ToList().ForEach(x => _tweets.Add(new Tweet(x)));
                sentimentDetector.ExecuteDetector(_tweets);
                _tweets = _tweets.OrderBy(o=>o.TweetContents.Get("date")).ToList();
                var tweetDates = _tweets.Select(
                    x => DateTools.StringToDate(x.TweetContents.Get("date"))
                    ).Distinct().ToList();
                int pointCount = tweetDates.Count();
                var posValues = new List<double>(pointCount);
                var negValues = new List<double>(pointCount);
                foreach (var d in tweetDates)
                {
                    var tweetsInDate = _tweets.Where(x => x.Date == d).ToList();
                    var tweetAmount_n = tweetsInDate.Count;
                    var positives = tweetsInDate.Count(x => x.MlOutput > 0.5)*100 / tweetAmount_n;
                    var negatives = tweetsInDate.Count(x => x.MlOutput < 0.5)*100 / tweetAmount_n;
                    posValues.Add(positives);
                    negValues.Add(negatives);
                }
                // generate sample data (arrays of individual DateTimes and values)
                //double[] values = ScottPlot.DataGen.RandomWalk(rand, pointCount);
                // use LINQ and DateTime.ToOADate() to convert DateTime[] to double[]
                double[] xs = tweetDates.Select(x => x.ToOADate()).ToArray();

                // plot the double arrays using a traditional scatter plot
                formsPlot1.Plot.AddScatter(xs, posValues.ToArray());
                //formsPlot1.Plot.AddScatter(xs, negValues.ToArray());

                // indicate the horizontal axis tick labels should display DateTime units
                formsPlot1.Plot.XAxis.DateTimeFormat(true);
                formsPlot1.Refresh();
            }
        }

        private void EndResponsive()
        {
            if(Width < 500)
            {
                tableLayoutPanel2.ColumnStyles[1].Width = 350;
            } else if (Width < 1280)
            {
                tableLayoutPanel2.ColumnStyles[1].Width = tableLayoutPanel2.Width - (formsPlot1.Width);
            }
            else
            {
                // le falta
                tableLayoutPanel2.ColumnStyles[1].Width = tableLayoutPanel2.Width - (formsPlot1.Width + label10.Width);
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }
    }
}