using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubparRacing
{
    public class DisplayLayoutManager
    {
        public void SaveLayout(DisplayLayout layout)
        {
            string json = JsonConvert.SerializeObject(layout);
            File.WriteAllText("userLayout.json", json);
        }

        public DisplayLayout LoadLayout()
        {
            if (File.Exists("userLayout.json"))
            {
                string json = File.ReadAllText("userLayout.json");
                return JsonConvert.DeserializeObject<DisplayLayout>(json);
            }
            return new DisplayLayout();  // Return a default layout if the file doesn't exist
        }
    }
}
