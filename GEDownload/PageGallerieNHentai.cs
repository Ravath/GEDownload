using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEDownload {
	public class PageGallerieNHentai : PageGallerie {
		public PageGallerieNHentai( string url ):base(url) {}

		protected override PageImage TrouverDebutGallerie() {
			HtmlNode thumbnail = Dom.GetElementbyId("thumbnail-container");
			string href = thumbnail.Descendants().First(n => n.Name == "a").GetHref();
			return new PageImageNHentai("https://nhentai.net" + href);
        }

		protected override string TrouverNomGallerie() {
			HtmlNode info = Dom.GetElementbyId("info");
			HtmlNode titre = info.Descendants().First(n => n.Name == "h1");
			return titre.InnerText;
		}
	}
}
