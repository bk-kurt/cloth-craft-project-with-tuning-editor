using Lean.Touch;

public interface ISelectable
{
    void Select(LeanFinger leanFinger);
    void Deselect();
}
public interface IExecutableObjectProvider
{
    ExecutableObject GetExecutableObject();
}

public interface ISpawnable
{
    void Spawn(ExecutableObject executableObject);
}
