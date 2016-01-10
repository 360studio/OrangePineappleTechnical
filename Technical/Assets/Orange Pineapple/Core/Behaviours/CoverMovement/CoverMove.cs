using UnityEngine;
using System.Collections;
using Lockstep;
[CustomActiveAbility]
public class CoverMove : Move {
    FPSTurn cachedTurn {get; set;}
    Cover CurrentCover {get; set;}
    private long Input;
    [Lockstep (true)]
    public bool IsTransitioning {get; private set;}
    [Lockstep (true)]
    public long CurrentDegree {get; private set;}
    protected override void OnSetup()
    {
        base.OnSetup();
        cachedTurn = Agent.GetAbility<FPSTurn>();
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
    }

    protected override void OnExecute(Command com)
    {
        if (com.ContainsData<DefaultData>()) {
            DefaultData data = com.GetData<DefaultData> ();
            if (data.DataType == DataType.UShort) {
                Cover cover = CoverManager.GetCover((ushort)data.Value);
                this.StartTransition(cover);
            }
            else if (data.DataType == DataType.Long) {
                Input = (long)data.Value;
            }
        }
    }

    protected override void OnSimulate()
    {
        if (IsTransitioning == false) {
            if (Input != 0) {
                
                CurrentDegree += CurrentCover.GetDegreeSpeed(base.Speed.Mul(Input / 2));
                Agent.Body.Position = CurrentCover.GetDegreePoint(CurrentDegree);
            }
        }
        base.OnSimulate();
    }
    public void StartTransition (Cover next) {
        CurrentCover = next;
        CurrentDegree = next.GetClosestDegree(this.Agent.Body.Position);
        IsTransitioning = true;
        base.StartMove(CurrentCover.GetDegreePoint(CurrentDegree));
    }
    protected override void OnArrive()
    {
        IsTransitioning = false;
    }

    public Command GenerateTransitionCommand () {
        Cover cover = CoverManager.FindCover(cachedTurn.GetBodiesInLine(FixedMath.One * 200),CurrentCover);
        if (cover != null) {
            Command com = new Command(this.Interfacer.ListenInputID,this.Agent.Controller.ControllerID);
            com.Add<DefaultData> (new DefaultData(DataType.UShort,cover.ID));
            return com;
        }
        return null;
    }
    public Command GenerateMovementCommand (float input) {
        if (this.IsTransitioning) return null;
        Command com = new Command(this.Interfacer.ListenInputID,this.Agent.Controller.ControllerID);
        com.Add<DefaultData> (new DefaultData(DataType.Long,FixedMath.Create(input)));
        return com;
    }
}
