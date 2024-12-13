#if ANDROID || WINDOWS

using DimensionalTag.Portal;
using System.ComponentModel;

namespace DimensionalTag
{
    /// <summary>
    /// Instance of a Lego Dimensions Portal.
    /// </summary>
    public class LegoPortal : IDisposable, ILegoPortal, INotifyPropertyChanged
    {

        //Class variables
        private byte _messageId;
        private const int ReceiveTimeout = 2000;
        private Thread? _readThread;
        private CancellationTokenSource? _cancelThread; 
        private List<PresentTag> _presentTags = [];
        private bool _nfcEnabled = true;
        private bool _isConnected = false;
        private PortalConnectionService? _service;

        // We do have only 3 Pads
        // This one is to store the last message ID request for details
        private List<PadTag> _padTag = [];
        private List<CommandId> _commandId = [];

        /// <inheritdoc/>
        public event EventHandler<LegoTagEventArgs>? LegoTagEvent;

        /// <summary>
        /// Gets the list of present tags.
        /// </summary>
        public IEnumerable<PresentTag> PresentTags => _presentTags;

        /// <inheritdoc/>
        public bool NfcEnabled
        {
            get => _nfcEnabled;
            set
            {
                if (_nfcEnabled == value) 
                    return;
                _nfcEnabled = value;
                var message = new Message(MessageCommand.ConfigActive);
                message.AddPayload(_nfcEnabled);
                SendMessage(message);
                OnPropertyChanged(nameof(NfcEnabled));
            }
        }

        public bool IsConnected 
        {
            get => _isConnected;
            set
            {
                if (_isConnected == value)
                    return;
                _isConnected = value;
                OnPropertyChanged(nameof(IsConnected));
            }
        } 

        /// <inheritdoc/>
        public bool GetTagDetails { get; set; } = true;
        public byte[]? SerialNumber { get; internal set; }
       
        public int Id { get; internal set; }

        public LegoPortal()
        {
            //Use a service for Platform specific usb connection.
            PortalConnectionService service = new();

            var connection = service.GetConnection();
            IsConnected = service.OpenIt();
            if (!IsConnected)
            {
                return;
            }

            // Read the first 32 bytes
            var readBuffer = new byte[32];
            var returned = service.GetIt(readBuffer);
            _service = service;

            // Start the read thread
            _cancelThread = new CancellationTokenSource();
            Task.Run(() =>
            {
                ReadThread(_cancelThread.Token);
            });

            // WakeUp the portal
            Task.Run(WakeUp);
        }

        /// <inheritdoc/>
        public void WakeUp()
        {
            Message message = new(MessageCommand.Wake);
            message.AddPayload("(c) LEGO 2014");
            _messageId = 0;
            var getSerial = new ManualResetEvent(false);
            SerialNumber = [];
            var commandId = new CommandId(SendMessage(message), MessageCommand.Wake, getSerial);
            _commandId.Add(commandId);

            getSerial.WaitOne(ReceiveTimeout, true);

            if (commandId.Result != null)
            { 
                SerialNumber = (byte[])commandId.Result; 
            } 

            _commandId.Remove(commandId);
            
        }

        /// <inheritdoc/>
        public void SetColor(Pad pad, Color color)
        {
            Message message = new(MessageCommand.Color);
            message.AddPayload(pad, color);
            SendMessage(message);
        }

        /// <inheritdoc/>
        public Color GetColor(Pad pad)
        {
            Message message = new(MessageCommand.GetColor);
            message.AddPayload(pad);
            var getColor = new ManualResetEvent(false);
            var commandId = new CommandId(SendMessage(message), MessageCommand.GetColor, getColor);
            _commandId.Add(commandId);

            // In case we won't get any color, use the default black one
            Color padColor = Color.Black;

            // Wait maximum 2 seconds
            getColor.WaitOne(ReceiveTimeout, true);

            if (commandId.Result != null)
            { 
                padColor = (Color)commandId.Result; 
            }
               
            _commandId.Remove(commandId);
            return padColor;
        }

