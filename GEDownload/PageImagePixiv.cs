using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEDownload
{
	public class PageImagePixiv : PageImage
	{
		public PageImagePixiv(string url) : base(url)
		{
		}

		public override string Titre => throw new NotImplementedException();

		public override PageImage DernierePage()
		{
			throw new NotImplementedException();
		}

		public override PageImage PageSuivante()
		{
			var navLinks = Dom.GetElementbyId("wrapper").GetFirstDescendant("a", "after").FirstChild;
			return new PageImagePixiv(navLinks.GetHref());
		}

		protected override int NombreImages()
		{
			throw new NotImplementedException();
		}

		protected override int NumeroImage()
		{
			throw new NotImplementedException();
		}

		protected override string TrouverImage()
		{
			var navLinks = Dom.GetElementbyId("wrapper").GetFirstDescendant("div", "works_display").FirstChild;
			return navLinks.GetHref();
		}
	}
}
