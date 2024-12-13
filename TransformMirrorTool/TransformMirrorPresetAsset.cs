using UnityEngine;

[CreateAssetMenu(fileName = "NewTransformMirrorPreset", menuName = "Transform Mirror Tool/Transform Mirror Preset")]
public class TransformMirrorPresetAsset : ScriptableObject
{
    public TransformMirrorPreset preset = new TransformMirrorPreset();
}