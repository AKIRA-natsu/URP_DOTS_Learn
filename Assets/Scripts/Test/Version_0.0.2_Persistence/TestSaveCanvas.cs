using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSaveCanvas : MonoBehaviour
{
    public InputField xmlIntInput;
    public InputField xmlStrInput;
    public InputField xmlFloatInput;
    public InputField jsonIntInput;
    public InputField jsonStrInput;
    public InputField jsonFloatInput;

    public Text text;

    public SaveTest save;

    private void Start() {
        #if UNITY_EDITOR
        text.text = $"Path: {Application.streamingAssetsPath}";
        #else
        text.text = $"Path: {Application.persistentDataPath}";
        #endif
    }

    public void Save() {
        save.xmlData.intValue = xmlIntInput.text.TryParseInt();
        save.xmlData.strValue = xmlStrInput.text;
        save.xmlData.floatValue = xmlFloatInput.text.TryParseFloat();
        save.jsonData.intValue = jsonIntInput.text.TryParseInt();
        save.jsonData.strValue = jsonStrInput.text;
        save.jsonData.floatValue = jsonFloatInput.text.TryParseFloat();
        save.TestSave();
    }

    public void Read() {
        xmlIntInput.text = save.xmlData.intValue.ToString();
        xmlStrInput.text = save.xmlData.strValue.ToString();
        xmlFloatInput.text = save.xmlData.floatValue.ToString();
        jsonIntInput.text = save.jsonData.intValue.ToString();
        jsonStrInput.text = save.jsonData.strValue.ToString();
        jsonFloatInput.text = save.jsonData.floatValue.ToString();
        save.TestRead();
    }
}
