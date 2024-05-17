using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.IO;

namespace GEDownload {
	public abstract class PageImage : Page {
		#region members
		private string _uri = null;
		private string _nom = null;
        #endregion

        #region Properties
		public bool IsLastPage { get { return NombreTotal == Numero; } }
		public int Numero { get { return NumeroImage(); } }
		public int NombreTotal { get { return NombreImages(); } }
		public string UriImage {
			get {
				if(_uri == null)
					_uri = TrouverImage();
				return _uri;
			}
		}
		public string NomImage {
            get {
				if(_nom == null)
					_nom = UriImage.Substring(UriImage.LastIndexOf("/")+1);
				return _nom;
			}
		}

		public abstract string Titre {
			get;
		}
		#endregion

		#region Init
		public PageImage( string url ) : base(url) {

		}
		protected abstract int NombreImages();
		protected abstract int NumeroImage();
		protected abstract string TrouverImage();
		#endregion

		public abstract PageImage PageSuivante();
		public abstract PageImage DernierePage();

		public bool TelechargerImage( string outPath, bool forceDuplicates) {
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UriImage);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			// Check that the remote file was found. The ContentType
			// check is performed since a request for a non-existent
			// image file might be redirected to a 404-page, which would
			// yield the StatusCode "OK", even though the image was not
			// found.
			if((response.StatusCode == HttpStatusCode.OK ||
				response.StatusCode == HttpStatusCode.Moved ||
				response.StatusCode == HttpStatusCode.Redirect) &&
				response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase)) {

				// Do special management if asked
				FileInfo of = new FileInfo(outPath);
				string destination = outPath;
				if (forceDuplicates)
				{
					// If the file already exists, create new one with another name
					int index = 0;
					while (File.Exists(destination))
					{
						destination = of.DirectoryName + "/"+ index + "_" + of.Name;
						index++;
					}
				}

				// if the remote file was found, download it
				using(Stream inputStream = response.GetResponseStream())
				using(Stream outputStream = File.OpenWrite(destination)) {
					inputStream.ReadTimeout = (int)1E4;
					byte[] buffer = new byte[4096];
					int bytesRead;
					do
					{
						bytesRead = inputStream.Read(buffer, 0, buffer.Length);
						outputStream.Write(buffer, 0, bytesRead);
					} while (bytesRead != 0);
				}
				return true;
			} else { return false; }
		}

		public virtual IEnumerable<PageImage> GetImages() {
			PageImage page = this;
			do {
				yield return page;
				page = page.PageSuivante();
			} while(!page.IsLastPage);
			yield return page;
		}
	}
}
