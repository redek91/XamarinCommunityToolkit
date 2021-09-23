using System;
using System.Diagnostics;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace Xamarin.CommunityToolkit.Sample.Pages.Views
{
	public partial class CameraViewPage : BasePage
	{
		// Note: not all options of the CameraView are on this page, make sure to discover them for yourself!
		public CameraViewPage()
		{
			InitializeComponent();

			zoomLabel.Text = string.Format("Zoom: {0}", zoomSlider.Value);
		}

		void ZoomSlider_ValueChanged(object? sender, ValueChangedEventArgs e)
		{
			cameraView.Zoom = (float)zoomSlider.Value;
			zoomLabel.Text = string.Format("Zoom: {0}", Math.Round(zoomSlider.Value));
		}

		void VideoSwitch_Toggled(object? sender, ToggledEventArgs e)
		{
			var captureVideo = e.Value;

			if (captureVideo)
				cameraView.CaptureMode = CameraCaptureMode.Video;
			else
				cameraView.CaptureMode = CameraCaptureMode.Photo;

			previewPicture.IsVisible = !captureVideo;

			doCameraThings.Text = e.Value ? "Start Recording"
				: "Snap Picture";
		}

		// You can also set it to Default and External
		void Camera_Changed(object? sender, CheckedChangedEventArgs e)
		{
			if(sender != null && e.Value)
			{
				var selectedRadioBtn = sender as RadioButton;

				switch (selectedRadioBtn?.Content)
				{
					case "Front":
						cameraView.CameraOptions = CameraOptions.Front;
						break;
					case "Back":
						cameraView.CameraOptions = CameraOptions.Back;
						break;
					case "External":
						cameraView.CameraOptions = CameraOptions.External;
						break;
				}
			}
		}

		// You can also set it to Torch (always on) and Auto
		void FlashSwitch_Toggled(object? sender, ToggledEventArgs e)
			=> cameraView.FlashMode = e.Value ? CameraFlashMode.On : CameraFlashMode.Off;

		void EnablePreview_Toggled(object? sender, ToggledEventArgs e)
			=> cameraView.IsPreviewEnabled = e.Value;

		void DoCameraThings_Clicked(object? sender, EventArgs e)
		{
			cameraView.Shutter();
			doCameraThings.Text = cameraView.CaptureMode == CameraCaptureMode.Video
				? "Stop Recording"
				: "Snap Picture";
		}

		void CameraView_OnRecording(object? sender, bool e)
		{
			previewSwitch.IsEnabled = !e;
			camerasRadioButtonGroup.IsEnabled = !e;
			videoSwitch.IsEnabled = !e;
		}

		void CameraView_OnAvailable(object? sender, bool e)
		{
			if (e)
			{
				zoomSlider.Value = cameraView.Zoom;
				var max = cameraView.MaxZoom;
				if (max > zoomSlider.Minimum && max > zoomSlider.Value)
					zoomSlider.Maximum = max;
				else
					zoomSlider.Maximum = zoomSlider.Minimum + 1; // if max == min throws exception
			}

			doCameraThings.IsEnabled = e;
			zoomSlider.IsEnabled = e;
			flashSwitch.IsEnabled = e;
			videoSwitch.IsEnabled = e;
		}

		void CameraView_MediaCaptured(object? sender, MediaCapturedEventArgs e)
		{
			switch (cameraView.CaptureMode)
			{
				default:
				case CameraCaptureMode.Default:
				case CameraCaptureMode.Photo:
					previewVideo.IsVisible = false;
					previewPicture.IsVisible = true;
					previewPicture.Rotation = e.Rotation;
					previewPicture.Source = e.Image;
					doCameraThings.Text = "Snap Picture";
					break;
				case CameraCaptureMode.Video:
					previewPicture.IsVisible = false;
					previewVideo.IsVisible = true;
					previewVideo.Source = e.Video;
					doCameraThings.Text = "Start Recording";
					break;
			}
		}
	}
}