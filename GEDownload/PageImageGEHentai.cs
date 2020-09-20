using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.IO;

namespace GEDownload {
	public class PageImageGEHentai : PageImage {
		#region members
		#endregion

		#region Properties
		public override string Titre {
			get {
				var n = Dom.DocumentNode.GetFirstDescendant("h1");
				return n.InnerText;
			}
		}
		#endregion

		#region Init
		public PageImageGEHentai( string url ) : base(url) { }

		protected override int NombreImages() {
			var position = Dom.DocumentNode.GetFirstDescendant("div", "sn").ChildNodes.First(p => p.Name == "div");
			return int.Parse(position.ChildNodes.ElementAt(2).InnerText);
		}
		protected override int NumeroImage() {
			var position = Dom.DocumentNode.GetFirstDescendant("div", "sn").ChildNodes.First(p => p.Name == "div");
			return int.Parse(position.ChildNodes.ElementAt(0).InnerText);
		}
		protected override string TrouverImage() {
			var n = Dom.GetElementbyId("img");
			return n.GetAttributeValue("src", "");
		}
		#endregion

		public override PageImage PageSuivante() {
			var navLinks = Dom.DocumentNode.GetFirstDescendant("div", "sn").ChildNodes.Where(p => p.Name == "a");
			return new PageImageGEHentai(navLinks.ElementAt(2).GetHref());
		}
		public override PageImage DernierePage() {
			var navLinks = Dom.DocumentNode.GetFirstDescendant("div", "sn").ChildNodes.Where(p => p.Name == "a");
			return new PageImageGEHentai(navLinks.ElementAt(3).GetHref());
		}
	}
}
