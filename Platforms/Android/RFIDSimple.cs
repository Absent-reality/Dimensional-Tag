using Android.Content;
using Android.Nfc;
using Android.Nfc.Tech;
using AndroidX.Core.Content;
using CommunityToolkit.Maui.Views;
using DimensionalTag.Enums;
using DimensionalTag.Interfaces;
using System.Text;

namespace DimensionalTag
{
    public class RFID_Simple
    {
        public const string ALERT_TITLE = "Oops!";

        //
        // Summary:
        //     UUID is the Serial Number, called MAC sometimes
        public static byte[] Uid = [0];

        //
        // Summary:
        //     Authentication key
        public static byte[] AuthenticationKey = [0];

        //
        // Summary:
        //     The Data which has been read or to write for the specific block
        public static byte[] Data { get; set; } = new byte[0];

        /// <summary>
        ///     The Information about the card. Whether vehicle or character.
        /// </summary>
        public static string CardInfo { get; set; } = "";

        //
        // Summary:
        //     The command to execute on the card
        public static UltralightCommand CardCommand { get; set; }

        /// <summary>
        ///    Instance of Tag (not sure that we really need this...
        /// </summary>
        public static bool TagConnected { get; set; } = false;

        public static UltralightCardType CardType { get; set; }

        public static int NdefCapacity;

        public static StringBuilder ForDebug = new();
        //
        // Summary:
        //     The block number to authenticate or read or write
        public static byte BlockNumber { get; set; }

        /// <summary>
        /// Switch intent based on device version.
        /// </summary>
        /// <param name="intent"></param>
        /// <returns></returns>
        public static string TryRead(Intent intent)
        {
            try
            {               
                Tag? thisTag;

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)
                {
                    //get Tag from Intent
                    var cSharpType = typeof(Tag);
                    var javaType = Java.Lang.Class.FromType(cSharpType);
                    thisTag = (Tag?)IntentCompat.GetParcelableExtra(intent, NfcAdapter.ExtraTag, javaType);

                    if (thisTag == null)
                    {
                        ErrorReport("Failed to read tag.");                       
                        return "";
                    }
                    CardInfo = GrabTagInfo(thisTag);
                }
                else
                {
#pragma warning disable CA1422
                    // intent.GetParcelableArrayExtra(string) is obsolete on android 33 and later.
                    thisTag = (Tag?)intent.GetParcelableExtra(NfcAdapter.ExtraTag);                
#pragma warning restore CA1422

                    if (thisTag == null) 
                    {
                        ErrorReport("Failed to read tag.");
                        return "";
                    }
                    CardInfo = GrabTagInfo(thisTag);
                }
            }
            catch (Exception e)
            {
                ErrorReport(e.Message);
            }
            return CardInfo;
        }

