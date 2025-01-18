using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Com.Google.Android.Exoplayer2;

namespace DimensionalTag
{

    public partial class PortalConnectionService
    {
        // Constants
        private const int Wii_Ps_Pid = 0x0241;
        private const int Wii_Ps_Vid = 0x0E6F;
        private const int Xb_Pid = 0xFA01;
        private const int Xb_Vid = 0x24C6;
        private const int ReadWriteTimeout = 1000;
        private const int MessageLength = 32;


        // Class variables
        private UsbDevice? _portal;
        private UsbManager? _manager;
        private UsbDeviceConnection? _connection;
        private UsbInterface? _interface;
        private UsbEndpoint? _readEndpoint;
        private UsbEndpoint? _writeEndpoint;
        private PortalType? _portalType;

        public partial object? GetConnection()
        {
            var portal = GetPortal();
            if (portal.usbManager == null || portal.device == null)
            {
                return null;
            }

            _portal = portal.device;
            _manager = portal.usbManager;
            _interface = portal.device.GetInterface(0);
            _writeEndpoint = _interface.GetEndpoint(1);
            _readEndpoint = _interface.GetEndpoint(0); 
            switch (portal.device.ProductId)
            {
                case Xb_Pid:
                    _portalType = PortalType.Xbox;
                    break;

                case Wii_Ps_Pid:
                    _portalType = PortalType.Wii_PS;
                    break;
            }
            
            return portal;
        }

        /// <summary>
        /// Gets all the available USB device that matches the Lego Dimensions Portal.
        /// </summary>
        /// <returns>Array of USB devices.</returns>
        public static (UsbManager? usbManager, UsbDevice? device) GetPortal()
        {
            Activity? act = Platform.CurrentActivity;
            var manager = (UsbManager?)act!.GetSystemService(Context.UsbService);
            if (manager == null) { return (null, null); }
            //Get a list of all connected devices                 
            IDictionary<string, UsbDevice> devicesDirectory = manager.DeviceList!;
            if (devicesDirectory == null || devicesDirectory.Count < 1)
            { return (null, null); }
            //Find the device by vendor and pid in the list

            UsbDevice portal = devicesDirectory.FirstOrDefault().Value;
            // ||64001 9414
            if (!(portal.ProductId == Xb_Pid && portal.VendorId == Xb_Vid) && !(portal.ProductId == Wii_Ps_Pid && portal.VendorId == Wii_Ps_Vid))
            {
               return (null, null);
            }          

            if (manager.HasPermission(portal) == false)
            {
                string ACTION_USB_PERMISSION = "rzepak";
                PendingIntent? mPermissionIntent = PendingIntent.GetBroadcast(act, 0, new Intent(ACTION_USB_PERMISSION), PendingIntentFlags.Immutable);
                IntentFilter filter = new IntentFilter(ACTION_USB_PERMISSION);
                manager.RequestPermission(portal, mPermissionIntent);
            }
            return (manager, portal);
        }

        public bool OpenIt()
        {
            if (_manager == null) { return false; }

            _connection = _manager.OpenDevice(_portal);

            if (_connection == null) { return false; }
            var check = _connection.ClaimInterface(_interface, true);

            if (!check) { return false; }
            return true;
        }

        public int SendIt(byte[] bytes)
        {
            byte[] message = [];
            switch (_portalType)
            {
                case PortalType.Xbox:
                   // byte[] prefix = { 0x0b, 0x16 };                  
                    message = [0x0b, 0x16, ..bytes.AsSpan(0, 30).ToArray()];                   
                    break;

                case PortalType.Wii_PS:
                    message = bytes;
                    break;

                case null:
                    return 0;
            }
            
            if (_connection == null) { return 0; }
            var returned = _connection.BulkTransfer(_writeEndpoint, message, MessageLength, ReadWriteTimeout);
            return returned;
        }

        public int GetIt(byte[] readBuffer)
        {         
            if (_connection == null) { return 0; }
            var returned = _connection.BulkTransfer(_readEndpoint, readBuffer, MessageLength, ReadWriteTimeout);

            return returned;
        }

        public bool CloseIt()
        {
            if (_connection == null) return false;
            _connection.ReleaseInterface(_interface);
            _connection.Close();
            _connection.Dispose();
            if (_connection == null) { return false; }
            return true;
        }
    }
}
