/*
 * Работа водитела автобуса
 * Версия 1.0
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using System.IO;
using MySql.Data.MySqlClient;
using System.Data;
using System.Timers;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace RP_Server
{
    public class JobBus : Script
    {
        private List<Tuple<Vector3, PointType>> BusRoute = new List<Tuple<Vector3, PointType>>();
        private List<List<Tuple<Vector3, PointType>>> BusRoutes = new List<List<Tuple<Vector3, PointType>>>();
        private List<Bus> JobBusList = new List<Bus>();

        class Bus
        {
            public VehicleHash Type;
            public Client Player;
            public Vector3 Coords;
            public Vector3 Rotation;
            public VehInfo JobBus;
        }

        private enum PointType
        {
            BusPoint,
            BusStop,
            BusLastStop,
            BusGarage
        }

        public JobBus()
        {
            API.onResourceStart += onResourceStart;
            API.onEntityEnterColShape += onMarkerEnter;
            API.onClientEventTrigger += onClientEvent;
            API.onPlayerEnterVehicle += onPlayerEnterVehicle;
            API.onPlayerExitVehicle += onPlayerExitVehicle;
            API.onPlayerDeath += onPlayerDeathHandler;
            API.onVehicleDeath += onVehicleDeathHandler;
        }

        public void onResourceStart()
        {
            lock (Global.Lock)
            {
                try
                {
                    MarkerInfo mInfo = new MarkerInfo(1, new Vector3(437.00, -624.30, 27.90), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
                    new BlipInfo(new Vector3(437.00, -624.30, 28.71), 513, 46, false, 0, "Работа водителя автобуса");
                    new TLabelInfo("Работа водителя автобуса", new Vector3(437.00, -624.30, 28.71), 25, 1);
                    mInfo.SetData("JobBuzAction", 1);

                    #region "новые остановки для маршрута 1"
                    API.createObject(1888204845, new Vector3(-744.5518f, -2435.282f, 13.49f), new Vector3(0f, 0.9667061f, -119.9994f));
                    API.createObject(1888204845, new Vector3(0.699228f, -1708.61f, 28.21f), new Vector3(0f, 0f, -156.2589f));
                    API.createObject(1888204845, new Vector3(453.8635f, -961.7674f, 27.48f), new Vector3(-4.46236E-05f, -5.008956E-06f, -177.3061f));
                    #endregion

                    #region "Инициализируем маршруты"
                    #region "Инициализируем маршрут #1"
                    BusRoute.Clear();
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(455.441, -651.495, 26.99), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(306.8411, -766.1552, 28.23), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-109.0075, -1686.941, 28.18), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1018.717, -2732.411, 12.67), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-746.2972, -2433.931, 13.46), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-157.9107, -2186.088, 9.27), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-0.004008889, -1706.658, 28.16), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(277.2107, -1553.361, 28.06), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(453.5601, -959.6331, 27.39), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(455.441, -651.495, 26.99), PointType.BusStop));
                    BusRoutes.Add(BusRoute);
                    #endregion
                    /*
                    #region "Инициализируем маршрут #2"
                    BusRoute.Clear();
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(422.419, -659.114, 27.9334), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(457.567, -647.27, 28.2496), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(455.216, -583.936, 27.823), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(423.9649, -619.2914, 28.49657), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(341.484, -688.231, 28.6716), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(305.167, -771.223, 28.5404), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(173.207, -1126.37, 28.5698), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(49.8899, -1229.27, 28.5282), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(41.7059, -1359.03, 28.5884), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-286.578, -1462.6, 30.5584), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-426.344, -1848.5, 19.0572), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-825.284, -2218.67, 16.663), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1023.98, -2733.62, 19.479), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-804.662, -2479.66, 13.0457), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-685.972, -2111.02, 13.1456), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-98.8138, -1738.83, 29.1041), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(572.54, -1439.54, 28.9859), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(806.592, -1348.02, 25.579), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(760.855, -1002.61, 25.5079), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(400.747, -1023.92, 28.7404), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(453.41, -958.428, 27.7634), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(503.208, -908.524, 25.3577), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(457.567, -647.27, 28.2496), PointType.BusLastStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(467.94, -611.941, 28.4957), PointType.CheckPass));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(446.764, -576.368, 27.8199), PointType.BusGarage));
                    BusRoutes.Add(BusRoute);
                    #endregion

                    #region "Инициализируем маршрут #3"
                    BusRoute.Clear();
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(421.012, -649.304, 27.9456), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(458.609, -639.797, 28.4803), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(455.216, -583.936, 27.823), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(426.433, -613.481, 28.4962), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-23.218, -544.816, 38.7386), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(7.89167, -264.283, 46.6574), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-331.407, -180.187, 38.2298), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-467.665, -126.727, 38.0712), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-693.596, -6.53661, 37.4588), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-767.081, 104.275, 54.5077), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1001.1, 92.1409, 51.3498), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1418.12, 186.228, 57.1096), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1399.46, -130.731, 48.6736), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1266.49, -426.795, 33.5502), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1249.41, -607.079, 26.8355), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1418.86, -786.063, 20.9619), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1211.8, -1220.88, 7.24112), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1125.25, -1333.6, 4.83154), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-616.419, -863.97, 25.0907), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-556.211, -845.81, 27.3955), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-246.682, -883.912, 30.1564), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-168.962, -861.496, 29.209), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-45.7593, -550.939, 39.5909), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(458.609, -639.797, 28.4803), PointType.BusLastStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(467.94, -611.941, 28.4957), PointType.CheckPass));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(446.764, -576.368, 27.8199), PointType.BusGarage));
                    BusRoutes.Add(BusRoute);
                    #endregion

                    #region "Инициализируем маршрут #4"
                    BusRoute.Clear();
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(421.012, -649.304, 27.9456), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(458.568, -631.972, 29.33), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(436.712, -589.754, 27.8227), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(402.453, -705.611, 28.47778), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(414.93, -859.219, 29.31992), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1155.54, -871.508, 54.0427), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1362.19, -1589.47, 52.73868), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2413.56, -447.481, 71.8119), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2567.6, 145.659, 97.61258), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2594.04, 338.311, 108.2648), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2574.21, 760.665, 91.11195), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2457.75, 1272.61, 52.26698), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2547.06, 1607.51, 30.06583), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2455.92, 2863.65, 48.89825), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2804.1, 4458.58, 48.12378), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(569.481, 6551.7, 27.9254), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-215.193, 6173.68, 31.21096), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-2499.29, 3611.28, 14.17644), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-3137.29, 1043.63, 19.89664), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1651.46, -731.018, 11.42774), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-747.932, -530.663, 25.14336), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(86.7405, -542.901, 33.79305), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(199.532, -833.07, 31.01124), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(368.618, -677.397, 29.16312), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(458.568, -631.972, 29.33), PointType.BusLastStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(467.94, -611.941, 28.4957), PointType.CheckPass));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(446.764, -576.368, 27.8199), PointType.BusGarage));
                    BusRoutes.Add(BusRoute);
                    #endregion

                    #region "Инициализируем маршрут #5"
                    BusRoute.Clear();
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(421.012, -649.304, 27.9456), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(459.881, -626.019, 28.4959), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(455.216, -583.936, 27.823), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(426.433, -613.481, 28.4962), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(305.167, -771.223, 28.5404), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(173.207, -1126.37, 28.5698), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(49.8899, -1229.27, 28.5282), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(104.344, -1451.87, 29.3383), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-107.764, -1687.71, 29.2347), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-248.576, -1840.52, 28.8312), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-159.452, -1993.79, 23.0166), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-121.281, -2089.56, 25.4805), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(166.449, -2023.52, 18.1881), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(253.324, -1830.6, 26.7261), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-94.8735, -1330.4, 29.2749), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(170.413, -407.609, 41.2087), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(236.289, -369.869, 44.2338), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(215.96, -664.497, 37.7117), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(258.939, -849.593, 29.463), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(460.609, -667.203, 27.5749), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(459.881, -626.019, 28.4959), PointType.BusLastStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(467.94, -611.941, 28.4957), PointType.CheckPass));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(446.764, -576.368, 27.8199), PointType.BusGarage));
                    BusRoutes.Add(BusRoute);
                    #endregion

                    #region "Инициализируем маршрут #6"
                    BusRoute.Clear();
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(421.012, -649.304, 27.9456), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(460.159, -617.787, 29.3314), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(455.216, -583.936, 27.823), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(426.433, -613.481, 28.4962), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(305.167, -771.223, 28.5404), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(203.802, -762.621, 33.5915), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(242.336, -348.598, 45.1279), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(208.834, -296.637, 47.1301), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(620.222, -351.272, 44.2691), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1129.27, 555.708, 98.3531), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1304.88, 1124.7, 106.472), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1159.66, 1826.91, 73.7572), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1159.66, 1826.91, 73.7572), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(909.731, 2220.95, 49.3085), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(594.187, 2342.05, 49.8371), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(341.156, 2647.74, 45.5738), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1291.35, 2675.14, 38.3505), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2178.82, 2919.29, 47.3048), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1885.42, 2634.33, 46.4714), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1859.89, 2626.62, 46.4716), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1926.73, 2602.61, 47.1375), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2174.31, 3028.76, 46.1989), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1814.02, 3314.13, 43.1601), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1819.34, 3701.1, 34.7023), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2493.08, 4146.11, 38.8947), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2468.44, 4566.38, 37.1369), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2081.7, 4711.53, 41.9428), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(783.57, 4274.82, 57.2192), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(144.431, 4440.31, 79.5122), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-596.196, 5018.38, 142.439), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1036.17, 4973.41, 201.037), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-777.747, 5629.4, 26.702), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-570.798, 6066.98, 17.0447), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-335.043, 6189.37, 32.0728), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(87.6714, 6591.66, 32.3146), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(67.8836, 6455.1, 32.1785), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-214.837, 6173.76, 32.0532), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1434.28, 5072.33, 62.1316), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-2505.56, 3594.04, 15.3488), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-2645.55, 2282.79, 24.4936), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-2077.91, 2278.58, 40.7709), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1900.33, 2017.94, 141.783), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-2504.14, 1822.51, 164.377), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-2639.5, 1481.51, 124.044), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1986.49, 527.392, 109.64), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1056.22, -748.768, 20.0543), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-692.259, -668.181, 31.6134), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-32.4384, -550.785, 40.413), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(460.609, -667.203, 27.5749), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(459.612, -618.298, 29.3278), PointType.BusLastStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(467.94, -611.941, 28.4957), PointType.CheckPass));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(446.764, -576.368, 27.8199), PointType.BusGarage));
                    BusRoutes.Add(BusRoute);
                    #endregion

                    #region "Инициализируем маршрут #7"
                    BusRoute.Clear();
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(421.012, -659.304, 27.9456), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(459.114, -608.504, 28.0751), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(455.216, -583.936, 27.823), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(439.274, -679.441, 28.4285), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(475.031, -1128.82, 28.9561), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(49.6684, -1204.53, 28.7515), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(178.08, -1415.34, 28.9183), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(774.982, -1751.9, 28.969), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(768.049, -1985.87, 28.7634), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(797.03, -2452.17, 21.3353), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(965.745, -2469.77, 27.9915), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1045.82, -2386.89, 29.8194), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1123.3, -2091.05, 39.1426), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1309.41, -2039.83, 44.9901), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1437.19, -1893.18, 71.1097), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1373.43, -1582.17, 53.2095), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2005.2, -908.225, 78.664), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2604.7, -486.057, 80.8062), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2492.08, -284.719, 92.5682), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2542.11, -301.857, 92.5686), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2531.93, -567.265, 66.6434), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2417.16, -450.63, 71.2948), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2618.44, 306.782, 97.3641), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2441.59, 1006.4, 84.7308), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2549.22, 1597.71, 29.6794), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2565.36, 2667.78, 40.2984), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2267.29, 3169.91, 48.5421), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2039.3, 3320.22, 45.2546), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2060.1, 3401.45, 44.3825), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2041.3, 3457.57, 43.3645), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1889.51, 3192.96, 45.5007), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2077.54, 3058.15, 45.673), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2184.45, 2984.96, 46.0046), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2174.64, 2938.37, 46.0647), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1961.89, 2628.47, 45.5538), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1885.56, 2630.31, 45.2475), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1873.59, 2682.79, 45.2723), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1859.49, 2621.82, 45.2473), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1938.87, 2606.61, 45.9101), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1887.71, 2962.79, 45.3293), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1171.67, 2690.7, 37.4485), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(454.232, 2683.9, 43.086), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(278.011, 2673.2, 44.0217), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(223.312, 3177.22, 41.9866), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(415.142, 3505.07, 34.1298), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(325.543, 3575.47, 33.1361), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(452.98, 3485.74, 34.0182), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1421.66, 3503.56, 35.489), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1879.86, 3606.36, 33.8847), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2924.56, 4381.63, 49.8955), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2991.93, 3429.67, 70.9309), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(3481.7, 3783.06, 29.8285), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(3598.06, 3783.3, 29.5326), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(3535.05, 3770.41, 30.763), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(3396.52, 3738.76, 30.9643), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2935.27, 4390.61, 49.7236), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2911.7, 4742.4, 48.8719), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2871.86, 5020.88, 31.3689), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(3036.72, 5034.23, 25.4631), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(3806.88, 4453.93, 3.80684), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(3793.58, 4468.02, 5.26851), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2754.51, 4996.79, 34.7796), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(2633.62, 5157.53, 44.3604), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(1546.62, 6445.53, 23.3492), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(520.812, 6568.57, 26.9457), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-217.238, 6171.82, 30.8003), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-2039.84, 4487.9, 56.6559), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-2502.32, 3609.7, 13.7849), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-2634.61, 2284.09, 24.3392), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1656.42, 2378.42, 36.3156), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1269.34, 2165.37, 58.9474), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-837.837, 1794.7, 171.322), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-760.54, 1682.84, 200.885), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-143.063, 1859.48, 197.731), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(142.914, 1586.61, 229.188), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(262.599, 1137.96, 221.234), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-880.428, 436.699, 86.5396), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1514.94, 238.443, 60.5854), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1917.54, 94.7007, 86.9146), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-2303.81, 375.599, 173.631), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-2299.51, 471.733, 173.539), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-1040.68, -736.361, 18.5646), PointType.BusPoint));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-691.192, -667.38, 30.0147), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(-504.145, -668.008, 32.1421), PointType.BusStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(459.612, -618.298, 29.3278), PointType.BusLastStop));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(467.94, -611.941, 28.4957), PointType.CheckPass));
                    BusRoute.Add(new Tuple<Vector3, PointType>(new Vector3(446.764, -576.368, 27.8199), PointType.BusGarage));
                    BusRoutes.Add(BusRoute);
                    #endregion*/
                    #endregion

                    #region "Спавним автобусы"
                    JobBusList.Add(new Bus() { Type = VehicleHash.Bus, Coords = new Vector3(399.7879, -655.0015, 28.49626), Rotation = new Vector3(-0.03351496, -0.2430249, -88.96815), JobBus = new VehInfo(VehicleHash.Bus, new Vector3(399.7879, -655.0015, 28.49626), new Vector3(-0.03351496, -0.2430249, -88.96815), 57, 57, 100, Jobs.JOB_BUS, 0).DoorLock() });
                    JobBusList.Add(new Bus() { Type = VehicleHash.Bus, Coords = new Vector3(399.5878, -649.3994, 28.49504), Rotation = new Vector3(-0.03946875, 0.02634213, -86.91249), JobBus = new VehInfo(VehicleHash.Bus, new Vector3(399.5878, -649.3994, 28.49504), new Vector3(-0.03946875, 0.02634213, -86.91249), 57, 57, 100, Jobs.JOB_BUS, 0).DoorLock() });
                    JobBusList.Add(new Bus() { Type = VehicleHash.Bus, Coords = new Vector3(399.3259, -643.7366, 28.49519), Rotation = new Vector3(-0.03257437, -8.597922E-05, -88.48151), JobBus = new VehInfo(VehicleHash.Bus, new Vector3(399.3259, -643.7366, 28.49519), new Vector3(-0.03257437, -8.597922E-05, -88.48151), 57, 57, 100, Jobs.JOB_BUS, 0).DoorLock() });
                    JobBusList.Add(new Bus() { Type = VehicleHash.Bus, Coords = new Vector3(399.258, -638.1654, 28.49527), Rotation = new Vector3(-0.03359096, -0.0003670798, -89.33932), JobBus = new VehInfo(VehicleHash.Bus, new Vector3(399.258, -638.1654, 28.49527), new Vector3(-0.03359096, -0.0003670798, -89.33932), 57, 57, 100, Jobs.JOB_BUS, 0).DoorLock() });
                    JobBusList.Add(new Bus() { Type = VehicleHash.Bus, Coords = new Vector3(406.5527, -625.1669, 28.49508), Rotation = new Vector3(-0.03391325, -9.977291E-05, -91.81745), JobBus = new VehInfo(VehicleHash.Bus, new Vector3(406.5527, -625.1669, 28.49508), new Vector3(-0.03391325, -9.977291E-05, -91.81745), 57, 57, 100, Jobs.JOB_BUS, 0).DoorLock() });
                    JobBusList.Add(new Bus() { Type = VehicleHash.Coach, Coords = new Vector3(429.801, -606.3447, 28.4948), Rotation = new Vector3(-0.03098145, 0.0002555825, 179.53), JobBus = new VehInfo(VehicleHash.Coach, new Vector3(429.801, -606.3447, 28.4948), new Vector3(-0.03098145, 0.0002555825, 179.53), 57, 57, 100, Jobs.JOB_BUS, 0).DoorLock() });
                    JobBusList.Add(new Bus() { Type = VehicleHash.Coach, Coords = new Vector3(424.2149, -606.2841, 28.49482), Rotation = new Vector3(-0.03149922, 0.0003925146, -178.8092), JobBus = new VehInfo(VehicleHash.Coach, new Vector3(424.2149, -606.2841, 28.49482), new Vector3(-0.03149922, 0.0003925146, -178.8092), 57, 57, 100, Jobs.JOB_BUS, 0).DoorLock() });
                    JobBusList.Add(new Bus() { Type = VehicleHash.Coach, Coords = new Vector3(419.1743, -606.4861, 28.49475), Rotation = new Vector3(-0.03183031, 0.0003270483, -177.3605), JobBus = new VehInfo(VehicleHash.Coach, new Vector3(419.1743, -606.4861, 28.49475), new Vector3(-0.03183031, 0.0003270483, -177.3605), 57, 57, 100, Jobs.JOB_BUS, 0).DoorLock() });
                    #endregion
                }
                catch (Exception exp)
                {
                    API.consoleOutput("Произошла ошибка при инициализации миссии иммигранта: " + exp.Message);
                    return;
                }
            }
        }

        private void onVehicleDeathHandler(NetHandle vehicle)
        {
            try
            {
                var myBus = JobBusList.FirstOrDefault(x => x.JobBus.Veh == vehicle);
                if (myBus != null && myBus.JobBus.Veh == vehicle)
                {
                    if (myBus.Player != null)
                    {
                        myBus.JobBus.ToRespawn();
                        API.sendChatMessageToPlayer(myBus.Player, "~y~[Server] ~w~Автобус уничтожен. ~r~Миссия провалена.");
                        API.sendChatMessageToPlayer(myBus.Player, "~y~[Server] ~w~Заработок за рейс ~r~0$~w~.");
                        JobBusYval(myBus.Player);
                    }
                }
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в onVehicleDeathHandler " + exp.ToString());
                ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onVehicleDeathHandler");
                return;
            }
        }

        private void onPlayerDeathHandler(Client player, NetHandle entityKiller, int weapon)
        {
            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
            switch ((Jobs)pInfo.Job)
            {
                case Jobs.JOB_BUS_ONE:
                case Jobs.JOB_BUS_TWO:
                case Jobs.JOB_BUS_THREE:
                case Jobs.JOB_BUS_FOUR:
                case Jobs.JOB_BUS_FIVE:
                case Jobs.JOB_BUS_SIX:
                case Jobs.JOB_BUS_SEVEN:
                    API.sendChatMessageToPlayer(player, "~y~[Server] ~w~Вы погибли. ~r~Миссия провалена.");
                    API.sendChatMessageToPlayer(player, "~y~[Server] ~w~Заработок за рейс ~r~0$~w~.");
                    JobBusYval(player);
                    break;
            }
        }

        private void onPlayerExitVehicle(Client player, NetHandle vehicle)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.Job != (int)Jobs.NULL)
                    {
                        var myBus = JobBusList.Where(i => i.Player == player).FirstOrDefault(); // Проверяем что пользователь вышел из своего автобуса
                        if (myBus != null && myBus.JobBus.Veh == vehicle)
                        {
                            API.sendChatMessageToPlayer(player, "~y~[Server] ~r~Вернитесь в автобус. Иначе миссия будет\r\n~r~завершена!");
                            API.sendChatMessageToPlayer(player, "~y~[Server] ~w~У вас есть ~r~15 с.~w~, чтобы вернуться в автобус.");

                            lifeRP_GM.mainClass.Stream(15000, () =>
                            {
                                if (!player.isInVehicle || API.getPlayerVehicle(player) != myBus.JobBus.Veh)
                                {
                                    API.sendChatMessageToPlayer(player, "~y~[Server] ~w~Вы не вернулись в автобус. ~r~Миссия провалена.");
                                    API.sendChatMessageToPlayer(player, "~y~[Server] ~w~Заработок за рейс ~r~0$~w~.");
                                    JobBusYval(player);
                                }
                            });
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                JobBusYval(player);
                API.consoleOutput("Произошла ошибка в onPlayerExitVehicle " + exp.ToString());
                ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onPlayerExitVehicle");
                return;
            }
        }

        private void onPlayerEnterVehicle(Client player, NetHandle vehicle)
        {
            try
            {
                if (API.shared.getEntitySyncedData(vehicle, Constants.VehJob) == (int)Jobs.JOB_BUS)
                {
                    var myBus = JobBusList.Where(i => i.Player == player).FirstOrDefault(); // Проверяем что пользователь сел в свой автобус
                    if (myBus != null && myBus.JobBus.Veh == vehicle)
                    {
                        API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Вы должны ехать по своему маршруту");
                        API.sendChatMessageToPlayer(player, "~b~и на остановках подбирать пассажиров.");
                        if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                        {
                            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                            PlayerInfo.PlEnt pe = pInfo.playerEntity.FirstOrDefault(x => x.Key == Constants.Blip && x.Value.Job == (Jobs)pInfo.Job);
                            if (pe.Key != null)
                            {
                                BlipInfo bi = pe.Value;
                                bi.DeleteBlip();
                            }
                            ProcessBusPoint(player);
                        }
                        return;
                    }
                    API.sendChatMessageToPlayer(player, "~y~[Server] ~r~У Вас нет доступа к данному транспортному\r\n~r~средству.");
                    API.warpPlayerOutOfVehicle(player);
                }

            }
            catch (Exception exp)
            {
                JobBusYval(player);
                API.consoleOutput("Произошла ошибка в onPlayerEnterVehicle " + exp.ToString());
                ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onPlayerEnterVehicle");
            }
        }

        private void onClientEvent(Client player, string eventName, params object[] arguments)
        {
            if (eventName == "JobBusStart")
            {
                PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                if (pInfo.Job != (int)Jobs.NULL)
                {
                    API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Сначала завершите предыдущую работу.");
                    return;
                }
            }

            switch (eventName)
            {
                case "Yval":
                    JobBusYval(player);
                    break;

                case "JobBusStart":
                    JobBusStart(player, int.Parse(arguments[0].ToString()));
                    break;
            }
        }

        private void StartMission(Client player, VehicleHash vech, Jobs job)
        {
            var myBus = JobBusList.Where(i => i.Player == null).FirstOrDefault(); // Ищем автобус который ни кому не принадлежит
            if (myBus == null)
            {
                API.sendChatMessageToPlayer(player, "~y~[Server] ~b~В данный момент все автобусы на маршруте.");
                return;
            }

            myBus = JobBusList.Where(i => i.Type == vech && i.Player == null).FirstOrDefault(); // Ищем автобус который ни кому не принадлежит по типу
            if (myBus != null)
            {
                myBus.JobBus.ToRespawn();
                myBus.JobBus.DoorUnLock();
                myBus.Player = player;
            }
            else
            {
                API.sendChatMessageToPlayer(player, "~y~[Server] ~b~В данный момент нет подходящего автобуса.");
                API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Выберите другой маршрут или дождитесь");
                API.sendChatMessageToPlayer(player, "~b~прибытия автобуса с маршрута.");
                return;
            }

            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
            pInfo.Job = (int)job;
            API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Вы устроились на работу водителем автобуса.");
            API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Ваш автобус ждёт вас рядом.");
            new BlipInfo(player, myBus.Coords, 1, 60, false, 0, "Начать работу", job);

            if (pInfo.sex)
            {   // Мужской персонаж
                API.setPlayerClothes(player, 11, 26, 1);
                API.setPlayerClothes(player, 4, 35, 0);
            }
            else
            {   // Женский персонаж

            }

            /*
             * было бы прикольно чтобы механик доставлял автобус остановку
            NetHandle myPed = API.createPed(PedHash.Abner, new Vector3(437.00, -624.30, 28.71), 0);
            API.setEntityPositionFrozen(myPed, false);
            API.sendNativeToAllPlayers(0xC20E50AA46D09CA8, myPed, myBus.JobBus.Veh, -1, 0, 1.0, 1, 0);
            API.sendNativeToAllPlayers(0xE2A2AA2F659D77A7, myPed, myBus.JobBus.Veh, 399.7879, -655.0015, 28.49626, 15, 0, vech, 2, 0, 0);
            */
        }

        private void ProcessBusPoint(Client player, int point = 0)
        {
            try
            {
                var myBus = JobBusList.Where(i => i.Player == player).FirstOrDefault();
                if (myBus != null)
                {
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    List<Tuple<Vector3, PointType>> MyBusRoute;
                    switch ((Jobs)pInfo.Job)
                    {
                        case Jobs.JOB_BUS_ONE:
                            MyBusRoute = BusRoutes[0];
                            break;

                        case Jobs.JOB_BUS_TWO:
                            MyBusRoute = BusRoutes[1];
                            break;

                        case Jobs.JOB_BUS_THREE:
                            MyBusRoute = BusRoutes[2];
                            break;

                        case Jobs.JOB_BUS_FOUR:
                            MyBusRoute = BusRoutes[3];
                            break;

                        case Jobs.JOB_BUS_FIVE:
                            MyBusRoute = BusRoutes[4];
                            break;

                        case Jobs.JOB_BUS_SIX:
                            MyBusRoute = BusRoutes[5];
                            break;

                        case Jobs.JOB_BUS_SEVEN:
                            MyBusRoute = BusRoutes[6];
                            break;

                        default:
                            MyBusRoute = BusRoutes[0];
                            break;
                    }
                    var MyBusPoint = MyBusRoute[point];
                    switch (MyBusPoint.Item2)
                    {
                        case PointType.BusPoint:
                            AddBusPoint(player, MyBusPoint.Item1, (Jobs)pInfo.Job, point);
                            break;

                        case PointType.BusStop:
                            AddBusStop(player, MyBusPoint.Item1, (Jobs)pInfo.Job, point);
                            break;

                        case PointType.BusLastStop:
                            AddBusLastStop(player, MyBusPoint.Item1, (Jobs)pInfo.Job, point);
                            break;

                        case PointType.BusGarage:
                            //AddBusGaragePoint(Point[1], Point[2], Point[3])
                            break;
                    }
                }
            }
            catch (Exception exp)
            {
                JobBusYval(player);
                API.consoleOutput("Произошла ошибка в ProcessBusPoint " + exp.ToString());
                ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в ProcessBusPoint");
            }
        }

        private void AddBusPoint(Client player, Vector3 pos, Jobs job, int point)
        {
            try
            {
                BlipInfo bInfo = new BlipInfo(player, pos, 1, 60, false, 0, "Точка маршрута", job, true, 60);
                MarkerInfo minfoBus = new MarkerInfo(player, 1, pos, new Vector3(), new Vector3(), new Vector3(1.5, 1.5, 2), 207, 32, 85, 32, 6, 2, job);
                minfoBus.SetData("BusJobPoint", ++point);
            }
            catch (Exception exp)
            {
                JobBusYval(player);
                API.consoleOutput("Произошла ошибка в AddBusPoint " + exp.ToString());
                ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в AddBusPoint");
            }
        }

        private void AddBusStop(Client player, Vector3 pos, Jobs job, int point)
        {
            try
            {
                var blip = new BlipInfo(player, pos, 1, 60, false, 0, "Остановка", job, true, 60);
                MarkerInfo minfoBus = new MarkerInfo(player, 1, pos, new Vector3(), new Vector3(), new Vector3(1.5, 1.5, 2), 219, 0, 50, 85, 6, 2, job);
                minfoBus.SetData("BusJobStop", ++point);
            }
            catch (Exception exp)
            {
                JobBusYval(player);
                API.consoleOutput("Произошла ошибка в AddBusStop " + exp.ToString());
                ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в AddBusStop");
            }
        }

        private void AddBusLastStop(Client player, Vector3 pos, Jobs job, int point)
        {
            try
            {
                var blip = new BlipInfo(player, pos, 1, 60, false, 0, "Конечная остановка", job, true, 60);
                MarkerInfo minfoBus = new MarkerInfo(player, 1, pos, new Vector3(), new Vector3(), new Vector3(1.5, 1.5, 2), 219, 0, 50, 85, 6, 2, job);
                minfoBus.SetData("BusJobLastStop", ++point);
            }
            catch (Exception exp)
            {
                JobBusYval(player);
                API.consoleOutput("Произошла ошибка в AddBusLastStop " + exp.ToString());
                ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в AddBusLastStop");
            }
        }

        public void JobBusStart(Client player, int Rout)
        {
            switch (Rout)
            {
                case 1:
                    StartMission(player, VehicleHash.Bus, Jobs.JOB_BUS_ONE);
                    break;

                case 2:
                    StartMission(player, VehicleHash.Bus, Jobs.JOB_BUS_TWO);
                    break;

                case 3:
                    StartMission(player, VehicleHash.Bus, Jobs.JOB_BUS_THREE);
                    break;

                case 4:
                    StartMission(player, VehicleHash.Coach, Jobs.JOB_BUS_FOUR);
                    break;

                case 5:
                    StartMission(player, VehicleHash.Bus, Jobs.JOB_BUS_FIVE);
                    break;

                case 6:
                    StartMission(player, VehicleHash.Coach, Jobs.JOB_BUS_SIX);
                    break;

                case 7:
                    StartMission(player, VehicleHash.Coach, Jobs.JOB_BUS_SEVEN);
                    break;
            }
        }

        private void onMarkerEnter(ColShape colshape, NetHandle entity)
        {
            if (colshape.hasData(Constants.MarkerClass))
            {
                MarkerInfo mInfo = colshape.getData(Constants.MarkerClass);
                Client player;

                int type = (int)API.getEntityType(entity);
                if (type == 6) player = API.getPlayerFromHandle(entity);
                else return;

                if (mInfo.local && mInfo.player != player) return;

                if (mInfo.HasData("JobBuzAction"))
                {
                    API.triggerClientEvent(player, "JobBus");
                    return;
                }
                else if (mInfo.HasData("BusJobPoint"))
                {
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    PlayerInfo.PlEnt pe = pInfo.playerEntity.FirstOrDefault(x => x.Key == Constants.Blip && x.Value.Job == (Jobs)pInfo.Job);
                    if (pe.Key != null)
                    {
                        BlipInfo bi = pe.Value;
                        bi.DeleteBlip();
                    }
                    ProcessBusPoint(player, mInfo.GetData("BusJobPoint"));
                    mInfo.DeleteMarker();
                }
                else if (mInfo.HasData("BusJobStop"))
                {
                    API.sendNotificationToPlayer(player, "~y~[Server] ~w~Ожидайте пассажиров 15 секунд.");
                    lifeRP_GM.mainClass.Stream(15000, () =>
                    {
                        if (player.position.DistanceTo(mInfo.pos) <= 5)
                        {
                            API.sendNotificationToPlayer(player, "~y~[Server] ~w~Вы можете ехать дальше.");
                            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                            PlayerInfo.PlEnt pe = pInfo.playerEntity.FirstOrDefault(x => x.Key == Constants.Blip && x.Value.Job == (Jobs)pInfo.Job);
                            if (pe.Key != null)
                            {
                                BlipInfo bi = pe.Value;
                                bi.DeleteBlip();
                            }
                            ProcessBusPoint(player, mInfo.GetData("BusJobStop"));
                            mInfo.DeleteMarker();
                        }
                        else if (!mInfo.HasData("BusJobAlreadyIn"))
                        {
                            mInfo.SetData("BusJobAlreadyIn", 1);
                            API.sendNotificationToPlayer(player, "~y~[Server] ~w~Вернитесь на остановку.");
                        }
                    });
                }
                else if (mInfo.HasData("BusJobLastStop"))
                {
                    API.sendNotificationToPlayer(player, "~y~[Server] ~w~Конечная! Ожидайте пока пассажиры покинут автобус.");
                    var myBus = JobBusList.Where(i => i.Player == player).FirstOrDefault();
                    if (myBus != null)
                    {
                        Client[] passengers = API.getVehicleOccupants(myBus.JobBus.Veh);
                        foreach (var passenger in passengers)
                        {
                            if (passenger != player)
                            {
                                API.warpPlayerOutOfVehicle(passenger);
                                API.sendChatMessageToPlayer(passenger, "~y~[Server] ~b~Конечная, просьба выйти всем из автобуса.");
                            }
                        }
                        myBus.Player = null;
                    }

                    lifeRP_GM.mainClass.Stream(15000, () =>
                    {
                        if (player.position.DistanceTo(mInfo.pos) <= 5)
                        {
                            API.sendNotificationToPlayer(player, "~y~[Server] ~w~Вы можете ехать дальше.");
                            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                            PlayerInfo.PlEnt pe = pInfo.playerEntity.FirstOrDefault(x => x.Key == Constants.Blip && x.Value.Job == (Jobs)pInfo.Job);
                            if (pe.Key != null)
                            {
                                BlipInfo bi = pe.Value;
                                bi.DeleteBlip();
                            }
                            ProcessBusPoint(player, mInfo.GetData("BusJobLastStop"));
                            mInfo.DeleteMarker();
                        }
                        else if (!mInfo.HasData("BusJobAlreadyIn"))
                        {
                            mInfo.SetData("BusJobAlreadyIn", 1);
                            API.sendNotificationToPlayer(player, "~y~[Server] ~w~Вернитесь на остановку.");
                        }
                    });
                }

            }
            else API.deleteColShape(colshape);
        }

        public void JobBusYval(Client player)//закончить рабочий день
        {
            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
            switch ((Jobs)pInfo.Job)
            {
                case Jobs.JOB_BUS_ONE:
                case Jobs.JOB_BUS_TWO:
                case Jobs.JOB_BUS_THREE:
                case Jobs.JOB_BUS_FOUR:
                case Jobs.JOB_BUS_FIVE:
                case Jobs.JOB_BUS_SIX:
                case Jobs.JOB_BUS_SEVEN:
                    /* 
                     * Можно сделать проверку что водитель проехал весь маршрут
                     *
                     */
                    var myBus = JobBusList.Where(i => i.Player == player).FirstOrDefault(); // Освобождаем автобус от пользователя
                    if (myBus != null)
                    {
                        Client[] passengers = API.getVehicleOccupants(myBus.JobBus.Veh);
                        foreach (var passenger in passengers)
                        {
                            if (passenger != player) // Чуть водителя не выгнал 
                            {
                                API.warpPlayerOutOfVehicle(passenger);
                                API.sendChatMessageToPlayer(passenger, "~y~[Server] ~b~Водитель закончил работу. Вам придется\r\n~b~добиратся самостоятельно ;)");
                            }
                        }
                        myBus.Player = null;
                    }

                    var Markers = pInfo.playerEntity.Where(x => x.Key == Constants.Marker); // Удаляем все пользовательские маркеры связанные с работой
                    foreach (var Marker in Markers.ToArray())
                    {
                        MarkerInfo bi = Marker.Value;
                        if (bi.Job == (Jobs)pInfo.Job)
                        {
                            bi.DeleteMarker();
                        }
                    }

                    var Blips = pInfo.playerEntity.Where(x => x.Key == Constants.Blip); // Удаляем все пользовательские блипы связанные с работой
                    foreach (var Blip in Blips.ToArray())
                    {
                        BlipInfo bi = Blip.Value;
                        if (bi.Job == (Jobs)pInfo.Job)
                        {
                            bi.DeleteBlip();
                        }
                    }

                    pInfo.Job = (int)Jobs.NULL;
                    API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Вы закончили рабочий день.");
                    API.clearPlayerAccessory(player, 0); // удаляем аксесуары с головы
                    pInfo.UpdateDress(); // меняем одежду на пользовательскую
                    break;

                default:
                    API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Вы здесь не работаете.");
                    break;
            }
        }
    }
}
