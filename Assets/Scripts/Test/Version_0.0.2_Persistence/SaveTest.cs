using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTest : MonoBehaviour, IJson {
    public XML xml;

    public JsonSaveData xmlData = new JsonSaveData();
    public JsonSaveData jsonData = new JsonSaveData();

    public string Path => "Test.json";

    public object Data { get => jsonData; set => jsonData = (JsonSaveData)value; }

    private void Start() {
        xml = new XML("Test.xml", false);
        xmlData = new JsonSaveData();
        jsonData = new JsonSaveData();
    }

    [ContextMenu("Test Save")]
    public void TestSave() {
        if (!xml.Exist()) {
            xml.Create(x => {
                var data = x.CreateElement("Data");
                data.SetAttribute("IntValue", xmlData.intValue.ToString());
                data.SetAttribute("StrValue", xmlData.strValue.ToString());
                data.SetAttribute("FloatValue", xmlData.floatValue.ToString());
                x.AppendChild(data);
            });
        } else {
            xml.Update(x => {
                var data = x.SelectSingleNode("Data") as XmlElement;
                data.SetAttribute("IntValue", xmlData.intValue.ToString());
                data.SetAttribute("StrValue", xmlData.strValue.ToString());
                data.SetAttribute("FloatValue", xmlData.floatValue.ToString());
            });
        }

        this.Save();

        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        #endif
    }

    [ContextMenu("Test Read")]
    public void TestRead() {
        if (xml.Exist()) {
            xml.Read(x => {
                var data = x.SelectSingleNode("Data") as XmlElement;
                xmlData.intValue = data.GetAttribute("IntValue").TryParseInt();
                xmlData.strValue = data.GetAttribute("StrValue");
                xmlData.floatValue = data.GetAttribute("FloatValue").TryParseFloat();
            });
        }
        
        jsonData = this.Read<JsonSaveData>();
    }

    [ContextMenu("Test Delete")]
    public void TestDelete() {
        xml.Delete();
        this.Delete();

        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        #endif
    }

    [System.Serializable]
    public class JsonSaveData {
        public int intValue;
        public string strValue;
        public float floatValue;

        public override string ToString()
        {
            return $"{intValue} {strValue} {floatValue}";
        }
    }
}