        /// <summary>
        /// Start reading tag info.
        /// </summary>
        /// <param name="tag">Nfc tag from intent.</param>
        /// <returns></returns>
        public static string GrabTagInfo(Tag tag)
        {
            try
            {
                StringBuilder info = new();               
                Tag thisTag = tag;

                var uuid = thisTag.GetId();
                if (uuid != null)
                {
                    Uid = uuid;
                }
                else
                {
                    ErrorReport("Failed to read tag id.");
                }

                ForDebug.AppendLine("Uid: " + BitConverter.ToString(Uid));
                AuthenticationKey = LegoTag.GenerateCardPassword(Uid);
                ForDebug.AppendLine("AuthenticationKey: " + BitConverter.ToString(AuthenticationKey));

                string tech = "";
                var this_tag = thisTag.GetTechList();
                if (this_tag != null)
                {
                    foreach (string t in this_tag) //check for card capability
                    {
                       tech += t + "\n";
                    }
                    ForDebug.AppendLine("Available tech: " + tech);
                }

                if (tech.Contains("android.nfc.tech.NfcA"))
                {
                    NfcA? nfcA = NfcA.Get(thisTag);
                    if (nfcA != null)
                    {
                        nfcA.Connect();

                        if (nfcA.IsConnected)
                        {
                            TagConnected = true;

                            //Let's get card type
                            CardCommand = UltralightCommand.GetVersion;
                            byte[] dataFromCard = SendAway(nfcA); //Get byte[] based on command setting.

                            GetVersion(dataFromCard);
                            ForDebug.AppendLine($"Tag type: {CardType} \n Capacity: {NdefCapacity}");
                            try
                            {
                                //Try to read from block 0x24. 
                                BlockNumber = 0x24;
                                CardCommand = UltralightCommand.Read16Bytes; //Reads 4 pages starting at BlockNumber
                                dataFromCard = SendAway(nfcA);

                                if (IsEmptyCard(dataFromCard.AsSpan(8, 4).ToArray()))
                                {
                                    ErrorReport("Tag doesnt contain lego data and may be empty.");
                                    return "";
                                }
                            }
                            catch 
                            {  //try again
                                try
                                {
                                    //Get authorization to read the card
                                    CardCommand = UltralightCommand.PasswordAuthentication;
                                    dataFromCard = SendAway(nfcA);

                                    //Try to read from block 0x24
                                    BlockNumber = 0x24;
                                    CardCommand = UltralightCommand.Read16Bytes; //Reads 4 pages starting at BlockNumber
                                    dataFromCard = SendAway(nfcA);
                                }
                                catch (Exception e)
                                {
                                    ErrorReport($"Failed to read tag. Select a different tag and try again.{e.Message}");
                                    return "";
                                }
                            }
                            
                            // If page 0x26 == 00 01 00 00 we have a vehicle
                            if (LegoTag.IsVehicle(dataFromCard.AsSpan(8, 4).ToArray()))
                            {
                                info.AppendLine("Found a vehicle!\r\n");

                                // The 2 first one used
                                var id = LegoTag.GetVehicleId(dataFromCard);
                                ForDebug.AppendLine($" Vehicle ID: {id}: ");
                                Vehicle? vec = Vehicle.Vehicles.FirstOrDefault(m => m.Id == id);
                                if (vec is not null)
                                {
                                    info.AppendLine($" {vec.Name}, {vec.Rebuild} build. World: {vec.World}.\r\n");
                                    info.AppendLine("  Capabilities: ");
                                    for (int i = 0; i < vec.Abilities.Count; i++)
                                    {
                                        info.AppendLine($"{vec.Abilities[i]}{(i != vec.Abilities.Count - 1 ? ", " : "")}");
                                    }
                                }
                                else
                                {
                                    ErrorReport("Vehicle does not exist!");
                                }
                            }
                            else
                            {
                                info.AppendLine("Found a character!\r\n");
                                var id = LegoTag.GetCharacterId(Uid, dataFromCard.AsSpan(0, 8).ToArray());
                                ForDebug.AppendLine($" Character ID: {id}: ");
                                if (id == 0)
                                {
                                    ErrorReport("Character does not exist! Make sure tag is a lego character or vehicle.");
                                    return "";
                                }
                                Character? car = Character.Characters.FirstOrDefault(m => m.Id == id);
                                if (car is not null)
                                {
                                    info.AppendLine($"{car.Name} - World: {car.World}.\r\n");
                                    info.AppendLine("  Capabilities: ");
                                    for (int i = 0; i < car.Abilities.Count; i++)
                                    {
                                        info.AppendLine($"{car.Abilities[i]}{(i != car.Abilities.Count - 1 ? ", " : "")}");
                                    }
                                }
                                else
                                {
                                    ErrorReport("Character does not exist!");
                                }
                            }
                            CardInfo = info.ToString();
                            nfcA.Close();
                            if (!nfcA.IsConnected) { TagConnected = false; }
                        }                        
                    }
                }

            }
            catch (Exception e)
            {
                ErrorReport("Failed to read tag. Tag maybe empty, or incompatible.");
                ForDebug.AppendLine(e.Message);
            }
            return CardInfo;

        }

