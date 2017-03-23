using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Profiles
{
    [Serializable]
    public class Player
    {
        #region Eigenschaften
        public int Id;

        public int Coins;

        public int Collisions;

        public string Name;

        public string DirectoryPath;

        public string ConfigFilePath;

        public int CurrentHighscore { get; set; }

        public List<int> Highscores;

        public string CurrentMovementFilePath { get; set; }

        public string CurrentPlayerPositionFilePath { get; set; }

        public string CurrentLevelPostionFilePath { get; set; }

        public string CurrentGameFolderPath { get; set; }
        #endregion

        #region Konstruktor
        public Player()
        {

        }

        public Player(int id, string name, string directoryPath, string configFilePath)
        {
            this.Id = id;
            this.Name = name;
            this.DirectoryPath = directoryPath;
            this.ConfigFilePath = configFilePath;
            this.Highscores = new List<int>();
        }
        #endregion
    }
}
