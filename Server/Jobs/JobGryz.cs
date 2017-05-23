/*
 * Работа грузчика
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
    public class JobGryz : Script
    {
        List<Box> GryzBoxes = new List<Box>();
        int Money = 12; // зп за один ящик

        class Box
        {
            public Client Player;
            public NetHandle Object;
            public int Type { get; set; }
        }

        public JobGryz()
        {
            API.onResourceStart += onResourceStart;
            API.onEntityEnterColShape += onMarkerEnter;
            API.onEntityExitColShape += onMarkerExit;
            API.onClientEventTrigger += onClientEvent;
            API.onPlayerDeath += onPlayerDeathHandler;
        }

        void SpawnBoxes()
        {
            #region "Спавн коробок"   
            GryzBoxes.Add(new Box() { Type = 1, Object = API.createObject(250374685, new Vector3(-769.18f, -2654.126f, 12.81986), new Vector3(0, 0, -31.99994)) });
            GryzBoxes.Add(new Box() { Type = 1, Object = API.createObject(250374685, new Vector3(-769.537f, -2654.717f, 12.81976), new Vector3(0, 0, -31.99994)) });
            GryzBoxes.Add(new Box() { Type = 1, Object = API.createObject(250374685, new Vector3(-769.9357f, -2655.314f, 12.81949), new Vector3(0, 0, -31.99994)) });
            GryzBoxes.Add(new Box() { Type = 1, Object = API.createObject(250374685, new Vector3(-770.2675f, -2655.864f, 12.8192), new Vector3(0, 0, -31.99994)) });
            GryzBoxes.Add(new Box() { Type = 1, Object = API.createObject(250374685, new Vector3(-769.7789f, -2656.181f, 13.20207), new Vector3(0, 0, -31.99994)) });
            GryzBoxes.Add(new Box() { Type = 1, Object = API.createObject(250374685, new Vector3(-769.8141f, -2656.163f, 12.81918), new Vector3(0, 0, -31.99994)) });
            GryzBoxes.Add(new Box() { Type = 1, Object = API.createObject(250374685, new Vector3(-769.4968f, -2655.582f, 13.20236), new Vector3(0, 0, -31.99994)) });
            GryzBoxes.Add(new Box() { Type = 1, Object = API.createObject(250374685, new Vector3(-769.4318f, -2655.623f, 12.81948), new Vector3(0, 0, -31.99994)) });
            GryzBoxes.Add(new Box() { Type = 1, Object = API.createObject(250374685, new Vector3(-769.137f, -2654.984f, 13.20268), new Vector3(0, 0, -31.99994)) });
            GryzBoxes.Add(new Box() { Type = 1, Object = API.createObject(250374685, new Vector3(-769.0726f, -2655.029f, 12.81979), new Vector3(0, 0, -31.99994)) });
            GryzBoxes.Add(new Box() { Type = 1, Object = API.createObject(250374685, new Vector3(-768.7393f, -2654.425f, 13.20297), new Vector3(0, 0, -31.99994)) });
            GryzBoxes.Add(new Box() { Type = 1, Object = API.createObject(250374685, new Vector3(-768.6763, -2654.496, 12.82009), new Vector3(0, 0, -31.99994)) });

            GryzBoxes.Add(new Box() { Type = 2, Object = API.createObject(-1515940233, new Vector3(-772.6821, -2658.759, 13.84637), new Vector3(0, 0, -55.99984)) });
            GryzBoxes.Add(new Box() { Type = 2, Object = API.createObject(-1515940233, new Vector3(-771.9576, -2657.993, 13.8468), new Vector3(0, 0, -55.99984)) });
            GryzBoxes.Add(new Box() { Type = 2, Object = API.createObject(-1515940233, new Vector3(-771.1292, -2657.424, 13.84722), new Vector3(0, 0, -55.99984)) });
            GryzBoxes.Add(new Box() { Type = 2, Object = API.createObject(-1515940233, new Vector3(-771.7107, -2656.532, 13.33), new Vector3(0, 0, -55.99984)) });
            GryzBoxes.Add(new Box() { Type = 2, Object = API.createObject(-1515940233, new Vector3(-773.3956, -2657.754, 13.33214), new Vector3(0, 0, -55.99984)) });
            GryzBoxes.Add(new Box() { Type = 2, Object = API.createObject(-1515940233, new Vector3(-772.4794, -2657.225, 13.33256), new Vector3(0, 0, -55.99984)) });
            GryzBoxes.Add(new Box() { Type = 2, Object = API.createObject(-1515940233, new Vector3(-772.7729, -2658.624, 13.33197), new Vector3(0, 0, -55.99984)) });
            GryzBoxes.Add(new Box() { Type = 2, Object = API.createObject(-1515940233, new Vector3(-771.9977, -2657.945, 13.3324), new Vector3(0, 0, -55.99984)) });
            GryzBoxes.Add(new Box() { Type = 2, Object = API.createObject(-1515940233, new Vector3(-771.1791, -2657.365, 13.33282), new Vector3(0, 0, -55.99984)) });
            GryzBoxes.Add(new Box() { Type = 2, Object = API.createObject(-1515940233, new Vector3(-771.6302, -2656.53, 12.81863), new Vector3(0, 0, -55.99984)) });
            GryzBoxes.Add(new Box() { Type = 2, Object = API.createObject(-1515940233, new Vector3(-772.5018, -2657.213, 12.81816), new Vector3(0, 0, -55.99984)) });
            GryzBoxes.Add(new Box() { Type = 2, Object = API.createObject(-1515940233, new Vector3(-773.3819, -2657.78, 12.81774), new Vector3(0, 0, -55.99984)) });
            GryzBoxes.Add(new Box() { Type = 2, Object = API.createObject(-1515940233, new Vector3(-772.8646, -2658.511, 12.81757), new Vector3(0, 0, -55.99984)) });
            GryzBoxes.Add(new Box() { Type = 2, Object = API.createObject(-1515940233, new Vector3(-772.0079, -2657.916, 12.818), new Vector3(0, 0, -55.99984)) });
            GryzBoxes.Add(new Box() { Type = 2, Object = API.createObject(-1515940233, new Vector3(-771.147, -2657.325, 12.81843), new Vector3(0, 0, -55.99984)) });

            GryzBoxes.Add(new Box() { Type = 3, Object = API.createObject(377646791, new Vector3(-777.0083, -2660.277, 13.7), new Vector3(-4.462355, -6.329292, 60.99992)) }); // ящики
            GryzBoxes.Add(new Box() { Type = 3, Object = API.createObject(377646791, new Vector3(-777.6655, -2660.045, 13.7), new Vector3(-4.462355, -6.329292, 60.99992)) });
            GryzBoxes.Add(new Box() { Type = 3, Object = API.createObject(377646791, new Vector3(-778.2114, -2659.693, 13.7), new Vector3(-4.462355, -6.329292, 60.99992)) });
            GryzBoxes.Add(new Box() { Type = 3, Object = API.createObject(377646791, new Vector3(-776.9972, -2660.274, 13.1), new Vector3(-4.462355, -6.329292, 60.99992)) });
            GryzBoxes.Add(new Box() { Type = 3, Object = API.createObject(377646791, new Vector3(-777.634, -2660.052, 13.1), new Vector3(-4.462355, -6.329292, 60.99992)) });
            GryzBoxes.Add(new Box() { Type = 3, Object = API.createObject(377646791, new Vector3(-778.235, -2659.69, 13.1), new Vector3(-4.462355, -6.329292, 60.99992)) });
            GryzBoxes.Add(new Box() { Type = 3, Object = API.createObject(377646791, new Vector3(-777.3239, -2661.05, 13.65), new Vector3(-4.462355, -6.329292, 60.99992)) });
            GryzBoxes.Add(new Box() { Type = 3, Object = API.createObject(377646791, new Vector3(-777.9757, -2660.641, 13.7), new Vector3(-4.462355, -6.329292, 60.99992)) });
            GryzBoxes.Add(new Box() { Type = 3, Object = API.createObject(377646791, new Vector3(-778.5827, -2660.355, 13.7), new Vector3(-4.462355, -6.329292, 60.99992)) });
            GryzBoxes.Add(new Box() { Type = 3, Object = API.createObject(377646791, new Vector3(-778.5789, -2660.332, 13.1), new Vector3(-4.462355, -6.329292, 60.99992)) });
            GryzBoxes.Add(new Box() { Type = 3, Object = API.createObject(377646791, new Vector3(-777.9609, -2660.657, 13.09), new Vector3(-4.462355, -6.329292, 60.99992)) });
            GryzBoxes.Add(new Box() { Type = 3, Object = API.createObject(377646791, new Vector3(-777.3362, -2661.049, 13.08), new Vector3(-4.462355, -6.329291, 60.99992)) });

            GryzBoxes.Add(new Box() { Type = 4, Object = API.createObject(1470358132, new Vector3(-781.5743f, -2658.378f, 13.03f), new Vector3(0f, 0f, 57.99987f)) }); // маленькие ящики с канатными ручками
            GryzBoxes.Add(new Box() { Type = 4, Object = API.createObject(1470358132, new Vector3(-781.2275f, -2657.85f, 13.03f), new Vector3(0f, 0f, 57.99987f)) });
            GryzBoxes.Add(new Box() { Type = 4, Object = API.createObject(1470358132, new Vector3(-780.9852f, -2658.777f, 13.03f), new Vector3(0f, 0f, 57.99987f)) });
            GryzBoxes.Add(new Box() { Type = 4, Object = API.createObject(1470358132, new Vector3(-780.6859f, -2658.195f, 13.03f), new Vector3(0f, 0f, 57.99987f)) });
            GryzBoxes.Add(new Box() { Type = 4, Object = API.createObject(1470358132, new Vector3(-780.4235f, -2659.095f, 13.03f), new Vector3(0f, 0f, 57.99987f)) });
            GryzBoxes.Add(new Box() { Type = 4, Object = API.createObject(1470358132, new Vector3(-780.1711f, -2658.512f, 13.03f), new Vector3(0f, 0f, 57.99987f)) });
            GryzBoxes.Add(new Box() { Type = 4, Object = API.createObject(1470358132, new Vector3(-779.9551f, -2659.379f, 13.03f), new Vector3(0f, 0f, 57.99987f)) });
            GryzBoxes.Add(new Box() { Type = 4, Object = API.createObject(1470358132, new Vector3(-779.6435f, -2658.786f, 13.03f), new Vector3(0f, 0f, 57.99987f)) });
            GryzBoxes.Add(new Box() { Type = 4, Object = API.createObject(1470358132, new Vector3(-779.5147f, -2659.654f, 13.03f), new Vector3(0f, 0f, 57.99987f)) });
            GryzBoxes.Add(new Box() { Type = 4, Object = API.createObject(1470358132, new Vector3(-779.1564f, -2659.067f, 13.03f), new Vector3(0f, 0f, 57.99987f)) });

            GryzBoxes.Add(new Box() { Type = 5, Object = API.createObject(1470358132, new Vector3(-775.2434, -2658.178, 13.03726), new Vector3(0, 0, -68.67301)) });
            GryzBoxes.Add(new Box() { Type = 5, Object = API.createObject(1470358132, new Vector3(-775.7636, -2658.395, 13.03701), new Vector3(0, 0, -68.67301)) });
            GryzBoxes.Add(new Box() { Type = 5, Object = API.createObject(1470358132, new Vector3(-775.6279, -2658.735, 13.03704), new Vector3(0, 0, -68.67301)) });
            GryzBoxes.Add(new Box() { Type = 5, Object = API.createObject(1470358132, new Vector3(-775.117, -2658.491, 13.0373), new Vector3(0, 0, -68.67301)) });
            GryzBoxes.Add(new Box() { Type = 5, Object = API.createObject(1470358132, new Vector3(-774.6111, -2658.301, 13.03755), new Vector3(0, 0, -68.67301)) });
            GryzBoxes.Add(new Box() { Type = 5, Object = API.createObject(1470358132, new Vector3(-774.7673, -2657.982, 13.0375), new Vector3(0, 0, -68.67301)) });

            GryzBoxes.Add(new Box() { Type = 6, Object = API.createObject(-335465691, new Vector3(-783.0701f, -2656.439f, 12.81205f), new Vector3(0f, 0f, -32.99994f)) }); // Пакеты
            GryzBoxes.Add(new Box() { Type = 6, Object = API.createObject(-335465691, new Vector3(-783.2599f, -2656.75f, 12.81205f), new Vector3(0f, 0f, -32.99994f)) });
            GryzBoxes.Add(new Box() { Type = 6, Object = API.createObject(-335465691, new Vector3(-783.4825f, -2657.087f, 12.81205f), new Vector3(0f, 0f, -32.99994f)) });
            GryzBoxes.Add(new Box() { Type = 6, Object = API.createObject(-335465691, new Vector3(-783.6802f, -2657.402f, 12.81205f), new Vector3(0f, 0f, -32.99994f)) });
            GryzBoxes.Add(new Box() { Type = 6, Object = API.createObject(-335465691, new Vector3(-782.5052f, -2656.776f, 12.81234f), new Vector3(0f, 0f, -32.99994f)) });
            GryzBoxes.Add(new Box() { Type = 6, Object = API.createObject(-335465691, new Vector3(-782.7109f, -2657.107f, 12.81243f), new Vector3(0f, 0f, -32.99994f)) });
            GryzBoxes.Add(new Box() { Type = 6, Object = API.createObject(-335465691, new Vector3(-782.9609f, -2657.434f, 12.81246f), new Vector3(0f, 0f, -32.99994f)) });
            GryzBoxes.Add(new Box() { Type = 6, Object = API.createObject(-335465691, new Vector3(-783.1762f, -2657.716f, 12.81248f), new Vector3(0f, 0f, -32.99994f)) });
            GryzBoxes.Add(new Box() { Type = 6, Object = API.createObject(-335465691, new Vector3(-781.9636f, -2657.077f, 12.8133f), new Vector3(0f, 0f, -32.99995f)) });
            GryzBoxes.Add(new Box() { Type = 6, Object = API.createObject(-335465691, new Vector3(-782.229f, -2657.392f, 12.8133f), new Vector3(0f, 0f, -32.99995f)) });
            GryzBoxes.Add(new Box() { Type = 6, Object = API.createObject(-335465691, new Vector3(-782.4382f, -2657.69f, 12.81335f), new Vector3(0f, 0f, -32.99995f)) });
            GryzBoxes.Add(new Box() { Type = 6, Object = API.createObject(-335465691, new Vector3(-782.6542f, -2657.991f, 12.81339f), new Vector3(0f, 0f, -32.99996f)) });

            /*
               GryzBoxes.Add(new Box() { Type = 7, Object = API.createObject(-2031321722, new Vector3(55.31, -1259.78, -10.0), new Vector3()) });
               GryzBoxes.Add(new Box() { Type = 8, Object = API.createObject(1735046030, new Vector3(55.31, -1259.78, -10.0), new Vector3()) });
               GryzBoxes.Add(new Box() { Type = 9, Object = API.createObject(-524841151, new Vector3(55.31, -1259.78, -10.0), new Vector3()) });
               GryzBoxes.Add(new Box() { Type = 10, Object = API.createObject(1959542339, new Vector3(55.31, -1259.78, -10.0), new Vector3()) });
             */
            #endregion
        }

        private void onPlayerDeathHandler(Client player, NetHandle entityKiller, int weapon)
        {
            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
            if ((Jobs)pInfo.Job == Jobs.JOB_JOBGRYZ_ONE)
            {
                API.sendChatMessageToPlayer(player, "~y~[Server] ~w~Вы погибли. ~r~Миссия провалена.");
                JobGryzYval(player);
            }
        }

        private void HandleTimer(System.Object source, ElapsedEventArgs e)
        {
            if (this.GryzBoxes.Count == 0)
            {
                SpawnBoxes();
            }
        }

        public void onResourceStart()
        {
            lock (Global.Lock)
            {
                new BlipInfo(new Vector3(-745.11, -2551.56, 13.9), 68, 6, false, 0, "Работа грузчика", Jobs.JOB_JOBGRYZ_ONE);
                new MarkerInfo(1, new Vector3(-745.11, -2551.56, 13.1), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1), 100, 255, 255, 255, 1, 1, Jobs.JOB_JOBGRYZ_ONE);

                SpawnBoxes();
                System.Timers.Timer timer = new System.Timers.Timer(10 * 60 * 1000); // Если все коробки перетащили по переспавним их 
                timer.AutoReset = true;
                timer.Elapsed += HandleTimer;
                timer.Start();

                #region "Забор"
                API.createObject(-994492850, new Vector3(-789.6503, -2653.956, 12.81205), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-789.2096, -2654.75, 12.81205), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-788.389, -2655.202, 12.81205), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-787.5263, -2655.702, 12.81205), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-786.6897, -2656.185, 12.81205), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-785.8237, -2656.672, 12.81205), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-784.929, -2657.192, 12.81205), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-784.0118, -2657.781, 12.81205), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-783.1033, -2658.232, 12.81309), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-782.1632, -2658.773, 12.81477), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-781.1611, -2659.36, 12.81495), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-780.2264, -2659.924, 12.81494), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-779.3403, -2660.441, 12.81494), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-778.4513, -2660.907, 12.81505), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-777.4667, -2661.528, 12.81536), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-776.3776, -2662.123, 12.81537), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-775.2924, -2662.798, 12.81535), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-774.2773, -2663.471, 12.81532), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-773.163, -2664.178, 12.81529), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-772.6677, -2663.295, 12.81575), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-772.1647, -2662.409, 12.81621), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-771.6884, -2661.591, 12.81664), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-771.2081, -2660.782, 12.81706), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-770.7186, -2659.91, 12.81751), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-770.1356, -2658.904, 12.81804), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-769.6246, -2657.981, 12.81851), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-769.0507, -2657.034, 12.81901), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-768.5313, -2656.164, 12.81947), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-767.9894, -2655.254, 12.81995), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-767.4566, -2654.321, 12.82043), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-767.0068, -2653.309, 12.82093), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-766.3951, -2652.465, 12.8214), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-765.8244, -2651.552, 12.82164), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-765.1339, -2650.341, 12.82192), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-764.6122, -2649.326, 12.82223), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-764.2053, -2648.588, 12.82247), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-764.3941, -2647.236, 12.82257), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-764.7121, -2645.889, 12.82279), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-765.4285, -2645.386, 12.82147), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-766.242, -2644.892, 12.81999), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-767.0336, -2644.427, 12.81856), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-767.8345, -2643.973, 12.81714), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-768.6244, -2643.515, 12.81572), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-769.5464, -2642.925, 12.81401), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-770.227, -2642.481, 12.81275), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-770.8858, -2642.169, 12.81205), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-771.3678, -2642.971, 12.81205), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-771.9128, -2643.857, 12.8121), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-772.3817, -2644.757, 12.81244), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-772.9225, -2645.656, 12.8127), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-773.4984, -2646.672, 12.81302), new Vector3(0, 0, -30));
                API.createObject(-994492850, new Vector3(-774.4734, -2647.07, 12.81225), new Vector3(0, 0, -63.9999));
                API.createObject(-994492850, new Vector3(-775.4937, -2647.543, 12.81205), new Vector3(0, 0, -63.99991));
                API.createObject(-994492850, new Vector3(-776.4147, -2647.943, 12.81205), new Vector3(0, 0, -63.99991));
                API.createObject(-994492850, new Vector3(-777.3093, -2648.368, 12.81205), new Vector3(0, 0, -63.99991));
                API.createObject(-994492850, new Vector3(-778.355, -2648.765, 12.81205), new Vector3(0, 0, -63.99991));
                API.createObject(-994492850, new Vector3(-779.3643, -2649.213, 12.81205), new Vector3(0, 0, -63.99991));
                API.createObject(-994492850, new Vector3(-780.35, -2649.62, 12.81205), new Vector3(0, 0, -63.99991));
                API.createObject(-994492850, new Vector3(-781.0364, -2649.948, 12.81205), new Vector3(0, 0, -63.99991));
                API.createObject(-994492850, new Vector3(-782.163, -2650.43, 12.81205), new Vector3(0, 0, -63.99991));
                API.createObject(-994492850, new Vector3(-783.1387, -2650.823, 12.81205), new Vector3(0, 0, -63.99991));
                API.createObject(-994492850, new Vector3(-784.101, -2651.232, 12.81205), new Vector3(0, 0, -63.99991));
                API.createObject(-994492850, new Vector3(-784.9805, -2651.626, 12.81205), new Vector3(0, 0, -63.99991));
                API.createObject(-994492850, new Vector3(-785.887, -2652.024, 12.81205), new Vector3(0, 0, -63.99991));
                API.createObject(-994492850, new Vector3(-787.1371, -2652.529, 12.81205), new Vector3(0, 0, -63.99991));
                API.createObject(-994492850, new Vector3(-788.3948, -2653.044, 12.81205), new Vector3(0, 0, -63.99991));

                #endregion}
            }
        }

        private void onClientEvent(Client player, string eventName, params object[] arguments)
        {
            switch (eventName)
            {
                case "JobGryzClothes":
                    JobGryzClothes(player);
                    break;
                case "JobGryzYval":
                    JobGryzYval(player);
                    break;
                    /*
                                    // Проверка очередности снятий коробок
                                    case "LOL":
                                        var BoxTypes = GryzBoxes.Where(i => i.Player == null).GroupBy(d => d.Type).Select(y => y.First()).Distinct().ToList(); // Смотрим какие типы ящиков остались                         
                                        if (BoxTypes.Count == 0)
                                        {
                                            API.sendChatMessageToPlayer(player, "~b~В данный момент товар для отгрузки отсутсвует.");
                                            return;
                                        }
                                        var rnd = new Random();
                                        int BoxType = BoxTypes[rnd.Next(0, BoxTypes.Count)].Type; // Выбираем тип груза и склада куда его отнести                               

                                        var myBox = GryzBoxes.Where(i => i.Type == BoxType && i.Player == null).FirstOrDefault(); // Ищем ящик который ни кому не принадлежит
                                        GryzBoxes.Remove(myBox);
                                        API.deleteEntity(myBox.Object);
                                        break;
                    */
            }
        }

        public void JobGryzClothes(Client player)
        {
            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
            if (pInfo.Job == (int)Jobs.NULL)
            {
                pInfo.Job = (int)Jobs.JOB_JOBGRYZ_ONE;
                API.sendNativeToPlayer(player, Hash.DO_SCREEN_FADE_OUT, 200);
                API.delay(200, true, () =>
                {
                    BlipInfo JobGryzClothes = new BlipInfo(player, new Vector3(-773.26, -2646.6, 13.0), 1, 60, false, 0, "Начать работу", Jobs.JOB_JOBGRYZ_ONE);
                    MarkerInfo JobGryzMarkerInfo = new MarkerInfo(player, 1, new Vector3(-774.0269, -2653.793, 12.83), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1), 100, 255, 255, 0, 1, 1, Jobs.JOB_JOBGRYZ_ONE);
                    JobGryzMarkerInfo.SetData("JobGryzAction", 1);
                    API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Заберите ящик из зоны разгрузки");

                    if (pInfo.sex)
                    {   // Мужской персонаж
                        API.setPlayerClothes(player, 11, 89, 0);
                        API.setPlayerClothes(player, 8, 59, 1);
                        API.setPlayerAccessory(player, 0, 2, 7);
                    }
                    else
                    {   // Женский персонаж
                        API.setPlayerClothes(player, 11, 14, 6);
                        API.setPlayerClothes(player, 3, 0, 0);
                        API.setPlayerClothes(player, 8, 2, 0);
                        API.setPlayerAccessory(player, 0, 29, 2);
                    }

                    MarkerInfo JobGryzZone = new MarkerInfo(player, new Vector3(-748.75, -2601.46, 13.1), 80, 10, Jobs.JOB_JOBGRYZ_ONE); // утанавливаем зону задания, чтобы ящики не воровал.
                    JobGryzZone.SetData("JobGryzZone", 1);
                    API.sendNativeToPlayer(player, Hash.DO_SCREEN_FADE_IN, 750);
                });
            }
            else
            {
                API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Сначала завершите предыдущую работу.");
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

                if (mInfo.Job == Jobs.JOB_JOBGRYZ_ONE)
                {
                    if (mInfo.HasData("JobGryzAction"))
                    {
                        Box myBox;
                        switch ((int)mInfo.GetData("JobGryzAction"))
                        {
                            case 1:
                                var BoxTypes = GryzBoxes.Where(i => i.Player == null).GroupBy(d => d.Type).Select(y => y.First()).Distinct().ToList(); // Смотрим какие типы ящиков остались                         
                                if (BoxTypes.Count == 0)
                                {
                                    API.sendChatMessageToPlayer(player, "~y~[Server] ~b~В данный момент товар для отгрузки отсутсвует.");
                                    return;
                                }
                                var rnd = new Random();
                                int BoxType = BoxTypes[rnd.Next(0, BoxTypes.Count)].Type; // Выбираем тип груза и склада куда его отнести                               

                                myBox = GryzBoxes.Where(i => i.Type == BoxType && i.Player == null).FirstOrDefault(); // Ищем ящик который ни кому не принадлежит
                                if (myBox != null)
                                {
                                    myBox.Player = player;
                                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                                    pInfo.FreezePlayer();
                                    API.playPlayerAnimation(player, (int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "anim@heists@load_box", "lift_box");
                                    lifeRP_GM.mainClass.Stream(1500, () =>
                                    {
                                        NetHandle BoxObject = myBox.Object;
                                        API.attachEntityToEntity(BoxObject, player, "60309", new Vector3(), new Vector3(-70, 2, 0));
                                        API.sendChatMessageToPlayer(player, string.Format("~y~[Server] ~b~Отнесите ящик на {0}-й склад", BoxType));
                                        pInfo.UnFreezePlayer();
                                    });
                                    #region "Маркеры складов"    
                                    switch (BoxType)
                                    {
                                        case 1:
                                            MarkerInfo minfo1 = new MarkerInfo(player, 0, new Vector3(-772.8375f, -2605.233f, 13.45), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1), 100, 0, 255, 0, 1, 1, Jobs.JOB_JOBGRYZ_ONE);
                                            minfo1.SetData("JobGryzAction", 0);//склад 1
                                            minfo1.SetData("JobGryzSklad", 1);
                                            break;
                                        case 2:
                                            MarkerInfo minfo2 = new MarkerInfo(player, 0, new Vector3(-770.2049, -2600.777, 13.28), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1), 100, 0, 255, 0, 1, 1, Jobs.JOB_JOBGRYZ_ONE);
                                            minfo2.SetData("JobGryzAction", 0);//склад 2
                                            minfo2.SetData("JobGryzSklad", 2);
                                            break;
                                        case 3:
                                            MarkerInfo minfo3 = new MarkerInfo(player, 0, new Vector3(-767.5933, -2596.403, 13.28), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1), 100, 0, 255, 0, 1, 1, Jobs.JOB_JOBGRYZ_ONE);
                                            minfo3.SetData("JobGryzAction", 0);//склад 3
                                            minfo3.SetData("JobGryzSklad", 3);
                                            break;
                                        case 4:
                                            MarkerInfo minfo4 = new MarkerInfo(player, 0, new Vector3(-764.9606, -2592.063, 13.28), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1), 100, 0, 255, 0, 1, 1, Jobs.JOB_JOBGRYZ_ONE);
                                            minfo4.SetData("JobGryzAction", 0);//склад 4
                                            minfo4.SetData("JobGryzSklad", 4);
                                            break;
                                        case 5:
                                            MarkerInfo minfo5 = new MarkerInfo(player, 0, new Vector3(-762.4879, -2587.585, 13.28), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1), 100, 0, 255, 0, 1, 1, Jobs.JOB_JOBGRYZ_ONE);
                                            minfo5.SetData("JobGryzAction", 0);//склад 5
                                            minfo5.SetData("JobGryzSklad", 5);
                                            break;
                                        case 6:
                                            MarkerInfo minfo6 = new MarkerInfo(player, 0, new Vector3(-759.9476, -2583.071, 13.28), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1), 100, 0, 255, 0, 1, 1, Jobs.JOB_JOBGRYZ_ONE);
                                            minfo6.SetData("JobGryzAction", 0);//склад 6
                                            minfo6.SetData("JobGryzSklad", 6);
                                            break;
                                        case 7:
                                            MarkerInfo minfo7 = new MarkerInfo(player, 0, new Vector3(-757.3046, -2578.625, 13.28), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1), 100, 0, 255, 0, 1, 1, Jobs.JOB_JOBGRYZ_ONE);
                                            minfo7.SetData("JobGryzAction", 0);//склад 7
                                            minfo7.SetData("JobGryzSklad", 7);
                                            break;
                                        case 8:
                                            MarkerInfo minfo8 = new MarkerInfo(player, 0, new Vector3(-754.7167, -2574.311, 13.28), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1), 100, 0, 255, 0, 1, 1, Jobs.JOB_JOBGRYZ_ONE);
                                            minfo8.SetData("JobGryzAction", 0);//склад 8
                                            minfo8.SetData("JobGryzSklad", 8);
                                            break;
                                        case 9:
                                            MarkerInfo minfo9 = new MarkerInfo(player, 0, new Vector3(-752.1079, -2569.722, 13.28), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1), 100, 0, 255, 0, 1, 1, Jobs.JOB_JOBGRYZ_ONE);
                                            minfo9.SetData("JobGryzAction", 0);//склад 9
                                            minfo9.SetData("JobGryzSklad", 9);
                                            break;
                                        case 10:
                                            MarkerInfo minfo10 = new MarkerInfo(player, 0, new Vector3(-749.6968, -2565.644, 13.28), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1), 100, 0, 255, 0, 1, 1, Jobs.JOB_JOBGRYZ_ONE);
                                            minfo10.SetData("JobGryzAction", 0);//склад 10
                                            minfo10.SetData("JobGryzSklad", 10);
                                            break;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    API.sendChatMessageToPlayer(player, "~y~[Server] ~b~В данный момент товар для отгрузки отсутсвует.");
                                }
                                break;
                            case 0:
                                if (mInfo.HasData("JobGryzSklad"))
                                {
                                    myBox = GryzBoxes.Where(i => i.Player == player).FirstOrDefault();
                                    if (myBox != null)
                                    {
                                        if ((int)mInfo.GetData("JobGryzSklad") == myBox.Type)
                                        {
                                            MarkerInfo minfo10 = new MarkerInfo(player, 1, new Vector3(-774.0269, -2653.793, 12.83), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1), 100, 255, 255, 0, 1, 1, Jobs.JOB_JOBGRYZ_ONE);
                                            minfo10.SetData("JobGryzAction", 1);
                                            API.playPlayerAnimation(player, (int)(AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "anim@heists@load_box", "load_box_2");
                                            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                                            pInfo.FreezePlayer();
                                            lifeRP_GM.mainClass.Stream(1600, () =>
                                            {
                                                API.detachEntity(myBox.Object);
                                                API.setEntityPosition(myBox.Object, new Vector3(55.31, -1259.78, -10.0));
                                                API.deleteEntity(myBox.Object);
                                                GryzBoxes.Remove(myBox);
                                                API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Вы положили ящик на склад");
                                                pInfo.UnFreezePlayer();
                                                API.stopPlayerAnimation(player);
                                                pInfo.jobMoney = pInfo.jobMoney + Money;
                                            });
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    else if (mInfo.HasData("JobGryzZone"))
                    {
                        // Работник в зоне, не трогаем, пусть работает.
                    }
                    else
                    {
                        API.triggerClientEvent(player, "JobGryzStart");
                    }
                }
            }
            else API.deleteColShape(colshape);
        }

        /// <summary>
        /// Завершение рабочего дня
        /// </summary>
        public void JobGryzYval(Client player)
        {
            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
            if (pInfo.Job == (int)Jobs.JOB_JOBGRYZ_ONE)
            {
                var myBox = GryzBoxes.Where(i => i.Player == player).FirstOrDefault();
                if (myBox != null)
                {
                    API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Сначала отнесите ящик на склад.");
                    return;
                }

                API.sendNativeToPlayer(player, Hash.DO_SCREEN_FADE_OUT, 200);
                API.delay(200, true, () =>
                {
                    pInfo.Job = (int)Jobs.NULL;
                    API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Вы закончили рабочий день.");
                    API.clearPlayerAccessory(player, 0); // удаляем аксесуары с головы
                    pInfo.UpdateDress(); // меняем одежду на пользовательскую

                    pInfo.money = pInfo.money + pInfo.jobMoney;
                    pInfo.jobMoney = 0;
                    pInfo.UpdateBD();
                    var Markers = pInfo.playerEntity.Where(x => x.Key == Constants.Marker); // Удаляем все пользовательские маркеры связанные с работой
                    foreach (var Marker in Markers.ToArray())
                    {
                        MarkerInfo bi = Marker.Value;
                        if (bi.Job == Jobs.JOB_JOBGRYZ_ONE)
                        {
                            bi.DeleteMarker();
                        }
                    }

                    var Blips = pInfo.playerEntity.Where(x => x.Key == Constants.Blip); // Удаляем все пользовательские блипы связанные с работой
                    foreach (var Blip in Blips.ToArray())
                    {
                        BlipInfo bi = Blip.Value;
                        if (bi.Job == Jobs.JOB_JOBGRYZ_ONE)
                        {
                            bi.DeleteBlip();
                        }
                    }
                    API.sendNativeToPlayer(player, Hash.DO_SCREEN_FADE_IN, 750);
                });
            }
            else
            {
                API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Вы здесь не работаете.");
            }
        }

        /// <summary>
        /// Выход из маркера
        /// </summary>
        private void onMarkerExit(ColShape colshape, NetHandle entity)
        {
            if (colshape.hasData(Constants.MarkerClass))
            {
                MarkerInfo mInfo = colshape.getData(Constants.MarkerClass);
                Client player;

                int type = (int)API.getEntityType(entity);
                if (type == 6) player = API.getPlayerFromHandle(entity);
                else return;

                if (mInfo.local && mInfo.player != player) return;

                if (mInfo.Job == Jobs.JOB_JOBGRYZ_ONE)
                {
                    if (mInfo.HasData("JobGryzAction"))
                    {
                        switch ((int)mInfo.GetData("JobGryzAction"))
                        {
                            case 0:
                                mInfo.DeleteMarker();
                                break;

                            case 1:
                                mInfo.DeleteMarker();
                                break;
                        }
                    }
                    else if (mInfo.HasData("JobGryzZone")) // Работник пытается украсть ящик.
                    {
                        PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                        if (pInfo.Job == (int)Jobs.JOB_JOBGRYZ_ONE)
                        {
                            pInfo.Job = (int)Jobs.NULL;
                            API.sendNativeToPlayer(player, Hash.DO_SCREEN_FADE_OUT, 200);
                            API.delay(200, true, () =>
                            {
                                var myBox = GryzBoxes.Where(i => i.Player == player).FirstOrDefault();
                                if (myBox != null)
                                {
                                        //Удаляем украденный ящик
                                        API.detachEntity(myBox.Object);
                                    API.setEntityPosition(myBox.Object, new Vector3(55.31, -1259.78, -10.0));
                                    API.deleteEntity(myBox.Object);
                                    GryzBoxes.Remove(myBox);
                                    API.stopPlayerAnimation(player);

                                    API.sendChatMessageToPlayer(player, "~y~[Server] ~r~Вы пытались украсть груз.");
                                    API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Ваш рабочий день окончен.");
                                    API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Вы ни чего не получите за работу.");
                                    API.clearPlayerAccessory(player, 0); // удаляем аксесуары с головы
                                    pInfo.UpdateDress(); // меняем одежду на пользовательскую
                                    pInfo.jobMoney = 0;
                                }
                                else
                                {
                                    API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Вы покинули зону. Ваш рабочий день окончен.");
                                    API.clearPlayerAccessory(player, 0); // удаляем аксесуары с головы
                                    pInfo.UpdateDress(); // меняем одежду на пользовательскую
                                    pInfo.money = pInfo.money + pInfo.jobMoney;
                                    pInfo.jobMoney = 0;
                                    pInfo.UpdateBD();
                                }

                                var Markers = pInfo.playerEntity.Where(x => x.Key == Constants.Marker); // Удаляем все пользовательские маркеры связанные с работой
                                foreach (var Marker in Markers.ToArray())
                                {
                                    MarkerInfo bi = Marker.Value;
                                    if (bi.Job == Jobs.JOB_JOBGRYZ_ONE)
                                    {
                                        bi.DeleteMarker();
                                    }
                                }

                                var Blips = pInfo.playerEntity.Where(x => x.Key == Constants.Blip); // Удаляем все пользовательские блипы связанные с работой
                                foreach (var Blip in Blips.ToArray())
                                {
                                    BlipInfo bi = Blip.Value;
                                    if (bi.Job == Jobs.JOB_JOBGRYZ_ONE)
                                    {
                                        bi.DeleteBlip();
                                    }
                                }
                                API.sendNativeToPlayer(player, Hash.DO_SCREEN_FADE_IN, 750);
                            });
                        }
                    }
                    else
                    {
                        API.triggerClientEvent(player, "JobGryzOff");
                    }
                }
            }
            else API.deleteColShape(colshape);
        }
    }
}