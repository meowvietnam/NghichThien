using Network;
using Network.Extensions;
using UnityEngine;

namespace AllMaps.NgoDao
{
    public class NgoDao : BaseData, IMemorySerializable<NgoDao>
    {
        [SerializeField] public int require;
        [SerializeField] public BaseSkill.ETypeSkill[] eTypeRequire;
        
        public virtual void Learn(INetworkEvent.ILearnNgoDao data)
        {
        }    
        public virtual string GetDes() => string.Empty;
        public override int ByteSize()
        {
            var byteSize = base.ByteSize();
            return byteSize + require.ByteSize() + eTypeRequire.ByteSize(); 
        }

        public void Deserialize(IReader reader, ref NgoDao value)
        {
            BaseData baseValue = value;
            baseValue.Deserialize(reader, ref baseValue);
            value.require = reader.Read<int>();
            value.eTypeRequire = reader.ReadArray<BaseSkill.ETypeSkill>();
        }
        public override void Serialize(IWriter writer)
        {
            base.Serialize(writer);
            writer.Write(require);
            writer.WriteArray(eTypeRequire);
        }
    }
}
