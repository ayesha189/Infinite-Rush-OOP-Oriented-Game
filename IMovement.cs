namespace GameFrameWork
{
    // Interface for objects that can move
    public interface IMovement
    {
        void Move(GameObject obj, GameTime gameTime);
    }
}
