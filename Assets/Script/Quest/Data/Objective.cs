public abstract class Objective
{
    protected ObjectiveData _data;
    public abstract bool IsCompleted { get; }
    protected Objective(ObjectiveData data)
    {
        _data = data;
    }
    public abstract void Register();
    public abstract void Unregister();
}
