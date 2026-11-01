using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlReader
{
    public class HtmlHelper
    {
        private readonly static HtmlHelper _instance=new HtmlHelper();
        public static HtmlHelper Instance=> _instance;
        public  string[] HtmlTags { get;  }
        public  string[] HtmlVoidTags { get;  }
        private HtmlHelper()
        {
            var tags = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Seed", "HtmlTags.json"));
            HtmlTags =System.Text.Json.JsonSerializer.Deserialize<string[]>(tags);

            var voidTags=File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Seed", "HtmlVoidTags.json"));
            HtmlVoidTags=System.Text.Json.JsonSerializer.Deserialize<string[]>(voidTags);

        }

    }
}
