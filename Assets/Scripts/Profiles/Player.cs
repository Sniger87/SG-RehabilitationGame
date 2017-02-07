using System;
using System.Collections;
using System.Collections.Generic;

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

        public List<int> Highscores;

        public List<string> MovementFilePaths;

        public string CurrentMovementFilePath { get; set; }
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
            this.MovementFilePaths = new List<string>();
        }
        #endregion
    }
}
