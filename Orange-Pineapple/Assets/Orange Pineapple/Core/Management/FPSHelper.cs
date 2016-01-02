using UnityEngine;
using System.Collections;
using Lockstep;
using Lockstep.Data;
public class FPSHelper : BehaviourHelper {
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
            int playerID = (int)com.GetData<DefaultData> ().Value;
            AgentController cont = AgentController.Create();
            PlayerManager.AddController(cont);
            if (playerID == ClientManager.ClientID) {
                FPSAgent = cont.CreateAgent (this._FPSAgentCode);
                cont.AddToSelection(FPSAgent);
            }
        }
    }


}
