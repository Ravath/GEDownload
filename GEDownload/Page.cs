using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace GEDownload {
	public class Page {
		private static HtmlWeb _loader = new HtmlAgilityPack.HtmlWeb();

		public string Url { get; private set; }
		public HtmlDocument Dom{ get; private set; }

		public Page(string url) {
			Url = url;
			Dom = _loader.Load(url);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		/// <param name="usingGet">for using get or post</param>
		protected virtual void ReLoad( string url ) {
			Url = url;
			Dom = _loader.Load(url);
        }
	}

	public static class HtmlFacilities
	{
		public static string GetClass(this HtmlNode node)
		{
			return node.GetAttributeValue("class", "");
		}
		public static string GetHref(this HtmlNode node)
		{
			return node.GetAttributeValue("href", "");
		}

		public static HtmlNode GetFirstDescendant(this HtmlNode node, string name)
		{
			return node.Descendants().FirstOrDefault(n => n.Name == name);
		}

		public static HtmlNode GetFirstDescendant(this HtmlNode node, string name, string className)
		{
			return node.Descendants().FirstOrDefault(n => n.Name == name && n.GetClass() == className);
		}

		public static HtmlNode GetFirstDescendantByClass(this HtmlNode node, string className)
		{
			return node.Descendants().FirstOrDefault(n => n.GetClass() == className);
		}
	}
}
