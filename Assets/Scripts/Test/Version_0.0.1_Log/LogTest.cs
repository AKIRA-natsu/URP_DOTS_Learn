using AKIRA;
using UnityEngine;

public class LogTest : MonoBehaviour {
    [ContextMenu("测试日志")]
    public void Test() {
        "this is logTest（Editor）".Log(GameData.Log.Editor);
        "this is logTest（GameState）".Log(GameData.Log.GameState);
        "this is logTest（AI）".Log(GameData.Log.AI);
        "this is logTest（Success）".Log(GameData.Log.Success);
        "this is logTest（Source）".Log(GameData.Log.Source);
        "this is logTest（Guide）".Log(GameData.Log.Guide);
        "this is logTest".Log();
    }
}