public interface IHealable : IDamageable
{
    void ProvideHealing(int healAmount);
    void ProvideHealing(float healAmount);
}
