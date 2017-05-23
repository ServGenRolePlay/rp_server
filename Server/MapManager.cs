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
    public class lifeRP_MM : Script
    {
        public void CreateBlipList()
        {            
            return; // Это все нано проверять но мне лень >_<
            new BlipInfo(new Vector3(-766.22, -2061.01, 9.02), 269, 77); // автошкола 1
            new BlipInfo(new Vector3(-64.17, -1449.79, 32.52), 374, 2); // квартира до 500000 2
            new BlipInfo(new Vector3(-46.10, -1445.92, 32.43), 374, 2); // квартира до 500000 3
            new BlipInfo(new Vector3(-525.22, -1211.40, 18.18), 361, 17); // бензозаправка 4
            new BlipInfo(new Vector3(-1141.75, -1992.42, 13.16), 446, 72);  // мастерская тюнинг 5
            new BlipInfo(new Vector3(2683.24, 3281.95, 55.24), 52, 38); // бизнес 6
            new BlipInfo(new Vector3(236.73, -408.09, 47.92), 419, 0, false); // мэрия 7
            new BlipInfo(new Vector3(265.31, -1263.11, 29.29), 361, 17); // бензозаправка 8
            new BlipInfo(new Vector3(-70.26, -1760.11, 29.53), 361, 17); // бензозаправка 9
            new BlipInfo(new Vector3(2581.36, 361.83, 108.47), 361, 17); // бензозаправка 10
            new BlipInfo(new Vector3(1703.65, 6416.82, 32.64), 361, 17); // бензозаправка 11
            new BlipInfo(new Vector3(179.60, 6602.79, 31.87), 361, 17); // бензозаправка 12
            new BlipInfo(new Vector3(-2555.23, 2334.59, 33.08), 361, 17); // бензозаправка 13
            new BlipInfo(new Vector3(-2096.92, -321.52, 13.17), 361, 17); // бензозаправка 14
            new BlipInfo(new Vector3(1846.48, 2585.97, 45.67), 285, 52); // тюрьма чилиада 15
            new BlipInfo(new Vector3(-960.62, -1982.74, 14.48), 1, 46);// трудоустройство с мэрии без мэрии 16
            new BlipInfo(new Vector3(1018.94, -2511.59, 28.4805), 1, 46); // трудоустройство с мэрии без мэрии 17
            new BlipInfo(new Vector3(-1078.10, -2137.69, 13.3924), 1, 46); // трудоустройство с мэрии без мэрии 18
            new BlipInfo(new Vector3(346.334, 3406.29, 35.5023), 67, 46); // работу водителя дальнобойщика ??????????????????? 19
            new BlipInfo(new Vector3(-2193.28, 4286.4, 49.1751), 67, 46); // работу водителя дальнобойщика ??????????????????? 20
            new BlipInfo(new Vector3(-329.141, -2715.66, 11.4946), 67, 46); // работу водителя дальнобойщика ????????????????? 21            
            new BlipInfo(new Vector3(432.36, -981.31, 30.71), 60, 38, false);// Police 23 
            new BlipInfo(new Vector3(124.39, -1929.71, 21.38), 206, 25); // Grove Street Families 24
            new BlipInfo(new Vector3(-561.85, 294.62, 87.49), 68, 37); // работа эвакуаторщика 25
            new BlipInfo(new Vector3(806.8278, -809.9581, 26.2029), 318, 46); // работа мусоровоза черной машины 26
            new BlipInfo(new Vector3(825.80, -1290.23, 28.24), 60, 38); // Police 27
            new BlipInfo(new Vector3(175.93, -1562.29, 29.26), 361, 17); // бензозаправка 28
            new BlipInfo(new Vector3(-817.25, -1079.97, 11.13), 73, 3); // магазин одежды 29
            new BlipInfo(new Vector3(364.79, -2064.46, 21.74), 106, 46); // банда вагос 30
            new BlipInfo(new Vector3(1184.56, -1620.50, 44.85), 76, 53); // банда ацтеки 31
            new BlipInfo(new Vector3(-319.53, -1471.50, 30.55), 361, 17); // бензозаправка 32
            new BlipInfo(new Vector3(-724.13, -933.71, 19.21), 361, 17); // бензозаправка 33
            new BlipInfo(new Vector3(-711.69, -917.53, 19.21), 52, 38); // бизнес 34
            new BlipInfo(new Vector3(-2974.46, 390.77, 15.03), 52, 38); // бизнес 35
            new BlipInfo(new Vector3(-2967.87, 482.93, 15.47), 277, 25); // банк 36
            new BlipInfo(new Vector3(-3238.32, 1004.35, 12.46), 52, 38); // бизнес 37
            new BlipInfo(new Vector3(118.36, -1920.85, 21.32), 374, 2); // квартира до 500000 38
            new BlipInfo(new Vector3(101.00, -1912.38, 21.41), 374, 2); // квартира до 500000 39
            new BlipInfo(new Vector3(114.29, -1961.10, 21.33), 374, 2); // квартира до 500000 40
            new BlipInfo(new Vector3(85.57, -1959.33, 21.12), 374, 2); // квартира до 500000 41
            new BlipInfo(new Vector3(76.72, -1948.38, 21.17), 374, 2);  // квартира до 500000 42
            new BlipInfo(new Vector3(72.19, -1938.89, 21.37), 374, 2); // квартира до 500000 43
            new BlipInfo(new Vector3(56.61, -1922.70, 21.91), 374, 2); // квартира до 500000 44
            new BlipInfo(new Vector3(39.25, -1911.96, 21.95), 374, 2); // квартира до 500000 45
            new BlipInfo(new Vector3(23.43, -1896.37, 22.97), 374, 2); // квартира до 500000 46
            new BlipInfo(new Vector3(5.05, -1884.02, 23.70), 374, 2); // квартира до 500000 47
            new BlipInfo(new Vector3(-5.01, -1872.05, 24.15), 374, 2); // квартира до 500000 48
            new BlipInfo(new Vector3(-20.68, -1858.80, 25.41), 374, 2); // квартира до 500000 49
            new BlipInfo(new Vector3(-33.92, -1847.01, 26.19), 374, 2); // квартира до 500000 50
            new BlipInfo(new Vector3(21.07, -1844.63, 24.60), 374, 2); // квартира до 500000 51
            new BlipInfo(new Vector3(29.76, -1854.64, 24.07), 374, 2); // квартира до 500000 52
            new BlipInfo(new Vector3(45.92, -1864.26, 23.28), 374, 2); // квартира до 500000	53		
            new BlipInfo(new Vector3(54.41, -1873.02, 22.81), 374, 2); // квартира до 500000	54		
            new BlipInfo(new Vector3(332.20, -2018.76, 22.35), 374, 2); // квартира до 500000	55		
            new BlipInfo(new Vector3(335.91, -2021.91, 22.35), 374, 2); // квартира до 500000	56		
            new BlipInfo(new Vector3(343.36, -2027.85, 22.35), 374, 2); // квартира до 500000	57		
            new BlipInfo(new Vector3(344.79, -2028.87, 22.35), 374, 2); // квартира до 500000	58		
            new BlipInfo(new Vector3(351.91, -2035.39, 22.35), 374, 2); // квартира до 500000 59
            new BlipInfo(new Vector3(353.31, -2036.57, 22.35), 374, 2); // квартира до 500000 60
            new BlipInfo(new Vector3(360.54, -2042.58, 22.35), 374, 2); // квартира до 500000		61	
            new BlipInfo(new Vector3(364.23, -2045.67, 22.35), 374, 2); // квартира до 500000 62
            new BlipInfo(new Vector3(372.14, -2055.82, 21.74), 374, 2); // квартира до 500000 63
            new BlipInfo(new Vector3(371.09, -2057.11, 21.74), 374, 2); // квартира до 500000 64
            new BlipInfo(new Vector3(357.88, -2073.30, 21.74), 374, 2); // квартира до 50000 65
            new BlipInfo(new Vector3(356.69, -2074.62, 21.74), 374, 2); // квартира до 500000	66		
            new BlipInfo(new Vector3(345.72, -2067.24, 20.94), 374, 2); // квартира до 500000		67	
            new BlipInfo(new Vector3(342.03, -2064.25, 20.94), 374, 2); // квартира до 500000		68	
            new BlipInfo(new Vector3(334.66, -2058.12, 20.94), 374, 2); // квартира до 500000		69	
            new BlipInfo(new Vector3(325.81, -2050.95, 20.94), 374, 2); // квартира до 500000		70	
            new BlipInfo(new Vector3(324.52, -2049.66, 20.94), 374, 2); // квартира до 500000		71	
            new BlipInfo(new Vector3(317.09, -2043.59, 20.94), 374, 2); // квартира до 500000 72
            new BlipInfo(new Vector3(313.55, -2040.10, 20.94), 374, 2); // квартира до 500000		73	
            new BlipInfo(new Vector3(298.22, -2034.41, 19.84), 374, 2);// квартира до 500000			74
            new BlipInfo(new Vector3(236.24, -2046.23, 18.38), 374, 2); // квартира до 500000 75
            new BlipInfo(new Vector3(251.18, -2030.18, 18.71), 374, 2); // квартира до 500000 76
            new BlipInfo(new Vector3(256.76, -2023.73, 19.27), 374, 2); // квартира до 500000 77
            new BlipInfo(new Vector3(279.98, -1993.85, 20.80), 374, 2); // квартира до 500000 79
            new BlipInfo(new Vector3(295.89, -1972.08, 22.90), 374, 2); // квартира до 500000		80	
            new BlipInfo(new Vector3(312.04, -1956.23, 24.62), 374, 2); // квартира до 500000			81
            new BlipInfo(new Vector3(1193.32, -1656.27, 43.03), 374, 2); // квартира до 500000		82	
            new BlipInfo(new Vector3(1193.28, -1622.77, 45.22), 374, 2); // квартира до 500000		83	
            new BlipInfo(new Vector3(1214.24, -1644.11, 48.65), 374, 2); // квартира до 500000		84	
            new BlipInfo(new Vector3(1210.64, -1607.01, 50.73), 374, 2); // квартира до 500000 85
            new BlipInfo(new Vector3(1245.09, -1626.63, 53.28), 374, 2); // квартира до 500000 86
            new BlipInfo(new Vector3(1230.70, -1590.98, 53.77), 374, 2); // квартира до 500000		87	
            new BlipInfo(new Vector3(1261.42, -1616.49, 54.74), 374, 2); // квартира до 500000	88		
            new BlipInfo(new Vector3(1286.57, -1604.22, 54.82), 374, 2); // квартира до 500000	89		
            new BlipInfo(new Vector3(1333.91, -1566.25, 54.45), 374, 2); // квартира до 500000 90
            new BlipInfo(new Vector3(1327.50, -1553.15, 54.05), 374, 2); // квартира до 500000	91		
            new BlipInfo(new Vector3(1315.89, -1526.46, 51.81), 374, 2); // квартира до 500000	92		
            new BlipInfo(new Vector3(-3089.13, 221.41, 14.07), 374, 2);// квартира до 500000		94	
            new BlipInfo(new Vector3(-3116.70, 242.48, 12.49), 374, 2); // квартира до 500000	95
            new BlipInfo(new Vector3(-3115.79, 294.18, 8.97), 374, 2); // квартира до 500000		96	
            new BlipInfo(new Vector3(-3108.74, 303.40, 8.38), 374, 2); // квартира до 500000	97
            new BlipInfo(new Vector3(-3111.70, 315.60, 8.38), 374, 2); // квартира до 500000	98	
            new BlipInfo(new Vector3(-3110.63, 335.12, 7.49), 374, 2);// квартира до 500000	99		
            new BlipInfo(new Vector3(-3093.29, 349.00, 7.54), 374, 2);// квартира до 500000 100
            new BlipInfo(new Vector3(-3091.48, 379.32, 7.11), 374, 2); // квартира до 500000	 101		
            new BlipInfo(new Vector3(-3081.09, 405.36, 6.97), 374, 2); // квартира до 500000	102		
            new BlipInfo(new Vector3(-3071.24, 442.70, 6.36), 374, 2); // квартира до 500000 103
            new BlipInfo(new Vector3(-3053.24, 486.72, 6.78), 374, 2); // квартира до 500000		104	
            new BlipInfo(new Vector3(-3038.60, 492.61, 6.77), 374, 2); // квартира до 50000			105
            new BlipInfo(new Vector3(-3031.30, 524.94, 7.41), 374, 2); // квартира до 500000			106
            new BlipInfo(new Vector3(-3036.94, 544.69, 7.51), 374, 2); // квартира до 500000			107
            new BlipInfo(new Vector3(-3036.89, 559.22, 7.51), 374, 2); // квартира до 500000			107
            new BlipInfo(new Vector3(-3029.60, 568.67, 7.82), 374, 2); // квартира до 500000			109
            new BlipInfo(new Vector3(-3077.85, 659.01, 11.67), 374, 2); // квартира до 500000			110
            new BlipInfo(new Vector3(-3107.66, 718.93, 20.65), 374, 2); // квартира до 500000			111
            new BlipInfo(new Vector3(-3101.47, 743.78, 21.28), 374, 2); // квартира до 500000	112
            new BlipInfo(new Vector3(-3017.80, 746.27, 27.59), 374, 2); // квартира до 500000		113
            new BlipInfo(new Vector3(-2992.73, 707.32, 28.50), 374, 2); // квартира до 500000			114
            new BlipInfo(new Vector3(-2990.33, 695.66, 27.78), 374, 2); // квартира до 500000			115
            new BlipInfo(new Vector3(-2994.49, 682.51, 25.04), 374, 2); // квартира до 500000			116
            new BlipInfo(new Vector3(-2973.03, 642.29, 25.80), 374, 2); // квартира до 500000			117
            new BlipInfo(new Vector3(-2977.56, 609.30, 20.24), 374, 2);  // квартира до 500000		118
            new BlipInfo(new Vector3(-3225.20, 910.98, 13.99), 374, 2); // квартира до 500000			119
            new BlipInfo(new Vector3(-3228.49, 927.52, 13.97), 374, 2); // квартира до 500000			120
            new BlipInfo(new Vector3(-3232.23, 934.95, 13.80), 374, 2); // квартира до 500000		121
            new BlipInfo(new Vector3(-3237.49, 952.79, 13.14), 374, 2);  // квартира до 500000		122
            new BlipInfo(new Vector3(-3250.91, 1027.32, 11.76), 374, 2); // квартира до 500000			123
            new BlipInfo(new Vector3(-379.92, 6118.44, 31.85), 436, 1);// пожарная часть N*5			124
            new BlipInfo(new Vector3(105.13, 6613.66, 32.40), 446, 85);// мастерская тюнинг			125
            new BlipInfo(new Vector3(399.529, 6605.65, 27.9767), 1, 28); // трудоустройство с мэрии без мэрии 126			
            new BlipInfo(new Vector3(419.612, 6480.21, 28.5473), 1, 28); // трудоустройство с мэрии без мэрии	127		
            new BlipInfo(new Vector3(408.78, 6489.23, 27.7544), 1, 28); // трудоустройство с мэрии без мэрии			128
            new BlipInfo(new Vector3(2238.8, 5155.52, 57.2059), 1, 28); // трудоустройство с мэрии без мэрии			129
            new BlipInfo(new Vector3(2023.4, 4976.26, 40.9668), 1, 28); // трудоустройство с мэрии без мэрии			130
            new BlipInfo(new Vector3(2412.32, 4990.01, 45.9626), 1, 28); // трудоустройство с мэрии без мэрии	131
            new BlipInfo(new Vector3(2563.38, 4685.75, 33.8527), 1, 28); // трудоустройство с мэрии без мэрии		132	
            new BlipInfo(new Vector3(2846.93, 4561.82, 46.582), 1, 28); // трудоустройство с мэрии без мэрии		133
            new BlipInfo(new Vector3(-83.3971, 1879.98, 197.013), 1, 28);// трудоустройство с мэрии без мэрии		134
            new BlipInfo(new Vector3(-1919.59, 2052.85, 140.473), 1, 28); // трудоустройство с мэрии без мэрии			135
            new BlipInfo(new Vector3(-830.39, 115.10, 56.03), 350, 2);// квартира выше 500000			136
            new BlipInfo(new Vector3(-913.48, 108.36, 55.51), 350, 2); // квартира выше 500000			137
            new BlipInfo(new Vector3(-971.26, 122.27, 57.05), 350, 2); // квартира выше 500000			138
            new BlipInfo(new Vector3(-998.20, 157.97, 62.32), 350, 2); // квартира выше 500000		139
            new BlipInfo(new Vector3(-949.25, 196.80, 67.39), 350, 2); // квартира выше 500000			140
            new BlipInfo(new Vector3(-842.14, -25.16, 40.40), 350, 2); // квартира выше 500000			141
            new BlipInfo(new Vector3(-896.70, -5.13, 43.80), 350, 2); // квартира выше 500000			142
            new BlipInfo(new Vector3(-888.22, 42.68, 49.15), 350, 2); // квартира выше 500000			143
            new BlipInfo(new Vector3(-930.32, 19.49, 48.53), 350, 2); // квартира выше 500000	144
            new BlipInfo(new Vector3(-902.33, 191.56, 69.45), 350, 2);  // квартира выше 500000	145		
            new BlipInfo(new Vector3(-1038.17, 222.26, 64.38), 350, 2); // квартира выше 500000		146
            new BlipInfo(new Vector3(-1038.42, 312.05, 67.27), 350, 2); // квартира выше 500000			147
            new BlipInfo(new Vector3(-1026.22, 360.56, 71.36), 350, 2); // квартира до 500000 148
            new BlipInfo(new Vector3(-1075.32, -1645.02, 4.50), 106, 27);  // банда Баллас 149
            new BlipInfo(new Vector3(-1134.77, -1568.84, 4.41), 374, 2); // квартира до 500000 150	
            new BlipInfo(new Vector3(-1114.85, -1577.59, 3.54), 374, 2); // квартира до 500000	151
            new BlipInfo(new Vector3(-1122.60, -1557.79, 4.10), 374, 2); // квартира до 500000		152
            new BlipInfo(new Vector3(-1114.19, -1579.70, 7.68), 374, 2); // квартира до 500000			153
            new BlipInfo(new Vector3(-1112.43, -1578.27, 7.68), 374, 2);// квартира до 500000			154
            new BlipInfo(new Vector3(-1155.89, -1574.62, 7.35), 374, 2); // квартира до 500000		155
            new BlipInfo(new Vector3(-1108.32, -1527.17, 5.78), 374, 2); // квартира до 500000			156
            new BlipInfo(new Vector3(-1084.81, -1558.73, 3.50), 374, 2); // квартира до 500000			157
            new BlipInfo(new Vector3(-1077.04, -1554.07, 3.63), 374, 2); // квартира до 500000			158
            new BlipInfo(new Vector3(-1087.51, -1529.21, 3.70), 374, 2); // квартира до 500000			159
            new BlipInfo(new Vector3(-1065.84, -1545.75, 3.90), 374, 2); // квартира до 500000			160
            new BlipInfo(new Vector3(-1078.28, -1524.26, 3.89), 374, 2); // квартира до 500000			161
            new BlipInfo(new Vector3(-1057.88, -1540.38, 4.05), 374, 2); // квартира до 500000			162
            new BlipInfo(new Vector3(-1070.16, -1514.99, 4.11), 374, 2); // квартира до 500000			163
            new BlipInfo(new Vector3(-1125.71, -1544.04, 4.38), 374, 2); // квартира до 500000			164
            new BlipInfo(new Vector3(35.44, 6663.18, 32.19), 374, 2); // квартира до 500000		165
            new BlipInfo(new Vector3(-9.60, 6654.22, 30.70), 374, 2); // квартира до 500000		166
            new BlipInfo(new Vector3(-41.66, 6637.33, 30.09), 374, 2); // квартира до 500000		167
            new BlipInfo(new Vector3(-130.44, 6551.60, 28.52), 374, 2);  // квартира до 500000		168
            new BlipInfo(new Vector3(-214.84, 6444.45, 31.31), 374, 2); // квартира до 500000			169
            new BlipInfo(new Vector3(-238.28, 6423.57, 27.47), 374, 2); // квартира до 500000			170
            new BlipInfo(new Vector3(-272.38, 6400.85, 29.50), 374, 2); // квартира до 500000		171
            new BlipInfo(new Vector3(2004.98, 3774.12, 32.40), 361, 17); // бензозаправка			172
            new BlipInfo(new Vector3(1965.57, 3740.12, 32.33), 52, 38); // бизнес			173
            new BlipInfo(new Vector3(1934.14, 3724.66, 32.80), 71, 38); // бизнес			173
            new BlipInfo(new Vector3(1840.24, 3673.78, 34.28), 61, 1); // больница номер N*5 174		
            new BlipInfo(new Vector3(1855.32, 3683.23, 32.27), 60, 38);// Police Sheriff 175			
            new BlipInfo(new Vector3(-2353.10, 3257.40, 32.81), 421, 38); // Армия Добро пожаловать на службу 176
            new BlipInfo(new Vector3(26.19, -1221.70, 29.29), 280, 49); // Спавно новичков 177
            new BlipInfo(new Vector3(213.89, -808.49, 31.01), 198, 46); // Таксопарк 1 178
            new BlipInfo(new Vector3(101.13, -1073.60, 28.37), 198, 46); // Таксопарк 2 179
            new BlipInfo(new Vector3(-38.68, -1109.37, 25.44), 434, 46); //Магазин новых машин 180
            new BlipInfo(new Vector3(1385.72, -593.13, 74.49), 181, 46); // Русская мафия 181 432.36
        }

        public void CreatMarkersList()
        {            
            return; // Это все нано проверять но мне лень >_<
            new MarkerInfo(0, new Vector3(1529.46, 3778.76, 34.51), new Vector3(1, 1, 1), new Vector3(1, 1, 1), new Vector3(0, 0, 0), 100, 1, 1, 1, 1, 2);
            new MarkerInfo(1, new Vector3(-766.22, -2061.01, 9.02), new Vector3(10, 10, 10), new Vector3(10, 10, 10), new Vector3(10, 10, 10), 100, 1, 1, 1, 1, 2);
            new MarkerInfo(1, new Vector3(-379.92, 6118.44, 28.85), new Vector3(1, 1, 1), new Vector3(36, 1, 1), new Vector3(1.5, 1.5, 5), 100, 237, 36, 36, 1, 2);
            new MarkerInfo(1, new Vector3(1840.24, 3673.78, 32.28), new Vector3(1, 1, 1), new Vector3(36, 1, 1), new Vector3(1.5, 1.5, 5), 100, 237, 36, 36, 1, 2);
            new MarkerInfo(1, new Vector3(1855.32, 3683.23, 32.27), new Vector3(1, 1, 1), new Vector3(36, 1, 1), new Vector3(1.5, 1.5, 5), 100, 237, 36, 36, 1, 2);
            new MarkerInfo(1, new Vector3(399.529, 6605.65, 25.9767), new Vector3(1, 1, 1), new Vector3(36, 1, 1), new Vector3(1.5, 1.5, 5), 100, 237, 36, 36, 1, 2);
            new MarkerInfo(1, new Vector3(-1026.22, 360.56, 69.36), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 5), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-1038.42, 312.05, 67.27), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 5), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-1038.17, 222.26, 63.38), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 5), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(2, new Vector3(-1038.17, 222.26, 64.38), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1, 1, 1), 100, 237, 36, 36, 1, 2);
            new MarkerInfo(0, new Vector3(-1075.32, -1645.02, 4.50), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1, 1, 1), 100, 237, 36, 36, 1, 2);
            new MarkerInfo(1, new Vector3(-1134.77, -1568.84, 3.41), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-1114.85, -1577.59, 3.54), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-1122.60, -1557.79, 4.10), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-1114.19, -1579.70, 7.68), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-1112.43, -1578.27, 7.68), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-1155.89, -1574.62, 7.35), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-1108.32, -1527.17, 5.78), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-1084.81, -1558.73, 3.50), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-1077.04, -1554.07, 3.63), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-1087.51, -1529.21, 3.70), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-1065.84, -1545.75, 3.90), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-1078.28, -1524.26, 3.89), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-1057.88, -1540.38, 4.05), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-1070.16, -1514.99, 4.11), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-1125.71, -1544.04, 4.38), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(35.44, 6663.18, 32.19), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-9.60, 6654.22, 30.70), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-41.66, 6637.33, 30.09), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-130.44, 6551.60, 28.52), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-214.84, 6444.45, 31.31), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-238.28, 6423.57, 27.47), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-272.38, 6400.85, 29.50), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(1934.14, 3724.66, 30.80), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(1965.57, 3740.12, 30.33), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
            new MarkerInfo(1, new Vector3(-2353.10, 3257.40, 31.81), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
          
        }

        public void CreatetLabelList()
        {
            return; // Это все нано проверять но мне лень >_<
            new TLabelInfo("Автошкола", new Vector3(-766.22, -2061.01, 9.02), 25, 1);
            new TLabelInfo("Бензозаправка", new Vector3(-525.22, -1211.40, 18.18), 25, 1);
            new TLabelInfo("Бензозаправка", new Vector3(2004.98, 3774.12, 32.40), 25, 1);
            new TLabelInfo("бензозаправка", new Vector3(175.93, -1562.29, 29.26), 25, 1);
            new TLabelInfo("Бензозаправка", new Vector3(-319.53, -1471.50, 30.55), 25, 1);
            new TLabelInfo("Бензозаправка", new Vector3(-724.13, -933.71, 19.21), 25, 1);
            new TLabelInfo("Мастерская", new Vector3(-1141.75, -1992.42, 13.16), 25, 1);
            new TLabelInfo("Мастерская", new Vector3(105.13, 6613.66, 32.40), 25, 1);
            new TLabelInfo("Магазин одежды", new Vector3(-817.25, -1079.97, 11.13), 25, 1);
            new TLabelInfo("Бизнес", new Vector3(2683.24, 3281.95, 55.24), 25, 1);
            new TLabelInfo("Бизнес", new Vector3(-711.69, -917.53, 19.21), 25, 1);
            new TLabelInfo("Бизнес", new Vector3(1934.14, 3724.66, 32.80), 25, 1);
            new TLabelInfo("Бизнес", new Vector3(1965.57, 3740.12, 32.33), 25, 1);
            new TLabelInfo("Мэрия", new Vector3(236.73, -408.09, 47.92), 25, 1);
            new TLabelInfo("Армия. Добро пожаловать на службу!", new Vector3(236.73, -408.09, 47.92), 25, 1);
            new TLabelInfo("Тюрьма чилиада", new Vector3(1846.48, 2585.97, 45.67), 25, 1);
            new TLabelInfo("Развозчик продуктов", new Vector3(-960.62, -1982.74, 14.48), 25, 1);
            new TLabelInfo("Развозчик продуктов по магазинам 3 уровня", new Vector3(1018.94, -2511.59, 28.4805), 25, 1);
            new TLabelInfo("Развозчик продуктов  2 уровня", new Vector3(-1078.10, -2137.69, 13.3924), 25, 1);            
            new TLabelInfo("Los Santos Police Department", new Vector3(432.36, -981.31, 30.7), 25, 1);
            new TLabelInfo("Grove Street Families", new Vector3(124.39, -1929.71, 21.38), 25, 1);
            new TLabelInfo("Банда вагос", new Vector3(364.79, -2064.46, 21.74), 25, 1);
            new TLabelInfo("Банда ацтеки", new Vector3(1184.56, -1620.50, 44.85), 25, 1);
            new TLabelInfo("Банда баллас", new Vector3(-1075.32, -1645.02, 4.50), 25, 1);
            new TLabelInfo("Работа эвакуаторщика", new Vector3(-561.85, 294.62, 87.49), 25, 1);
            new TLabelInfo("Работа мусоровоза черной машины", new Vector3(-561.85, 294.62, 87.49), 25, 1);
            new TLabelInfo("Police Department", new Vector3(825.80, -1290.23, 28.24), 25, 1);
            new TLabelInfo("Police Sheriff", new Vector3(1855.32, 3683.23, 32.27), 25, 1);
            new TLabelInfo("Пожарная часть N*5", new Vector3(-379.92, 6118.44, 31.85), 25, 1);
            new TLabelInfo("Больница номер N*5", new Vector3(1840.24, 3673.78, 34.28), 25, 1);
            new TLabelInfo("Ферма N*1", new Vector3(399.529, 6605.65, 27.9767), 25, 1);
            new TLabelInfo("Ферма N*2", new Vector3(419.612, 6480.21, 28.5473), 25, 1);
            new TLabelInfo("Ферма N*3", new Vector3(408.78, 6489.23, 27.7544), 25, 1);
            new TLabelInfo("Ферма N*4", new Vector3(2238.8, 5155.52, 57.2059), 25, 1);
            new TLabelInfo("Ферма N*5", new Vector3(2023.4, 4976.26, 40.9668), 25, 1);
            new TLabelInfo("Ферма N*6", new Vector3(2412.32, 4990.01, 45.9626), 25, 1);
            new TLabelInfo("Ферма N*7", new Vector3(2563.38, 4685.75, 33.8527), 25, 1);
            new TLabelInfo("Ферма N*8", new Vector3(2846.93, 4561.82, 46.5824), 25, 1);
            new TLabelInfo("Ферма N*9", new Vector3(-83.3971, 1879.98, 197.013), 25, 1);
            new TLabelInfo("Ферма N*10", new Vector3(-1919.59, 2052.85, 140.473), 25, 1);
        }
        
        public void CreateVehList()
        {
            return; // Это все нано проверять но мне лень >_<
            //new VehInfo(VehicleHash.Asterope, new Vector3(51.38, -1255.23, 29.33), new Vector3(0, 0, 314.14), 39, 39, 100, Jobs.NULL, 40);
            //new VehInfo(VehicleHash.T20, new Vector3(59.74, -1255.78, 29.35), new Vector3(0, 0, 314.14), 39, 39, 100, Jobs.NULL, 40);
            //new VehInfo(VehicleHash.Asterope, new Vector3(68.34, -1256.56, 29.33), new Vector3(0, 0, 314.14), 39, 39, 100, Jobs.NULL, 40);


            new VehInfo(VehicleHash.Asterope, new Vector3(-758.3881, -2062.54, 8.4095), new Vector3(0, 0, 314.14), 39, 39, 100);
            new VehInfo(VehicleHash.Asterope, new Vector3(-760.8437, -2060.09, 8.4082), new Vector3(0, 0, 314.14), 39, 39, 100);
            new VehInfo(VehicleHash.Asterope, new Vector3(-765.7252, -2055.18, 8.4058), new Vector3(0, 0, 314.14), 39, 39, 100);
            new VehInfo(VehicleHash.Asterope, new Vector3(-765.7252, -2055.18, 8.4058), new Vector3(0, 0, 314.14), 39, 39, 100);
            new VehInfo(VehicleHash.Asterope, new Vector3(-768.1755, -2052.83, 8.4042), new Vector3(0, 0, 314.14), 39, 39, 100);
            new VehInfo(VehicleHash.Asterope, new Vector3(-770.5789, -2050.40, 8.4014), new Vector3(0, 0, 314.14), 39, 39, 100);
            new VehInfo(VehicleHash.Asterope, new Vector3(-773.0509, -2047.96, 8.4017), new Vector3(0, 0, 314.14), 39, 39, 100);
            new VehInfo(VehicleHash.Asterope, new Vector3(-763.2760, -2057.53, 8.4074), new Vector3(0, 0, 314.14), 39, 39, 100);
            new VehInfo(VehicleHash.Asterope, new Vector3(-775.4725, -2045.56, 8.3992), new Vector3(0, 0, 314.14), 39, 39, 100);
            new VehInfo(VehicleHash.Speedo, new Vector3(-780.15, -2040.56, 8.62), new Vector3(0, 0, 314.14), 39, 39, 100);
            new VehInfo(VehicleHash.Speedo, new Vector3(-777.6793, -2043.0018, 8.6225), new Vector3(0, 0, 314.14), 39, 39, 100);
            new VehInfo(VehicleHash.Nemesis, new Vector3(-728.0934, -2054.0522, 8.4440), new Vector3(0, 0, 139.22), 39, 39, 100);
            new VehInfo(VehicleHash.Nemesis, new Vector3(-725.5465, -2056.4389, 8.4322), new Vector3(0, 0, 139.22), 39, 39, 100);
            new VehInfo(VehicleHash.PCJ, new Vector3(-723.0559, -2058.7998, 8.3830), new Vector3(0, 0, 136.33), 39, 39, 100);
            new VehInfo(VehicleHash.PCJ, new Vector3(-720.6233, -2061.0878, 8.3732), new Vector3(0, 0, 136.33), 39, 39, 100);
            new VehInfo(VehicleHash.PCJ, new Vector3(-717.9270, -2063.3925, 8.3719), new Vector3(0, 0, 136.33), 39, 39, 100);
            new VehInfo(VehicleHash.Mule, new Vector3(-743.0601, -2033.6281, 9.1442), new Vector3(0, 0, 89.474), 39, 39, 100);
            new VehInfo(VehicleHash.Mule, new Vector3(-742.8992, -2030.1220, 9.1387), new Vector3(0, 0, 89.474), 39, 39, 100);
            new VehInfo(VehicleHash.Pounder, new Vector3(-731.8133, -1969.4718, 8.9410), new Vector3(0, 0, 101.93), 39, 39, 100);
            new VehInfo(VehicleHash.Pounder, new Vector3(-731.1399, -1972.9144, 8.9574), new Vector3(0, 0, 101.93), 39, 39, 100);
            new VehInfo(VehicleHash.Hauler, new Vector3(-727.5502, -1979.09, 9.1014), new Vector3(0, 0, 102.65), 39, 39, 100);
            new VehInfo(VehicleHash.Hauler, new Vector3(-726.8465, -1982.3297, 9.1020), new Vector3(0, 0, 102.65), 39, 39, 100);
            new VehInfo(VehicleHash.Phantom, new Vector3(-745.8092, -1976.73, 8.9358), new Vector3(0, 0, 201.68), 39, 39, 100);
            new VehInfo(VehicleHash.Phantom, new Vector3(-744.0770, -1970.26, 8.9346), new Vector3(0, 0, 201.68), 39, 39, 100);
            new VehInfo(VehicleHash.Packer, new Vector3(-725.0015, -1988.98, 8.9539), new Vector3(0, 0, 102.65), 39, 39, 100);
            new VehInfo(VehicleHash.Packer, new Vector3(-724.2239, -1992.5046, 8.9540), new Vector3(0, 0, 102.65), 39, 39, 100);
            new VehInfo(VehicleHash.Tractor, new Vector3(-714.7003, -2032.44, 8.4200), new Vector3(0, 0, 107.65), 39, 39, 100);

            new VehInfo(VehicleHash.Police3, new Vector3(285.39, -424.72, 43.87), new Vector3(0, 0, 1.76), 72, 72, 100);
            new VehInfo(VehicleHash.Windsor2, new Vector3(284.83, -417.56, 43.98), new Vector3(0, 0, 8.46), 72, 72, 100);
            new VehInfo(VehicleHash.Limo2, new Vector3(283.47, -411.10, 44.40), new Vector3(0, 0, 17.87), 72, 72, 100);
            new VehInfo(VehicleHash.Stretch, new Vector3(280.35, -403.37, 44.75), new Vector3(0, 0, 26.03), 72, 72, 100);
            new VehInfo(VehicleHash.FBI2, new Vector3(275.94, -396.68, 44.61), new Vector3(0, 0, 41.90), 72, 72, 100);


            /* Развозчик еды 3 уровень*/
            new VehInfo(VehicleHash.Benson, new Vector3(993.48, -2548.28, 28.2776), new Vector3(0, 0, 354.81), 32, 72, 100);
            new VehInfo(VehicleHash.Benson, new Vector3(987.80, -2547.76, 28.2767), new Vector3(0, 0, 354.81), 72, 88, 100);
            new VehInfo(VehicleHash.Benson, new Vector3(982.26, -2547.24, 28.2767), new Vector3(0, 0, 354.81), 84, 72, 100);
            new VehInfo(VehicleHash.Benson, new Vector3(976.99, -2546.77, 28.2767), new Vector3(0, 0, 354.81), 22, 95, 100);
            new VehInfo(VehicleHash.Benson, new Vector3(971.69, -2546.29, 28.2767), new Vector3(0, 0, 354.81), 25, 45, 100);


            /* Развозчик еды 2 уровень*/
            new VehInfo(VehicleHash.Mule3, new Vector3(-1089.81, -2136.69, 13.5155), new Vector3(0, 0, 225.14), 25, 45, 100);
            new VehInfo(VehicleHash.Mule3, new Vector3(-1092.29, -2139.19, 13.5172), new Vector3(0, 0, 225.14), 25, 45, 100);
            new VehInfo(VehicleHash.Mule3, new Vector3(-1094.66, -2141.69, 13.5191), new Vector3(0, 0, 225.14), 25, 45, 100);
            new VehInfo(VehicleHash.Mule2, new Vector3(-1097.11, -2144.13, 13.5207), new Vector3(0, 0, 225.14), 25, 45, 100);
            new VehInfo(VehicleHash.Mule2, new Vector3(-1099.60, -2146.59, 13.5217), new Vector3(0, 0, 225.14), 25, 45, 100);
            new VehInfo(VehicleHash.Mule2, new Vector3(-1102.092, -2149.034, 13.5213), new Vector3(0, 0, 225.14), 25, 45, 100);
            new VehInfo(VehicleHash.Mule, new Vector3(-1104.36, -2151.41, 13.5220), new Vector3(0, 0, 225.14), 25, 45, 100);
            new VehInfo(VehicleHash.Mule, new Vector3(-1106.90, -2154.0505, 13.5227), new Vector3(0, 0, 225.14), 25, 45, 100);

            /* Дальнобои */
            new VehInfo(VehicleHash.Phantom, new Vector3(-2230.35, 4224.6, 46.8569), new Vector3(0, 0, 340.30), 5, 4, 100);
            new VehInfo(VehicleHash.Phantom, new Vector3(-2224.13, 4229.6, 47.1161), new Vector3(0, 0, 339.84), 7, 6, 100);
            new VehInfo(VehicleHash.Phantom, new Vector3(-2215.67, 4240.66, 47.442), new Vector3(0, 0, 36.18), 25, 45, 100);
            new VehInfo(VehicleHash.Phantom, new Vector3(-2211.85, 4245.91, 47.5242), new Vector3(0, 0, 36.89), 18, 4, 100);
            new VehInfo(VehicleHash.Phantom, new Vector3(-2198.19, 4244.41, 48.0188), new Vector3(0, 0, 36.87), 35, 45, 100);
            new VehInfo(VehicleHash.Phantom, new Vector3(-2193.14, 4248.36, 48.0697), new Vector3(0, 0, 38.26), 25, 11, 100);
            new VehInfo(VehicleHash.Phantom, new Vector3(-2181.64, 4263.35, 49.01), new Vector3(0, 0, 109.35), 22, 45, 100);
            new VehInfo(VehicleHash.Phantom, new Vector3(-2192.77, 4266.22, 48.67), new Vector3(0, 0, 60.98), 99, 55, 100);

            /* Дальнобои */
            new VehInfo(VehicleHash.Packer, new Vector3(-314.295, -2739.48, 6.08606), new Vector3(0, 0, 134.94), 99, 63, 100);
            new VehInfo(VehicleHash.Packer, new Vector3(-318.783, -2734.91, 6.09079), new Vector3(0, 0, 133.53), 54, 55, 100);
            new VehInfo(VehicleHash.Packer, new Vector3(-323.299, -2730.29, 6.09227), new Vector3(0, 0, 135.12), 21, 99, 100);
            new VehInfo(VehicleHash.Packer, new Vector3(-327.795, -2725.86, 6.0961), new Vector3(0, 0, 134.63), 32, 91, 100);
            new VehInfo(VehicleHash.Packer, new Vector3(-325.693, -2750.96, 6.09508), new Vector3(0, 0, 315.27), 7, 2, 100);
            new VehInfo(VehicleHash.Packer, new Vector3(-330.178, 2746.38, 6.1029), new Vector3(0, 0, 314.15), 9, 2, 100);
            new VehInfo(VehicleHash.Packer, new Vector3(-334.659, 2741.82, 6.1111), new Vector3(0, 0, 314.63), 1, 99, 100);
            new VehInfo(VehicleHash.Packer, new Vector3(-339.176, -2737.35, 6.11881), new Vector3(0, 0, 314.48), 26, 55, 100);
                        
            /* Полиция */
            new VehInfo(VehicleHash.Polmav, new Vector3(449.56, -981.60, 44.08), new Vector3(0, 0, 93.07), 0, 0, 100);
            new VehInfo(VehicleHash.Police3, new Vector3(458.15, -1024.57, 28.13), new Vector3(0, 0, 1.80), 131, 1, 100);
            new VehInfo(VehicleHash.Police3, new Vector3(456.01, -1024.70, 2.97), new Vector3(0, 0, 1.80), 131, 1, 100);
            new VehInfo(VehicleHash.Police3, new Vector3(453.461, -1024.74, 28.25), new Vector3(0, 0, 3.50), 131, 1, 100);
            new VehInfo(VehicleHash.Police3, new Vector3(451.11, -1024.92, 28.29), new Vector3(0, 0, 3.85), 131, 1, 100);
            new VehInfo(VehicleHash.Police3, new Vector3(448.79, -1025.06, 28.34), new Vector3(0, 0, 4.69), 131, 1, 100);
            new VehInfo(VehicleHash.Police3, new Vector3(446.37, -1025.26, 28.38), new Vector3(0, 0, 4.84), 131, 1, 100);
            new VehInfo(VehicleHash.Police3, new Vector3(443.93, -1025.45, 28.43), new Vector3(0, 0, 5.68), 131, 1, 100);
            new VehInfo(VehicleHash.Police3, new Vector3(441.48, -1025.68, 28.47), new Vector3(0, 0, 6.34), 131, 1, 100);
            new VehInfo(VehicleHash.Police3, new Vector3(438.92, -1025.91, 28.52), new Vector3(0, 0, 6.26), 131, 1, 100);
            new VehInfo(VehicleHash.PoliceT, new Vector3(462.79, -1014.73, 28.06), new Vector3(0, 0, 91.07), 131, 1, 100);
            new VehInfo(VehicleHash.PoliceT, new Vector3(463.02, -1019.51, 28.08), new Vector3(0, 0, 90.68), 131, 1, 100);
            new VehInfo(VehicleHash.Riot, new Vector3(452.46, -997.25, 25.41), new Vector3(0, 0, 179.09), 131, 1, 100);
            new VehInfo(VehicleHash.Riot, new Vector3(447.50, -997.42, 25.41), new Vector3(0, 0, 178.37), 131, 1, 100);
            new VehInfo(VehicleHash.Primo2, new Vector3(91.40, -1938.97, 20.12), new Vector3(0, 0, -152.27), 57, 57, 100);
            new VehInfo(VehicleHash.Chino2, new Vector3(96.25, -1947.03, 20.01), new Vector3(0, 0, -145.41), 57, 57, 100);
            new VehInfo(VehicleHash.Youga2, new Vector3(85.35, -1971.83, 20.77), new Vector3(0, 0, -39.65), 57, 57, 100);
            new VehInfo(VehicleHash.Moonbeam2, new Vector3(101.23, -1957.21, 20.23), new Vector3(0, 0, -5.18), 57, 57, 100);
            new VehInfo(VehicleHash.Moonbeam2, new Vector3(104.36, -1957.52, 20.23), new Vector3(0, 0, 0.44), 57, 57, 100);
            new VehInfo(VehicleHash.Virgo3, new Vector3(109.48, -1950.31, 20.07), new Vector3(0, 0, -67.62), 57, 57, 100);
            new VehInfo(VehicleHash.Patriot, new Vector3(116.87, -1949.51, 20.43), new Vector3(0, 0, 47.36), 57, 57, 100);
            new VehInfo(VehicleHash.Voodoo, new Vector3(115.64, -1940.88, 20.14), new Vector3(0, 0, -5.80), 57, 57, 100);
            new VehInfo(VehicleHash.Faction2, new Vector3(113.55, -1932.48, 20.01), new Vector3(0, 0, 37.03), 57, 57, 100);
            new VehInfo(VehicleHash.Buccaneer2, new Vector3(102.88, -1927.78, 19.81), new Vector3(0, 0, 73.80), 57, 57, 100);
            new VehInfo(VehicleHash.Blade, new Vector3(93.87, -1923.95, 20.06), new Vector3(0, 0, 61.91), 57, 57, 100);
            new VehInfo(VehicleHash.Blade, new Vector3(93.87, -1923.95, 20.06), new Vector3(0, 0, 61.91), 57, 57, 100);
            new VehInfo(VehicleHash.TowTruck, new Vector3(-619.39, 345.98, 85.08), new Vector3(0, 0, 174.93), 57, 57, 100);
            new VehInfo(VehicleHash.TowTruck, new Vector3(-615.52, 345.60, 85.08), new Vector3(0, 0, 174.36), 57, 57, 100);
            new VehInfo(VehicleHash.TowTruck, new Vector3(-611.92, 345.27, 85.08), new Vector3(0, 0, 174.88), 57, 57, 100);
            new VehInfo(VehicleHash.TowTruck, new Vector3(-608.37, 344.89, 85.08), new Vector3(0, 0, 174.68), 57, 57, 100);
            new VehInfo(VehicleHash.TowTruck, new Vector3(-605.15, 344.64, 85.08), new Vector3(0, 0, 176.31), 57, 57, 100);
            new VehInfo(VehicleHash.Trash2, new Vector3(831.0806, -785.8225, 25.9730), new Vector3(0, 0, 115.95), 57, 57, 100);
            new VehInfo(VehicleHash.Trash2, new Vector3(832.5711, -789.0053, 25.9839), new Vector3(0, 0, 114.52), 57, 57, 100);
            new VehInfo(VehicleHash.Trash2, new Vector3(835.3048, -794.9215, 25.9843), new Vector3(0, 0, 116.66), 57, 57, 100);
            new VehInfo(VehicleHash.Trash2, new Vector3(836.9758, -798.0812, 25.9876), new Vector3(0, 0, 115.75), 57, 57, 100);
            new VehInfo(VehicleHash.Police, new Vector3(833.41, -1258.07, 25.94), new Vector3(0, 0, 179.71), 131, 1, 100);
            new VehInfo(VehicleHash.Police, new Vector3(822.66, -1258.04, 25.85), new Vector3(0, 0, 179.57), 131, 1, 100);
            new VehInfo(VehicleHash.Police, new Vector3(827.96, -1258.09, 25.89), new Vector3(0, 0, -179.76), 131, 1, 100);
            new VehInfo(VehicleHash.Faction2, new Vector3(320.24, -2020.94, 20.15), new Vector3(0, 0, 88.42), 89, 89, 100);
            new VehInfo(VehicleHash.Chino2, new Vector3(324.94, -2024.51, 20.36), new Vector3(0, 0, 85.89), 89, 89, 100);
            new VehInfo(VehicleHash.Voodoo, new Vector3(328.97, -2027.91, 20.63), new Vector3(0, 0, 84.65), 89, 89, 100);
            new VehInfo(VehicleHash.Virgo2, new Vector3(333.22, -2031.25, 20.69), new Vector3(0, 0, 80.86), 89, 89, 100);
            new VehInfo(VehicleHash.Youga2, new Vector3(335.40, -2040.88, 21.05), new Vector3(0, 0, 47.30), 89, 89, 100);
            new VehInfo(VehicleHash.Moonbeam2, new Vector3(330.73, -2044.43, 20.24), new Vector3(0, 0, 14.33), 89, 89, 100);
            new VehInfo(VehicleHash.Patriot, new Vector3(326.68, -2041.07, 20.46), new Vector3(0, 0, 13.89), 89, 89, 100);
            new VehInfo(VehicleHash.Primo2, new Vector3(323.19, -2038.40, 20.15), new Vector3(0, 0, 13.26), 89, 89, 100);
            new VehInfo(VehicleHash.Primo2, new Vector3(320.06, -2035.25, 20.11), new Vector3(0, 0, 11.62), 89, 89, 100);
            new VehInfo(VehicleHash.Tornado5, new Vector3(316.82, -2032.59, 20.24), new Vector3(0, 0, 8.38), 89, 89, 100);
            new VehInfo(VehicleHash.Peyote, new Vector3(313.56, -2029.99, 19.86), new Vector3(0, 0, 12.23), 89, 89, 100);
            new VehInfo(VehicleHash.Faction2, new Vector3(1170.36, -1646.23, 36.21), new Vector3(0, 0, 152.06), 64, 64, 100);
            new VehInfo(VehicleHash.Primo2, new Vector3(1165.82, -1643.59, 36.42), new Vector3(0, 0, 169.68), 64, 64, 100);
            new VehInfo(VehicleHash.Coquette3, new Vector3(1162.60, -1644.36, 36.35), new Vector3(0, 0, -155.98), 64, 64, 100);
            new VehInfo(VehicleHash.Moonbeam2, new Vector3(1160.19, -1645.77, 36.39), new Vector3(0, 0, -156.98), 64, 64, 100);
            new VehInfo(VehicleHash.Patriot, new Vector3(1157.90, -1647.55, 36.65), new Vector3(0, 0, -154.94), 64, 64, 100);
            new VehInfo(VehicleHash.Youga2, new Vector3(1149.03, -1642.18, 36.29), new Vector3(0, 0, -153.17), 64, 64, 100);
            new VehInfo(VehicleHash.Glendale, new Vector3(1150.74, -1652.87, 36.20), new Vector3(0, 0, -108.84), 64, 64, 100);
            new VehInfo(VehicleHash.Voodoo, new Vector3(1152.93, -1656.88, 36.05), new Vector3(0, 0, -118.97), 64, 64, 100);
            new VehInfo(VehicleHash.Virgo2, new Vector3(1154.62, -1660.76, 36.02), new Vector3(0, 0, -122.39), 64, 64, 100);
            new VehInfo(VehicleHash.Chino2, new Vector3(1157.26, -1665.48, 35.99), new Vector3(0, 0, -125.45), 64, 64, 100);
            new VehInfo(VehicleHash.FireTruck, new Vector3(-372.76, 6123.97, 31.51), new Vector3(0, 0, 45.14), 27, 27, 100);
            new VehInfo(VehicleHash.FireTruck, new Vector3(-369.25, 6127.47, 31.51), new Vector3(0, 0, 44.44), 27, 27, 100);
            new VehInfo(VehicleHash.Speedo, new Vector3(-933.863, -1978.36, 12.9272), new Vector3(0, 0, 135.11), 46, 20, 100);
            new VehInfo(VehicleHash.Rumpo2, new Vector3(-936.353, -1976.0035, 12.9309), new Vector3(0, 0, 135.11), 15, 27, 100);
            new VehInfo(VehicleHash.Rumpo3, new Vector3(-938.869, -1973.58, 12.9309), new Vector3(0, 0, 135.11), 32, 27, 100);
            new VehInfo(VehicleHash.Rumpo, new Vector3(-941.219, -1971.19, 12.9309), new Vector3(0, 0, 135.11), 63, 27, 100);
            new VehInfo(VehicleHash.Pony, new Vector3(-943.695, -1968.81, 12.9309), new Vector3(0, 0, 135.11), 88, 0, 100);
            new VehInfo(VehicleHash.Pony, new Vector3(-946.142, -1966.36, 12.9309), new Vector3(0, 0, 135.11), 81, 57, 100);
            new VehInfo(VehicleHash.Speedo, new Vector3(-948.537, -1963.99, 12.9309), new Vector3(0, 0, 135.11), 52, 88, 100);
            new VehInfo(VehicleHash.Youga, new Vector3(-950.983, -1961.51, 12.9309), new Vector3(0, 0, 135.11), 12, 27, 100);
            new VehInfo(VehicleHash.Youga, new Vector3(-953.568, -1958.88, 12.9309), new Vector3(0, 0, 135.11), 48, 96, 100);
            new VehInfo(VehicleHash.Youga, new Vector3(-955.845, -1956.5, 12.9309), new Vector3(0, 0, 135.11), 87, 33, 100);
            new VehInfo(VehicleHash.Tractor2, new Vector3(2807.56, 4640.57, 45.145), new Vector3(0, 0, 192.44), 87, 33, 100);
            new VehInfo(VehicleHash.DLoader, new Vector3(2872.79, 4563.74, 47.5253), new Vector3(0, 0, 49.84), 87, 33, 100);
            new VehInfo(VehicleHash.DLoader, new Vector3(2869.92, 4560.9, 47.5427), new Vector3(0, 0, 44.80), 87, 33, 100);
            new VehInfo(VehicleHash.Kalahari, new Vector3(408.848, 6601.36, 27.4398), new Vector3(0, 0, 35.29), 87, 33, 100); // фирма номер 1
            new VehInfo(VehicleHash.Youga2, new Vector3(-1081.46, -1670.13, 4.64), new Vector3(0, 0, -51.93), 148, 148, 100); // банда Баллас
            new VehInfo(VehicleHash.Youga2, new Vector3(-1081.46, -1670.13, 4.64), new Vector3(0, 0, -51.93), 148, 148, 100); // банда Баллас
            new VehInfo(VehicleHash.Tornado5, new Vector3(-1063.96, -1663.48, 4.22), new Vector3(0, 0, 124.82), 148, 148, 100);// банда Баллас
            new VehInfo(VehicleHash.Peyote, new Vector3(-1077.17, -1676.98, 3.90), new Vector3(0, 0, -49.15), 148, 148, 100); // банда Баллас
            new VehInfo(VehicleHash.Primo2, new Vector3(-1072.76, -1654.75, 3.93), new Vector3(0, 0, 134.37), 148, 148, 100); // банда Баллас
            new VehInfo(VehicleHash.Dubsta3, new Vector3(-1106.98, -1634.36, 4.48), new Vector3(0, 0, -54.71), 148, 148, 100); // банда Баллас
            new VehInfo(VehicleHash.Voodoo, new Vector3(-1091.84, -1632.00, 4.16), new Vector3(0, 0, 125.70), 148, 148, 100); // банда Баллас
            new VehInfo(VehicleHash.Virgo2, new Vector3(-1089.95, -1634.54, 4.10), new Vector3(0, 0, 125.28), 148, 148, 100); // банда Баллас
            new VehInfo(VehicleHash.Moonbeam2, new Vector3(-1077.32, -1651.01, 3.94), new Vector3(0, 0, 131.11), 148, 148, 100); // банда Баллас
            new VehInfo(VehicleHash.Faction2, new Vector3(-1102.45, -1617.27, 4.07), new Vector3(0, 0, 62.64), 148, 148, 100); // банда Баллас
            new VehInfo(VehicleHash.Chino2, new Vector3(-1106.54, -1601.87, 4.03), new Vector3(0, 0, 126.14), 148, 148, 100); // банда Баллас
            new VehInfo(VehicleHash.Picador, new Vector3(-1086.58, -1661.83, 4.29), new Vector3(0, 0, -54.38), 148, 148, 100); // банда Баллас
            new VehInfo(VehicleHash.Ambulance, new Vector3(1835.53, 3664.02, 33.57), new Vector3(0, 0, -148.46), 48, 96, 100); // больница номер N*5
            new VehInfo(VehicleHash.Ambulance, new Vector3(1832.39, 3662.09, 33.64), new Vector3(0, 0, -148.46), 48, 96, 100); // больница номер N*5
            new VehInfo(VehicleHash.Ambulance, new Vector3(1826.05, 3658.13, 33.79), new Vector3(0, 0, -148.46), 48, 96, 100); // больница номер N*5
            new VehInfo(VehicleHash.Ambulance, new Vector3(1829.14, 3660.05, 33.70), new Vector3(0, 0, -148.46), 48, 96, 100); // больница номер N*5
            new VehInfo(VehicleHash.Ambulance, new Vector3(1826.05, 3658.13, 33.79), new Vector3(0, 0, -148.46), 48, 96, 100); // больница номер N*5
            new VehInfo(VehicleHash.Sheriff, new Vector3(1860.98, 3680.19, 33.32), new Vector3(0, 0, -149.63), 131, 1, 100); // Police Sheriff
            new VehInfo(VehicleHash.Sheriff, new Vector3(1863.61, 3681.69, 33.32), new Vector3(0, 0, -149.63), 131, 1, 100); // Police Sheriff
            new VehInfo(VehicleHash.Sheriff2, new Vector3(1866.39, 3683.05, 33.33), new Vector3(0, 0, -149.63), 131, 1, 100); // Police Sheriff
            new VehInfo(VehicleHash.Sheriff2, new Vector3(1869.08, 3684.51, 33.35), new Vector3(0, 0, -149.63), 131, 1, 100); // Police Sheriff
            new VehInfo(VehicleHash.PoliceT, new Vector3(1869.73, 3692.16, 33.62), new Vector3(0, 0, -149.63), 131, 1, 100); // Police Sheriff
                                                                                                                             // new VehInfo(VehicleHash.Xls, new Vector3(-2312.22, 3266.12, 32.75), new Vector3(0, 0, 59.87), 49, 49, 100); // армия внедорожник генерала !!!ЗАМЕНИТЬ 
            new VehInfo(VehicleHash.Crusader, new Vector3(-2310.63, 3268.93, 32.33), new Vector3(0, 0, 59.87), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Crusader, new Vector3(-2308.85, 3271.83, 32.33), new Vector3(0, 0, 61.23), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Crusader, new Vector3(-2307.19, 3274.86, 32.33), new Vector3(0, 0, 61.66), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Patriot, new Vector3(-2305.48, 3277.67, 32.56), new Vector3(0, 0, 60.41), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Patriot, new Vector3(-2303.78, 3280.69, 32.56), new Vector3(0, 0, 59.95), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Patriot, new Vector3(-2302.12, 3283.47, 32.56), new Vector3(0, 0, 60.31), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Insurgent2, new Vector3(-2287.8, 3325.04, 32.75), new Vector3(0, 0, -119.46), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Insurgent, new Vector3(-2286.15, 3328.04, 32.74), new Vector3(0, 0, -119.37), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Insurgent, new Vector3(-2284.45, 3331.02, 32.74), new Vector3(0, 0, -119.38), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Mesa3, new Vector3(-2282.5, 3333.87, 32.59), new Vector3(0, 0, -120.01), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Mesa3, new Vector3(-2280.92, 3336.75, 32.59), new Vector3(0, 0, -119.67), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Mesa3, new Vector3(-2279.21, 3339.76, 32.59), new Vector3(0, 0, -119.95), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Rhino, new Vector3(-2358.62, 3376.32, 32.79), new Vector3(0, 0, 149.83), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Rhino, new Vector3(-2368.11, 3381.8, 32.79), new Vector3(0, 0, 150.25), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Rhino, new Vector3(-2377.18, 3387.05, 32.79), new Vector3(0, 0, 149.86), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Barracks3, new Vector3(-1791.67, 3127.29, 32.44), new Vector3(0, 0, 61.06), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Barracks3, new Vector3(-1788.98, 3131.98, 32.45), new Vector3(0, 0, 59.62), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Barracks3, new Vector3(-1787.35, 3134.74, 32.45), new Vector3(0, 0, 60.54), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Barracks, new Vector3(-1776.6, 3133.4, 32.44), new Vector3(0, 0, -120.09), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Barracks, new Vector3(-1779.25, 3128.8, 32.44), new Vector3(0, 0, -119.47), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Barracks, new Vector3(-1780.93, 3125.88, 32.44), new Vector3(0, 0, -118.78), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Barracks, new Vector3(-1783.53, 3121.42, 32.44), new Vector3(0, 0, -120.44), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Barracks2, new Vector3(-1779.37, 3044.29, 32.55), new Vector3(0, 0, -29.75), 49, 49, 100);// армия 
            new VehInfo(VehicleHash.Barracks2, new Vector3(-1771.14, 3039.66, 32.55), new Vector3(0, 0, -29.52), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Barracks2, new Vector3(-1762.97, 3035.09, 32.55), new Vector3(0, 0, -30.16), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Barracks2, new Vector3(-1755.09, 3030.64, 32.56), new Vector3(0, 0, -30.05), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Contender, new Vector3(-2078.22, 2818.08, 33.04), new Vector3(0, 0, -95.28), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Contender, new Vector3(-2078.55, 2814.71, 33.04), new Vector3(0, 0, -95.54), 49, 49, 100); // армия
            new VehInfo(VehicleHash.FireTruck, new Vector3(-2120.57, 2832.6, 32.88), new Vector3(0, 0, -5.54), 49, 49, 100); // армия
            new VehInfo(VehicleHash.FireTruck, new Vector3(-2115.16, 2832.22, 32.88), new Vector3(0, 0, -4.45), 49, 49, 100); // армия
            new VehInfo(VehicleHash.FireTruck, new Vector3(-2109.88, 2831.74, 32.88), new Vector3(0, 0, -5), 49, 49, 100); // армия
            new VehInfo(VehicleHash.FireTruck, new Vector3(-2104.56, 2831.28, 32.88), new Vector3(0, 0, -5.45), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Valkyrie2, new Vector3(-1859.45, 2795.67, 33.19), new Vector3(0, 0, 146.57), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Valkyrie2, new Vector3(-1877.03, 2805.93, 33.19), new Vector3(0, 0, 148.36), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Buzzard, new Vector3(-2392.64, 3244.52, 32.9), new Vector3(0, 0, 148.66), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Buzzard, new Vector3(-2381.75, 3238.85, 32.94), new Vector3(0, 0, 147.87), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Cargobob4, new Vector3(-1928.76, 3120.52, 33.27), new Vector3(0, 0, 147.26), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Cargobob3, new Vector3(-1886.89, 3092.37, 33.27), new Vector3(0, 0, 147.3), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Titan, new Vector3(-1987.99, 3065.23, 33.65), new Vector3(0, 0, -29), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Titan, new Vector3(-2053.67, 3102.69, 33.65), new Vector3(0, 0, -30.26), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Titan, new Vector3(-2118.81, 3140.11, 33.65), new Vector3(0, 0, -30.21), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Titan, new Vector3(-2184, 3177.95, 33.65), new Vector3(0, 0, -31.41), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Lazer, new Vector3(-2235.61, 3272.49, 33.32), new Vector3(0, 0, -120.43), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Lazer, new Vector3(-2248.84, 3247.36, 33.32), new Vector3(0, 0, -118.34), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Lazer, new Vector3(-2264.48, 3224.05, 33.32), new Vector3(0, 0, -120.69), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Lazer, new Vector3(-2287.15, 3181.91, 33.31), new Vector3(0, 0, -119.98), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Hydra, new Vector3(-2142.81, 3022.33, 33.35), new Vector3(0, 0, -30.35), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Hydra, new Vector3(-2012.4, 2949.91, 33.35), new Vector3(0, 0, -30.4), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Faggio, new Vector3(40.55, -1220.30, 28.75), new Vector3(0, 0, -76.79), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Faggio, new Vector3(40.68, -1221.89, 28.76), new Vector3(0, 0, -87.62), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Faggio, new Vector3(39.92, -1229.02, 28.76), new Vector3(0, 0, -93.85), 49, 49, 100); // армия
            new VehInfo(VehicleHash.Faggio, new Vector3(40.17, -1231.55, 28.76), new Vector3(0, 0, -100.48), 49, 49, 100); // армия          
        }
    }
}