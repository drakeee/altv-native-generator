using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltV.Native.Generator
{
    internal class YSCFile
    {
        //Source: https://gtamods.com/wiki/Script_Container#Opcodes
        public ushort[] OpcodesSize = new ushort[]
        {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 2, 3, 4, 5, 5, 1, 1, 4, 5, 3, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1,
            2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4,
            4, 4, 2, 1, 1, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        };

        public string[] OpcodesName = new string[]
        {
            "NOP", "IADD", "ISUB", "IMUL", "IDIV", "IMOD", "INOT", "INEG", "IEQ", "INE", "IGT", "IGE", "ILT", "ILE", "FADD", "FSUB",
            "FMUL", "FDIV", "FMOD", "FNEG", "FEQ", "FNE", "FGT", "FGE", "FLT", "FLE", "VADD", "VSUB", "VMUL", "VDIV", "VNEG", "IAND",
            "IOR", "IXOR", "I2F", "F2I", "F2V", "PUSH_CONST_U8", "PUSH_CONST_U8_U8", "PUSH_CONST_U8_U8_U8", "PUSH_CONST_U32", "PUSH_CONST_F", "DUP", "DROP", "NATIVE", "ENTER", "LEAVE", "LOAD",
            "STORE", "STORE_REV", "LOAD_N", "STORE_N", "ARRAY_U8", "ARRAY_U8_LOAD", "ARRAY_U8_STORE", "LOCAL_U8", "LOCAL_U8_LOAD", "LOCAL_U8_STORE", "STATIC_U8", "STATIC_U8_LOAD", "STATIC_U8_STORE", "IADD_U8", "IMUL_U8", "IOFFSET",
            "IOFFSET_U8", "IOFFSET_U8_LOAD", "IOFFSET_U8_STORE", "PUSH_CONST_S16", "IADD_S16", "IMUL_S16", "IOFFSET_S16", "IOFFSET_S16_LOAD", "IOFFSET_S16_STORE", "ARRAY_U16", "ARRAY_U16_LOAD", "ARRAY_U16_STORE", "LOCAL_U16", "LOCAL_U16_LOAD", "LOCAL_U16_STORE", "STATIC_U16",
            "STATIC_U16_LOAD", "STATIC_U16_STORE", "GLOBAL_U16", "GLOBAL_U16_LOAD", "GLOBAL_U16_STORE", "J", "JZ", "IEQ_JZ", "INE_JZ", "IGT_JZ", "IGE_JZ", "ILT_JZ", "ILE_JZ", "CALL", "GLOBAL_U24", "GLOBAL_U24_LOAD",
            "GLOBAL_U24_STORE", "PUSH_CONST_U24", "SWITCH", "STRING", "STRINGHASH", "TEXT_LABEL_ASSIGN_STRING", "TEXT_LABEL_ASSIGN_INT", "TEXT_LABEL_APPEND_STRING", "TEXT_LABEL_APPEND_INT", "TEXT_LABEL_COPY", "CATCH", "THROW", "CALLINDIRECT", "PUSH_CONST_M1", "PUSH_CONST_0", "PUSH_CONST_1",
            "PUSH_CONST_2", "PUSH_CONST_3", "PUSH_CONST_4", "PUSH_CONST_5", "PUSH_CONST_6", "PUSH_CONST_7", "PUSH_CONST_FM1", "PUSH_CONST_F0", "PUSH_CONST_F1", "PUSH_CONST_F2", "PUSH_CONST_F3", "PUSH_CONST_F4", "PUSH_CONST_F5", "PUSH_CONST_F6", "PUSH_CONST_F7", "IS_BIT_SET"
        };

        //Source: https://gtamods.com/wiki/Script_Container#File_Format
        public struct YSCHeader
        {
            public long pageBase = 0; // 0x00
            public long pageMapPointer = 0; // 0x08
            public long CodeBlocksBasePointer = 0; // 0x10 Points to an array of code block offsets
            public uint GlobalsSignature = 0; // 0x18
            public uint CodeSize = 0; // 0x1C - The size of all the code tables
            public uint ParameterCount = 0; // 0x20 - These are for starting a script with args. The args appear at the start of the script static variables
            public uint StaticCount = 0; // 0x24 - The number of static variables in the script
            public uint GlobalCount = 0; // 0x28 - This is used for scripts that seem to initialise global variable tables
            public uint NativesCount = 0; // 0x2C - The total amount of natives in the native table
            public long StaticsPointer = 0; // 0x30 - The Offset in file where static variables are initialised
            public long GlobalsPointer = 0; // 0x38 - The Offset in file where global variales are initilaised(only used for registration scripts)
            public long NativesPointer = 0; // 0x40 - The Offset in file where the natives table is stored
            public long Null1 = 0; //0x48
            public long Null2 = 0; //0x50;
            public uint ScriptNameHash = 0; //0x58 - A Jenkins hash of the scripts name
            public uint UnkUsually1 = 0; //0x5C
            public long ScriptNamePointer = 0; //0x60 - Points to an offset in the file that has the name of the script
            public long StringBlocksBasePointer = 0; //0x68 - Points to an array of string block offsets
            public uint StringSize = 0; //0x70 - The Size of all the string tables
            public uint Null3 = 0; //0x74
            public uint Null4 = 0; //0x78
            public uint Null5 = 0; //0x7C
        }

        private uint CodePageCount = 0;
        private uint[] CodePagesSize;
        private uint[] CodePagesOffset;
        private byte[] ByteCodes;
        private SortedDictionary<uint, NativeInfo> NativeDict = new SortedDictionary<uint, NativeInfo>();

        public struct NativeInfo
        {
            //1 byte
            public ushort ArgumentSize = 0;
            public ushort ReturnSize = 0;

            //1 byte
            public ushort NativeIndex = 0;
            public ulong NativeHash = 0;

            public uint NativeCalls = 0;
            public List<uint> NativeOpcodeCalls = new List<uint>();

            public override String ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"Native = 0x{NativeHash.ToString("X16")}\n");
                sb.Append($"ArgumentSize = {ArgumentSize}\n");
                sb.Append($"ReturnSize = {ReturnSize}\n");
                sb.Append($"NativeCalls = {NativeCalls}\n");
                sb.Append($"NativeOpcodeCalls = [ {String.Join(", ", NativeOpcodeCalls)} ]\n");
                return sb.ToString();
            }
        }

        private MemoryStream _stream;
        private YSCHeader _header;

        private ulong[] _AllHashes;
        public ulong[] AllHashes { get { return this.GetAllHashes(); } }

        //public NativeInfo[] AllHashesInfo
        //{
        //    get { return this.GetAllHashesInfo(); }
        //}

        public YSCHeader Header
        {
            get { return this._header; }
        }

        public YSCFile(byte[] data)
        {
            _stream = new MemoryStream(data);
            this.ReadFile();
        }

        public YSCFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found in given path: \"{path}\"");

            _stream = new MemoryStream(File.ReadAllBytes(path));
            this.ReadFile();
        }

        private long GetPointerOffset(long ptr)
        {
            return Convert.ToInt64(ptr & 0xFFFFFF);
        }

        private void ReadFile()
        {
            _header = _stream.ReadStruct<YSCHeader>();

            this.CodePageCount = (_header.CodeSize + 0x3FFF) >> 14;

            _stream.Position = this.GetPointerOffset(_header.CodeBlocksBasePointer);

            List<uint> pageOffsets = new List<uint>();
            List<uint> pageSizes = new List<uint>();

            for (int index = 0; index < this.CodePageCount; index++)
            {
                //Calculate page's size
                if (index > (_header.CodeSize >> 14) || index < 0)
                    pageSizes.Add(0);
                else if (index == (_header.CodeSize >> 14))
                    pageSizes.Add(_header.CodeSize & 0x3FFF);
                else
                    pageSizes.Add(0x4000);

                //Get code pages offsets
                byte[] data = new byte[4];
                _stream.Read(data, 0, 4);
                pageOffsets.Add((uint)this.GetPointerOffset(BitConverter.ToUInt32(data)));

                //Skip 4 bytes
                _stream.Position += 4;
            }

            this.CodePagesOffset = pageOffsets.ToArray();
            this.CodePagesSize = pageSizes.ToArray();

            List<byte> tmpByteCodes = new List<byte>();

            for (int index = 0; index < this.CodePageCount; index++)
            {
                _stream.Position = this.CodePagesOffset[index];

                byte[] code = new byte[(int)this.CodePagesSize[index]];
                _stream.Read(code, 0, (int)this.CodePagesSize[index]);
                tmpByteCodes.AddRange(code);
            }

            this.ByteCodes = tmpByteCodes.ToArray();
            this.ProcessInstruction();
        }

        ~YSCFile()
        {
            this.Close();
        }

        private void ProcessInstruction()
        {
            int PC = 0;
            while (PC < this.ByteCodes.Length)
            {
                int opcode = this.ByteCodes[PC];

                switch(opcode)
                {
                    case 44: //Native
                        {
                            byte argAndReturn = this.ByteCodes[PC + 1];
                            ushort argumentSize = (ushort)(argAndReturn >> 2); //first 6 bit
                            ushort returnSize = (ushort)(argAndReturn & 0x3); //last 2 bit
                            ushort nativeIndex = (ushort)((this.ByteCodes[PC + 2] << 8) | this.ByteCodes[PC + 3]);

                            if (!this.NativeDict.ContainsKey(nativeIndex))
                            {
                                NativeInfo tmpInfo = new NativeInfo();
                                tmpInfo.NativeHash = this.AllHashes[nativeIndex];
                                tmpInfo.NativeIndex = nativeIndex;
                                tmpInfo.ReturnSize = returnSize;
                                tmpInfo.ArgumentSize = argumentSize;
                                tmpInfo.NativeCalls = 0;
                                this.NativeDict[nativeIndex] = tmpInfo;
                            }

                            NativeInfo tmp = this.NativeDict[nativeIndex];
                            tmp.NativeCalls++;
                            tmp.NativeOpcodeCalls.Add((uint)PC);
                            this.NativeDict[nativeIndex] = tmp;

                            //this.NativeDict
                            //Utils.Log.Info("NATIVE: 0x{0} - {1} - {2}", this.AllHashes[nativeIndex].ToString("X16"), argumentSize, returnSize);

                            PC += 4; //we processed 3 bytes in the process
                            break;
                        }
                    case 45: //ENTER - 5 bytes + value of 5th byte
                        {
                            PC += this.ByteCodes[PC + 4] + 5;
                            //Utils.Log.Info(OpcodesName[opcode]);
                            break;
                        }
                    case 98: //SWITCH - (Byte after opcode * 6) + 2 bytes
                        {
                            PC += ((this.ByteCodes[PC + 1] * 6) + 2);
                            //Utils.Log.Info(OpcodesName[opcode]);
                            break;
                        }
                    default:
                        //Utils.Log.Info(OpcodesName[opcode]);

                        PC += this.OpcodesSize[opcode];
                        break;
                }
            }
        }

        public void Close()
        {
            _stream.Close();
            _stream.DisposeAsync();
        }

        public SortedDictionary<uint, NativeInfo> GetNativeDictionary() { return this.NativeDict; }

        public ulong[] GetAllHashes()
        {
            if (this._AllHashes != null)
                return this._AllHashes;

            List<ulong> hashes = new List<ulong>();
            for (uint index = 0; index < _header.NativesCount; index++)
            {
                hashes.Add(this.GetNativeHash(index));
            }

            this._AllHashes = hashes.ToArray();
            return this._AllHashes;
        }

        //public NativeInfo GetNativeInfo(ulong hash)
        //{
        //    NativeInfo info = new NativeInfo();

        //    return info;
        //}

        //public NativeInfo[] GetAllHashesInfo()
        //{
        //    List<NativeInfo> natives = new List<NativeInfo>();

        //    return natives.ToArray();
        //}

        public ulong GetNativeHash(uint index)
        {
            if (index >= _header.NativesCount)
                return 0;

            //Skip bytes
            _stream.Position = (_header.NativesPointer & 0xFFFFFF) + (index * 8);

            //Read native hash
            byte[] nativeBytes = new byte[8];
            _stream.Read(nativeBytes, 0, 8);
            ulong nativeValue = BitConverter.ToUInt64(nativeBytes);

            //Rotate it to get the right hash
            int rotate = (int)(index + _header.CodeSize) & 0x3F;
            return (((nativeValue << rotate) | (nativeValue >> (64 - rotate))));
        }
    }
}
