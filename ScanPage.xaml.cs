
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using DimensionalTag.Tools;

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

        public bool cameToWrite = false;
        public bool isNotBusy = true;

        public ScanViewModel Vm { get; set; }
        public ScanPage(ScanViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            Vm = vm;
            
            this.Loaded += ScanPage_Loaded;

#if ANDROID
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
            SwapBg(cameToWrite = true);
            sfx.Source = MediaSource.FromResource("lego_pieces.mp3");

            bool result = await Vm.BeginWrite(item);
           
            sfx.Play();               
            cameToWrite = false;
            SwapBg(cameToWrite);

        }

        private async void On_Arrived(object sender, NavigatedToEventArgs e)
        {        
                await Task.Delay(600);
                SwapBg(cameToWrite);           
        }

        private async void On_Goodbye(object sender, NavigatedFromEventArgs e)
        {
            await Lbl_scan.FadeTo(0,250);
            img_write.IsVisible = false;
            cameToWrite = false;
#if ANDROID
            CardToolsGetter.WriteCardCancel();          
#endif
        }

        private async void SwapBg(bool switchItUp)
        {
            if (!isNotBusy)
            {
                return;
            }
            switch (switchItUp)
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

