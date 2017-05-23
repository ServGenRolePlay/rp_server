using System;
using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

namespace RP_Server
{
    class TaxiCommands : Script
    {
        [Command("taxi")]
        public void calltaxi(Client player)
        {
            JobTaxi t = new JobTaxi();
            t.CMD_taxi(player);
        }
        [Command("job")]
        public void taxiJob(Client player)
        {
            JobTaxi t = new JobTaxi();
            t.makeTaxist(player);
        }
        [Command("quit")]
        public void quitTaxi(Client player)
        {
            JobTaxi t = new JobTaxi();
            t.removeTaxist(player);
        }
        [Command("accept")]
        public void acceptTaxi(Client player)
        {
            JobTaxi t = new JobTaxi();
            t.CMD_acceptTaxi(player);
        }
        [Command("finish")]
        public void finishTaxi(Client player)
        {
            JobTaxi t = new JobTaxi();
            t.CMD_finishTaxi(player);
        }
        [Command("gotaxi")]
        public void getPosition(Client player)
        {
            API.setEntityPosition(player, new Vector3(-912.36614990234, -2049.8950195313, 8.9043598175049));
        }
    }

    class JobTaxi : Script
    {
        class TaxiClient
        {
            public Client client;
            public double senderX;
            public double senderY;
        }

        private static List<TaxiClient> clients = new List<TaxiClient>();

        public JobTaxi()
        {
            API.onPlayerFinishedDownload += API_onPlayerFinishedDownload;
            API.onResourceStart += API_onResourceStart;
            API.onPlayerEnterVehicle += API_onPlayerEnterVehicle;
        }

        private void API_onPlayerFinishedDownload(Client player)
        {
            API.sendChatMessageToPlayer(player, "~r~/gotaxi чтобы тпшнуться на работу таксиста");
        }

