using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using SoundsOrganizer.Models.Presentation;

namespace SoundsOrganizer.ViewModels.Presentation
{
    public class VisualisationViewModel
    {
        public Canvas MainCanvas { get; set; }

        private double _height;
        private double _width;
        private int _renderPosition;
        private double _yTranslate = 40;
        private double _yScale = 40;
        private const int BlankZone = 10;
        private readonly Polyline _topLine = new Polyline();
        private readonly Polyline _bottomLine = new Polyline();
        
        public VisualisationViewModel(VisualisationModel mainModel)
        {
            CreateMainCanvas(mainModel);
        }

        private void CreateMainCanvas(VisualisationModel mainModel)
        {
            MainCanvas = mainModel.MainCanvas;
            MainCanvas.SizeChanged += OnSizeChanged;
            _topLine.Stroke = new SolidColorBrush(Colors.DarkBlue);
            _bottomLine.Stroke = new SolidColorBrush(Colors.DarkBlue);
            _topLine.StrokeThickness = 1;
            _bottomLine.StrokeThickness = 1;
            MainCanvas.Children.Add(_topLine);
            MainCanvas.Children.Add(_bottomLine);
        }

        public void Reset()
        {
            _renderPosition = 0;
            ClearAllPoints();
        }

        public bool AnythingDrawn()
        {
            return _topLine.Points.Count > 0 || _bottomLine.Points.Count > 0;;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _renderPosition = 0;
            ClearAllPoints();

            _height = MainCanvas.ActualHeight;
            _width = MainCanvas.ActualWidth;

            _yTranslate = _height / 2;
            _yScale = _height / 2;
        }

        public void AddValue(float maxValue, float minValue)
        {
            var pixelWidth = (int)_width;
            if (pixelWidth <= 0)
                return;
            CreatePoint(maxValue, minValue);

            if (_renderPosition > _width)
                _renderPosition = 0;

            var erasePosition = (_renderPosition + BlankZone) % pixelWidth;
            if (erasePosition >= _topLine.Points.Count)
                return;

            var yPos = SampleToYPosition(0);
            _topLine.Points[erasePosition] = new Point(erasePosition, yPos);
            _bottomLine.Points[erasePosition] = new Point(erasePosition, yPos);
        }

        private void CreatePoint(float topValue, float bottomValue)
        {
            var topLinePos = SampleToYPosition(topValue);
            var bottomLinePos = SampleToYPosition(bottomValue);
            if (_renderPosition >= _topLine.Points.Count)
            {
                _topLine.Points.Add(new Point(_renderPosition, topLinePos));
                _bottomLine.Points.Add(new Point(_renderPosition, bottomLinePos));
            }
            else
            {
                _topLine.Points[_renderPosition] = new Point(_renderPosition, topLinePos);
                _bottomLine.Points[_renderPosition] = new Point(_renderPosition, bottomLinePos);
            }
            _renderPosition++;
        }

        private double SampleToYPosition(float value)
        {
            return _yTranslate + value * _yScale;
        }

        private void ClearAllPoints()
        {
            _topLine.Points.Clear();
            _bottomLine.Points.Clear();
        }
    }
}
