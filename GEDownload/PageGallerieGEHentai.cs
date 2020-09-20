using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEDownload {
	public class PageGallerieGEHentai : PageGallerie {
		#region members
		#endregion

		#region Properties
		#endregion

		#region Init
		public PageGallerieGEHentai( string url ) : base(url) {
			if(Titre == "") {
				ReLoad(Url + "?nw=session");
			}
		}
		protected override string TrouverNomGallerie() {
			var n = Dom.GetElementbyId("gn");
			return n?.InnerText ?? "";
		}
		/// <summary>
		/// Trouve la première image d'une gallerie.
		/// </summary>
		/// <param name="Dom">Dom de la gallerie</param>
		/// <returns>Lien vers la première image de la gallerie.</returns>
		protected override PageImage TrouverDebutGallerie() {
			var n = Dom.GetElementbyId("gdt");
			var link = n.FirstChild.FirstChild.FirstChild;
			return new PageImageGEHentai(link.GetHref());
		}
		#endregion
	}
}
