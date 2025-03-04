﻿using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using DimensionalTag.Portal;
using System.Text;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using System.Threading;
using System.Threading.Tasks;

namespace DimensionalTag
{
    public partial class PortalViewModel(AppSettings settings, IAlert alert) : BaseViewModel(settings, alert)
    {
        public IAlert Alerts { get; set; } = alert;
        public AppSettings AppSettings { get; set; } = settings;

        int PadNumber = 0;

        [ObservableProperty]
        StringBuilder toDebug = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NotEnabled))]
        [NotifyPropertyChangedFor(nameof(Tagged))]
        bool isConnected = false;

        [ObservableProperty]
        bool isDisconnecting = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Tagged))]
        bool cameToWrite = false;

        public bool WriteEnabled = false;

        public bool NotEnabled => !IsConnected;

        [ObservableProperty]
        Thread? newPortalLink;

        public CancellationTokenSource? CancelWriteRequest = null;

        [NotifyPropertyChangedFor(nameof(CenterPad_Present), nameof(CenterPad_Img))]
        [ObservableProperty]
        LegoTagEventArgs? _centerTag;

        [NotifyPropertyChangedFor(nameof(LeftPad0_Present), nameof(LeftPad0_Img))]
        [ObservableProperty]
        LegoTagEventArgs? _leftTag0;

        [NotifyPropertyChangedFor(nameof(LeftPad1_Present), nameof(LeftPad1_Img))]
        [ObservableProperty]
        LegoTagEventArgs? _leftTag1;

        [NotifyPropertyChangedFor(nameof(LeftPad2_Present), nameof(LeftPad2_Img))]
        [ObservableProperty]
        LegoTagEventArgs? _leftTag2;

        [NotifyPropertyChangedFor(nameof(LeftPad3_Present), nameof(LeftPad3_Img))]
        [ObservableProperty]
        LegoTagEventArgs? _leftTag3;

        public bool CenterPad_Present 
        {
            get { if (CenterTag is null) { return false; }              
                   else { return CenterTag.Present; } }                              
        }
               
        public string CenterPad_Img
        {
            get { if (CenterTag == null) { return ""; }
                   else { return ThisTagInfo(CenterTag); } }  
        }

        public bool LeftPad0_Present
        {
            get { if (LeftTag0 is null) { return false; }
                   else { return LeftTag0.Present; } }         
        }

        public string LeftPad0_Img
        {
            get { if (LeftTag0 == null) { return ""; }
                   else { return ThisTagInfo(LeftTag0); } }          
        }

        public bool LeftPad1_Present
        {
            get { if (LeftTag1 is null) { return false; }
                   else { return LeftTag1.Present; } }              
        }

        public string LeftPad1_Img
        {
            get { if (LeftTag1 == null) { return ""; }
                   else { return ThisTagInfo(LeftTag1); } }                                 
        }

        public bool LeftPad2_Present
        {
            get { if (LeftTag2 is null) { return false; }
                   else { return LeftTag2.Present; } }          
        }

        public string LeftPad2_Img
        {
            get { if (LeftTag2 == null) { return ""; }
                   else { return ThisTagInfo(LeftTag2); } }    
        }

        public bool LeftPad3_Present
        {
            get { if (LeftTag3 is null) { return false; }
                   else { return LeftTag3.Present; } }     
        }

        public string LeftPad3_Img
        {
            get { if (LeftTag3 == null) { return ""; }
                else { return ThisTagInfo(LeftTag3); } }
        }

        [NotifyPropertyChangedFor(nameof(RightPad0_Present), nameof(RightPad0_Img))]
        [ObservableProperty]
        LegoTagEventArgs? _rightTag0;

        [NotifyPropertyChangedFor(nameof(RightPad1_Present), nameof(RightPad1_Img))]
        [ObservableProperty]
        LegoTagEventArgs? _rightTag1;

        [NotifyPropertyChangedFor(nameof(RightPad2_Present), nameof(RightPad2_Img))]
        [ObservableProperty]
        LegoTagEventArgs? _rightTag2;

        [NotifyPropertyChangedFor(nameof(RightPad3_Present), nameof(RightPad3_Img))]
        [ObservableProperty]
        LegoTagEventArgs? _rightTag3;

        public bool RightPad0_Present
        {
            get { if (RightTag0 is null) { return false; }
                else { return RightTag0.Present; } } 
        }

        public string RightPad0_Img
        {
            get { if (RightTag0 == null) { return ""; }
                else { return ThisTagInfo(RightTag0); } }   
        }

        public bool RightPad1_Present
        {
            get { if (RightTag1 is null) { return false; }
                else { return RightTag1.Present; } }
        }

        public string RightPad1_Img
        {
            get { if (RightTag1 == null) { return ""; }   
                else { return ThisTagInfo(RightTag1); } }       
        }

        public bool RightPad2_Present
        {
            get { if (RightTag2 is null) { return false; }
                else { return RightTag2.Present; } }    
        }

        public string RightPad2_Img
        {
            get { if (RightTag2 == null) { return ""; }
                else { return ThisTagInfo(RightTag2); } } 
        }

        public bool RightPad3_Present
        {
            get { if (RightTag3 is null) { return false; }
                else { return RightTag3.Present; } }
        }

        public string RightPad3_Img
        {
            get { if (RightTag3 == null) { return ""; }
                else { return ThisTagInfo(RightTag3); } }  
        }

        public ObservableCollection<LegoTagEventArgs> Tags = new();

        [ObservableProperty]
        ObservableCollection<LegoTagEventArgs> _leftTagList = new();

        [ObservableProperty]
        ObservableCollection<LegoTagEventArgs> _rightTagList = new();

        [ObservableProperty]
        Microsoft.Maui.Graphics.Color color1 = Colors.Black;

        [ObservableProperty]
        Microsoft.Maui.Graphics.Color color2 = Colors.Black;

        [ObservableProperty]
        Microsoft.Maui.Graphics.Color color3 = Colors.Black;

        [ObservableProperty]
        Microsoft.Maui.Graphics.Color pickedColor = Colors.Black;

