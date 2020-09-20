using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEDownload {
	public class PageGallerieJapscan : PageGallerie {
		#region members
		#endregion

		#region Properties
		#endregion

		#region Init
		public PageGallerieJapscan( string url ) : base(url) {}

		protected override string TrouverNomGallerie() {
			return null;
		}

		/// <summary>
		/// Trouve la première image d'une gallerie.
		/// </summary>
		/// <param name="Dom">Dom de la gallerie</param>
		/// <returns>Lien vers la première image de la gallerie.</returns>
		protected override PageImage TrouverDebutGallerie()
		{
			return null;
		}
		#endregion
	}
}
