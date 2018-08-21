using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

using LudicrousElectron.Assets;

namespace LudicrousElectron.Engine.Audio
{
    public static class SoundManager
    {
        public static readonly int InvalidSoundID = -1;
        public static readonly int MaxSoundID = 10000;

        private static List<string> MusicSet = new List<string>();

        private static Dictionary<string, SoundEffect> SoundMap = new Dictionary<string, SoundEffect>();

        private class SoundInstance
        {
            public SoundEffectInstance Instance = null;
            public AudioEmitter Emitter = null;

            public SoundInstance(SoundEffectInstance inst, AudioEmitter emit = null)
            {
                Instance = inst;
                Emitter = emit;
            }
        }

        private static Dictionary<int, SoundInstance> ActiveSounds = new Dictionary<int, SoundInstance>();

        private static AudioListener Listener = new AudioListener();
        private static bool UsePostionalSound = true;
        private static bool NeedListenerUpdate = false;

        public class SpeachEventArgs : EventArgs
        {
            public string Text = string.Empty;
            public bool UseInternal = true;

            internal SpeachEventArgs(string text)
            {
                Text = text;
            }
        }

        public static event EventHandler<SpeachEventArgs> SetTextVoice = null;
        public static event EventHandler<SpeachEventArgs> GetTextVoice = null;
		public static event EventHandler<SpeachEventArgs> GetTextVoiceList = null;
		public static event EventHandler<SpeachEventArgs> SpeakText = null;
        public static event EventHandler<SpeachEventArgs> StopAllSpeach = null;

        public class SoundEventArgs : EventArgs
        {
            public int SoundID = InvalidSoundID;

            internal SoundEventArgs(int id)
            {
                SoundID = id;
            }
        }

        public static event EventHandler<SoundEventArgs> EffectEnded = null;

        static SoundManager()
        {
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
            Listener.Up = new Microsoft.Xna.Framework.Vector3(0, 1, 0);
        }

		internal static void Setup()
		{
			FrameworkDispatcher.Update(); // to let sound update
		}

        internal static void Cleanup()
        {
            StopTextToSpeech();

            foreach (var sound in ActiveSounds.Values)
                sound.Instance.Stop();

            ActiveSounds.Clear();

            foreach (var sound in SoundMap)
                sound.Value.Dispose();

            SoundMap.Clear();

            MediaPlayer.Stop();
            MusicSet.Clear();
        }

        // updates

        internal static void Update()
        {
			FrameworkDispatcher.Update(); // to let sound update
			List<int> toKill = new List<int>();

            foreach (var sound in ActiveSounds)
            {
                if (sound.Value == null || sound.Value.Instance.State == SoundState.Stopped)
                {
                    toKill.Add(sound.Key);
                    EffectEnded?.Invoke(null, new SoundEventArgs(sound.Key));
                }
                else if (UsePostionalSound && NeedListenerUpdate)
                {
                    if (sound.Value.Emitter != null)
                    {
                        sound.Value.Instance.Apply3D(Listener, sound.Value.Emitter);
                    }
                }
            }
            NeedListenerUpdate = false;

            foreach (var sound in toKill)
                ActiveSounds.Remove(sound);
        }

        // music

        private static void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            if (MusicSet.Count > 0)
            {
                if (MediaPlayer.State == MediaState.Stopped)
                {
                    string filename = MusicSet[0];
                    MusicSet.RemoveAt(0);
                    PlayMusic(filename);
                }
            }
        }

        public static void PlayMusic(string filename)
        {
            string path = AssetManager.GetAssetFullPath(filename);
            if (string.IsNullOrEmpty(path))
                return;

            MediaPlayer.Play(Song.FromUri(Path.GetFileNameWithoutExtension(filename), new Uri("file://" + path)));
        }

        public static void PlayMusicSet(List<string> filenames)
        {
            MusicSet = new List<string>(filenames.ToArray());
            MediaPlayer_MediaStateChanged(null, EventArgs.Empty);
        }

        public static void StopMusic()
        {
            MediaPlayer.Stop();
        }

        public static void SetMusicVolume(float volume)
        {
            MediaPlayer.Volume = System.Math.Min(volume / 100.0f, 100.0f);
        }

        public static float GetMusicVolume()
        {
            return MediaPlayer.Volume * 100.0f;
        }

        public static void SetMasterSoundVolume(float volume)
        {
            SoundEffect.MasterVolume = System.Math.Min(volume / 100.0f, 100.0f);
        }

        public static float GetMasterSoundVolume()
        {
            return SoundEffect.MasterVolume * 100.0f;
        }

        // sounds

        private static SoundEffect GetSound(string name)
        {
            if (SoundMap.ContainsKey(name))
                return SoundMap[name];

            Stream soundStream = AssetManager.GetAssetStream(name);
            if (soundStream == null)
                return null;

            SoundEffect sound = SoundEffect.FromStream(soundStream);
            SoundMap.Add(name, sound);
            return sound;
        }

