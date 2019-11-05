using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class StatesMaskAnimation
{
    private const float SMOOTH_TIME = 0.2f;

    // PROPERTIES: --------------------------------------------------------------------------------

    private PlayableGraph graph;
    private AnimationLayerMixerPlayable mixer;

    private int currentStateHash = 0;
    private float currentWeight = 1.0f;

    private float targetWeight = 1.0f;
    private float smoothTime = SMOOTH_TIME;
    private float _weightVelocity = 0.0f;

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

    public void SetState(RuntimeAnimatorController rtc, AvatarMask mask, float weight, float time, float speed)
    {
        if (rtc.GetHashCode() == this.currentStateHash)
        {
            this.targetWeight = 1.0f - weight;
            this.smoothTime = time;

            Playable input = this.mixer.GetInput(0);
            if (input.IsValid()) input.SetSpeed(speed);
        }
        else
        {
            this.currentStateHash = rtc.GetHashCode();
            this.SetState(
                (rtc == null ? Playable.Null : AnimatorControllerPlayable.Create(this.graph, rtc)),
                mask,
                weight,
                time,
                speed
            );
        }
    }

    public void SetState(AnimationClip clip, AvatarMask mask, float weight, float time, float speed)
    {
        if (clip.GetHashCode() == this.currentStateHash)
        {
            this.targetWeight = 1.0f - weight;
            this.smoothTime = time;

            Playable input = this.mixer.GetInput(0);
            if (input.IsValid()) input.SetSpeed(speed);
        }
        else
        {
            this.currentStateHash = clip.GetHashCode();
            this.SetState(
                (clip == null ? Playable.Null : AnimationClipPlayable.Create(this.graph, clip)),
                mask,
                weight,
                time,
                speed
            );
        }
    }

    public void ResetState(float time)
    {
        this.targetWeight = 1.0f;
        this.smoothTime = time;
    }

    public void SetWeight(float weight)
    {
        this.targetWeight = 1.0f - weight;
        this._weightVelocity = 0.0f;
    }

    public void SetSpeed(float speedCoefficient)
    {
        this.mixer.SetSpeed(speedCoefficient);
    }

    public void Update()
    {
        this.currentWeight = Mathf.Clamp01(Mathf.SmoothDamp(
            this.currentWeight,
            this.targetWeight,
            ref this._weightVelocity,
            this.smoothTime
        ));

        if (this.mixer.CanSetWeights())
        {
            this.mixer.SetInputWeight(0, 1.0f);
            this.mixer.SetInputWeight(1, 1f - this.currentWeight);
        }
    }

    // PRIVATE METHODS: ---------------------------------------------------------------------------

    private void SetState(Playable input, AvatarMask mask, float weight, float time, float speed)
    {
        input.SetSpeed(speed);

        this.graph.Disconnect(this.mixer, 1);
        this.graph.Connect(input, 0, this.mixer, 1);

        float weight1 = this.mixer.GetInputWeight(0);
        this.mixer.SetInputWeight(1, 1.0f - weight1);
        this.mixer.SetLayerMaskFromAvatarMask(1, mask);

        this.targetWeight = 1.0f - weight;
        this.smoothTime = time;
    }
}