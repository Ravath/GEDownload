using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEDownload {
	public abstract class PageGallerie : Page {
		#region members
		private PageImage _firstImage;
		#endregion

		#region Properties
		public string Titre { get; private set; }
		public PageImage FirstImage {
			get {
				if(_firstImage == null) {
					_firstImage = TrouverDebutGallerie();
				}
				return _firstImage;
			}
		}
		#endregion

		#region Init
		public PageGallerie( string url ) : base(url) {
			Titre = TrouverNomGallerie();
		}
		protected override void ReLoad( string url ) {
			base.ReLoad(url);
			Titre = TrouverNomGallerie();
		}
		protected abstract string TrouverNomGallerie();
		/// <summary>
		/// Trouve la première image d'une gallerie.
		/// </summary>
		/// <param name="Dom">Dom de la gallerie</param>
		/// <returns>Lien vers la première image de la gallerie.</returns>
		protected abstract PageImage TrouverDebutGallerie();
		#endregion

		public IEnumerable<PageImage> GetImages() {
			return FirstImage.GetImages();
		}
	}
}
