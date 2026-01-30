namespace GameFrameWork
{
    // Interface for animatable objects
    public interface IAnimatable
    {
        AnimationComponent Animator { get; }
        AnimationState CurrentAnimationState { get; set; }
    }
}