#if ANDROID || WINDOWS
        [ObservableProperty]
        LegoPortal? portal1;
#endif

        private string _tagged = "Please connect portal.";
         public string Tagged
        {
            get { if (!IsConnected) { _tagged = "Please connect portal."; }               
                  else if (IsConnected && CameToWrite) { _tagged = "Place empty tag on center pad."; }
                  else { _tagged = "Tap on pad to select, then tap on color to choose pad color."; }
                  return _tagged; }
           set 
            { if (_tagged == value) 
                    return;
                _tagged = value;
                OnPropertyChanged(nameof(Tagged));
            }
        }

        [RelayCommand]
        public void ImageTapped(string position)
        {
 #if ANDROID || WINDOWS 
            if (position is not null)
                switch (position)
                {
                    case "LeftPad":

                        PadNumber = 1;
                        Color1 = Colors.WhiteSmoke;
                        break;

                    case "RightPad":

                        PadNumber = 2;
                        Color2 = Colors.WhiteSmoke;
                        break;

                    case "CenterPad":

                        PadNumber = 3;
                        Color3 = Colors.WhiteSmoke;
                        break;
                }
#endif  
        }

        [RelayCommand]
        void EllipseTapped(string color)
        {
#if ANDROID || WINDOWS
            if (color != null)
               switch (PadNumber)
                {
                    case 1:

                        Color1 = Microsoft.Maui.Graphics.Color.Parse(color);
                        LightUpPad(1, Color1);
                        break;


                    case 2:

                        Color2 = Microsoft.Maui.Graphics.Color.Parse(color);
                        LightUpPad(2, Color2);
                        break;

                    case 3:

                        Color3 = Microsoft.Maui.Graphics.Color.Parse(color);
                        LightUpPad(3, Color3);
                        break;
                }           
#endif
        }

        [RelayCommand]
        private void Canvas_Tapped()
        {
#if ANDROID || WINDOWS
            switch (PadNumber)
            {
                case 1:

                    Color1 = PickedColor;
                    LightUpPad(1, Color1);
                    break;

                case 2:

                    Color2 = PickedColor;
                    LightUpPad(2, Color2);
                    break;

                case 3:

                    Color3 = PickedColor;
                    LightUpPad(3, Color3);
                    break;
            }
#endif
        }

        void LightUpPad(int padNum, Microsoft.Maui.Graphics.Color MauiColor)
        {
#if ANDROID || WINDOWS
            if (!IsConnected) { return; }

            var newColor = Color.FromHex(MauiColor.ToArgbHex());

            if (Portal1 != null)
                switch (padNum)
                {
                    case 1:
                        Portal1.SetColor(Pad.Left, newColor);
                        break;

                    case 2:
                        Portal1.SetColor(Pad.Right, newColor);
                        break;

                    case 3:
                        Portal1.SetColor(Pad.Center, newColor);
                        break;
                }           
#endif
        }

        [RelayCommand]
        async void CloseIt()
        {
#if ANDROID || WINDOWS
            if (IsDisconnecting) { return; }
            if (IsConnected && Portal1 != null)
            {
                IsDisconnecting = true;
                (RightTag0, RightTag1, RightTag2, RightTag3) = (null, null, null, null);
                (LeftTag0, LeftTag1, LeftTag2, LeftTag3) = (null, null, null, null);
                CenterTag = null;
                CameToWrite = false;
                Portal1.SetColorAll(Color.Black, Color.Black, Color.Black);
                Thread.Sleep(200);
                Task disconnect = Task.Run(Dispose);
                await disconnect;
                IsConnected = false;
                IsDisconnecting = false;
            }
#endif
        }

        [RelayCommand]
        void TestFade()
        {
#if ANDROID || WINDOWS
            if (Portal1 != null && IsConnected)
            Portal1.FadeAll(new FadePad(75, 2, Color.Purple), new FadePad(50, 2, Color.Cyan), new FadePad(50, 2, Color.Green));

            /*      
            Portal1.Fade(Pad.Center, new FadePad(50, 5, Color.Green));
            Thread.Sleep(200);
            Portal1.Fade(Pad.Left, new FadePad(255, 1, Color.Purple));
            Thread.Sleep(200);
            Portal1.Fade(Pad.Right, new FadePad(200, 5, Color.LightBlue));
            Thread.Sleep(2000);
            Portal1.FadeAll(new FadePad(200, 5, Color.Cyan), new FadePad(200, 5, Color.Cyan), new FadePad(200, 5, Color.Cyan));
            */
#endif
        }

        [RelayCommand]
         async Task DisplayItem(string item)
        {
#if ANDROID || WINDOWS
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
           object? tag = null;
            switch (item)
            {
                case "RightTag0":
                    if (RightTag0 is null) { return; }
                    tag = GetItem(RightTag0);
                    break;

                case "RightTag1":
                    if (RightTag1 is null) { return; }
                    tag = GetItem(RightTag1);
                    break;

                case "RightTag2":
                    if (RightTag2 is null) { return; }
                    tag = GetItem(RightTag2);
                    break;

                case "RightTag3":
                    if (RightTag3 is null) { return; }
                    tag = GetItem(RightTag3);
                    break;

                case "LeftTag0":
                    if (LeftTag0 is null) { return; }
                    tag = GetItem(LeftTag0);
                    break;

                case "LeftTag1":
                    if (LeftTag1 is null) { return; }
                    tag = GetItem(LeftTag1);
                    break;

                case "LeftTag2":
                    if (LeftTag2 is null) { return; }
                    tag = GetItem(LeftTag2);
                    break;

                case "LeftTag3":
                    if (LeftTag3 is null) { return; }
                    tag = GetItem(LeftTag3);
                    break;

                case "CenterTag":
                    if (CenterTag is null) { return; }
                    tag = GetItem(CenterTag);
                break;

            }
            if (tag is null) { return; }
               var popup = new PopupPage(false, tag);
               await Shell.Current.ShowPopupAsync(popup);
#endif
        }

        [RelayCommand]
        void GrabPortal()
        {
#if ANDROID || WINDOWS
            if (IsConnected && IsDisconnecting) return;
            NewPortalLink = new(GetNewPortal) { IsBackground = true };  
            NewPortalLink?.Start();
#endif
        }
