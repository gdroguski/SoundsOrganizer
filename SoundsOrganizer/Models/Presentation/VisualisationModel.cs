using System.Windows.Controls;

namespace SoundsOrganizer.Models.Presentation
{
    public class VisualisationModel
    {
        public Canvas MainCanvas { get; }

        public VisualisationModel()
        {
            MainCanvas= new Canvas() ;
        }
    }
}
