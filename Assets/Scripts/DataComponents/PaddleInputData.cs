using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct PaddleInputData : IComponentData
{
    public KeyCode upKey;
    public KeyCode downKey;
}