        private void API_onResourceStart()
        {
            lock (Global.Lock)
            {
                // taxi vehicles
                #region TaxiVehicles   
                new VehInfo(VehicleHash.Taxi, new Vector3(-897.5254f, -2035.502f, 8.904017f), new Vector3(0, 0, -135.3084f), 0, 0, 100, Jobs.JOB_TAXI_PARK_ONE, 0);
                new VehInfo(VehicleHash.Taxi, new Vector3(-899.7668f, -2038.035f, 8.90379f), new Vector3(0, 0, -135.3176f), 0, 0, 100, Jobs.JOB_TAXI_PARK_ONE, 0);
                new VehInfo(VehicleHash.Taxi, new Vector3(-902.3703f, -2040.493f, 8.903711f), new Vector3(0, 0, -134.8492f), 0, 0, 100, Jobs.JOB_TAXI_PARK_ONE, 0);
                new VehInfo(VehicleHash.Taxi, new Vector3(-904.8074f, -2042.999f, 8.903594f), new Vector3(0, 0, -136.4715f), 0, 0, 100, Jobs.JOB_TAXI_PARK_ONE, 0);
                new VehInfo(VehicleHash.Taxi, new Vector3(-907.111f, -2045.528f, 8.903583f), new Vector3(0, 0, -136.4113f), 0, 0, 100, Jobs.JOB_TAXI_PARK_ONE, 0);
                new VehInfo(VehicleHash.Taxi, new Vector3(-909.6328f, -2047.868f, 8.903583f), new Vector3(0, 0, -135.688f), 0, 0, 100, Jobs.JOB_TAXI_PARK_ONE, 0);
                new VehInfo(VehicleHash.Taxi, new Vector3(-912.2091f, -2050.099f, 8.903601f), new Vector3(0, 0, -134.351f), 0, 0, 100, Jobs.JOB_TAXI_PARK_ONE, 0);
                new VehInfo(VehicleHash.Taxi, new Vector3(-882.4277f, -2048.99f, 8.903764f), new Vector3(0, 0, 44.82626f), 0, 0, 100, Jobs.JOB_TAXI_PARK_ONE, 0);
                new VehInfo(VehicleHash.Taxi, new Vector3(-884.9864f, -2051.37f, 8.90558f), new Vector3(0, 0, 45.86894f), 0, 0, 100, Jobs.JOB_TAXI_PARK_ONE, 0);
                new VehInfo(VehicleHash.Taxi, new Vector3(-887.3887f, -2053.851f, 8.911674f), new Vector3(0, 0, 42.99739f), 0, 0, 100, Jobs.JOB_TAXI_PARK_ONE, 0);
                new VehInfo(VehicleHash.Taxi, new Vector3(-889.7358f, -2056.304f, 8.910381f), new Vector3(0, 0, 44.67617f), 0, 0, 100, Jobs.JOB_TAXI_PARK_ONE, 0);
                #endregion

                new BlipInfo(new Vector3(-915.1155, -2038.782, 8.40499), 56, 5, false, 0, "Downtown Cab Co.");
                MarkerInfo mInfo = new MarkerInfo(1, new Vector3(-915.1155, -2038.782, 8.40499), new Vector3(1, 1, 1), new Vector3(35.5, 1, 1), new Vector3(1.5, 1.5, 2), 100, 23, 182, 71, 1, 2);
                mInfo.SetData("JobTaxiAction", 1);
                new TLabelInfo("Работа в такси", new Vector3(-915.1155, -2038.782, 9.40499), 25, 1);
            }
        }
        // метод для вызова такси.
        public void CMD_taxi(Client player)
        {
            if (API.getEntitySyncedData(player, "PLAYER_JOB_HAS_CALLED_TAXI") != true)
            {
                bool taxiWasCalled = false;
                foreach (var driver in API.getAllPlayers())
                {
                    PlayerInfo pInfo = API.getEntityData(driver.handle, Constants.PlayerAccount);
                    if ((Jobs)pInfo.Job == Jobs.JOB_TAXI_PARK_ONE)
                    {
                        if (driver.isInVehicle)
                        {
                            if (API.shared.getEntitySyncedData(driver.vehicle, Constants.VehJob) == (int)Jobs.JOB_TAXI_PARK_ONE)
                            {
                                API.sendPictureNotificationToPlayer(driver, player.name + " вызывает такси, желаете принять вызов?", "CHAR_TAXI", 0, 1, "Downtown Cab Co.", "Job");
                                taxiWasCalled = true;
                            }
                        }
                    }
                }
                if (taxiWasCalled)
                {
                    API.sendChatMessageToPlayer(player, "~y~[Server] ~o~Такси было вызвано, ожидайте..");
                    TaxiClient t = new TaxiClient();
                    t.client = player;
                    t.senderX = API.getEntityPosition(player).X;
                    t.senderY = API.getEntityPosition(player).Y;
                    clients.Add(t);
                    API.setEntitySyncedData(player, "PLAYER_JOB_HAS_CALLED_TAXI", true);
                }
                else
                {
                    API.sendChatMessageToPlayer(player, "~y~[Server] ~r~Работающих таксистов нету.");
                }
            }
            else
                API.sendChatMessageToPlayer(player, "~y~[Server] ~r~Вы уже вызвали такси.");
        }

