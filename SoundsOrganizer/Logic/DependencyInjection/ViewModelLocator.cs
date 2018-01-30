using Autofac;
using SoundsOrganizer.Logic.Audio.Recording;
using SoundsOrganizer.Models.Presentation;
using SoundsOrganizer.Models.Rating;
using SoundsOrganizer.Models.Recording;
using SoundsOrganizer.Models.UI;
using SoundsOrganizer.ViewModels.Presentation;
using SoundsOrganizer.ViewModels.Rating;
using SoundsOrganizer.ViewModels.Recording;
using SoundsOrganizer.ViewModels.UI;

namespace SoundsOrganizer.Logic.DependencyInjection
{
    public class ViewModelLocator
    {
        private readonly IContainer _container;

        public ViewModelLocator()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<UsbDeviceDetector>().AsSelf();
            builder.RegisterType<WasapiCaptureModel>().AsSelf().SingleInstance();
            builder.RegisterType<RecordingsModel>().AsSelf().SingleInstance();
            builder.RegisterType<VisualisationModel>().AsSelf().SingleInstance();
            builder.RegisterType<RatingModel>().AsSelf().SingleInstance();
            builder.RegisterType<AudioPlayerModel>().AsSelf();
            builder.Register(c => new RatingControllerViewModel(c.Resolve<RatingModel>(), c.Resolve<RecordingsModel>())).AsSelf();
            builder.Register(c => new RatingViewModel(c.Resolve<RatingModel>())).AsSelf();
            builder.Register(c=> new RecordingsViewModel(c.Resolve<RecordingsModel>())).AsSelf();
            builder.Register(c => new WasapiCaptureViewModel(c.Resolve<WasapiCaptureModel>(), c.Resolve<RecordingsModel>(), c.Resolve<UsbDeviceDetector>())).AsSelf();
            builder.Register(c => new VisualisationViewModel(c.Resolve<VisualisationModel>())).AsSelf().SingleInstance();
            builder.Register(c => new AudioPlayerViewModel(c.Resolve<VisualisationViewModel>(), c.Resolve<RecordingsModel>(), c.Resolve<AudioPlayerModel>(), c.Resolve<RatingModel>())).AsSelf();

            _container = builder.Build();
        }

        public RatingViewModel RatingViewModel => _container.Resolve<RatingViewModel>();

        public RatingControllerViewModel RatingControllerViewModel => _container.Resolve<RatingControllerViewModel>();

        public RecordingsViewModel RecordingsViewModel => _container.Resolve<RecordingsViewModel>();

        public WasapiCaptureViewModel WasapiCaptureViewModel => _container.Resolve<WasapiCaptureViewModel>();

        public VisualisationViewModel VisualisationViewModel => _container.Resolve<VisualisationViewModel>();

        public AudioPlayerViewModel AudioPlayerViewModel => _container.Resolve<AudioPlayerViewModel>();
    }
}
