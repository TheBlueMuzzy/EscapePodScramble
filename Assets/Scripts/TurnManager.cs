// TurnManager.cs
using System.Collections.Generic;
using UnityEngine;

// ICommand interface for actions we can execute & undo
public interface IAction
{
    void Execute();
    void Undo();
}

// A simple MoveCommand that moves a CrewMember and records its from/to
public class MoveCommand : IAction
{
    private CrewMember crew;
    private Vector3 from, to;

    public MoveCommand(CrewMember c, Vector3 target)
    {
        crew = c;
        from = c.transform.position;
        to = target;
    }

    public void Execute() => crew.transform.position = to;
    public void Undo() => crew.transform.position = from;
}

// The TurnManager that keeps a stack of actions for undo/redo
public class TurnManager : MonoBehaviour
{
    private Stack<IAction> history = new Stack<IAction>();

    public void DoAction(IAction action)
    {
        action.Execute();
        history.Push(action);
    }

    public void UndoLast()
    {
        if (history.Count > 0)
            history.Pop().Undo();
    }

    public void CancelTurn()
    {
        while (history.Count > 0)
            history.Pop().Undo();
    }
}
