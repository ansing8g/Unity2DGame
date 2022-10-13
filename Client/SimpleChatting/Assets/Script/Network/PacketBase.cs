namespace Network
{
    public class PacketBase<PacketIndex>
    {
        public PacketBase()
        {
            Index = default(PacketIndex)!;
        }

        public PacketBase(PacketIndex index)
        {
            Index = index;
        }

        public PacketIndex Index { get; set; }
    }
}