namespace SR2E.Saving;

public abstract class RootSave : SR2ESaveableBase {
    public byte[] ToBytes() => SaveDataSerializer.Serialize(this);
    
    public static T FromBytes<T>(byte[] data) where T : RootSave {
        return SaveDataSerializer.Deserialize<T>(data);
    }
    public static RootSave FromBytes(byte[] data) {
        return SaveDataSerializer.Deserialize<RootSave>(data);
    }
}