using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlReader
{
    public static class HtmlElementExtensions
    {

        public static HashSet<HtmlElement> FindElements(this HtmlElement element, Selector selector)
        {
            var result = new HashSet<HtmlElement>();
            FindRecursive(element, selector, result);
            return result;
        }
        private static void FindRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> result)
        {
            var descendants = element.Descendants()
                 .Skip(1).Where(e => selector.Matches(e));
            if (selector.child == null)
            {
                foreach (var descendant in descendants)
                {
                    result.Add(descendant);
                }
                return;
            }


            foreach (var descendant in descendants)
            {
                FindRecursive(descendant, selector.child, result);
            }
        }
    }
}
