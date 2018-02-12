namespace CGM.Communication.MiniMed.Model
{
    public enum SgvTrend
    {
        None,
        DoubleUp,
        SingleUp,
        FortyFiveUp,
        Flat,
        FortyFiveDown,
        SingleDown,
        DoubleDown,
        NotComputable,
        RateOutOfRange,
        NotSet
    }



}

//public enum CgmTrend
//{
//    None =,
//    DoubleUp = 0xc0,
//    SingleUp = 0xa0,
//    FortyFiveUp = 0x80,
//    Flat = 0x60,
//    FortyFiveDown = 0x40,
//    SingleDown = 0x20,
//    DoubleDown = 0x00,
//    NotComputable =,
//    RateOutOfRange =,
//    NotSet =
//    }
//               case (byte)0x60:
//                    return CgmTrend.Flat;
//                case (byte)0xc0:
//                    return CgmTrend.DoubleUp;
//                case (byte)0xa0:
//                    return CgmTrend.SingleUp;
//                case (byte)0x80:
//                    return CgmTrend.FortyFiveUp;
//                case (byte)0x40:
//                    return CgmTrend.FortyFiveDown;
//                case (byte)0x20:
//                    return CgmTrend.SingleDown;
//                case (byte)0x00:
//                    return CgmTrend.DoubleDown;
//                default:
//                    return CgmTrend.NotComputable;