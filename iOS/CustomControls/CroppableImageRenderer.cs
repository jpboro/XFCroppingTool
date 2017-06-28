using System;
using System.IO;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XFCroppingTool.CustomControls;
using XFCroppingTool.iOS;
using XFCroppingTool.iOS.CustomControls;

[assembly: ExportRenderer(typeof(Cropper), typeof(CroppableImageRenderer))]
namespace XFCroppingTool.iOS
{
public class CroppableImageRenderer : ViewRenderer<Cropper, UIView>
{
	private UIImageView imgImage;
	private UIImage CurrentBitmap;
	private UIView layout;
	private UIView stkLayout;
	private CropView rectCrop;

	protected override void OnElementChanged(ElementChangedEventArgs<Cropper> e)
	{
		base.OnElementChanged(e);
		if (e.OldElement != null || this.Element == null)
			return;



		stkLayout = new UIView();
		stkLayout.Hidden = !Element.HasImage;
		stkLayout.BackgroundColor = Color.Blue.ToUIColor();

		stkLayout.AutoresizingMask = UIViewAutoresizing.All;

		layout = new UIView();
		//layout.Hidden = !Element.HasImage;
		layout.BackgroundColor = UIColor.DarkGray;
		layout.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

		imgImage = new UIImageView();
		imgImage.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
		imgImage.ContentMode = UIViewContentMode.ScaleAspectFit;
		//imgImage.Hidden = !Element.HasImage;

		LoadImage();


		rectCrop = new CropView();

		rectCrop.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

		rectCrop.RectChange += rectCrop_RectChange;



		layout.AddSubview(imgImage);

		layout.AddSubview(rectCrop);


		stkLayout.AddSubview(layout);
		SetNativeControl(stkLayout);

		stkLayout.Hidden = true;

		imgImage.Hidden = true;
		layout.Hidden = true;


		imgImage.AutosizesSubviews = true;
		layout.AutosizesSubviews = true;
		stkLayout.AutosizesSubviews = true;
		rectCrop.AutosizesSubviews = true;


	}

	void rectCrop_RectChange(object sender, EventArgs e)
	{
		UpdateCropRect();
	}

	protected void UpdateCropRect()
	{
		if (CurrentBitmap != null)
		{
			CGRect _CropValue = rectCrop.CropValue;
			nfloat rectX = _CropValue.Left;
			nfloat rectY = _CropValue.Top;
			nfloat rectW = _CropValue.Width;
			nfloat rectH = _CropValue.Height;


			BitmapRectScale brs = ImageUtil.GetBitmapBoundsInsideImage(CurrentBitmap, imgImage);

			var cropX = (rectX - brs.Rect.Left) * brs.Scale;
			var cropY = (rectY - brs.Rect.Top) * brs.Scale;
			var cropW = brs.Scale * rectW;
			var cropH = brs.Scale * rectH;

			if (cropX < 0) { cropW += cropX; cropX = 0; }
			if (cropY < 0) { cropH += cropY; cropY = 0; }
			if (cropW > brs.OrigWidth) { cropW = brs.OrigWidth; }
			if (cropH > brs.OrigHeight) { cropH = brs.OrigHeight; }
			if (cropX + cropW > brs.OrigWidth) { cropW = brs.OrigWidth - cropX; }
			if (cropY + cropH > brs.OrigHeight) { cropH = brs.OrigHeight - cropY; }

			Element.CropRect = new Xamarin.Forms.Rectangle((double)cropX, (double)cropY, (double)cropW, (double)cropH);

		}
	}


	protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		base.OnElementPropertyChanged(sender, e);
		if (this.Element == null || this.Control == null)
			return;

		if (e.PropertyName == Cropper.SourceProperty.PropertyName)
		{
			LoadImage();

			UpdateCropRect();

			layout.AddSubview(rectCrop);


			stkLayout.AddSubview(layout);
			SetNativeControl(stkLayout);
		}

		if (e.PropertyName == Cropper.HasImageProperty.PropertyName)
		{
			stkLayout.Hidden = false;
			rectCrop.Hidden = false;
			imgImage.Hidden = false;
			layout.Hidden = false;





		}
	}

	private void LoadImage()
	{
		CurrentBitmap = null;
		if (Element.Source != null)
		{
			if (Element.Source.GetType().Equals(typeof(FileImageSource)))
			{
				CurrentBitmap = new UIImage(((FileImageSource)Element.Source).File);
			}

			if (Element.Source.GetType().Equals(typeof(StreamImageSource)))
			{
				StreamImageSource iSource = (StreamImageSource)Element.Source;
				Stream s = iSource.Stream(System.Threading.CancellationToken.None).Result;
				MemoryStream ms = new MemoryStream();
				s.Position = 0;
				s.CopyTo(ms);
				NSData data = NSData.FromArray(ms.ToArray());
				CurrentBitmap = UIImage.LoadFromData(data);


			}

			if (CurrentBitmap != null)
			{


				stkLayout.Hidden = false;

				imgImage.Hidden = false;
				layout.Hidden = false;

				stkLayout.AutoresizingMask = UIViewAutoresizing.All;
				layout.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
				imgImage.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
				imgImage.ContentMode = UIViewContentMode.ScaleAspectFit;

				rectCrop.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

				rectCrop.RectChange += rectCrop_RectChange;

				CurrentBitmap = ImageUtil.Rotate(CurrentBitmap);

				imgImage.Image = CurrentBitmap;

				rectCrop.CropLimit = ImageUtil.GetBitmapBoundsInsideImage(CurrentBitmap, imgImage).Rect;
				rectCrop.SetNeedsDisplay();

				UpdateCropRect();


			}

		}

	}	}
}
