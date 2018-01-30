using System;
using System.IO;
using System.Reflection;
using SoundsOrganizer.Logic.PropertyHelpers;

namespace SoundsOrganizer.Models.Rating
{

    public class RatingModel : PropertyChangedBase
    {
        private double _loudness;
        private double _length;
        private double _pitch;
        private double _humidity;
        private int _breaksCount;
        private double _burpage;
        private double _epsilon;
        private string _currentRating;

        public RatingModel()
        {
            LoadDefaultData();
        }

        private void LoadDefaultData()
        {
            _epsilon = 0.0001;
            _loudness = _length = _pitch = _humidity = _burpage = 0.5;
            _breaksCount = 0;
            RatingsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Path.GetTempPath(), @"BurpDB\Ratings");
        }

        public double Loudness
        {
            get
            {
                return _loudness;
            }

            set
            {
                if (Math.Abs(_loudness - value) < _epsilon)
                    return;
                _loudness = value;
                OnPropertyChanged(nameof(Loudness));
            }
        }

        public double Length
        {
            get { return _length; }

            set
            {
                if (Math.Abs(_length - value) < _epsilon)
                    return;
                _length = value;
                OnPropertyChanged(nameof(Length));
            }
        }

        public double Pitch
        {
            get
            {
                return _pitch;
            }

            set
            {
                if (Math.Abs(_pitch - value) < _epsilon)
                    return;
                _pitch = value;
                OnPropertyChanged(nameof(Pitch));
            }
        }

        public double Humidity
        {
            get
            {
                return _humidity;
            }

            set
            {
                if (Math.Abs(_humidity - value) < _epsilon)
                    return;
                _humidity = value;
                OnPropertyChanged(nameof(Humidity));
            }
        }

        public int BreaksCount
        {
            get
            {
                return _breaksCount;
            }

            set
            {
                if (_breaksCount == value)
                    return;
                _breaksCount = value;
                OnPropertyChanged(nameof(BreaksCount));
            }
        }

        public double Burpage
        {
            get
            {
                return _burpage;
            }

            set
            {
                if (Math.Abs(_burpage - value) < _epsilon)
                    return;
                _burpage = value;
                OnPropertyChanged(nameof(Burpage));
            }
        }

        public string CurrentRating
        {
            get
            {
                return _currentRating;
            }

            set
            {
                if (_currentRating == value)
                    return;
                _currentRating = value;
                OnPropertyChanged(nameof(CurrentRating));
            }
        }

        public string RatingsFolder { get; private set; }
    }
}
