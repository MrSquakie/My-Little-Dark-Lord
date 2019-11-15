namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[System.Serializable]
	public class AudioBuffer
	{
		private enum AUDIO_STATE
		{
			NORMAL,
			FADE_OUT,
			FADE_IN
		}

		// PROPERTIES: ----------------------------------------------------------------------------

		private AudioSource audio;
		private float maxVolume;

		private AUDIO_STATE state;
		private float stateTime;
		private float stateDuration;

		// INITIALIZE: ----------------------------------------------------------------------------

		public AudioBuffer(AudioSource audio, float maxVolume)
		{
			this.audio = audio;
			this.maxVolume = maxVolume;

			this.state = AUDIO_STATE.NORMAL;
			this.stateTime = -100.0f;
			this.stateDuration = 0.0f;
		}

		// UPDATE: --------------------------------------------------------------------------------

		public void Update()
		{
			if (this.audio.clip == null) return;

			if (this.state == AUDIO_STATE.FADE_IN)
			{
				float t = (Time.time - this.stateTime) / this.stateDuration;

				this.audio.volume = Mathf.Lerp(0.0f, 1.0f, t) * this.maxVolume;
				if (t >= 1.0f) this.state = AUDIO_STATE.NORMAL;
			}
			else if (this.state == AUDIO_STATE.FADE_OUT)
			{
				float t = (Time.time - this.stateTime) / this.stateDuration;
				this.audio.volume = Mathf.Lerp(1.0f, 0.0f, t) * this.maxVolume;
				if (t >= 1.0f) 
				{
					this.state = AUDIO_STATE.NORMAL;
					this.audio.Stop();
				}
			}
			else
			{
				this.audio.volume = this.maxVolume;
			}
		}

		// PUBLIC METHODS: ------------------------------------------------------------------------

        public void Play(AudioClip audioClip, float fadeIn = 0.0f)
		{
			this.audio.clip = audioClip;
			this.audio.Play();

			if (Mathf.Approximately(fadeIn, 0.0f))
			{
				this.state = AUDIO_STATE.NORMAL;
				this.stateTime = -100.0f;
				this.stateDuration = 0.0f;
			}
			else
			{
				this.state = AUDIO_STATE.FADE_IN;
				this.stateTime = Time.time;
				this.stateDuration = fadeIn;
				this.audio.volume = 0.0f;
			}
		}

		public void Stop(float fadeOut = 0.0f)
		{
			if (Mathf.Approximately(fadeOut, 0.0f))
			{
				this.audio.Stop();
				this.state = AUDIO_STATE.NORMAL;
				this.stateTime = -100.0f;
				this.stateDuration = 0.0f;
			}
			else
			{
				this.state = AUDIO_STATE.FADE_OUT;
				this.stateTime = Time.time;
				this.stateDuration = fadeOut;
			}
		}

		public void SetMaxVolume(float maxVolume)
		{
			this.maxVolume = maxVolume;
		}

		public AudioClip GetAudioClip()
		{
			return this.audio.clip;
		}

        public void SetPosition(Vector3 position)
        {
            this.audio.transform.position = position;
        }

        public void SetPitch(float pitch)
        {
            this.audio.pitch = pitch;
        }

        public void SetSpatialBlend(float spatialBlend)
        {
            this.audio.spatialize = !Mathf.Approximately(spatialBlend, 0);
            this.audio.spatialBlend = spatialBlend;
        }
	}
}