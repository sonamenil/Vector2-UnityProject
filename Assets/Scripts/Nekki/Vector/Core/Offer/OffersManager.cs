using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.DataValidation;
using SimpleJSON;

namespace Nekki.Vector.Core.Offer
{
	public static class OffersManager
	{
		private const int _MaxDownloadAttempt = 2;

		private const string _Filename = "offers.xml";

		private static List<Offer> _Offers = new List<Offer>();

		private static List<Offer> _NewOffers = new List<Offer>();

		private static List<Offer> _ToDeactivateOffers = new List<Offer>();

		private static string Filename
		{
			get
			{
				return VectorPaths.OffersExternal + "/offers.xml";
			}
		}

		public static bool IsUpdateAvalible
		{
			get
			{
				return _NewOffers.Count > 0 || _ToDeactivateOffers.Count > 0;
			}
		}

		private static Offer GetOfferByName(string p_name)
		{
			foreach (Offer offer in _Offers)
			{
				if (offer.Name == p_name)
				{
					return offer;
				}
			}
			return null;
		}

		private static bool IsOfferExists(string p_name)
		{
			Offer offerByName = GetOfferByName(p_name);
			return offerByName != null;
		}

		public static void Init()
		{
			ClearData();
			LoadFile();
		}

		private static void LoadFile()
		{
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocumentAndCheckHash(Filename, string.Empty, XmlUtils.OpenXmlType.ForcedExternal);
			if (xmlDocument == null || !UserDataValidator.IsValid)
			{
				FileUtils.DeleteFileAndHash(Filename);
				return;
			}
			foreach (XmlNode childNode in xmlDocument["Offers"].ChildNodes)
			{
				Offer offer = new Offer(childNode);
				_Offers.Add(offer);
				if (offer.IsWaitingForDeactivate)
				{
					_ToDeactivateOffers.Add(offer);
				}
			}
		}

