using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class GestureMaskAnimation
{
    public const float TRANSITION = 0.25f;

    // PROPERTIES: --------------------------------------------------------------------------------

    protected PlayableGraph graph;
    protected AnimationLayerMixerPlayable mixer;
    protected AnimationClipPlayable gesturePlayable;

    private bool gesturePlaying = false;
    private float gestureTransitionIn = TRANSITION;
    private float gestureTransitionOut = TRANSITION;
    private float gestureDuration = 0.0f;

    // INITIALIZERS: ------------------------------------------------------------------------------

    public Playable Setup(PlayableGraph graph, Playable input)
    {
        this.graph = graph;

        this.mixer = AnimationLayerMixerPlayable.Create(this.graph, 2);
        this.mixer.ConnectInput(0, input, 0);
        this.mixer.ConnectInput(1, Playable.Null, 0);

        mixer.SetInputWeight(0, 1.0f);
        mixer.SetInputWeight(1, 0.0f);

        return this.mixer;
    }

    // PUBLIC METHODS: ----------------------------------------------------------------------------

    public virtual void PlayGesture(AnimationClip clip, AvatarMask avatarMask, float speed,
                                    float transitionIn = TRANSITION, float transitionOut = TRANSITION)
    {
        if (clip == null) return;

        this.gesturePlayable = AnimationClipPlayable.Create(this.graph, clip);
        this.gesturePlayable.SetTime(0f);
        this.gesturePlayable.SetSpeed(speed);
        this.gesturePlayable.SetDuration(clip.length);

        this.graph.Disconnect(this.mixer, 1);
        this.graph.Connect(this.gesturePlayable, 0, this.mixer, 1);

        this.mixer.SetInputWeight(0, 1.0f);
        this.mixer.SetInputWeight(1, 0.0f);
        this.mixer.SetLayerMaskFromAvatarMask(1, avatarMask);

        this.gesturePlaying = true;
        this.gestureDuration = clip.length;
        this.gestureTransitionIn = transitionIn;
        this.gestureTransitionOut = transitionOut;
    }

    public void StopGesture(float transition = 0.0f)
    {
        if (Mathf.Approximately(transition, 0.0f))
        {
            this.mixer.SetInputWeight(0, 1.0f);
            this.graph.Disconnect(this.mixer, 1);
            this.gesturePlaying = false;
        }
        else if (this.gesturePlayable.IsValid())
        {
            this.gestureTransitionOut = transition;
            float time = (float)this.gesturePlayable.GetTime();
            this.gestureDuration = time + this.gestureTransitionOut;
        }
    }

    public void Update()
    {
        if (this.gesturePlaying)
        {
            if (this.gesturePlayable.IsDone())
            {
                this.StopGesture();
                return;
            }
            else
            {
                float time = (float)this.gesturePlayable.GetTime();
                if (time + this.gestureTransitionOut >= this.gestureDuration)
                {
                    float t = (this.gestureDuration - time) / this.gestureTransitionOut;

                    t = Mathf.Clamp01(t);
                    mixer.SetInputWeight(1, t);
                }
                else if (time <= this.gestureTransitionIn)
                {
                    float t = time / this.gestureTransitionIn;

                    t = Mathf.Clamp01(t);
                    mixer.SetInputWeight(1, t);
                }
                else
                {
                    mixer.SetInputWeight(1, 1.0f);
                }
            }
        }
    }
}