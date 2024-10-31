using SkiaSharp.Views.Maui.Controls;
using SkiaSharp.Views.Maui;
using SkiaSharp;

namespace DimensionalTag
{
    [QueryProperty(nameof(WriteCharacter), nameof(WriteCharacter))]
    [QueryProperty(nameof(WriteVehicle), nameof(WriteVehicle))]

    public partial class PortalPage : ContentPage
    {

        public Character WriteCharacter
        {
            set => SendToWrite(value);
        }

        public Vehicle WriteVehicle
        {
            set => SendToWrite(value);
        }

        public Microsoft.Maui.Graphics.Color Picked
        {
            get { return Vm.PickedColor; }
            set { Vm.PickedColor = value; OnPropertyChanged(); }       
        }

        public bool CameHereToWrite
        {
            get { return Vm.CameToWrite; }
            set { Vm.CameToWrite = value; OnPropertyChanged(); }   
        }

  
        public PortalViewModel Vm;
        public PortalPage(PortalViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            Vm = vm;

            this.Loaded += Page_Loaded;
        }

        private void Page_Loaded(object? sender, EventArgs e)
        {
            this.Loaded -= Page_Loaded;
            sfx.Volume = Vm.CheckValue("Sfx", sfx.Volume);          
        }

        public void SKCanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            var canvasWidth = info.Width; // Android = 300 windows = 100
            var canvasHeight = info.Height;// same as above

            canvas.Clear();
            using (SKPaint paint = new SKPaint())
            {
                paint.IsAntialias = true;

                //Define the color array
                SKColor[] colors = new SKColor[8];

                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = SKColor.FromHsl(i * 360f / 7, 100, 50);
                }

                SKPoint center = new SKPoint(info.Rect.MidX, info.Rect.MidY); //Center position changes on platform change.

                //Create sweep gradient based on center of canvas
                paint.Shader = SKShader.CreateSweepGradient(center, colors, null);

                //Draw a circle with a wide line
                int strokeWidth = (Math.Min(info.Width, info.Height)) / 2; //set as rect dia to adjust for platform
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeWidth = strokeWidth;
                float radius = (Math.Min(info.Width, info.Height) - strokeWidth) / 2;

                canvas.DrawCircle(center, radius, paint);

                //convert the above to a bitmap to read pixel info
                SKColor touchPointColor;

                using (SKBitmap bitmap = new SKBitmap(info))
                {
                    //pixel buffer
                    IntPtr dstpixels = bitmap.GetPixels();

                    //read surface
                    surface.ReadPixels(info, dstpixels, info.RowBytes,
                        (int)lastTouchPoint.X, (int)lastTouchPoint.Y);

                    // grab our color.
                    touchPointColor = bitmap.GetPixel(0, 0);

                    //paint the touch point
                    using (SKPaint paintTouchPoint = new SKPaint())
                    {
                        paintTouchPoint.Style = SKPaintStyle.Fill;
                        paintTouchPoint.Color = SKColors.White;
                        paintTouchPoint.IsAntialias = true;

                        //outer ring
                        var outerRingRadius = ((float)canvasWidth / (float)canvasHeight) * (float)14;
                        canvas.DrawCircle(lastTouchPoint.X, lastTouchPoint.Y,
                                           outerRingRadius, paintTouchPoint);

                        //draw another circle with picked color
                        paintTouchPoint.Color = touchPointColor;

                        //inner ring
                        var innerRingRadius = ((float)canvasWidth / (float)canvasHeight) * (float)12;
                        canvas.DrawCircle(lastTouchPoint.X, lastTouchPoint.Y,
                                            innerRingRadius, paintTouchPoint);

                        //set selected color
                        Picked = touchPointColor.ToMauiColor();

                    }
                }

            }

        }

        SKPoint lastTouchPoint;
        private void canvasView_Touch(object sender, SKTouchEventArgs e)
        {
            //Get location from touch event
            if (sender is SKCanvasView canvasView)
            {
                lastTouchPoint = e.Location;
                var canvasSize = canvasView.CanvasSize;
                if ((e.Location.X > 0 && e.Location.X < canvasSize.Width) &&
                    (e.Location.Y > 0 && e.Location.Y < canvasSize.Height))
                {
                    e.Handled = true;
                    canvasView.InvalidateSurface();
                }

                //these are for other tests. Can remove
                var id = e.Id;
                var a = e.ActionType;
                var contact = e.InContact;
            }
        }

        public async void SendToWrite(object item)
        {
#if ANDROID || WINDOWS
           bool result = await Vm.BeginWrite(item);
#endif
        }

        private void OnGoodbye(object sender, NavigatedFromEventArgs e)
        {
            CameHereToWrite = false;
        }
    }
}
