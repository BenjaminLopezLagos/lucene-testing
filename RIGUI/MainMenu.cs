using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Documents;
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
        string _eventString = "No event.";
        Form1? _form1;
        public MainMenu()
        {
            InitializeComponent();
        }

        // couldn't figure out how to execute this in a different thread without
        // the program commiting suicide.
        private void LoadTweetsToDataGridView()
        {
            if(_tweets != null && _tweets.Count > 0)
            {
                foreach (var tweet in _tweets)
                {
                    DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                    row.Cells[0].Value = tweet.Get("user");
                    row.Cells[1].Value = tweet.Get("content");
                    row.Cells[2].Value = DateTools.StringToDate(tweet.Get("date")).ToShortDateString();
                    var mlout = double.Parse(tweet.Get("mloutput").Replace(',', '.'));
                    if (mlout > 0.5)
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = Color.LightSalmon;
                    }
                    dataGridView1.Rows.Add(row);
                }
            }
        }

        private async void btnDashboard_Click(object sender, EventArgs e)
        {
            LoadTweets();
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Rows.Clear();
            }
            if (_tweets != null)
            {
                
                LoadTweetsToDataGridView();
                dataGridView1.Visible = true;
                _form1 = new Form1(_tweets);
                await Task.Run(_form1.LoadResults);
                _form1.label8.Text = _eventString;
                dataGridView1.Visible = true;
                _form1.ShowDialog();
            }
        }

        private void LoadTweets()
        {
            if (_query != null)
            {
                _tweets = _tweetSearcher.CustomQuery(_query, numberOfResults: 300000);
            }
        }

        private void btnEvent1_Click(object sender, EventArgs e)
        {
            _query = new BooleanQuery();
            AddTermsToAvoid(_query);
            _eventString = "elon offers to buy Twitter at $54.20";
            _query.Add(new TermQuery(new Term("content", "elon")), Occur.MUST);
            _query.Add(NumericRangeQuery.NewInt64Range(field: "date", min: 20220414, max: 20220424, true, true), Occur.MUST);
        }

        private void btnEvent2_Click(object sender, EventArgs e)
        {
            _query = new BooleanQuery();
            AddTermsToAvoid(_query);
            _eventString = "The social media platform accepts Musk's offer and announces the deal valuation at $44 billion.";
            _query.Add(new TermQuery(new Term("content", "elon")), Occur.MUST);
            _query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20220425, max:20220427,true,true), Occur.MUST);

        }

        private void btnEvent3_Click(object sender, EventArgs e)
        {
            _query = new BooleanQuery();
            AddTermsToAvoid(_query);
            _eventString = "Musk says that the Twitter deal is ‘on hold’. He cites the prevalence of bots and spam accounts on the platform.";
            _query.Add(new TermQuery(new Term("content", "elon")), Occur.MUST);
            _query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20220513, max:20220515,true,true), Occur.MUST);
        }

        private void btnEvent4_Click(object sender, EventArgs e)
        {
            _query = new BooleanQuery();
            AddTermsToAvoid(_query);
            _eventString = "Twitter shareholders bring class-action lawsuit against Musk over alleged stock manipulation tied to the acquisition process.";
            _query.Add(new TermQuery(new Term("content", "elon")), Occur.MUST);
            _query.Add(NumericRangeQuery.NewInt64Range(field: "date", min: 20220526, max: 20220529, true, true), Occur.MUST);

        }

        private void btnEvent5_Click(object sender, EventArgs e)
        {
            _query = new BooleanQuery();
            AddTermsToAvoid(_query);
            _eventString = "/Musk threatens to withdraw from the deal if the social media giant does not disclose information about bots and spams on the platform.";
            _query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20220606, max:20220608,true,true), Occur.MUST);
            _query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);

        }

        private void btnEvent6_Click(object sender, EventArgs e)
        {
            _query = new BooleanQuery();
            AddTermsToAvoid(_query);
            _eventString = "Musk moves to terminate his deal on issue of fake accounts";
            _query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20220708, max:20220710,true,true), Occur.MUST);
            _query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);

        }

        private void btnEvent7_Click(object sender, EventArgs e)
        {
            _query = new BooleanQuery();
            AddTermsToAvoid(_query);
            _eventString = "Twitter sues Musk in Delaware court to force him to complete the deal.\r\n";
            _query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20220712, max:20220714,true,true), Occur.MUST);
            _query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);
        }

        private void btnEvent8_Click(object sender, EventArgs e)
        {
            _query = new BooleanQuery();
            AddTermsToAvoid(_query);
            _eventString = "Musk challenges former Twitter CEO Parag Agrawal to a public debate about spam accounts and polls followers on whether they believe less than 5% of Twitter’s daily active users are fake.";
            _query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20220806, max:20220810,true,true), Occur.MUST);
            _query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);

        }

        private void btnEvent9_Click(object sender, EventArgs e)
        {
            _query = new BooleanQuery();
            AddTermsToAvoid(_query);
            _eventString = "Musk submits a proposal to move forward with the acquisition at the originally agreed-upon price of $44 billion on the condition that Twitter drops its lawsuit. Elon's larger goal is to create X.";
            _query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20221004, max:20221006,true,true), Occur.MUST);
            _query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);

        }

        private void btnEvent10_Click(object sender, EventArgs e)
        {
            _query = new BooleanQuery();
            AddTermsToAvoid(_query);
            _eventString = "According to a report in The Washington Post, Musk is telling investors he plans to terminate nearly 75% of Twitter’s staff.";
            _query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20221020, max:20221023,true,true), Occur.MUST);
            _query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);
            _query.Add(new TermQuery(new Term("content", "crypto")), Occur.MUST_NOT);
            _query.Add(new TermQuery(new Term("content", "doge")), Occur.MUST_NOT);

        }

        private void btnEvent11_Click(object sender, EventArgs e)
        {
            _query = new BooleanQuery();
            AddTermsToAvoid(_query);
            _eventString = " Elon enters twitter with a sink Then closes deal with twitter to terminate nearly 75% of Twitter’s staff.";
            _query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20221026, max:20221029,true,true), Occur.MUST);
            _query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);
            _query.Add(new TermQuery(new Term("content", "crypto")), Occur.MUST_NOT);
            _query.Add(new TermQuery(new Term("content", "doge")), Occur.MUST_NOT);

        }

        private void btnEvent12_Click(object sender, EventArgs e)
        {
            _query = new BooleanQuery();
            AddTermsToAvoid(_query);
            _eventString = " Elon plans to add a new verification system. Then confirms that it will cost 8 bucks.";
            _query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20221030, max:20221103,true,true), Occur.MUST);
            _query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);
            _query.Add(new TermQuery(new Term("content", "crypto")), Occur.MUST_NOT);
            _query.Add(new TermQuery(new Term("content", "doge")), Occur.MUST_NOT);
        }

        private void btnEvent13_Click(object sender, EventArgs e)
        {
            _query = new BooleanQuery();
            AddTermsToAvoid(_query);
            _eventString = " Elon fires half of his staff. He did it in order to cut costs due to massive drop in revenue.";
            _query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20221104, max:20221108,true,true), Occur.MUST);
            _query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);
            _query.Add(new TermQuery(new Term("content", "crypto")), Occur.MUST_NOT);
            _query.Add(new TermQuery(new Term("content", "doge")), Occur.MUST_NOT);

        }

        private void btnEvent14_Click(object sender, EventArgs e)
        {
            _query = new BooleanQuery();
            AddTermsToAvoid(_query);
            _eventString = " Twitter Blue begins... and then people start impersonating brands. During this time, elon talks about possible bankruptcy.";
            _query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20221109, max:20221112,true,true), Occur.MUST);
            _query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);
            _query.Add(new TermQuery(new Term("content", "crypto")), Occur.MUST_NOT);
            _query.Add(new TermQuery(new Term("content", "doge")), Occur.MUST_NOT);

        }

        private void btnEvent15_Click(object sender, EventArgs e)
        {
            _query = new BooleanQuery();
            AddTermsToAvoid(_query);
            _eventString = " Musk send an ultimatum to staff, asking to work long hours or leave.";
            _query.Add(NumericRangeQuery.NewInt64Range(field:"date",min:20221116, max:20221118,true,true), Occur.MUST);
            _query.Add(new TermQuery(new Term("content", "twitter")), Occur.MUST);
            _query.Add(new TermQuery(new Term("content", "staff")), Occur.SHOULD);

        }
        private void AddTermsToAvoid(BooleanQuery query)
        {
            query.Add(new TermQuery(new Term("user", "ElonAnnounces")), Occur.MUST_NOT);
            query.Add(new FuzzyQuery(new Term("content", "Breaking")), Occur.MUST_NOT);
            query.Add(new FuzzyQuery(new Term("content", "BreakingNews")), Occur.MUST_NOT);
            query.Add(new FuzzyQuery(new Term("content", "In a recent interview")), Occur.MUST_NOT);
            query.Add(new FuzzyQuery(new Term("user", "breaking"), 2, 4), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("content", "BREAKING:")), Occur.MUST_NOT);
            query.Add(new FuzzyQuery(new Term("user", "news"), 2, 2), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("content", "LATEST:")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "DefiCinema")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "movierecite")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "kosmatos")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "africansinnews")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "SobhitSinha")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "CryptonewZi")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "StockandmoreCom")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "themetav3rse")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "MorningBrew")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "VisitoryNews")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "TraderMarcoCost")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "marketexplainor")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "KMJNOW")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "theblaze")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "UrbanRecorderPk")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "Raw_News1st")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "Andrey_Daniel_B")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "macronewswire")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "BoricuaEnMaui")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "BradHound")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "PBandJaimie")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "breakingmkts")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "faststocknewss")), Occur.MUST_NOT);
            query.Add(new FuzzyQuery(new Term("content", "What You Need to Know - CNET"), 2, 2), Occur.MUST_NOT);
            query.Add(new FuzzyQuery(new Term("user", "cgtnafrica"), 2, 2), Occur.MUST_NOT);
            query.Add(new FuzzyQuery(new Term("user", "Taipan57602002"), 2, 2), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "CoinDiscoveryy")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "Claire__James")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("content", "@realDailyWire")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "StockMKTNewz")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "W3B_news")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("content", "SCOOP")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "0xthefutureis")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "delete75522330")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("content", "bloomberg")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("content", "Cryptocurrency Price")), Occur.MUST_NOT);
            query.Add(new TermQuery(new Term("user", "EmpiricalNooz")), Occur.MUST_NOT);

        }
    }
}
