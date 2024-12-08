using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubparRacing
{
    public class DisplayLayout
    {
        public List<DisplayElement> elements { get; set; } = new List<DisplayElement>();
    }

    public enum DisplayElementType
    {
        BASE,
        TEXT,
        LABELLEDDATAPOINT
    }

    public class DisplayElement
    {
        public string ID { get; set; }  // For identifying the widget (e.g., "Speedometer")
        public string Label { get; set; }
        public string DefaultValue { get; set; }
        public DisplayElementType type {  get; set; }
        public int X { get; set; }     // X position
        public int Y { get; set; }     // Y position
    }

}
