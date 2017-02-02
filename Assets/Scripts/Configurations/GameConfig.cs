using System;
using System.Collections;
using System.Collections.Generic;

namespace Configurations
{
    [Serializable]
    public class GameConfig
    {
        public int VolumeLevel { get; set; }
        public bool Mute { get; set; }
        public List<string> PlayersDirectoryPath { get; set; }

        public GameConfig()
        {

        }
    }
}