        /// <summary>
        /// Switch intent based on device version.
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="id">Lego tag id.</param>
        /// <param name="legoType">Either "Character" or "Vehicle".</param>
        public static void PrepareToWrite(Intent intent, ushort id, string legoType)
        {
            try
            {
                Tag? thisTag;

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)
                {
                    //get Tag from Intent
                    var cSharpType = typeof(Tag);
                    var javaType = Java.Lang.Class.FromType(cSharpType);
                    thisTag = (Tag?)IntentCompat.GetParcelableExtra(intent, NfcAdapter.ExtraTag, javaType);

                    if (thisTag == null)
                    {
                        ErrorReport("Failed to read tag.");
                        return;
                    }
                    BeginWrite(thisTag, id, legoType);

                }
                else
                {
#pragma warning disable CA1422
                    // intent.GetParcelableArrayExtra(string) is obsolete on android 33 and later.
                    thisTag = (Tag?)intent.GetParcelableExtra(NfcAdapter.ExtraTag);
#pragma warning restore CA1422

                    if (thisTag == null) 
                    {
                        ErrorReport("Failed to read tag.");
                        return; 
                    }
                    BeginWrite(thisTag, id, legoType);

                }
            }
            catch (System.Exception e)
            {
                ErrorReport(e.Message);
                return;
            }
            
        }


        public static void BeginWrite(Tag? tag, ushort id, string legoType)
        {
            try
            {

                StringBuilder info = new();
                Tag? thisTag = tag;

                var uuid = thisTag?.GetId();
                if (uuid != null)
                {
                    Uid = uuid;
                }
                else
                {
                    ErrorReport("Failed to read tag id.");
                }

                ForDebug.AppendLine("Uid: " + BitConverter.ToString(Uid));
                AuthenticationKey = LegoTag.GenerateCardPassword(Uid);
                ForDebug.AppendLine("AuthenticationKey: " + BitConverter.ToString(AuthenticationKey));

                string tech = "";
                foreach (string t in thisTag.GetTechList()) //check for card capability
                {
                    tech += t + "\n";
                }
                ForDebug.AppendLine("Available tech: " + tech);

                if (tech.Contains("android.nfc.tech.NfcA"))
                {
                    NfcA? nfcA = NfcA.Get(thisTag);
                    if (nfcA != null)
                    {
                        nfcA.Connect();

                        if (nfcA.IsConnected)
                        {
                            TagConnected = true;

                            //Let's get card type
                            CardCommand = UltralightCommand.GetVersion;
                            byte[] dataFromCard = SendAway(nfcA); //Get byte[] based on command setting.
                            GetVersion(dataFromCard);
                            ForDebug.AppendLine($"Cardtype: {CardType} \n Capacity: {NdefCapacity}");

                            if (CardType == UltralightCardType.UltralightNtag213 || CardType == UltralightCardType.UltralightNtag215 || CardType == UltralightCardType.UltralightNtag216)
                            {
                                try
                                {
                                    //Try to read from block 0x24. 
                                    BlockNumber = 0x24;
                                    CardCommand = UltralightCommand.Read16Bytes; //Reads 4 pages starting at BlockNumber
                                    dataFromCard = SendAway(nfcA);

                                    if (IsEmptyCard(dataFromCard.AsSpan(8, 4).ToArray()))
                                    {
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
                                    }
                                    else
                                    {
                                        ErrorReport("Card is not empty. Please use a blank card.");
                                        ForDebug.AppendLine($"Theres something here. {dataFromCard}");
                                        return;
                                    }

                                    try
                                    {
                                        Data = AuthenticationKey;
                                        CardCommand = UltralightCommand.Write4Bytes;
                                        SendAway(nfcA);
                                    }
                                    catch (Exception e)
                                    {
                                        ErrorReport(e.Message);
                                    }

                                    switch (legoType)
                                    {
                                        case "Character":
                                            {
                                                // Get the encrypted character ID
                                                var car = LegoTag.EncrypCharactertId(Uid, id);

                                                // Write Charater to blocks 0x24 and 0x25.
                                                Data = car.AsSpan(0, 4).ToArray();
                                                BlockNumber = 0x24;
                                                CardCommand = UltralightCommand.Write4Bytes;
                                                SendAway(nfcA);

                                                Data = car.AsSpan(4, 4).ToArray();
                                                BlockNumber = 0x25;
                                                CardCommand = UltralightCommand.Write4Bytes;
                                                SendAway(nfcA);

                                                //Try to read from block 0x25. 
                                                BlockNumber = 0x25;
                                                CardCommand = UltralightCommand.Read16Bytes; //Reads 4 pages starting at BlockNumber
                                                dataFromCard = SendAway(nfcA);
                                                if (!dataFromCard.SequenceEqual(Data.AsSpan(0, 4).ToArray()))
                                                {
                                                    ForDebug.AppendLine("Failed to write to card.");
                                                    return;
                                                }

                                            }
                                            break;

                                        case "Vehicle":
                                            {
                                                // Get the encrypted vehicle ID
                                                var vec = LegoTag.EncryptVehicleId(id);

                                                // Write Vehicle to blocks 0x24 and 0x26.
                                                Data = vec;
                                                BlockNumber = 0x24;
                                                CardCommand = UltralightCommand.Write4Bytes;
                                                SendAway(nfcA);

                                                Data = new byte[] { 0x00, 0x01, 0x00, 0x00 };
                                                BlockNumber = 0x26;
                                                CardCommand = UltralightCommand.Write4Bytes;
                                                SendAway(nfcA);

                                                //Try to read from block for a match. 
                                                BlockNumber = 0x26;
                                                CardCommand = UltralightCommand.Read16Bytes; //Reads 4 pages starting at BlockNumber
                                                dataFromCard = SendAway(nfcA);
                                                if (!dataFromCard.SequenceEqual(Data.AsSpan(0, 4).ToArray()))
                                                {
                                                    ForDebug.AppendLine("Failed to write to card.");
                                                    return;
                                                }
                                            }
                                            break;

                                    }

                                }
                                catch (Exception e)
                                {
                                    ErrorReport(e.Message);
                                    return;
                                }
                            }
                            else
                            {                                
                                ErrorReport($"Not a supported NTAG card, only 213, 215 and 216 ar supported. \n Current detected type: {CardType}");
                                return;
                            }

                          nfcA.Close();
                          if (!nfcA.IsConnected) { TagConnected = false; }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorReport(e.Message);
            }

        }

        /// <summary>
        /// Display an alert
        /// </summary>
        /// <param name="message">Message to be displayed</param>
        /// <param name="title">Alert title</param>
        /// <returns>The task to be performed</returns>
        public static Task Announce(string message, string title) => Shell.Current.ShowPopupAsync(new AlertPopup(title, message, "Ok", "", false));
        public static Task ErrorReport(string message, string title = null) => Shell.Current.ShowPopupAsync(new AlertPopup(string.IsNullOrWhiteSpace(title) ? ALERT_TITLE : title, message, "Ok", "", false));
        /// <summary>
        /// Serialize command for sending to card.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] Serialize()
        {
            byte[] array = null;

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
        public static byte[] GetByteSize()
        {
            byte[] array = new byte[0];

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
        public static byte[] SendAway(NfcA nfcA)
        {
            
           byte[] dataToSend = Serialize(); //get back serialized command array.
           byte[] dataFromCard = GetByteSize(); //get proper array size
           dataFromCard = nfcA.Transceive(dataToSend); //send command and receive response as array
            return dataFromCard;
        }

        /// <summary>
        /// Checks if the tag is a vehicles.
        /// </summary>
        /// <param name="data">Page 0x26 of the NFC data.</param>
        /// <returns>True if the tag is a vehicle.</returns>
        public static bool IsEmptyCard(byte[] data)
        {
            return data.SequenceEqual(new byte[] { 0x00, 0x00, 0x00, 0x00 });
        }


        /// <summary>
        /// Gets Card type and capacity
        /// </summary>
        /// <param name="cardInfo"></param>
        public static void GetVersion(byte[] cardInfo)
        {
            if (cardInfo != null && cardInfo.Length == 8)
            {
                if (cardInfo[2] == 4 && cardInfo[3] == 1 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 11)
                { NdefCapacity = 48;     CardType = UltralightCardType.UltralightNtag210; }
                                                       
                else if (cardInfo[2] == 4 && cardInfo[3] == 1 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 14)
                { NdefCapacity = 128;    CardType = UltralightCardType.UltralightNtag212; }
                                                   
                else if (cardInfo[2] == 4 && cardInfo[3] == 2 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 15)
                { NdefCapacity = 144;    CardType = UltralightCardType.UltralightNtag213; }
                                                 
                else if (cardInfo[2] == 4 && cardInfo[3] == 4 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 15)
                { NdefCapacity = 144;    CardType = UltralightCardType.UltralightNtag213F; }                                  
               
                else if (cardInfo[2] == 4 && cardInfo[3] == 2 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 17)
                { NdefCapacity = 504;    CardType = UltralightCardType.UltralightNtag215; }
                                                 
                else if (cardInfo[2] == 4 && cardInfo[3] == 1 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 19)
                { NdefCapacity = 888;    CardType = UltralightCardType.UltralightNtag216; }
                                                 
                else if (cardInfo[2] == 4 && cardInfo[3] == 4 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 19)
                { NdefCapacity = 888;    CardType = UltralightCardType.UltralightNtag216F; }
                                                  
                else if (cardInfo[2] == 4 && cardInfo[3] == 2 && cardInfo[4] == 1 && cardInfo[5] == 1 && cardInfo[6] == 19)
                { NdefCapacity = 888;    CardType = UltralightCardType.UltralightNtagI2cNT3H1101; }                   
                              
                else if (cardInfo[2] == 4 && cardInfo[3] == 5 && cardInfo[4] == 2 && cardInfo[5] == 1 && cardInfo[6] == 19)
                { NdefCapacity = 888;    CardType = UltralightCardType.UltralightNtagI2cNT3H1101W0; }                                   
               
                else if (cardInfo[2] == 4 && cardInfo[3] == 5 && cardInfo[4] == 2 && cardInfo[5] == 2 && cardInfo[6] == 19)
                { NdefCapacity = 888;    CardType = UltralightCardType.UltralightNtagI2cNT3H2111W0; }                                   
               
                else if (cardInfo[2] == 4 && cardInfo[3] == 2 && cardInfo[4] == 1 && cardInfo[5] == 1 && cardInfo[6] == 21)
                { NdefCapacity = 1912;   CardType = UltralightCardType.UltralightNtagI2cNT3H2101; }
                                                 
                else if (cardInfo[2] == 4 && cardInfo[3] == 5 && cardInfo[4] == 2 && cardInfo[5] == 1 && cardInfo[6] == 21)
                { NdefCapacity = 1912;   CardType = UltralightCardType.UltralightNtagI2cNT3H1201W0; }
                                                  
                else if (cardInfo[2] == 4 && cardInfo[3] == 5 && cardInfo[4] == 2 && cardInfo[5] == 2 && cardInfo[6] == 21)
                { NdefCapacity = 1912;   CardType = UltralightCardType.UltralightNtagI2cNT3H2211W0; }
                                                    
                else if (cardInfo[2] == 3 && cardInfo[3] == 1 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 11)
                { NdefCapacity = 48;     CardType = UltralightCardType.UltralightEV1MF0UL1101; }
                                                   
                else if (cardInfo[2] == 3 && cardInfo[3] == 2 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 11)
                { NdefCapacity = 48;     CardType = UltralightCardType.UltralightEV1MF0ULH1101; }
                                                 
                else if (cardInfo[2] == 3 && cardInfo[3] == 1 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 14)
                { NdefCapacity = 128;    CardType = UltralightCardType.UltralightEV1MF0UL2101; }                                 
               
                else if (cardInfo[2] == 3 && cardInfo[3] == 2 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 14)
                { NdefCapacity = 128;    CardType = UltralightCardType.UltralightEV1MF0ULH2101; }
                                                   
                else if (cardInfo[2] == 33 && cardInfo[3] == 1 && cardInfo[4] == 1 && cardInfo[5] == 0 && cardInfo[6] == 14)
                { NdefCapacity = 128;    CardType = UltralightCardType.UltralightNtag203; }
                                                  
                else
                { NdefCapacity = 1 << (cardInfo[6] >> 1); }
                                  
                return;
            }
        }
        //
        // Summary:
        //     Get the number of blocks for a specific sector
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

    }
}
