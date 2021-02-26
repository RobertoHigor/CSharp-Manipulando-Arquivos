using System;

public enum WaterLevel
{
    Low,
    High
}

public class CanalLock
{
    // Query canal lock state
    public WaterLevel CanalLockWaterLevel{ get; private set;} = WaterLevel.Low;
    public bool HighWaterGateOpen { get; private set; } = false;
    public bool LowWaterGateOpen { get; private set; } = false;

    // Change the upper gate
    public void SetHighGate(bool open)
    {
        /*if (open && (CanalLockWaterLevel == WaterLevel.High))
            HighWaterGateOpen = true;
        else if (open && (CanalLockWaterLevel == WaterLevel.Low))
            throw new InvalidOperationException("Cannot open high gate when water is low");*/
        
        // Com switch expression para diversos estados
        HighWaterGateOpen = (open, HighWaterGateOpen, CanalLockWaterLevel) switch
        {
            (false, _, _) => false,           
            (true, _, WaterLevel.High) => true,
            (true, false, WaterLevel.Low) => throw new InvalidOperationException("Cannot open high gate when the water is low"),
            _ => throw new InvalidOperationException("Invalid internal state"),
        };

        // A última linha serve como um catch all para remover o aviso de que todos os possíveis inputs não foram colocados no switch
        // Nesse caso, o erro ocorre por se tratar de um enum e o switch reconhece como int
    }

    // change the lower gaet
    public void SetLowGate(bool open)
    {
        LowWaterGateOpen = open;
    }

    // Change water level
    public void SetWaterLevel(WaterLevel newLevel)
    {
        CanalLockWaterLevel = newLevel;
    }

    public override string ToString() => 
    $"The lower Gate is {(LowWaterGateOpen ? "Open": "Closed")}. " +
    $"The upper gate is {(HighWaterGateOpen ? "Open" : "Closed")}. " +
    $"The Water level is {CanalLockWaterLevel}.";
}