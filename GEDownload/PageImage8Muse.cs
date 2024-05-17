using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GEDownload
{
	public class PageImage8Muse : PageImage {
	#region members
	#endregion

	#region Properties
	public override string Titre
	{
		get
		{
			var t = Url.Split('/');
			return t[t.Length - 2];
		}
	}
	#endregion

	#region Init
	public PageImage8Muse(string url) : base(url) { }

	protected override int NombreImages()
	{
		return -1;
	}
	protected override int NumeroImage()
	{
		var t = Url.Split('/');
		return int.Parse(t[t.Length - 1]);
	}
	protected override string TrouverImage()
	{
		var n = Dom.DocumentNode.GetFirstDescendantByClass("image");
		return n.GetAttributeValue("src", "");
	}
	#endregion

	public override PageImage PageSuivante()
	{
		var navLinks = Dom.DocumentNode.GetFirstDescendant("a", "page-next");
		return new PageImage8Muse(navLinks.GetHref());
	}
	public override PageImage DernierePage()
	{
		var navLinks = Dom.DocumentNode.GetFirstDescendant("a", "page-prev");
		return new PageImage8Muse(navLinks.GetHref());
	}
}
}
