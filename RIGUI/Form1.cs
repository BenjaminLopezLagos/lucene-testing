using System.Security;

namespace RIGUI
{
    public partial class Form1 : Form
    {
        private double[] pValues = { 1, 1, 1, 1, 1, 1, 1 };
        private double[] nValues = { 0, 0, 0, 0, 0, 0, 0 };


        public Form1()
        {
            InitializeComponent();

            // generate sample data (arrays of individual DateTimes and values)
            int pointCount = 100;
            Random rand = new Random(0);
            double[] values = ScottPlot.DataGen.RandomWalk(rand, pointCount);
            DateTime[] dates = Enumerable.Range(0, pointCount)
                                          .Select(x => new DateTime(2016, 06, 27).AddDays(x))
                                          .ToArray();

            // use LINQ and DateTime.ToOADate() to convert DateTime[] to double[]
            double[] xs = dates.Select(x => x.ToOADate()).ToArray();

            // plot the double arrays using a traditional scatter plot
            formsPlot1.Plot.AddScatter(xs, values);

            // indicate the horizontal axis tick labels should display DateTime units
            formsPlot1.Plot.XAxis.DateTimeFormat(true);


            //formsPlot1.Plot.AddSignal(pValues, label: "p");
            //formsPlot1.Plot.AddSignal(nValues, label: "n");
            formsPlot1.Refresh();
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