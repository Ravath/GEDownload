using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.IO;

namespace GEDownload {
	public class PageImageJapscan : PageImage {
		#region members
		#endregion

		#region Properties
		public override string Titre {
			get
			{
				var n = Dom.DocumentNode.GetFirstDescendant("h1");
				return n.InnerText;
			}
		}
		#endregion

		#region Init
		public PageImageJapscan( string url ) : base(url) { }

		protected override int NombreImages() {
			var position = Dom.DocumentNode.GetFirstDescendant("div", "card").ChildNodes.ElementAt(4);
			return int.Parse(position.InnerText.Split(' ')[1]);
		}
		protected override int NumeroImage() {
			var position = Dom.DocumentNode.GetFirstDescendant("ol").ChildNodes.First(p => p.Name == "div");
			return int.Parse(position.InnerText.Split(' ')[1]);
		}
		protected override string TrouverImage() {
			var n = Dom.GetElementbyId("image");
			return n.GetAttributeValue("data-src", "");
		}
		#endregion

		public override PageImage PageSuivante()
		{
			var navLinks = Dom.GetElementbyId("image").ChildNodes.Where(p => p.Name == "a");
			return new PageImageGEHentai(navLinks.ElementAt(0).GetHref());
		}
		public override PageImage DernierePage() {
			return null;
		}
	}
}
