using System.Collections.Generic;
using Nekki.Vector.Core.Detector;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Result;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Controllers
{
	public class ControllerPlatform
	{
		private ModelHuman _Model;

		private DetectorLine _DetectorH;

		private DetectorLine _DetectorV;

		private List<Belong> _Belongs = new List<Belong>();

		private List<QuadRunner> _Vertical = new List<QuadRunner>();

		private List<QuadRunner> _Horizontal = new List<QuadRunner>();

		public List<Belong> Belongs
		{
			get
			{
				return _Belongs;
			}
		}

		public List<QuadRunner> Vertical
		{
			get
			{
				return _Vertical;
			}
		}

		public List<QuadRunner> Horizontal
		{
			get
			{
				return _Horizontal;
			}
		}

		public ControllerPlatform(ModelHuman p_model)
		{
			_Model = p_model;
			_DetectorH = p_model.ModelObject.DetectorHorizontalLine;
			_DetectorV = p_model.ModelObject.DetectorVerticalLine;
		}

		public void Clear(List<QuadRunner> p_quads)
		{
			foreach (QuadRunner p_quad in p_quads)
			{
				int index = GetIndex(p_quad, _DetectorV.Type);
				int index2 = GetIndex(p_quad, _DetectorH.Type);
				if (index > -1)
				{
					Remove(index, _DetectorV.Type);
				}
				if (index2 > -1)
				{
					Remove(index2, _DetectorH.Type);
				}
			}
		}

		public void Render(QuadRunner p_platform)
		{
			Render(p_platform, _DetectorH);
			Render(p_platform, _DetectorV);
		}

		private void Render(QuadRunner p_platform, DetectorLine p_detector)
		{
			Affiliation affiliation = p_platform.Affiliation(p_detector);
			int index = GetIndex(p_platform, p_detector.Type);
			if (affiliation != null && index == -1)
			{
				int p_side = p_platform.Side(p_detector, affiliation, _Model.Sign);
				affiliation.Clear();
				switch (p_detector.Type)
				{
				case DetectorLine.DetectorType.Horizontal:
					_Horizontal.Add(p_platform);
					break;
				case DetectorLine.DetectorType.Vertical:
					_Vertical.Add(p_platform);
					break;
				}
				_Belongs.Add(new Belong(p_platform, p_detector));
				Update(p_detector);
				Input(p_platform, p_detector, p_side);
				return;
			}
			if (index > -1 && (affiliation == null || (affiliation.CrossList3.Count == 0 && !affiliation.Hits)))
			{
				index = Remove(index, p_detector.Type);
				for (int num = _Belongs.Count - 1; num >= 0; num--)
				{
					if (_Belongs[num].Platform == p_platform && _Belongs[num].Detector == p_detector)
					{
						_Belongs.RemoveAt(num);
					}
				}
				Update(p_detector);
				if (index == 0)
				{
					Output(p_platform, p_detector);
				}
			}
			if (affiliation != null)
			{
				affiliation.Clear();
			}
		}

		public void Update(DetectorLine p_detector)
		{
			List<QuadRunner> list = null;
			switch (p_detector.Type)
			{
			default:
				return;
			case DetectorLine.DetectorType.Horizontal:
				list = _Horizontal;
				break;
			case DetectorLine.DetectorType.Vertical:
				list = _Vertical;
				break;
			}
			if (list.Count > 0)
			{
				p_detector.Node.Data = list[list.Count - 1];
			}
		}

		public void Input(QuadRunner p_platform, DetectorLine p_detector, int p_side)
		{
			DetectorEvent p_event = new DetectorEvent(p_detector, DetectorEvent.DetectorEventType.On, p_side);
			_Model.ControllerAnimation.CheckDetectors(p_event);
		}

		public void Output(QuadRunner p_platform, DetectorLine p_detector)
		{
			DetectorEvent p_event = new DetectorEvent(p_detector, DetectorEvent.DetectorEventType.Off, -1);
			_Model.ControllerAnimation.CheckDetectors(p_event);
			p_detector.Node.Data = null;
		}

		public int GetIndex(QuadRunner p_platform, DetectorLine.DetectorType p_type)
		{
			switch (p_type)
			{
			case DetectorLine.DetectorType.Horizontal:
			{
				for (int j = 0; j < _Horizontal.Count; j++)
				{
					if (p_platform == _Horizontal[j])
					{
						return j;
					}
				}
				break;
			}
			case DetectorLine.DetectorType.Vertical:
			{
				for (int i = 0; i < _Vertical.Count; i++)
				{
					if (p_platform == _Vertical[i])
					{
						return i;
					}
				}
				break;
			}
			default:
				return -1;
			}
			return -1;
		}

		public int Remove(int p_index, DetectorLine.DetectorType p_type)
		{
			switch (p_type)
			{
			case DetectorLine.DetectorType.Horizontal:
				_Horizontal.RemoveAt(p_index);
				return _Horizontal.Count;
			case DetectorLine.DetectorType.Vertical:
				_Vertical.RemoveAt(p_index);
				return _Vertical.Count;
			default:
				return 0;
			}
		}

		public void Remove(QuadRunner p_runner, DetectorLine.DetectorType p_type)
		{
			switch (p_type)
			{
			case DetectorLine.DetectorType.Horizontal:
				_Horizontal.Remove(p_runner);
				break;
			case DetectorLine.DetectorType.Vertical:
				_Vertical.Remove(p_runner);
				break;
			}
		}

		public List<QuadRunner> ActivePlatform(DetectorLine.DetectorType p_type)
		{
			switch (p_type)
			{
			case DetectorLine.DetectorType.Horizontal:
				return _Horizontal;
			case DetectorLine.DetectorType.Vertical:
				return _Vertical;
			default:
				return new List<QuadRunner>();
			}
		}

		public void RemovePlatform(QuadRunner p_runner)
		{
			for (int num = _Belongs.Count - 1; num >= 0; num--)
			{
				if (_Belongs[num].Platform == p_runner)
				{
					Remove(p_runner, _Belongs[num].Detector.Type);
					Output(_Belongs[num].Platform, _Belongs[num].Detector);
					_Belongs.RemoveAt(num);
				}
			}
		}

		public void Reset()
		{
			_Vertical.Clear();
			_Horizontal.Clear();
			_Belongs.Clear();
			_DetectorH.Node.Data = null;
			_DetectorV.Node.Data = null;
		}
	}
}
