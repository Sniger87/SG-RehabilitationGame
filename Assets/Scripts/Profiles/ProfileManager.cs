using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Configurations;
using FileIO;
using UnityEngine;

namespace Profiles
{
    public class ProfileManager
    {
        #region Konstanten
        private const string PlayerFolderPrefix = "Player";
        #endregion

        #region Felder
        private static ProfileManager current;

        private List<Player> players;
        private Player currentPlayer;
        #endregion

        #region Eigenschaften
        public static ProfileManager Current
        {
            get
            {
                if (current == null)
                {
                    current = new ProfileManager();
                }
                return current;
            }
        }

        public List<Player> Players
        {
            get
            {
                return players;
            }
            set
            {
                if (value != players)
                {
                    players = value;
                }
            }
        }

        public Player CurrentPlayer
        {
            get
            {
                return currentPlayer;
            }
            set
            {
                if (value != currentPlayer)
                {
                    currentPlayer = value;
                }
            }
        }
        #endregion

        #region Konstruktor
        private ProfileManager()
        {
            this.Players = new List<Player>();
        }
        #endregion

        #region Implementierungen
        public Player CreatePlayer(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(name, "Name des Spielers darf nicht leer sein.");
            }

            int playerId = this.Players.Count;
            string playerFolderName = string.Format("{0}_{1}", PlayerFolderPrefix, playerId);

            string directoryPath = Path.Combine(ConfigManager.GameFolderName, playerFolderName);

            FileManager.CreateDirectory(directoryPath);

            Player p = new Player(playerId, name, directoryPath);

            this.Players.Add(p);

            ConfigManager.Current.GameConfig.PlayersDirectoryPath.Add(directoryPath);

            return p;
        }

        public void LoadPlayers()
        {
            this.Players.Clear();

            foreach (string playerFolderPath in ConfigManager.Current.GameConfig.PlayersDirectoryPath)
            {
                string content = FileManager.Read(playerFolderPath);
                Player p = JsonUtility.FromJson<Player>(content);
                this.Players.Add(p);
            }
        }

        public bool DeletePlayer(Player player)
        {
            if (this.Players.Contains(player))
            {
                return this.Players.Remove(player);
            }
            return false;
        }

        public void CreateFileAtCurrentPlayer()
        {

        }

        public void WriteFileAtCurrentPlayer()
        {

        }
        #endregion
    }
}
