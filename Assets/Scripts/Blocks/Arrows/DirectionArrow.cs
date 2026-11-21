using System;
using NaughtyAttributes;
using Unity.Mathematics;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Стрелка направления на жёлтом блоке.
/// Вращается по взаимодействию (E) независимо от цвета блока и того, движется ли он.
/// Движением занимается YellowMovingBlock (подписан на событие Interacted).
/// </summary>
public class DirectionArrow : BlockArrow
{
    [Header("Взаимодействие")]
    [Tooltip("Выключено: стрелка реагирует на любую часть блока. " +
             "Включено: только если игрок целится в саму стрелку (нужен коллайдер на стрелке).")]
    [FormerlySerializedAs("requireAimNearArrow")]
    [SerializeField] private bool requireAimAtArrow = false;
    [Header("Выбор направления")]
    [Dropdown("GetVectorValues"),SerializeField,OnValueChanged("SetRotation")]
    private Vector3 dir = Vector3.up;
    private DropdownList<Vector3> GetVectorValues()
    {
        return new DropdownList<Vector3>()
        {
            {"Up", Vector3.up},
            {"Down", Vector3.down},
            {"Right", Vector3.right},
            {"Left", Vector3.left},
            {"Forward", Vector3.forward},
            {"Back", Vector3.back}
        };
    }

    public override bool AcceptsAnyPartOfBlock => !requireAimAtArrow;

    protected override void OnInteract(Vector3 hitPoint) 
    {   
        var hitSide = hitPoint-transform.position;
        dir = Vector3.zero;
        if(math.abs(hitSide.x)>math.abs(hitSide.y) && math.abs(hitSide.x)>math.abs(hitSide.z))
            dir.x = MathF.Sign(hitSide.x);
        else if(math.abs(hitSide.y)>math.abs(hitSide.x) && math.abs(hitSide.y)>math.abs(hitSide.z))
            dir.y = MathF.Sign(hitSide.y);
        else
            dir.z = MathF.Sign(hitSide.z);
        print(dir);
        SetRotation();
    }

    private void SetRotation() => transform.localEulerAngles = new Vector3(dir.z*90f-dir.y*90f+Mathf.Abs(dir.y*90f),0,-dir.x*90f);

    public Vector3 ForwardDirection => dir;
}