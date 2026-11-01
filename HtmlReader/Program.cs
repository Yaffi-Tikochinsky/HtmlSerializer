using HtmlReader;

var html = await new HtmlSerializer().LoadHtmlAsync("https://example.com");
var dom = new HtmlSerializer().Serialize(html);
var result = dom.FindElements(Selector.FromQueryString("title"));
Console.WriteLine(result.Count);