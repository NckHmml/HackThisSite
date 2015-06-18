using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace HTS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            using (var reader = XmlReader.Create("plotMe.xml"))
            {
                var serializer = new XmlSerializer(typeof(PpcPlot));
                PpcPlot result = serializer.Deserialize(reader) as PpcPlot;

                foreach (var line in result.Lines)
                {
                    var toDraw = new Line()
                    {
                        X1 = line.XStart,
                        X2 = line.XEnd,
                        Y1 = 600 - line.YStart,
                        Y2 = 600 - line.YEnd,
                        Stroke = GetBrush(line.Color)
                    };

                    PpcCanvas.Children.Add(toDraw);
                }

                foreach (var arc in result.Arcs)
                {
                    var startAngle = arc.ArcStart;
                    var endAngle = arc.ArcExtend;

                    startAngle = 360 - startAngle;
                    endAngle = startAngle - endAngle;
                    var toDraw = new Arc()
                    {
                        Radius = arc.Radius,
                        Center = new Point(arc.XCenter, 600 - arc.YCenter),
                        StartAngle = Rad(startAngle),
                        EndAngle = Rad(endAngle),
                        Stroke = GetBrush(arc.Color),
                        SmallAngle = false
                    };

                    PpcCanvas.Children.Add(toDraw);
                }
            }
        }

        public double Rad(double x)
        {
            return x * Math.PI / 180;
        }

        public SolidColorBrush GetBrush(string color)
        {
            switch (color)
            {
                case "yellow":
                    return Brushes.Yellow;
                case "green":
                    return Brushes.Green;
                case "blue":
                    return Brushes.Blue;
                case "red":
                    return Brushes.Red;
                default:
                    return Brushes.Black;
            }
        }

        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            string hash = ReverseEncryption.BruteForce(TxtEncrypted.Text);
            if (String.IsNullOrWhiteSpace(hash))
            {
                TxtEncrypted.Text = "Could not find hash";
                return;
            }
            TxtEncrypted.Text = ReverseEncryption.DecryptString(TxtEncrypted.Text, hash);
        }

        private void Sudoku_Click(object sender, RoutedEventArgs e)
        {
            string[] input = TxtSudoku.Text.Replace("\r", "").Split('\n');
            string key;
            using (var sha1 = SHA1.Create())
            {
                string solution = Sudoku.Solve(input[0]);
                byte[] hash = sha1.ComputeHash(Encoding.ASCII.GetBytes(solution));
                key = String.Join("", hash.Select(x => x.ToString("x2")));
            }
            var crypto = new Blowfish(key);
            TxtSudoku.Text = crypto.Decrypt(input[1]);
        }
    }
}
