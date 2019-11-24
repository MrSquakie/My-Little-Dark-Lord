// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using PWCommon2;
using System.Text.RegularExpressions;

// Editor only
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GeNa
{
	/// <summary>
	/// Interface class for the GeNa settings. Handles storing and retrieving all scoped setting, including Editor scope settings.
	/// </summary>
    [XmlRoot("GeNaPreferences")]
    public class Preferences
	{
		#region Constants, Instance and other helper variables

		private const string ID = "GeNa";
		private const string PREF_FILE = "Preferences-" + ID;
		private const string PREF_EXTENSION = ".txt";

		private const string GENA_FOLDER_NAME = "GeNa";
		private const string GENA_RES_INTERNAL_PATH = "Scripts/Resources";

		private static bool ms_dirty = false;

		/// <summary>
		/// The instance that will hold the values in memory.
		/// </summary>
		private static Preferences Instance
		{
			get
			{
				if (m_instance == null)
				{
					Load();
				}

				return m_instance;
			}
		}
		private static Preferences m_instance;

		#endregion

		#region Undo Settings

		/// <summary>
		/// The maximum possible undo steps on each Spawner.
		/// </summary>
		public static int UndoSteps
		{
			get
			{
				return Instance.m_undoSteps;
			}
			set
			{
				// Minimum 5
				if (value != Instance.m_undoSteps && value > 5)
				{
					Instance.m_undoSteps = Mathf.Clamp(value, 0, 250);
#if UNITY_EDITOR
					Save();
					EditorPrefs.SetInt(Defaults.UNDO_STEPS_KEY, Instance.m_undoSteps);

					// Rotate any GameObject selection to reinit a possibly selected spawner.
					GameObject selected = Selection.activeGameObject;
					Selection.activeGameObject = null;
					Selection.activeGameObject = selected;
#endif
				}
			}
		}
		// The instance is private and the ctor is internal so this can be public for the XML serialiser.
		public int m_undoSteps = Defaults.DEF_UNDO_STEPS;

		/// <summary>
		/// Minutes after which Undo Records will be automatically purged.
		/// </summary>
		public static int UndoPurgeTime
		{
			get
			{
				return Instance.m_undoPurgeTime;
			}
			set
			{
				if (value != Instance.m_undoPurgeTime && value > 0)
				{
					// Max 5 days
					Instance.m_undoPurgeTime = Mathf.Clamp(value, 1, 5 * 24 * 60);
#if UNITY_EDITOR
					Save();
					EditorPrefs.SetInt(Defaults.UNDO_PURGE_TIME_KEY, Instance.m_undoPurgeTime);
#endif
				}
			}
		}
		// The instance is private and the ctor is internal so this can be public for the XML serialiser.
		public int m_undoPurgeTime = Defaults.DEF_UNDO_PURGE_TIME;

		/// <summary>
		/// Parses Purge Time from the provided string (formats hh:mm, or mmmmmm)
		/// </summary>
		public static void SetUndoPurgeTime(string timeString)
		{
			int minutes = 0;
			Match match = Regex.Match(timeString, @"^\s*\d+\s*$");
			if (match.Success)
			{
				int.TryParse(match.Value, out minutes);
			}
			else
			{
				match = Regex.Match(timeString, @"^\s*(\d{1,3})\s*\:\s*(\d{1,2})\s*$");
				if (match.Success)
				{
					int hours = 0;
					int.TryParse(match.Groups[1].Value, out hours);
					int.TryParse(match.Groups[2].Value, out minutes);

					minutes += 60 * hours;
				}
			}

			UndoPurgeTime = minutes;
		}

		/// <summary>
		/// Actions that happened less than or this many seconds apart will be grouped 
		/// in the Undo List to help the user identify actions that were part of the same 
		/// collective action (e.g. spawned monsters next to the entracnce).
		/// </summary>
		public static int UndoGroupingTime
		{
			get
			{
				return Instance.m_undoGroupingTime;
			}
			set
			{
				if (value != Instance.m_undoGroupingTime && value >= 0)
				{
					Instance.m_undoGroupingTime = Mathf.Clamp(value, 0, 120);
#if UNITY_EDITOR
					Save();
					EditorPrefs.SetInt(Defaults.UNDO_GROUPING_TIME_KEY, Instance.m_undoGroupingTime);
#endif
				}
			}
		}
		// The instance is private and the ctor is internal so this can be public for the XML serialiser.
		public int m_undoGroupingTime = Defaults.DEF_UNDO_GROUPING_TIME;

        /// <summary>
        /// When enabled, console messages inform about expired undo records.
        /// </summary>
        public static bool UndoExpiredMessages
        {
			get
			{
				return Instance.m_undoExpiredMessages;
			}
			set
			{
				if (value != Instance.m_undoExpiredMessages)
				{
					Instance.m_undoExpiredMessages = value;
#if UNITY_EDITOR
					Save();
					EditorPrefs.SetBool(Defaults.UNDO_SHOW_EXPIRED_MSGS_KEY, Instance.m_undoExpiredMessages);
#endif
				}
			}
		}
		// The instance is private and the ctor is internal so this can be public for the XML serialiser.
		public bool m_undoExpiredMessages = true;

		#endregion

		#region Advanced Settings

        /// <summary>
        /// The default 'Spawn To Target' value for new spawners.
        /// </summary>
        public static bool DefaultSpawnToTarget
        {
			get
			{
				return Instance.m_defSpawnToTarget;
			}
			set
			{
				if (value != Instance.m_defSpawnToTarget)
				{
					Instance.m_defSpawnToTarget = value;
#if UNITY_EDITOR
					Save();
					EditorPrefs.SetBool(Defaults.DEF_SPAWN_TO_TARGET_KEY, Instance.m_defSpawnToTarget);
#endif
				}
			}
		}
		// The instance is private and the ctor is internal so this can be public for the XML serialiser.
		public bool m_defSpawnToTarget = true;

		#endregion

		#region Constructors and related

		/// <summary>
		/// Instances of this mean nothing to users. This should be only used by deserialisation.
		/// </summary>
		private Preferences() { }

		#endregion

		#region Editor Settings Syncing and helpers

		/// <summary>
		/// Syncronises settings of an instance with the Editor. 
		/// See login in <seealso cref="EditorNeedsUpdate{T}(T, ref T, T)"/>.
		/// </summary>
		protected static void SyncWithEditor()
		{
#if UNITY_EDITOR
			if (m_instance == null)
			{
				Debug.LogErrorFormat("[GeNa] Can't call Editor syncing on null Preferences. Aborting.");
				return;
			}

			// Undo Settings
			SyncSetting(Defaults.UNDO_STEPS_KEY, ref m_instance.m_undoSteps, -1);
			SyncSetting(Defaults.UNDO_PURGE_TIME_KEY, ref m_instance.m_undoPurgeTime, -1);
			SyncSetting(Defaults.UNDO_GROUPING_TIME_KEY, ref m_instance.m_undoGroupingTime, -1);
			SyncSetting(Defaults.UNDO_SHOW_EXPIRED_MSGS_KEY, ref m_instance.m_undoExpiredMessages, true);

            // Advanced Settings
			SyncSetting(Defaults.DEF_SPAWN_TO_TARGET_KEY, ref m_instance.m_defSpawnToTarget, true);

			if (ms_dirty)
			{
				Save();
			}
#endif
		}

#if UNITY_EDITOR
		/// <summary>
		/// Syncs the Editor settings with the ones stored in the project, so they can be used runtime.
		/// </summary>
		private static void SyncSetting(string key, ref bool value, bool nonSetValue)
		{
			bool editorVal = EditorPrefs.GetBool(key, nonSetValue);
			if (EditorNeedsUpdate<bool>(editorVal, ref value, nonSetValue))
			{
				EditorPrefs.SetBool(key, value);
			}
		}

		/// <summary>
		/// Syncs the Editor settings with the ones stored in the project, so they can be used runtime.
		/// </summary>
		private static void SyncSetting(string key, ref float value, float nonSetValue)
		{
			float editorVal = EditorPrefs.GetFloat(key, nonSetValue);
			if (EditorNeedsUpdate<float>(editorVal, ref value, nonSetValue))
			{
				EditorPrefs.SetFloat(key, value);
			}
		}

		/// <summary>
		/// Syncs the Editor settings with the ones stored in the project, so they can be used runtime.
		/// </summary>
		private static void SyncSetting(string key, ref int value, int nonSetValue)
		{
			int editorVal = EditorPrefs.GetInt(key, nonSetValue);
			if (EditorNeedsUpdate<int>(editorVal, ref value, nonSetValue))
			{
				EditorPrefs.SetInt(key, value);
			}
		}

		/// <summary>
		/// Syncs the Editor settings with the ones stored in the project, so they can be used runtime.
		/// </summary>
		private static void SyncSetting(string key, ref string value, string nonSetValue)
		{
			string editorVal = EditorPrefs.GetString(key, nonSetValue);
			if (EditorNeedsUpdate<string>(editorVal, ref value, nonSetValue))
			{
				EditorPrefs.SetString(key, value);
			}
		}

		/// <summary>
		/// This is where the logic is implemented, which decides how we sync. Returns true if the Editor needs to be updated and handles everything else.
		/// </summary>
		/// <returns>Returns true if the Editor needs to be updated.</returns>
		private static bool EditorNeedsUpdate<T>(T editorVal, ref T storedVal, T nonSetValue) where T : IComparable
		{
			if (editorVal.CompareTo(storedVal) == 0)
			{
				return false;
			}

			// Editor setting takes precedence if it has been set
			if (editorVal.CompareTo(nonSetValue) != 0)
			{
				storedVal = editorVal;
				// Set dirty
				ms_dirty = true;
				return false;
			}

			// Let's update the editor setting otherwise.
			// For example, this could be a computer reinstall and we are getting the settings from version control.
			return true;
		}
#endif

		#endregion

		#region Serialisation

		/// <summary>
		/// Load the preferences.
		/// </summary>
		public static void Load()
		{
			TextAsset asset = Resources.Load<TextAsset>(PREF_FILE);
			if (asset != null)
			{
				m_instance = Deserialize(asset.text);
				if (m_instance == null)
				{
					Debug.LogError("[GeNa] Unable to get data from preference storage. Creating a new one.");
					m_instance = new Preferences();
					// Force save to save the new storage.
					ms_dirty = true;
				}
			}
			else
			{
				Debug.LogWarningFormat("[GeNa] Unable to locate GeNa preferences. Falling back to the defaults.");
				m_instance = new Preferences();
				// Force save to save to fix the corrupted storage.
				ms_dirty = true;
			}

			// Sync once loaded or created
			SyncWithEditor();
		}

		/// <summary>
		/// Deserialize preferences from XML string
		/// </summary>
		/// <param name="xmlString">XML code to deserialize</param>
		internal static Preferences Deserialize(string xmlString)
		{
			var serializer = new XmlSerializer(typeof(Preferences));
			return serializer.Deserialize(new StringReader(xmlString)) as Preferences;
		}

		//Only makes sense to save in Editor
#if UNITY_EDITOR
		/// <summary>
		/// Save the preferences.
		/// </summary>
		public static void Save()
		{
			bool needDBRefresh = false;

			TextAsset asset = Resources.Load<TextAsset>(PREF_FILE);
			string path = UnityEditor.AssetDatabase.GetAssetPath(asset);

			if (string.IsNullOrEmpty(path))
			{
				Debug.LogWarningFormat("Unable to locate GeNa preferences to save to. " +
					"Creating a new storage to store the settings.");
				path = GetPreferencesPath();
				needDBRefresh = true;
			}

			if (IsPathCorrect(path) == false)
			{
				Debug.LogErrorFormat("[GeNa] Preferences saving aborted. Were GeNa preferences moved or copied? Path is inccorrect: '{0}'.", path);
				return;
			}

			var serializer = new XmlSerializer(typeof(Preferences));
			using (var stream = new FileStream(path, FileMode.Create))
			{
				serializer.Serialize(stream, m_instance);
			}

			if (needDBRefresh)
			{
				AssetDatabase.Refresh();
			}
		}

		/// <summary>
		/// Gets path for a new preference file.
		/// </summary>
		private static string GetPreferencesPath()
		{
			string resFolder = GetResFolder();
			if (string.IsNullOrEmpty(resFolder))
			{
				Debug.LogErrorFormat("Unable to locate the GeNa resources folder at '{0}/{1}'", GENA_FOLDER_NAME, GENA_RES_INTERNAL_PATH);
				return null;
			}

			return resFolder + "/" + PREF_FILE + PREF_EXTENSION;
		}

		/// <summary>
		/// Checks if the path points to the desired folder.
		/// </summary>
		private static bool IsPathCorrect(string prefPath)
		{
			string resFolder = GetResFolder();
			if (string.IsNullOrEmpty(resFolder))
			{
				Debug.LogErrorFormat("Unable to locate the GeNa resources folder at '{0}/{1}'", GENA_FOLDER_NAME, GENA_RES_INTERNAL_PATH);
				return false;
			}

			if (Path.GetFullPath(prefPath) == Path.GetFullPath(resFolder + "/" + PREF_FILE + PREF_EXTENSION))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Returns the GeNa Resources folder or null.
		/// </summary>
		private static string GetResFolder()
		{
			return Utils.GetAppsSubfolder(GENA_FOLDER_NAME, GENA_RES_INTERNAL_PATH);
		}
#endif

		#endregion
	}
}
