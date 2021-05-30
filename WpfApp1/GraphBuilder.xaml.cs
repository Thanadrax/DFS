using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Microsoft.Win32;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для GraphBuilder.xaml
    /// </summary>
    public partial class GraphBuilder : Window
    {
        public List<double[]> aResultsForSave;
        public GraphBuilder()
        {
            InitializeComponent();
        }
        public void BuildGraph(String strChartName, List<double[]> aResults, double[] aInputs)
        {
            aResultsForSave = aResults;
            var chartVals = new ChartValues<Point>();
            foreach (var dRes in aResults)
            {
                var chrtPoint = new Point() {X = dRes[0], Y = dRes[1]};
                chartVals.Add(chrtPoint);
            }
            SplinedChart.Series.Clear();
            SplinedChart.Series.Add(new LineSeries
            {
                Configuration = new CartesianMapper<Point>().X(point => point.X).Y(point => point.Y),
                Values = chartVals,
                Title = "Логарифмированное давление",
            });
            ChartName.Content = strChartName;

            ValuesLabel.Content = "X=" + aInputs[0] + " м | " + " Y=" + aInputs[1] + " м | " + " NG=" + aInputs[2]
            + " | " + "DG=" + aInputs[3] + " м | " + " NP=" + aInputs[4] + " | " + " DP=" + aInputs[5] + " м | " + " F=" + aInputs[6] + " Гц | " + " C0=" + aInputs[7] + " м/с";
        }

        public void LoadGraph(String strFileName)
        {
            PureGraphValuesResult filePGVR = new JavaScriptSerializer().Deserialize<PureGraphValuesResult>(File.ReadAllText(strFileName));
            var chartVals = new ChartValues<Point>();
            foreach (var dRes in filePGVR.aResults)
            {
                var chrtPoint = new Point() { X = dRes[0], Y = dRes[1] };
                chartVals.Add(chrtPoint);
            }
            SplinedChart.Series.Clear();
            SplinedChart.Series.Add(new LineSeries
            {
                Configuration = new CartesianMapper<Point>().X(point => point.X).Y(point => point.Y),
                Values = chartVals,
                Title = "Логарифмированное давление",
            });
            ChartName.Content = filePGVR.strChartName;
            ValuesLabel.Content = filePGVR.strInputs;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void SaveToPng(FrameworkElement visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }

        void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            saveFileDialog.Filter = "PNG (*.png)|*.png|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveToPng(GraphArea, saveFileDialog.FileName);
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            saveFileDialog.Filter = "PRNS (*.prns)|*.prns|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                PureGraphValuesResult filePGVR = new PureGraphValuesResult(ChartName.Content.ToString(), aResultsForSave, ValuesLabel.Content.ToString());
                File.WriteAllText(saveFileDialog.FileName, new JavaScriptSerializer().Serialize(filePGVR));
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            openFileDialog.Filter = "PRNS (*.prns)|*.prns";
            if (openFileDialog.ShowDialog() == true)
            {
                PureGraphValuesResult filePGVR = new JavaScriptSerializer().Deserialize<PureGraphValuesResult>(File.ReadAllText(openFileDialog.FileName));
                var chartVals = new ChartValues<Point>();
                foreach (var dRes in filePGVR.aResults)
                {
                    var chrtPoint = new Point() { X = dRes[0], Y = dRes[1] };
                    chartVals.Add(chrtPoint);
                }
                SplinedChart.Series.Clear();
                SplinedChart.Series.Add(new LineSeries
                {
                    Configuration = new CartesianMapper<Point>().X(point => point.X).Y(point => point.Y),
                    Values = chartVals,
                    Title = "Логарифмированное давление",
                });
                ChartName.Content = filePGVR.strChartName;
                ValuesLabel.Content = filePGVR.strInputs;
            }
        }
    }
}
