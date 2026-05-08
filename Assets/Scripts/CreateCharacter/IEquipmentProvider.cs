using CreateCharacter;

namespace CreateCharacter
{
    /// <summary>
    /// Abstract Factory interface để cung cấp equipment từ các collection khác nhau
    /// </summary>
    public interface IEquipmentProvider
    {
        /// <summary>
        /// Lấy equipment theo ID
        /// </summary>
        Equipment GetEquipmentById(ushort equipmentId);
    }
}
