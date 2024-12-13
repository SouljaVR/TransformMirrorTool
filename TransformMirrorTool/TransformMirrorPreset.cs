using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransformMirrorPreset
{
    public string presetName;
    public List<TransformPairData> pairs = new List<TransformPairData>();
}
