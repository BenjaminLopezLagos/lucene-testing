using System.Security;
using KnowledgePicker.WordCloud;
using KnowledgePicker.WordCloud.Coloring;
using KnowledgePicker.WordCloud.Drawing;
using KnowledgePicker.WordCloud.Layouts;
using KnowledgePicker.WordCloud.Primitives;
using KnowledgePicker.WordCloud.Sizers;
using lucene_tweets;
using lucene_tweets.DetectionModels;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.Util;
using static Lucene.Net.Queries.Function.ValueSources.MultiFunction;
using ScottPlot;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace RIGUI
{
    public partial class Form1 : Form
    {
        private double[] pValues = { 1, 1, 1, 1, 1, 1, 1 };
        private double[] nValues = { 0, 0, 0, 0, 0, 0, 0 };
        private IList<Document>? _tweets;
        private int _positiveTweetsAmount = 0;
        private int _negativeTweetsAmount = 0;

        public Form1()
        {
            InitializeComponent();
            label1.Text = "Loading dashboard...";
            label4.Text = "Unavailable";
            label6.Text = "Unavailable";
            formsPlot1.Plot.Style(Style.Blue1);
            formsPlot1.Refresh();

            formsPlot2.Plot.Style(Style.Blue1);
            formsPlot2.Refresh();

            Task.Run(LoadResults);
        }
        private async Task LoadResults()
        {
            var mlnet = new MlNetModel(@"..\..\..\..\lucene tweets\DetectionModels\model.zip");
            var sentimentDetector = new SentimentDetection(mlnet);
            var indexPath = @"..\..\..\..\lucene tweets\ClassifiedTweetsIndex";
            var searcher = new TweetSearcher(indexPath);
            var query = new MatchAllDocsQuery();
            _tweets = searcher.CustomQuery(query, numberOfResults: 30000);
            if (_tweets != null)
            {
                _tweets = _tweets.OrderBy(o=>o.Get("date")).ToList();
                label3.Text = _tweets.Count.ToString();
                await Task.Run(UpdatePosNegTweetAmmount);
                var tasks = new[]
                {
                    Task.Run(GenerateOverallPolarity),
                    Task.Run(GeneratePlot_PolarityChanges),
                    Task.Run(GenerateWordCloud_PositiveTweets),
                    Task.Run(GenerateWordCloud_NegativeTweets),
                };
                Task.WaitAll(tasks);
                label1.Text = "DASHBOARD";
            }
        }
        
        private async Task UpdatePosNegTweetAmmount()
        {
            await Task.Run(() =>
            {
                _positiveTweetsAmount = _tweets.Count(x => 
                    double.Parse(x.Get("mloutput").Replace(',', '.')) > 0.5);
                _negativeTweetsAmount = _tweets.Count(x => 
                    double.Parse(x.Get("mloutput").Replace(',', '.')) < 0.5);
                label4.Text = _positiveTweetsAmount.ToString();
                label6.Text = _negativeTweetsAmount.ToString();
            });
        }
        /*
        private async Task TransformDocsToTweets(SentimentDetection detector,IEnumerable<Lucene.Net.Documents.Document> docs)
        {
            await Task.Run(() =>
            {
                _tweets = new List<Tweet>(docs.Count());
                docs.ToList().ForEach(x => _tweets.Add(new Tweet(x)));
                detector.ExecuteDetector(_tweets);
                _tweets = _tweets.OrderBy(o=>o.TweetContents.Get("date")).ToList();
            });
        }
        */

        private async Task GenerateOverallPolarity()
        {
            await Task.Run(() => 
            {
                var values = new double[] { _positiveTweetsAmount, _negativeTweetsAmount };
                var labels = new string[] { "Positives", "Negatives" };
                var pie = formsPlot2.Plot.AddPie(values);
                pie.SliceLabels = labels;
                pie.ShowPercentages = true;
                formsPlot2.Plot.Legend();
                formsPlot2.Refresh();
            });
        }

        private async Task GeneratePlot_PolarityChanges()
        {
            await Task.Run(() =>
            {
                var tweetDates = _tweets.Select(
                    x => DateTools.StringToDate(x.Get("date"))
                ).Distinct().ToList();
                int pointCount = tweetDates.Count();
                var posValues = new List<double>(pointCount);
                var negValues = new List<double>(pointCount);
                foreach (var d in tweetDates)
                {
                    var tweetsInDate = _tweets.Where(x => DateTools.StringToDate(x.Get("date")) == d).ToList();
                    var tweetAmount_n = tweetsInDate.Count;
                    var positives = 
                        tweetsInDate.Count(x => double.Parse(x.Get("mloutput").Replace(',', '.')) > 0.5) * 100 / tweetAmount_n;

                    var negatives = 
                        tweetsInDate.Count(x => double.Parse(x.Get("mloutput").Replace(',', '.')) < 0.5) * 100 / tweetAmount_n;
                    posValues.Add(positives);
                    negValues.Add(negatives);
                }
                // generate sample data (arrays of individual DateTimes and values)
                //double[] values = ScottPlot.DataGen.RandomWalk(rand, pointCount);
                // use LINQ and DateTime.ToOADate() to convert DateTime[] to double[]
                double[] xs = tweetDates.Select(x => x.ToOADate()).ToArray();

                // plot the double arrays using a traditional scatter plot
                formsPlot1.Plot.AddScatter(xs, posValues.ToArray());
                formsPlot1.Plot.SetAxisLimits(yMin: 0, yMax: 100);
                //formsPlot1.Plot.AddScatter(xs, negValues.ToArray());

                // indicate the horizontal axis tick labels should display DateTime units
                formsPlot1.Plot.XAxis.DateTimeFormat(true);
                formsPlot1.Refresh();
            });
        }
        
        /*
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
        */

        private async Task GenerateWordCloud_PositiveTweets()
        {
            await Task.Run(() =>
            {
                if (_tweets != null)
                {
                    var positiveDocs = _tweets.Where(x => double.Parse(x.Get("mloutput")) > 0.5);
                    var tf = TermFrequency.GetTermFrequency(positiveDocs)
                        .Take(300).ToDictionary(x => x.Key, x => x.Value);
                    const int k = 2; // scale
                    var wordCloud = new WordCloudInput(
                        tf.Select(p => new WordCloudEntry(p.Key, p.Value)))
                    {
                        Width = 1024 * k,
                        Height = 256 * k,
                        MinFontSize = 8 * k,
                        MaxFontSize = 32 * k
                    };
                    var sizer = new LogSizer(wordCloud);
                    using var engine = new SkGraphicEngine(sizer, wordCloud);
                    var layout = new SpiralLayout(wordCloud);
                    var colorizer = new RandomColorizer(); // optional
                    var wcg = new WordCloudGenerator<SKBitmap>(wordCloud, engine, layout, colorizer);

                    // Draw the bitmap on white background.
                    using var final = new SKBitmap(wordCloud.Width, wordCloud.Height);
                    using var canvas = new SKCanvas(final);
                    canvas.Clear(SKColors.White);
                    using var bitmap = wcg.Draw();
                    canvas.DrawBitmap(bitmap, 0, 0);

                    // Save to PNG.
                    using var data = final.Encode(SKEncodedImageFormat.Png, 100);
                    using var writer = File.Create(@"..\..\..\output_pos.png");
                    data.SaveTo(writer);
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox1.Image = final.ToBitmap();
                }
            });
        }
        private async Task GenerateWordCloud_NegativeTweets()
        {
            await Task.Run(() =>
            {
                if (_tweets != null)
                {
                    var positiveDocs = _tweets.Where(x => double.Parse(x.Get("mloutput")) < 0.5);
                    var tf = TermFrequency.GetTermFrequency(positiveDocs)
                        .Take(300).ToDictionary(x => x.Key, x => x.Value);
                    const int k = 2; // scale
                    var wordCloud = new WordCloudInput(
                        tf.Select(p => new WordCloudEntry(p.Key, p.Value)))
                    {
                        Width = 1024 * k,
                        Height = 256 * k,
                        MinFontSize = 8 * k,
                        MaxFontSize = 32 * k
                    };
                    var sizer = new LogSizer(wordCloud);
                    using var engine = new SkGraphicEngine(sizer, wordCloud);
                    var layout = new SpiralLayout(wordCloud);
                    var colorizer = new RandomColorizer(); // optional
                    var wcg = new WordCloudGenerator<SKBitmap>(wordCloud, engine, layout, colorizer);

                    // Draw the bitmap on white background.
                    using var final = new SKBitmap(wordCloud.Width, wordCloud.Height);
                    using var canvas = new SKCanvas(final);
                    canvas.Clear(SKColors.White);
                    using var bitmap = wcg.Draw();
                    canvas.DrawBitmap(bitmap, 0, 0);

                    // Save to PNG.
                    using var data = final.Encode(SKEncodedImageFormat.Png, 100);
                    using var writer = File.Create(@"..\..\..\output_neg.png");
                    data.SaveTo(writer);
                    pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox2.Image = final.ToBitmap();
                }
            });
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }
    }
}