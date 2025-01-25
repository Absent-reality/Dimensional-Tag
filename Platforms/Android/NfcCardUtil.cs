using Android.Content;
using Android.Nfc;
using Android.Nfc.Tech;
using AndroidX.Core.Content;
using System.ComponentModel;
using System.Text;

namespace DimensionalTag
{
    public class NfcCardUtil : INotifyPropertyChanged
    {
        private byte[] Uid = [0];
        private byte[] AuthenticationKey = [0];
        private readonly byte[] vehicleBlock = [0x00, 0x01, 0x00, 0x00];
        private readonly byte[] emptyBlock = [0x00, 0x00, 0x00, 0x00];
        private UltralightCommand CardCommand { get; set; }
        private UltralightCardType CardType { get; set; }
        private int NdefCapacity;
        private byte BlockNumber { get; set; }

        /// <summary>
        /// In the event debug info is desired.
        /// </summary>
        public StringBuilder ForDebug = new();

        public IAlert Alert;

        /// <summary>
        /// Toy tag to be obtained.
        /// </summary>
        private ToyTag? _readTag = null;
        public ToyTag? ReadTag
        {
            get { return _readTag; }
            set
            {
                if (_readTag == value)
                    return;
                _readTag = value;
                OnPropertyChanged(nameof(ReadTag));
            }
        }

        /// <summary>
        /// Data to be passed for commands.
        /// </summary>
        private byte[] _data = [0];
        public byte[] Data
        {
            get { return _data; }
            set
            {
                if (_data == value)
                    return;
                _data = value;
                OnPropertyChanged(nameof(Data));
            }
        }

        /// <summary>
        /// Represents the connection to the nfc tag.
        /// </summary>
        private bool _tagConnected = false;
        public bool TagConnected
        {
            get { return _tagConnected; }
            set
            {
                if (_tagConnected == value)
                    return;
                _tagConnected = value;
                OnPropertyChanged(nameof(TagConnected));
            }
        }

        /// <summary>
        /// To ignore existing data warning when writing vehicle tag.
        /// </summary>
        private bool _overWriteTag = false;
        public bool OverWriteTag
        {
            get { return _overWriteTag; }
            set
            {
                if (_overWriteTag == value)
                    return;
                _overWriteTag = value;
                OnPropertyChanged(nameof(OverWriteTag));
            }
        }

        /// <summary>
        /// Bool to erase (write zeros) to tag.
        /// </summary>
        private bool _shouldEraseTag = false;
        public bool ShouldEraseTag
        {
            get { return _shouldEraseTag; }
            set
            {
                if (_shouldEraseTag == value)
                    return;
                _shouldEraseTag = value;
                OnPropertyChanged(nameof(ShouldEraseTag));
            }
        }

        public NfcCardUtil(IAlert alertInterface)
        {
            Alert = alertInterface;
        }

        /// <summary>
        /// Switch intent based on device version.
        /// </summary>
        /// <param name="intent"></param>
        /// <returns>Status of card read attempt.</returns>
        public async Task<ProgressStatus> TryRead(Intent intent)
        {
            ForDebug = new();

            try
            {
                Tag? thisTag;
                ProgressStatus progressStatus = new();

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)
                {
                    //get Tag from Intent
                    var cSharpType = typeof(Tag);
                    var javaType = Java.Lang.Class.FromType(cSharpType);
                    thisTag = (Tag?)IntentCompat.GetParcelableExtra(intent, NfcAdapter.ExtraTag, javaType);

                    if (thisTag == null)
                    {
                        progressStatus = ProgressStatus.NoConnection;
                    }
                    else
                    {
                        progressStatus = await GrabTagInfo(thisTag);
                    }
                }
                else
                {
#pragma warning disable CA1422
                    // intent.GetParcelableArrayExtra(string) is obsolete on android 33 and later.
                    thisTag = (Tag?)intent.GetParcelableExtra(NfcAdapter.ExtraTag);
#pragma warning restore CA1422

                    if (thisTag == null)
                    {
                        progressStatus = ProgressStatus.NoConnection;
                    }
                    else
                    {
                        progressStatus = await GrabTagInfo(thisTag);
                    }
                }

                return progressStatus;
            }
            catch (Exception e)
            {
                ForDebug.AppendLine($"@TryRead, exception: {e.Message}");
                return ProgressStatus.Failed;
            }
        }

