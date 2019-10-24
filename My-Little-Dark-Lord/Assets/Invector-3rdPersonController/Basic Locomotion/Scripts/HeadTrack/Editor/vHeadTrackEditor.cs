using UnityEngine;
using UnityEditor;
using UnityEditor.Events;

namespace Invector.vCharacterController
{
    [CustomEditor(typeof(vHeadTrack))]
    public class vHeadTrackEditor : vEditorBase
    {
        Animator animator;
        vHeadTrack track;

        protected override void OnEnable()
        {
            base.OnEnable();
            AddEventToTpInput();
        }

        void AddEventToTpInput()
        {
            track = target as vHeadTrack;
            if (!track) return;

            var tpInput = track.GetComponent<vThirdPersonInput>();
            if (tpInput)
            {
                bool containsListener = false;
                for (int i = 0; i < tpInput.OnLateUpdate.GetPersistentEventCount(); i++)
                {
                    if (tpInput.OnLateUpdate.GetPersistentTarget(i).GetType().Equals(typeof(vHeadTrack)) && tpInput.OnLateUpdate.GetPersistentMethodName(i).Equals("UpdateHeadTrack"))
                    {
                        containsListener = true;
                        break;
                    }

                }
                if (!containsListener)
                {
                    UnityEventTools.AddPersistentListener(tpInput.OnLateUpdate, track.UpdateHeadTrack);
                    SerializedObject tpI = new SerializedObject(tpInput);
                    EditorUtility.SetDirty(tpInput);
                    tpI.ApplyModifiedProperties();
                }
            }

        } 
    }
}