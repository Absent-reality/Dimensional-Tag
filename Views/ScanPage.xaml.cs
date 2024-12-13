
using CommunityToolkit.Maui.Views;

#if ANDROID
using Android.Nfc;
using Android.Content;
#endif

namespace DimensionalTag
{
    [QueryProperty(nameof(WriteTag), nameof(WriteTag))]

    public partial class ScanPage : ContentPage
    {    
        public ToyTag WriteTag
        {
            set => SendToWrite(value);
        }

        private bool cameToWrite = false;
        public bool CameToWrite
        {
            get => cameToWrite;
            set { if (cameToWrite == value) { return; }
                cameToWrite = value;
                OnPropertyChanged(nameof(CameToWrite));             
            }
        }

        public bool isNotBusy = true;
#if ANDROID
        public NfcAdapter? Adapter;
#endif
        public INfcTools NfcTools;
        public ScanViewModel Vm { get; set; }

        public ScanPage(ScanViewModel vm, INfcTools nfcTools)
        {           
            InitializeComponent();
            BindingContext = vm;
            Vm = vm;
            NfcTools = nfcTools;
            this.Loaded += ScanPage_Loaded;
            nfcTools.NfcTagEvent += NfcTools_NfcTagEvent;
        
#if ANDROID
            var manager = Android.App.Application.Context.GetSystemService(Context.NfcService) as NfcManager;
            var adapter = manager!.DefaultAdapter;
            Adapter = adapter;
#endif          
        }

        private void NfcTools_NfcTagEvent(object? sender, NfcTagEventArgs e)
        {
            ToyTag toy = new ToyTag(e.Id, e.Name, e.World, e.Abilities, e.ToyTagType);
            Vm.LoadTo(toy);
        }

        private void ScanPage_Loaded(object? sender, EventArgs e)
        {
            this.Loaded -= ScanPage_Loaded;
            sfx.Volume = Vm.CheckValue("Sfx", sfx.Volume);
        }

        private void SendToWrite(ToyTag item)
        {
            sfx.Source = MediaSource.FromResource("lego_pieces.mp3");
            if (item == null || item.ToyTagType == ToyTagType.None) { return; }  
            SwapBg(CameToWrite = true);
            Vm.BeginWrite(item);
            sfx.Play();

            WriteTag = new ToyTag(0, "", "", [], ToyTagType.None);          
            SwapBg(CameToWrite = false);
        }

        private async void On_Arrived(object sender, NavigatedToEventArgs e)
        {        
            await Task.Delay(600);
            SwapBg(CameToWrite);
#if ANDROID
            if (Adapter != null && !Adapter.IsEnabled)
            {
               await Vm.Alert.SendAlert("Oops.", "Nfc is not enabled. Please enable from your device settings.", "ok", "", false);
            }
#endif
        }

        private async void On_Goodbye(object sender, NavigatedFromEventArgs e)
        {
            WriteTag = new ToyTag(0, "", "", [], ToyTagType.None);
            await Lbl_scan.FadeTo(0,250);
            img_write.IsVisible = false;     
            SwapBg(CameToWrite = false);
#if ANDROID
            NfcTools.WriteCardCancel();          
#endif
        }

        private async void SwapBg(bool change)
        {
            if (!isNotBusy)
            { return; }

            switch (change)
            {
                case true:
                    {
                        isNotBusy = false;
                        Lbl_scan.Text = "Hold phone on empty tag to write.";
                        await Lbl_scan.FadeTo(1, 1000);
                        img_write.IsVisible = true;
                        await Task.Delay(200);
                        await img_scan.TranslateTo(0, -50, 500);
                        await Task.Delay(600);
                        await img_scan.TranslateTo(0, 0, 500);
                        await Task.Delay(500);
                        await img_scan.TranslateTo(0, -50, 500);
                        await Task.Delay(600);
                        await img_scan.TranslateTo(0, 0, 500);

                        isNotBusy = true;
                    }
                    break;

                case false:
                    {
                        isNotBusy = false;

                        img_write.IsVisible = false;
                        Lbl_scan.Text = "Place phone on tag to scan.";
                        await Lbl_scan.FadeTo(1, 1000);

                        for (int idc = 1; idc < 10; idc++)
                        {
                            img_scan.Source = "scan_one.png";
                            await Task.Delay(200);
                            img_scan.Source = "scan_two.png";
                            await Task.Delay(200);
                            img_scan.Source = "scan_three.png";
                            await Task.Delay(200);
                            img_scan.Source = "scan_four.png";
                            await Task.Delay(200);

                            isNotBusy = true;
                        }
                    }
                    break;
            }
        }
    }
}