        /// <inheritdoc/>
        public void SetColorAll(Color padCenter, Color padLeft, Color padRight)
        {
            Message message = new(MessageCommand.ColorAll);
            message.AddPayload(true, padCenter, true, padLeft, true, padRight);
        }

        /// <inheritdoc/>
        public void SwitchOffAll()
        {
            Message message = new(MessageCommand.ColorAll);
            message.AddPayload(false, Color.Black, false, Color.Black, false, Color.Black);
            SendMessage(message);
        }

        /// <inheritdoc/>
        public void Flash(Pad pad, FlashPad flashPad)
        {
            Message message = new(MessageCommand.Flash);
            message.AddPayload(pad, flashPad.TickOn, flashPad.TickOff, flashPad.TickCount, flashPad.Color);
            SendMessage(message);
        }

        /// <inheritdoc/>
        public void FlashAll(FlashPad flashPadCenter, FlashPad flashPadLeft, FlashPad flashPadRight)
        {
            Message message = new(MessageCommand.FlashAll);
            message.AddPayload(flashPadCenter.Enabled, flashPadCenter.TickOn, flashPadCenter.TickOff, flashPadCenter.TickCount, flashPadCenter.Color);
            message.AddPayload(flashPadLeft.Enabled, flashPadLeft.TickOn, flashPadLeft.TickOff, flashPadLeft.TickCount, flashPadLeft.Color);
            message.AddPayload(flashPadRight.Enabled, flashPadRight.TickOn, flashPadRight.TickOff, flashPadRight.TickCount, flashPadRight.Color);
            SendMessage(message);
        }

        /// <inheritdoc/>
        public void Fade(Pad pad, FadePad fadePad)
        {
            Message message = new(MessageCommand.Fade);
            message.AddPayload(pad, fadePad.TickTime, fadePad.TickCount, fadePad.Color);
            SendMessage(message);
        }

        /// <inheritdoc/>
        public void FadeAll(FadePad fadePadCenter, FadePad fadePadLeft, FadePad fadePadRight)
        {
            Message message = new(MessageCommand.FadeAll);
            message.AddPayload(fadePadCenter.Enabled, fadePadCenter.TickTime, fadePadCenter.TickCount, fadePadCenter.Color);
            message.AddPayload(fadePadLeft.Enabled, fadePadLeft.TickTime, fadePadLeft.TickCount, fadePadLeft.Color);
            message.AddPayload(fadePadRight.Enabled, fadePadRight.TickTime, fadePadRight.TickCount, fadePadRight.Color);
            SendMessage(message);
        }

        /// <inheritdoc/>
        public void FadeRandom(Pad pad, byte tickTime, byte tickCount)
        {
            Message message = new(MessageCommand.FadeRandom);
            message.AddPayload(pad, tickTime, tickCount);
            SendMessage(message);
        }

        /// <summary>
        /// Read 16 bytes from a tag.
        /// </summary>
        /// <param name="index">The tag index to read.</param>
        /// <param name="page">The page to read.</param>
        /// <returns>A byte array of 16 bytes in case of success.</returns>
        public byte[] ReadTag(byte index, byte page)
        {
            Message message = new(MessageCommand.Read);
            message.AddPayload(index, page);
            var getRead = new ManualResetEvent(false);
            var commandId = new CommandId(SendMessage(message), MessageCommand.Read, getRead);
            _commandId.Add(commandId);

            byte[] readBytes = [];         
            getRead.WaitOne(ReceiveTimeout, true);

            if (commandId.Result != null)
            {
                readBytes = (byte[])commandId.Result;
            }

            _commandId.Remove(commandId);
            return readBytes;
        }