        /// <summary>
        /// Start reading tag info.
        /// </summary>
        /// <param name="tag">Nfc tag from intent.</param>
        /// <returns>Object representing the card.</returns>
        public async Task<ProgressStatus> GrabTagInfo(Tag tag)
        {
            Tag thisTag = tag;
            var uuid = thisTag.GetId();
            if (uuid == null)
            {
                ForDebug.AppendLine("@GrabTagInfo, GetId: Failed to read tag id. ");
                return ProgressStatus.Failed;
            }

            Uid = uuid;
            ForDebug.AppendLine("Uid: " + BitConverter.ToString(Uid));
            AuthenticationKey = LegoTagTools.GenerateCardPassword(Uid);
            ForDebug.AppendLine("AuthenticationKey: " + BitConverter.ToString(AuthenticationKey));

            string tech = "";
            var this_tag = thisTag.GetTechList();
            if (this_tag == null)
            {
                ForDebug.AppendLine($"@GrabTagInfo, TechList: {nameof(Tag)}: {tech}. Failed to get list.");
                return ProgressStatus.Failed;
            }

            foreach (string t in this_tag) //check for card capability
            {
                tech += t + "\n";
            }

            ForDebug.AppendLine("Available tech: " + tech);

            if (!tech.Contains("android.nfc.tech.NfcA"))
            {
                ForDebug.AppendLine("@GrabTagInfo: NfcA unavailible.");
                return ProgressStatus.Failed;
            }

            NfcA? nfcA = NfcA.Get(thisTag);
            if (nfcA == null)
            {
                ForDebug.AppendLine("@GrabTagInfo, Get: nfcA null");
                return ProgressStatus.Failed;
            }
            try
            {
                nfcA.Connect();
                TagConnected = nfcA.IsConnected;
                if (!TagConnected)
                {
                    ForDebug.AppendLine("GrabTagInfo, nfca.Connect: No Connection to tag.");
                    return ProgressStatus.NoConnection;
                }

                //Let's get card type
                CardCommand = UltralightCommand.GetVersion;
                byte[] dataFromCard = SendAway(nfcA); //Get byte[] based on command setting.

                GetVersion(dataFromCard);
                ForDebug.AppendLine($"Tag type: {CardType} \n Capacity: {NdefCapacity}");

                //Try to read from block 0x24. 
                BlockNumber = 0x24;
                CardCommand = UltralightCommand.Read16Bytes; //Reads 4 pages starting at BlockNumber
                try
                {
                   dataFromCard = SendAway(nfcA);
                }
                catch (Exception e)
                {
                    ForDebug.AppendLine($"@GrabTagInfo, first read: {e.Message}");

                    //Incase of original tag, then we'll neet to get authorization to read the card
                    CardCommand = UltralightCommand.PasswordAuthentication;
                    dataFromCard = SendAway(nfcA);

                    //Try to read from block 0x24
                    BlockNumber = 0x24;
                    CardCommand = UltralightCommand.Read16Bytes; //Reads 4 pages starting at BlockNumber
                    dataFromCard = SendAway(nfcA);
                } 

                if (IsEmptyCard(dataFromCard.AsSpan(0, 16).ToArray()))
                {
                    ForDebug.AppendLine(" Tag doesnt contain lego data or may be empty. ");
                    ForDebug.AppendLine($"@GrabTagInfo, IsEmpty? Block 0x24:");
                    ForDebug.AppendLine($"{BitConverter.ToString(dataFromCard.AsSpan(0, 4).ToArray())} \n " +
                    $"{BitConverter.ToString(dataFromCard.AsSpan(4, 4).ToArray())} \n " +
                    $"{BitConverter.ToString(dataFromCard.AsSpan(8, 4).ToArray())} \n " +
                    $"{BitConverter.ToString(dataFromCard.AsSpan(12, 4).ToArray())}");
                    return ProgressStatus.EmptyData;                  
                }
                // If page 0x26 == 00 01 00 00 we have a vehicle
                if (LegoTagTools.IsVehicle(dataFromCard.AsSpan(8, 4).ToArray()))
                {
                    ForDebug.AppendLine("Found a vehicle!\r\n");

                    var id = LegoTagTools.GetVehicleId(dataFromCard);
                    ForDebug.AppendLine($" Vehicle ID: {id}: ");

                    if (id > 999 && id < 1300)
                    {
                        Vehicle? vec = Vehicle.Vehicles.FirstOrDefault(m => m.Id == id);
                        if (vec == null)
                        {
                            // ErrorReport("Vehicle does not exist!");
                            ForDebug.AppendLine($"@GrabTagInfo: No such Vehicle: {id}");
                            return ProgressStatus.Failed;
                        }

                        ForDebug.AppendLine($" {vec.Name}, {vec.Rebuild} build. World: {vec.World}.\r\n");
                        ForDebug.AppendLine("  Capabilities: ");
                        for (int i = 0; i < vec.Abilities.Count; i++)
                        {
                            ForDebug.AppendLine($"{vec.Abilities[i]}{(i != vec.Abilities.Count - 1 ? ", " : "")}");
                        }
               
                        ReadTag = new ToyTag( vec.Id, vec.Name, vec.World, vec.Abilities, ToyTagType.Vehicle );
                    }
                }
                else
                {
                    ForDebug.AppendLine("Found a character!\r\n");
                    var id = LegoTagTools.GetCharacterId(Uid, dataFromCard.AsSpan(0, 8).ToArray());
                    ForDebug.AppendLine($" Character ID: {id}: ");
                    if (id > 0 && id < 999)
                    {
                        Character? car = Character.Characters.FirstOrDefault(m => m.Id == id);

                        if (car == null)
                        {
                            // ErrorReport("Character does not exist!");
                            ForDebug.AppendLine($"@GrabTagInfo: No such Charater: {id}");
                            return ProgressStatus.Failed;
                        }

                        ForDebug.AppendLine($"{car.Name} - World: {car.World}.\r\n");
                        ForDebug.AppendLine("  Capabilities: ");
                        for (int i = 0; i < car.Abilities.Count; i++)
                        {
                            ForDebug.AppendLine($"{car.Abilities[i]}{(i != car.Abilities.Count - 1 ? ", " : "")}");
                        }

                        ReadTag = new ToyTag(car.Id, car.Name, car.World,  car.Abilities, ToyTagType.Character );
                    }
                    else
                    {
                        // ErrorReport("Item does not exist! Make sure tag is a lego character or vehicle.");
                        ForDebug.AppendLine("@GrabTagInfo: Failed to match id to vehicle or charater");
                        return ProgressStatus.Failed;
                    }
                }
            }
            catch (Exception e)
            {              
                await Alert.SendAlert("Oops", "Failed to read tag.\n Tag maybe empty, or incompatible.", "Ok", "", false);
                ForDebug.AppendLine($"@GrabTagInfo, exception: {e.Message}");
            }

            nfcA.Close();
            TagConnected = nfcA.IsConnected;

            return ProgressStatus.Success;
        }

