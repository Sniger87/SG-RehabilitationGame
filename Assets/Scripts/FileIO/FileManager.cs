using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FileIO {
	public static class FileManager {
		#region Felder

		#endregion

		#region Eigenschaften

		#endregion

		#region Konstruktor

		#endregion

		#region Implementierungen

		public static string Read (string path) {
			if (string.IsNullOrEmpty (path)) {
				throw new ArgumentNullException (path, "Pfad der Datei darf nicht leer sein.");
			}

			if (!File.Exists (path)) {
				throw new FileNotFoundException ("Datei wurde nicht gefunden.");
			}

			return File.ReadAllText (path);
		}

		public static void Write (string path, string contents) {
			if (string.IsNullOrEmpty (path)) {
				throw new ArgumentNullException (path, "Pfad der Datei darf nicht leer sein.");
			}

			if (!File.Exists (path)) {
				throw new FileNotFoundException ("Datei wurde nicht gefunden.");
			}

			File.WriteAllText (path, contents);
		}

		public static void Append (string path, string contents) {
			if (string.IsNullOrEmpty (path)) {
				throw new ArgumentNullException (path, "Pfad der Datei darf nicht leer sein.");
			}

			if (!File.Exists (path)) {
				throw new FileNotFoundException ("Datei wurde nicht gefunden.");
			}

			File.AppendAllText (path, contents);
		}

		public static bool Create (string path, bool overwrite = false) {
			if (string.IsNullOrEmpty (path)) {
				throw new ArgumentNullException (path, "Pfad der Datei darf nicht leer sein.");
			}

			if (File.Exists (path)) {
				if (overwrite) {
					Delete (path);
				} else {
					return false;
				}
			}

			FileStream fs = File.Create (path);
			fs.Close ();

			return true;
		}

		public static bool Delete (string path) {
			if (string.IsNullOrEmpty (path)) {
				throw new ArgumentNullException (path, "Pfad der Datei darf nicht leer sein.");
			}

			if (!File.Exists (path)) {
				return false;
			}

			File.Delete (path);

			return true;
		}

		public static bool CreateDirectory (string path) {
			if (string.IsNullOrEmpty (path)) {
				throw new ArgumentNullException (path, "Pfad des Ordners darf nicht leer sein.");
			}

			if (Directory.Exists (path)) {
				return false;
			}

			Directory.CreateDirectory (path);

			return true;
		}

		public static bool DeleteDirectory (string path, bool recursive = false) {
			if (string.IsNullOrEmpty (path)) {
				throw new ArgumentNullException (path, "Pfad des Ordners darf nicht leer sein.");
			}

			if (!Directory.Exists (path)) {
				return false;
			}

			Directory.Delete (path, recursive);

			return true;
		}

		public static void AppendLog (string logPath, string content) {
			// create if not exists
			Create (logPath);
			// append content to log
			Append (logPath, content + Environment.NewLine);
		}
		#endregion
	}
}
