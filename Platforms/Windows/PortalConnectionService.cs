using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;

namespace DimensionalTag
{
    
    public partial class PortalConnectionService
    {
        
        // Constants
        private const int ProductId = 0x0241;
        private const int VendorId = 0x0E6F;
        private const int ReadWriteTimeout = 1000;

        // Class variables
        private IUsbDevice? _portal;
        private UsbEndpointReader? _endpointReader;
        private UsbEndpointWriter? _endpointWriter;
        private byte _messageId;

        // This needs to be static to keep the context otherwise, the app will close it
        private static UsbContext context = null;

        public partial object? GetConnection()
        {
            var portal = GetPortal();

           if (ReferenceEquals(portal, null))
            {
                return null;
            }
            _portal = portal;
          
            return _portal;
        }

        public static IUsbDevice? GetPortal()
        {
            context ??= new UsbContext();
#if DEBUG
            context.SetDebugLevel(LogLevel.Info);
#else
            context.SetDebugLevel(LogLevel.Error);
#endif
            //Get a list of all connected devices
            var usbDeviceCollection = context.List();

            //Narrow down the device by vendor and pid
            var selectedDevice = usbDeviceCollection.Where(d => d.ProductId == ProductId && d.VendorId == VendorId);           
            var device = selectedDevice.FirstOrDefault();

            if (device == null)  { return null; }

            return device;

        }

        public bool OpenIt()
        {   
            if (_portal == null) { return false; }

            _portal.Open();
             var check = _portal.ClaimInterface(_portal.Configs[0].Interfaces[0].Number);

            _endpointWriter = _portal.OpenEndpointWriter(WriteEndpointID.Ep01);
            _endpointReader = _portal.OpenEndpointReader(ReadEndpointID.Ep01);
          

            if (!check) { return false; }
            return true;
        }

        public int SendIt(byte[] bytes)
        {
      
            _endpointWriter.Write(bytes, ReadWriteTimeout, out var bytesRead);
            var returned = bytesRead;
            return returned;
        }

        public int GetIt(byte[] readBuffer)
        {
             int numBytes = 0;
            _endpointReader.Read(readBuffer, ReadWriteTimeout,out numBytes);
            if (numBytes == 33) { numBytes = 32; }
            var returned = numBytes;
            return returned;

        }

        public bool CloseIt()
        {
            if( _portal == null) { return false; }
            _portal.ReleaseInterface(0);
            _portal.Close();
            _portal = null;
            if (_portal == null) { return false; }
            return true;
        }

    }
} 