        public static int PlaySound(string name, float pitch = 0.0f, float volume = 100.0f, bool loop = false)
        {
            SoundEffect sound = GetSound(name);
            if (sound == null)
                return InvalidSoundID;

            int id = MaxSoundID;

            for (int i = 1; i < MaxSoundID; i++)
            {
                if (!ActiveSounds.ContainsKey(i))
                {
                    id = i;
                    break;
                }
            }

            if (id == MaxSoundID)
                return InvalidSoundID;

            SoundEffectInstance instance = sound.CreateInstance();
			if (pitch != 0)
				instance.Pitch = pitch;
            instance.IsLooped = loop;
            instance.Volume = System.Math.Min(volume / 100.0f, 1.0f);
            instance.Play();

            ActiveSounds.Add(id, new SoundInstance(instance));
            return id;
        }

        public static  int PlaySound(string name, OpenTK.Vector3 position, float pitch = 0.0f, float volume = 100.0f, bool loop = false)
        {
            if (!UsePostionalSound)
                return PlaySound(name, pitch, volume, loop);

            SoundEffect sound = GetSound(name);
            if (sound == null)
                return InvalidSoundID;

            int id = MaxSoundID;

            for (int i = 1; i < MaxSoundID; i++)
            {
                if (!ActiveSounds.ContainsKey(i))
                {
                    id = i;
                    break;
                }
            }

            if (id == MaxSoundID)
                return InvalidSoundID;

            SoundEffectInstance instance = sound.CreateInstance();
			if (pitch != 0)
				instance.Pitch = pitch;
			instance.IsLooped = loop;
            AudioEmitter emmiter = new AudioEmitter();
            emmiter.Position = new Microsoft.Xna.Framework.Vector3(position.X, position.Y, position.Z);
            instance.Apply3D(Listener, emmiter);
            instance.Volume = System.Math.Min(volume / 100.0f, 1.0f);
            instance.Play();

            ActiveSounds.Add(id, new SoundInstance(instance,emmiter));
            return id;
        }

        public static void SetListenerPosition(OpenTK.Vector3 position, float angle)
        {
            UsePostionalSound = true;
            NeedListenerUpdate = true;

            Listener.Position = new Microsoft.Xna.Framework.Vector3(position.X, position.Y, position.Z);

            var forward = LudicrousElectron.Engine.Math.MathHelper.Vector3FromAngle(angle, false);
            Listener.Forward = new Microsoft.Xna.Framework.Vector3(forward.X, forward.Y, forward.Z);
        }

        public static void DisablePositionalSound()
        {
            UsePostionalSound = false;
            foreach (var instance in ActiveSounds)
            {
                if (instance.Value.Emitter != null)
                {
                    instance.Value.Emitter.Position = Listener.Position;
                    instance.Value.Instance.Apply3D(Listener, instance.Value.Emitter);
                }
            }
        }

        public static void StopSound(int index)
        {
            if (ActiveSounds.ContainsKey(index))
            {
                ActiveSounds[index].Instance.Stop();
                ActiveSounds.Remove(index);
                EffectEnded?.Invoke(null, new SoundEventArgs(index));
            }
        }

        public static void SetSoundVolume(int index, float volume) // Valid values 0.0f-100.0f
        {
            if (ActiveSounds.ContainsKey(index))
                ActiveSounds[index].Instance.Volume = (System.Math.Min(volume/100,1.0f));
        }

        public static float GetSoundVolume(int index)
        {
            if (ActiveSounds.ContainsKey(index))
                return ActiveSounds[index].Instance.Volume * 100.0f;

            return 0;
        }

        public static void SetSoundPitch(int index, float pitch) // Valid values 0.0f+; 1.0 = default
        {
            if (ActiveSounds.ContainsKey(index))
                ActiveSounds[index].Instance.Pitch = (System.Math.Min(pitch, 0));  
        }

        public static float GetSoundPitch(int index)
        {
            if (ActiveSounds.ContainsKey(index))
                return ActiveSounds[index].Instance.Pitch;

            return 0;
        }

        // Text To Speach
        public static void SetTextToSpeachVoice(string name)
        {
            SpeachEventArgs args = new SpeachEventArgs(name);
            SetTextVoice?.Invoke(null, args);
        }

        public static string GetTextToSpeachVoice()
        {
            SpeachEventArgs args = new SpeachEventArgs(string.Empty);
            GetTextVoice?.Invoke(null, args);
			return args.Text;
        }

        public static List<string> GetTextToSpeachVoices(bool female = true, bool male = true)
        {
			string t = string.Empty;
			if (female)
				t = "female;";
			if (male)
				t += "male;";

			SpeachEventArgs args = new SpeachEventArgs(t);
			GetTextVoiceList?.Invoke(null, args);

			if (!string.IsNullOrEmpty(args.Text))
				return new List<string>(args.Text.Split(";".ToCharArray()));

			return new List<string>();
        }

        public static void PlayTextToSpeech(string text)
        {
            SpeachEventArgs args = new SpeachEventArgs(text);
            SpeakText?.Invoke(null, args);
		}

        public static void StopTextToSpeech()
        {
            SpeachEventArgs args = new SpeachEventArgs(string.Empty);
            StopAllSpeach?.Invoke(null, args);
        }
    }
}
