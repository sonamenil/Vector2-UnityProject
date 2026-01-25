using System.IO;
using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.News;
using SimpleJSON;

namespace Nekki.Vector.Core.Offer
{
	public class Offer
	{
		private const string _UrlDir = "offers/";

		private const string _ContentForwardDir = "forward";

		private const string _ContentRevertDir = "revert";

		private const string _NewsDir = "news";

		public string Name { get; set; }

		public string Hash { get; set; }

		public long Duration { get; set; }

		public bool IsCancelable { get; set; }

		public OfferStatus Status { get; set; }

		public bool IsWaitingForActivate
		{
			get
			{
				return Status == OfferStatus.WaitingForActivate;
			}
		}

		public bool IsActivated
		{
			get
			{
				return Status == OfferStatus.Activated;
			}
		}

		public bool IsWaitingForDeactivate
		{
			get
			{
				return Status == OfferStatus.WaitingForDeactivate;
			}
		}

		public bool IsDone
		{
			get
			{
				return Status == OfferStatus.Done;
			}
		}

		public string URL_Content
		{
			get
			{
				return URLCreator.Make("offers/" + Name + ".bin");
			}
		}

		public string OfferDir
		{
			get
			{
				return VectorPaths.OffersExternal + "/" + Name;
			}
		}

		private string ContentForwardDir
		{
			get
			{
				return OfferDir + "/forward";
			}
		}

		private string ContentRevertDir
		{
			get
			{
				return OfferDir + "/revert";
			}
		}

		private string NewsDir
		{
			get
			{
				return OfferDir + "/news";
			}
		}

		private string Namespace
		{
			get
			{
				return "offer_" + Name;
			}
		}

		public Offer(JSONNode p_node)
		{
			Name = p_node["name"].Value;
			Hash = p_node["hash"].Value;
			Duration = ((!p_node.HasValue("duration")) ? (p_node["endDate"].AsLong * 1000 - TimeManager.UTCTime) : (p_node["duration"].AsLong * 1000));
			IsCancelable = !p_node.HasValue("duration");
			Status = OfferStatus.New;
		}

		public Offer(XmlNode p_node)
		{
			Name = XmlUtils.ParseString(p_node.Attributes["Name"], string.Empty);
			Hash = XmlUtils.ParseString(p_node.Attributes["Hash"], string.Empty);
			Duration = XmlUtils.ParseLong(p_node.Attributes["Duration"]);
			IsCancelable = XmlUtils.ParseBool(p_node.Attributes["Cancelable"]);
			Status = XmlUtils.ParseEnum(p_node.Attributes["Status"], OfferStatus.New);
		}

		public void Save(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("Offer");
			xmlElement.SetAttribute("Name", Name);
			xmlElement.SetAttribute("Hash", Hash);
			xmlElement.SetAttribute("Duration", Duration.ToString());
			xmlElement.SetAttribute("Cancelable", (!IsCancelable) ? "0" : "1");
			xmlElement.SetAttribute("Status", Status.ToString());
			p_node.AppendChild(xmlElement);
		}

		public bool ExtractContent(byte[] p_data)
		{
			string offerDir = OfferDir;
			FileUtils.SafeDeleteDirectory(offerDir);
			Directory.CreateDirectory(offerDir);
			bool flag = CompressUtils.DecompressToDirectory(p_data, offerDir);
			if (flag)
			{
				DebugUtils.LogFormat("{0} - Extract content success!", this);
			}
			return flag;
		}

		public void BeginActivate()
		{
			string contentForwardDir = ContentForwardDir;
			if (Directory.Exists(contentForwardDir))
			{
				UpdateUtils.ProcessUpdateFromFolder(contentForwardDir);
				Directory.Delete(contentForwardDir, true);
			}
			Status = OfferStatus.WaitingForActivate;
			DebugUtils.LogFormat("{0} - WaitingForActivate!", this);
		}

		public void StartTimer()
		{
			TimersManager.CreateTimerForOfferEnd(Name, Duration);
			Status = OfferStatus.Activated;
			DebugUtils.LogFormat("{0} - Activated!", this);
		}

		public void BeginDeactivate()
		{
			Status = OfferStatus.WaitingForDeactivate;
			DebugUtils.LogFormat("{0} - WaitingForDeactivate!", this);
		}

		public void RevertChanges()
		{
			string contentRevertDir = ContentRevertDir;
			if (Directory.Exists(contentRevertDir))
			{
				UpdateUtils.ProcessUpdateFromFolder(contentRevertDir);
			}
			FileUtils.SafeDeleteDirectory(OfferDir);
			DebugUtils.LogFormat("{0} - RevertChanges!", this);
		}

		public void Done()
		{
			CounterController.Current.ClearCounterNamespace(Namespace);
			TimersManager.RemoveTimerForOfferEnd(Name);
			Status = OfferStatus.Done;
			DebugUtils.LogFormat("{0} - Done!", this);
		}

		public void ShowNews()
		{
			string newsDir = NewsDir;
			if (Directory.Exists(newsDir))
			{
				NewsManager.ActivateCustomNews(newsDir);
			}
		}

		public override string ToString()
		{
			return string.Format("[Offer: Name={0}, Duration={1}, IsCancelable={2}, Status={3}]", Name, Duration, IsCancelable, Status);
		}
	}
}
