using System;
using System.Collections;
using System.Collections.Generic;

namespace Profiles
{
    [Serializable]
    public class Player
    {
        #region Eigenschaften
        public int Id { get; set; }
        public string Name { get; set; }
        public string DirectoryPath { get; set; }
        public List<int> Highscores { get; set; }
        public List<string> MovementFileNames { get; set; }
        #endregion

        #region Konstruktor
        public Player()
        {

        }

        public Player(int id, string name, string directoryPath)
        {
            this.Id = id;
            this.Name = name;
            this.DirectoryPath = directoryPath;
            this.Highscores = new List<int>();
            this.MovementFileNames = new List<string>();
        }
        #endregion
    }
}
