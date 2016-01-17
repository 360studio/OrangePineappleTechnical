using UnityEngine;
using System.Collections;
using Lockstep;
using Lockstep.Data;
public class FPSHelper : BehaviourHelper {
    public static FPSHelper Instance {get; private set;}
    public ushort RegisterCode {get; private set;}
    [SerializeField]
    private Vector2d _spawnPos = new Vector2d(175,50);

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

    int comCount = 10;
    int comHash {get {return comCount++;}}
    protected override void OnGameStart()
    {
        Command registerCom = new Command(RegisterCode);
        registerCom.Add<DefaultData> (new DefaultData(DataType.Int, ClientManager.ClientID));
        for (int i = 0; i < 1; i++)
        {
            CommandManager.SendCommand(registerCom);
        }
    }


    protected override void OnRawExecute(Command com)
    {
        if (com.InputCode == RegisterCode) {
            AgentController cont = AgentController.Create();
            var agent = cont.CreateAgent(this._FPSAgentCode,_spawnPos);
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