#if ANDROID || WINDOWS
        public void Portal_LegoTagEvent(object? sender, LegoTagEventArgs e)
        {
            string tagInfo = "";

            Console.WriteLine($"Tag is present: {e.Present} - UID: {BitConverter.ToString(e.CardUid)}");
            if (e.Present)
            {
                if (e.CardUid != null)
                {
                    switch (e.Pad)
                    {
                        case Pad.Left:

                            var leftTags = LeftTagList.FirstOrDefault(m => m.CardUid.SequenceEqual(e.CardUid));
                            if (leftTags is null)
                            {
                                LeftTagList.Add(e);

                                if (LeftTag0 == null) { LeftTag0 = e; }
                                else if (LeftTag1 == null) { LeftTag1 = e; }
                                else if (LeftTag2 == null) { LeftTag2 = e; }
                                else if (LeftTag3 == null) { LeftTag3 = e; }

                            }
                            break;

                        case Pad.Right:

                            var rightTags = RightTagList.FirstOrDefault(m => m.CardUid.SequenceEqual(e.CardUid));
                            if (rightTags is null)
                            {
                                RightTagList.Add(e);

                                if (RightTag0 == null) { RightTag0 = e; }
                                else if (RightTag1 == null) { RightTag1 = e; }
                                else if (RightTag2 == null) { RightTag2 = e; }
                                else if (RightTag3 == null) { RightTag3 = e; }
                            }
                            break;

                        case Pad.Center:

                            if (CameToWrite)
                            {
                                CenterTag = e;
                                WriteEnabled = true;
                            }

                            else
                            {
                                if (CenterTag == null)
                                { CenterTag = e; }
                                                                
                                else if (!CenterTag.CardUid.SequenceEqual(e.CardUid))
                                { CenterTag = e; }                                                                
                            }
                            break;
                    }

                    Console.WriteLine($"Tag is a {e.LegoTag?.GetType().Name} - Name= {e.LegoTag?.Name}");
                    tagInfo = $"Pad: {e.Pad} \n Tag is a {e.LegoTag?.GetType().Name} \n Name= {e.LegoTag?.Name}";                   
                }
            }
            else if (!e.Present)
            {
                switch (e.Pad)
                {
                    case Pad.Left:

                        var leftTags = LeftTagList.FirstOrDefault(m => m.CardUid.SequenceEqual(e.CardUid));
                        if (leftTags != null)
                        {
                            LeftTagList.Remove(leftTags);

                            if (leftTags == LeftTag0) { LeftTag0 = null; }
                            else if (leftTags == LeftTag1) { LeftTag1 = null; }
                            else if (leftTags == LeftTag2) { LeftTag2 = null; }
                            else if (leftTags == LeftTag3) { LeftTag3 = null; }
                        }
                        break;

                    case Pad.Right:

                        var rightTags = RightTagList.FirstOrDefault(m => m.CardUid.SequenceEqual(e.CardUid));
                        if (rightTags != null)
                        {
                            RightTagList.Remove(rightTags);

                            if (rightTags == RightTag0) { RightTag0 = null; }
                            else if (rightTags == RightTag1) { RightTag1 = null; }
                            else if (rightTags == RightTag2) { RightTag2 = null; }
                            else if (rightTags == RightTag3) { RightTag3 = null; }
                        }
                        break;

                    case Pad.Center:
                        if (CenterTag != null)
                        {
                            if (CenterTag!.CardUid.SequenceEqual(e.CardUid))
                            {
                                CenterTag = null;
                                WriteEnabled = false;
                            }
                        }
                        break;
                }
            }
        }
