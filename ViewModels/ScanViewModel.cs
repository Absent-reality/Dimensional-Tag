using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using DimensionalTag.Tools;

namespace DimensionalTag
{
    public partial class ScanViewModel : SettingsViewModel
    {

        public async void LoadTo(object obj)
        {
            switch (obj)
            {
                case Character:
                    {
                        Character c = (Character)obj;
                        Character? character = Character.Characters.FirstOrDefault(m => m.Id == c.Id);

                        if (character == null)
                        {
                            await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong with character.", "Ok.", "", false));
                        }
                        else
                        {
                            var navParam = new Dictionary<string, object> { { "CharacterParam", character } };
                            await Shell.Current.GoToAsync($"///CharacterPage", navParam);
                        }
                    }
                    break;

                case Vehicle:
                    {
                        Vehicle v = (Vehicle)obj;
                        Vehicle? vehicle = Vehicle.Vehicles.FirstOrDefault(m => m.Id == v.Id);

                        if (vehicle == null)
                        {
                            await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Something went wrong with vehicle.", "Ok.", "", false));
                        }
                        else
                        {
                            if (vehicle.Form == 1)
                            {
                                var navParam = new Dictionary<string, object> { { "VehicleParam", vehicle } };
                                await Shell.Current.GoToAsync($"///VehiclesPage", navParam);
                            }
                            else if (vehicle.Form == 2)
                            {
                                var veh = Vehicle.Vehicles.FirstOrDefault(v => v.Id == vehicle.Id - 1);
                                if (veh != null)
                                {
                                    var navParam = new Dictionary<string, object> { { "VehicleParam", veh } };
                                    await Shell.Current.GoToAsync($"///VehiclesPage", navParam);
                                }
                            }
                            else if (vehicle.Form == 3)
                            {
                                var V = Vehicle.Vehicles.FirstOrDefault(x => x.Id == vehicle.Id - 2);
                                if (V != null)
                                {
                                    var navParam = new Dictionary<string, object> { { "VehicleParam", V } };
                                    await Shell.Current.GoToAsync($"///VehiclesPage", navParam);
                                }
                            }
                        }
                    }
                    break;

                case null:
                    {
                        await Shell.Current.ShowPopupAsync(new AlertPopup("Oops...", "Failed to load data.", "Ok.", "", false));
                    }
                    break;
            }
        }

        public async Task<bool> BeginWrite(object item)
        {  
            bool complete = false;
            switch (item)
            {               
                case Character:
                    {
                        Character c = (Character)item;
                        complete = await CardToolsGetter.WriteCard("Character", c.Id);                        
                    }
                    return complete;

                case Vehicle:
                    {
                        Vehicle v = (Vehicle)item;
                        complete = await CardToolsGetter.WriteCard("Vehicle", v.Id);                         
                    }
                    return complete;

                case null:
                    {
                        //If navigating here from write, then to the opposite type (ie character to vehicle) 
                        // It throws null since the other info clears.
                        complete = false;
                    }
                    return complete;          
            }
            return complete;
        }
    }
}
