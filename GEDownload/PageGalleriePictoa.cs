using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEDownload {
	public class PageGalleriePictoa : PageGallerie {
		#region members
		#endregion

		#region Properties
		#endregion

		#region Init
		public PageGalleriePictoa( string url ) : base(url) { }

		protected override string TrouverNomGallerie()
        {
            var title = Dom.DocumentNode.Descendants().Where(nd => nd.GetAttributeValue("class", "") == "title").First();
            title = title.GetFirstDescendant("h1");
			return title?.InnerText ?? "";
		}
		/// <summary>
		/// Trouve la première image d'une gallerie.
		/// </summary>
		/// <param name="Dom">Dom de la gallerie</param>
		/// <returns>Lien vers la première image de la gallerie.</returns>
		protected override PageImage TrouverDebutGallerie() {
            var link = Dom.GetElementbyId("flex1");
            link = link.Descendants().Where(nd => nd.GetAttributeValue("rel", "") == "nofollow").First();
			return new PageImagePictoa(link.GetHref());
		}
		#endregion
	}
}