        /// <summary>
        /// Switch intent based on device version.
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="toyTag">The tag to be written.</param>
        public async Task<ProgressStatus> PrepareToWrite(Intent intent, ToyTag toyTag)
        {
            ForDebug = new();

            try
            {
                Tag? thisTag;
                ProgressStatus progressStatus = new();

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)
                {
                    //get Tag from Intent
                    var cSharpType = typeof(Tag);
                    var javaType = Java.Lang.Class.FromType(cSharpType);
                    thisTag = (Tag?)IntentCompat.GetParcelableExtra(intent, NfcAdapter.ExtraTag, javaType);

                    if (thisTag == null)
                    {
                        progressStatus = ProgressStatus.NoConnection;
                    }
                    else
                    {
                        progressStatus = await BeginWrite(thisTag, toyTag);
                    }

                }
                else
                {
#pragma warning disable CA1422
                    // intent.GetParcelableArrayExtra(string) is obsolete on android 33 and later.
                    thisTag = (Tag?)intent.GetParcelableExtra(NfcAdapter.ExtraTag);
#pragma warning restore CA1422

                    if (thisTag == null)
                    {
                        progressStatus = ProgressStatus.NoConnection;
                    }
                    else
                    {
                        progressStatus = await BeginWrite(thisTag, toyTag);
                    }
                }

                return progressStatus;
            }
            catch (Exception e)
            {
                ForDebug.AppendLine($"@PrepareWrite exception: {e.Message}");
                return ProgressStatus.Failed;
            }

        }


        private async Task<ProgressStatus> BeginWrite(Tag tag, ToyTag toyTag)
        {

            Tag thisTag = tag;

            var uuid = thisTag?.GetId();
            if (uuid == null)
            {
                ForDebug.AppendLine("@BeginWrite, GetId: Failed to read tag id.");
                return ProgressStatus.Failed;
            }

            Uid = uuid;

            ForDebug.AppendLine("Uid: " + BitConverter.ToString(Uid));
            AuthenticationKey = LegoTagTools.GenerateCardPassword(Uid);
            ForDebug.AppendLine("AuthenticationKey: " + BitConverter.ToString(AuthenticationKey));

            string tech = "";
            var this_tag = thisTag?.GetTechList();
            if (this_tag == null)
            {
                ForDebug.AppendLine($"@BeginWrite, GetTech: {nameof(Tag)}: {tech}. Failed to get list.");
                return ProgressStatus.Failed;
            }

            foreach (string t in this_tag) //check for card capability
            {
                tech += t + "\n";
            }

            ForDebug.AppendLine("Available tech: " + tech);

            NfcA? nfcA = NfcA.Get(thisTag);
            if (nfcA == null)
            {
                ForDebug.AppendLine("@BeginWrite, NfcA.Get: nfcA null");
                return ProgressStatus.Failed;
            }

            try
            {
                nfcA.Connect();
                TagConnected = nfcA.IsConnected;
                if (!TagConnected)
                {
                    ForDebug.AppendLine("@BeginWrite, Connect: No Connection to tag.");
                    return ProgressStatus.NoConnection;
                }

                //Let's get card type
                CardCommand = UltralightCommand.GetVersion;
                byte[] dataFromCard = SendAway(nfcA); //Get byte[] based on command setting.
                GetVersion(dataFromCard);
                ForDebug.AppendLine($"Cardtype: {CardType}\n Capacity: {NdefCapacity}");

                if (CardType != UltralightCardType.UltralightNtag213 && CardType != UltralightCardType.UltralightNtag215 && CardType != UltralightCardType.UltralightNtag216)
                {
                    ForDebug.AppendLine($"@BeginWrite, CardType: Incorrect card type {CardType}");
                    return ProgressStatus.IncorrectType;
                }

                //Try to read from block 0x24. 
                BlockNumber = 0x24;
                CardCommand = UltralightCommand.Read16Bytes; //Reads 4 pages starting at BlockNumber
                dataFromCard = SendAway(nfcA);
                if (toyTag.ToyTagType == ToyTagType.Vehicle && OverWriteTag || ShouldEraseTag) { }
                else
                {
                    if (!IsEmptyCard(dataFromCard.AsSpan(0, 16).ToArray()))
                    {
                        ForDebug.AppendLine($"@BeginWrite, IsEmpty?: There's something here.");
                        ForDebug.AppendLine($"{BitConverter.ToString(dataFromCard.AsSpan(0, 4).ToArray())} \n " +
                                            $"{BitConverter.ToString(dataFromCard.AsSpan(4, 4).ToArray())} \n " +
                                            $"{BitConverter.ToString(dataFromCard.AsSpan(8, 4).ToArray())} \n " +
                                            $"{BitConverter.ToString(dataFromCard.AsSpan(12, 4).ToArray())}");

                        return ProgressStatus.DataExists;
                    }
                }

                switch (CardType)
                {
                    case UltralightCardType.UltralightNtag213:
                        { BlockNumber = 0x2B; }
                        break;

                    case UltralightCardType.UltralightNtag215:
                        { BlockNumber = 0x85; }
                        break;

                    case UltralightCardType.UltralightNtag216:
                        { BlockNumber = 0xE5; }
                        break;
                }

                Data = AuthenticationKey;
                CardCommand = UltralightCommand.Write4Bytes;
                SendAway(nfcA);

                if (ShouldEraseTag)
                {                  
                    var progress = await EraseTag(nfcA, [0x24, 0x25, 0x26]);
                    return progress;
                }
                else
                {
                    switch (toyTag.ToyTagType)
                    {
                        case ToyTagType.Character:
                            {
                                // Get the encrypted character ID
                                var car = LegoTagTools.EncrypCharactertId(Uid, toyTag.Id);

                                // Write Charater to blocks 0x24 and 0x25.
                                Data = car.AsSpan(0, 4).ToArray();
                                BlockNumber = 0x24;
                                CardCommand = UltralightCommand.Write4Bytes;
                                SendAway(nfcA);

                                Data = car.AsSpan(4, 4).ToArray();
                                BlockNumber = 0x25;
                                CardCommand = UltralightCommand.Write4Bytes;
                                SendAway(nfcA);

                                // Write Vehicle to blocks 0x24 and 0x26.
                                Data = emptyBlock;
                                BlockNumber = 0x26;
                                CardCommand = UltralightCommand.Write4Bytes;
                                SendAway(nfcA);

                                //Try to read from block 0x25. (Let's see if it worked.)
                                BlockNumber = 0x24;
                                CardCommand = UltralightCommand.Read16Bytes; //Reads 4 pages starting at BlockNumber
                                dataFromCard = SendAway(nfcA);
                                if (!dataFromCard.AsSpan(0, 8).SequenceEqual(car.AsSpan(0, 8).ToArray()))
                                {
                                    ForDebug.AppendLine($"@BeginWrite, Character: Failed to write to card. \n" +
                                        $"{BitConverter.ToString(dataFromCard)}");

                                    return ProgressStatus.Failed;
                                }
                            }
                            break;

                        case ToyTagType.Vehicle:
                            {
                                // Get the encrypted vehicle ID
                                var vec = LegoTagTools.EncryptVehicleId(toyTag.Id);

                                // Write Vehicle to blocks 0x24 and 0x26.
                                Data = vec;
                                BlockNumber = 0x24;
                                CardCommand = UltralightCommand.Write4Bytes;
                                SendAway(nfcA);

                                // Write Vehicle to blocks 0x24 and 0x26.
                                Data = emptyBlock;
                                BlockNumber = 0x25;
                                CardCommand = UltralightCommand.Write4Bytes;
                                SendAway(nfcA);

                                Data = vehicleBlock;
                                BlockNumber = 0x26;
                                CardCommand = UltralightCommand.Write4Bytes;
                                SendAway(nfcA);

                                //Try to read from block for a match. 
                                BlockNumber = 0x24;
                                CardCommand = UltralightCommand.Read16Bytes; //Reads 4 pages starting at BlockNumber
                                dataFromCard = SendAway(nfcA);
                                if (!dataFromCard.AsSpan(0, 4).SequenceEqual(vec.AsSpan(0, 4).ToArray()) || !dataFromCard.AsSpan(8, 4).SequenceEqual(vehicleBlock))
                                {
                                    ForDebug.AppendLine("@BeginWrite, vehicle: Failed to write to card.\n" +
                                        $"{BitConverter.ToString(dataFromCard)}");
                                    return ProgressStatus.Failed;
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                await Alert.SendAlert("Oops", "Something went wrong. \n Failed to write", "Ok", "", false);
                ForDebug.AppendLine($"@BeginWrite: {e.Message}");
                return ProgressStatus.Failed;
            }

            nfcA.Close();
            TagConnected = nfcA.IsConnected;

            return ProgressStatus.Success;
        }

        /// <summary>
        /// Erases (Writes 0's over) bytes at pages specified 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="pages"></param>
        /// <returns></returns>
        private async Task<ProgressStatus> EraseTag(NfcA tag, byte[] pages)
        {
            if (!tag.IsConnected) { return ProgressStatus.NoConnection; }
            if (pages == null || pages.Length <= 0)
            {
                ForDebug.AppendLine($"@EraseTag(): Invalid page #s");
                return ProgressStatus.Failed;
            }
            try
            {
                foreach (byte page in pages)
                {
                    // Write 0 to blocks from page #s.
                    Data = emptyBlock;
                    BlockNumber = page;
                    CardCommand = UltralightCommand.Write4Bytes;
                    SendAway(tag);
                }
                return ProgressStatus.Success;
            }
            catch (Exception e)
            {
                await Alert.SendAlert("Oops", "Something went wrong. \n Failed to write", "Ok", "", false);
                ForDebug.AppendLine($"@EraseTag(): {e.Message}");
                return ProgressStatus.Failed;
            }
        }

            /// <summary>
            /// Serialize command for sending to card.
            /// </summary>
            /// <returns></returns>
            /// <exception cref="ArgumentException"></exception>
            public byte[] Serialize()
            {
                byte[] array;
                switch (CardCommand)
                {
                    case UltralightCommand.GetVersion:
                        array = new byte[1] { (byte)CardCommand };
                        break;

                    case UltralightCommand.PasswordAuthentication:
                        {
                            byte[] data = AuthenticationKey;
                            if (data == null || data.Length != 4)
                            {
                                throw new ArgumentException("Authentication key can't be null and has to be 4 bytes.");
                            }

                            array = new byte[1 + AuthenticationKey.Length];
                            array[0] = (byte)CardCommand;
                            AuthenticationKey.CopyTo(array, 1);
                            break;
                        }

                    case UltralightCommand.Read16Bytes:
                        array = new byte[2] { (byte)CardCommand, BlockNumber };
                        break;

                    case UltralightCommand.Write4Bytes:
                        if (Data == null)
                        {
                            throw new ArgumentException("Card is not configured for writing.");
                        }

                        array = new byte[2 + Data.Length];
                        array[0] = (byte)CardCommand;
                        array[1] = BlockNumber;
                        if (Data.Length != 0)
                        {
                            Data.CopyTo(array, 2);
                        }

                        break;

                    default:
                        throw new ArgumentException("Not a supported command.");
                }
                return array;
            }

            /// <summary>
            /// Gets byte array size for a given Nfc command.
            /// </summary>
            /// <returns></returns>
            public byte[] GetByteSize()
            {
                byte[] array = [0];

                switch (CardCommand)
                {
                    case UltralightCommand.GetVersion:
                        { array = new byte[8]; }
                        break;

                    case UltralightCommand.PasswordAuthentication:
                        { array = new byte[2]; }
                        break;

                    case UltralightCommand.Read16Bytes:
                        { array = new byte[16]; }
                        break;
                }
                return array;
            }

        /// <summary>
        /// Prepare and send data to tag.
        /// </summary>
        /// <param name="nfcA"></param>        
        /// <returns>A Byte array response from tag.</returns>
        public byte[] SendAway(NfcA nfcA)
        {
            if (nfcA == null || !nfcA.IsConnected) return [];
            byte[] dataToSend = Serialize(); //get back serialized command array.
            byte[] dataFromCard = GetByteSize(); //get proper array size                                                
            dataFromCard = nfcA.Transceive(dataToSend); //send command and receive response as array
            if (dataFromCard == null) return [];
            return dataFromCard;
        }

            /// <summary>
            /// Checks if the tag is empty.
            /// </summary>
            /// <param name="data">16 bytes of the NFC data read from card.</param>
            /// <returns>True if the tag is empty.</returns>
            public bool IsEmptyCard(byte[] data)
            {
                var result = data.SequenceEqual(new byte[] { 0x00, 0x00, 0x00, 0x00,
                                                   0x00, 0x00, 0x00, 0x00,
                                                   0x00, 0x00, 0x00, 0x00,
                                                   0x00, 0x00, 0x00, 0x00 });
                return result;
            }

            /// <summary>
            /// Gets Card type and capacity
            /// </summary>
            /// <param name="cardInfo"></param>
            public void GetVersion(byte[] cardInfo)
            {
                if (cardInfo != null && cardInfo.Length == 8)
                {
                    if (cardInfo[2] == 4 && cardInfo[3] == 1 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 11)
                    { NdefCapacity = 48; CardType = UltralightCardType.UltralightNtag210; }

                    else if (cardInfo[2] == 4 && cardInfo[3] == 1 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 14)
                    { NdefCapacity = 128; CardType = UltralightCardType.UltralightNtag212; }

                    else if (cardInfo[2] == 4 && cardInfo[3] == 2 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 15)
                    { NdefCapacity = 144; CardType = UltralightCardType.UltralightNtag213; }

                    else if (cardInfo[2] == 4 && cardInfo[3] == 4 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 15)
                    { NdefCapacity = 144; CardType = UltralightCardType.UltralightNtag213F; }

                    else if (cardInfo[2] == 4 && cardInfo[3] == 2 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 17)
                    { NdefCapacity = 504; CardType = UltralightCardType.UltralightNtag215; }

                    else if (cardInfo[2] == 4 && cardInfo[3] == 1 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 19)
                    { NdefCapacity = 888; CardType = UltralightCardType.UltralightNtag216; }

                    else if (cardInfo[2] == 4 && cardInfo[3] == 4 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 19)
                    { NdefCapacity = 888; CardType = UltralightCardType.UltralightNtag216F; }

                    else if (cardInfo[2] == 4 && cardInfo[3] == 2 && cardInfo[4] == 1 && cardInfo[5] == 1 && cardInfo[6] == 19)
                    { NdefCapacity = 888; CardType = UltralightCardType.UltralightNtagI2cNT3H1101; }

                    else if (cardInfo[2] == 4 && cardInfo[3] == 5 && cardInfo[4] == 2 && cardInfo[5] == 1 && cardInfo[6] == 19)
                    { NdefCapacity = 888; CardType = UltralightCardType.UltralightNtagI2cNT3H1101W0; }

                    else if (cardInfo[2] == 4 && cardInfo[3] == 5 && cardInfo[4] == 2 && cardInfo[5] == 2 && cardInfo[6] == 19)
                    { NdefCapacity = 888; CardType = UltralightCardType.UltralightNtagI2cNT3H2111W0; }

                    else if (cardInfo[2] == 4 && cardInfo[3] == 2 && cardInfo[4] == 1 && cardInfo[5] == 1 && cardInfo[6] == 21)
                    { NdefCapacity = 1912; CardType = UltralightCardType.UltralightNtagI2cNT3H2101; }

                    else if (cardInfo[2] == 4 && cardInfo[3] == 5 && cardInfo[4] == 2 && cardInfo[5] == 1 && cardInfo[6] == 21)
                    { NdefCapacity = 1912; CardType = UltralightCardType.UltralightNtagI2cNT3H1201W0; }

                    else if (cardInfo[2] == 4 && cardInfo[3] == 5 && cardInfo[4] == 2 && cardInfo[5] == 2 && cardInfo[6] == 21)
                    { NdefCapacity = 1912; CardType = UltralightCardType.UltralightNtagI2cNT3H2211W0; }

                    else if (cardInfo[2] == 3 && cardInfo[3] == 1 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 11)
                    { NdefCapacity = 48; CardType = UltralightCardType.UltralightEV1MF0UL1101; }

                    else if (cardInfo[2] == 3 && cardInfo[3] == 2 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 11)
                    { NdefCapacity = 48; CardType = UltralightCardType.UltralightEV1MF0ULH1101; }

                    else if (cardInfo[2] == 3 && cardInfo[3] == 1 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 14)
                    { NdefCapacity = 128; CardType = UltralightCardType.UltralightEV1MF0UL2101; }

                    else if (cardInfo[2] == 3 && cardInfo[3] == 2 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 14)
                    { NdefCapacity = 128; CardType = UltralightCardType.UltralightEV1MF0ULH2101; }

                    else if (cardInfo[2] == 33 && cardInfo[3] == 1 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 14)
                    { NdefCapacity = 128; CardType = UltralightCardType.UltralightNtag203; }

                    else
                    { NdefCapacity = 1 << (cardInfo[6] >> 1); }

                    return;
                }
            }

        /// <summary>
        ///  Get the number of blocks for a specific sector
        /// </summary>
        public int NumberBlocks => CardType switch
        {
            UltralightCardType.UltralightNtag210 => 16,
            UltralightCardType.UltralightNtag212 => 45,
            UltralightCardType.UltralightNtag213 => 45,
            UltralightCardType.UltralightNtag213F => 45,
            UltralightCardType.UltralightNtag215 => 135,
            UltralightCardType.UltralightNtag216 => 231,
            UltralightCardType.UltralightNtag216F => 231,
            UltralightCardType.UltralightEV1MF0UL1101 => 41,
            UltralightCardType.UltralightEV1MF0ULH1101 => 41,
            UltralightCardType.UltralightEV1MF0UL2101 => NdefCapacity + 9,
            UltralightCardType.UltralightEV1MF0ULH2101 => NdefCapacity + 9,
            UltralightCardType.UltralightNtagI2cNT3H1101 => 231,
            UltralightCardType.UltralightNtagI2cNT3H1101W0 => 231,
            UltralightCardType.UltralightNtagI2cNT3H2111W0 => 485,
            UltralightCardType.UltralightNtagI2cNT3H2101 => 485,
            UltralightCardType.UltralightNtagI2cNT3H1201W0 => 485,
            UltralightCardType.UltralightNtagI2cNT3H2211W0 => 1020,
            UltralightCardType.UltralightC => 36,
            UltralightCardType.UltralightNtag203 => 168,
            UltralightCardType.MifareUltralight => 48,
            _ => 0,
        };

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventArgs ea = new PropertyChangedEventArgs(propertyName);
            PropertyChanged?.Invoke(this, ea);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

    }

}

    

