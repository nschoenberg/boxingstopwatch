﻿namespace TimeKeep.C.Domain.Extensions
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    using TimeKeep.Domain.RoundManager;
using TimeKeep.Domain.SoundController;
using System.Collections;
using System.Collections.Generic;

    public static class ExtensionMethods
    {
        public static StringCollection ToStringCollection(this ObservableCollection<RoundTemplate> self)
        {
            // Attention: If you change this, change ToRoundTemplateCollection as well
            var result = new StringCollection();
            foreach (var template in self)
            {
                result.Add(
                    string.Format(
                        "{0};{1};{2}",
                        template.Definition.GetMaxRounds(),
                        template.Definition.GetPauseTimeInSeconds(),
                        template.Definition.GetRoundTimeInSeconds()));
            }

            return result;
        }

        public static ObservableCollection<RoundTemplate> ToRoundTemplateCollection(this StringCollection self)
        {
            // Attention: If you change this, change AddFromStringCollection as well       
            var result = new ObservableCollection<RoundTemplate>();
            var sep = new char[] { ';' };
            foreach (var line in self)
            {
                var seperated = line.Split(sep);

                result.Add(new RoundTemplate(new StaticRoundDefinition
                {
                    MaxRounds = int.Parse(seperated[0]),
                    PauseInSeconds = int.Parse(seperated[1]),
                    RoundInSeconds = int.Parse(seperated[2])
                }));
            }

            return result;
        }


        public static StringCollection ToStringCollection(this IEnumerable<SoundTemplate> self)
        {
            var result = new StringCollection();

            foreach (var definition in self)
            {
                result.Add(definition.ToString());
            }

            return result;
        }

        public static ObservableCollection<SoundTemplate> ToSoundDefinitionCollection(this StringCollection self)
        {
            var result = new ObservableCollection<SoundTemplate>();
            var sep = new char[] { ';' };
            foreach (var line in self)
            {
                var template = SoundTemplate.FromString(line);
                result.Add(template);
            }

            return result;
        }
    }
}