#endif
        [RelayCommand]
        void CancelWrite()
        {
            if (CancelWriteRequest == null) { return; }
            CancelWriteRequest.Cancel();
        }

#if ANDROID || WINDOWS
        public async Task<ProgressStatus> PrepareWrite(ToyTag tagType)
        {
            GrabPortal();
            ProgressStatus taskStatus = new();
            ToDebug = new();
            CancelWriteRequest = new();
            var token = CancelWriteRequest.Token;

            if (!IsConnected)
            {
                int i = 0;
                while(i < 2000)
                {
                    if (IsConnected) { break; }
                    await Task.Delay(100);
                    i++;
                }
            }
            
            if (Portal1 is null || !IsConnected || IsDisconnecting) { return ProgressStatus.TimedOut; }

            Task<ProgressStatus> task = BeginWrite(tagType, token);
           
            try
            {
                taskStatus = await task;
            }
            catch (Exception ex)            
            { System.Diagnostics.Debug.WriteLine($"Exception: {ex}"); }
            
            if (task.IsCanceled) { taskStatus = ProgressStatus.Cancelled; } 

             var (title, message, cancelText, confirmText, debug) = ("", "", "Ok.", "", false);                                        
            switch (taskStatus)
            {
                case ProgressStatus.Cancelled:
                    (title, message) = ("", "Write cancelled.");
                    break;

                case ProgressStatus.TimedOut:
                    break;

                case ProgressStatus.NoConnection:
                    title = "Oops...";
                    message = "Portal not detected. Please connect lego portal and try again.";
                    confirmText = "View Log?";
                    debug = true;
                    break;

                case ProgressStatus.Failed:
                    title = "Oops..";
                    message = "Write failed.";
                    confirmText = "View Log?";
                    debug = true;
                    break;

                case ProgressStatus.Success:
                    (title, message) = ("Yay!", "Write complete.");
                    break;
            }
            
            Portal1?.Fade(Pad.Center, new FadePad(20, 1, Color.Black));
            WriteEnabled = false;
            CameToWrite = false;

            var shouldDebug = await Alert.SendAlert(title, message, cancelText, confirmText, debug);    
            if (shouldDebug)
            {
                await Shell.Current.ShowPopupAsync(new DebugPopup(ToDebug));
            }
            return taskStatus;
        }

        private async Task<ProgressStatus> BeginWrite(ToyTag tagType, CancellationToken token)
        {    
            WriteEnabled = false;
            token.ThrowIfCancellationRequested();

            if (Portal1 is null || !IsConnected)
            {
                ToDebug.AppendLine("Portal is either null or not connected");
                return ProgressStatus.NoConnection;            
            }

            Tagged = "Place empty card on center pad.";
            Portal1.Fade(Pad.Center, new FadePad(50, 1, Color.Cyan));
            CameToWrite = true;

            while (!WriteEnabled)
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(500, token);
            }

            if (CenterTag == null || CenterTag == null)
            {
                await Alert.SendAlert("Oops...", "Failed to read tag on center pad.", "Ok.", "", false);
                return ProgressStatus.TimedOut;
            }
            ProgressStatus status = new();
            var pageBytes = Read4Pages(CenterTag, 0x24);
            if (!IsEmptyCard(pageBytes))
            {
                var confirm = await Alert.SendAlert(" Alert! ", " Card may not be empty. Proceed with writing data? ", " Cancel?", " Write? ", true);
                if (confirm)
                { status = await WriteToCard(CenterTag, tagType, token); }

                ToDebug.AppendLine(BitConverter.ToString(pageBytes));
            }
            else { status = await WriteToCard(CenterTag, tagType, token); }
 
            return status;
        }
