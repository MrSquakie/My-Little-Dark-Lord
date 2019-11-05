namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("Game Creator/Managers/AudioManager", 100)]
	public class AudioManager : Singleton<AudioManager>, IGameSave
	{
		[System.Serializable]
		public class Volume
		{
			public float music;
            public float sound2D;
            public float sound3D;
            public float voice;

			public Volume(float music, float sound, float voice)
			{
				this.music = music;
				this.sound2D = sound;
                this.sound3D = sound;
                this.voice = voice;
			}
		}

		private const int MAX_MUSIC_SOURCES = 2;
		private const int MAX_SOUND_SOURCES = 30;
        private const int MAX_VOICE_SOURCES = 30;

		private static float GLOBAL_MUSIC_VOLUME = 1.0f;
		private static float GLOBAL_SOUND_VOLUME = 1.0f;
        private static float GLOBAL_VOICE_VOLUME = 1.0f;

		// PROPERTIES: ----------------------------------------------------------------------------

		private int musicIndex   = 0;
        private int sound2DIndex = 0;
        private int sound3DIndex = 0;
        private int voiceIndex   = 0;

		private AudioBuffer[] musicSources;
        private AudioBuffer[] sound2DSources;
        private AudioBuffer[] sound3DSources;
        private AudioBuffer[] voiceSources;

		// INITIALIZE: ----------------------------------------------------------------------------

		protected override void OnCreate ()
		{
			this.musicSources = new AudioBuffer[MAX_MUSIC_SOURCES];
			this.sound2DSources = new AudioBuffer[MAX_SOUND_SOURCES];
            this.sound3DSources = new AudioBuffer[MAX_SOUND_SOURCES];
            this.voiceSources = new AudioBuffer[MAX_VOICE_SOURCES];

			for (int i = 0; i < this.musicSources.Length; ++i) this.musicSources[i] = this.CreateMusicSource(i);
			for (int i = 0; i < this.sound2DSources.Length; ++i) this.sound2DSources[i] = this.CreateSoundSource(i, "2D");
            for (int i = 0; i < this.sound3DSources.Length; ++i) this.sound3DSources[i] = this.CreateSoundSource(i, "3D");
            for (int i = 0; i < this.voiceSources.Length; ++i) this.voiceSources[i] = this.CreateVoiceSource(i);

			SaveLoadManager.Instance.Initialize(this);
		}

		private AudioBuffer CreateMusicSource(int index)
		{
            AudioSource clip = this.CreateAudioAsset("music", index);
			clip.loop = true;
			return new AudioBuffer(clip, GLOBAL_MUSIC_VOLUME);
		}

		private AudioBuffer CreateSoundSource(int index, string suffix)
		{
            AudioSource clip = this.CreateAudioAsset("sound" + suffix, index);
			clip.loop = false;
			return new AudioBuffer(clip, GLOBAL_SOUND_VOLUME);
		}

        private AudioBuffer CreateVoiceSource(int index)
        {
            AudioSource clip = this.CreateAudioAsset("voice", index);
            clip.loop = false;
            return new AudioBuffer(clip, GLOBAL_VOICE_VOLUME);
        }

        private AudioSource CreateAudioAsset(string audioName, int index)
		{
			GameObject asset = new GameObject(audioName + "_" + index);
			asset.transform.parent = transform;

            AudioSource clip = asset.AddComponent<AudioSource>();
			clip.playOnAwake = false;
			return clip;
		}

		// UPDATE: --------------------------------------------------------------------------------

		private void Update()
		{
			for (int i = 0; i < this.musicSources.Length; ++i)
			{
				this.musicSources[i].Update();
			}

			for (int i = 0; i < this.sound2DSources.Length; ++i)
			{
				this.sound2DSources[i].Update();
			}

            for (int i = 0; i < this.sound3DSources.Length; ++i)
            {
                this.sound3DSources[i].Update();
            }

            for (int i = 0; i < this.voiceSources.Length; ++i)
            {
                this.voiceSources[i].Update();
            }
		}

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public void PlayMusic(AudioClip audioClip, float fadeIn = 0.0f)
		{
			this.musicIndex = (++this.musicIndex < this.musicSources.Length ? this.musicIndex : 0);
			this.musicSources[this.musicIndex].Play(audioClip, fadeIn);
		}

		public void StopMusic(float fadeOut = 0.0f)
		{
			for (int i = 0; i < this.musicSources.Length; ++i)
			{
				this.musicSources[i].Stop(fadeOut);
			}
		}

		public void PlaySound2D(AudioClip audioClip, float fadeIn = 0.0f)
		{
			this.sound2DIndex = (++this.sound2DIndex < this.sound2DSources.Length ? this.sound2DIndex : 0);
			this.sound2DSources[this.sound2DIndex].Play(audioClip, fadeIn);
		}

		public void StopSound2D(AudioClip audioClip, float fadeOut = 0.0f)
		{
			for (int i = 0; i < this.sound2DSources.Length; ++i)
			{
				AudioClip clip = this.sound2DSources[i].GetAudioClip();
				if (clip != null && clip.name == audioClip.name)
				{
					this.sound2DSources[i].Stop(fadeOut);
				}
			}
		}

        public void PlaySound3D(AudioClip audioClip, float fadeIn, Vector3 position, float spatialBlend, float pitch)
        {
            this.sound3DIndex = (++this.sound3DIndex < this.sound3DSources.Length ? this.sound3DIndex : 0);
            this.sound3DSources[this.sound3DIndex].SetSpatialBlend(spatialBlend);
            this.sound3DSources[this.sound3DIndex].SetPitch(pitch);
            this.sound3DSources[this.sound3DIndex].SetPosition(position);
            this.sound3DSources[this.sound3DIndex].Play(audioClip, fadeIn);
        }

        public void StopAllSounds(float fadeOut = 0.0f)
        {
            for (int i = 0; i < this.sound2DSources.Length; ++i)
            {
                this.sound2DSources[i].Stop(fadeOut);
            }

            for (int i = 0; i < this.sound3DSources.Length; ++i)
            {
                this.sound3DSources[i].Stop(fadeOut);
            }
        }

        public void PlayVoice(AudioClip audioClip, float fadeIn = 0.0f)
        {
            this.voiceIndex = (++this.voiceIndex < this.voiceSources.Length ? this.voiceIndex : 0);
            this.voiceSources[this.voiceIndex].Play(audioClip, fadeIn);
        }

        public void StopVoice(AudioClip audioClip, float fadeOut = 0.0f)
        {
            for (int i = 0; i < this.voiceSources.Length; ++i)
            {
                AudioClip clip = this.voiceSources[i].GetAudioClip();
                if (clip != null && clip.name == audioClip.name)
                {
                    this.voiceSources[i].Stop(fadeOut);
                }
            }
        }

        public void StopAllVoices(float fadeOut = 0.0f)
        {
            for (int i = 0; i < this.voiceSources.Length; ++i)
            {
                this.voiceSources[i].Stop(fadeOut);
            }
        }

        public void StopAllAudios(float fadeOut = 0.0f)
        {
            this.StopMusic(fadeOut);
            this.StopAllSounds(fadeOut);
            this.StopAllVoices(fadeOut);
        }

        // VOLUME METHODS: ------------------------------------------------------------------------

		public void SetGlobalMusicVolume(float volume) 
		{ 
			AudioManager.GLOBAL_MUSIC_VOLUME = volume;
			for (int i = 0; i < this.musicSources.Length; ++i)
			{
				this.musicSources[i].SetMaxVolume(volume);
			}
		}

		public void SetGlobalSoundVolume(float volume)
		{
			AudioManager.GLOBAL_SOUND_VOLUME = volume;
			for (int i = 0; i < this.sound2DSources.Length; ++i)
			{
				this.sound2DSources[i].SetMaxVolume(volume);
			}

            for (int i = 0; i < this.sound3DSources.Length; ++i)
            {
                this.sound3DSources[i].SetMaxVolume(volume);
            }
		}

        public void SetGlobalVoiceVolume(float volume)
        {
            AudioManager.GLOBAL_VOICE_VOLUME = volume;
            for (int i = 0; i < this.voiceSources.Length; ++i)
            {
                this.voiceSources[i].SetMaxVolume(volume);
            }
        }

		public float GetGlobalMusicVolume() { return AudioManager.GLOBAL_MUSIC_VOLUME; }
		public float GetGlobalSoundVolume() { return AudioManager.GLOBAL_SOUND_VOLUME; }
        public float GetGlobalVoiceVolume() { return AudioManager.GLOBAL_VOICE_VOLUME; }

		// INTERFACE ISAVELOAD: -------------------------------------------------------------------

		public string GetUniqueName()
		{
			return "volume";
		}

		public System.Type GetSaveDataType()
		{
			return typeof(AudioManager.Volume);
		}

		public System.Object GetSaveData()
		{
			return new AudioManager.Volume(
				AudioManager.GLOBAL_MUSIC_VOLUME,
				AudioManager.GLOBAL_SOUND_VOLUME,
                AudioManager.GLOBAL_VOICE_VOLUME
			);
		}

		public void ResetData()
		{
			AudioManager.Instance.SetGlobalMusicVolume(1.0f);
			AudioManager.Instance.SetGlobalSoundVolume(1.0f);
            AudioManager.Instance.SetGlobalVoiceVolume(1.0f);
		}

		public void OnLoad(System.Object generic)
		{
			AudioManager.Volume volume = (AudioManager.Volume)generic;
			AudioManager.Instance.SetGlobalMusicVolume(volume.music);
			AudioManager.Instance.SetGlobalSoundVolume(volume.sound2D);
            AudioManager.Instance.SetGlobalVoiceVolume(volume.voice);
		}
	}
}