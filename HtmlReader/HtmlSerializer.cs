using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlReader
{
    public class HtmlSerializer
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public async Task<string> LoadHtmlAsync(string url)
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private List<string> SplitHtml(string html)
        {
            var cleanHtml = Regex.Replace(html, @"\s+", " ");
            var parts = Regex.Split(cleanHtml, @"(?=<)|(?<=>)")
                .Select(s => s.Trim())
                .Where(s => s.Length > 0).ToList();
            return parts;
        }
        public HtmlElement Serialize(string html)
        {
            var parts = SplitHtml(html);

            var root = new HtmlElement { Name = "root" };
            var current = root;

            foreach (var part in parts)
            {
                if (!part.StartsWith("<"))
                {
                    current.InnerHtml += part;
                    continue;
                }

                if (part.StartsWith("<!"))
                    continue;

                var content = part.Substring(1, part.Length - 2).Trim();
                var firstWord = content.Split(' ')[0].ToLower();

                if (firstWord == "/html")
                    break;

                if (firstWord.StartsWith("/"))
                {
                    var closing = firstWord[1..];          // "/div" → "div"
                    var target = current;
                    while (target != null && target.Name != closing)
                        target = target.Parent;
                    if (target?.Parent != null)
                        current = target.Parent;
                    continue;
                }

                var tagName = firstWord.TrimEnd('/');

                if (!HtmlHelper.Instance.HtmlTags.Contains(tagName))
                {
                    current.InnerHtml += part;
                    continue;
                }

                var element = new HtmlElement
                {
                    Name = tagName,
                    Parent = current
                };
                current.Children.Add(element);

                ParseAttributes(element, content.Substring(firstWord.Length));

                bool isSelfClosing = content.EndsWith("/")
                    || HtmlHelper.Instance.HtmlVoidTags.Contains(tagName);

                if (!isSelfClosing)
                    current = element;
            }

            return root;
        }
        void ParseAttributes(HtmlElement element, string content)
        {
            var regex = new Regex(@"([\w\-:]+)(?:\s*=\s*(""[^""]*""|'[^']*'|[^\s>]+))?");
            var matches = regex.Matches(content);

            foreach (Match match in matches)
            {
                var name = match.Groups[1].Value;
                var value = match.Groups[2].Success ? match.Groups[2].Value.Trim('"', '\'') : null;
                if (name == "id")
                    element.Id = value;
                else if (name == "class")
                {
                    if (value != null)
                        element.Classes.AddRange(value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
                }
                else
                    element.Attributes[name] = value;
            }
        }



    }
}