#endif
#if ANDROID || WINDOWS
        private async Task<ProgressStatus> WriteToCard(LegoTagEventArgs centerTag, ToyTag thisToy, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            ProgressStatus status = new();
            if (Portal1 is null || !IsConnected)
            {
                ToDebug.AppendLine("WriteToCard: Portal null or not connected");
                return ProgressStatus.NoConnection;
            }

            var Uid = centerTag.CardUid;
            var index = centerTag.Index;
            var auth = LegoTagTools.GenerateCardPassword(Uid);

            switch (thisToy.ToyTagType)
            {
                case ToyTagType.Character:
                    {
                        var car = LegoTagTools.EncrypCharactertId(Uid, thisToy.Id);
                        Portal1.SetTagPassword(PortalPassword.Disable, index);

                        bool success1 = Portal1.WriteTag(index, 0x24, car.AsSpan().Slice(0, 4).ToArray());
                        if (!success1)
                        {
                            //Failed to write;
                            ToDebug.AppendLine("Failed to write Character on 0x24");
                            return ProgressStatus.Failed;
                        }

                        bool success2 = Portal1.WriteTag(index, 0x25, car.AsSpan().Slice(4, 4).ToArray());
                        if (!success2)
                        {
                            //Failed to write;
                            ToDebug.AppendLine("Failed to write Character on 0x25");
                            return ProgressStatus.Failed;
                        }
                      
                        byte[] Data = [0x00, 0x00, 0x00, 0x00];
                        var success3 = Portal1.WriteTag(index, 0x26, Data);

                        if (!success3)
                        {
                            //Failed to write;
                            ToDebug.AppendLine("Failed to write 0s on 0x26");
                            return ProgressStatus.Failed;
                        }

                        var success4 = Portal1.WriteTag(index, (byte)0x2b, auth);

                        if (success1 && success2 && success3)
                        { status = ProgressStatus.Success; } 

                        // "Automatic password again."
                        Portal1.SetTagPassword(PortalPassword.Automatic, index);
                        break;
                    }

                case ToyTagType.Vehicle:
                    {
                        var vec = LegoTagTools.EncryptVehicleId(thisToy.Id);
                        Portal1.SetTagPassword(PortalPassword.Disable, index, auth);

                        bool success1 = Portal1.WriteTag(index, 0x24, vec);
                        if (!success1)
                        {
                            //failed to write
                            ToDebug.AppendLine("Failed to write Vehicle on 0x24");
                            return ProgressStatus.Failed;
                        }

                        byte[] Data = [0x00, 0x00, 0x00, 0x00];
                        var success2 = Portal1.WriteTag(index, 0x25, Data);

                        if (!success2)
                        {
                            //failed to write
                            ToDebug.AppendLine("Failed to write 0s on 0x25");
                            return ProgressStatus.Failed;
                        }

                        Data = [0x00, 0x01, 0x00, 0x00];
                        var success3 = Portal1.WriteTag(index, 0x26, Data);

                        if (!success3)
                        {
                            //failed to write
                            ToDebug.AppendLine("Failed to write Vehicle on 0x26");
                            return ProgressStatus.Failed;
                        }

                        var success4 = Portal1.WriteTag(index, (byte)0x2b, auth);

                        if (success1 && success2 && success3)
                        { status = ProgressStatus.Success; }

                        Portal1.SetTagPassword(PortalPassword.Custom, index, auth);
                        break;
                    }             
            }        
            return await Task.FromResult(status);
        }
