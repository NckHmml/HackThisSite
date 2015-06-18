using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HTS
{
    [XmlRoot("ppcPlot")]
    public class PpcPlot
    {
        [XmlElement("Arc")]
        public Arc[] Arcs { get; set; }

        [XmlElement("Line")]
        public Line[] Lines { get; set; }

        public struct Arc
        {
            public string Color { get; set; }

            public int ArcStart { get; set; }

            public int ArcExtend { get; set; }

            public double YCenter { get; set; }

            public double XCenter { get; set; }

            public double Radius { get; set; }
        }

        public struct Line
        {
            public string Color { get; set; }

            public double XEnd { get; set; }

            public double XStart { get; set; }

            public double YEnd { get; set; }

            public double YStart { get; set; }
        }
    }
}
