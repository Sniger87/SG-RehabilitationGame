using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Configurations
{
    [Serializable]
    public class GameConfig
    {
        public float VolumeLevel;

        public bool Mute;

        public List<string> PlayersConfigPath;

        public GameConfig()
        {
            this.Mute = false;
            this.VolumeLevel = 1.0f;
            this.PlayersConfigPath = new List<string>();
        }
    }
}
