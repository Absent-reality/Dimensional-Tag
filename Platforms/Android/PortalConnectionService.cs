using Android.App;
using Android.Content;
using Android.Hardware.Usb;

namespace DimensionalTag
{

    public partial class PortalConnectionService
    {
        // Constants
        private const int ProductId = 0x0241;
        private const int VendorId = 0x0E6F;
        private const int ReadWriteTimeout = 1000;

        // Class variables
        private UsbDevice? _portal;
        private UsbManager? _manager;
        private UsbDeviceConnection? _connection;
        private UsbInterface? _interface;
        private UsbEndpoint? _readEndpoint;
        private UsbEndpoint? _writeEndpoint;

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

            return portal;
        }

        /// <summary>
        /// Gets all the available USB device that matches the Lego Dimensions Portal.
        /// </summary>
        /// <returns>Array of USB devices.</returns>
        public static (UsbManager? usbManager, UsbDevice? device) GetPortal()
        {
            Activity? act = Platform.CurrentActivity;
            var manager = (UsbManager?)act.GetSystemService(Context.UsbService);
            //Get a list of all connected devices
            IDictionary<string, UsbDevice> devicesDirectory = manager.DeviceList;
            if (devicesDirectory == null || devicesDirectory.Count < 1)
            { return (null, null); }
            //Find the device by vendor and pid in the list

            UsbDevice portal = devicesDirectory.FirstOrDefault().Value;

            if (portal.ProductId != ProductId && portal.VendorId != VendorId)
            {
                throw new Exception("Please connect portal");
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
           var returned = _connection.BulkTransfer(_writeEndpoint, bytes, 32, ReadWriteTimeout);
            return returned;
        }

        public int GetIt(byte[] readBuffer)
        {
          var returned = _connection.BulkTransfer(_readEndpoint, readBuffer, 32, ReadWriteTimeout);
            return returned;

        }

        public bool CloseIt()
        {
            _connection.ReleaseInterface(_interface);
            _connection.Close();
            _connection.Dispose();
            if (_connection == null) { return false; }
            return true;
        }

    }

}
