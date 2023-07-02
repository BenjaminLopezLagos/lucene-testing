using Lucene.Net.Index;
using Lucene.Net.Search;
using lucene_tweets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RIGUI
{
    public partial class MainMenu : Form
    {
        IList<Lucene.Net.Documents.Document>? _tweets;
        BooleanQuery? _query;
        TweetSearcher _tweetSearcher = new TweetSearcher(@"..\..\..\..\lucene tweets\ClassifiedTweetsIndex");
        public MainMenu()
        {
            InitializeComponent();
        }

        private async void btnDashboard_Click(object sender, EventArgs e)
        {
            LoadTweets();
            if(_tweets != null)
            {
                var dashboardForm = new Form1(_tweets);
                dashboardForm.ShowDialog();
                await Task.Run(dashboardForm.LoadResults);
            }
        }

        private void LoadTweets()
        {
            if (_query != null)
            {
                _tweets = _tweetSearcher.CustomQuery(_query, numberOfResults: 30000);
            }
        }

        private void btnEvent1_Click(object sender, EventArgs e)
        {
            _query = new BooleanQuery();
            // elon offers to buy Twitter at $54.20
            _query.Add(new TermQuery(new Term("content", "elon")), Occur.MUST);
            _query.Add(NumericRangeQuery.NewInt64Range(field: "date", min: 20220414, max: 20220416, true, true), Occur.MUST);
        }

        private void btnEvent2_Click(object sender, EventArgs e)
        {
            _query = new BooleanQuery();
            // The social media platform accepts Musk's offer and announces the deal valuation at $44 billion.
            _query.Add(new TermQuery(new Term("content", "elon")), Occur.MUST);
            _query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20220425, max:20220427,true,true), Occur.MUST);

        }

        private void btnEvent3_Click(object sender, EventArgs e)
        {
            _query = new BooleanQuery();
            // Musk says that the Twitter deal is ‘temporarily on hold’. He cites the prevalence of bots and spam accounts on the microblogging platform.
            _query.Add(new TermQuery(new Term("content", "elon")), Occur.MUST);
            _query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20220513, max:20220515,true,true), Occur.MUST);
        }

        private void btnEvent4_Click(object sender, EventArgs e)
        {
            _query = new BooleanQuery();
            // Twitter shareholders bring class-action lawsuit against Musk over alleged stock manipulation tied to the acquisition process.
            _query.Add(new TermQuery(new Term("content", "elon")), Occur.MUST);
            _query.Add(NumericRangeQuery.NewInt64Range(field: "date", min: 20220526, max: 20220529, true, true), Occur.MUST);

        }

        private void btnEvent5_Click(object sender, EventArgs e)
        {

        }

        private void btnEvent6_Click(object sender, EventArgs e)
        {

        }

        private void btnEvent7_Click(object sender, EventArgs e)
        {

        }

        private void btnEvent8_Click(object sender, EventArgs e)
        {

        }

        private void btnEvent9_Click(object sender, EventArgs e)
        {

        }

        private void btnEvent10_Click(object sender, EventArgs e)
        {

        }

        private void btnEvent11_Click(object sender, EventArgs e)
        {

        }

        private void btnEvent12_Click(object sender, EventArgs e)
        {

        }

        private void btnEvent13_Click(object sender, EventArgs e)
        {

        }

        private void btnEvent14_Click(object sender, EventArgs e)
        {

        }

        private void btnEvent15_Click(object sender, EventArgs e)
        {

        }
    }
}
