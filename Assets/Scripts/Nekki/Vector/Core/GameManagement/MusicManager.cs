using System.Collections.Generic;
using Nekki.Vector.Core.Audio;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class MusicManager : ZoneResource<MusicManager>
	{
		private List<Preset> _Presets = new List<Preset>();

		private List<MusicContent> _MusicContent = new List<MusicContent>();

		private MusicContent _SelectedMusic;

		private MusicContent _SelectedAmbient;

		protected override string ResourceId
		{
			get
			{
				return "Music";
			}
		}

		protected override void Parse()
		{
			YamlDocumentNekki yamlDocumentNekki = YamlUtils.OpenYamlFile(VectorPaths.GeneratorData, base.FilePath);
			foreach (Mapping item in yamlDocumentNekki.GetRoot(0))
			{
				_Presets.Add(Preset.Create(item, _Presets));
			}
		}

		public void GenerateMusic()
		{
			_MusicContent.Clear();
			foreach (Preset preset in _Presets)
			{
				preset.GetMusicContent(_MusicContent);
			}
			SelectMusicAndAmbient();
		}

		private void SelectMusicAndAmbient()
		{
			_SelectedMusic = null;
			_SelectedAmbient = null;
			int i = 0;
			for (int count = _MusicContent.Count; i < count; i++)
			{
				if (_SelectedMusic == null && !string.IsNullOrEmpty(_MusicContent[i].TrackName))
				{
					_SelectedMusic = _MusicContent[i];
					DebugUtils.LogFormat("[MusicManager]: select music={0}", _SelectedMusic.TrackName);
				}
				if (_SelectedAmbient == null && !string.IsNullOrEmpty(_MusicContent[i].AmbientName))
				{
					_SelectedAmbient = _MusicContent[i];
					DebugUtils.LogFormat("[MusicManager]: select ambient={0}, volume={1}", _SelectedAmbient.AmbientName, _SelectedAmbient.Volume);
				}
				if (_SelectedMusic != null && _SelectedAmbient != null)
				{
					break;
				}
			}
		}

		public void PlayMusic()
		{
			if (_SelectedMusic != null)
			{
				AudioManager.PlaySpecificRunMusic(_SelectedMusic.TrackName);
			}
		}

		public void PlayAmbient()
		{
			if (_SelectedAmbient != null)
			{
				AudioManager.PlayAmbient(_SelectedAmbient.AmbientName, _SelectedAmbient.Volume);
			}
		}
	}
}