#endif
#if ANDROID || WINDOWS
        /// <summary>
        /// Reads 4 pages of a tag.
        /// </summary>
        /// <param name="thisTag"> Tag to be read.</param>
        /// <param name="page">Starting page to read.</param>
        /// <returns>Byte[] from pages, or empty array if failed.</returns>
        public byte[] Read4Pages(LegoTagEventArgs thisTag, byte page)
        {
            if (Portal1 == null && thisTag == null)
            { return []; }

            var idx = thisTag.Index;
            byte[] bytes = Portal1!.ReadTag(idx, page);

            return bytes;
        }
#endif
        /// <summary>
        /// Checks if the data is empty for a given 16 blocks.
        /// </summary>
        /// <param name="data">16 bytes of the NFC data read from card.</param>
        /// <returns>True if the tag is empty.</returns>
        public static bool IsEmptyCard(byte[] data)
        {
            var result = data.SequenceEqual(new byte[] { 0x00, 0x00, 0x00, 0x00,
                                                   0x00, 0x00, 0x00, 0x00,
                                                   0x00, 0x00, 0x00, 0x00,
                                                   0x00, 0x00, 0x00, 0x00 });
            return result;
        }

        public string ThisTagInfo(LegoTagEventArgs legoTag)
        {
            if (legoTag == null) { return ""; }
            if (legoTag.LegoTag?.GetType().Name == "Character")
            { 
                Character? character = Character.Characters.FirstOrDefault(x => x.Id.Equals(legoTag.LegoTag.Id));
                if (character == null) { return ""; }        
                   return (character.Images);
            }
            else if (legoTag.LegoTag?.GetType().Name == "Vehicle")
            { 
                Vehicle? vehicle = Vehicle.Vehicles.FirstOrDefault(x => x.Id.Equals(legoTag.LegoTag.Id));
                if (vehicle == null) { return ""; }
                return (vehicle.Images);
            }

            else { return ""; }
        }

        public object? GetItem(LegoTagEventArgs legoTag)
        {     
            if(legoTag is null) { return null; }
            if (legoTag.LegoTag?.GetType().Name == "Character")
            {
                Character? character = Character.Characters.FirstOrDefault(x => x.Id.Equals(legoTag.LegoTag.Id));
                if (character == null) { return null; }
                return (character);
            }
            else if (legoTag.LegoTag?.GetType().Name == "Vehicle")
            {
                Vehicle? vehicle = Vehicle.Vehicles.FirstOrDefault(x => x.Id.Equals(legoTag.LegoTag.Id));
                if (vehicle == null) { return null; }
                return (vehicle);
            }
            else { return null; }        
        }

        private void GetNewPortal()
        {
#if ANDROID                 
            var portal = new LegoPortal();
            if (portal == null || !portal.IsConnected) 
            {
                Application.Current!.Dispatcher.Dispatch(SendToAlert);              
                return; 
            }
            Portal1 = portal;            
            IsConnected = portal.IsConnected;

            TestFade();
            portal.LegoTagEvent += Portal_LegoTagEvent;
#endif
        }

        public void Dispose()
        {
#if ANDROID || WINDOWS

            if (Portal1 == null || !Portal1.IsConnected) return;
            Portal1.Dispose();        
            Thread.Sleep(200);
            NewPortalLink?.Join();
#endif         
        }

        void SendToAlert() => AlertMe("Oops...", "Portal not detected. Please connect lego portal.");
        public Task<object?> AlertMe(string title, string message) => Shell.Current.ShowPopupAsync(new AlertPopup(title, message, "Ok.", "", false));
    }
}
    