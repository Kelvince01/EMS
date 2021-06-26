using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace EMS.Views.Charts
{
    public sealed partial class MapsView : UserControl
    {
        public MapsView()
        {
            this.InitializeComponent();
        }

        async void Map_LoadedAsync(object sender, RoutedEventArgs args)
        {
            /*mapControl.Center = new Geopoint(
                new BasicGeoposition()
                    { Latitude = 0, Longitude = 30 }); */

            mapControl.ZoomLevel = 12;
            mapControl.LandmarksVisible = true;
            // Set map style  
            mapControl.Style = MapStyle.Aerial3DWithRoads;
            //mapControl.StyleSheet = MapStyleSheet.RoadDark();

            /*mapControl.StyleSheet = MapStyleSheet.ParseFromJson(@"
                    {
                    ""version"": ""1.0"",
                    ""settings"": {
                        ""landColor"": ""#FFFFFF"",
                        ""spaceColor"": ""#000000""
                    },
                    ""elements"": {
                        ""mapElement"": {
                            ""labelColor"": ""#000000"",
                            ""labelOutlineColor"": ""#FFFFFF""
                        },
                        ""water"": {
                            ""fillColor"": ""#DDDDDD""
                        },
                        ""area"": {
                            ""fillColor"": ""#EEEEEE""
                        },
                        ""political"": {
                            ""borderStrokeColor"": ""#CCCCCC"",
                            ""borderOutlineColor"": ""#00000000""
                        }
                    }
                }
            ");*/

            MapStyleSheet customSheet = MapStyleSheet.ParseFromJson(@"
                {
                    ""version"": ""1.0"",
                    ""elements"": {
                        ""water"": {
                            ""fillColor"": ""#DDDDDD""
                        }
                    }
                }
            ");

            MapStyleSheet builtInSheet = MapStyleSheet.RoadDark();

            mapControl.StyleSheet = MapStyleSheet.Combine(new List<MapStyleSheet> { builtInSheet, customSheet });


            // Set your current location.
            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    // Get the current location.  
                    Geolocator geolocator = new Geolocator();
                    Geoposition pos = await geolocator.GetGeopositionAsync();
                    Geopoint myLocation = pos.Coordinate.Point;
                    // Set the map location.  
                    mapControl.Center = myLocation;
                    break;

                case GeolocationAccessStatus.Denied:
                    mapControl.Center = new Geopoint(
                        new BasicGeoposition()
                    { Latitude = 0, Longitude = 30 });
                    break;

                case GeolocationAccessStatus.Unspecified:
                    mapControl.Center = new Geopoint(
                         new BasicGeoposition()
                    { Latitude = 0, Longitude = 30 });
                    break;
            }
        }

        private void Seattle_OnClick(object sender, RoutedEventArgs e)
        {
            Geopoint seattlePoint = new Geopoint
                (new BasicGeoposition { Latitude = 47.6062, Longitude = -122.3321 });

            PlaceInfo spaceNeedlePlace = PlaceInfo.Create(seattlePoint);

            FrameworkElement targetElement = (FrameworkElement)sender;

            GeneralTransform generalTransform =
                targetElement.TransformToVisual((FrameworkElement)targetElement.Parent);

            Rect rectangle = generalTransform.TransformBounds(new Rect(new Point
                (targetElement.Margin.Left, targetElement.Margin.Top), targetElement.RenderSize));

            spaceNeedlePlace.Show(rectangle, Windows.UI.Popups.Placement.Below);
        }

        private void SpaceNeedle_Click(object sender, RoutedEventArgs e)
        {
            Geopoint spaceNeedlePoint = new Geopoint
                (new BasicGeoposition { Latitude = 47.6205, Longitude = -122.3493 });

            PlaceInfoCreateOptions options = new PlaceInfoCreateOptions();

            options.DisplayAddress = "400 Broad St, Seattle, WA 98109";
            options.DisplayName = "Seattle Space Needle";

            PlaceInfo spaceNeedlePlace = PlaceInfo.Create(spaceNeedlePoint, options);

            FrameworkElement targetElement = (FrameworkElement)sender;

            GeneralTransform generalTransform =
                targetElement.TransformToVisual((FrameworkElement)targetElement.Parent);

            Rect rectangle = generalTransform.TransformBounds(new Rect(new Point
                (targetElement.Margin.Left, targetElement.Margin.Top), targetElement.RenderSize));

            spaceNeedlePlace.Show(rectangle, Windows.UI.Popups.Placement.Below);
        }

        private async void showStreetsideView()
        {
            // Check if Streetside is supported.
            if (mapControl.IsStreetsideSupported)
            {
                // Find a panorama near Avenue Gustave Eiffel.
                BasicGeoposition cityPosition = new BasicGeoposition() { Latitude = 48.858, Longitude = 2.295 };
                Geopoint cityCenter = new Geopoint(cityPosition);
                StreetsidePanorama panoramaNearCity = await StreetsidePanorama.FindNearbyAsync(cityCenter);

                // Set the Streetside view if a panorama exists.
                if (panoramaNearCity != null)
                {
                    // Create the Streetside view.
                    StreetsideExperience ssView = new StreetsideExperience(panoramaNearCity);
                    ssView.OverviewMapVisible = true;
                    mapControl.CustomExperience = ssView;
                }
            }
            else
            {
                // If Streetside is not supported
                ContentDialog viewNotSupportedDialog = new ContentDialog()
                {
                    Title = "Streetside is not supported",
                    Content = "\nStreetside views are not supported on this device.",
                    PrimaryButtonText = "OK"
                };
                await viewNotSupportedDialog.ShowAsync();
            }
        }

        private async void display3DLocation()
        {
            if (mapControl.Is3DSupported)
            {
                // Set the aerial 3D view.
                mapControl.Style = MapStyle.Aerial3DWithRoads;

                // Specify the location.
                BasicGeoposition hwGeoposition = new BasicGeoposition() { Latitude = 43.773251, Longitude = 11.255474 };
                Geopoint hwPoint = new Geopoint(hwGeoposition);

                // Create the map scene.
                MapScene hwScene = MapScene.CreateFromLocationAndRadius(hwPoint,
                    80, /* show this many meters around */
                    0, /* looking at it to the North*/
                    60 /* degrees pitch */);
                // Set the 3D view with animation.
                await mapControl.TrySetSceneAsync(hwScene, MapAnimationKind.Bow);
            }
            else
            {
                // If 3D views are not supported, display dialog.
                ContentDialog viewNotSupportedDialog = new ContentDialog()
                {
                    Title = "3D is not supported",
                    Content = "\n3D views are not supported on this device.",
                    PrimaryButtonText = "OK"
                };
                await viewNotSupportedDialog.ShowAsync();
            }
        }

        private void ShowStreetsideView_OnClick(object sender, RoutedEventArgs e)
        {
            showStreetsideView();
        }

        private void Display3DLocation_OnClick(object sender, RoutedEventArgs e)
        {
            display3DLocation();
        }

        public void AddSpaceNeedleIcon()
        {
            var MyLandmarks = new List<MapElement>();

            BasicGeoposition snPosition = new BasicGeoposition { Latitude = 47.620, Longitude = -122.349 };
            Geopoint snPoint = new Geopoint(snPosition);

            var spaceNeedleIcon = new MapIcon
            {
                Location = snPoint,
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                ZIndex = 0,
                Title = "Space Needle"
            };

            MyLandmarks.Add(spaceNeedleIcon);

            var LandmarksLayer = new MapElementsLayer
            {
                ZIndex = 1,
                MapElements = MyLandmarks
            };

            mapControl.Layers.Add(LandmarksLayer);

            mapControl.Center = snPoint;
            mapControl.ZoomLevel = 14;

        }

        private void DisplayNeedleBtn_OnClick(object sender, RoutedEventArgs e)
        {
            AddSpaceNeedleIcon();
        }
    }
}
