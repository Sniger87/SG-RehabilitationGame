using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Configurations
{
    [Serializable]
    public class GameConfig
    {
        public int VolumeLevel;

        public bool Mute;

        public List<string> PlayersConfigPath;

        public GameConfig()
        {
            this.PlayersConfigPath = new List<string>();
        }
    }
}
