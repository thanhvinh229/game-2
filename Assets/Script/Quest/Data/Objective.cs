using System;

public abstract class Objective
{
    protected ObjectiveData _data;
    public abstract bool IsCompleted { get; }
    public event Action OnProgressChanged;
     
    public string Id => _data.Id;
    
    protected Objective(ObjectiveData data)
    {
        _data = data;
    }
 
    protected void NotifyProgressChanged()
    {
        OnProgressChanged?.Invoke();
    }
 
    public abstract void Register();
    public abstract void Unregister();
}
 