        /// <summary>
        /// Write 4 bytes to a tag.
        /// </summary>
        /// <param name="index">The tag index to write.</param>
        /// <param name="page">The page to write.</param>
        /// <param name="bytes">An array of 4 bytes to write.</param>
        /// <returns>True if success.</returns>
        public bool WriteTag(byte index, byte page, byte[] bytes)
        {
            if (bytes.Length != 4)
            {
                throw new ArgumentException("Write to card must be 4 bytes.");
            }

            Message message = new(MessageCommand.Write);
            message.AddPayload(index, page, bytes);
            var getWrite = new ManualResetEvent(false);
            var commandId = new CommandId(SendMessage(message), MessageCommand.Write, getWrite);
            _commandId.Add(commandId);

            bool success = false;
            getWrite.WaitOne(ReceiveTimeout, true);

            if (commandId.Result != null)
            { 
                success = (bool)commandId.Result;
            }     

            _commandId.Remove(commandId);
            return success;
        }

        /// <summary>
        /// Read 8 bytes from a tag at page 0x24 and 0x25.
        /// </summary>
        /// <param name="encryoptedIndex">The tag index to read encrypted on 8 bytes.</param>
        /// <returns>A byte array of 8 bytes in case of success.</returns>
        public byte[] GetTagInformation(byte[] encryoptedIndex)
        {
            Message message = new(MessageCommand.Model);
            message.AddPayload(encryoptedIndex);
            var getRead = new ManualResetEvent(false);
            var commandId = new CommandId(SendMessage(message), MessageCommand.Model, getRead);
            _commandId.Add(commandId);

            byte[] readBytes = [];
            getRead.WaitOne(ReceiveTimeout, true);

            if (commandId.Result != null)
            {
                readBytes = (byte[])commandId.Result;
            }

            _commandId.Remove(commandId);
            return readBytes;
        }

        /// <summary>
        /// Get a challenge from the portal.
        /// </summary>
        /// <returns>A byte array of 8 bytes in case of success.</returns>
        public byte[] GetChallenge()
        {
            Message message = new(MessageCommand.Challenge);
            var getChallenge = new ManualResetEvent(false);
            var commandId = new CommandId(SendMessage(message), MessageCommand.Challenge, getChallenge);
            _commandId.Add(commandId);

            byte[] readBytes = [];
            getChallenge.WaitOne(ReceiveTimeout, true);

            if (commandId.Result != null)
            {
                readBytes = (byte[])commandId.Result;
            }

            _commandId.Remove(commandId);
            return readBytes;
        }

        public IEnumerable<PresentTag> ListTags()
        {
            Message message = new(MessageCommand.TagList);
            var getTagList = new ManualResetEvent(false);
            var commandId = new CommandId(SendMessage(message), MessageCommand.TagList, getTagList);
            _commandId.Add(commandId);
            while (!getTagList.WaitOne(ReceiveTimeout))
            { }

            // We don't do anything as we manage the result globally
            _commandId.Remove(commandId);

            return _presentTags;
        }

        /// <summary>
        /// Sets the tag password behavior.
        /// </summary>
        /// <param name="password">The desired state, Automatic is the default value.</param>
        /// <param name="index">The tag index.</param>
        /// <param name="newPassword">The new 4 bytes password if any.</param>
        public void SetTagPassword(PortalPassword password, byte index, byte[] newPassword = null)
        {
            if (password == PortalPassword.Custom)
            {
                if (newPassword != null && newPassword.Length != 4)
                {
                    throw new ArgumentException("New password must be 4 bytes");
                }
            }

            Message message = new Message(MessageCommand.ConfigPassword);
            message.AddPayload((byte)password, index, newPassword == null ? new byte[4] : newPassword);
            SendMessage(message);
        }

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="messageId">The message ID, leave to 0 to use the internal message count.</param>
        /// <returns>The message ID for the request.</returns>
        public byte SendMessage(Message message, byte messageId = 0)
        {
            var bytes = message.GetBytes(messageId == 0 ? IncreaseMessageId() : messageId);
            var returned = _service!.SendIt(bytes);
            // Assume it's awake if we send 32 bytes properly
            System.Diagnostics.Debug.WriteLine($"SND: {BitConverter.ToString(bytes)}");
            return messageId == 0 ? _messageId : messageId;
        }

