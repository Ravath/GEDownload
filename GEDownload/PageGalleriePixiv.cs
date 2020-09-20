using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEDownload
{
	public class PageGalleriePixiv : PageGallerie
	{
		#region Init
		public PageGalleriePixiv(string url) : base(url) { }

		protected override string TrouverNomGallerie()
		{
			HtmlNode wrp = Dom.GetElementbyId("wrapper");
			HtmlNode name = wrp.GetFirstDescendant("a", "user-name");
			return name?.InnerHtml ?? "";
		}
		/// <summary>
		/// Trouve la première image d'une gallerie.
		/// </summary>
		/// <param name="Dom">Dom de la gallerie</param>
		/// <returns>Lien vers la première image de la gallerie.</returns>
		protected override PageImage TrouverDebutGallerie()
		{
			HtmlNode wrp = Dom.GetElementbyId("wrapper");
			HtmlNode link = wrp.GetFirstDescendant("li", "image-item");
			return new PageImagePixiv(link.FirstChild.GetHref());
		}
		#endregion
	}
}
