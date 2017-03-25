using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Configurations
{
    [Serializable]
    public class GameConfig
    {
        public float MusicVolumeLevel;

        public bool MusicMute;

        public float EffectsVolumeLevel;

        public bool EffectsMute;

        public bool WeightUnitLb;

        public List<string> PlayersConfigPath;

        public GameConfig()
        {
            this.MusicMute = false;
            this.MusicVolumeLevel = 1.0f;
            this.EffectsMute = false;
            this.EffectsVolumeLevel = 1.0f;
            this.WeightUnitLb = false;
            this.PlayersConfigPath = new List<string>();
        }
    }
}
