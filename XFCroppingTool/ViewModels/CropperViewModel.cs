using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using XFCroppingTool.Interfaces;

namespace XFCroppingTool
{
	public class CropperViewModel : INotifyPropertyChanged
	{
		private ImageSource imageSource;
		public ImageSource ImgSource
		{
			get
			{
				return this.imageSource;
			}
			set
			{
				this.imageSource = value;
				OnPropertyChanged();
			}
				}

		private ImageSource croppedImage;
		public ImageSource CroppedImage
		{ 
			get 
			{
				return this.croppedImage;
			}
			set
			{
				this.croppedImage = value;
				OnPropertyChanged();
			}
		}

		private Rectangle cropRect;

		public Rectangle CropRect
		{
			get
			{
				return cropRect;
			}
			set
			{
				cropRect = value;
				OnPropertyChanged();
			}
		}

		private double _cropperSize;
		public double CropperSize
		{
			get { return this._cropperSize; }
			set
			{
				this._cropperSize = value;
				OnPropertyChanged();
			}
		}

		private bool _isImageAvailable = false;
		public bool IsImageAvailable
		{
			get
			{
				return this._isImageAvailable;
			}
			set
			{
				this._isImageAvailable = value;
				OnPropertyChanged();
			}
				}

		private bool _isImagePresent;
		public bool IsImagePresent
		{
			get
			{
				return this._isImagePresent;
			}
			set
			{
				this._isImagePresent = value;
				OnPropertyChanged();
			}
				}

		private MediaFile photo;
		private bool crossMedia = false;


		public ICommand ImageFromGalleryCommand
		{
			get { return new Command(async () => await GetImageFromGallery()); }
		}

		public ICommand TakePictureCommand => new Command(async
														  () => await TakePicture());

		public ICommand CropImageCommand => new Command(async
														  () => await CropImage());

			private async Task GetImageFromGallery()
			{
				if (crossMedia == false)
				{
					crossMedia = true;

					await CrossMedia.Current.Initialize();

					photo = await CrossMedia.Current.PickPhotoAsync();
					crossMedia = false;

					if (photo == null)
					{
						IsImageAvailable = false;
						return;
					}
					else
					{
						CropperSize = 400;

						ImgSource = ImageSource.FromStream(photo.GetStream);
					OnPropertyChanged("ImgSource");
						IsImageAvailable = true;
						IsImagePresent = true;
						
					}


				}
					}

			private async Task TakePicture()
			{
				if (crossMedia == false)
				{
					crossMedia = true;

					await CrossMedia.Current.Initialize();
					photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
					{
						Directory = "ERNIOCR",
						Name = "photoOCR.png",
						PhotoSize = PhotoSize.Small

					});

					crossMedia = false;

					if (photo == null)
					{
						return;
					}
					else
					{
						CropperSize = 300f;
						ImgSource = ImageSource.FromStream(photo.GetStream);
						IsImageAvailable = true;
						IsImagePresent = true;
						
					}

					
				}
					}

		private async Task CropImage()
		{
			using (UserDialogs.Instance.Loading("Extracting Image"))
			{

				using (var stream = photo.GetStream())
				{

					var croppedImageSource = DependencyService.Get<IImageCropper>().CropImage(ImgSource, CropRect, new Size(200, 200));

					if (croppedImageSource == null)
					{
						return;
					}

					CroppedImage = croppedImageSource;
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged([CallerMemberName] string name="")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