		private static void SaveFile()
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));
			XmlElement xmlElement = xmlDocument.CreateElement("Offers");
			xmlDocument.AppendChild(xmlElement);
			foreach (Offer offer in _Offers)
			{
				offer.Save(xmlElement);
			}
			XmlUtils.SaveDocumentAndUpdateHash(xmlDocument, Filename);
		}

		private static void DoneEndedOffers()
		{
			bool flag = false;
			foreach (Offer offer in _Offers)
			{
				if (offer.IsWaitingForDeactivate)
				{
					flag = true;
					offer.RevertChanges();
				}
			}
			if (flag)
			{
				SaveFile();
			}
		}

		public static void CheckServerOffers(JSONArray p_node)
		{
			_NewOffers.Clear();
			if (p_node != null)
			{
				CancelUnavaliableOffers(p_node);
				if (p_node.Count > 0)
				{
					CheckAvaliableOffers(p_node);
				}
				else
				{
					RemoveDoneOffers();
				}
				SaveFile();
			}
		}

		private static void CancelUnavaliableOffers(JSONArray p_node)
		{
			foreach (Offer offer in _Offers)
			{
				if (offer.IsActivated && offer.IsCancelable && !IsOfferAvaliable(offer.Name, p_node))
				{
					offer.BeginDeactivate();
					_ToDeactivateOffers.Add(offer);
				}
			}
		}

		private static bool IsOfferAvaliable(string p_offerId, JSONArray p_node)
		{
			foreach (JSONClass child in p_node.Children)
			{
				if (child["name"].Value == p_offerId)
				{
					return true;
				}
			}
			return false;
		}

		private static void CheckAvaliableOffers(JSONArray p_node)
		{
			foreach (JSONClass child in p_node.Children)
			{
				CheckOffer(child);
			}
		}

		private static void CheckOffer(JSONClass p_node)
		{
			string value = p_node["name"].Value;
			int asInt = p_node["data_version"].AsInt;
			if (ResourcesValidator.GamedataVersion >= asInt && !IsOfferExists(value))
			{
				_NewOffers.Add(new Offer(p_node));
			}
		}

		private static void RemoveDoneOffers()
		{
			List<Offer> list = new List<Offer>();
			foreach (Offer offer in _Offers)
			{
				if (offer.IsDone)
				{
					list.Add(offer);
				}
			}
			if (list.Count <= 0)
			{
				return;
			}
			foreach (Offer item in list)
			{
				_Offers.Remove(item);
			}
		}

		private static void TryToLoadOffer(Offer p_offer, int p_downloadAttempt = 0)
		{
			if (p_downloadAttempt >= 2)
			{
				OnLoadFail();
				return;
			}
			InternetUtils.DownloadFileResult downloadFileResult = InternetUtils.DownloadFile(p_offer.URL_Content);
			if (downloadFileResult.IsError)
			{
				DebugUtils.LogFormat("[OffersManager]: load url - {0}, error - {1}", downloadFileResult.Url, downloadFileResult.ErrorMsg);
				TryToLoadOffer(p_offer, p_downloadAttempt + 1);
			}
			else if (MD5Utils.CheckBytesHash(downloadFileResult.Data, p_offer.Hash, ResourcesValidator.Salt))
			{
				if (p_offer.ExtractContent(downloadFileResult.Data))
				{
					ActivateOffer(p_offer);
				}
				else
				{
					TryToLoadOffer(p_offer, p_downloadAttempt + 1);
				}
			}
			else
			{
				TryToLoadOffer(p_offer, p_downloadAttempt + 1);
			}
		}

		private static void OnLoadFail()
		{
		}

		private static void ActivateOffer(Offer p_offer)
		{
			if (p_offer.Duration <= 0)
			{
				FileUtils.SafeDeleteDirectory(p_offer.OfferDir);
				DebugUtils.LogError("[OffersManager]: offer <" + p_offer.Name + "> duration <= 0!");
			}
			else
			{
				p_offer.BeginActivate();
				_Offers.Add(p_offer);
			}
		}

		public static void RunUpdate()
		{
			if (_NewOffers.Count > 0)
			{
				foreach (Offer newOffer in _NewOffers)
				{
					TryToLoadOffer(newOffer);
				}
				_NewOffers.Clear();
			}
			if (_ToDeactivateOffers.Count > 0)
			{
				foreach (Offer toDeactivateOffer in _ToDeactivateOffers)
				{
					toDeactivateOffer.RevertChanges();
				}
			}
			SaveFile();
		}

		public static void OnOfferEnd(string p_name)
		{
			Offer offerByName = GetOfferByName(p_name);
			if (offerByName != null)
			{
				if (offerByName.IsActivated)
				{
					offerByName.BeginDeactivate();
					SaveFile();
				}
			}
			else
			{
				DebugUtils.Log("[OffersManager]: try to stop non existing offer - " + p_name);
			}
		}

		public static void ShowNewsForActivatedOffers()
		{
			foreach (Offer offer in _Offers)
			{
				if (offer.IsWaitingForActivate || offer.IsActivated)
				{
					offer.ShowNews();
				}
			}
		}

		private static void ClearData()
		{
			_Offers.Clear();
			_ToDeactivateOffers.Clear();
		}

		public static void StartTimersForActivatedOffers()
		{
			bool flag = false;
			foreach (Offer offer in _Offers)
			{
				if (offer.IsWaitingForActivate)
				{
					flag = true;
					offer.StartTimer();
				}
			}
			if (flag)
			{
				SaveFile();
			}
		}

		public static void DeactivateEndedOffers()
		{
			if (_ToDeactivateOffers.Count <= 0)
			{
				return;
			}
			foreach (Offer toDeactivateOffer in _ToDeactivateOffers)
			{
				toDeactivateOffer.Done();
			}
			SaveFile();
			_ToDeactivateOffers.Clear();
		}
	}
}
