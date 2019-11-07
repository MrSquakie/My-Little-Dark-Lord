namespace GeNa
{
    /// <summary>
    /// GeNa Constants
    /// </summary>
    public static class Defaults
	{
		#region Undo Settings

		/// <summary>
		/// EditorPrefs key for the default Undo steps count.
		/// </summary>
		public const string UNDO_STEPS_KEY = "GeNa_UndoSteps";

		/// <summary>
		/// Default Undo steps count.
		/// </summary>
		public const int DEF_UNDO_STEPS = 50;

		/// <summary>
		/// EditorPrefs key for the default minutes after which Undo Records will be 
		/// automatically purged.
		/// </summary>
		public const string UNDO_PURGE_TIME_KEY = "GeNa_UndoPurgeTime";

		/// <summary>
		/// Default minutes after which Undo Records will be automatically purged.
		/// </summary>
		public const int DEF_UNDO_PURGE_TIME = 60;

		/// <summary>
		/// EditorPrefs key for the default seconds interval in which spawns are considered to 
		/// belong to a single Undo Group.
		/// </summary>
		public const string UNDO_GROUPING_TIME_KEY = "GeNa_UndoGroupingTime";

		/// <summary>
		/// Default seconds interval in which spawns are considered to belong to a single Undo 
		/// Group.
		/// </summary>
		public const int DEF_UNDO_GROUPING_TIME = 3;

        /// <summary>
        /// EditorPrefs key for the preference that enables console messages when expired undo records are purged.
        /// </summary>
        public const string UNDO_SHOW_EXPIRED_MSGS_KEY = "GeNa_UndoShowExpiredMessages";

        /// <summary>
        /// EditorPrefs key for the default value of 'Spawn To Target' preference.
        /// </summary>
        public const string DEF_SPAWN_TO_TARGET_KEY = "GeNa_DefaultSpawnToTarget";

        #endregion
    }
}
