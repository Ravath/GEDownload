using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace GEDownload
{
	public class PageGallerie8Muse : PageGallerie {
	#region members
	#endregion

	#region Properties
	#endregion

	#region Init
	public PageGallerie8Muse(string url) : base(url)
	{
	}

	protected override string TrouverNomGallerie()
	{
		var t = Url.Split('/');
		return t[t.Length - 1];
	}
	/// <summary>
	/// Trouve la première image d'une gallerie.
	/// </summary>
	/// <param name="Dom">Dom de la gallerie</param>
	/// <returns>Lien vers la première image de la gallerie.</returns>
	protected override PageImage TrouverDebutGallerie()
	{
		return new PageImage8Muse(Url + "/1");
	}
	#endregion
}
}
