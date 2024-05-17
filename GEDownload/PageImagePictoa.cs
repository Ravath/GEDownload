using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.IO;

namespace GEDownload {
	public class PageImagePictoa : PageImage {
		#region members
		#endregion

		#region Properties
		public override string Titre {
			get {
                var n = Dom.GetElementbyId("left");
                n = n.GetFirstDescendant("a");
				return n.InnerText.Trim(new char[]{ ' ', '\r' });
			}
		}
        #endregion

        #region Init
        public PageImagePictoa( string url ) : base(url) {}

        public new bool IsLastPage { get { return true; } }
        protected override int NombreImages() {
            throw new NotImplementedException("No gallery size available on Pictoa");
		}

		protected override int NumeroImage() {
            throw new NotImplementedException("No page number available on Pictoa");
        }
		protected override string TrouverImage() {
			var n = Dom.GetElementbyId("player");
            n = n.GetFirstDescendant("img");
			return n.GetAttributeValue("src", "");
		}
		#endregion

		public override PageImage PageSuivante()
        {
            var navLinks = Dom.GetElementbyId("next");
			return new PageImagePictoa(navLinks.GetHref());
		}
		public override PageImage DernierePage()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Comme ne peux pas compter les images, pour savoir si dernière image, on vérifie si on a bouclé.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<PageImage> GetImages()
        {
            PageImage page = this;
            string startUrl = page.Url;

            do
            {
                yield return page;
                page = page.PageSuivante();
            } while (startUrl != page.Url);
        }
    }
}
