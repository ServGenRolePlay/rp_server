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
    public class JobImmigrant : Script
    {
        private Random SuperRandom = new Random();
        private byte BottleCost = 1;
        private byte PaperCost = 3;
        private byte CabelCost = 5;

        private Vector3[] TrashCoords = new Vector3[49]
        {
            new Vector3(120.300590515137f, -1327.88012695313f, 28.3668689727783f),
            new Vector3(122.139869689941f, -1326.2724609375f, 28.3936595916748f),
            new Vector3(137.315368652344f, -1313.58825683594f, 28.2048759460449f),
            new Vector3(155.005325317383f, -1309.21569824219f, 28.2023086547852f),
            new Vector3(156.111862182617f, -1306.90954589844f, 28.2023029327393f),
            new Vector3(166.222137451172f, -1293.95727539063f, 28.4507827758789f),
            new Vector3(165.014205932617f, -1286.60266113281f, 28.2985935211182f),
            new Vector3(146.639434814453f, -1289.49853515625f, 28.3321514129639f),
            new Vector3(144.544616699219f, -1290.6416015625f, 28.3571815490723f),
            new Vector3(144.816009521484f, -1262.58178710938f, 28.249044418335f),
            new Vector3(143.522979736328f, -1260.06774902344f, 28.2932910919189f),
            new Vector3(91.3440017700195f, -1284.18823242188f, 28.2853698730469f),
            new Vector3(89.642707824707f, -1285.15698242188f, 28.2959613800049f),
            new Vector3(91.2898635864258f, -1304.55041503906f, 28.241907119751f),
            new Vector3(92.146110534668f, -1306.13989257813f, 28.2794494628906f),
            new Vector3(-10.7335319519043f, -1308.46704101563f, 28.2928924560547f),
            new Vector3(-22.476863861084f, -1295.73181152344f, 28.35573387146f),
            new Vector3(-43.7875823974609f, -1299.85327148438f, 28.0883731842041f),
            new Vector3(-43.966739654541f, -1285.81518554688f, 28.103178024292f),
            new Vector3(-55.9420394897461f, -1265.353515625f, 28.1245098114014f),
            new Vector3(-78.2049713134766f, -1265.94799804688f, 28.1764106750488f),
            new Vector3(-80.9084548950195f, -1265.77575683594f, 28.1635589599609f),
            new Vector3(-87.35595703125f, -1278.26318359375f, 28.2981071472168f),
            new Vector3(-87.375732421875f, -1287.60803222656f, 28.2981338500977f),
            new Vector3(-87.4567337036133f, -1298.59680175781f, 28.3011837005615f),
            new Vector3(-87.2403030395508f, -1330.27673339844f, 28.2935619354248f),
            new Vector3(-80.1475448608398f, -1314.29724121094f, 28.2296085357666f),
            new Vector3(-70.3141479492188f, -1313.78161621094f, 28.2496547698975f),
            new Vector3(-49.3782234191895f, -1315.83850097656f, 28.1623477935791f),
            new Vector3(-39.0160636901855f, -1352.06335449219f, 28.3386993408203f),
            new Vector3(-28.0031623840332f, -1352.20288085938f, 28.317850112915f),
            new Vector3(2.18822073936462f, -1351.44360351563f, 28.3241424560547f),
            new Vector3(65.8645248413086f, -1392.42565917969f, 28.38112449646f),
            new Vector3(65.8609771728516f, -1395.43603515625f, 28.3781318664551f),
            new Vector3(66.592658996582f, -1408.77038574219f, 28.3520832061768f),
            new Vector3(48.9806098937988f, -1400.16015625f, 28.3602542877197f),
            new Vector3(54.7644805908203f, -1436.11730957031f, 28.3117389678955f),
            new Vector3(53.3917045593262f, -1437.69885253906f, 28.3117389678955f),
            new Vector3(39.9233818054199f, -1453.64819335938f, 28.3116855621338f),
            new Vector3(15.7049961090088f, -1411.13757324219f, 28.3340492248535f),
            new Vector3(13.2799835205078f, -1410.92321777344f, 28.3277187347412f),
            new Vector3(-17.5822906494141f, -1390.9326171875f, 28.3694934844971f),
            new Vector3(-29.9385414123535f, -1416.47766113281f, 28.314208984375f),
            new Vector3(-53.8897514343262f, -1416.6064453125f, 28.3231792449951f),
            new Vector3(-79.2503890991211f, -1425.30334472656f, 28.663948059082f),
            new Vector3(-79.2948837280273f, -1426.93090820313f, 28.6720333099365f),
            new Vector3(-76.8773422241211f, -1406.51086425781f, 28.3207550048828f),
            new Vector3(-79.3566818237305f, -1390.38891601563f, 28.3207607269287f),
            new Vector3(-93.4373474121094f, -1470.71960449219f, 32.074951171875f)
        };

        private static List<CylinderColShape> JobImmigrantMarkers = new List<CylinderColShape>();
        private static List<Blip> JobImmigrantBlips = new List<Blip>();
        private static List<TextLabel> JobImmigrantTextLabels = new List<TextLabel>();

        public JobImmigrant()
        {
            API.onResourceStart += onResourceStart;
            API.onEntityEnterColShape += onMarkerEnter;
            API.onPlayerDeath += onPlayerDeathHandler;
        }

        private void onPlayerDeathHandler(Client player, NetHandle entityKiller, int weapon)
        {
            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
            if ((Jobs)pInfo.Job == Jobs.JOB_IMMIGRANT)
            {
                API.sendChatMessageToPlayer(player, "~y~[Server] ~w~Вы погибли. ~r~Миссия провалена.");
                JobImmigrantYval(player);
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
                PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);

                if (mInfo.HasData("JOB_IMMIGRANT_SEARCH"))
                {
                    var percent = ((pInfo.Bottles * 0.1 + pInfo.Paper + pInfo.Cabel) / 40) * 100;
                    if (percent > 100)
                    {
                        API.sendChatMessageToPlayer(player, "~y~[Server] ~r~Рюкзак переполнен!");
                        API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Сдайте ваш инвентарь на завод!");
                        return;
                    }
                    API.setEntityRotation(player, new Vector3(0, 0, -90));
                    API.playPlayerAnimation(player, 0, "timetable@maid@cleaning_surface@idle_a", "idle_a");
                    byte bottles = (byte)SuperRandom.Next(1, 5);
                    pInfo.Bottles += bottles;
                    byte paper = (byte)SuperRandom.Next(0, 3);
                    pInfo.Paper += paper;
                    byte cabel = (byte)SuperRandom.Next(0, 3);
                    pInfo.Cabel += cabel;

                    lifeRP_GM.mainClass.Stream(4500, () =>
                    {
                        API.triggerClientEvent(player, "SEND_BOMJ_DATA", pInfo.Bottles.ToString(), pInfo.Paper.ToString(), pInfo.Cabel.ToString());
                        API.sendChatMessageToPlayer(API.getPlayerFromHandle(entity), string.Format("~y~Вы нашли:\r\nБутылок - {0} шт.!\r\nБумаги - {1} кг.!\r\nЦветного металла - {2} кг.!", bottles, paper, cabel));
                    });

                    var binfo = mInfo.GetData("JOB_IMMIGRANT_SEARCH");
                    mInfo.DeleteMarker();
                    if (binfo != null)
                    {
                        binfo.DeleteBlip();
                    }
                }
                else if (mInfo.HasData("JOB_IMMIGRANT_CVETMET"))
                {
                    if (pInfo.Job == (int)Jobs.NULL)
                    {
                        API.sendChatMessageToPlayer(player, "~y~[Server] ~b~У вас нету рюкзака для продажи.");
                    }
                    else if (pInfo.Job == (int)Jobs.JOB_IMMIGRANT)
                    {
                        if (pInfo.Cabel != 0)
                        {
                            int CabelMoney = pInfo.Cabel * CabelCost;
                            pInfo.money += CabelMoney;
                            pInfo.Cabel = 0;
                            pInfo.UpdateBD();
                            API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Вы продали цветной металл на: ~r~$" + CabelMoney.ToString());
                            API.triggerClientEvent(player, "SEND_BOMJ_DATA", pInfo.Bottles.ToString(), pInfo.Paper.ToString(), pInfo.Cabel.ToString());
                            if (pInfo.Bottles == 0 && pInfo.Cabel == 0 && pInfo.Paper == 0)
                            {
                                JobImmigrantYval(player);
                            }
                        }
                        else API.sendChatMessageToPlayer(player, "~y~[Server] ~b~У вас нету цвет. металла!");
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Сначала завершите старую работу.");
                    }
                    return;
                }
                else if (mInfo.HasData("JOB_IMMIGRANT_STEKLO"))
                {
                    if (pInfo.Job == (int)Jobs.NULL)
                    {
                        API.sendChatMessageToPlayer(player, "~y~[Server] ~b~У вас нету рюкзака для продажи.");
                    }
                    else if (pInfo.Job == (int)Jobs.JOB_IMMIGRANT)
                    {
                        if (pInfo.Bottles != 0)
                        {
                            int BottlesMoney = pInfo.Bottles * BottleCost;
                            pInfo.money += BottlesMoney;
                            pInfo.Bottles = 0;
                            pInfo.UpdateBD();
                            API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Вы продали бутылки на: ~r~$" + BottlesMoney.ToString());
                            API.triggerClientEvent(player, "SEND_BOMJ_DATA", pInfo.Bottles.ToString(), pInfo.Paper.ToString(), pInfo.Cabel.ToString());
                            if (pInfo.Bottles == 0 && pInfo.Cabel == 0 && pInfo.Paper == 0)
                            {
                                JobImmigrantYval(player);
                            }
                        }
                        else API.sendChatMessageToPlayer(player, "~y~[Server] ~b~У вас нету бутылок!");
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Сначала завершите старую работу.");
                    }
                    return;
                }
                else if (mInfo.HasData("JOB_IMMIGRANT_BUMAGA"))
                {
                    if (pInfo.Job == (int)Jobs.NULL)
                    {
                        API.sendChatMessageToPlayer(player, "~y~[Server] ~b~У вас нету рюкзака для продажи.");
                    }
                    else if (pInfo.Job == (int)Jobs.JOB_IMMIGRANT)
                    {
                        if (pInfo.Paper != 0)
                        {
                            int PaperMoney = pInfo.Paper * PaperCost;
                            pInfo.money += PaperMoney;
                            pInfo.Paper = 0;
                            pInfo.UpdateBD();
                            API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Вы продали бумагу на: ~r~$" + PaperMoney.ToString());
                            API.triggerClientEvent(player, "SEND_BOMJ_DATA", pInfo.Bottles.ToString(), pInfo.Paper.ToString(), pInfo.Cabel.ToString());
                            if (pInfo.Bottles == 0 && pInfo.Cabel == 0 && pInfo.Paper == 0)
                            {
                                JobImmigrantYval(player);
                            }
                        }
                        else API.sendChatMessageToPlayer(player, "~y~[Server] ~b~У вас нету бумаги!");
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Сначала завершите старую работу.");
                    }
                    return;
                }
                else if (mInfo.HasData("JOB_IMMIGRANT"))
                {
                    if (pInfo.Job == (int)Jobs.NULL)
                    {
                        for (byte i = 0; i < TrashCoords.Length; i++)
                        {
                            var mark = new MarkerInfo(player, 1, new Vector3(TrashCoords[i].X, TrashCoords[i].Y, TrashCoords[i].Z), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 1), 100, 236, 147, 89, 1, 1, Jobs.JOB_IMMIGRANT);
                            var blip = new BlipInfo(player, new Vector3(TrashCoords[i].X, TrashCoords[i].Y, TrashCoords[i].Z), 1, 17, true, 0, "Мусорный ящик", Jobs.JOB_IMMIGRANT);
                            mark.SetData("JOB_IMMIGRANT_SEARCH", blip);
                        }

                        new MarkerInfo(player, 1, new Vector3(1083.46, -1974.33, 30.0165), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 236, 147, 89, 1, 2, Jobs.JOB_IMMIGRANT).SetData("JOB_IMMIGRANT_CVETMET", 1);
                        new MarkerInfo(player, 1, new Vector3(1137.99, -2046.25, 30.0165), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 236, 147, 89, 1, 2, Jobs.JOB_IMMIGRANT).SetData("JOB_IMMIGRANT_STEKLO", 1);
                        new MarkerInfo(player, 1, new Vector3(1070.04, -1971.78, 30.0321), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 236, 147, 89, 1, 2, Jobs.JOB_IMMIGRANT).SetData("JOB_IMMIGRANT_BUMAGA", 1);

                        new BlipInfo(player, new Vector3(1083.46, -1974.33, 30.0165), 440, 17, true, 0, "Пункт приема цветного металла", Jobs.JOB_IMMIGRANT);
                        new BlipInfo(player, new Vector3(1137.99, -2046.25, 30.0165), 440, 17, true, 0, "Пункт приема стеклотары", Jobs.JOB_IMMIGRANT);
                        new BlipInfo(player, new Vector3(1070.04, -1971.78, 30.0321), 440, 17, true, 0, "Пункт приема бумаги", Jobs.JOB_IMMIGRANT);

                        new TLabelInfo(player, "Пункт приема цветного металла", new Vector3(1083.46, -1974.33, 31.0165), 25, 1, false, 0, Jobs.JOB_IMMIGRANT);
                        new TLabelInfo(player, "Пункт приема стеклотары", new Vector3(1137.99, -2046.25, 31.0165), 25, 1, false, 0, Jobs.JOB_IMMIGRANT);
                        new TLabelInfo(player, "Пункт приема бумаги", new Vector3(1070.04, -1971.78, 31.0321), 25, 1, false, 0, Jobs.JOB_IMMIGRANT);

                        pInfo.PISUN = API.createObject(-2137120552, new Vector3(806.8278, -809.9581, 25.2029), new Vector3(0, 0, 0));
                        API.sendChatMessageToPlayer(player, "~y~[Server] ~g~Вы устроились на работу.");
                        pInfo.Job = (int)Jobs.JOB_IMMIGRANT;
                        API.attachEntityToEntity(pInfo.PISUN, player.handle, "SKEL_Spine2", new Vector3(0.15, -0.25, 0), new Vector3(-180, -90, -180));
                        new ServerWayPoint().CreateWayPoint(player, 1073, -1965);
                        pInfo.Bottles += 1;
                        pInfo.Paper += 1;
                        pInfo.Cabel += 1;
                        API.playPlayerAnimation(player, 0, "timetable@maid@cleaning_surface@idle_a", "idle_a");
                        API.triggerClientEvent(player, "OPEN_TEST_BAG");
                        lifeRP_GM.mainClass.Stream(4500, () =>
                        {
                            API.sendChatMessageToPlayer(player, string.Format("~y~Вы нашли:\r\nБутылок - {0} шт.!\r\nБумаги - {1} кг.!\r\nЦветного металла - {2} кг.!", 1, 1, 1));
                            API.triggerClientEvent(player, "SEND_BOMJ_DATA", pInfo.Bottles.ToString(), pInfo.Paper.ToString(), pInfo.Cabel.ToString());
                        });
                    }
                    else if (pInfo.Job == (int)Jobs.JOB_IMMIGRANT)
                    {
                        API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Сдайте ваш инвентарь на завод!");
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Сначала завершите старую работу.");
                    }
                    return;
                }
            }
        }

        private void onResourceStart()
        {
            lock (Global.Lock)
            {
                try
                {
                    var JOB_IMMIGRANT_MARKER = new MarkerInfo(1, new Vector3(84.0330581665039, -1291.85900878906, 28.2591037750244), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 1), 100, 236, 147, 89, 1, 2);
                    JOB_IMMIGRANT_MARKER.SetData("JOB_IMMIGRANT", 1);
                    new BlipInfo(new Vector3(84.0330581665039, -1291.85900878906, 28.2591037750244), 408, 2, true, 0, "Работа иммигранта");
                    new TLabelInfo("Работа иммигранта", new Vector3(84.0330581665039, -1291.85900878906, 30.2591037750244), 25, 1);                   
                }
                catch (Exception exp)
                {
                    API.consoleOutput("Произошла ошибка при инициализации миссии иммигранта: " + exp.Message);
                    return;
                }
            }
        }

        private void JobImmigrantYval(Client player)//закончить рабочий день
        {
            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
            pInfo.Bottles = 0;
            pInfo.Cabel = 0;
            pInfo.Paper = 0;
            pInfo.Job = (int)Jobs.NULL;
            API.deleteEntity(pInfo.PISUN);

            API.triggerClientEvent(player, "CLOSE_TEST_BAG");
            API.sendChatMessageToPlayer(player, "~y~[Server] ~g~Вы завершили работу.");

            pInfo.DeletePlayerMarkers(); // Удаляем все пользовательские маркеры  
            pInfo.DeletePlayerBlips(); // Удаляем все пользовательские блипы            
            pInfo.DeletePlayerTLabels(); // Удаляем все пользовательские текстовые поля   
        }
    }
}