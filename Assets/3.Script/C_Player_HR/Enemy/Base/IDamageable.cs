
/// <summary>
/// 데미지를 입을 수 있는 모든 객체가 구현해야 하는 인터페이스입니다.
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// 지정된 양의 데미지를 입힙니다.
    /// </summary>
    /// <param name="damage">입힐 데미지 수치</param>
    void TakeDamage(int damage);
}