        // метод для принятия вызова(таксисту)
        public void CMD_acceptTaxi(Client player)
        {
            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
            if ((Jobs)pInfo.Job == Jobs.JOB_TAXI_PARK_ONE)
            {
                if (player.isInVehicle)
                {
                    if (API.shared.getEntitySyncedData(player.vehicle, Constants.VehJob) == (int)Jobs.JOB_TAXI_PARK_ONE)
                    {
                        if (clients.Count > 0)
                        {
                            if (API.getEntitySyncedData(player, "PLAYER_JOB_TAXI_IS_BUSY") != true)
                            {
                                API.setEntitySyncedData(player, "PLAYER_JOB_TAXI_IS_BUSY", true);
                                API.sendChatMessageToPlayer(player, "~g~Вы приняли вызов, езжайте на место.");
                                API.triggerClientEvent(player, "markOnMap", clients[clients.Count - 1].senderX, clients[clients.Count - 1].senderY);
                                API.sendPictureNotificationToPlayer(clients[clients.Count - 1].client, player.name + " выехал на вызов, ожидайте на месте.", "CHAR_TAXI", 0, 1, "Downtown Cab Co.", "Message");
                                API.setEntitySyncedData(clients[clients.Count - 1].client, "PLAYER_JOB_HAS_CALLED_TAXI", false);
                                clients.Remove(clients[clients.Count - 1]);
                            }
                            else
                                API.sendChatMessageToPlayer(player, "~y~[Server] ~r~У вас уже есть заказ.");
                        }
                        else
                            API.sendChatMessageToPlayer(player, "~y~[Server] ~o~Вызовов на такси нету.");
                    }
                }
            }
        }

        // метод закончить рейс такси
        public void CMD_finishTaxi(Client player)
        {
            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
            if ((Jobs)pInfo.Job == Jobs.JOB_TAXI_PARK_ONE)
            {
                if (player.isInVehicle)
                {
                    if (API.shared.getEntitySyncedData(player.vehicle, Constants.VehJob) == (int)Jobs.JOB_TAXI_PARK_ONE)
                    {
                        if (API.getEntitySyncedData(player, "PLAYER_JOB_TAXI_IS_BUSY") == true)
                        {
                            API.triggerClientEvent(player, "finishTaxiClient");
                            API.setEntitySyncedData(player, "PLAYER_JOB_TAXI_IS_BUSY", false);
                            API.sendChatMessageToPlayer(player, "~y~[Server] ~o~Вы закончили выполнять текущий заказ.");
                        }
                        else
                            API.sendChatMessageToPlayer(player, "~y~[Server] ~o~У вас нет заказа.");
                    }
                }
            }
        }

        // метод при устройстве на работу такси
        public void makeTaxist(Client player)
        {
            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
            if ((Jobs)pInfo.Job != Jobs.JOB_TAXI_PARK_ONE)
            {
                pInfo.Job = (int)Jobs.JOB_TAXI_PARK_ONE;
                API.sendChatMessageToPlayer(player, "~y~[Server] ~g~Вы устроились на работу таксиста.");
                API.sendChatMessageToPlayer(player, "~y~[Server] ~o~Садитесь в любую из машин такси.");
            }
            else
            {
                API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Сначала завершите предыдущую работу.");
            }
        }

        // метод когда увольняешься с работы такси
        public void removeTaxist(Client player)
        {
            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
            if ((Jobs)pInfo.Job == Jobs.JOB_TAXI_PARK_ONE)
            {
                pInfo.Job = (int)Jobs.NULL;
                API.sendChatMessageToPlayer(player, "~y~[Server] ~g~Вы уволились с работы таксиста.");
            }
            else
            {
                API.sendChatMessageToPlayer(player, "~y~[Server] ~b~Вы здесь не работаете.");
            }
        }

        // чтоб выкидывало из машины, когда пытаешься сесть в машину такси, при этом не работая таксистом.
        private void API_onPlayerEnterVehicle(Client player, NetHandle vehicle)
        {
            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
            if (API.shared.getEntitySyncedData(vehicle, Constants.VehJob) == (int)Jobs.JOB_TAXI_PARK_ONE)
            {
                Client[] passengers = API.getVehicleOccupants(vehicle);
                if (passengers.Length == 1 && (Jobs)pInfo.Job != Jobs.JOB_TAXI_PARK_ONE)
                {
                    API.warpPlayerOutOfVehicle(player);
                    API.sendChatMessageToPlayer(player, "~y~[Server] ~r~У Вас нет доступа к данному транспортному\r\n~r~средству.");
                }
            }
        }
    }
}