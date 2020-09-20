using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEDownload {
	public class PageImageNHentai : PageImage {

		#region Properties
		public override string Titre {
			get {
				throw new NotImplementedException();
			}
		}
		#endregion

		public PageImageNHentai(string url):base(url) {}


		public override PageImage DernierePage() {
			HtmlNode num = Dom.GetElementbyId("pagination-page-top");
			var a = num.GetFirstDescendantByClass("last");
			string href = a?.GetHref();
			if(string.IsNullOrWhiteSpace(href)) { return null; }
			return new PageImageNHentai("https://nhentai.net" + href);
		}

		public override PageImage PageSuivante() {
			HtmlNode num = Dom.GetElementbyId("pagination-page-top");
			var a = num.GetFirstDescendantByClass("next");
			string href = a?.GetHref();
			if(string.IsNullOrWhiteSpace(href)) { return null; }
			return new PageImageNHentai("https://nhentai.net" + href);
		}

		protected override int NombreImages() {
			HtmlNode num = Dom.GetElementbyId("pagination-page-top");
			var span = num.GetFirstDescendantByClass("num-pages");
			int res = -1;
			return int.TryParse(span.InnerText, out res) ? res : -1;
		}

		protected override int NumeroImage() {
			HtmlNode num = Dom.GetElementbyId("pagination-page-top");
			var span = num.GetFirstDescendantByClass("current");
			int res = -1;
			return int.TryParse(span.InnerText, out res) ? res : -1;
		}

		protected override string TrouverImage() {
			HtmlNode cont = Dom.GetElementbyId("image-container");
			string ret = cont.Elements("a").First().Elements("img").First().GetAttributeValue("src", "");
			if(!ret.StartsWith("https:"))
				ret = "https:" + ret;
			return ret;
		}
	}
}
