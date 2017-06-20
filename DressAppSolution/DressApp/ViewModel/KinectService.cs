using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using DressApp.Model;
using System.Windows.Media.Media3D;

namespace DressApp.ViewModel
{
    public class KinectService : ViewModelBase
    {
        #region Atributos Privados
        
        // Captura de esqueletos
        private Skeleton[] _skeletons;
        
        //kinect default
        private KinectSensor _kinectSensor;

        // WritableBitmap se escribe el origen de la camara 
        private WriteableBitmap _kinectCameraImage;

        //limites de camara
        private Int32Rect _cameraSourceBounds;

        // numero de bytes por linea
        private int _colorStride;

        // Mano del Usuario
        private HandTracking _hand;

#if DEBUG
        // The skeleton manager
        private SkeletonManager _skeletonManager;
#endif

        // vista de ErrorGrid 
        private Visibility _errorGridVisibility;
        
        // vista de ClothesArea 
        private Visibility _clothesAreaVisibility;
        
        //ancho de la imagen
        private double _imageWidth;
        
        //Alto de la imagen
        private double _imageHeight;

        //mensaje de error
        private string _errorGridMessage;
        #endregion


        #region atributos publicos
        //kinect default
        public KinectSensor Kinect
        {
            get { return _kinectSensor; }
            set
            {
                if (_kinectSensor != null)
                {
                    UninitializeKinectSensor(_kinectSensor);
                    _kinectSensor = null;
                }
                if (value != null && value.Status == KinectStatus.Connected)
                {
                    _kinectSensor = value;
                    InitializeKinectSensor(_kinectSensor);
                }
            }
        }
        
        /// Obtiene (Gets) o establece (sets) la imagen de la cámara del Kinect.
        public WriteableBitmap KinectCameraImage
        {
            get { return _kinectCameraImage; }
            set
            {
                if (Equals(_kinectCameraImage, value))
                    return;
                _kinectCameraImage = value;
                OnPropertyChanged("KinectCameraImage");
            }
        }
        //Gets o sets de la mano
        public HandTracking Hand
        {
            get { return _hand; }
            set
            {
                if (_hand == value)
                    return;
                _hand = value;
                OnPropertyChanged("Hand");
            }
        }

#if DEBUG
        // Gets o sets de skeleton manager.
        public SkeletonManager SkeletonManager
        {
            get { return _skeletonManager; }
            set
            {
                if (_skeletonManager == value)
                    return;
                _skeletonManager = value;
                OnPropertyChanged("SkeletonManager");
            }
        }
#endif


        // Gets o sets de ancho.
        public double Width
        {
            get { return _imageWidth; }
            set
            {
                if (_imageWidth == value)
                    return;
                _imageWidth = value;
                OnPropertyChanged("Width");
            }
        }

        /// Gets o sets de Alto.
        public double Height
        {
            get { return _imageHeight; }
            set
            {
                if (_imageHeight == value)
                    return;
                _imageHeight = value;
                OnPropertyChanged("Height");
            }
        }
        
        /// Gets o sets de ClothesArea
        public Visibility ClothesAreaVisibility
        {
            get { return _clothesAreaVisibility; }
            set
            {
                if (_clothesAreaVisibility == value)
                    return;
                _clothesAreaVisibility = value;
                OnPropertyChanged("ClothesAreaVisibility");
            }
        }

        /// Gets o sets de ErrorGrid
        
        public Visibility ErrorGridVisibility
        {
            get { return _errorGridVisibility; }
            set
            {
                if (_errorGridVisibility == value)
                    return;
                _errorGridVisibility = value;
                OnPropertyChanged("ErrorGridVisibility");
            }
        }
        
        /// Gets o sets grid del mensaje de error
        public string ErrorGridMessage
        {
            get { return _errorGridMessage; }
            set
            {
                if (_errorGridMessage == value)
                    return;
                _errorGridMessage = value;
                OnPropertyChanged("ErrorGridMessage");
            }
        }
        #endregion


        #region Metodos Privados

        // Habilita ColorStream de KinectSensor recién detectado y establece la imagen de salida
        private void InitializeKinectSensor(KinectSensor sensor)
        {
            if (sensor != null)
            {
                ColorImageStream colorStream = sensor.ColorStream;
                colorStream.Enable();

                KinectCameraImage = new WriteableBitmap(colorStream.FrameWidth, colorStream.FrameHeight
                    , 96, 96, PixelFormats.Bgr32, null);

                _cameraSourceBounds = new Int32Rect(0, 0, colorStream.FrameWidth, colorStream.FrameHeight);
                _colorStride = colorStream.FrameWidth * colorStream.FrameBytesPerPixel;
                sensor.ColorFrameReady += KinectSensor_ColorFrameReady; ///METODOS DE KINECT

                sensor.SkeletonStream.AppChoosesSkeletons = false;
                sensor.SkeletonStream.Enable();
                _skeletons = new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];
                sensor.SkeletonFrameReady += KinectSensor_SkeletonFrameReady;
                try
                {
                    sensor.Start();
                }
                catch (Exception)
                {
                    UninitializeKinectSensor(sensor);
                    Kinect = null;
                    ErrorGridVisibility = Visibility.Visible;
                    ErrorGridMessage = "Kinect está siendo utilizado por otro proceso. " + Environment.NewLine +
                        "Trata de desconectar y volver a conectar el dispositivo al ordenador." + Environment.NewLine +
                        "Asegúrese de que todos los programas que utilizan Kinect se han desactivado.";
                }
            }
        }

