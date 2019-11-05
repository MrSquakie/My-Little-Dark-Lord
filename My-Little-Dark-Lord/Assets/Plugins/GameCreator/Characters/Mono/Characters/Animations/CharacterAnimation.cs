namespace GameCreator.Characters
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Animations;
    using UnityEngine.Playables;

    public class CharacterAnimation
    {
        public enum Layer
        {
            Layer1,
            Layer2,
            Layer3,
        }

        private enum StateType
        {
            None,
            Norm,
            Mask
        }

        private class StateNode
        {
            public StateType activeState = StateType.None;
            public CharacterState stateAsset;
            public StatesNormAnimation statesNorm = new StatesNormAnimation();
            public StatesMaskAnimation statesMask = new StatesMaskAnimation();
        }

        private const int MAX_LAYERS = 3;

        private const string NAME_ANIMATOR_OUTPUT = "Character Output";

        // PROPERTIES: ----------------------------------------------------------------------------

        private CharacterAnimator characterAnimator;
        private RuntimeAnimatorController runtimeController;

        private PlayableGraph graph;
        private AnimationPlayableOutput animationOutput;

        private StateNode[] states;
        private GestureAnimation gesture;
        private GestureMaskAnimation gestureMask;

        // INITIALIZERS: --------------------------------------------------------------------------

        public CharacterAnimation(CharacterAnimator characterAnimator, CharacterState defaultState)
        {
            this.characterAnimator = characterAnimator;
            this.SetupTopology(defaultState == null 
                ? null
                : defaultState.GetRuntimeAnimatorController()
            );
        }

        public void SetupTopology(RuntimeAnimatorController controller = null)
        {
            controller = controller ?? characterAnimator.animator.runtimeAnimatorController;
            controller = controller ?? this.runtimeController;
            this.runtimeController = controller;

            this.characterAnimator.animator.runtimeAnimatorController = null;

            if (!this.graph.Equals(null) && this.graph.IsValid()) this.graph.Destroy();
            this.graph = PlayableGraph.Create();

            if (!this.characterAnimator.animator.playableGraph.Equals(null) &&
                this.characterAnimator.animator.playableGraph.IsValid())
            {
                this.characterAnimator.animator.playableGraph.Destroy();
            }

            this.animationOutput = AnimationPlayableOutput.Create(
                this.graph,
                NAME_ANIMATOR_OUTPUT,
                this.characterAnimator.animator
            );

            Playable previousPlayable = AnimatorControllerPlayable.Create(
                this.graph,
                this.runtimeController
            );

            this.states = new StateNode[MAX_LAYERS];
            for (int i = 0; i < MAX_LAYERS; ++i)
            {
                this.states[i] = new StateNode();
                previousPlayable = this.states[i].statesNorm.Setup(this.graph, previousPlayable);
                previousPlayable = this.states[i].statesMask.Setup(this.graph, previousPlayable);
            }

            this.gesture = new GestureAnimation();
            previousPlayable = this.gesture.Setup(this.graph, previousPlayable);

            this.gestureMask = new GestureMaskAnimation();
            previousPlayable = this.gestureMask.Setup(this.graph, previousPlayable);
            
            this.animationOutput.SetSourcePlayable(previousPlayable);

            #if UNITY_2018_2_OR_NEWER
            this.animationOutput.SetSourceOutputPort(0);
            #else
            this.animationOutput.SetSourceInputPort(0);
            #endif

            this.graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            this.graph.Play();
        }

        public void OnDestroy()
        {
            if (!this.graph.Equals(null)) this.graph.Destroy();
        }

        public void Update()
        {
            for (int i = 0; i < this.states.Length; ++i)
            {
                this.states[i].statesNorm.Update();
                this.states[i].statesMask.Update();
            }

            this.gesture.Update();
            this.gestureMask.Update();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void PlayGesture(AnimationClip clip, AvatarMask avatarMask, float speed,
                                float transitionIn = GestureAnimation.TRANSITION,
                                float transitionOut = GestureAnimation.TRANSITION)
        {
            if (avatarMask == null) this.gesture.PlayGesture(clip, speed, transitionIn, transitionOut);
            else this.gestureMask.PlayGesture(clip, avatarMask, speed, transitionIn, transitionOut);
        }

        public void StopGesture(float transitionOut = 0.0f)
        {
            this.gesture.StopGesture(transitionOut);
            this.gestureMask.StopGesture(transitionOut);
        }

        public void SetState(AnimationClip clip, AvatarMask avatarMask,
                             float weight, float time, float speed, Layer layer)
        {
            if (avatarMask == null)
            {
                this.states[(int)layer].activeState = StateType.Norm;
                this.states[(int)layer].statesNorm.SetState(clip, weight, time, speed);
                this.states[(int)layer].statesMask.ResetState(time);
            }
            else
            {
                this.states[(int)layer].activeState = StateType.Mask;
                this.states[(int)layer].statesNorm.ResetState(time);
                this.states[(int)layer].statesMask.SetState(clip, avatarMask, weight, time, speed);
            }
        }

        public void SetState(CharacterState stateAsset, AvatarMask avatarMask,
                            float weight, float time, float speed, Layer layer)
        {
            stateAsset.StartState(this);
            this.states[(int)layer].stateAsset = stateAsset;
            this.SetState(
                stateAsset.GetRuntimeAnimatorController(), avatarMask, 
                weight, time, speed, layer
            );
        }

        public void SetState(RuntimeAnimatorController rtc, AvatarMask avatarMask, 
                             float weight, float time, float speed, Layer layer)
        {
            if (avatarMask == null)
            {
                this.states[(int)layer].activeState = StateType.Norm;
                this.states[(int)layer].statesNorm.SetState(rtc, weight, time, speed);
                this.states[(int)layer].statesMask.ResetState(time);
            }
            else
            {
                this.states[(int)layer].activeState = StateType.Mask;
                this.states[(int)layer].statesNorm.ResetState(time);
                this.states[(int)layer].statesMask.SetState(rtc, avatarMask, weight, time, speed);
            }
        }

        public void ResetState(float time, Layer layer)
        {
            if (this.states[(int)layer].stateAsset != null)
            {
                this.states[(int)layer].stateAsset.ExitState(this);
            }

            this.states[(int)layer].activeState = StateType.None;
            this.states[(int)layer].statesNorm.ResetState(time);
            this.states[(int)layer].statesMask.ResetState(time);
        }

        public void ChangeStateWeight(Layer layer, float weight)
        {
            switch (this.states[(int)layer].activeState)
            {
                case StateType.Norm : this.states[(int)layer].statesNorm.SetWeight(weight); break;
                case StateType.Mask : this.states[(int)layer].statesMask.SetWeight(weight); break;
            }
        }

        public CharacterState GetState(Layer layer)
        {
            if (this.states[(int)layer].stateAsset == null) return null;
            return this.states[(int)layer].stateAsset;
        }
    }
}