        private void ReadThread(CancellationToken token)
        {
            // Read is always 32 bytes
            var readBuffer = new byte[32];
            int bytesRead;

            while (!token!.IsCancellationRequested)
            {
                try
                {
                    bytesRead = _service!.GetIt(readBuffer);
                    if (bytesRead > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"REC: {BitConverter.ToString(readBuffer)}");
                    }

                    if (bytesRead == 32)
                    {
                        var message = Message.CreateFromBuffer(readBuffer, MessageSource.Portal);
                        if (message.MessageType == MessageType.Event)
                        {
                            // In the case of an event the Message Type event, all is in the payload
                            byte pad = (byte)message.Payload[0];
                            if ((pad < 1) || (pad > 3))
                            {
                                // Not a valid message
                                continue;
                            }

                            bool present = message.Payload[3] == 0;
                            var tadType = (TagType)message.Payload[1];
                            byte[] uuid = new byte[7];
                            Array.Copy(message.Payload, 4, uuid, 0, uuid.Length);
                            // Find the tag if existing in the list
                            var legoTag = _padTag.FirstOrDefault(m => m.CardUid.SequenceEqual(uuid));
                            byte padIndex = message.Payload[2];
                            if (present)
                            {

                                if (legoTag == null)
                                {
                                    _presentTags.Add(new PresentTag((Pad)pad, tadType, padIndex, uuid));
                                    legoTag = new PadTag() { Pad = (Pad)pad, TagIndex = padIndex, Present = present, CardUid = uuid, TagType = tadType };
                                    _padTag.Add(legoTag);

                                    if (GetTagDetails)
                                    {
                                        // Ask for more with the read command for 0x24
                                        var msgToSend = new Message(MessageCommand.Read);
                                        msgToSend.AddPayload(padIndex, (byte)0x24);
                                        legoTag.LastMessageId = SendMessage(msgToSend);
                                        _commandId.Add(new CommandId(legoTag.LastMessageId, MessageCommand.Read, null));
                                    }
                                    else
                                    {
                                        // Directly raise the event
                                        LegoTagEvent?.Invoke(this, new LegoTagEventArgs(legoTag));
                                    }
                                }
                                else
                                {
                                    legoTag.Present = present;
                                }
                            }
                            else
                            {
                                if (legoTag != null)
                                {
                                    legoTag.Present = present;
                                    LegoTagEvent?.Invoke(this, new LegoTagEventArgs(legoTag));
                                    var presentTag = _presentTags.FirstOrDefault(m => m.Pad == legoTag.Pad && m.Index == legoTag.TagIndex);
                                    if (presentTag != null)
                                    {
                                        _presentTags.Remove(presentTag);
                                    }

                                    _padTag.Remove(legoTag);
                                }
                            }
                        }
                        else if (message.MessageType == MessageType.Normal)
                        {
                            // In case the paylod is 17, then we do have a response to a read command
                            var cmdId = _commandId.Where(m => m.MessageId == _messageId).FirstOrDefault();
                            if (message.MessageCommand == MessageCommand.None && cmdId != null && cmdId.MessageCommand == MessageCommand.Read)
                            {
                                // In this case the request is coming from the event
                                if (cmdId.ManualResetEvent == null)
                                {
                                    var legoTag = _padTag.FirstOrDefault(m => m.LastMessageId == message.MessageId);
                                    if (legoTag == null)
                                    {
                                        continue;
                                    }

                                    // We should have our 0x24
                                    if (LegoPortal.IsEmptyCard(message.Payload.AsSpan(1, 8).ToArray()))
                                    {
                                        legoTag.LegoTag = null;
                                    }

                                    else if (LegoTagTools.IsVehicle(message.Payload.AsSpan(9, 4).ToArray()))
                                    {
                                        var vecId = LegoTagTools.GetVehicleId(message.Payload.AsSpan(1, 4).ToArray());
                                        var vec = Vehicle.Vehicles.FirstOrDefault(m => m.Id == vecId);
                                        legoTag.LegoTag = vec;
                                    }

                                    else
                                    {
                                        var carId = LegoTagTools.GetCharacterId(legoTag.CardUid, message.Payload.AsSpan(1, 8).ToArray());
                                        var car = Character.Characters.FirstOrDefault(m => m.Id == carId);
                                        legoTag.LegoTag = car;
                                    }

                                    LegoTagEvent?.Invoke(this, new LegoTagEventArgs(legoTag));
                                }
                                else
                                {
                                    // This case is a normal read and we will set the buffer
                                    if (message.Payload[0] == 0)
                                    {
                                        // if no error, we set the result
                                        cmdId.Result = message.Payload[1..];
                                    }

                                    cmdId.ManualResetEvent.Set();
                                }
                            }
                            else if (message.MessageCommand == MessageCommand.None && cmdId != null && cmdId.MessageCommand == MessageCommand.GetColor)
                            {
                                cmdId.Result = Color.FromArgb(message.Payload[0], message.Payload[1], message.Payload[2]);
                                cmdId.ManualResetEvent?.Set();
                            }
                            else if (message.MessageCommand == MessageCommand.None && cmdId != null && cmdId.MessageCommand == MessageCommand.Model)
                            {
                                if (message.Payload[0] == 0)
                                {
                                    // if no error, we set the result
                                    cmdId.Result = message.Payload[1..];
                                }

                                cmdId.ManualResetEvent?.Set();
                            }
                            else if (message.MessageCommand == MessageCommand.None && cmdId != null && cmdId.MessageCommand == MessageCommand.Write)
                            {
                                cmdId.Result = message.Payload[0] == 0;
                                cmdId.ManualResetEvent?.Set();
                            }
                            else if (message.MessageCommand == MessageCommand.None && cmdId != null && cmdId.MessageCommand == MessageCommand.Wake)
                            {
                                cmdId.Result = message.Payload[10..];
                                cmdId.ManualResetEvent?.Set();
                            }
                            else if (message.MessageCommand == MessageCommand.None && cmdId != null && cmdId.MessageCommand == MessageCommand.Challenge)
                            {
                                cmdId.Result = message.Payload;
                                cmdId.ManualResetEvent?.Set();
                            }
                            else if (message.MessageCommand == MessageCommand.None && cmdId != null && cmdId.MessageCommand == MessageCommand.TagList)
                            {
                                _presentTags.Clear();
                                for (int i = 0; i < message.Payload.Length / 2; i++)
                                {
                                    var index = (byte)(message.Payload[i * 2] & 0xF);
                                    var uid = _padTag.FirstOrDefault(x => x.TagIndex == index)?.CardUid ?? Array.Empty<byte>();
                                    PresentTag presentTag = new PresentTag((Pad)(message.Payload[i * 2] >> 4), (TagType)message.Payload[i * 2 + 1], index, uid);
                                    _presentTags.Add(presentTag);
                                }

                                cmdId.ManualResetEvent?.Set();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception: {ex}");
                }
            }
        }

        private byte IncreaseMessageId()
        {
            _messageId = (byte)(_messageId == 255 ? 1 : ++_messageId);
            return _messageId;
        }

        /// <summary>
        /// Checks if the tag is empty.
        /// </summary>
        /// <param name="data">16 bytes of the NFC data read from card.</param>
        /// <returns>True if the tag is empty.</returns>
        public static bool IsEmptyCard(byte[] data)
        {
            var result = data.SequenceEqual(new byte[] { 0x00, 0x00, 0x00, 0x00,
                                                   0x00, 0x00, 0x00, 0x00, });
            return result;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if(_cancelThread == null) return;

            _cancelThread.Cancel();

            // Make sure the thread is stopped
            _readThread?.Join();
            IsConnected = _service!.CloseIt(); 
            GC.SuppressFinalize(this);
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventArgs ea = new PropertyChangedEventArgs(propertyName);
            PropertyChanged?.Invoke(this, ea);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
#endif

