using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DimensionalTag
{
    public partial class PopupViewModel(AppSettings settings, IAlert alert) : BaseViewModel(settings, alert)
    {
        public IAlert Alerts { get; set; } = alert;
        public AppSettings AppSettings { get; set; } = settings;

        [ObservableProperty]
        bool hereToWrite;

        [ObservableProperty]
        bool shouldClose = false;

        [ObservableProperty]
        string imgSource = "";

        [ObservableProperty]
        string form2 = "";

        [ObservableProperty]
        string form3 = "";

        [ObservableProperty]
        string name = "";

        [ObservableProperty]
        string world = "";

        [ObservableProperty]
        string abilities = "";

        [ObservableProperty]
        bool visible = false;
     
        public Popup? Popup { get; set; }

        public void LoadTo(object item)
        {
            switch (item)
            {
                case Character:
                    {
                        Character c = (Character)item;
                        if (c == null) { return; }

                        Name = c.Name;
                        ImgSource = c.Images;
                        World = $"\n World:\n {c.World}";
                        foreach (string a in c.Abilities)
                        {
                            Abilities += $"{a} \n";
                        }
                    }
                    break;

                case Vehicle:
                    {
                        Vehicle v = (Vehicle)item;
                        if (v == null) { return; }

                        var form2 = Vehicle.Vehicles.FirstOrDefault(x => x.Id == v.Id + 1);
                        var form3 = Vehicle.Vehicles.FirstOrDefault(x => x.Id == v.Id + 2);
                        if (form2 != null) { Form2 = form2.Name; }
                        if (form3 != null) { Form3 = form3.Name; }

                        Visible = true;
                        Name = v.Name;
                        ImgSource = v.Images;
                        World = $"\n World:\n {v.World}";

                        foreach (string a in v.Abilities)
                        {
                            Abilities += $"{a} \n";
                        }
                    }
                    break;

                case null:
                    break;
            }
        }

        [RelayCommand]
        async Task Tapped(string form)
        {
            ShouldClose = false;
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            var confirm = await Alert.SendAlert(" Alert! ", " Are you sure you want to write this data? ", " Cancel?", " Write? ", true);
            if (confirm)
            {
                Vehicle? current = null;
                switch (form)
                {
                    case "Form2":
                        current = Vehicle.Vehicles.FirstOrDefault(x => x.Name == Form2);
                        break;

                    case "Form3":
                        current = Vehicle.Vehicles.FirstOrDefault(y => y.Name == Form3);
                        break;
                }

                if (current == null) { return; }
                if (Popup != null)
                {
                    ToyTag toyTag = ToyTag.ConvertTo(current);
                    LetsWriteIt(toyTag);
                    Popup.Close();
                }
            
            }
        }      

    }
}
