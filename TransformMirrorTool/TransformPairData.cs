using UnityEngine;

[System.Serializable]
public class TransformPairData
{
    public string baseObjectName;
    public string targetObjectName;

    public bool mirrorX = true;
    public bool mirrorY = true;
    public bool mirrorZ = true;

    public bool bypassXPosition = false;
    public bool bypassYPosition = false;
    public bool bypassZPosition = false;
    public bool bypassXRotation = false;
    public bool bypassYRotation = false;
    public bool bypassZRotation = false;

    // Initial transforms
    public Vector3 initialBasePosition;
    public Quaternion initialBaseRotation;
    public Vector3 initialBaseScale;

    public Vector3 initialTargetPosition;
    public Quaternion initialTargetRotation;
    public Vector3 initialTargetScale;
}