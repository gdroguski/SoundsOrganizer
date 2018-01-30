using System.ComponentModel;
using SoundsOrganizer.Logic.PropertyHelpers;
using SoundsOrganizer.Models.Rating;

namespace SoundsOrganizer.ViewModels.Rating
{
    public class RatingViewModel : PropertyChangedBase
    {
        private readonly RatingModel _ratingModel;

        public RatingViewModel(RatingModel ratingModel)
        {
            _ratingModel = ratingModel;
            _ratingModel.PropertyChanged += ModelPropertyChanged;
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        public double Loudness
        {
            get { return _ratingModel.Loudness; }
            set { _ratingModel.Loudness = value; }
        }

        public double Length
        {
            get { return _ratingModel.Length; }
            set { _ratingModel.Length = value; }
        }

        public double Pitch
        {
            get { return _ratingModel.Pitch; }
            set { _ratingModel.Pitch = value; }
        }

        public double Humidity
        {
            get { return _ratingModel.Humidity; }
            set { _ratingModel.Humidity = value; }
        }

        public int BreaksCount
        {
            get { return _ratingModel.BreaksCount; }
            set { _ratingModel.BreaksCount = value; }
        }

        public double Burpage
        {
            get { return _ratingModel.Burpage; }
            set { _ratingModel.Burpage = value; }
        }
    }
}
