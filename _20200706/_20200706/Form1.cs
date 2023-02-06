using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Google.Maps;
using Google.Maps.Direction;
using Google.Maps.Geocoding;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace _20200706
{
    public partial class Form1 : Form
    {
        List<PointLatLng> pointList = new List<PointLatLng>();
        public Form1()
        {
            string authKey = "AIzaSyDi9t-vPKn-xXNP9swFQk3wdeE5mU4IDj4";
            InitializeComponent();
            initMapControl();
            GoogleSigned.AssignAllServices(new GoogleSigned(authKey));

            var request = new GeocodingRequest();
            request.Address = "1600 Pennsylvania Ave NW, Washington, DC 20500";
            var response = new GeocodingService().GetResponse(request);

        }

        private GMapControl mainMap;

        private void initMapControl()
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;

            mainMap = new GMapControl();

            mainMap.Name = "MainMap";
            mainMap.RoutesEnabled = true;//路線
            mainMap.BackColor = Color.White;
            mainMap.Dock = DockStyle.Fill;
            mainMap.Location = new Point(0, 0);
            mainMap.PolygonsEnabled = true;
            mainMap.MarkersEnabled = true;//圖標
            mainMap.NegativeMode = false;
            mainMap.RetryLoadTile = 0;
            mainMap.ShowTileGridLines = false;//格線
            mainMap.AllowDrop = true;
            mainMap.IgnoreMarkerOnMouseWheel = true;
            mainMap.DragButton = MouseButtons.Left;
            mainMap.DisableFocusOnMouseEnter = true;
            mainMap.MinZoom = 0;
            mainMap.MaxZoom = 24;
            mainMap.Zoom = 9;
            mainMap.Position = new PointLatLng(22.6272784, 120.3014353);
            mainMap.MapProvider = GMapProviders.GoogleMap;
            mainMap.TabIndex = 0;//mainMap可以以Tab鍵取得聚焦(默認順序)

            mainMap.OnMapTypeChanged += MainMap_OnMapTypeChanged;
            mainMap.OnMapClick += UserControlGMap_OnMapClick;

            panel1.Controls.Add(mainMap);



        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            getRoute();

        }
        private void MainMap_OnMapTypeChanged(GMapProvider type)
        {
            throw new NotImplementedException();
        }

        private void UserControlGMap_OnMapClick(PointLatLng PointClick, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                GMapMarker myCity = new GMarkerGoogle(PointClick, GMarkerGoogleType.green_small);
                myCity.ToolTipMode = MarkerTooltipMode.Always;
                myCity.ToolTipText = "Welcome to Kaohsiung!";



                //創建一個名爲“markers”的圖層
                GMapOverlay markers = new GMapOverlay("markers");
                //創建標記，並設置位置及樣式
                GMapMarker marker = new GMarkerGoogle(PointClick, GMarkerGoogleType.red);
                //將標記添加到圖層
                markers.Markers.Add(marker);
                //將圖層添加到地圖
                this.mainMap.Overlays.Add(markers);
              
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            foreach (var overlay in mainMap.Overlays)
            {
                if (overlay.Id.Equals("routes"))
                {
                    mainMap.Overlays.Remove(overlay);
                    mainMap.Refresh();
                    break;
                }
            }


            //var marks = this.mainMap.Overlays.Where(x => x.Id.Equals("markers")).First();
            //this.mainMap.Overlays.Remove(marks);
            //this.mainMap.Refresh();

        }


        public void getRoute()
        {
            var request = new DirectionRequest();
            request.Origin = "place_id:ChIJTba1dMUPbjQRtM7f7P0_kJk";
            request.Destination = "place_id:ChIJi6-fW8YPbjQRqXV2cRXAU9U";
            var response= new DirectionService().GetResponse(request);
            DirectionStep[] steps = response.Routes[0].Legs[0].Steps;
            foreach (var step in steps)
            {
                pointList.Add(new PointLatLng(step.StartLocation.Latitude,step.StartLocation.Longitude));
                pointList.Add(new PointLatLng(step.EndLocation.Latitude, step.EndLocation.Longitude));               
         
            }

            GMapOverlay routes = new GMapOverlay("routes");
            GMapRoute mapRoute = new GMapRoute(pointList, "mapRoute");
            mapRoute.Stroke = new Pen(Color.Red, 3);
            routes.Routes.Add(mapRoute);
            mainMap.Overlays.Add(routes);
            mainMap.ZoomAndCenterRoute(mapRoute);//focus在哪條路
        }



    
    }
}
