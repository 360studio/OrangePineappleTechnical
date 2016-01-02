using UnityEngine;
using System.Collections;
using Lockstep;
using Lockstep.Data;
public class FPSHelper : BehaviourHelper {
    public static FPSHelper Instance {get; private set;}
    public ushort RegisterCode {get; private set;}


    [SerializeField, DataCode ("Agents")]
    private string _FPSAgentCode;

    bool Setted;

    public LSAgent FPSAgent {get; private set;}

    protected override void OnInitialize()
    {
        if (!Setted) {
            Setted = true;
            this.RegisterCode = InputCodeManager.GetCodeID ("Register");
        }
        Instance = this;
    }

    protected override void OnGameStart()
    {
        Command registerCom = new Command(RegisterCode);
        registerCom.Add<DefaultData> (new DefaultData(DataType.Int, ClientManager.ClientID));
        CommandManager.SendCommand(registerCom);
    }

    protected override void OnRawExecute(Command com)
    {
        if (com.LeInput == RegisterCode) {
            AgentController cont = AgentController.Create();
            LSAgent agent = cont.CreateAgent (this._FPSAgentCode);
            int playerID = (int)com.GetData<DefaultData> ().Value;
            if (playerID == ClientManager.ClientID) {
                PlayerManager.AddController(cont);
                FPSAgent = agent;
                FPSAgent.GetAbility<FPSTurn> ().IsControlling = true;
                cont.AddToSelection(FPSAgent);
            }
        }
    }


}
