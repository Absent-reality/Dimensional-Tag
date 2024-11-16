using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using DimensionalTag.Tools;

#if ANDROID
using Android.Nfc;
using Android.Content;
#endif

namespace DimensionalTag
{
    [QueryProperty(nameof(WriteCharacter), nameof(WriteCharacter))]
    [QueryProperty(nameof(WriteVehicle), nameof(WriteVehicle))]
  
    public partial class ScanPage : ContentPage
    {
        
        public Character WriteCharacter
        {
            set => SendToWrite(value);
        }

        public Vehicle WriteVehicle
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

        public ScanViewModel Vm { get; set; }
        public ScanPage(ScanViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            Vm = vm;

            this.Loaded += ScanPage_Loaded;
#if ANDROID
            var manager = Android.App.Application.Context.GetSystemService(Context.NfcService) as NfcManager;
            var adapter = manager!.DefaultAdapter;
            Adapter = adapter;

            CardToolsGetter.SetOnCardReceive(async (cardInfo) =>
            {              
                if (cardInfo != null)
                {
                   Vm.LoadTo(cardInfo);                   
                }             
            });
#endif          
        }

        private void ScanPage_Loaded(object? sender, EventArgs e)
        {
            this.Loaded -= ScanPage_Loaded;
            sfx.Volume = Vm.CheckValue("Sfx", sfx.Volume);
        }

        private async void SendToWrite(object item)
        {

            sfx.Source = MediaSource.FromResource("lego_pieces.mp3");
                                
            switch (item)
            {
                case Character:

                    Character c = (Character)item;
                    if (c == null || c.Name == "") { return; }
                  
                    SwapBg(CameToWrite = true);
                    bool result1 = await Vm.BeginWrite(c);
                    sfx.Play();
                    break;

                case Vehicle:

                    Vehicle v = (Vehicle)item;
                    if (v == null || v.Name == "") { return; }

                    SwapBg(CameToWrite = true);
                    bool result2 = await Vm.BeginWrite(v);
                    sfx.Play();
                    break;
            }

            WriteCharacter = new Character(0, "", "", "", []);
            WriteVehicle = new Vehicle(0, 0, "", "", "", []);
            
            SwapBg(CameToWrite = false);
        }

        private async void On_Arrived(object sender, NavigatedToEventArgs e)
        {        
            await Task.Delay(600);
            SwapBg(CameToWrite);
#if ANDROID
            if (Adapter != null && !Adapter.IsEnabled)
            {
                await Shell.Current.ShowPopupAsync(new AlertPopup("Oops.", "Nfc is not enabled. Please enable from your device settings.", "ok", "", false));
            }
#endif

        }

        private async void On_Goodbye(object sender, NavigatedFromEventArgs e)
        {
            WriteCharacter = new Character(0, "", "", "", []);
            WriteVehicle = new Vehicle(0, 0, "", "", "", []);
            await Lbl_scan.FadeTo(0,250);
            img_write.IsVisible = false;
         
            SwapBg(CameToWrite = false);

#if ANDROID
            CardToolsGetter.WriteCardCancel();          
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

