using UnityEngine;

/// <summary>
/// 인스펙터에서 할당 가능한 증강 에셋의 베이스 클래스입니다.
/// </summary>
public abstract class AugmentSO : ScriptableObject, IAugment
{
    public string augmentName;
    [TextArea] public string description;
    
    public abstract void ModifyFire(FireContext context);
}
