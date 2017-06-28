using System;
using Xamarin.Forms;

namespace XFCroppingTool.CustomControls
{
	public class Cropper : ContentView
	{
//Backing field for Image Property of Cropper
//public static readonly BindableProperty SourceProperty =
//	BindableProperty.Create<Cropper, ImageSource>(
//		p => p.Source, null);
public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source),
		                           typeof(ImageSource), typeof(Cropper), default(ImageSource));

//property Source get/sets SourceProperty
[TypeConverter(typeof(ImageSourceConverter))]

public ImageSource Source
{
get { return (ImageSource)GetValue(SourceProperty); }
set { SetValue(SourceProperty, value); }
}

public static readonly BindableProperty CropRectProperty = BindableProperty.Create<Cropper, Rectangle>(p => p.CropRect, new Rectangle(0, 0, 100, 100));

public Rectangle CropRect
{
get { return (Rectangle)GetValue(CropRectProperty); }
set { SetValue(CropRectProperty, value); }
}


//----------------------------------------------------
//imagewidth
public static readonly BindableProperty ImageWidthRequestProperty =
	BindableProperty.Create<Cropper, int>(
		p => p.ImageWidthRequest, default(int));

public int ImageWidthRequest
{
get { return (int)GetValue(ImageWidthRequestProperty); }
set { SetValue(ImageWidthRequestProperty, value); }
}

//----------------------------------------------------
//Backing field for iamge height property
public static readonly BindableProperty ImageHeightRequestProperty =
	BindableProperty.Create<Cropper, int>(
		p => p.ImageHeightRequest, default(int));

//ImageHeightRequest property gets/sets value of ImageHeightRequestProperty
public int ImageHeightRequest
{
get { return (int)GetValue(ImageHeightRequestProperty); }
set { SetValue(ImageHeightRequestProperty, value); }
}
//------------------------------------------------------
//Boolean isCropped
public static readonly BindableProperty DoneCroppingProperty = BindableProperty.Create<Cropper, Boolean>(p => p.DoneCropping, false);

public Boolean DoneCropping
{
get { return (Boolean)GetValue(DoneCroppingProperty); }
set { SetValue(DoneCroppingProperty, value); }
}

public static readonly BindableProperty HasImageProperty = BindableProperty.Create<Cropper, Boolean>(p => p.HasImage, false);

public Boolean HasImage
{
	get { return (Boolean)GetValue(HasImageProperty); }
	set { SetValue(HasImageProperty, value); }
		}	
	}
}
