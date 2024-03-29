﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeKeep.Domain.SoundController
{
    using System.Diagnostics;
    using System.Media;
    using System.Threading;

    using TimeKeep.Domain.RoundManager;
    using TimeKeep.Foundation.Threading;
    using TimeKeep.Foundation.Threading.Interfaces;
    using TimeKeep.Properties;
    using System.Collections.Specialized;

    public class SoundManager
    {

        public struct SoundKey
        {
            public TimeSpan Time { get; set; }
            public ManagerPhase Phase { get; set; }
        }

        private readonly Dictionary<SoundKey, ISoundDefinition> _sounds;

        public SoundManager()
        {
            _sounds = new Dictionary<SoundKey, ISoundDefinition>();
            InitSounds();
        }

        private void InitSounds()
        {
            if (Settings.Default.SoundTemplates == null)
            {
                CreateDefaultSoundTemplates();
            }

            foreach (var stringDef in Settings.Default.SoundTemplates)
            {
                var template = SoundTemplate.FromString(stringDef);
                if (template.IsActive)
                {
                    AddSound(template.Definition);
                }
            }
        }

        public void ReloadSounds() 
        {
            Sounds.Clear();
            InitSounds();
        }

        private void CreateDefaultSoundTemplates()
        {
            if (Settings.Default.SoundTemplates == null)
            {
                var list = new StringCollection();
                var template = new SoundTemplate { IsActive = false, Definition = new RoundEndBoxingBellSoundDefinition() };
                list.Add(template.ToString());

                template = new SoundTemplate { IsActive = false, Definition = new RoundBeginBoxingBellSoundDefinition() };
                list.Add(template.ToString());

                template = new SoundTemplate { IsActive = false, Definition = new TrippleWoodTockSoundDefinition() };
                list.Add(template.ToString());

                template = new SoundTemplate { IsActive = false, Definition = new DingSoundDefinition() };
                list.Add(template.ToString());

                template = new SoundTemplate { IsActive = false, Definition = new DingSoundDefinition(ManagerPhase.Pause) };
                list.Add(template.ToString());
                
                

                Settings.Default.SoundTemplates = list;
                Settings.Default.Save();
            }
        }

        private void AddSound(ISoundDefinition def)
        {
            var key = new SoundKey { Phase = def.PhaseToPlaySound, Time = def.BeginPlayback };
            if (!Sounds.ContainsKey(key))
            {
                Sounds.Add(key, def);
            }
        }

        public void ProcessSounds(TimeSpan origin, ManagerPhase phase)
        {
            var key = new SoundKey { Phase = phase, Time = origin };
            if (Sounds.ContainsKey(key))
            {
                var def = Sounds[key];
                if (def.IsRepeatPlaybackEnabled)
                {
                    var thread = new Thread(PlayRepeatSound);
                    thread.Start(def);
                }
                else
                {
                    PlaySound(def.SoundLocation);
                }
            }
        }

        private void PlayRepeatSound(object param)
        {
            var def = param as ISoundDefinition;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var sp = new SoundPlayer(def.SoundLocation))
            {
                while (stopwatch.Elapsed < def.RepeatDuration)
                {
                    sp.PlaySync();
                }
            }

            stopwatch.Stop();
        }

        private void PlaySound(string soundLocation)
        {
            using (var sp = new SoundPlayer(soundLocation))
            {
                sp.Play();
            }
        }


        private void PlaySoundInBackgroundThread(IBackgroundAction action)
        {

        }

        public Dictionary<SoundKey, ISoundDefinition> Sounds
        {
            get
            {
                return _sounds;
            }
        }
    }
}
