using lucene_tweets.DetectionModels;

namespace LuceneTweetsGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var a = new Vader();
        }
    }
}