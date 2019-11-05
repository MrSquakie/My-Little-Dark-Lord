﻿namespace GameCreator.Characters
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CharacterState : ScriptableObject
    {
        public enum StateType
        {
            Simple,
            Locomotion,
            Other
        }

        public StateType type = StateType.Simple;

        public AnimatorOverrideController rtcSimple;
        public AnimatorOverrideController rtcLocomotion;
        public RuntimeAnimatorController rtcOther;

        public AnimationClip enterClip;
        public AvatarMask enterAvatarMask;

        public AnimationClip exitClip;
        public AvatarMask exitAvatarMask;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public RuntimeAnimatorController GetRuntimeAnimatorController()
        {
            switch (this.type)
            {
                case StateType.Simple : return this.rtcSimple;
                case StateType.Locomotion : return this.rtcLocomotion;
                case StateType.Other : return this.rtcOther;
            }

            return null;
        }

        public void StartState(CharacterAnimation character)
        {
            if (character == null) return;
            if (this.enterClip == null) return;

            character.PlayGesture(this.enterClip, this.enterAvatarMask, 1.0f, 0.25f, 0.1f);
        }

        public void ExitState(CharacterAnimation character)
        {
            if (character == null) return;
            if (this.exitClip == null) return;

            character.PlayGesture(this.exitClip, this.exitAvatarMask, 1.0f, 0.1f, 0.25f);
        }
    }
}