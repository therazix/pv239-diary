/***************************************************************************************
*    Title: Bertuzzi.MAUI.PinchZoomImage
*    Author: Thiago Bertuzzi
*    Date: 2022-08-17
*    Code version: 1.0.0
*    Availability: https://github.com/TBertuzzi/Bertuzzi.MAUI.PinchZoomImage
*    The code was modified to fit the needs of this project.
*    Date of modification: 2024-05-16
***************************************************************************************/

namespace Diary.Controls;

public class PinchZoom : ContentView
{
    private double _currentScale = 1;
    private double _startScale = 1;
    private double _xOffset = 0;
    private double _yOffset = 0;
    private bool _secondDoubleTapp = false;
    private double _windowWidth = Application.Current?.Windows[0].Width ?? 0;
    private double _windowHeight = Application.Current?.Windows[0].Height ?? 0;

    public PinchZoom()
    {
        var pinchGesture = new PinchGestureRecognizer();
        pinchGesture.PinchUpdated += PinchUpdated;
        GestureRecognizers.Add(pinchGesture);

        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;
        GestureRecognizers.Add(panGesture);

        var tapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
        tapGesture.Tapped += DoubleTapped;
        GestureRecognizers.Add(tapGesture);
    }

    private void PinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
    {
        if (e.Status == GestureStatus.Started)
        {
            _startScale = Content.Scale;
            Content.AnchorX = 0;
            Content.AnchorY = 0;
        }
        else if (e.Status == GestureStatus.Running)
        {
            _currentScale += (e.Scale - 1) * _startScale;
            _currentScale = Math.Max(1, _currentScale);

            var renderedX = Content.X + _xOffset;
            var deltaX = renderedX / Width;
            var deltaWidth = Width / (Content.Width * _startScale);
            var originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

            var renderedY = Content.Y + _yOffset;
            var deltaY = renderedY / Height;
            var deltaHeight = Height / (Content.Height * _startScale);
            var originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

            var targetX = _xOffset - (originX * Content.Width) * (_currentScale - _startScale);
            var targetY = _yOffset - (originY * Content.Height) * (_currentScale - _startScale);

            Content.TranslationX = Math.Min(0, Math.Max(targetX, -Content.Width * (_currentScale - 1)));
            Content.TranslationY = Math.Min(0, Math.Max(targetY, -Content.Height * (_currentScale - 1)));

            Content.Scale = _currentScale;
        }
        else if (e.Status == GestureStatus.Completed)
        {
            _xOffset = Content.TranslationX;
            _yOffset = Content.TranslationY;
        }
    }

    public void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (Content.Scale == 1)
        {
            return;
        }

        if (e.StatusType == GestureStatus.Running)
        {
            var newX = (e.TotalX * Scale) + _xOffset;
            var newY = (e.TotalY * Scale) + _yOffset;

            var width = Content.Width * Content.Scale;
            var height = Content.Height * Content.Scale;

            var minX = Content.Width - width;
            var maxX = 0;

            var minY = Content.Height - height;
            var maxY = 0;

            Content.TranslationX = width > _windowWidth ? Math.Clamp(newX, minX, maxX) : 0;
            Content.TranslationY = height > _windowHeight ? Math.Clamp(newY, minY, maxY) : 0;
        }
        else if (e.StatusType == GestureStatus.Completed)
        {
            _xOffset = Content.TranslationX;
            _yOffset = Content.TranslationY;
        }
    }

    public async void DoubleTapped(object? sender, TappedEventArgs e)
    {
        var multiplicator = Math.Pow(2, 1.0 / 10.0);
        _startScale = Content.Scale;
        Content.AnchorX = 0;
        Content.AnchorY = 0;

        for (var i = 0; i < 10; i++)
        {
            if (!_secondDoubleTapp)
            {
                _currentScale *= multiplicator;
            }
            else
            {
                _currentScale /= multiplicator;
            }

            var renderedX = Content.X + _xOffset;
            var deltaX = renderedX / Width;
            var deltaWidth = Width / (Content.Width * _startScale);
            var originX = (0.5 - deltaX) * deltaWidth;

            var renderedY = Content.Y + _yOffset;
            var deltaY = renderedY / Height;
            var deltaHeight = Height / (Content.Height * _startScale);
            var originY = (0.5 - deltaY) * deltaHeight;

            var targetX = _xOffset - (originX * Content.Width) * (_currentScale - _startScale);
            var targetY = _yOffset - (originY * Content.Height) * (_currentScale - _startScale);

            Content.TranslationX = Math.Min(0, Math.Max(targetX, -Content.Width * (_currentScale - 1)));
            Content.TranslationY = Math.Min(0, Math.Max(targetY, -Content.Height * (_currentScale - 1)));

            Content.Scale = _currentScale;
            await Task.Delay(10);
        }
        _secondDoubleTapp = !_secondDoubleTapp;
        _xOffset = Content.TranslationX;
        _yOffset = Content.TranslationY;
    }
}
