using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlReader
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public Selector Parent { get; set; }
        public Selector child { get; set; }
        public static Selector FromQueryString(string query)
        {
            var levels = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Selector root = null;
            Selector current = null;

            foreach (var level in levels)
            {
                var selector = new Selector();
                var parts = Regex.Split(level, @"(?=[#.])");

                foreach (var part in parts)
                {

                    if (string.IsNullOrEmpty(part))
                        continue;

                    if (part.StartsWith("#"))
                    {
                        selector.Id = part.Substring(1);
                    }
                    else if (part.StartsWith("."))
                    {
                        if (selector.Classes == null)
                            selector.Classes = new List<string>();
                        selector.Classes.Add(part.Substring(1));
                    }
                    else if (HtmlHelper.Instance.HtmlTags.Contains(part.ToLower()))
                    {
                        selector.TagName = part.ToLower();
                    }




                }
                if (root == null)
                {
                    root = selector;
                    current = root;
                }
                else
                {
                    current.child = selector;
                    selector.Parent = current;
                    current = selector;
                }



            }
            return root!;

        }
        public bool Matches(HtmlElement element)
        {
            if (TagName == null && Id == null && Classes == null)
                return false;
            if (TagName != null && !string.Equals(element.Name, TagName, StringComparison.OrdinalIgnoreCase))
                return false;
            if (Id != null && !string.Equals(element.Id, Id, StringComparison.Ordinal))
                return false;
            if (Classes != null && Classes.Any(c => !element.Classes.Contains(c)))
                return false;
            return true;
        }



    }
}
