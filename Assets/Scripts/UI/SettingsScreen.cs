using UnityEngine;
using UnityEngine.Audio;

namespace FMS.TapperRedone.UI
{
	public class SettingsScreen : MonoBehaviour
	{
		[SerializeField] private AudioMixer masterMixer;
		[SerializeField] private string masterVolumeParamName;
		[SerializeField] private string musicVolumeParamName;
		[SerializeField] private string effectsVolumeParamName;

		private void OnVolumeValueChanged(string paramName, float value)
		{
			value = Mathf.Lerp(-80.0f, 0.0f, value);
            masterMixer.SetFloat(paramName, value);
		}

		public void OnMasterValueChanged(float value)
		{
			OnVolumeValueChanged(masterVolumeParamName, value);
		}

		public void OnMusicValueChanged(float value)
		{
			OnVolumeValueChanged(musicVolumeParamName, value);
		}

		public void OnEffectsValueChanged(float value)
		{
			OnVolumeValueChanged(effectsVolumeParamName, value);
		}
	}
}
