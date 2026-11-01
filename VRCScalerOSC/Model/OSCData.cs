using System.Buffers.Binary;
using System.Text;

namespace VRCScalerOSC.Model
{
    public class OSCData
    {
        public byte[] Message = [];

        private static readonly byte[] _trackingVRsystemHead = "/tracking/vrsystem/head/pose"u8.ToArray();
        private static readonly byte[] _trackingVRSystemLeftWrist = "/tracking/vrsystem/leftwrist/pose"u8.ToArray();
        private static readonly byte[] _trackingVRSystemRightWrist = "/tracking/vrsystem/rightwrist/pose"u8.ToArray();
        public static readonly string TrackingVRsysHandDistance = "/tracking/vrsystem/HandDistance";

        private static readonly byte[] _tF = [ // ,F
            0x2C, 0x46, 0x00, 0x00];
        private static readonly byte[] _tT = [ // ,T
            0x2C, 0x54, 0x00, 0x00];
        private static readonly byte[] _ts = [ // ,s
            0x2C, 0x73, 0x00, 0x00];
        private static readonly byte[] _ti = [ // ,i
            0x2C, 0x69, 0x00, 0x00];
        private static readonly byte[] _tf = [ // ,f
            0x2C, 0x66, 0x00, 0x00];
        private static readonly byte[] _tff = [ // ,ff
            0x2C, 0x66, 0x66, 0x00];
        private static readonly byte[] _tfff = [ // ,fff
            0x2C, 0x66, 0x66, 0x66, 0x00, 0x00, 0x00, 0x00];
        private static readonly byte[] _tffff = [ // ,ffff
            0x2C, 0x66, 0x66, 0x66, 0x66, 0x00, 0x00, 0x00];
        private static readonly byte[] _tffffff = [ // ,ffffff
            0x2C, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x00];
        public OSCData(byte[] message)
        {
            Message = message;
        }
        public OSCData(ReadOnlySpan<char> addr, ReadOnlySpan<char> typeString, object data)
        {
            Message = new byte[(Encoding.UTF8.GetBytes(addr.ToString()).Length + 1024 & ~3)];
            Span<byte> span = Message;
            int offset = 0;
            //addr
            int addrBytes = Encoding.UTF8.GetBytes(addr, span[offset..]);
            offset += addrBytes;
            int addrTotal = (addrBytes + 4) & ~3;
            span.Slice(offset, addrTotal - addrBytes).Clear();
            offset = addrTotal;

            //typeString
            ReadOnlySpan<char> typeTag = typeString.StartsWith(",", StringComparison.Ordinal) ? typeString : "," + typeString.ToString();
            int typeBytes = Encoding.UTF8.GetBytes(typeTag, span[offset..]);
            offset += typeBytes;
            int typeTotal = (typeBytes + 4) & ~3;
            span.Slice(offset, typeTotal - typeBytes).Clear();
            offset = addrTotal + typeTotal;

            //data
            int sBytes;
            switch (TypeString)
            {
                case "s":
                    if (data is string valueS)
                    {
                        sBytes = Encoding.UTF8.GetBytes(valueS, span[offset..]);
                        int sTotal = (sBytes + 4) & ~3;
                        span.Slice(offset + sBytes, sTotal - sBytes).Clear();
                        offset += sTotal;
                    }
                    break;
                case "i":
                    if (data is int valueI)
                    {
                        BinaryPrimitives.WriteInt32BigEndian(span[offset..], valueI);
                        offset += 4;
                    }
                    break;
                case "f":
                    if (data is float valueF)
                    {
                        FloatToBytes(ref span, ref offset, [valueF]);
                    }
                    break;
                case "ff":
                    if (data is Vector2 valueVector2)
                    {
                        FloatToBytes(ref span, ref offset, valueVector2.Data);
                    }
                    break;
                case "fff":
                    if (data is Vector3 valueVector3)
                    {
                        FloatToBytes(ref span, ref offset, valueVector3.Data);
                    }
                    break;
                case "ffff":
                    if (data is Vector4 valueVector4)
                    {
                        FloatToBytes(ref span, ref offset, valueVector4.Data);
                    }
                    break;
                case "ffffff":
                    if (data is PoseData valuePoseData)
                    {
                        FloatToBytes(ref span, ref offset, valuePoseData.Data);
                    }
                    break;
                default:
                    return;
            }
            Message = Message.AsSpan(0, offset).ToArray();
        }
        public static void FloatToBytes(ref Span<byte> span, ref int offset, float[] datas)
        {
            foreach (float data in datas)
            {
                int sBytes = BitConverter.SingleToInt32Bits(data);
                BinaryPrimitives.WriteInt32BigEndian(span[offset..], sBytes);
                offset += 4;
            }
        }
        public ReadOnlySpan<char> Addr
        {
            get
            {
                if (Message.Length >= 228 && Message.AsSpan().IndexOf(_trackingVRsystemHead) >= 0 && Message.AsSpan().IndexOf(_trackingVRSystemLeftWrist) >= 0 && Message.AsSpan().IndexOf(_trackingVRSystemRightWrist) >= 0)
                {
                    return TrackingVRsysHandDistance.AsSpan();
                }
                else if (Message.Length > 0 && Array.IndexOf(Message, (byte)0) >= 0 && Array.IndexOf(Message, (byte)44) < Message.Length)
                {
                    return Encoding.UTF8.GetString(Message.AsSpan()[..Array.IndexOf(Message, (byte)44)].ToArray()).TrimEnd('\0');
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public ReadOnlySpan<char> TypeString
        {
            get
            {
                if (IsVRSystem)
                {
                    return "D";
                }
                else if (Message.AsSpan().IndexOf((byte)',') >= 0)
                {
                    Span<byte> type = Message.AsSpan()[Message.AsSpan().IndexOf((byte)',')..];
                    type = type[..(type.IndexOf((byte)0) + 3 & ~3)];
                    return Encoding.UTF8.GetString(type).TrimStart(',').TrimEnd('\0');
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public float? ValueF
        {
            get
            {
                if (IsVRSystem)
                {
                    ReadOnlySpan<byte> leftP = Message.AsSpan().Slice(8 + Message.AsSpan().IndexOf(_trackingVRSystemLeftWrist) + Message.AsSpan()[Message.AsSpan().IndexOf(_trackingVRSystemLeftWrist)..].IndexOf(_tffffff), 24);
                    ReadOnlySpan<byte> rightP = Message.AsSpan().Slice(8 + Message.AsSpan().IndexOf(_trackingVRSystemRightWrist) + Message.AsSpan()[Message.AsSpan().IndexOf(_trackingVRSystemRightWrist)..].IndexOf(_tffffff), 24);
                    float leftX = BinaryPrimitives.ReadSingleBigEndian(leftP[..4]);
                    float leftY = BinaryPrimitives.ReadSingleBigEndian(leftP.Slice(4, 4));
                    float leftZ = BinaryPrimitives.ReadSingleBigEndian(leftP.Slice(8, 4));
                    float rightX = BinaryPrimitives.ReadSingleBigEndian(rightP[..4]);
                    float rightY = BinaryPrimitives.ReadSingleBigEndian(rightP.Slice(4, 4));
                    float rightZ = BinaryPrimitives.ReadSingleBigEndian(rightP.Slice(8, 4));
                    float dX = leftX - rightX;
                    float dY = leftY - rightY;
                    float dZ = leftZ - rightZ;
                    float sumOfSquares = (dX * dX) + (dY * dY) + (dZ * dZ);
                    return MathF.Sqrt(sumOfSquares);
                }
                else if (Message.AsSpan().IndexOf(_tf) >= 0)
                {
                    return GetValueF;
                }
                else if (Message.AsSpan().IndexOf(_ti) >= 0)
                {
                    return GetValueI;
                }
                else if (Message.AsSpan().IndexOf(_tT) >= 0)
                {
                    return 1f;
                }
                else if (Message.AsSpan().IndexOf(_tF) >= 0)
                {
                    return 0f;
                }
                return null;
            }
        }
        public int? ValueI
        {
            get
            {
                if (Message.AsSpan().IndexOf(_ti) >= 0)
                {
                    return GetValueI;
                }
                else if (Message.AsSpan().IndexOf(_tT) >= 0)
                {
                    return 1;
                }
                else if (Message.AsSpan().IndexOf(_tF) >= 0)
                {
                    return 0;
                }
                if (Message.AsSpan().IndexOf(_tf) >= 0)
                {
                    return (int?)GetValueF;
                }
                return null;
            }
        }
        public bool? ValueB
        {
            get
            {
                if (Message.AsSpan().IndexOf(_tf) >= 0)
                {
                    if (GetValueF.HasValue)
                    {
                        return GetValueF.Value > 0.000001f;
                    }
                }
                else if (Message.AsSpan().IndexOf(_ti) >= 0)
                {
                    if (GetValueI.HasValue)
                    {
                        return GetValueI.Value > 0;
                    }
                }
                else if (Message.AsSpan().IndexOf(_tT) >= 0)
                {
                    return true;
                }
                else if (Message.AsSpan().IndexOf(_tF) >= 0)
                {
                    return false;
                }
                return null;
            }
        }
        public Vector2? ValueV2
        {
            get
            {
                if (Message.AsSpan().IndexOf(_tff) >= 0)
                {
                    return GetValueV2;
                }
                return null;
            }
        }
        public Vector3? ValueV3
        {
            get
            {
                if (Message.AsSpan().IndexOf(_tfff) >= 0)
                {
                    return GetValueV3;
                }
                return null;
            }
        }
        public Vector4? ValueV4
        {
            get
            {
                if (Message.AsSpan().IndexOf(_tffff) >= 0)
                {
                    return GetValueV4;
                }
                return null;
            }
        }
        public PoseData? ValuePose
        {
            get
            {
                if (Message.AsSpan().IndexOf(_tffffff) >= 0)
                {
                    return GetValuePose;
                }
                return null;
            }
        }
        private float? GetValueF
        {
            get
            {
                Span<byte> value = Message.AsSpan()[(Message.AsSpan().IndexOf(_tf) + _tf.Length)..];
                if (value.Length < 4)
                {
                    return null;
                }
                return BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32BigEndian(value[..4]));
            }
        }
        private int? GetValueI
        {
            get
            {
                Span<byte> value = Message.AsSpan()[(Message.AsSpan().IndexOf(_ti) + _ti.Length)..];
                if (value.Length < 4)
                {
                    return null;
                }
                return BinaryPrimitives.ReadInt32BigEndian(value[..4]);
            }
        }
        private Vector2? GetValueV2
        {
            get
            {
                Span<byte> value = Message.AsSpan()[(Message.AsSpan().IndexOf(_tff) + _tff.Length)..];
                if (value.Length < 8)
                {
                    return null;
                }
                return new Vector2(BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32BigEndian(value[..4])),
                                   BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32BigEndian(value.Slice(4, 4))));
            }
        }
        private Vector3? GetValueV3
        {
            get
            {
                Span<byte> value = Message.AsSpan()[(Message.AsSpan().IndexOf(_tfff) + _tfff.Length)..];
                if (value.Length < 12)
                {
                    return null;
                }
                return new Vector3(BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32BigEndian(value[..4])),
                                   BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32BigEndian(value.Slice(4, 4))),
                                   BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32BigEndian(value.Slice(8, 4))));
            }
        }
        private Vector4? GetValueV4
        {
            get
            {
                Span<byte> value = Message.AsSpan()[(Message.AsSpan().IndexOf(_tffff) + _tffff.Length)..];
                if (value.Length < 16)
                {
                    return null;
                }
                return new Vector4(BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32BigEndian(value[..4])),
                                   BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32BigEndian(value.Slice(4, 4))),
                                   BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32BigEndian(value.Slice(8, 4))),
                                   BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32BigEndian(value.Slice(12, 4))));
            }
        }
        private PoseData? GetValuePose
        {
            get
            {
                Span<byte> value = Message.AsSpan()[(Message.AsSpan().IndexOf(_tffffff) + _tffffff.Length)..];
                if (value.Length < 24)
                {
                    return null;
                }
                return new PoseData(BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32BigEndian(value[..4])),
                                    BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32BigEndian(value.Slice(4, 4))),
                                    BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32BigEndian(value.Slice(8, 4))),
                                    BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32BigEndian(value.Slice(12, 4))),
                                    BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32BigEndian(value.Slice(16, 4))),
                                    BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32BigEndian(value.Slice(20, 4))));
            }
        }
        public object? Value
        {
            get
            {
                if (IsVRSystem)
                {
                    return GetValueF;
                }
                else if (Message.AsSpan().IndexOf(_tffffff) >= 0)
                {
                    return GetValuePose;
                }
                else if (Message.AsSpan().IndexOf(_tffff) >= 0)
                {
                    return GetValueV4;
                }
                else if (Message.AsSpan().IndexOf(_tfff) >= 0)
                {
                    return GetValueV3;
                }
                else if (Message.AsSpan().IndexOf(_tff) >= 0)
                {
                    return GetValueV2;
                }
                else if (Message.AsSpan().IndexOf(_tf) >= 0)
                {
                    return GetValueF;
                }
                else if (Message.AsSpan().IndexOf(_ti) >= 0)
                {
                    return GetValueI;
                }
                else if (Message.AsSpan().IndexOf(_ts) >= 0)
                {
                    Span<byte> value = Message.AsSpan()[(Message.AsSpan().IndexOf(_ts) + _ts.Length)..];
                    return Encoding.UTF8.GetString(value).TrimEnd('\0'); ;
                }
                if (Message.AsSpan().IndexOf(_tT) >= 0)
                {
                    return true;
                }
                else if (Message.AsSpan().IndexOf(_tF) >= 0)
                {
                    return false;
                }
                else
                {
                    return null;
                }
            }
        }
        public string ValueString
        {
            get
            {
                return Value?.ToString() ?? string.Empty;
            }
        }
        public override string ToString()
        {
            return $"{Addr} ,{TypeString} {ValueString}";
        }
        public bool IsBool
        {
            get { return Message.AsSpan().IndexOf(_tT) >= 0 || Message.AsSpan().IndexOf(_tF) >= 0; }
        }
        public bool IsFloat
        {
            get { return Message.AsSpan().IndexOf(_tf) >= 0; }
        }
        public bool IsVector2
        {
            get { return Message.AsSpan().IndexOf(_tff) >= 0; }
        }
        public bool IsVector3
        {
            get { return Message.AsSpan().IndexOf(_tfff) >= 0; }
        }
        public bool IsVector4
        {
            get { return Message.AsSpan().IndexOf(_tffff) >= 0; }
        }
        public bool IsVectorPoseData
        {
            get { return Message.AsSpan().IndexOf(_tffffff) >= 0; }
        }
        public bool IsVRSystem
        {
            get { return Message.Length >= 228 && Message.AsSpan().IndexOf(_trackingVRsystemHead) >= 0 && Message.AsSpan().IndexOf(_trackingVRSystemLeftWrist) >= 0 && Message.AsSpan().IndexOf(_trackingVRSystemRightWrist) >= 0; }
        }
        public bool VOID
        {
            get { return Message.Length == 0 || Message.AsSpan().IndexOf((byte)',') < 0 || Value == null; }
        }
        public static OSCData GetTrueOSCData(ReadOnlySpan<char> Addr, ReadOnlySpan<char> TypeString)
        {
            return TypeString switch
            {
                "T" or "F" => new OSCData(Addr, "T", true),
                "i" => new OSCData(Addr, "i", 1),
                "f" => new OSCData(Addr, "f", 1f),
                _ => new OSCData([]),
            };
        }
        public static OSCData GetFalseOSCData(ReadOnlySpan<char> Addr, ReadOnlySpan<char> TypeString)
        {
            return TypeString switch
            {
                "T" or "F" => new OSCData(Addr, "F", false),
                "i" => new OSCData(Addr, "i", 0),
                "f" => new OSCData(Addr, "f", 0f),
                "s" => new OSCData(Addr, "s", string.Empty),
                _ => new OSCData([]),
            };
        }
        public static byte[] GetEyeHeightByteArray(double eyeHeight)
        {
            byte[] data = [
            0x2F, 0x61, 0x76, 0x61, 0x74, 0x61, 0x72, 0x2F,
            0x65, 0x79, 0x65, 0x68, 0x65, 0x69, 0x67, 0x68,
            0x74, 0x00, 0x00, 0x00, 0x2C, 0x66, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00];
            BinaryPrimitives.WriteDoubleBigEndian(data.AsSpan()[24..], eyeHeight);
            return data;
        }
    }
}
