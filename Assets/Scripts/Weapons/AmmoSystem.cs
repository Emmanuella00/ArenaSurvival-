using System;

// ENCAPSULATION + RESOURCE MANAGEMENT. Plain C# class (not a MonoBehaviour).
// OBSERVER: raises OnAmmoChanged so the HUD can update itself.
public class AmmoSystem
{
    private int magazine;
    private int reserve;
    private readonly int magazineSize;

    public event Action<int, int> OnAmmoChanged;   // (magazine, reserve)

    public int Magazine { get { return magazine; } }
    public int Reserve { get { return reserve; } }

    public AmmoSystem(int magazineSize, int startingReserve)
    {
        this.magazineSize = magazineSize;
        magazine = magazineSize;
        reserve = startingReserve;
    }

    public bool TryUseAmmo(int amount)
    {
        if (magazine < amount) return false;     // not enough loaded
        magazine -= amount;
        Notify();
        return true;
    }

    public void Reload()
    {
        int needed = magazineSize - magazine;             // empty slots
        int taken = Math.Min(needed, reserve);            // can't take more than the reserve has
        magazine += taken;
        reserve -= taken;
        Notify();
    }

    public void AddAmmo(int amount)
    {
        if (amount <= 0) return;
        reserve += amount;
        Notify();
    }

    private void Notify()
    {
        OnAmmoChanged?.Invoke(magazine, reserve);
    }
}