        // Deshabilita ColorStream de KinectSensor desconectado
        private void UninitializeKinectSensor(KinectSensor sensor)
        {
            if (sensor == null) return;
            sensor.Stop();
            sensor.ColorFrameReady -= KinectSensor_ColorFrameReady;
            sensor.SkeletonFrameReady -= KinectSensor_SkeletonFrameReady;
            sensor.SkeletonStream.Disable();
            _skeletons = null;
        }

        // Metodo para obtener la camara de kinect
        private void KinectSensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null) return;
                var pixels = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(pixels);

                KinectCameraImage.WritePixels(_cameraSourceBounds, pixels, _colorStride, 0);
                OnPropertyChanged("KinectCameraImage");
            }
        }

        private void KinectSensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (var frame = e.OpenSkeletonFrame())
            {
                if (frame == null || frame.SkeletonArrayLength == 0)
                    return;
                frame.CopySkeletonDataTo(_skeletons);

                var skeleton = GetPrimarySkeleton(_skeletons);
                if (skeleton == null)
                {
                    ErrorGridVisibility = Visibility.Visible;
                    ErrorGridMessage = "No detectado esqueleto pierde o su posición." + Environment.NewLine +
                        "Espera un minuto y compruebe si está de pie a una distancia apropiada del dispositivo.";
                    ClothesAreaVisibility = Visibility.Hidden;
                    
                    return;
                }
                if (ClothesAreaVisibility == Visibility.Hidden)
                {
                    ErrorGridVisibility = Visibility.Collapsed;
                    ClothesAreaVisibility = Visibility.Visible;
                }
                Hand.UpdateHandCursor(skeleton, Kinect, Width, Height);
                ClothingManager.Instance.UpdateItemPosition(skeleton, Kinect, Width, Height);
#if DEBUG
                Brush brush = Brushes.Coral;
                SkeletonManager.DrawSkeleton(_skeletons, brush, _kinectSensor, Width, Height);
#endif

            }
        }

        //Se suscribe al evento StatusChanged e inicializa KinectSensor
        private void DiscoverKinectSensors()
        {
            KinectSensor.KinectSensors.StatusChanged += KinectSensor_StatusChanged;
            Kinect = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
            if (Kinect == null)
            {
                ErrorGridVisibility = Visibility.Visible;
                ErrorGridMessage = "Conectar Kinect";
            }
        }
        // actualiza si el kinect ha cambiado
        private void KinectSensor_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Initializing:
                    ErrorGridVisibility = Visibility.Visible;
                    ErrorGridMessage = "Inicializando Kinect...";
                    break;
                case KinectStatus.Connected:
                    ErrorGridVisibility = Visibility.Hidden;
                    if (Kinect == null)
                        Kinect = e.Sensor;
                    break;
                case KinectStatus.Disconnected:
                    if (Kinect == e.Sensor)
                    {
                        Kinect = null;
                        Kinect = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
                        if (Kinect == null)
                        {
                            ErrorGridVisibility = Visibility.Visible;
                            ErrorGridMessage = "conecte a su equipo el Kinect.";
                        }
                    }
                    break;
                case KinectStatus.NotPowered:
                    ErrorGridVisibility = Visibility.Visible;
                    ErrorGridMessage = "Enchufe el cable de alimentación en la toma de corriente";
                    break;
                default:
                    ErrorGridVisibility = Visibility.Visible;
                    ErrorGridMessage = "Kinect no se puede iniciar. Espera un minuto, o reiniciar el programa.";
                    break;
            }
        }

        #endregion

        #region Metodos Publicos
        //Inicializa Instancias
        public void Initialize()
        {
            Hand = new HandTracking();
#if DEBUG
            SkeletonManager = new SkeletonManager();
#endif
            ErrorGridVisibility = Visibility.Hidden;
            ClothesAreaVisibility = Visibility.Visible;
            DiscoverKinectSensors();
        }

        // Busca el esqueleto más cercano
        public static Skeleton GetPrimarySkeleton(Skeleton[] skeletons)
        {
            Skeleton skeleton = null;

            if (skeletons != null)
                foreach (Skeleton skelet in skeletons.Where(s => s.TrackingState == SkeletonTrackingState.Tracked))
                    if (skeleton == null || skelet.Position.Z < skeleton.Position.Z)
                        skeleton = skelet;

            return skeleton;
        }
       

        //Muestra en el espacio de kinect de canvas
        public static Point3D GetJointPoint(Joint joint, KinectSensor sensor, double width, double height)
        {
            var point = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(joint.Position, sensor.DepthStream.Format);

            return new Point3D(point.X * (width / sensor.DepthStream.FrameWidth)
                , point.Y * (height / sensor.DepthStream.FrameHeight), point.Depth);
        }

        //Calucular la distancia entre dos puntos
        public static Point CalculateDistanceBetweenJoints(Point joint1, Point joint2)
        {
            return new Point(Math.Abs(joint1.X - joint2.X), Math.Abs(joint1.Y - joint2.Y));
        }
        //limpiar instancias
        public void Cleanup()
        {
            Kinect = null;
        }
        #endregion

    }
}
