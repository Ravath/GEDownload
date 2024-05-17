using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GEDownload {
	/// <summary>
	/// Logique d'interaction pour MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		#region Members
		private Queue<ArgsDownloader> _listeAttente = new Queue<ArgsDownloader>();
		private BackgroundWorker _bgw = new BackgroundWorker();
		private object _queueLock = new object();
		private object _consoleLock = new object();
		private bool forceDuplicates = true;
		private int _imgFailed = 0;
		#endregion

		#region Properties
		public IEnumerable<string> ListeAttente {
			get {
				lock (_queueLock) {
					foreach(ArgsDownloader couple in _listeAttente) {
						yield return couple.Dossier + " : " + couple.Url;
					}
				}
			}
			set { }
		}
		#endregion

		#region Init
		public MainWindow() {
			InitializeComponent();
			this.DataContext = this;
			_bgw.DoWork += Bgw_DoWork;
			_bgw.RunWorkerCompleted += Bgw_RunWorkerCompleted;
			_bgw.ProgressChanged += Bgw_ProgressChanged;
			_bgw.WorkerReportsProgress = true;
		} 
		#endregion

		#region BackgroundWorker_galleryDownload
		/// <summary>
		/// Classe d'arguments à passer au BackgroundWorker
		/// </summary>
		public class ArgsDownloader {
			public string Dossier { get; set; }
			public string Url { get; set; }
		}
		private void Bgw_ProgressChanged( object sender, ProgressChangedEventArgs e ) {
			string message = e.UserState as string;
			if (e.ProgressPercentage == 1)
				Report(string.Format("Download of {0} has started", message));
			else if (e.ProgressPercentage == 2)
				Report(string.Format("Download of {0} has ended", message));
			else if (e.ProgressPercentage == -1)
			{
				Report(string.Format("Download of img {0} has failed", message));
				_imgFailed++;
			}
			else if (e.ProgressPercentage == -2)
				Report(string.Format("Exception occured: {0}", message));
			xListAttente.ItemsSource = ListeAttente;//maj la liste
		}

		private void Bgw_RunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e ) {
			if(e.Cancelled) {
				Report("Work cancelled.");
			} else if(e.Error != null) {
				Report("Ended with error:" + e.Error.Message);
			} else {
				Report(string.Format("Ended normaly ({0} image failed).", _imgFailed));
				_imgFailed = 0;
			}
			xListAttente.ItemsSource = ListeAttente;//maj la liste
		}

		private void Bgw_DoWork( object sender, DoWorkEventArgs e ) {
			ArgsDownloader args = (ArgsDownloader)e.Argument;
			string argDoss;
			string argUrl;
			do {//work
				//Get inputs
				argDoss = args.Dossier;
				argUrl = args.Url;

				//init
				PageGallerie gallerie = null;
				PageImage image = null;
				Boolean forceDuplicates = this.forceDuplicates;

				//identify site source
				if (argUrl.StartsWith("https://e-hentai.org/")) {
					Page p = new Page(CheckInPath(argUrl));
					if(p.Dom.DocumentNode.Descendants().Where(nd => nd.GetAttributeValue("class", "") == "sni").Count() > 0)
                        image = new PageImageGEHentai(CheckInPath(argUrl));
					else
						gallerie = new PageGallerieGEHentai(CheckInPath(argUrl));
				} else if(argUrl.StartsWith("https://nhentai.net")) {
					gallerie = new PageGallerieNHentai(CheckInPath(argUrl));
				}
				else if (argUrl.StartsWith("https://www.pixiv.net")) {
					gallerie = new PageGalleriePixiv(CheckInPath(argUrl));
                }
                else if (argUrl.StartsWith("https://www.japscan.co"))
				{
					if (argUrl.Contains("lecture-en-ligne"))
						image = new PageImageJapscan(CheckInPath(argUrl));
					else
						gallerie = new PageGallerieJapscan(CheckInPath(argUrl));
                }
                else if (argUrl.Contains("pictoa.com"))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    Page p = new Page(CheckInPath(argUrl));
                    if (p.Dom.GetElementbyId("next") != null)
                        image = new PageImagePictoa(CheckInPath(argUrl));
                    else
                        gallerie = new PageGalleriePictoa(CheckInPath(argUrl));
				}
				else if (argUrl.Contains("comics.8muses.com"))
				{
					if (argUrl.Contains("picture"))
						image = new PageImage8Muse(CheckInPath(argUrl));
					else
						gallerie = new PageGallerie8Muse(CheckInPath(argUrl));
				}
				else {
					throw new WebException("Domaine inconnu.");
				}

				// commencer téléchargement
				string gTitre;
                string dossier;
                if(gallerie != null) {
					// Commencer depuis la première image
					gTitre = gallerie.Titre;
					dossier = CreateDirectory(gTitre, argDoss);
					((BackgroundWorker)sender).ReportProgress(1, gTitre);

					List<Task> imgDownloads = new List<Task>();
					foreach(PageImage im in gallerie.GetImages())
					{
						// download each image with a separated task
						Task t = Task.Run(()=>
						{
							bool imgDownloaded = false;
							try
							{
								imgDownloaded = im.TelechargerImage(dossier + im.NomImage, forceDuplicates);
							}
							catch (Exception ex)
							{
								((BackgroundWorker)sender).ReportProgress(-2, ex.Message);
							}
							if (!imgDownloaded)
							{
								((BackgroundWorker)sender).ReportProgress(-1, im.NomImage);
							}
						});
						imgDownloads.Add(t);
                    }

					// Wait end of gallery downloading
					foreach (var task in imgDownloads)
					{
						task.Wait();
					}
				}
				else {
					// Commencer téléchargemnet de la gallerie à partir de l'image trouvée
					gTitre = image.Titre;
					dossier = CreateDirectory(gTitre, argDoss);
					((BackgroundWorker)sender).ReportProgress(1, gTitre);
					foreach(PageImage im in image.GetImages())
						im.TelechargerImage(dossier + im.NomImage, forceDuplicates);
				}
				((BackgroundWorker)sender).ReportProgress(2, gTitre);
				args = GetNextDownload();
			} while(args != null);
		}
		
		/// <summary>
		/// Create the directory with the name of the gallery within the designed repository.
		/// </summary>
		/// <param name="galleryName">The name of the gallery.</param>
		/// <param name="repoName">Where to create the directory.</param>
		/// <returns>The full name of the created directory.</returns>
		private string CreateDirectory ( string galleryName, string repoName ) {
			string dossier = CheckOutPath(repoName);
			string gTitre = galleryName;
            gTitre = gTitre.Replace('|', '_');
			Regex rg = new Regex("[:/\\<>*?\".]+");
			gTitre = rg.Replace(gTitre, "_");
			dossier += gTitre + "/";
			Directory.CreateDirectory(dossier);
			return dossier;
		}
		/// <summary>
		/// Vérifier la pertinence de l'url de la gallerie.
		/// </summary>
		/// <param name="url">Url à vérifier.</param>
		/// <returns>l'url à utiliser.</returns>
		private string CheckInPath( string url ) {
			if(string.IsNullOrWhiteSpace(url)) { return ""; }
			return url;
		}
		/// <summary>
		/// Met le chemin en forme et créé les répertoires si nécessaire.
		/// </summary>
		/// <param name="outPath">Chemin de sauvegarde.</param>
		/// <returns>Chemin de sauvegarde se terminant par un slash.</returns>
		private string CheckOutPath( string outPath ) {
			if(string.IsNullOrWhiteSpace(outPath)) { return ""; }
			Directory.CreateDirectory(outPath);
			if(!outPath.EndsWith("/"))
				outPath += "/";
			return outPath;
		}
		#endregion

		#region Evenements IHM
		private void xAddButton_Click( object sender, RoutedEventArgs e ) {
			if(_bgw.IsBusy) {
				AddDownload(xUrlPath.Text, xDocPath.Text);
				xListAttente.ItemsSource = ListeAttente;
			} else {
				ArgsDownloader arg = new ArgsDownloader() {
					Url = xUrlPath.Text,
					Dossier = xDocPath.Text
				};
				_bgw.RunWorkerAsync(arg);
			}
		}

		private void xUrlPath_GotFocus(object sender, RoutedEventArgs e)
		{
			xUrlPath.SelectAll();
		}
		#endregion

		#region Accès éléments à télécharger
		/// <summary>
		/// Ajouter un élément à télécharger
		/// </summary>
		/// <param name="url">Url de la gallerie.</param>
		/// <param name="dossier">Dossier de destination</param>
		public void AddDownload( string url, string dossier ) {
			lock (_queueLock) {
				_listeAttente.Enqueue(new ArgsDownloader() {
					Url = url, Dossier = dossier
				});
			}
		}
		/// <summary>
		/// Obtenir le prochain élément à télécharger.
		/// </summary>
		/// <returns>Null if nothing.</returns>
		public ArgsDownloader GetNextDownload() {
			lock (_queueLock) {
				if(_listeAttente.Count == 0)
					return null;
				return _listeAttente.Dequeue();
			}
		}
		#endregion

		#region Fonctions IHM
		/// <summary>
		/// Affiche un message à l'utilisateur dans la console, sur une nouvelle ligne.
		/// </summary>
		/// <param name="message">Message à afficher.</param>
		public void Report( string message ) {
			if(!message.EndsWith("\n")) {
				message += "\n";
			}
			lock (_consoleLock) {
				xConsole.Text = message + xConsole.Text;
			}
		}
		#endregion

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			this.forceDuplicates = ((CheckBox)sender).IsChecked ?? false;
		}
	}
}
