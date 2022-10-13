using Network;

namespace Shared
{
    public class Define
    {
        public enum PacketIndex : uint
        {
            CtoS_Enter = 1,
            CtoS_Chat,

            StoC_Enter = 1000,
            StoC_Char,
            StoC_Leave,
        }

        public static string CheckValue = "Test Check Value";
    }

    public class PacketCommon : PacketBase<Define.PacketIndex>
    {
        protected PacketCommon(Define.PacketIndex _Index)
            : base(_Index)
        {
            CheckValue = Define.CheckValue;
        }

        public string CheckValue { get; set; }
    }

    namespace CtoS
    {
        public class Enter : PacketCommon
        {
            public Enter()
                : base(Define.PacketIndex.CtoS_Enter)
            {
                NickName = "";
            }

            public string NickName;
        }

        public class Chat : PacketCommon
        {
            public Chat()
                : base(Define.PacketIndex.CtoS_Chat)
            {
                Message = "";
            }

            public string Message;
        }
    }

    namespace StoC
    {
        public class Enter : PacketCommon
        {
            public Enter()
                : base(Define.PacketIndex.StoC_Enter)
            {
                NickName = "";
            }

            public string NickName;
        }

        public class Chat : PacketCommon
        {
            public Chat()
                : base(Define.PacketIndex.StoC_Char)
            {
                NickName = "";
                Message = "";
            }

            public string NickName;
            public string Message;
        }

        public class Leave : PacketCommon
        {
            public Leave()
                : base(Define.PacketIndex.StoC_Leave)
            {
                NickName = "";
            }

            public string NickName;
        }
    }
}