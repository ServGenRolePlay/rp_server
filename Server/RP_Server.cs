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
    public class lifeRP_GM : Script
    {
        public static bool IsDbServerLocal = true; // Расположен ли сервер БД на локалке?
        public static bool Debug = true; // Отправлять в чат разработчиков информацию?

        public SqlConnecter sqlCon = IsDbServerLocal ? new SqlConnecter("127.0.0.1", "test_01", "test_01", "") : new SqlConnecter("37.59.147.15", "gta_mp", "gta_mp", "mqYglvT6"); // подключение к субд  

        public static lifeRP_GM mainClass; // Экземпляр основного класса исполняемого сервером
        public bool connectOpen = true; // доступно ли подключение
        public bool whitelistEnabled = true; // включен / выключен whitelist
        public List<Client> players = new List<Client>(); // массив имен игроков для ID системы
        public bool allchat = false; // доступен ли общий чат

        public lifeRP_GM()
        {
            API.onResourceStart += onResourceStart;
            API.onChatMessage += onChatMessage;
            API.onUpdate += onUpdate;
            API.onEntityDataChange += onEntityDataChange;
        }

        private void onResourceStart()
        {
            try
            {
                API.consoleOutput("4-life RP GameMode Starting...");
                mainClass = this;
                API.setGamemodeName("ServGun RP v0.8");
                API.consoleOutput(string.Format("Check the database connection - {0}.", sqlCon.checkConnect().ToString()));
                whitelistEnabled = API.getSetting<bool>("whiteList_Enabled");
                API.consoleOutput(string.Format("WhiteList - {0}.", whitelistEnabled.ToString()));
                API.consoleOutput(string.Format("ACL is Enabled - {0}.", API.isAclEnabled().ToString()));
                if (API.isAclEnabled())
                {
                    connectOpen = false;
                    API.consoleOutput(string.Format("Connection is not possible until ACL is Enabled!"));
                    return;
                }
                int max_player = API.getMaxPlayers();
                InitializeID(max_player);
                API.consoleOutput(string.Format("Initialization ID system completed. Count: {0}", max_player.ToString()));
                new lifeRP_MM().CreateVehList();
                API.consoleOutput(string.Format("Created {0} vechiles.", API.getAllVehicles().Count.ToString()));
                new lifeRP_MM().CreatMarkersList();
                API.consoleOutput(string.Format("Created {0} markers.", ServerEntity.EntityClass.Markers.Count().ToString()));
                new lifeRP_MM().CreateBlipList();
                API.consoleOutput(string.Format("Created {0} blips.", API.getAllBlips().Count.ToString()));
                new lifeRP_MM().CreatetLabelList();
                API.consoleOutput(string.Format("Created {0} textLabels.", API.getAllLabels().Count().ToString()));
                API.consoleOutput("4-life RP GameMode Starting - success! ");
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка при инициализации мода: " + exp.Message);
                return;
            }
        }

        private void onChatMessage(Client sender, string message, CancelEventArgs cancel)
        {
            try
            {
                if (Debug) ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] ~w~onChatMessage");
                cancel.Cancel = true;
                ProxDetector(15f, sender, message);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в onChatMessage - " + exp.ToString());
                ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onChatMessage");
                return;
            }
        }

        private void onUpdate()
        {
            try
            {
                DateTime dt = DateTime.Now;
                if (dt.Minute == 0 && dt.Second == 0) payday();
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в onUpdate - " + exp.ToString());
                ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onUpdate");
                return;
            }
        }

        private void onEntityDataChange(NetHandle entity, string key, object oldValue)
        {
            try
            {
                if (lifeRP_GM.Debug) ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] ~w~onEntityDataChange " + key);
                string[] mKey = key.Split('_');
                switch (mKey[0])
                {
                    case Constants.Player:
                    case Constants.Veh:
                    case Constants.Marker:
                    case Constants.Blip:
                    case Constants.tLabel:
                    case Constants.PlayerJob:
                        break;

                    case Constants.Global:
                        break;

                    default:
                        API.consoleOutput("Неизвестная модификация данных, Ключ: {0} Тип объекта: {1} ", key, (int)API.getEntityType(entity));
                        ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] Произошла Неизвестная модификация данных");
                        break;
                }
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в onEntityDataChange Ключ -  " + key + " " + exp.ToString());
                ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onEntityDataChange");
                return;
            }
        }

        #region Методы
        /// <summary>
        /// Сохранить в файл текст.
        /// </summary>
        /// <param name="text">Текст для сохранения.</param>
        public void saveToFile(string fileName, string text)
        {
            string FileName = Path.Combine(Environment.CurrentDirectory, fileName);
            using (StreamWriter sw = new StreamWriter(FileName, true, Encoding.Default))
            {
                sw.WriteLine(text);
            }
        }

        /// <summary>
        /// Логика во время нового часа.
        /// </summary>
        public void payday()
        {
            Stream(0, () =>
            {
                var players = API.getAllPlayers();
                foreach (var player in players)
                {
                    if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                    {
                        PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                        if (pInfo.islogin)
                        {
                            string str = "";
                            if (OverTime(pInfo.overtime) >= 25)
                            {
                                pInfo.exp++;
                                pInfo.overtime = DateTime.Now;
                            }

                            if ((pInfo.lvl + 2) * (pInfo.lvl + 1) <= pInfo.exp)
                            {
                                pInfo.lvl++;
                                pInfo.exp = 0;
                                API.sendChatMessageToPlayer(player, chatMessage("~g~Поздравляем с получением нового уровня."));
                            }
                            str += "Оклад: 0$\n";
                            str += "Переводов в этом месяце: 0$\n";
                            API.sendPictureNotificationToPlayer(player, str, "CHAR_BANK_MAZE", 0, 0, "=======[Maze Bank]=======", "Уведомление");
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Оправка сообщения игрокам с учетом растояния и измерения от отправителя.
        /// </summary>
        /// <param name="radius">Дистанция от отправителя.</param>
        /// <param name="player">Отправитель.</param>
        /// <param name="message">Сообщение.</param>
        public void ProxDetector(float radius, Client player, string message)
        {
            var players = API.getPlayersInRadiusOfPlayer(radius, player);
            var myDimension = API.getEntityDimension(player);
            string str1 = chatMessage(string.Format("~y~{0}: ~s~{1}", player.nametag, message));
            string str2 = chatMessage(string.Format("~y~{0}: ~m~{1}", player.nametag, message));
            string str3 = chatMessage(string.Format("~y~{0}: ~c~{1}", player.nametag, message));

            foreach (Client c in players)
            {
                var cDimension = API.getEntityDimension(c);
                if (myDimension == cDimension)
                {
                    if (player.position.DistanceTo(c.position) <= radius / 8) API.sendChatMessageToPlayer(c, str1);
                    else if (player.position.DistanceTo(c.position) <= radius / 2) API.sendChatMessageToPlayer(c, str2);
                    else if (player.position.DistanceTo(c.position) <= radius) API.sendChatMessageToPlayer(c, str3);
                }
            }
        }

        /// <summary>
        /// Оправка сообщения игрокам с учетом растояния и измерения от отправителя с указанием цвета.
        /// </summary>
        /// <param name="radius">Дистанция от отправителя.</param>
        /// <param name="player">Отправитель.</param>
        /// <param name="message">Сообщение.</param>
        /// <param name="col">Цвет.</param>
        public void ProxDetector(float radius, Client player, string message, string col)
        {
            var players = API.getPlayersInRadiusOfPlayer(radius, player);
            var myDimension = API.getEntityDimension(player);
            message = chatMessage(message);
            foreach (Client c in players)
            {
                var cDimension = API.getEntityDimension(c);
                if (myDimension == cDimension) API.sendChatMessageToPlayer(c, col, message);
            }
        }

        /// <summary>
        /// Расчет разници между указанным временем и текущем.
        /// </summary>
        /// <param name="dt">Расчетное время.</param>
        /// <returns>Разница в минутах(без остатка).</returns>
        public int OverTime(DateTime data)
        {
            var respontDT = DateTime.Now - data;
            return (int)respontDT.TotalMinutes;
        }

        /// <summary>
        /// Делит текст и переносит на новые строки чтобы правильно отобразить в игровом чате.
        /// </summary>
        /// <param name="text">Текст сообщения.</param>
        /// <returns>Готовая для отображения сообщение.</returns>
        public string chatMessage(string text)
        {
            string message = "";
            while (true)
            {
                if (text.Length > 60)
                {
                    if (text[58] == ' ')
                    {
                        message += text.Substring(0, 58) + "\r\n";
                        text = text.Substring(58);
                    }
                    else
                    {
                        message += text.Substring(0, 57) + "-\r\n";
                        text = text.Substring(57);
                    }
                }
                else
                {
                    message += text;
                    break;
                }
            }
            return message;
        }

        /// <summary>
        /// Выполнение кода в отдельном потоке с задержкой.
        /// </summary>
        /// <param name="ms">Задержка (в милисекундах).</param>
        /// <param name="action">Код.</param>
        public void Stream(int ms, Action action)
        {
            new Task(() =>
            {
                API api = new API();
                api.sleep(ms);
                action();
            }).Start();
        }

        /// <summary>
        /// Расчет координат перед заданными координатами на определенным расстоянии. Если нужно определить в другой стороне изменяйте Угол: -90 - справа; +90 - слева; -180 сзади.
        /// </summary>
        /// <param name="x">Коррдината Х.</param>
        /// <param name="y">Координата Y.</param>
        /// <param name="z">Координата Z.</param>
        /// <param name="a">Угол - сторона в которую делать расчет.</param>
        /// <param name="offset">Расстояние.</param>
        /// <returns>Координаты.</returns>
        public Vector3 offsetPos(double x, double y, double z, double аngle, double offset)
        {
            var degree = -аngle * (Math.PI / 180);
            var x1 = x + (offset * Math.Sin(degree));
            var y1 = y + (offset * Math.Cos(degree));
            return new Vector3(x1, y1, z);
        }

        #region WhiteList
        /// <summary>
        /// Проверка имении на наличии в whitelist.
        /// </summary>
        /// <param name="name">Имя для проверки.</param>
        /// <returns></returns>
        public bool checkWhitelist(string name)
        {
            DataTable dt = sqlCon.retSQLData(string.Format(@"SELECT * FROM `whitelist` WHERE `name`= '{0}'", name));
            return Convert.ToBoolean(dt.Rows.Count);
        }

        /// <summary>
        /// Добавить имя в whitelist.
        /// </summary>
        /// <param name="name">Добавляемое имя.</param>
        public void addWhitelist(string name)
        {
            if (!checkWhitelist(name)) sqlCon.retSQLData(string.Format("INSERT INTO `whitelist` (`name`) VALUES('{0}')", name));
        }

        /// <summary>
        /// Удалить имя из whiteList.
        /// </summary>
        /// <param name="name">Удаляемое имя.</param>
        public void RemoveWhitelist(string name)
        {
            if (checkWhitelist(name)) sqlCon.retSQLData(string.Format("DELETE FROM `whitelist` WHERE `name`='{0}'", name));
        }
        #endregion

        #region BlackList
        /// <summary>
        /// Проверка имении на наличии в Blacklist.
        /// </summary>
        /// <param name="name">Имя для проверки.</param>
        /// <returns></returns>
        public bool checkBlacklist(string name)
        {
            DataTable dt = sqlCon.retSQLData(string.Format(@"SELECT * FROM `blacklist` WHERE `name`='{0}'", name));
            return Convert.ToBoolean(dt.Rows.Count);
        }

        /// <summary>
        /// Добавить имя в Blacklist.
        /// </summary>
        /// <param name="name">Добавляемое имя.</param>
        public void addBlacklist(string name)
        {
            if (!checkBlacklist(name)) sqlCon.retSQLData(string.Format("INSERT INTO `blacklist` (`name`) VALUES('{0}')", name));
        }

        /// <summary>
        /// Удалить имя из Blacklist.
        /// </summary>
        /// <param name="name">Удаляемое имя.</param>
        public void RemoveBlacklist(string name)
        {
            if (checkBlacklist(name)) sqlCon.retSQLData(string.Format("DELETE FROM `blacklist` WHERE `name`='{0}'", name));
        }

        /// <summary>
        /// Удалить все имена из blacklist.
        /// </summary>
        public void RemoveAllBlacklist()
        {
            sqlCon.retSQLData(string.Format("TRUNCATE TABLE `blacklist`"));
        }
        #endregion

        #region ID system
        /// <summary>
        /// Инициализация ID системы.
        /// </summary>
        /// <param name="max_players">Кол-во слотов на сервере.</param>
        public void InitializeID(int max_players)
        {
            for (var i = 0; i < max_players; i++)
            {
                players.Add(null);
            }
        }

        /// <summary>
        /// Получения индекса свободного места в массиве.
        /// </summary>
        /// <returns>индекс первого пустого элемента в массиве.</returns>
        private int getFreeId()
        {
            foreach (var item in players)
            {
                if (item == null)
                {
                    return players.IndexOf(item);
                }
            }
            return -1;
        }

        /// <summary>
        /// Добавление пользователя в массив и назначение ID.
        /// </summary>
        /// <param name="player">Пользователь.</param>
        /// <returns>ID игрока.</returns>
        public int setPlayerID(Client player)
        {
            int index = getFreeId();
            if (index != -1)
            {
                players[index] = player;
            }
            return index;
        }

        /// <summary>
        /// Удаление пользователя из массива.
        /// </summary>
        /// <param name="player">Пользователь.</param>
        public void removePlayerID(Client player)
        {
            int index = players.IndexOf(player);
            if (index != -1)
            {
                players[players.IndexOf(player)] = null;
            }
        }

        /// <summary>
        /// Поиск Игрока по ID или части ника.
        /// </summary>
        /// <param name="sender">Пользователь вызывающий метод.</param>
        /// <param name="idOrName">ID или часть ника.</param>
        /// <returns>Игрока если был найден, иначе Null.</returns>
        public Client findPlayer(Client sender, string idOrName, bool message = true)
        {
            int id;

            if (int.TryParse(idOrName, out id))
            {
                return getClientFromId(sender, id, message);
            }

            Client returnClient = null;
            int playersCount = 0;
            foreach (var player in players)
            {
                if (player == null) continue;

                if (player.name.ToLower().Contains(idOrName.ToLower()))
                {
                    if ((player.name.Equals(idOrName, StringComparison.OrdinalIgnoreCase))) return player;
                    else
                    {
                        playersCount++;
                        returnClient = player;
                    }
                }
            }

            if (playersCount != 1)
            {
                if (message)
                {
                    if (playersCount > 0) API.sendChatMessageToPlayer(sender, chatMessage("~y~[Server] ~w~Ошибка: найдено несколько игроков."));
                    else API.sendChatMessageToPlayer(sender, chatMessage(Constants.NotFindName));
                }
                return null;
            }

            return returnClient;
        }

        /// <summary>
        /// Поиск Игрока по ID.
        /// </summary>
        /// <param name="sender">Пользователь вызывающий метод.</param>
        /// <param name="id">ID искомого игрока.</param>
        /// <returns>Игрока если был найден, иначе Null.</returns>
        public Client getClientFromId(Client sender, int id, bool message = true)
        {
            if (players[id] == null)
            {
                if (message) API.sendChatMessageToPlayer(sender, chatMessage(Constants.NotFindID));
                return null;
            }
            return players[id];
        }

        /// <summary>
        /// Поиск id указанного игрока.
        /// </summary>
        /// <param name="target">Искомый игрок.</param>
        /// <returns>ID игрока, -1 если не был найден.</returns>
        public int getIdFromClient(Client target)
        {
            return players.IndexOf(target);
        }
        #endregion
        #endregion

        [Command("tp", GreedyArg = true)]
        public void TeleportPlayerToPlayerCommand(Client player, string coord)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    lifeRP_GM MainClass = lifeRP_GM.mainClass;
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        if (pInfo.admin != 0)
                        {
                            string x, y, z;
                            var pos = API.getEntityPosition(player.handle);
                            var coords = coord.Split(' ');
                            if (coords.Length < 3)
                            {
                                coords = coord.Split(',');
                                x = coords[0].Replace(".", ",");
                                y = coords[1].Replace(".", ",");
                                z = coords[2].Replace(".", ",");
                            }
                            else
                            {
                                x = coords[0].Replace(",", "").Replace(".", ",");
                                y = coords[1].Replace(",", "").Replace(".", ",");
                                z = coords[2].Replace(",", "").Replace(".", ",");
                            }
                            Vector3 posout = new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));

                            API.createParticleEffectOnPosition("scr_rcbarry1", "scr_alien_teleport", pos, new Vector3(), 1f);
                            API.setEntityPosition(player.handle, posout);
                            API.sendChatMessageToPlayer(player, "~y~[Server] ~w~Телепорт выполнен.");
                        }
                        else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.NonAdminAccessDenied));
                    }
                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в adminChat -  " + exp.Message);
                ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в adminChat");
                return;
            }
        }
    }

    /// <summary>
    /// Класс обрабатывающий логику работы с клиентами.
    /// </summary>
    public class ServerPlayers : Script
    {
        public static ServerPlayers playersClass; // Экземпляр класса игроков исполняемого сервером.
        public List<Client> Admins = new List<Client>(); // Массив админов.
        public List<Client> Developers = new List<Client>(); // Массив Разработчиков.

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        public ServerPlayers()
        {
            API.onResourceStart += onResourceStart;
            API.onPlayerBeginConnect += onPlayerBeginConnect;
            API.onPlayerFinishedDownload += onPlayerConnect;
            API.onPlayerDisconnected += onDisconnect;
            API.onClientEventTrigger += onClientEvent;
            API.onUpdate += onUpdate;
            API.onPlayerRespawn += onPlayerRespawn;
            API.onEntityDataChange += onEntityDataChange;
            API.onPlayerDeath += onPlayerDeath;
        }

        #region Методы событий
        private void onResourceStart()
        {
            playersClass = this;
        }

        private void onPlayerBeginConnect(Client player, CancelEventArgs cancelConnection)
        {
            try
            {
                if (lifeRP_GM.Debug) ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] ~w~onPlayerBeginConnect");
                lifeRP_GM owner = lifeRP_GM.mainClass;
                if (!owner.connectOpen) API.kickPlayer(player, "Отказанно в подключении");
                else if (owner.whitelistEnabled)
                {
                    if (!owner.checkWhitelist(player.socialClubName)) API.kickPlayer(player, "WhiteList only");
                }
                else if (owner.checkBlacklist(player.socialClubName)) API.kickPlayer(player, "Banned");
                API.setEntityDimension(player, 1000);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка при старте подключения пользователя: " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onPlayerBeginConnect");
                return;
            }
        }

        private void onPlayerConnect(Client player)
        {
            try
            {
                if (lifeRP_GM.Debug) ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] ~w~onPlayerConnect");
                lifeRP_GM owner = lifeRP_GM.mainClass;
                DataTable dt = owner.sqlCon.retSQLData(string.Format(@"SELECT * FROM `accounts` WHERE `name` = '{0}'", player.socialClubName));
                bool avalbname = Convert.ToBoolean(dt.Rows.Count);
                if (avalbname) API.triggerClientEvent(player, Constants.LogPage);
                else API.triggerClientEvent(player, Constants.RegPage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка во время подключения: " + exp.Message);
                API.kickPlayer(player, "Произошла ошибка во время подключения.");
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onPlayerConnect");
                return;
            }
        }

        private void onDisconnect(Client player, string reason)
        {
            try
            {
                if (lifeRP_GM.Debug) ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] ~w~onPlayerDisconnect " + reason);
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    //pInfo.DellPlayerBlip();
                    if (pInfo.islogin)
                    {
                        pInfo.UpdateBD();
                        pInfo.UpdateFaceBD();
                    }
                    if (pInfo.admin != 0)
                    {
                        if (pInfo.admin > 3)
                        {
                            Admins.Remove(player);
                            Developers.Remove(player);
                        }
                        else Admins.Remove(player);
                    }
                    pInfo.DeleteAllplayerEntity();
                }
                lifeRP_GM.mainClass.removePlayerID(player);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка во время отключения: " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onDisconnect");
                return;
            }
        }

        private void onPlayerRespawn(Client player)
        {
            try
            {
                if (lifeRP_GM.Debug) ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] ~w~onPlayerRespawn");
                if (!API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    API.setEntityDimension(player, 1000);
                    return;
                }
                PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                pInfo.PlayerRespawn();
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка во время спавна: " + exp.Message);
                API.kickPlayer(player, "Произошла ошибка во время спавна.");
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onPlayerRespawn");
                return;
            }
        }

        private void onUpdate()
        {
            try
            {
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в onUpdate: " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onUpdate");
                return;
            }
        }

        private void onClientEvent(Client player, string eventName, params object[] arguments)
        {
            lifeRP_GM owner = lifeRP_GM.mainClass;
            switch (eventName)
            {
                case Constants.Register: // authorization - обращение на сервер SQL для регистрации
                    try
                    {
                        string password = CryptoPassword(arguments[0].ToString());
                        string regdate = DateTime.Now.ToString();
                        string lastenterdate = DateTime.Now.ToString();
                        string lastIp = player.address;

                        string queryString = string.Format(@"INSERT INTO `accounts` (`name`, `password`, `regdate`, `lastenterdate`, `lastIp`) 
                                                VALUES('{0}', '{1}', '{2}', '{3}', '{4}')", player.socialClubName, password, regdate, lastenterdate, lastIp);
                        owner.sqlCon.retSQLData(queryString);
                        showCharacterlist(player);
                    }
                    catch (Exception exp)
                    {
                        API.consoleOutput("Во время создания аккаунта произошла ошибка: " + exp.Message);
                        API.kickPlayer(player, "Во время создания аккаунта произошла ошибка.");
                        sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onClientEvent");
                        return;
                    }
                    break;

                case Constants.Logining: // authorization - обращение на сервер SQL для авторизации
                    try
                    {
                        string password = CryptoPassword(arguments[0].ToString());

                        DataTable dt = owner.sqlCon.retSQLData(string.Format(@"SELECT * FROM `accounts` WHERE `name` = '{0}' and `password` = '{1}'", player.socialClubName, password));
                        int NumbRows = dt.Rows.Count;
                        bool avalbname = Convert.ToBoolean(NumbRows);
                        if (!avalbname)
                        {
                            API.triggerClientEvent(player, Constants.showError, "Неверный пароль.", "1");
                            return;
                        }

                        string lastenterdate = DateTime.Now.ToString();
                        string lastIp = player.address;
                        string queryString = string.Format(@"UPDATE `accounts` SET `lastenterdate`='{0}', `lastIp`='{1}' WHERE `name` = '{2}'", lastenterdate, lastIp, player.socialClubName);
                        owner.sqlCon.retSQLData(queryString);
                        showCharacterlist(player);
                    }
                    catch (Exception exp)
                    {
                        API.consoleOutput("Во время авторизации произошла ошибка: " + exp.Message);
                        API.kickPlayer(player, "Во время авторизации произошла ошибка.");
                        sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onClientEvent");
                        return;
                    }
                    break;

                case Constants.ShowCharList: // перезагрузить список персов пользователя
                    try
                    {                      
                        showCharacterlist(player);
                    }
                    catch (Exception exp)
                    {
                        API.consoleOutput("Во время авторизации произошла ошибка: " + exp.Message);
                        API.kickPlayer(player, "Во время авторизации произошла ошибка.");
                        sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onClientEvent");
                        return;
                    }
                    break;

                case Constants.ResetFaceData: //4-lifeRP_GTAO_Char - Сброс глобальных данных лица персонажа
                    try
                    {
                        if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                        {
                            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                            pInfo.InitializeFace(true);
                            pInfo.sex = (bool)arguments[0];
                            if (pInfo.sex) API.setPlayerSkin(player, PedHash.FreemodeMale01);
                            else API.setPlayerSkin(player, PedHash.FreemodeFemale01);
                            API.sendNativeToPlayer(player, 0x45EEE61580806D63, player.handle);
                            API.triggerClientEvent(player, Constants.ResetFaceDataComplete, player.handle);
                        }
                        else API.kickPlayer(player, Constants.KickCheatMessage);
                    }
                    catch (Exception exp)
                    {
                        API.consoleOutput("Во время сброса глобальных данных лица произошла ошибка: " + exp.Message);
                        API.kickPlayer(player, "Во время сброса глобальных данных лица произошла ошибка.");
                        sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onClientEvent");
                        return;
                    }
                    break;

                case Constants.CompleteFaceCreate: //4-lifeRP_GTAO_Char - Завершение создания персонажа
                    try
                    {
                        if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                        {
                            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                            pInfo.FaceMenuClosed();
                        }
                        else API.kickPlayer(player, Constants.KickCheatMessage);
                    }
                    catch (Exception exp)
                    {
                        API.consoleOutput("Во время завершения создания лица персонажа произошла ошибка: " + exp.Message);
                        API.kickPlayer(player, "Во время завершения создания лица персонажа произошла ошибка.");
                        sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onClientEvent");
                        return;
                    }
                    break;

                case Constants.CharStartPlay: //4-lifeRP_CharList - Начало игры после выбора персонажа
                    try
                    {
                        if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                        {
                            API.delay(100, true, () =>
                            {
                                int id = Convert.ToInt32(arguments[0]);
                                PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                                pInfo.LoadDatafromBD(pInfo.basedata.Rows[id]);
                                pInfo.basedata = null;
                                pInfo.SetDefaultData();
                                pInfo.PlayerFace();
                                API.delay(700, true, () =>
                                {
                                    API.sendNativeToPlayer(player, Hash.DO_SCREEN_FADE_IN, 750);
                                });
                            });
                        }
                        else API.kickPlayer(player, Constants.KickCheatMessage);
                    }
                    catch (Exception exp)
                    {
                        API.consoleOutput("Во время завершения выбора персонажа произошла ошибка: " + exp.Message);
                        API.kickPlayer(player, "Во время завершения выбора персонажа произошла ошибка.");
                        sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onClientEvent");
                        return;
                    }
                    break;

                case Constants.RemoveChar: //4-lifeRP_CharList - удаление персонажа
                    try
                    {
                        if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                        {
                            int id = Convert.ToInt32(arguments[0]);
                            PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                            string name = pInfo.basedata.Rows[id]["name"].ToString();
                            owner.sqlCon.retSQLData(string.Format(@"DELETE FROM `players` WHERE `name` = '{0}'", name));
                            owner.sqlCon.retSQLData(string.Format(@"DELETE FROM `face` WHERE `name` = '{0}'", name));
                            pInfo.basedata.Rows.RemoveAt(id);
                        }
                        else API.kickPlayer(player, Constants.KickCheatMessage);
                    }
                    catch (Exception exp)
                    {
                        API.consoleOutput("Во время удаления персонажа произошла ошибка: " + exp.Message);
                        API.kickPlayer(player, "Во время удаления персонажа произошла ошибка.");
                        sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onClientEvent");
                        return;
                    }
                    break;

                case Constants.CreateNewChar: //4-lifeRP_CharList - Создание нового персонажа
                    try
                    {
                        if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                        {
                            string name = arguments[0].ToString();
                            DataTable dt = owner.sqlCon.retSQLData(string.Format(@"SELECT * FROM `players` WHERE `name` = '{0}'", name));
                            bool avalbname = Convert.ToBoolean(dt.Rows.Count);
                            if (avalbname) API.triggerClientEvent(player, Constants.BusyNameChar);
                            else
                            {
                                PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                                pInfo.LoadDataintoBD(name);
                                pInfo.SetDefaultData();
                                pInfo.PlayerFace();
                            }
                        }
                        else API.kickPlayer(player, Constants.KickCheatMessage);
                    }
                    catch (Exception exp)
                    {
                        API.consoleOutput("Во время завершения создания персонажа произошла ошибка: " + exp.Message);
                        API.kickPlayer(player, "Во время завершения создания персонажа произошла ошибка.");
                        sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onClientEvent");
                        return;
                    }
                    break;

                case Constants.mainMenuReport: //4-lifeRP_MainMenu - Отправка репорта
                    try
                    {
                        string message = arguments[0].ToString();
                        ReportMessage(player, message);
                    }
                    catch (Exception exp)
                    {
                        API.consoleOutput("Во время завершения создания персонажа произошла ошибка: " + exp.Message);
                        API.kickPlayer(player, "Во время завершения создания персонажа произошла ошибка.");
                        sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onClientEvent");
                        return;
                    }
                    break;
            }
        }

        private void onEntityDataChange(NetHandle entity, string key, object oldValue)
        {
            try
            {
                string[] mKey = key.Split('_');
                if (mKey[0] == Constants.Player)
                {
                    if (API.hasEntityData(entity, Constants.PlayerAccount))
                    {
                        dynamic accValue = null;
                        PlayerInfo pInfo = API.getEntityData(entity, Constants.PlayerAccount);
                        switch (key)
                        {
                            case Constants.PlayerName:
                                accValue = pInfo.player.name;
                                if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.player.name);
                                break;

                            case Constants.PlayerTag:
                                accValue = pInfo.player.nametag;
                                if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.player.nametag);
                                break;

                            case Constants.PlayerBlip:
                                accValue = pInfo.playerblip.handle;
                                if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.playerblip.handle);
                                break;

                            case Constants.PlayerIsLogin:
                                accValue = pInfo.islogin;
                                if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.islogin);
                                break;

                            case Constants.PlayerFact:
                                accValue = pInfo.faction;
                                if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.faction);
                                break;

                            case Constants.PlayerAdmin:
                                accValue = pInfo.admin;
                                if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.admin);
                                break;

                            case Constants.PlayerMoney:
                                accValue = pInfo.money;
                                if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.money);
                                break;

                            case Constants.PlayerBankMoney:
                                accValue = pInfo.bankmoney;
                                if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.bankmoney);
                                break;

                            case Constants.PlayerFactRank:
                                accValue = pInfo.factrank;
                                if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.factrank);
                                break;

                            case Constants.PlayerJob:
                                accValue = pInfo.Job;
                                if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.Job);
                                break;

                            case Constants.PlayerLevel:
                                accValue = pInfo.lvl;
                                if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.lvl);
                                break;

                            case Constants.PlayerExp:
                                accValue = pInfo.exp;
                                if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.exp);
                                break;

                            case Constants.PlayerFaceHasCharacterData:
                                if (!pInfo.isInCharpCreate)
                                {
                                    accValue = pInfo.hasCharpData;
                                    if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.hasCharpData);
                                }
                                break;

                            case Constants.PlayerFaceShapeFirstID:
                                if (!pInfo.isInCharpCreate)
                                {
                                    accValue = pInfo.shapeFirst;
                                    if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.shapeFirst);
                                }
                                break;

                            case Constants.PlayerFaceShapeSecondID:
                                if (!pInfo.isInCharpCreate)
                                {
                                    accValue = pInfo.shapeSecond;
                                    if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.shapeSecond);
                                }
                                break;

                            case Constants.PlayerFaceSkinFirstID:
                                if (!pInfo.isInCharpCreate)
                                {
                                    accValue = pInfo.skinFirst;
                                    if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.skinFirst);
                                }
                                break;

                            case Constants.PlayerFaceSkinSecondID:
                                if (!pInfo.isInCharpCreate)
                                {
                                    accValue = pInfo.skinSecond;
                                    if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.skinSecond);
                                }
                                break;

                            case Constants.PlayerFaceShapeMix:
                                if (!pInfo.isInCharpCreate)
                                {
                                    accValue = pInfo.shapeMix;
                                    if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.shapeMix);
                                }
                                break;

                            case Constants.PlayerFaceSkinMix:
                                if (!pInfo.isInCharpCreate)
                                {
                                    accValue = pInfo.skinMix;
                                    if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.skinMix);
                                }
                                break;


                            case Constants.PlayerFaceHairColor:
                                if (!pInfo.isInCharpCreate)
                                {
                                    accValue = pInfo.hairColor;
                                    if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.hairColor);
                                }
                                break;

                            case Constants.PlayerFaceHairHighlightColor:
                                if (!pInfo.isInCharpCreate)
                                {
                                    accValue = pInfo.hairHighColor;
                                    if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.hairHighColor);
                                }
                                break;

                            case Constants.PlayerFaceEyeColor:
                                if (!pInfo.isInCharpCreate)
                                {
                                    accValue = pInfo.eyeColor;
                                    if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.eyeColor);
                                }
                                break;

                            case Constants.PlayerFaceEyebrows:
                                if (!pInfo.isInCharpCreate)
                                {
                                    accValue = pInfo.eyebrows;
                                    if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.eyebrows);
                                }
                                break;

                            case Constants.PlayerFaceEyebrowsColor:
                                if (!pInfo.isInCharpCreate)
                                {
                                    accValue = pInfo.eyebrowsColor;
                                    if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.eyebrowsColor);
                                }
                                break;

                            case Constants.PlayerFaceEyebrowsColor2:
                                if (!pInfo.isInCharpCreate)
                                {
                                    accValue = pInfo.eyebrowsColor2;
                                    if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.eyebrowsColor2);
                                }
                                break;

                            case Constants.PlayerFaceMakeup:
                                if (!pInfo.isInCharpCreate)
                                {
                                    accValue = pInfo.makeup;
                                    if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.makeup);
                                }
                                break;

                            case Constants.PlayerFaceMakeupColor:
                                if (!pInfo.isInCharpCreate)
                                {
                                    accValue = pInfo.makeupColor;
                                    if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.makeupColor);
                                }
                                break;

                            case Constants.PlayerFaceMakeupColor2:
                                if (!pInfo.isInCharpCreate)
                                {
                                    accValue = pInfo.makeupColor2;
                                    if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.makeupColor2);
                                }
                                break;

                            case Constants.PlayerFaceLipstick:
                                if (!pInfo.isInCharpCreate)
                                {
                                    accValue = pInfo.lipstick;
                                    if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.lipstick);
                                }
                                break;

                            case Constants.PlayerFaceLipstickColor:
                                if (!pInfo.isInCharpCreate)
                                {
                                    accValue = pInfo.lipstickColor;
                                    if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.lipstickColor);
                                }
                                break;

                            case Constants.PlayerFaceLipstickColor2:
                                if (!pInfo.isInCharpCreate)
                                {
                                    accValue = pInfo.lipstickColor2;
                                    if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.lipstickColor2);
                                }
                                break;

                                /*case Constants.PlayerFaceFeaturesList:
                                     if (!pInfo.isInCharpCreate)
                                     {
                                        if (!pInfo.faceFeatures.Any()) break;
                                        List<object> accValue = new List<object>();
                                        accValue.AddRange(pInfo.faceFeatures.Cast<object>());

                                        //List<object> faceFeatureList = API.getEntitySyncedData(entity, key);
                                        //float[] faceFeatures = Array.ConvertAll(faceFeatureList.ToArray(), Convert.ToSingle);

                                        if (!accValue.Equals(API.getEntitySyncedData(entity, key))) API.setEntitySyncedData(entity, key, pInfo.faceFeatures);
                                     }
                                     break;

                                 case Constants.PlayerFaceClosesList:
                                     if (!pInfo.isInCharpCreate)
                                     {
                                        if (pInfo.closes.Any()) break;
                                        List<object> accValue = new List<object>();
                                        accValue.AddRange(pInfo.closes.Cast<object>());
                                        if (accValue != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, pInfo.closes);
                                     }
                                     break;*/
                        }
                    }
                    else API.kickPlayer(API.getPlayerFromHandle(entity), Constants.KickCheatMessage);
                }
            }
            catch (Exception e)
            {
                API.consoleOutput("Произошла ошибка проверке данных. Ключ - " + key + " " + e.ToString());
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onEntityDataChange");
                return;
            }
        }

        private void onPlayerDeath(Client player, NetHandle entityKiller, int weapon)
        {
            try
            {
                if (lifeRP_GM.Debug) ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] ~w~onPlayerDeath");
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    pInfo.isDead = true;
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка во смерти игрока: " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onPlayerDeath");
                return;
            }
        }
        #endregion

        #region Комманды
        #region Админ комманды
        [Command("a", "Использование: /a(/а) [Text]", Alias = "а", GreedyArg = true)] // adminchat
        public void adminChat(Client player, string text)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    lifeRP_GM MainClass = lifeRP_GM.mainClass;
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        if (pInfo.admin != 0)
                        {
                            string str = MainClass.chatMessage(string.Format("~y~[AdmChat] {0}{1} {2}: ~w~{3}", pInfo.admin >= 4 ? "~r~" : pInfo.admin >= 2 ? "~o~" : "", Enum.GetName(typeof(AdmRank), pInfo.admin), player.nametag, text));
                            sendChatMessageToAdmins(str);
                        }
                        else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.NonAdminAccessDenied));
                    }
                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в adminChat -  " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в adminChat");
                return;
            }
        }

        [Command("report", "Использование: /report(/рапорт) [Text]", Alias = "рапорт", GreedyArg = true)] // репорт игроков к админам
        public void ReportMessage(Client player, string text)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    lifeRP_GM MainClass = lifeRP_GM.mainClass;
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        string str = MainClass.chatMessage(string.Format("~r~[Report] {0}: ~w~{1}", player.nametag, text));
                        sendChatMessageToAdmins(str);
                        API.sendChatMessageToPlayer(player, MainClass.chatMessage("~y~[Server] ~w~Репорт отправлен администрацие."));
                    }
                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в ReportMessage -  " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в ReportMessage");
                return;
            }
        }

        [Command("pc", "Использование: /pc(/лс) [idOrName] [Text]", Alias = "лс", GreedyArg = true)] // личный чат админов
        public void PrivateСhat(Client player, string text)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    lifeRP_GM MainClass = lifeRP_GM.mainClass;
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        if (pInfo.admin != 0)
                        {
                            string[] strSplit = text.Split(' ');
                            if (strSplit.Count() >= 2)
                            {
                                Client target = MainClass.findPlayer(player, strSplit[0]);
                                if (target != null && API.hasEntityData(target.handle, Constants.PlayerAccount))
                                {
                                    if (target != player)
                                    {
                                        PlayerInfo tInfo = API.getEntityData(target.handle, Constants.PlayerAccount);
                                        if (tInfo.islogin)
                                        {
                                            text = text.Remove(0, strSplit[0].Length + 1);
                                            string str = MainClass.chatMessage(string.Format("~y~[PrChat] {0}{1} {2}: ~w~{3}", pInfo.admin >= 4 ? "~r~" : pInfo.admin >= 2 ? "~o~" : "", Enum.GetName(typeof(AdmRank), pInfo.admin), player.nametag, text));
                                            API.sendChatMessageToPlayer(target, str);
                                            API.sendChatMessageToPlayer(player, MainClass.chatMessage("~y~[Server] ~w~Сообщение отправлено."));
                                        }
                                        else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.targetNotLogin));
                                    }
                                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.yourselfMessage));
                                }
                            }
                            else API.sendChatMessageToPlayer(player, MainClass.chatMessage("Использование: /pc(/лс) [idOrName] [Text]"));
                        }
                        else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.NonAdminAccessDenied));
                    }
                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в PrivateСhat -  " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в PrivateСhat");
                return;
            }
        }

        [Command("all", "Использование: /all(/все) [Text]", Alias = "все", GreedyArg = true)] //Allchat
        public void allChat(Client player, string text)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    lifeRP_GM MainClass = lifeRP_GM.mainClass;
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        if (pInfo.admin != 0)
                        {
                            string str = MainClass.chatMessage(string.Format("~y~[AllChat] {0}{1} {2}: ~w~{3}", pInfo.admin >= 4 ? "~r~" : pInfo.admin >= 2 ? "~o~" : "", Enum.GetName(typeof(AdmRank), pInfo.admin), player.name, text));
                            API.sendChatMessageToAll(str);
                        }
                        else if (MainClass.allchat)
                        {
                            string str = MainClass.chatMessage(string.Format("~y~[AllChat] {0}: ~w~{1}", player.nametag, text));
                            API.sendChatMessageToAll(str);
                        }
                        else API.sendNotificationToPlayer(player, MainClass.chatMessage("~y~[Server] ~w~Общий чат отключен."));
                    }
                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в allChat -  " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в allChat");
                return;
            }
        }

        [Command("setadmin", "Использование: /setadmin(/назчадм) [idOrName] [AdmLvL]", Alias = "назчадм")] // смена ранга админа
        public void setAdminCommand(Client player, string idOrName, int AdmLvl)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    lifeRP_GM MainClass = lifeRP_GM.mainClass;
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        if (pInfo.admin != 0)
                        {
                            if (-1 < AdmLvl && AdmLvl < 4)
                            {
                                Client target = GetClientByName(player, idOrName);
                                if (target != null && API.hasEntityData(target.handle, Constants.PlayerAccount))
                                {
                                    PlayerInfo tInfo = API.getEntityData(target.handle, Constants.PlayerAccount);
                                    if (pInfo.player.name == tInfo.player.name)
                                    {
                                        API.sendChatMessageToPlayer(player, MainClass.chatMessage("~y~[Server] ~w~Ошибка: Нельзя изменить ранг самому себе."));
                                        return;
                                    }

                                    if (pInfo.admin <= tInfo.admin)
                                    {
                                        API.sendChatMessageToPlayer(player, MainClass.chatMessage("~y~[Server] ~w~Ошибка: Игрок старше или того же ранга."));
                                        return;
                                    }

                                    string str = MainClass.chatMessage(string.Format("~y~{0}{1} {2} ~w~{3} игрока {4}{5}.",
                                            pInfo.admin >= 4 ? "~r~" : pInfo.admin >= 2 ? "~o~" : "",
                                            Enum.GetName(typeof(AdmRank), pInfo.admin),
                                            player.name,
                                            AdmLvl != 0 ? "назначил" : "уволил",
                                            target.name,
                                            AdmLvl != 0 ? " на должность " + Enum.GetName(typeof(AdmRank), AdmLvl) : " из администрации"));

                                    tInfo.admin = AdmLvl;
                                    tInfo.SetSyncedData(Constants.PlayerAdmin, AdmLvl);
                                    API.sendChatMessageToPlayer(player, str);
                                    API.sendChatMessageToPlayer(target, str);
                                    MainClass.saveToFile(Constants.LogAdmFile, str + " " + DateTime.Now.ToString());
                                }
                                else API.sendChatMessageToPlayer(player, MainClass.chatMessage("~y~[Server] ~w~Ошибка: Игрок не найден."));
                            }
                            else API.sendChatMessageToPlayer(player, MainClass.chatMessage("~y~[Server] ~w~Ошибка: AdmLvL может быть: 0 - Игрок, 1 - Помощник, 2 - Админ, 3 - стАдмин."));
                        }
                        else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.NonAdminAccessDenied));
                    }
                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в setAdminCommand -  " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в setAdminCommand");
                return;
            }
        }

        [Command("kick", "Использование: /kick(/кик) [idOrName] [Reason]", Alias = "кик", GreedyArg = true)] // kick 
        public void kickCommand(Client player, string text)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    lifeRP_GM MainClass = lifeRP_GM.mainClass;
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        if (pInfo.admin != 0)
                        {
                            string[] strSplit = text.Split(' ');
                            if (strSplit.Count() >= 1)
                            {
                                int count = strSplit.Count();
                                string idOrName = strSplit[0];
                                string reason = count == 1 ? "sKick" : text.Remove(0, strSplit[0].Length + 1);

                                Client target = GetClientByName(player, idOrName);
                                if (target != null)
                                {
                                    if (API.hasEntityData(target.handle, Constants.PlayerAccount))
                                    {
                                        PlayerInfo tInfo = API.getEntityData(target.handle, Constants.PlayerAccount);
                                        if (tInfo.admin > 0)
                                        {
                                            API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.AdmProtect));
                                            return;
                                        }
                                    }
                                    string str = string.Format("~y~{0}{1} {2} ~w~кикнул игрока {3}. Причина: {4}", pInfo.admin >= 4 ? "~r~" : pInfo.admin >= 2 ? "~o~" : "", Enum.GetName(typeof(AdmRank), pInfo.admin), player.name, target.name, reason);
                                    if (count != 1) API.sendChatMessageToAll(MainClass.chatMessage(str));
                                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage("~y~[Server] ~w~Игрок был кикнут."));
                                    API.kickPlayer(target, reason);
                                    MainClass.saveToFile(Constants.LogAdmFile, str + " " + DateTime.Now.ToString());
                                }
                                else API.sendChatMessageToPlayer(player, MainClass.chatMessage("~y~[Server] ~w~Ошибка: Игрок не найден."));
                            }
                            else API.sendChatMessageToPlayer(player, MainClass.chatMessage("Использование: /kick(/кик) [idOrName] [Reason]"));
                        }
                        else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.NonAdminAccessDenied));
                    }
                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в kick -  " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в kick");
                return;
            }
        }

        [Command("ban", "Использование: /ban(/бан) [idOrName] [banType] [Reason]", Alias = "бан", GreedyArg = true)] // ban
        public void banCommand(Client player, string text)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    lifeRP_GM MainClass = lifeRP_GM.mainClass;
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        if (pInfo.admin > 1)
                        {
                            string[] strSplit = text.Split(' ');
                            if (strSplit.Count() >= 3)
                            {
                                string idOrName = strSplit[0];
                                string strBanType = strSplit[1];
                                int banType = 0;
                                string reason = text.Remove(0, strSplit[0].Length + strSplit[1].Length + 2);
                                DateTime datetime = DateTime.Now;

                                if (int.TryParse(strBanType, out banType))
                                {
                                    if (0 > banType || banType > 3)
                                    {
                                        API.sendChatMessageToPlayer(player, MainClass.chatMessage("~y~[Server] ~w~Ошибка: BanType может быть: 0 - разбан, 1 - на сутки, 2 - на месяц, 3 - навсегда."));
                                        return;
                                    }
                                }
                                else
                                {
                                    API.sendChatMessageToPlayer(player, MainClass.chatMessage("~y~[Server] ~w~Ошибка: BanType может быть: 0 - разбан, 1 - на сутки, 2 - на месяц, 3 - навсегда."));
                                    return;
                                }

                                Client target = GetClientByName(player, idOrName);
                                if (target != null)
                                {
                                    if (API.hasEntityData(target.handle, Constants.PlayerAccount))
                                    {
                                        PlayerInfo tInfo = API.getEntityData(target.handle, Constants.PlayerAccount);
                                        if (tInfo.admin > 0)
                                        {
                                            API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.AdmProtect));
                                            return;
                                        }

                                        if (CheckAdmOnAcc(target.socialClubName))
                                        {
                                            API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.AdmProtect));
                                            return;
                                        }

                                        string queryString = string.Format(@"UPDATE `players` SET `ban`='{0}', `banreason`='{1}', `bandate`='{2}' WHERE `name` = '{3}' AND `admin` = '0'", banType, reason, datetime.ToString(), target.name);
                                        MainClass.sqlCon.retSQLData(queryString);
                                        logoutCommand(target);
                                        if (banType != 0) banCountUp(player, target.socialClubName);
                                    }
                                    else
                                    {
                                        DataTable dt = MainClass.sqlCon.retSQLData(string.Format(@"SELECT * FROM `players` WHERE `name` = '{0}'", target.name));
                                        bool avalbname = Convert.ToBoolean(dt.Rows.Count);
                                        if (avalbname)
                                        {
                                            if (!CheckAdmOnAcc(target.socialClubName))
                                            {
                                                string queryString = string.Format(@"UPDATE `players` SET `ban`='{0}', `banreason`='{1}', `bandate`='{2}' WHERE `name` = '{3}' AND `admin` = '0'", banType, reason, datetime.ToString(), target.name);
                                                MainClass.sqlCon.retSQLData(queryString);
                                                API.kickPlayer(target, Constants.KickCheatMessage);
                                                if (banType != 0) banCountUp(player, target.socialClubName);
                                            }
                                            else
                                            {
                                                API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.AdmProtect));
                                                return;
                                            }
                                        }
                                        else API.kickPlayer(target, Constants.KickCheatMessage);
                                    }

                                    string str = MainClass.chatMessage(string.Format("~y~{0}{1} {2} ~w~{3} игрока {4}{5}.{6}",
                                            pInfo.admin >= 4 ? "~r~" : pInfo.admin >= 2 ? "~o~" : "",
                                            Enum.GetName(typeof(AdmRank), pInfo.admin), player.name,
                                            banType != 0 ? "забанил" : "разбанил", target.name,
                                            banType != 0 ? banType == 3 ? " навсегда" : " на " + Enum.GetName(typeof(BanTypes), banType) : "",
                                            banType != 0 ? " Причина: " + reason : ""));

                                    API.sendChatMessageToAll(str);
                                    MainClass.saveToFile(Constants.LogAdmFile, str + " " + DateTime.Now.ToString());
                                }
                                else
                                {
                                    int id;
                                    if (!int.TryParse(idOrName, out id))
                                    {
                                        DataTable dt = MainClass.sqlCon.retSQLData(string.Format(@"SELECT * FROM `players` WHERE `name` = '{0}'", idOrName));
                                        bool avalbname = Convert.ToBoolean(dt.Rows.Count);
                                        if (avalbname)
                                        {
                                            string login = dt.Rows[0]["socialClubName"].ToString();
                                            if (!CheckAdmOnAcc(login))
                                            {
                                                string queryString = string.Format(@"UPDATE `players` SET `ban`='{0}', `banreason`='{1}', `bandate`='{2}' WHERE `name` = '{3}'", banType, reason, datetime.ToString(), idOrName);
                                                MainClass.sqlCon.retSQLData(queryString);
                                                if (banType != 0) banCountUp(player, login);

                                                string str = MainClass.chatMessage(string.Format("~y~[Server] ~w~Вы успешно {0} игрока {1}{2}.{3}",
                                                    banType != 0 ? "забанили" : "разбанили", idOrName,
                                                    banType != 0 ? banType == 3 ? " навсегда" : " на " + Enum.GetName(typeof(BanTypes), banType) : "",
                                                    banType != 0 ? " Причина: " + reason : ""));

                                                API.sendChatMessageToPlayer(player, str);
                                                MainClass.saveToFile(Constants.LogAdmFile, str + " " + DateTime.Now.ToString());
                                            }
                                            else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.AdmProtect));
                                        }
                                        else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.NotFindName));
                                    }
                                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.NotFindID));
                                }
                            }
                            else API.sendChatMessageToPlayer(player, MainClass.chatMessage("Использование: /ban(/бан) [idOrName] [banType] [Reason]"));
                        }
                        else API.sendChatMessageToPlayer(player, pInfo.admin != 0 ? MainClass.chatMessage("~y~[Server] ~w~Ошибка: Доступно только для Админов и старше.") : MainClass.chatMessage(Constants.NonAdminAccessDenied));
                    }
                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в ban -  " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в ban");
                return;
            }
        }

        [Command("addbl", "Использование: /addbl(/допчл) [idOrName]", Alias = "допчл")] //Добавить имя в blacklist
        public void addBlacklistplayerCommand(Client player, string text)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    lifeRP_GM MainClass = lifeRP_GM.mainClass;
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        if (pInfo.admin > 2)
                        {
                            Client target = GetClientByName(player, text);
                            string login = target != null ? target.socialClubName : GetAccountByName(text);
                            if (login != null)
                            {
                                if (!CheckAdmOnAcc(login))
                                {
                                    string str = string.Format("{0} {1} добавил в черный лист имя {2}.", Enum.GetName(typeof(AdmRank), pInfo.admin), player.name, text);
                                    if (target != null) API.kickPlayer(target, "Banned");
                                    RemoveAccount(player, login);
                                    MainClass.saveToFile(Constants.LogAdmFile, str + " " + DateTime.Now.ToString());
                                }
                                else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.AdmProtect));
                            }
                            else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.NotFindName));
                        }
                        else API.sendChatMessageToPlayer(player, pInfo.admin != 0 ? MainClass.chatMessage("~y~[Server] ~w~Ошибка: Доступно только стАдмину и выше.") : MainClass.chatMessage(Constants.NonAdminAccessDenied));
                    }
                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в addBlacklistplayerCommand -  " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в addBlacklistplayerCommand");
                return;
            }
        }

        [Command("rembl", "Использование: /rembl(/удлчл) [Name]", Alias = "удлчл")] //Удалить имя из blacklist
        public void removeBlacklistCommand(Client player, string text)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    lifeRP_GM MainClass = lifeRP_GM.mainClass;
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        if (pInfo.admin > 2)
                        {
                            MainClass.RemoveBlacklist(text);
                            API.sendChatMessageToPlayer(player, MainClass.chatMessage("~y~[Server] ~w~Имя удалено из Blacklist."));
                            string str = string.Format("{0} {1} удалил из черного листа имя {2}.", Enum.GetName(typeof(AdmRank), pInfo.admin), player.name, text);
                            MainClass.saveToFile(Constants.LogAdmFile, str + " " + DateTime.Now.ToString());
                        }
                        else API.sendChatMessageToPlayer(player, pInfo.admin != 0 ? MainClass.chatMessage("~y~[Server] ~w~Ошибка: Доступно только стАдмин и выше.") : MainClass.chatMessage(Constants.NonAdminAccessDenied));
                    }
                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в removeBlacklistCommand -  " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в removeBlacklistCommand");
                return;
            }
        }

        [Command("remalbl", "Использование: /remalbl(/удлвчл)", Alias = "удлвчл")] //Удалить все имена из blacklist
        public void removeAllBlacklistCommand(Client player)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    lifeRP_GM MainClass = lifeRP_GM.mainClass;
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        if (pInfo.admin > 2)
                        {
                            MainClass.RemoveAllBlacklist();
                            API.sendChatMessageToPlayer(player, MainClass.chatMessage("~y~[Server] ~w~Blacklist очищен."));
                            string str = string.Format("{0} {1} удалил все имена из черного листа.", Enum.GetName(typeof(AdmRank), pInfo.admin), player.name);
                            MainClass.saveToFile(Constants.LogAdmFile, str + " " + DateTime.Now.ToString());
                        }
                        else API.sendChatMessageToPlayer(player, pInfo.admin != 0 ? MainClass.chatMessage("~y~[Server] ~w~Ошибка: Доступно только стАдмин и выше.") : MainClass.chatMessage(Constants.NonAdminAccessDenied));
                    }
                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в removeAllBlacklistCommand -  " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в removeAllBlacklistCommand");
                return;
            }
        }

        [Command("enablewl", "Использование: /enablewl(/вклбл)", Alias = "вклбл")] //включить\выключить whitelist
        public void enubledwhitelistCommand(Client player)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    lifeRP_GM MainClass = lifeRP_GM.mainClass;
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        if (pInfo.admin > 2)
                        {
                            MainClass.whitelistEnabled = !MainClass.whitelistEnabled;
                            API.sendChatMessageToPlayer(player, MainClass.chatMessage(string.Format("~y~[Server] ~w~Whitelist {0}.", MainClass.whitelistEnabled ? "включен" : "выключен")));
                            string str = string.Format("{0} {1} {2} белый лист.", Enum.GetName(typeof(AdmRank), pInfo.admin), player.name, MainClass.whitelistEnabled ? "включил" : "выключил");
                            MainClass.saveToFile(Constants.LogAdmFile, str + " " + DateTime.Now.ToString());
                        }
                        else API.sendChatMessageToPlayer(player, pInfo.admin != 0 ? MainClass.chatMessage("~y~[Server] ~w~Ошибка: Доступно только для стАдминов и выше.") : MainClass.chatMessage(Constants.NonAdminAccessDenied));
                    }
                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в enubledwhitelistCommand -  " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в enubledwhitelistCommand");
                return;
            }
        }

        [Command("addwl", "Использование: /addwl(/допбл) [Name]", Alias = "допбл")] //Добавить имя в whitelist
        public void addwhitelistCommand(Client player, string text)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    lifeRP_GM MainClass = lifeRP_GM.mainClass;
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        if (pInfo.admin > 2)
                        {
                            MainClass.addWhitelist(text);
                            API.sendChatMessageToPlayer(player, MainClass.chatMessage("~y~[Server] ~w~Имя добавлено в WhiteList."));
                            string str = string.Format("{0} {1} добавил имя {2} в белый лист.", Enum.GetName(typeof(AdmRank), pInfo.admin), player.name, text);
                            MainClass.saveToFile(Constants.LogAdmFile, str + " " + DateTime.Now.ToString());
                        }
                        else API.sendChatMessageToPlayer(player, pInfo.admin != 0 ? MainClass.chatMessage("~y~[Server] ~w~Ошибка: Доступно только стАдмин и выше.") : MainClass.chatMessage(Constants.NonAdminAccessDenied));
                    }
                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в addwhitelistCommand -  " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в addwhitelistCommand");
                return;
            }
        }

        [Command("addwlp", "Использование: /addwlp(/допбли) [idOrName]", Alias = "допбли")] //Добавить имя в whitelist указав игрока
        public void addwhitelistplayerCommand(Client player, string text)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    lifeRP_GM MainClass = lifeRP_GM.mainClass;
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        if (pInfo.admin > 2)
                        {
                            Client target = MainClass.findPlayer(player, text);
                            if (target != null && API.hasEntityData(target.handle, Constants.PlayerAccount))
                            {
                                PlayerInfo tInfo = API.getEntityData(target.handle, Constants.PlayerAccount);
                                if (tInfo.islogin)
                                {
                                    MainClass.addWhitelist(target.socialClubName);
                                    API.sendChatMessageToPlayer(player, MainClass.chatMessage("~y~[Server] ~w~Имя добавлено в WhiteList."));
                                    string str = string.Format("{0} {1} добавил имя {2} в белый лист.", Enum.GetName(typeof(AdmRank), pInfo.admin), player.name, target.socialClubName);
                                    MainClass.saveToFile(Constants.LogAdmFile, str + " " + DateTime.Now.ToString());
                                }
                                else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.targetNotLogin));
                            }
                        }
                        else API.sendChatMessageToPlayer(player, pInfo.admin != 0 ? MainClass.chatMessage("~y~[Server] ~w~Ошибка: Доступно только стАдмин и выше.") : MainClass.chatMessage(Constants.NonAdminAccessDenied));
                    }
                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в addwhitelistplayerCommand -  " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в addwhitelistplayerCommand");
                return;
            }
        }

        [Command("remwl", "Использование: /remwl(/удлбл) [Name]", Alias = "удлбл")] //Удалить имя из whitelist
        public void removeWhitelistCommand(Client player, string text)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    lifeRP_GM MainClass = lifeRP_GM.mainClass;
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        if (pInfo.admin > 2)
                        {
                            MainClass.RemoveWhitelist(text);
                            API.sendChatMessageToPlayer(player, MainClass.chatMessage("~y~[Server] ~w~Имя удалено из WhiteList."));
                            string str = string.Format("{0} {1} удалил имя {2} из белого листа.", Enum.GetName(typeof(AdmRank), pInfo.admin), player.name, text);
                            MainClass.saveToFile(Constants.LogAdmFile, str + " " + DateTime.Now.ToString());
                        }
                        else API.sendChatMessageToPlayer(player, pInfo.admin != 0 ? MainClass.chatMessage("~y~[Server] ~w~Ошибка: Доступно только стАдмин и выше.") : MainClass.chatMessage(Constants.NonAdminAccessDenied));
                    }
                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в removeWhitelistCommand -  " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в removeWhitelistCommand");
                return;
            }
        }

        [Command("remwlp", "Использование: /remwlp(/удлбли) [idOrName]", Alias = "удлбли")] //Удалить имя из whitelist указав игрока
        public void removeWhitelisplayerCommand(Client player, string text)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    lifeRP_GM MainClass = lifeRP_GM.mainClass;
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        if (pInfo.admin > 2)
                        {
                            Client target = MainClass.findPlayer(player, text);
                            if (target != null && API.hasEntityData(target.handle, Constants.PlayerAccount))
                            {
                                PlayerInfo tInfo = API.getEntityData(target.handle, Constants.PlayerAccount);
                                if (tInfo.islogin)
                                {
                                    MainClass.RemoveWhitelist(target.socialClubName);
                                    API.sendChatMessageToPlayer(player, MainClass.chatMessage("~y~[Server] ~w~Имя удалено из WhiteList."));
                                    string str = string.Format("{0} {1} удалил имя {2} из белого листа.", Enum.GetName(typeof(AdmRank), pInfo.admin), player.name, target.socialClubName);
                                    MainClass.saveToFile(Constants.LogAdmFile, str + " " + DateTime.Now.ToString());
                                }
                                else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.targetNotLogin));
                            }
                        }
                        else API.sendChatMessageToPlayer(player, pInfo.admin != 0 ? MainClass.chatMessage("~y~[Server] ~w~Ошибка: Доступно только стАдмин и выше.") : MainClass.chatMessage(Constants.NonAdminAccessDenied));
                    }
                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в removeWhitelisplayerCommand -  " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в removeWhitelisplayerCommand");
                return;
            }
        }

        [Command("weapon", "Использование: /weapon [Hash]")]
        public void WeaponCommand(Client player, WeaponHash hash)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    lifeRP_GM MainClass = lifeRP_GM.mainClass;
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        if (pInfo.admin > 2)
                        {
                            API.givePlayerWeapon(player, hash, 500, true, true);
                        }
                        else API.sendChatMessageToPlayer(player, pInfo.admin != 0 ? MainClass.chatMessage("~y~[Server] ~w~Ошибка: Доступно только стАдмин и выше.") : MainClass.chatMessage(Constants.NonAdminAccessDenied));
                    }
                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в WeaponCommand -  " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в WeaponCommand");
                return;
            }
        }
        #endregion

        [Command("logout", "Использование: /logout(/разлог)", Alias = "разлог")] // Смена персонажа
        public void logoutCommand(Client player)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        onDisconnect(player, "logout");
                        showCharacterlist(player);
                    }
                    else API.sendChatMessageToPlayer(player, lifeRP_GM.mainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в logoutCommand: " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в logoutCommand");
                return;
            }
        }

        [Command("id", "Использование: /id(/ид) [idOrName]", Alias = "ид")] // Показать имя по ид или наоборот
        public void idCommand(Client player, string text)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        Client target = lifeRP_GM.mainClass.findPlayer(player, text);
                        if (target != null && API.hasEntityData(target.handle, Constants.PlayerAccount))
                        {
                            PlayerInfo tInfo = API.getEntityData(target.handle, Constants.PlayerAccount);
                            API.sendChatMessageToPlayer(player, lifeRP_GM.mainClass.chatMessage(string.Format("~y~[Server] ~w~ID: {0} - {1}", tInfo.ID, target.name)));
                        }
                    }
                    else API.sendChatMessageToPlayer(player, lifeRP_GM.mainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в idCommand: " + exp.Message);
                sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в idCommand");
                return;
            }
        }
        #endregion

        #region Методы
        /// <summary>
        /// Отправить сообщение Админам.
        /// </summary>
        /// <param name="text">Сообщение.</param>
        public void sendChatMessageToAdmins(string text)
        {
            foreach (Client target in Admins)
            {
                if (target != null && API.hasEntityData(target.handle, Constants.PlayerAccount))
                {
                    PlayerInfo tInfo = API.getEntityData(target.handle, Constants.PlayerAccount);
                    if (tInfo.islogin) API.sendChatMessageToPlayer(target, text);
                }
            }
        }

        /// <summary>
        /// Отправить сообщение разработчикам.
        /// </summary>
        /// <param name="text">Сообщение.</param>
        public void sendChatMessageToDevelopers(string text)
        {
            foreach (Client target in Developers)
            {
                if (target != null && API.hasEntityData(target.handle, Constants.PlayerAccount))
                {
                    PlayerInfo tInfo = API.getEntityData(target.handle, Constants.PlayerAccount);
                    if (tInfo.islogin) API.sendChatMessageToPlayer(target, text);
                }
            }
        }

        /// <summary>
        /// Увеличивает значение кол-ва баннов на аккаунте.
        /// </summary>
        /// <param name="account">Логин.</param>
        public void banCountUp(Client player, string login)
        {
            string queryString = string.Format(@"SELECT * FROM `accounts` WHERE `name` = '{0}'", login);
            DataTable dt = lifeRP_GM.mainClass.sqlCon.retSQLData(queryString);
            if (dt.Rows.Count > 0)
            {
                int bancount = Convert.ToInt32(dt.Rows[0]["bancount"]);
                if (bancount > 1)
                {
                    Client target = GetClientBySocName(login);
                    if (target != null) API.kickPlayer(target, "Banned");
                    RemoveAccount(player, login);
                    return;
                }
                lifeRP_GM.mainClass.sqlCon.retSQLData(string.Format(@"UPDATE `accounts` SET `bancount`='{0}' WHERE `name` = '{1}'", ++bancount, login));
            }
        }

        /// <summary>
        /// Удаление и блокирование аккаунта.
        /// </summary>
        /// <param name="player">Администратор.</param>
        /// <param name="login">Логин Аккаунта.</param>
        public void RemoveAccount(Client player, string login)
        {
            lifeRP_GM MainClass = lifeRP_GM.mainClass;
            MainClass.addBlacklist(login);
            RemoveAllCharOnAcc(login);
            MainClass.sqlCon.retSQLData(string.Format(@"DELETE FROM `accounts` WHERE `name` = '{0}'", login));
            API.sendChatMessageToPlayer(player, MainClass.chatMessage("~y~[Server] ~w~Аккаунт пользователя удален и заблокирован."));
        }

        /// <summary>
        /// Удалить всех персонажей связанных с аккаунтом.
        /// </summary>
        /// <param name="login">Логин аккаунта.</param>
        public void RemoveAllCharOnAcc(string login)
        {
            lifeRP_GM MainClass = lifeRP_GM.mainClass;
            List<string> charsNames = new List<string>();
            DataTable dt = MainClass.sqlCon.retSQLData(string.Format(@"SELECT * FROM `players` WHERE `socialClubName` = '{0}'", login));
            foreach (DataRow row in dt.Rows)
            {
                string name = row["name"].ToString();
                MainClass.sqlCon.retSQLData(string.Format(@"DELETE FROM `players` WHERE `name` = '{0}'", name));
                MainClass.sqlCon.retSQLData(string.Format(@"DELETE FROM `face` WHERE `name` = '{0}'", name));
            }
        }

        /// <summary>
        /// Проверяет наличие персонажей - админов на аккаунте.
        /// </summary>
        /// <param name="login">Логин аккаунта.</param>
        public bool CheckAdmOnAcc(string login)
        {
            string queryString = string.Format(@"SELECT * FROM `players` WHERE `socialClubName` = '{0}' AND `admin` > '0'", login);
            DataTable dt = lifeRP_GM.mainClass.sqlCon.retSQLData(queryString);
            if (dt.Rows.Count != 0) return true;
            return false;
        }

        /// <summary>
        /// Получить Логин Аккаунта по Имени персонажа.
        /// </summary>
        /// <param name="name">Имя персонажа.</param>
        /// <returns>Логин Аккаунта.</returns>
        public string GetAccountByName(string name)
        {
            string account = null;
            string queryString = string.Format(@"SELECT * FROM `players` WHERE `name` = '{0}'", name);
            DataTable dt = lifeRP_GM.mainClass.sqlCon.retSQLData(queryString);
            if (dt.Rows.Count > 0) account = dt.Rows[0]["socialClubName"].ToString();
            return account;
        }

        /// <summary>
        /// Получить клиента по его socialClubName.
        /// </summary>
        /// <param name="socialClubName">socialClubName.</param>
        /// <returns>Клиента.</returns>
        public Client GetClientBySocName(string socialClubName)
        {
            List<Client> Players = API.getAllPlayers();
            return Players.FirstOrDefault(player => player.socialClubName == socialClubName);
        }

        /// <summary>
        /// Получить клиента по его Имени.
        /// </summary>
        /// <param name="socialClubName">socialClubName.</param>
        /// <returns>Клиента.</returns>
        public Client GetClientByName(Client sender, string Name)
        {
            int id;

            if (int.TryParse(Name, out id))
            {
                return lifeRP_GM.mainClass.getClientFromId(sender, id, false);
            }

            List<Client> Players = new List<Client>();
            Client returnClient = null;
            int playersCount = 0;
            foreach (Client player in Players)
            {
                if (player == null) continue;

                if (player.name.ToLower().Contains(Name.ToLower()))
                {
                    if ((player.name.Equals(Name, StringComparison.OrdinalIgnoreCase))) return player;
                    else
                    {
                        playersCount++;
                        returnClient = player;
                    }
                }
            }
            if (playersCount != 1) return null;
            return returnClient;
        }

        /// <summary>
        /// Шифрование пароля SHA256.
        /// </summary>
        /// <param name="password">Пароль подлежащий шифрованию.</param>
        /// <returns>Хеш пароля в String типе.</returns>
        public string CryptoPassword(string password)
        {
            string hashed_password = "";
            SHA256Managed crypt = new SHA256Managed();

            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
            crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));

            foreach (byte crypto_byte in crypto)
            {
                hashed_password += crypto_byte.ToString("x2");
            }
            return hashed_password;
        }

        /// <summary>
        /// Отображение списка персонажей связанных с игроком.
        /// </summary>
        /// <param name="player">Игрок.</param>
        public void showCharacterlist(Client player)
        {
            lifeRP_GM MainClass = lifeRP_GM.mainClass;
            PlayerInfo pInfo = null;
            if (API.hasEntityData(player.handle, Constants.PlayerAccount))
            {
                pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                pInfo.ID = MainClass.setPlayerID(player);
                pInfo.ResetPlayerData();
            }
            else pInfo = new PlayerInfo(player, MainClass.setPlayerID(player));
            pInfo.setDimension(pInfo.ID + 1001);
            pInfo.basedata = MainClass.sqlCon.retSQLData(string.Format(@"SELECT * FROM `players` WHERE `socialClubName` = '{0}'", player.socialClubName));
            API.triggerClientEvent(player, Constants.ShowCharList, DTparseToJson(pInfo.basedata));
        }

        /// <summary>
        /// Парсит данные из бд для отправки в клиент.
        /// </summary>
        /// <param name="dt">Данные из бд в DataTable типе.</param>
        /// <returns>Лист jSon для отправки в клиент.</returns>
        public List<string> DTparseToJson(DataTable dt)
        {
            var list = new List<string>();
            for (int i = 0; i < dt.Rows.Count && i < 4; i++)
            {
                DataRow Row = dt.Rows[i];
                var name = Row["name"].ToString();
                var sex = Convert.ToInt32(Row["sex"].ToString());
                var ban = Convert.ToInt32(Row["ban"].ToString());
                var bandate = DateTime.Parse(string.Format("{0:g}", Row["bandate"].ToString()));
                if (ban != 0)
                {
                    switch (ban)
                    {
                        case 1:
                            if (DateTime.Now > bandate.AddDays(1))
                            {
                                ban = 0;
                                unban(name);
                            }
                            break;

                        case 2:
                            if (DateTime.Now > bandate.AddMonths(1))
                            {
                                ban = 0;
                                unban(name);
                            }
                            break;

                        case 3:
                            break;
                    }
                }

                var dic = new Dictionary<string, object>();
                dic["id"] = i;
                dic["ban"] = ban;
                dic["sex"] = sex;
                dic["name"] = name;
                dic["unban"] = ban != 0 ? ban == 1 ? bandate.AddDays(1).ToString() : ban == 2 ? bandate.AddMonths(1).ToString() : "Никогда" : "";
                dic["banreason"] = Row["banreason"].ToString();
                list.Add(API.toJson(dic));
            }
            return list;
        }

        /// <summary>
        /// Убирает бан на персонаже в бд.
        /// </summary>
        /// <param name="name">Имя персонажа.</param>
        public void unban(string name)
        {
            string queryString = string.Format(@"UPDATE `players` SET `ban`='{0}' WHERE `name` = '{1}'", "0", name);
            lifeRP_GM.mainClass.sqlCon.retSQLData(queryString);
        }
        #endregion
    }

    /// <summary>
    /// Класс для игроков.
    /// </summary>
    public class PlayerInfo
    {
        #region Поля класса
        public DataTable basedata = new DataTable();
        public Client player { get; set; } // Игрок Нетворка
        public Blip playerblip { get; set; } // блип игрока
        public int ID { get; set; } // Ид игрока
        public bool islogin = false; // проверка на авторизацию
        public int faction = 0; // фракция
        public int factrank = 0; // ранг в фракции
        public int admin = 0; // уровень адмнинки
        public int money = 100; // деньги наличкой
        public int bankmoney = 0; // счет в банке
        public string regIp = "127.0.0.1"; // ип при регестриции
        public int ban = 0; // бан
        public string banreason = ""; // причина бана
        public DateTime bandate = DateTime.Now; // дата бана
        public int Job = 0;
        public int jobMoney = 0; // деньги которые должен получить игрок с работы
        public bool sex = true; // пол T - Ж, F - М
        public DateTime regdate = DateTime.Now; // дата регистрации
        public DateTime lastEnterdate = DateTime.Now; // дата последнего входа
        public int lvl = 0; // уровень
        public int exp = 0; // опыт
        public bool isDead = false; // умер ли игрок
        public DateTime overtime = DateTime.Now; // время проведеное в игре за один сеанс (Расчитывается по требованию)
        public List<PlEnt> playerEntity = new List<PlEnt>(); // объекты созданные для игрока
        public struct PlEnt // структура для поддержки удаления клиентских объектов
        {
            public string Key { get; set; }
            public dynamic Value { get; set; }
        }
        #region "работа иммигранта"
        public byte Bottles = 0;
        public byte Paper = 0;
        public byte Cabel = 0;
        public NetHandle PISUN { get; set; }
        #endregion

        #region Лицо
        public bool isInCharpCreate = false;
        public bool hasCharpData = false;
        public int shapeFirst = 0;
        public int shapeSecond = 0;
        public int skinFirst = 0;
        public int skinSecond = 0;
        public float shapeMix = 0f;
        public float skinMix = 0f;
        public int hairColor = 0;
        public int hairHighColor = 0;
        public int eyeColor = 0;
        public int eyebrows = 0;
        public int eyebrowsColor = 0;
        public int eyebrowsColor2 = 0;
        public int makeup = 0;
        public int makeupColor = 0;
        public int makeupColor2 = 0;
        public int lipstick = 0;
        public int lipstickColor = 0;
        public int lipstickColor2 = 0;
        public float[] faceFeatures = new float[21];
        public int[] closes = new int[10];
        #endregion
        #endregion

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="pl">Игрок.</param>
        public PlayerInfo(Client pl, int id)
        {
            player = pl;
            //SetPlayerBlip();
            ID = id;
            SetData(Constants.PlayerAccount, this);
            SetSyncedData(Constants.PlayerFaceHasCharacterData, hasCharpData);
            SetSyncedData(Constants.PlayerIsLogin, islogin);
        }

        #region Данные игрока
        /// <summary>
        /// Сброс данных аккаунта.
        /// </summary>
        public void ResetPlayerData()
        {
            basedata = new DataTable();
            islogin = false;
            faction = 0;
            factrank = 0;
            admin = 0;
            money = 100;
            bankmoney = 0;
            regIp = "127.0.0.1";
            ban = 0;
            banreason = "";
            bandate = DateTime.Now;
            Job = 0;
            jobMoney = 0;
            sex = true;
            regdate = DateTime.Now;
            lastEnterdate = DateTime.Now;
            lvl = 0;
            exp = 0;
            isDead = false;
            overtime = DateTime.Now;
            playerEntity = new List<PlEnt>();
            RemoveDefaultData();
            RemoveFace();
            SetData(Constants.PlayerAccount, this);
            SetSyncedData(Constants.PlayerFaceHasCharacterData, hasCharpData);
            SetSyncedData(Constants.PlayerIsLogin, islogin);
            //SetPlayerBlip();
        }

        /// <summary>
        /// Создание обязательных данных для игрока.
        /// </summary>
        public void SetDefaultData()
        {
            SetSyncedData(Constants.PlayerName, player.name);
            SetSyncedData(Constants.PlayerTag, player.nametag);
            SetSyncedData(Constants.PlayerIsLogin, islogin);
            SetSyncedData(Constants.PlayerFact, faction);
            SetSyncedData(Constants.PlayerAdmin, admin);
            SetSyncedData(Constants.PlayerMoney, money);
            SetSyncedData(Constants.PlayerBankMoney, bankmoney);
            SetSyncedData(Constants.PlayerFactRank, factrank);
            SetSyncedData(Constants.PlayerJob, Job);
            SetSyncedData(Constants.PlayerLevel, lvl);
            SetSyncedData(Constants.PlayerExp, exp);
            SetData(Constants.PlayerAccount, this);
        }

        /// <summary>
        /// Удаление обязательных данных для игрока.
        /// </summary>
        public void RemoveDefaultData()
        {
            ResetData(Constants.PlayerAccount);
            ResetSyncedData(Constants.PlayerName);
            ResetSyncedData(Constants.PlayerTag);
            ResetSyncedData(Constants.PlayerBlip);
            ResetSyncedData(Constants.PlayerIsLogin);
            ResetSyncedData(Constants.PlayerFact);
            ResetSyncedData(Constants.PlayerAdmin);
            ResetSyncedData(Constants.PlayerMoney);
            ResetSyncedData(Constants.PlayerBankMoney);
            ResetSyncedData(Constants.PlayerFactRank);
            ResetSyncedData(Constants.PlayerJob);
            ResetSyncedData(Constants.PlayerLevel);
            ResetSyncedData(Constants.PlayerExp);
        }

        /// <summary>
        /// Проверка на наличие данных для игрока.
        /// </summary>
        /// <param name="key">Имя данных.</param>
        /// <returns>true - если данные существуют, false - наоборот.</returns>
        public bool hasData(string key)
        {
            return API.shared.hasEntityData(player.handle, key);
        }

        /// <summary>
        /// Проверка на наличие глобальных данных для игрока.
        /// </summary>
        /// <param name="key">Имя данных.</param>
        /// <returns>true - если данные существуют, false - наоборот.</returns>
        public bool hasSyncedData(string key)
        {
            return API.shared.hasEntitySyncedData(player.handle, key);
        }

        /// <summary>
        /// Создание локальных данных для игрока.
        /// </summary>
        /// <param name="key">Имя данных.</param>
        /// <param name="value">Значение.</param>
        public void SetData(string key, object value)
        {
            API.shared.setEntityData(player.handle, key, value);
        }

        /// <summary>
        /// Создание глобальных данных для игрока.
        /// </summary>
        /// <param name="key">Имя данных.</param>
        /// <param name="value">Значение.</param>
        public void SetSyncedData(string key, object value)
        {
            API.shared.setEntitySyncedData(player.handle, key, value);
        }

        /// <summary>
        /// Удаление данных для игрока.
        /// </summary>
        /// <param name="key">Имя данных.</param>
        public void ResetData(string key)
        {
            if (hasData(key)) API.shared.resetEntityData(player.handle, key);
        }

        /// <summary>
        /// Удаление глобальных данных для игрока.
        /// </summary>
        /// <param name="key">Имя данных.</param>
        public void ResetSyncedData(string key)
        {
            if (hasSyncedData(key)) API.shared.resetEntitySyncedData(player.handle, key);
        }

        /// <summary>
        /// Получение данных игрока.
        /// </summary>
        /// <param name="key">Имя данных.</param>
        /// <returns>Полученное значение.</returns>
        public dynamic GetData(string key)
        {
            if (hasData(key)) return API.shared.getEntityData(player.handle, key);
            else return null;
        }

        /// <summary>
        /// Получение глобальных данных игрока.
        /// </summary>
        /// <param name="key">Имя данных.</param>
        /// <returns>Полученное значение.</returns>
        public dynamic GetSyncedData(string key)
        {
            if (hasSyncedData(key)) return API.shared.getEntitySyncedData(player.handle, key);
            else return null;
        }
        #endregion

        /// <summary>
        /// Создание блипа для игрока.
        /// </summary>
        /// <returns>Созданный блип.</returns>
        private Blip SetPlayerBlip()
        {
            //API.shared.attachEntityToEntity(pBlip, player, null, new Vector3(), new Vector3());
            BlipInfo bInfo = new BlipInfo(player.position);
            playerblip = bInfo.blip;
            SetSyncedData(Constants.PlayerBlip, playerblip.handle);
            return playerblip;
        }

        /// <summary>
        /// Удаление блипа для игрока.
        /// </summary>
        public void DellPlayerBlip()
        {
            ResetSyncedData(Constants.PlayerBlip);
            playerblip.delete();
            playerblip = null;
        }

        #region Работа с БД
        /// <summary>
        /// Запись в класс основных данных полученных из Базы Данных.
        /// </summary>
        /// <param name="data">Данные из Базы Данных.</param>
        public void LoadDatafromBD(DataRow Row)
        {
            player.name = Row["name"].ToString();
            player.nametag = string.Format("{0} ({1})", player.name, ID);
            faction = Convert.ToInt32(Row["faction"]);
            admin = Convert.ToInt32(Row["admin"]);
            money = Convert.ToInt32(Row["money"]);
            sex = Convert.ToBoolean(Row["sex"]);
            regIp = Row["regIp"].ToString();
            factrank = Convert.ToInt32(Row["factrank"]);
            ban = Convert.ToInt32(Row["ban"].ToString());
            banreason = Row["banreason"].ToString();
            bandate = DateTime.Parse(string.Format("{0:g}", Row["bandate"].ToString()));
            bankmoney = Convert.ToInt32(Row["bankmoney"]);
            jobMoney = Convert.ToInt32(Row["jobmoney"]);
            regdate = DateTime.Parse(string.Format("{0:g}", Row["regdate"].ToString()));
            lastEnterdate = DateTime.Now;
            lvl = Convert.ToInt32(Row["lvl"]);
            exp = Convert.ToInt32(Row["exp"]);
        }

        /// <summary>
        /// Запись основных данных класса в Базу Данных.
        /// </summary>
        /// <param name="name">Ник игрока.</param>
        /// <param name="password">Пароль.</param>
        public void LoadDataintoBD(string name)
        {
            player.name = name;
            player.nametag = string.Format("{0} ({1})", name, ID); ;
            int intsex = Convert.ToInt32(sex);
            string queryString = string.Format(@"INSERT INTO `players` (`name`, `faction`, `admin`, `money`, `sex`, `regIp`, `socialClubName`, `factrank`, `ban`, `banreason`, `bandate`, `jobmoney`, 
                                                `bankmoney`, `regdate`, `lastEnterdate`, `lvl`, `exp`) 
                                                VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}')",
                                      name, faction.ToString(), admin.ToString(), money.ToString(), intsex.ToString(), player.address,
                                      player.socialClubName, factrank.ToString(), ban.ToString(), banreason.ToString(), bandate.ToString(),
                                      jobMoney.ToString(), bankmoney.ToString(), regdate.ToString(), lastEnterdate.ToString(),
                                      lvl.ToString(), exp.ToString());

            lifeRP_GM.mainClass.sqlCon.retSQLData(queryString);
        }

        /// <summary>
        /// Обновление записи в Базе Данных на основе основных данных из класса.
        /// </summary>
        public void UpdateBD()
        {
            int intsex = Convert.ToInt32(sex);
            string queryString = string.Format(@"UPDATE `players` SET `faction`='{0}', `admin`='{1}', `money`='{2}', `sex`='{3}',
                                                `factrank`='{4}', `jobmoney`='{5}',  `bankmoney`='{6}', `lastEnterdate`='{7}', `lvl`='{8}', `exp`='{9}' WHERE `name` = '{10}'",
                                      faction.ToString(), admin.ToString(), money.ToString(), intsex.ToString(),
                                      factrank.ToString(), jobMoney.ToString(), bankmoney.ToString(),
                                      lastEnterdate.ToString(), lvl.ToString(), exp.ToString(), player.name);

            lifeRP_GM.mainClass.sqlCon.retSQLData(queryString);
        }
        #endregion

        #region Работа с лицом персонажа
        /// <summary>
        /// Сброс данных класса связанных с лицом персонажа.
        /// </summary>
        private void ResetFaceData()
        {
            hasCharpData = false;
            shapeFirst = 0;
            shapeSecond = 0;
            skinFirst = 0;
            skinSecond = 0;
            shapeMix = 0f;
            skinMix = 0f;
            hairColor = 0;
            hairHighColor = 0;
            eyeColor = 0;
            eyebrows = 0;
            eyebrowsColor = 0;
            eyebrowsColor2 = 0;
            makeup = 0;
            makeupColor = 0;
            makeupColor2 = 0;
            lipstick = 0;
            lipstickColor = 0;
            lipstickColor2 = 0;
            for (var i = 0; i < 21; i++)
            {
                faceFeatures[i] = 0f;
            }
            for (var i = 0; i < 10; i++)
            {
                closes[i] = 0;
            }
        }

        /// <summary>
        /// Создание глобальных данных связанных с лицом персонажа.
        /// </summary>
        /// <param name="reset">Сбрасывать данные класса.</param>
        public void InitializeFace(bool reset)
        {
            if (reset) ResetFaceData();
            hasCharpData = true;

            SetSyncedData(Constants.PlayerFaceHasCharacterData, hasCharpData);
            SetSyncedData(Constants.PlayerFaceShapeFirstID, shapeFirst);
            SetSyncedData(Constants.PlayerFaceShapeSecondID, shapeSecond);
            SetSyncedData(Constants.PlayerFaceSkinFirstID, skinFirst);
            SetSyncedData(Constants.PlayerFaceSkinSecondID, skinSecond);
            SetSyncedData(Constants.PlayerFaceShapeMix, shapeMix);
            SetSyncedData(Constants.PlayerFaceSkinMix, skinMix);
            SetSyncedData(Constants.PlayerFaceHairColor, hairColor);
            SetSyncedData(Constants.PlayerFaceHairHighlightColor, hairHighColor);
            SetSyncedData(Constants.PlayerFaceEyeColor, eyeColor);
            SetSyncedData(Constants.PlayerFaceEyebrows, eyebrows);
            SetSyncedData(Constants.PlayerFaceEyebrowsColor, eyebrowsColor);
            SetSyncedData(Constants.PlayerFaceEyebrowsColor2, eyebrowsColor2);
            if (makeup != 0)
            {
                SetSyncedData(Constants.PlayerFaceMakeup, makeup);
            }
            SetSyncedData(Constants.PlayerFaceMakeupColor, makeupColor);
            SetSyncedData(Constants.PlayerFaceMakeupColor2, makeupColor2);
            if (lipstick != 0)
            {
                SetSyncedData(Constants.PlayerFaceLipstick, lipstick);
            }
            SetSyncedData(Constants.PlayerFaceLipstickColor, lipstickColor);
            SetSyncedData(Constants.PlayerFaceLipstickColor2, lipstickColor2);
            SetSyncedData(Constants.PlayerFaceFeaturesList, faceFeatures);
            SetSyncedData(Constants.PlayerFaceClosesList, closes);
        }

        /// <summary>
        /// Запись в класс данных о лице персонажа полученных из Базы Данных.
        /// </summary>
        /// <param name="data">Данные из Базы Данных.</param>
        public void LoadFaceFromBD(DataTable data)
        {
            DataRow Row = data.Rows[0];
            shapeFirst = Convert.ToInt32(Row["shapeFirst"]);
            shapeSecond = Convert.ToInt32(Row["shapeSecond"]);
            skinFirst = Convert.ToInt32(Row["skinFirst"]);
            skinSecond = Convert.ToInt32(Row["skinSecond"]);
            shapeMix = Convert.ToSingle(Row["shapeMix"]);
            skinMix = Convert.ToSingle(Row["skinMix"]);
            hairColor = Convert.ToInt32(Row["hairColor"]);
            hairHighColor = Convert.ToInt32(Row["hairHighColor"]);
            eyeColor = Convert.ToInt32(Row["eyeColor"]);
            eyebrows = Convert.ToInt32(Row["eyebrows"]);
            eyebrowsColor = Convert.ToInt32(Row["eyebrowsColor"]);
            eyebrowsColor2 = Convert.ToInt32(Row["eyebrowsColor2"]);
            makeup = Convert.ToInt32(Row["makeup"]);
            makeupColor = Convert.ToInt32(Row["makeupColor"]);
            makeupColor2 = Convert.ToInt32(Row["makeupColor2"]);
            lipstick = Convert.ToInt32(Row["lipstick"]);
            lipstickColor = Convert.ToInt32(Row["lipstickColor"]);
            lipstickColor2 = Convert.ToInt32(Row["lipstickColor2"]);
            string[] STfaceFeatures = Row["faceFeatures"].ToString().Split(' ');
            string[] STcloses = Row["closes"].ToString().Split(' ');

            for (int i = 0; i < STfaceFeatures.Count(); i++)
            {
                if (STfaceFeatures[i] != null && STfaceFeatures[i] != "") faceFeatures[i] = float.Parse(STfaceFeatures[i]);
            }

            for (int i = 0; i < STcloses.Count(); i++)
            {
                if (STcloses[i] != null && STcloses[i] != "") closes[i] = int.Parse(STcloses[i]);
            }
            InitializeFace(false);
            UpdateFace();
            UpdateDress();
        }

        /// <summary>
        /// Запись данных о лице персонажа в Базу Данных.
        /// </summary>
        public void LoadFaceintoBD()
        {
            string STfaceFeatures = "";
            foreach (var ff in faceFeatures)
            {
                STfaceFeatures += ff.ToString() + " ";
            }

            string STcloses = "";
            foreach (var cl in closes)
            {
                STcloses += cl.ToString() + " ";
            }

            string queryString = string.Format(@"INSERT INTO `face` (`name`, `shapeFirst`, `shapeSecond`, `skinFirst`, `skinSecond`, `shapeMix`, `skinMix`, `hairColor`, 
                                               `hairHighColor`, `eyeColor`, `eyebrows`, `eyebrowsColor`, `eyebrowsColor2`, `makeupColor`, `makeupColor2`, `lipstickColor`, `lipstickColor2`, 
                                                `faceFeatures`, `closes`) 
                                                VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')",
                                                player.name, shapeFirst.ToString(), shapeSecond.ToString(), skinFirst.ToString(), skinSecond.ToString(), shapeMix.ToString(), skinMix.ToString(), hairColor.ToString(),
                                                hairHighColor.ToString(), eyeColor.ToString(), eyebrows.ToString(), eyebrowsColor.ToString(), eyebrowsColor2.ToString(), makeupColor.ToString(), makeupColor2.ToString(),
                                                lipstickColor.ToString(), lipstickColor2.ToString(), STfaceFeatures, STcloses);

            lifeRP_GM.mainClass.sqlCon.retSQLData(queryString);
        }

        /// <summary>
        /// Обновление записи в Базе Данных на основе данных о лице персонажа.
        /// </summary>
        public void UpdateFaceBD()
        {
            string STfaceFeatures = "";
            foreach (var ff in faceFeatures)
            {
                STfaceFeatures += ff.ToString() + " ";
            }

            string STcloses = "";
            foreach (var cl in closes)
            {
                STcloses += cl.ToString() + " ";
            }

            string queryString = string.Format(@"UPDATE `face` SET `shapeFirst`='{0}', `shapeSecond`='{1}', `skinFirst`='{2}', `skinSecond`='{3}', `shapeMix`='{4}',
                                                `skinMix`='{5}', `hairColor`='{6}',  `hairHighColor`='{7}', `eyeColor`='{8}', `eyebrows`='{9}', `eyebrowsColor`='{10}', `eyebrowsColor2`='{11}', 
                                                `makeupColor`='{12}', `makeupColor2`='{13}', `lipstickColor`='{14}', `lipstickColor2`='{15}', `faceFeatures`='{16}', `closes`='{17}' WHERE `name` = '{18}'",
                                                shapeFirst.ToString(), shapeSecond.ToString(), skinFirst.ToString(), skinSecond.ToString(), shapeMix.ToString(), skinMix.ToString(), hairColor.ToString(),
                                                hairHighColor.ToString(), eyeColor.ToString(), eyebrows.ToString(), eyebrowsColor.ToString(), eyebrowsColor2.ToString(), makeupColor.ToString(), makeupColor2.ToString(),
                                                lipstickColor.ToString(), lipstickColor2.ToString(), STfaceFeatures, STcloses, player.name);

            lifeRP_GM.mainClass.sqlCon.retSQLData(queryString);
        }

        /// <summary>
        /// Удаление данных о лице персонажа.
        /// </summary>
        public void RemoveFace()
        {
            ResetFaceData();

            ResetSyncedData(Constants.PlayerFaceHasCharacterData);
            ResetSyncedData(Constants.PlayerFaceShapeFirstID);
            ResetSyncedData(Constants.PlayerFaceShapeSecondID);
            ResetSyncedData(Constants.PlayerFaceSkinFirstID);
            ResetSyncedData(Constants.PlayerFaceSkinSecondID);
            ResetSyncedData(Constants.PlayerFaceShapeMix);
            ResetSyncedData(Constants.PlayerFaceSkinMix);
            ResetSyncedData(Constants.PlayerFaceHairColor);
            ResetSyncedData(Constants.PlayerFaceHairHighlightColor);
            ResetSyncedData(Constants.PlayerFaceEyeColor);
            ResetSyncedData(Constants.PlayerFaceEyebrows);
            ResetSyncedData(Constants.PlayerFaceEyebrowsColor);
            ResetSyncedData(Constants.PlayerFaceEyebrowsColor2);
            if (makeup != 0)
            {
                ResetSyncedData(Constants.PlayerFaceMakeup);
            }
            ResetSyncedData(Constants.PlayerFaceMakeupColor);
            ResetSyncedData(Constants.PlayerFaceMakeupColor2);
            if (lipstick != 0)
            {
                ResetSyncedData(Constants.PlayerFaceLipstick);
            }
            ResetSyncedData(Constants.PlayerFaceLipstick);
            ResetSyncedData(Constants.PlayerFaceLipstickColor2);
            ResetSyncedData(Constants.PlayerFaceFeaturesList);
            ResetSyncedData(Constants.PlayerFaceClosesList);
        }

        /// <summary>
        /// Проверка на существование данных о лице персонажа.
        /// </summary>
        /// <returns>Существует запись или нет.</returns>
        public bool isFaceValid()
        {
            if (!hasSyncedData(Constants.PlayerFaceShapeFirstID)) return false;
            if (!hasSyncedData(Constants.PlayerFaceShapeSecondID)) return false;
            if (!hasSyncedData(Constants.PlayerFaceSkinFirstID)) return false;
            if (!hasSyncedData(Constants.PlayerFaceSkinSecondID)) return false;
            if (!hasSyncedData(Constants.PlayerFaceShapeMix)) return false;
            if (!hasSyncedData(Constants.PlayerFaceSkinMix)) return false;
            if (!hasSyncedData(Constants.PlayerFaceHairColor)) return false;
            if (!hasSyncedData(Constants.PlayerFaceHairHighlightColor)) return false;
            if (!hasSyncedData(Constants.PlayerFaceEyeColor)) return false;
            if (!hasSyncedData(Constants.PlayerFaceEyebrows)) return false;
            if (!hasSyncedData(Constants.PlayerFaceEyebrowsColor)) return false;
            if (!hasSyncedData(Constants.PlayerFaceEyebrowsColor2)) return false;
            if (makeup != 0)
            {
                if (!hasSyncedData(Constants.PlayerFaceMakeup)) return false;
            }
            if (!hasSyncedData(Constants.PlayerFaceMakeupColor)) return false;
            if (!hasSyncedData(Constants.PlayerFaceMakeupColor2)) return false;
            if (lipstick != 0)
            {
                if (!hasSyncedData(Constants.PlayerFaceLipstick)) return false;
            }
            if (!hasSyncedData(Constants.PlayerFaceLipstickColor)) return false;
            if (!hasSyncedData(Constants.PlayerFaceLipstickColor2)) return false;
            if (!hasSyncedData(Constants.PlayerFaceFeaturesList)) return false;
            if (!hasSyncedData(Constants.PlayerFaceClosesList)) return false;

            return true;
        }

        /// <summary>
        /// Визуальное обновление лица персонажа.
        /// </summary>
        public void UpdateFace()
        {
            API.shared.triggerClientEventForAll(Constants.UpdateChar, player.handle);
        }

        /// <summary>
        /// Визуальное обновление одежды персонажа.
        /// </summary>
        public void UpdateDress()
        {
            API.shared.setPlayerClothes(player, 1, closes[0], 0);
            API.shared.setPlayerClothes(player, 2, closes[1], 0);
            API.shared.setPlayerClothes(player, 3, closes[2], 0);
            API.shared.setPlayerClothes(player, 4, closes[3], 0);
            API.shared.setPlayerClothes(player, 5, closes[4], 0);
            API.shared.setPlayerClothes(player, 6, closes[5], 0);
            API.shared.setPlayerClothes(player, 7, closes[6], 0);
            API.shared.setPlayerClothes(player, 8, closes[7], 0);
            API.shared.setPlayerClothes(player, 9, closes[8], 0);
            API.shared.setPlayerClothes(player, 11, closes[9], 0);
        }

        /// <summary>
        /// Подготовка к созданию\применению лица.
        /// </summary>
        public void PlayerFace()
        {
            if (sex) API.shared.setPlayerSkin(player, PedHash.FreemodeMale01);
            else API.shared.setPlayerSkin(player, PedHash.FreemodeFemale01);
            API.shared.sendNativeToPlayer(player, 0x45EEE61580806D63, player.handle);

            DataTable faceData = lifeRP_GM.mainClass.sqlCon.retSQLData(string.Format(@"SELECT * FROM `face` WHERE `name` = '{0}'", player.name));
            bool HasfaceData = Convert.ToBoolean(faceData.Rows.Count);
            if (HasfaceData)
            {
                LoadFaceFromBD(faceData);
                islogin = true;
                SetSyncedData(Constants.PlayerIsLogin, islogin);
                PlayerRespawn();
                return;
            }
            else OpenFaceMenu(true);
        }

        /// <summary>
        /// Открытие меню создания персонажа.
        /// </summary>
        /// <param name="FirstTime">Стоит сбрасивать данные о лице персонажа или нет.</param>
        public void OpenFaceMenu(bool FirstTime)
        {
            if (!FirstTime && isFaceValid())
            {
                isInCharpCreate = true;
                API.shared.triggerClientEvent(player, Constants.CharMenuOpen, player.handle, false);
                return;
            }
            isInCharpCreate = true;
            InitializeFace(true);
            API.shared.triggerClientEvent(player, Constants.CharMenuOpen, player.handle, true);
        }

        /// <summary>
        /// Закрытие меню создания персонажа.
        /// </summary>
        public void FaceMenuClosed()
        {
            shapeFirst = GetSyncedData(Constants.PlayerFaceShapeFirstID);
            shapeSecond = GetSyncedData(Constants.PlayerFaceShapeSecondID);
            skinFirst = GetSyncedData(Constants.PlayerFaceSkinFirstID);
            skinSecond = GetSyncedData(Constants.PlayerFaceSkinSecondID);
            shapeMix = GetSyncedData(Constants.PlayerFaceShapeMix);
            skinMix = GetSyncedData(Constants.PlayerFaceSkinMix);
            hairColor = GetSyncedData(Constants.PlayerFaceHairColor);
            hairHighColor = GetSyncedData(Constants.PlayerFaceHairHighlightColor);
            eyeColor = GetSyncedData(Constants.PlayerFaceEyeColor);
            eyebrows = GetSyncedData(Constants.PlayerFaceEyebrows);
            eyebrowsColor = GetSyncedData(Constants.PlayerFaceEyebrowsColor);
            eyebrowsColor2 = GetSyncedData(Constants.PlayerFaceEyebrowsColor2);
            makeup = GetSyncedData(Constants.PlayerFaceMakeup) ?? 0;
            makeupColor = GetSyncedData(Constants.PlayerFaceMakeupColor);
            makeupColor2 = GetSyncedData(Constants.PlayerFaceMakeupColor2);
            lipstick = GetSyncedData(Constants.PlayerFaceLipstick) ?? 0;
            lipstickColor = GetSyncedData(Constants.PlayerFaceLipstickColor);
            lipstickColor2 = GetSyncedData(Constants.PlayerFaceLipstickColor2);
            List<object> faceFeatureList = GetSyncedData(Constants.PlayerFaceFeaturesList);
            faceFeatures = Array.ConvertAll(faceFeatureList.ToArray(), Convert.ToSingle);
            List<object> closesList = GetSyncedData(Constants.PlayerFaceClosesList);
            closes = Array.ConvertAll(closesList.ToArray(), Convert.ToInt32);

            isInCharpCreate = false;
            LoadFaceintoBD();
            UpdateDress();
            UpdateFace();
            islogin = true;
            PlayerRespawn();
        }
        #endregion

        /// <summary>
        /// Логика респавна игрока.
        /// </summary>
        public void PlayerRespawn()
        {
            if (isDead)
            {
                API.shared.sendChatMessageToPlayer(player, lifeRP_GM.mainClass.chatMessage("~y~С Вас взята плата за мед. обслуживание."));
                API.shared.setEntityPosition(player.handle, new Vector3(339.6, -1395.87, 32.51));
                isDead = false;
                return;
            }
            API.shared.setEntityPosition(player.handle, new Vector3(26.19, -1221.70, 29.29));//респавн основной
            setDimension(0);
            overtime = DateTime.Now;
            API.shared.sendNotificationToPlayer(player, "~y~[Server] ~w~Аккаунт авторизирован.");
            if (admin != 0)
            {
                if (admin > 3)
                {
                    ServerPlayers.playersClass.Admins.Add(player);
                    ServerPlayers.playersClass.Developers.Add(player);
                    API.shared.sendChatMessageToAll(lifeRP_GM.mainClass.chatMessage(string.Format("~y~[Server] ~r~{0} ~w~{1} в сети.", Enum.GetName(typeof(AdmRank), admin), player.name)));
                }
                else
                {
                    ServerPlayers.playersClass.Admins.Add(player);
                    API.shared.sendChatMessageToAll(lifeRP_GM.mainClass.chatMessage(string.Format("~y~[Server] {0}{1} ~w~{2} в сети.", admin >= 2 ? "~o~" : "", Enum.GetName(typeof(AdmRank), admin), player.name)));
                }
            }
        }

        /// <summary>
        /// Удаление всех, связанных с игроком, клиентских объектов.
        /// </summary>
        public void DeleteAllplayerEntity()
        {
            for (int i = 0; i < playerEntity.Count; i++)
            {
                var pEntity = playerEntity[i];
                switch (pEntity.Key)
                {
                    case Constants.Blip:
                        BlipInfo bInfo = pEntity.Value;
                        bInfo.DeleteBlip();
                        break;

                    case Constants.Marker:
                        MarkerInfo mInfo = pEntity.Value;
                        mInfo.DeleteMarker();
                        break;

                    case Constants.tLabel:
                        TLabelInfo tlInfo = pEntity.Value;
                        tlInfo.DeleteTextLabel();
                        break;
                }
                i--;
            }
        }

        /// <summary>
        /// Удаление всех, связанных с игроком, клиентских Блипов.
        /// </summary>
        public void DeletePlayerBlips()
        {
            for (int i = 0; i < playerEntity.Count; i++)
            {
                var pEntity = playerEntity[i];
                if (pEntity.Key == Constants.Blip)
                {
                    BlipInfo bInfo = pEntity.Value;
                    bInfo.DeleteBlip();
                    i--;
                }
            }
        }

        /// <summary>
        /// Удаление всех, связанных с игроком, клиентских Маркеров.
        /// </summary>
        public void DeletePlayerMarkers()
        {
            for (int i = 0; i < playerEntity.Count; i++)
            {
                var pEntity = playerEntity[i];
                if (pEntity.Key == Constants.Marker)
                {
                    MarkerInfo mInfo = pEntity.Value;
                    mInfo.DeleteMarker();
                    i--;
                }
            }
        }

        /// <summary>
        /// Удаление всех, связанных с игроком, клиентских Текстлейблов.
        /// </summary>
        public void DeletePlayerTLabels()
        {
            for (int i = 0; i < playerEntity.Count; i++)
            {
                var pEntity = playerEntity[i];
                if (pEntity.Key == Constants.tLabel)
                {
                    TLabelInfo tlInfo = pEntity.Value;
                    tlInfo.DeleteTextLabel();
                    i--;
                }
            }
        }

        /// <summary>
        /// Изменение измерения.
        /// </summary>
        /// <param name="dim">Номер измерения.</param>
        public void setDimension(int dim)
        {
            if (playerblip != null) API.shared.setEntityDimension(playerblip, dim);
            API.shared.setEntityDimension(player, dim);
        }

        /// <summary>
        /// Заблокировать управление пользователя..
        /// </summary>        
        public void FreezePlayer()
        {
            API.shared.freezePlayer(player, true);
        }

        /// <summary>
        /// Возобновить управление пользователя.
        /// </summary>       
        public void UnFreezePlayer()
        {
            API.shared.freezePlayer(player, false);
        }
    }

    /// <summary>
    /// Класс обрабатывающий логику работы с транспортом.
    /// </summary>
    public class ServerVehicle : Script
    {
        public static ServerVehicle vehicleClass; // Экземпляр класса автомобилей исполняемого сервером.

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        public ServerVehicle()
        {
            API.onResourceStart += onResourceStart;
            API.onPlayerEnterVehicle += onPlayerEnterVehicle;
            API.onPlayerExitVehicle += onPlayerExitVehicle;
            API.onVehicleDeath += onVehicleDeath;
            API.onVehicleTrailerChange += onVehicleTrailerChange;
            API.onClientEventTrigger += onClientEvent;
            API.onUpdate += onUpdate;
            API.onEntityDataChange += onEntityDataChange;
        }

        #region Методы событий
        private void onResourceStart()
        {
            vehicleClass = this;
        }

        private void onPlayerEnterVehicle(Client player, NetHandle vehicle)
        {
            try
            {
                if (lifeRP_GM.Debug) ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] ~w~onPlayerEnterVehicle");
                if (API.hasEntityData(vehicle, Constants.VehClass))
                {
                    VehInfo vInfo = API.getEntityData(vehicle, Constants.VehClass);
                    vInfo.timer.Stop();
                    //API.sendChatMessageToPlayer(player, lifeRP_GM.mainClass.chatMessage(string.Format("Кол-во топлива: {0} Класс: {1} Расход: {2} Бак: {3} Тип топлива: {4}.", vInfo.VehData.AmountFuel, vInfo.VehData.vehClass.VehClass, vInfo.VehData.vehClass.FuelConsum, vInfo.VehData.vehClass.FuelTank, vInfo.VehData.vehClass.FuelType)));
                    if (API.getPlayerVehicleSeat(player) == -1) API.sendNotificationToPlayer(player, "~y~[Server] ~w~Завести двигатель на кнопку \"2\".");
                }
                else API.deleteEntity(vehicle);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка во время посадки в транспорт: " + exp.Message);
                ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onPlayerEnterVehicle");
                return;
            }
        }

        private void onPlayerExitVehicle(Client player, NetHandle vehicle)
        {
            try
            {
                if (lifeRP_GM.Debug) ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] ~w~onPlayerExitVehicle");
                if (API.hasEntityData(vehicle, Constants.VehClass))
                {
                    VehInfo vInfo = API.getEntityData(vehicle, Constants.VehClass);
                    if (API.getVehicleOccupants(vehicle).Count() == 0) if (vInfo.timerOut != 0) vInfo.timer.Start();
                }
                else API.deleteEntity(vehicle);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка во время выхода из транспорта: " + exp.Message);
                ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onPlayerExitVehicle");
                return;
            }
        }

        private void onVehicleDeath(NetHandle vehicle)
        {
            try
            {
                if (lifeRP_GM.Debug) ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] ~w~onVehicleDeath");
                if (API.hasEntityData(vehicle, Constants.VehClass))
                {
                    VehInfo vInfo = API.getEntityData(vehicle, Constants.VehClass);
                    if (vInfo.Job == Jobs.ADMIN)
                    {
                        API.deleteEntity(vehicle);
                        return;
                    }
                    vInfo.ToRespawn(15000);
                }
                else API.deleteEntity(vehicle);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка во время уничтожения транспорта: " + exp.Message);
                ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onVehicleDeath");
                return;
            }
        }

        private void onVehicleTrailerChange(NetHandle tower, NetHandle trailer)
        {
            if (lifeRP_GM.Debug) ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] ~w~onVehicleTrailerChange");
        }

        private void onClientEvent(Client player, string eventName, params object[] arguments)
        {
            switch (eventName)
            {
                case Constants.VehEngineToggle: // 4-lifeRP_keyevent - включение мотора авто
                    try
                    {
                        if (player.isInVehicle)
                        {
                            if (API.getPlayerVehicleSeat(player) == -1)
                            {
                                Vehicle veh = player.vehicle;
                                if (veh.engineStatus) player.vehicle.engineStatus = false;
                                else player.vehicle.engineStatus = true;
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        API.consoleOutput("Во время запуска двиготеля произошла ошибка: " + exp.Message);
                        ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onClientEvent");
                        return;
                    }
                    break;
            }
        }

        private void onUpdate()
        {
        }

        private void onEntityDataChange(NetHandle entity, string key, object oldValue)
        {
            try
            {
                string[] mKey = key.Split('_');
                if (mKey[0] == Constants.Veh)
                {
                    if (API.hasEntityData(entity, Constants.VehClass))
                    {
                        dynamic ClassData = null;
                        VehInfo vInfo = API.getEntityData(entity, Constants.VehClass);
                        switch (key)
                        {
                            case Constants.VehJob:
                                ClassData = (int)vInfo.Job;
                                if (ClassData != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, (int)vInfo.Job);
                                break;

                            case Constants.VehFuel:
                                ClassData = JsonConvert.SerializeObject(vInfo.VehData);
                                //VehInfo.vehData data = JsonConvert.DeserializeObject<VehInfo.vehData>((string)API.getEntitySyncedData(entity, key));
                                if (ClassData != API.getEntitySyncedData(entity, key)) API.setEntitySyncedData(entity, key, (int)vInfo.Job);
                                break;
                        }
                    }
                    else API.deleteEntity(entity);
                }
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в onEntityDataChange Ключ -  " + key + " " + exp.ToString());
                ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в onEntityDataChange");
                return;
            }
        }
        #endregion

        #region Комманды        
        [Command("car")]
        public void SpawnCarCommand(Client player, VehicleHash model)
        {
            try
            {
                if (API.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    lifeRP_GM MainClass = lifeRP_GM.mainClass;
                    PlayerInfo pInfo = API.getEntityData(player.handle, Constants.PlayerAccount);
                    if (pInfo.islogin)
                    {
                        if (pInfo.admin > 0)
                        {
                            var rot = API.getEntityRotation(player.handle);
                            var veh = new VehInfo(model, player.position, rot, 0, 0, 100, Jobs.ADMIN, 10).Veh;
                            API.setPlayerIntoVehicle(player, veh, -1);
                        }
                        else API.sendChatMessageToPlayer(player, pInfo.admin != 0 ? MainClass.chatMessage("~y~[Server] ~w~Ошибка: Доступно только стАдмин и выше.") : MainClass.chatMessage(Constants.NonAdminAccessDenied));
                    }
                    else API.sendChatMessageToPlayer(player, MainClass.chatMessage(Constants.errorAutor));
                }
                else API.kickPlayer(player, Constants.KickCheatMessage);
            }
            catch (Exception exp)
            {
                API.consoleOutput("Произошла ошибка в SpawnCarCommand -  " + exp.Message);
                ServerPlayers.playersClass.sendChatMessageToDevelopers("~r~[DevChat] Произошла ошибка в SpawnCarCommand");
                return;
            }
        }
        #endregion
    }

    /// <summary>
    /// Класс для работы над транспортом.
    /// </summary>
    public class VehInfo
    {
        #region Поля класса
        public Vehicle Veh { get; set; } // транспорт
        public Vector3 pos = new Vector3(); // позиция транспорта
        public Vector3 rot = new Vector3(); // разворот транспорта
        public int dim = 0;
        public Jobs Job = Jobs.NULL; // работа к которой относиться транспорт
        public System.Timers.Timer timer = new System.Timers.Timer(); // таймер для спавна
        public int timerOut = new int(); // через сколько, после выхода из транспорта, транспорт спавнить
        private Dictionary<string, dynamic> EntityData = new Dictionary<string, dynamic>(); // Массив локальных данных
        private Dictionary<string, dynamic> SynEntityData = new Dictionary<string, dynamic>(); // Массив глобальных данных
        public vehData VehData { get; set; }  //бензин
        public struct vehData //Скруктура для бензина
        {
            public int AmountFuel { get; set; }  // кол-во топлива
            public VehicleClasses.vehClassData vehClass { get; set; }  // класс траспорта

            public vehData(int AmountFuel, VehicleClasses.vehClassData vehClass) : this() // конструктор структуры
            {
                this.AmountFuel = AmountFuel;
                this.vehClass = vehClass;
            }
        }
        #endregion

        /// <summary>
        /// Создание серверного транспорта.
        /// </summary>
        /// <param name="Veh">Транспорт.</param>
        /// <param name="Job">Работа, к которой принадлежит транспорт.</param>
        /// <param name="timerOut">Задержка перед респавном (0 чтобы машина не респавнилась).</param>
        /// <param name="dim">Дименшен.</param>
        public VehInfo(VehicleHash vHash, Vector3 pos, Vector3 rot, int color1, int color2, int AmountFuel, Jobs Job = Jobs.NULL, int timerOut = 60 * 5, int dim = 0)
        {
            this.dim = dim;
            this.Job = Job;
            this.timerOut = timerOut;
            if (timerOut != 0) timer.Interval = 1000 * timerOut;
            timer.Elapsed += OnTimedEvent;
            this.pos = pos;
            this.rot = rot;
            VehicleClasses.vehClassData vehclass = new VehicleClasses.vehClassData((int)vehClasses.ClassA, 100, 1000, (int)FuelTypes.petrol);
            VehicleClasses.Clases.TryGetValue(vHash.ToString(), out vehclass);
            VehData = new vehData(AmountFuel, vehclass);

            Veh = API.shared.createVehicle(vHash, pos, rot, color1, color2, dim);
            API.shared.setVehicleNumberPlate(Veh, "4LifeRP.ru"); // номерной знак
            API.shared.setVehicleNumberPlateStyle(Veh, 1); //стиль номерного знака 0-5
            Veh.engineStatus = false;
            SetDefaultData();
        }

        public VehInfo DoorLock()
        {
            API.shared.setVehicleLocked(Veh, true);
            return this;
        }

        public VehInfo DoorUnLock()
        {
            API.shared.setVehicleLocked(Veh, false);
            return this;
        }

        /// <summary>
        /// Удалить транспорт.
        /// </summary>
        public void DeleteVehicle()
        {
            timer.Stop();
            API.shared.deleteEntity(Veh.handle);
        }

        /// <summary>
        /// Создание обязательных данных.
        /// </summary>
        public void SetDefaultData()
        {
            SetData(Constants.VehClass, this);
            SetSyncedData(Constants.VehJob, (int)Job);
            SetSyncedData(Constants.VehFuel, JsonConvert.SerializeObject(VehData));
        }

        /// <summary>
        /// Удаление обязательных данных.
        /// </summary>
        public void ResetDefultData()
        {
            ResetData(Constants.VehClass);
            ResetSyncedData(Constants.VehJob);
            ResetSyncedData(Constants.VehFuel);
        }

        /// <summary>
        /// Создание локальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <param name="value">Значение.</param>
        public void SetData(string key, object value)
        {
            API.shared.setEntityData(Veh.handle, key, value);
        }

        /// <summary>
        /// Создание глобальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <param name="value">Значение.</param>
        public void SetSyncedData(string key, object value)
        {
            API.shared.setEntitySyncedData(Veh.handle, key, value);
        }

        /// <summary>
        /// Проверка на наличие локальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <returns>true - если данные существуют, false - наоборот.</returns>
        public bool hasData(string key)
        {
            return API.shared.hasEntityData(Veh.handle, key);
        }

        /// <summary>
        /// Проверка на наличие глобальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <returns>true - если данные существуют, false - наоборот.</returns>
        public bool hasSyncedData(string key)
        {
            return API.shared.hasEntitySyncedData(Veh.handle, key);
        }

        /// <summary>
        /// Получение локальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <returns>Значение.</returns>
        public dynamic GetData(string key)
        {
            if (hasData(key)) return API.shared.getEntityData(Veh.handle, key);
            else return null;
        }

        /// <summary>
        /// Получение глобальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <returns>Значение.</returns>
        public dynamic GetSyncedData(string key)
        {
            if (hasSyncedData(key)) return API.shared.getEntitySyncedData(Veh.handle, key);
            else return null;
        }

        /// <summary>
        /// Удаление локальных данных
        /// </summary>
        /// <param name="key">Имя поля.</param>
        public void ResetData(string key)
        {
            if (hasData(key)) API.shared.resetEntityData(Veh.handle, key);
        }

        /// <summary>
        /// Удаление глобальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        public void ResetSyncedData(string key)
        {
            if (hasSyncedData(key)) API.shared.resetEntitySyncedData(Veh.handle, key);
        }

        /// <summary>
        /// Собитые таймера.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTimedEvent(System.Object source, ElapsedEventArgs e)
        {
            timer.Stop();
            if (Job == Jobs.ADMIN)
            {
                DeleteVehicle();
                return;
            }
            ToRespawn();
        }

        /// <summary>
        /// Респавн транспорта.
        /// </summary>
        /// <param name="ms">Задержка перед респавном.</param>
        public void ToRespawn(int ms = 0, bool ReplaceData = false)
        {
            lifeRP_GM.mainClass.Stream(ms, () =>
            {
                if (ReplaceData) getAllEntityData();

                var color1 = API.shared.getVehiclePrimaryColor(Veh.handle);
                var color2 = API.shared.getVehicleSecondaryColor(Veh.handle);
                var model = API.shared.getEntityModel(Veh.handle);
                var dim = API.shared.getEntityDimension(Veh.handle);

                API.shared.deleteEntity(Veh.handle);
                Veh = API.shared.createVehicle((VehicleHash)model, pos, rot, color1, color2, dim);
                Veh.engineStatus = false;
                if (ReplaceData) setAllEntityData();
                SetDefaultData();
                API.shared.setVehicleEngineStatus(Veh.handle, false);
            });
        }

        /// <summary>
        /// Получить все локальные и глобальные данные.
        /// </summary>
        public void getAllEntityData()
        {
            EntityData.Clear();
            SynEntityData.Clear();

            var EntityKeys = API.shared.getAllEntityData(Veh.handle);
            foreach (var Key in EntityKeys)
                if (hasData(Key)) EntityData.Add(Key, API.shared.getEntityData(Veh.handle, Key));

            var SynEntityKeys = API.shared.getAllEntitySyncedData(Veh.handle);
            foreach (var SynKey in SynEntityKeys)
                if (hasSyncedData(SynKey)) SynEntityData.Add(SynKey, API.shared.getEntitySyncedData(Veh.handle, SynKey));
        }

        /// <summary>
        /// Создать все локальные и глобальные данные.
        /// </summary>
        public void setAllEntityData()
        {
            foreach (var entData in EntityData)
                SetData(entData.Key, entData.Value);

            foreach (var SynEntData in SynEntityData)
                SetSyncedData(SynEntData.Key, SynEntData.Value);

            EntityData.Clear();
            SynEntityData.Clear();
        }
    }

    /// <summary>
    /// Класс хранящий Классы автомобилей привязанных к моделе.
    /// </summary>
    public static class VehicleClasses
    {
        /// <summary>
        /// Классы автомобилей.
        /// </summary>
        public static Dictionary<string, vehClassData> Clases = new Dictionary<string, vehClassData>//топливо у авто
        {
            {"Asterope",  new vehClassData((int)vehClasses.ClassA, 100, 1000, (int)FuelTypes.petrol)},
            {"T20",  new vehClassData((int)vehClasses.ClassS, 150, 900, (int)FuelTypes.petrol)}
        };

        public struct vehClassData //Скруктура для данных об автомобиле
        {
            public int VehClass { get; set; } // класс траспорта
            public int FuelConsum { get; set; } // расход топлива
            public int FuelTank { get; set; } // размер бака топлива
            public int FuelType { get; set; } // тип топлива

            public vehClassData(int VehClass, int FuelConsum, int FuelTank, int FuelType) : this() // конструктор структуры
            {
                this.VehClass = VehClass;
                this.FuelConsum = FuelConsum;
                this.FuelTank = FuelTank;
                this.FuelType = FuelType;
            }
        }
    }

    /// <summary>
    /// Класс для работы с серверными и клиентскими объектами.
    /// </summary>
    public class ServerEntity : Script
    {
        public static ServerEntity EntityClass; // Экземпляр класса объектов исполняемого сервером.
        public List<MarkerInfo> Markers = new List<MarkerInfo>(); // массив маркеров

        public BlipInfo testblip;
        public MarkerInfo testmark;
        public TLabelInfo testlabel;

        public ServerEntity()
        {
            API.onResourceStart += onResourceStart;
        }

        #region Методы событий
        private void onResourceStart()
        {
            EntityClass = this;
        }
        #endregion     
    }

    /// <summary>
    /// Класс для создания серверных и клиентских маркеров.
    /// </summary>
    public class MarkerInfo
    {
        #region Поля класса
        public Marker marker { get; set; } //Маркер
        public CylinderColShape colshape { get; set; } // ColShape маркера, срабатывает на ивенте
        public Client player { get; set; } // Игрок, если клиентский маркер
        public int markerType = 1; // внешний вид маркера
        public Vector3 pos = new Vector3(); // позиция
        public Vector3 dir = new Vector3();  // разворот вертикальный
        public Vector3 rot = new Vector3(); // разворот горизонтальный 
        public Vector3 scale = new Vector3(); // размер
        public int alpha = 100, r = 0, g = 0, b = 0, dim = 0; // прозрачность, цвет
        public float range = 0f, height = 0f; // длина ширина
        public bool local = false; // локальный или не локальный
        public Jobs Job = Jobs.NULL; // работа к который пренадлежит маркер
        #endregion

        /// <summary>
        /// Создание серверного маркера.
        /// </summary>
        /// <param name="markerType">Тип маркера.</param>
        /// <param name="pos">Позиция.</param>
        /// <param name="dir">Разворот вертикальный.</param>
        /// <param name="rot">Разворот горизонтальный.</param>
        /// <param name="scale">Размер.</param>
        /// <param name="alpha">Прозрачность.</param>
        /// <param name="r">Цвет.</param>
        /// <param name="g">Цвет.</param>
        /// <param name="b">Цвет.</param>
        /// <param name="range">Ширина.</param>
        /// <param name="height">Длина</param>
        /// <param name="Job">Работа, к которой пренадлежит маркер.</param>
        /// <param name="dim">Дименшен.</param>
        public MarkerInfo(int markerType, Vector3 pos, Vector3 dir, Vector3 rot, Vector3 scale, int alpha, int r, int g, int b, float range, float height, Jobs Job = Jobs.NULL, int dim = 0)
        {
            this.markerType = markerType;
            this.pos = pos;
            this.dir = dir;
            this.rot = rot;
            this.scale = scale;
            this.alpha = alpha;
            this.r = r;
            this.g = g;
            this.b = b;
            local = false;
            this.Job = Job;
            this.dim = dim;
            this.range = range;
            this.height = height;
            player = null;

            marker = API.shared.createMarker(markerType, pos, dir, rot, scale, alpha, r, g, b, dim);
            colshape = API.shared.createCylinderColShape(pos, range, height);
            SetDefaultData();
            ServerEntity.EntityClass.Markers.Add(this);
        }

        /// <summary>
        /// Создание серверного колшейпа.
        /// </summary>
        /// <param name="pos">Позиция.</param>       
        /// <param name="range">Ширина.</param>
        /// <param name="height">Длина</param>
        /// <param name="Job">Работа, к которой пренадлежит маркер.</param>        
        public MarkerInfo(Vector3 pos, float range, float height, Jobs Job = Jobs.NULL)
        {
            this.pos = pos;
            local = false;
            this.Job = Job;
            this.range = range;
            this.height = height;
            player = null;

            colshape = API.shared.createCylinderColShape(pos, range, height);
            SetDefaultData();
            ServerEntity.EntityClass.Markers.Add(this);
        }

        /// <summary>
        /// Создание клиентского маркера.
        /// </summary>
        /// <param name="player">Игрок, у которого создается маркер.</param>
        /// <param name="markerType">Тип маркера.</param>
        /// <param name="pos">Позиция.</param>
        /// <param name="dir">Разворот вертикальный.</param>
        /// <param name="rot">Разворот горизонтальный.</param>
        /// <param name="scale">Размер.</param>
        /// <param name="alpha">Прозрачность.</param>
        /// <param name="r">Цвет.</param>
        /// <param name="g">Цвет.</param>
        /// <param name="b">Цвет.</param>
        /// <param name="range">Ширина.</param>
        /// <param name="height">Длина</param>
        /// <param name="Job">Работа, к которой пренадлежит маркер.</param>
        /// <param name="dim">Дименшен.</param>
        public MarkerInfo(Client player, int markerType, Vector3 pos, Vector3 dir, Vector3 rot, Vector3 scale, int alpha, int r, int g, int b, float range, float height, Jobs Job = Jobs.NULL, int dim = 0)
        {
            if (API.shared.hasEntityData(player.handle, Constants.PlayerAccount))
            {
                PlayerInfo pInfo = API.shared.getEntityData(player.handle, Constants.PlayerAccount);
                this.markerType = markerType;
                this.pos = pos;
                this.dir = dir;
                this.rot = rot;
                this.scale = scale;
                this.alpha = alpha;
                this.r = r;
                this.g = g;
                this.b = b;
                local = true;
                this.Job = Job;
                this.dim = dim;
                this.range = range;
                this.height = height;
                this.player = player;

                API.shared.triggerClientEvent(player, Constants.CreateEntity, Constants.Marker, markerType, pos, dir, rot, scale, r, g, b, alpha);
                colshape = API.shared.createCylinderColShape(pos, range, height);
                pInfo.playerEntity.Add(new PlayerInfo.PlEnt { Key = Constants.Marker, Value = this });
                SetDefaultData();
                ServerEntity.EntityClass.Markers.Add(this);
            }
        }


        /// <summary>
        /// Создание клиентского колшейпа.
        /// </summary>
        /// <param name="player">Игрок, у которого создается маркер.</param>
        /// <param name="pos">Позиция.</param>       
        /// <param name="range">Ширина.</param>
        /// <param name="height">Длина</param>
        /// <param name="Job">Работа, к которой пренадлежит маркер.</param>
        /// <param name="dim">Дименшен.</param>
        public MarkerInfo(Client player, Vector3 pos, float range, float height, Jobs Job = Jobs.NULL)
        {
            if (API.shared.hasEntityData(player.handle, Constants.PlayerAccount))
            {
                PlayerInfo pInfo = API.shared.getEntityData(player.handle, Constants.PlayerAccount);
                this.pos = pos;
                local = true;
                this.Job = Job;
                this.range = range;
                this.height = height;
                this.player = player;
                colshape = API.shared.createCylinderColShape(pos, range, height);
                pInfo.playerEntity.Add(new PlayerInfo.PlEnt { Key = Constants.Marker, Value = this });
                SetDefaultData();
                ServerEntity.EntityClass.Markers.Add(this);
            }
        }

        /// <summary>
        /// Удаление маркера.
        /// </summary>
        public void DeleteMarker()
        {
            if (local)
            {
                if (API.shared.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    PlayerInfo pInfo = API.shared.getEntityData(player.handle, Constants.PlayerAccount);
                    API.shared.triggerClientEvent(player, Constants.DeleteEntity, Constants.Marker, pos);
                    ServerEntity.EntityClass.Markers.Remove(this);
                    API.shared.deleteColShape(colshape);
                    pInfo.playerEntity.Remove(new PlayerInfo.PlEnt { Key = Constants.Marker, Value = this });
                }
            }
            else
            {
                ServerEntity.EntityClass.Markers.Remove(this);
                API.shared.deleteEntity(marker);
                API.shared.deleteColShape(colshape);
            }
        }

        /// <summary>
        /// Создание обязательных данных.
        /// </summary>
        private void SetDefaultData()
        {
            SetData(Constants.MarkerClass, this);
            SetData(Constants.MarkerJob, (int)Job);
        }

        /// <summary>
        /// Удаление обязательных данных.
        /// </summary>
        public void ResetDefaultData()
        {
            ResetData(Constants.MarkerClass);
            ResetData(Constants.MarkerJob);
        }

        /// <summary>
        /// Создание локальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <param name="value">Значение.</param>
        public void SetData(string key, object value)
        {
            colshape.setData(key, value);
        }

        /// <summary>
        /// Проверка на наличие локальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <returns>True - eсли существует поле, False - если нет.</returns>
        public bool HasData(string key)
        {
            return colshape.hasData(key);
        }

        /// <summary>
        /// Получение локальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <returns>Значение.</returns>
        public dynamic GetData(string key)
        {
            if (HasData(key)) return colshape.getData(key);
            else return null;
        }

        /// <summary>
        /// Удаление данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        public void ResetData(string key)
        {
            if (HasData(key)) colshape.resetData(key);
        }
    }

    /// <summary>
    /// Класс для создания Локальных и Серверных блипов.
    /// </summary>
    public class BlipInfo
    {
        #region Поля класса
        public Blip blip { get; set; } // блип
        public Client player { get; set; } // Игрок, для которого создан клиентский блип
        public Vector3 pos = new Vector3(); // позиция блипа
        public int Sprite = 0; //спрайт
        public Jobs Job = Jobs.NULL; // работа к который пренадлежит блип
        public int Color = 0; // цвет
        public bool ShortRange = true; // скрывать если далеко
        public bool local = false; // локальный или серверный
        public int dim = 0; // дименшен
        #endregion

        /// <summary>
        /// Создание серверного блипа.
        /// </summary>
        /// <param name="pos">Координаты.</param>
        /// <param name="Sprite">Спрайт.</param>
        /// <param name="Color">Цвет.</param>
        /// <param name="ShortRange">Скрывать если далеко от игрока.</param>
        /// <param name="dim">Дименшен.</param>
        /// <param name="name">Название.</param>
        /// <param name="Job">Связанная работа.</param>
        public BlipInfo(Vector3 pos, int Sprite = 1, int Color = 4, bool ShortRange = true, int dim = 0, string name = null, Jobs Job = Jobs.NULL, bool ShowRoute = false, int RoutColor = 60)
        {
            this.pos = pos;
            this.Sprite = Sprite;
            this.Color = Color;
            this.ShortRange = ShortRange;
            local = false;
            this.dim = dim;
            player = null;
            this.Job = Job;
            blip = API.shared.createBlip(pos);
            blip.sprite = Sprite;
            blip.color = Color;
            blip.shortRange = ShortRange;
            blip.dimension = dim;
            API.shared.setBlipRouteVisible(blip, ShowRoute);
            API.shared.setBlipRouteColor(blip, RoutColor);
            API.shared.setBlipName(blip, name);
            SetDefaultData();
        }

        /// <summary>
        /// Создание клиентского блипа.
        /// </summary>
        /// <param name="player">Игрок, у которого он будет создан.</param>
        /// <param name="pos">Координаты.</param>
        /// <param name="Sprite">Спрайт.</param>
        /// <param name="Color">Цвет.</param>
        /// <param name="ShortRange">Скрывать если далеко от игрока.</param>
        /// <param name="dim">Дименшен.</param>
        /// <param name="name">Имя.</param>
        /// <param name="name">Название.</param>
        /// <param name="Job">Связанная работа.</param>
        public BlipInfo(Client player, Vector3 pos, int Sprite = 1, int Color = 4, bool ShortRange = true, int dim = 0, string name = null, Jobs Job = Jobs.NULL, bool ShowRoute = false, int RoutColor = 60)
        {
            if (API.shared.hasEntityData(player.handle, Constants.PlayerAccount))
            {
                PlayerInfo pInfo = API.shared.getEntityData(player.handle, Constants.PlayerAccount);
                this.pos = pos;
                this.Sprite = Sprite;
                this.Color = Color;
                this.ShortRange = ShortRange;
                local = true;
                this.dim = dim;
                this.Job = Job;
                this.player = player;
                pInfo.playerEntity.Add(new PlayerInfo.PlEnt { Key = Constants.Blip, Value = this });
                API.shared.triggerClientEvent(player, Constants.CreateEntity, Constants.Blip, pos, Sprite, Color, ShortRange, name, ShowRoute, RoutColor);
            }
        }

        /// <summary>
        /// Удаление блипа.
        /// </summary>
        public void DeleteBlip()
        {
            if (local)
            {
                if (API.shared.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    PlayerInfo pInfo = API.shared.getEntityData(player.handle, Constants.PlayerAccount);
                    API.shared.triggerClientEvent(player, Constants.DeleteEntity, Constants.Blip, pos);
                    pInfo.playerEntity.Remove(new PlayerInfo.PlEnt { Key = Constants.Blip, Value = this });
                }
            }
            else API.shared.deleteEntity(blip);
        }

        /// <summary>
        /// Создание обязательных данных.
        /// </summary>
        private void SetDefaultData()
        {
            SetData(Constants.BlipClass, this);
        }

        /// <summary>
        /// Удаление обязательных данных.
        /// </summary>
        public void ResetDefaultData()
        {
            ResetData(Constants.BlipClass);
        }

        /// <summary>
        /// Создание локальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <param name="value">Значение.</param>
        public void SetData(string key, object value)
        {
            API.shared.setEntityData(blip.handle, key, value);
        }

        /// <summary>
        /// Создание глобальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <param name="value">Значение.</param>
        public void SetSyncedData(string key, object value)
        {
            API.shared.setEntitySyncedData(blip.handle, key, value);
        }

        /// <summary>
        /// Проверка на наличие данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <returns>True - если поле существует, False - если нет.</returns>
        public bool HasData(string key)
        {
            return API.shared.hasEntityData(blip.handle, key);
        }

        /// <summary>
        /// Проверка на наличие глобальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <returns>True - если поле существует, False - если нет.</returns>
        public bool HasSyncedData(string key)
        {
            return API.shared.hasEntitySyncedData(blip.handle, key);
        }

        /// <summary>
        /// Удаление локальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        public void ResetData(string key)
        {
            if (HasData(key)) API.shared.resetEntityData(blip.handle, key);
        }

        /// <summary>
        /// Удаление глобальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        public void ResetSyncedData(string key)
        {
            if (HasSyncedData(key)) API.shared.resetEntitySyncedData(blip.handle, key);
        }

        /// <summary>
        /// Получение глобальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <returns>Значение.</returns>
        public dynamic GetData(string key)
        {
            if (HasData(key)) return API.shared.getEntityData(blip.handle, key);
            else return null;
        }

        /// <summary>
        /// Получение локальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <returns>Значение.</returns>
        public dynamic GetSyncedData(string key)
        {
            if (HasSyncedData(key)) return API.shared.getEntitySyncedData(blip.handle, key);
            else return null;
        }
    }

    /// <summary>
    /// Класс для работы над серверным и клиентским текстлейблом.
    /// </summary>
    public class TLabelInfo
    {
        #region Поля класса
        public TextLabel tLabel { get; set; } // текстлейбл
        public Client player { get; set; } // игрок, у которого создан тейкслейбл если он клиентский
        public string text = ""; // текст
        public Vector3 pos = new Vector3(); // позиция
        public float range = 0f, size = 0f; // дистанция, размер шрифта
        public bool seethrough = false, local = false; // прозрачный, серверный или клиентский
        public int dim = 0; // дименшен
        public Jobs Job = Jobs.NULL;
        #endregion

        /// <summary>
        /// Создание серверного текстлейбла.
        /// </summary>
        /// <param name="text">Текст.</param>
        /// <param name="pos">Позиция.</param>
        /// <param name="range">Дистанция.</param>
        /// <param name="size">Размер шрифта.</param>
        /// <param name="seethrough">Прозрачность.</param>
        /// <param name="local">серверный или клиентский.</param>
        /// <param name="dim">Дименшен.</param>
        public TLabelInfo(string text, Vector3 pos, float range, float size, bool seethrough = false, int dim = 0, Jobs Job = Jobs.NULL)
        {
            this.text = text;
            this.pos = pos;
            this.range = range;
            this.size = size;
            this.seethrough = seethrough;
            this.local = false;
            this.dim = dim;
            player = null;
            this.Job = Job;
            tLabel = API.shared.createTextLabel(text, pos, range, size, seethrough, dim);
            SetDefaultData();
        }

        /// <summary>
        /// Создание клиентского текстлейбла.
        /// </summary>
        /// <param name="player">игрок, у которого создан тейкслейбл если он клиентский</param>
        /// <param name="text">Текст.</param>
        /// <param name="pos">Позиция.</param>
        /// <param name="range">Дистанция.</param>
        /// <param name="size">Размер шрифта.</param>
        /// <param name="seethrough">Прозрачность.</param>
        /// <param name="local">серверный или клиентский.</param>
        /// <param name="dim">Дименшен.</param>
        public TLabelInfo(Client player, string text, Vector3 pos, float range, float size, bool seethrough = false, int dim = 0, Jobs Job = Jobs.NULL)
        {
            if (API.shared.hasEntityData(player.handle, Constants.PlayerAccount))
            {
                PlayerInfo pInfo = API.shared.getEntityData(player.handle, Constants.PlayerAccount);
                this.text = text;
                this.pos = pos;
                this.range = range;
                this.size = size;
                this.seethrough = seethrough;
                this.local = true;
                this.dim = dim;
                this.player = player;
                this.Job = Job;
                pInfo.playerEntity.Add(new PlayerInfo.PlEnt { Key = Constants.tLabel, Value = this });
                API.shared.triggerClientEvent(player, Constants.CreateEntity, Constants.tLabel, text, pos, range, size, seethrough);
            }
        }

        /// <summary>
        /// Удаление текстлейбла.
        /// </summary>
        public void DeleteTextLabel()
        {
            if (local)
            {
                if (API.shared.hasEntityData(player.handle, Constants.PlayerAccount))
                {
                    PlayerInfo pInfo = API.shared.getEntityData(player.handle, Constants.PlayerAccount);
                    API.shared.triggerClientEvent(player, Constants.DeleteEntity, Constants.tLabel, pos);
                    pInfo.playerEntity.Remove(new PlayerInfo.PlEnt { Key = Constants.tLabel, Value = this });
                }
            }
            else API.shared.deleteEntity(tLabel);
        }

        /// <summary>
        /// Создание обязательных данных.
        /// </summary>
        private void SetDefaultData()
        {
            SetData(Constants.tLabelClass, this);
        }

        /// <summary>
        /// Удаление обязательных данных.
        /// </summary>
        public void ResetDefaultData()
        {
            ResetData(Constants.tLabelClass);
        }

        /// <summary>
        /// Создание локальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <param name="value">Значение.</param>
        public void SetData(string key, object value)
        {
            API.shared.setEntityData(tLabel.handle, key, value);
        }

        /// <summary>
        /// Создание глобальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <param name="value">Значение.</param>
        public void SetSyncedData(string key, object value)
        {
            API.shared.setEntitySyncedData(tLabel.handle, key, value);
        }

        /// <summary>
        /// Проверка на наличие данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <returns>True - если поле существует, False - если нет.</returns>
        public bool HasData(string key)
        {
            return API.shared.hasEntityData(tLabel.handle, key);
        }

        /// <summary>
        /// Проверка на наличие глобальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <returns>True - если поле существует, False - если нет.</returns>
        public bool HasSyncedData(string key)
        {
            return API.shared.hasEntitySyncedData(tLabel.handle, key);
        }

        /// <summary>
        /// Удаление локальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        public void ResetData(string key)
        {
            if (HasData(key)) API.shared.resetEntityData(tLabel.handle, key);
        }

        /// <summary>
        /// Удаление глобальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        public void ResetSyncedData(string key)
        {
            if (HasSyncedData(key)) API.shared.resetEntitySyncedData(tLabel.handle, key);
        }

        /// <summary>
        /// Получение локальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <returns>Значение.</returns>
        public dynamic GetData(string key)
        {
            if (HasData(key)) return API.shared.getEntityData(tLabel.handle, key);
            else return null;
        }

        /// <summary>
        /// Получение глобальных данных.
        /// </summary>
        /// <param name="key">Имя поля.</param>
        /// <returns>Значение.</returns>
        public dynamic GetSyncedData(string key)
        {
            if (HasSyncedData(key)) return API.shared.getEntitySyncedData(tLabel.handle, key);
            else return null;
        }
    }

    /// <summary>
    /// Класс обеспечивающий серверный waypoint
    /// </summary>
    public class ServerWayPoint : Script
    {
        public ServerWayPoint()
        {
        }

        /// <summary>
        /// Создание waypoint у игрока.
        /// </summary>
        /// <param name="player">Игрок.</param>
        /// <param name="x">Координата Х.</param>
        /// <param name="y">Координата Y.</param>
        public void CreateWayPoint(Client player, double x, double y)
        {
            API.shared.triggerClientEvent(player, "CREATE_ENTITY", "WAYPOINT", x, y);
        }

        /// <summary>
        /// Удаление waypoint у игрока.
        /// </summary>
        /// <param name="player">Игрок.</param>
        public void DeleteWayPoint(Client player)
        {
            API.shared.triggerClientEvent(player, "DELETE_ENTITY", "WAYPOINT");
        }
    }

    /// <summary>
    /// Класс для работы с СУБД.
    /// </summary>
    public class SqlConnecter
    {
        private MySqlConnectionStringBuilder mysqlCSB = new MySqlConnectionStringBuilder(); //сборка для подключения

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="Server">IP сервера.</param>
        /// <param name="Database">Имя базыданных.</param>
        /// <param name="UserID">Имя пользователя.</param>
        /// <param name="Password">Пароль.</param>
        public SqlConnecter(string Server, string Database, string UserID, string Password)
        {
            mysqlCSB = new MySqlConnectionStringBuilder(string.Format("database={0};uid={1};server={2};password={3}", Database, UserID, Server, Password));
        }

        /// <summary>
        /// Проверка подключения к СУБД.
        /// </summary>
        /// <returns>Удалось ли подключиться.</returns>
        public bool checkConnect()
        {
            if (mysqlCSB == null) throw new ArgumentException("Не была объявлена сборка для SQL подключения (SqlConnecter.createConBuilder).", "mysqlCSB");

            string queryString = string.Format(@"SELECT 1");

            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = mysqlCSB.ConnectionString;
                MySqlCommand com = new MySqlCommand(queryString, con);
                try
                {
                    con.Open();
                    con.Close();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Отправить запрос в СУБД.
        /// </summary>
        /// <param name="queryString">Строка запроса.</param>
        /// <returns>Данные из СУБД.</returns>
        public DataTable retSQLData(string queryString)
        {
            if (mysqlCSB == null) throw new ArgumentException("Не была объявлена сборка для SQL подключения.(SqlConnecter.createConBuilder)", "mysqlCSB");

            DataTable dt = new DataTable();
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = mysqlCSB.ConnectionString;
                MySqlCommand com = new MySqlCommand(queryString, con);
                con.Open();
                using (MySqlDataReader dr = com.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        dt.Load(dr);
                    }
                }
            }
            return dt;
        }
    }

    /// <summary>
    /// Класс с константами.
    /// </summary>
    public class Constants
    {
        #region Глобальные
        /// <summary>
        /// Глобаная константа.
        /// </summary>
        public const string Global = "GLOBAL";

        /// <summary>
        /// Запрос на отображение списка игроков.
        /// </summary>
        public const string showPlayerList = "SHOW_PLAYER_LIST";

        /// <summary>
        /// Сообщение о подозрении в читерстве.
        /// </summary>
        public const string KickCheatMessage = "Подозрение в читерстве.";

        /// <summary>
        /// Сообщение об ошибке авторизации.
        /// </summary>
        public const string errorAutor = "~y~[Server] ~w~Ошибка: Необходимо авторизироваться.";

        /// <summary>
        /// Сообщение об ошибке отказа доступа.
        /// </summary>
        public const string NonAdminAccessDenied = "~y~[Server] ~w~Ошибка: Доступно только для администрации.";

        /// <summary>
        /// Сообщение об ошибке при неавторизированной игроке - цели комманды.
        /// </summary>
        public const string targetNotLogin = "~y~[Server] ~w~Ошибка: Игрок - цель не авторизирован.";

        /// <summary>
        /// Сообщение об ошибке при отправке самому себе.
        /// </summary>
        public const string yourselfMessage = "~y~[Server] ~w~Ошибка: Нельзя писать самому себе.";

        /// <summary>
        /// Сообщение об ошибке попытки кикнуть/ забанить администрацию.
        /// </summary>
        public const string AdmProtect = "~y~[Server] ~w~Ошибка: Игрок - цель администратор.";

        /// <summary>
        /// Сообщение об ошибке не найден по имени.
        /// </summary>
        public const string NotFindName = "~y~[Server] ~w~Ошибка: Игрок с таким именем не найден.";

        /// <summary>
        /// Сообщение об ошибке не найден по ID.
        /// </summary>
        public const string NotFindID = "~y~[Server] ~w~Ошибка: Игрок с таким ID не найден.";

        /// <summary>
        /// Файл для лога админов.
        /// </summary>
        public const string LogAdmFile = "LogAdm.txt";
        #endregion

        #region Связанные с игроком
        /// <summary>
        /// Отправка репорта из меню.
        /// </summary>
        public const string mainMenuReport = "MAINMENU_REPORT";

        /// <summary>
        /// Показать страницу авторизации.
        /// </summary>
        public const string LogPage = "SHOW_LOGIN_PAGE";

        /// <summary>
        /// Показать страницу регистрации.
        /// </summary>
        public const string RegPage = "SHOW_REGISTER_PAGE";

        /// <summary>
        /// Скрыть страницу CEF.
        /// </summary>
        public const string closePage = "CLOSE_PAGE";

        /// <summary>
        /// Клиент регистрируется.
        /// </summary>
        public const string Register = "PLAYER_REGISTER";

        /// <summary>
        /// Клиент авторизируется.
        /// </summary>
        public const string Logining = "PLAYER_LOGIN";

        /// <summary>
        /// Отобразить ошибку на странице CEF.
        /// </summary>
        public const string showError = "SHOW_ERROR_MESSAGE";

        /// <summary>
        /// Сброс данных лица персонажа.
        /// </summary>
        public const string ResetFaceData = "PLAYER_RESET_FACE_DATA";

        /// <summary>
        /// Сброс данных лица персонажа выполненно.
        /// </summary>
        public const string ResetFaceDataComplete = "PLAYER_RESET_FACE_DATA_COMPLETE";

        /// <summary>
        /// Завершение создания лица персонажа.
        /// </summary>
        public const string CompleteFaceCreate = "PLAYER_COMPLETE_FACE_CREATE";

        /// <summary>
        /// Отобразить список персонажей.
        /// </summary>
        public const string ShowCharList = "SHOW_CHARACTER_LIST";

        /// <summary>
        /// Начать играть за персонажа.
        /// </summary>
        public const string CharStartPlay = "CHARLIST_START_PLAY";

        /// <summary>
        /// Удалить персонажа.
        /// </summary>
        public const string RemoveChar = "CHARLIST_REMOVE_CHAR";

        /// <summary>
        /// Создание нового персонажа.
        /// </summary>
        public const string CreateNewChar = "CHARLIST_CREATE_NEW";

        /// <summary>
        /// Имя персонажа занято.
        /// </summary>
        public const string BusyNameChar = "CHARLIST_BUSY_NAME";

        /// <summary>
        /// Обновить персонажа.
        /// </summary>
        public const string UpdateChar = "UPDATE_CHARACTER";

        /// <summary>
        /// Открыть меню создания персонажа.
        /// </summary>
        public const string CharMenuOpen = "CHARACTER_MENU_OPEN";

        /// <summary>
        /// Клиент.
        /// </summary>
        public const string Player = "PLAYER";

        /// <summary>
        /// Аккаунт клиента.
        /// </summary>
        public const string PlayerAccount = "PLAYER_ACCOUNT";

        /// <summary>
        /// Имя клиента.
        /// </summary>
        public const string PlayerName = "PLAYER_NAME";

        /// <summary>
        /// Тэг клиента.
        /// </summary>
        public const string PlayerTag = "PLAYER_TAG";

        /// <summary>
        /// Блип клиента.
        /// </summary>
        public const string PlayerBlip = "PLAYER_BLIP";

        /// <summary>
        /// Авторизирован клиент.
        /// </summary>
        public const string PlayerIsLogin = "PLAYER_ISLOGIN";

        /// <summary>
        /// Фракция клиента.
        /// </summary>
        public const string PlayerFact = "PLAYER_FACTION";

        /// <summary>
        /// Админ клиента.
        /// </summary>
        public const string PlayerAdmin = "PLAYER_ADMIN";

        /// <summary>
        /// Деньги клиента.
        /// </summary>
        public const string PlayerMoney = "PLAYER_MONEY";

        /// <summary>
        /// Деньги на счету клиента.
        /// </summary>
        public const string PlayerBankMoney = "PLAYER_BANKMONEY";

        /// <summary>
        /// Уровень клиента.
        /// </summary>
        public const string PlayerLevel = "PLAYER_LEVEL";

        /// <summary>
        /// Опыт клиента.
        /// </summary>
        public const string PlayerExp = "PLAYER_EXP";

        /// <summary>
        /// Ранг фракции клиента.
        /// </summary>
        public const string PlayerFactRank = "PLAYER_FACTRANK";

        /// <summary>
        /// Работа клиента.
        /// </summary>
        public const string PlayerJob = "PLAYER_JOB";

        /// <summary>
        /// Имеются данные о лице персонажа.
        /// </summary>
        public const string PlayerFaceHasCharacterData = "PLAYER_FACE_HAS_CHARACTER_DATA";

        /// <summary>
        /// Первая форма лица.
        /// </summary>
        public const string PlayerFaceShapeFirstID = "PLAYER_FACE_SHAPE_FIRST_ID";

        /// <summary>
        /// Вторая форма лица.
        /// </summary>
        public const string PlayerFaceShapeSecondID = "PLAYER_FACE_SHAPE_SECOND_ID";

        /// <summary>
        /// Первый скин.
        /// </summary>
        public const string PlayerFaceSkinFirstID = "PLAYER_FACE_SKIN_FIRST_ID";

        /// <summary>
        /// Второй скин.
        /// </summary>
        public const string PlayerFaceSkinSecondID = "PLAYER_FACE_SKIN_SECOND_ID";

        /// <summary>
        /// Смешанная форма лица.
        /// </summary>
        public const string PlayerFaceShapeMix = "PLAYER_FACE_SHAPE_MIX";

        /// <summary>
        /// Смешанный скин.
        /// </summary>
        public const string PlayerFaceSkinMix = "PLAYER_FACE_SKIN_MIX";

        /// <summary>
        /// Основной цвет волос.
        /// </summary>
        public const string PlayerFaceHairColor = "PLAYER_FACE_HAIR_COLOR";

        /// <summary>
        /// Дополнительный цвет волос.
        /// </summary>
        public const string PlayerFaceHairHighlightColor = "PLAYER_FACE_HAIR_HIGHLIGHT_COLOR";

        /// <summary>
        /// Цвет глаз.
        /// </summary>
        public const string PlayerFaceEyeColor = "PLAYER_FACE_EYE_COLOR";

        /// <summary>
        /// Брови персонажа.
        /// </summary>
        public const string PlayerFaceEyebrows = "PLAYER_FACE_EYEBROWS";

        /// <summary>
        /// Цвет бровей персонажа.
        /// </summary>
        public const string PlayerFaceEyebrowsColor = "PLAYER_FACE_EYEBROWS_COLOR";

        /// <summary>
        /// Дополнительный цвет бровей персонажа.
        /// </summary>
        public const string PlayerFaceEyebrowsColor2 = "PLAYER_FACE_EYEBROWS_COLOR2";

        /// <summary>
        /// Макияж персонажа.
        /// </summary>
        public const string PlayerFaceMakeup = "PLAYER_FACE_MAKEUP";

        /// <summary>
        /// Цвет макияжа персонажа.
        /// </summary>
        public const string PlayerFaceMakeupColor = "PLAYER_FACE_MAKEUP_COLOR";

        /// <summary>
        /// Дополнительный цвет макияж персонажа.
        /// </summary>
        public const string PlayerFaceMakeupColor2 = "PLAYER_FACE_MAKEUP_COLOR2";

        /// <summary>
        /// Губы персонажа.
        /// </summary>
        public const string PlayerFaceLipstick = "PLAYER_FACE_LIPSTICK";

        /// <summary>
        /// Цвет губ персонажа.
        /// </summary>
        public const string PlayerFaceLipstickColor = "PLAYER_FACE_LIPSTICK_COLOR";

        /// <summary>
        /// Дополнительный цвет губ персонажа.
        /// </summary>
        public const string PlayerFaceLipstickColor2 = "PLAYER_FACE_LIPSTICK_COLOR2";

        /// <summary>
        /// Раширенные настройки персонажа.
        /// </summary>
        public const string PlayerFaceFeaturesList = "PLAYER_FACE_FEATURES_LIST";

        /// <summary>
        /// Одежда персонажа.
        /// </summary>
        public const string PlayerFaceClosesList = "PLAYER_FACE_CLOSES_LIST";

        #endregion

        #region Связанные с транспортом
        /// <summary>
        /// Конст. транспорта.
        /// </summary>
        public const string Veh = "VEHICLE";

        /// <summary>
        /// Переколючение двигателя.
        /// </summary>
        public const string VehEngineToggle = "VEHICLE_ENGINE_TOGGLE";

        /// <summary>
        /// Вызвать класс связанный с транспортом.
        /// </summary>
        public const string VehClass = "VEHICLE_CLASS";

        /// <summary>
        /// Вызвать id работы к которой принадлежит транспорт.
        /// </summary>
        public const string VehJob = "VEHICLE_JOB";

        /// <summary>
        /// Вызвать данные о топлеве транспорта.
        /// </summary>
        public const string VehFuel = "VEHICLE_FUEL";
        #endregion

        #region Связанные с объектами
        /// <summary>
        /// Создание локальных объектов.
        /// </summary>
        public const string CreateEntity = "CREATE_ENTITY";

        /// <summary>
        /// Удаление локальных объектов.
        /// </summary>
        public const string DeleteEntity = "DELETE_ENTITY";
        #endregion

        #region Связанные с маркерами
        /// <summary>
        /// Конст. маркеров.
        /// </summary>
        public const string Marker = "MARKER";

        /// <summary>
        /// Вызвать класс связанный с маркером.
        /// </summary>
        public const string MarkerClass = "MARKER_CLASS";

        /// <summary>
        /// Работа связанная с маркером.
        /// </summary>
        public const string MarkerJob = "MARKER_JOB";
        #endregion

        #region Связанные с блипами
        /// <summary>
        /// Конст. блипа.
        /// </summary>
        public const string Blip = "BLIP";

        /// <summary>
        /// Вызвать класс связанный с блипом.
        /// </summary>
        public const string BlipClass = "BLIP_CLASS";
        #endregion

        #region Связанные с текстЛейблами
        /// <summary>
        /// Конст. текстЛейбла.
        /// </summary>
        public const string tLabel = "TEXT";

        /// <summary>
        /// Вызвать класс связанный с текстЛейблом.
        /// </summary>
        public const string tLabelClass = "TEXT_CLASS";
        #endregion
    }

    public class Playerlist : Script
    {
        private DateTime m_lastTick = DateTime.Now;

        public Playerlist()
        {
            if (API.isResourceRunning("colorednames"))
            {
                API.exported.colorednames.onGotColoredName += new ExportedEvent(ColoredNames_onGotColoredName);
            }
            else
            {
                API.onPlayerConnected += API_onPlayerConnected;
            }
            API.onPlayerDisconnected += API_onPlayerDisconnected;
            API.onPlayerFinishedDownload += API_onPlayerFinishedDownload;

            API.onClientEventTrigger += API_onClientEventTrigger;

            API.onUpdate += API_onUpdate;
        }

        private void API_onPlayerConnected(Client player)
        {
            API.triggerClientEventForAll("playerlist_join", player.socialClubName, player.name, ColorForPlayer(player));
        }

        private void API_onPlayerDisconnected(Client player, string reason)
        {
            API.triggerClientEventForAll("playerlist_leave", player.socialClubName);
        }

        private string ColorForPlayer(Client player)
        {
            if (!API.isResourceRunning("colorednames"))
            {
                return "FFFFFF";
            }
            string ret = player.getData("PROFILE_color");
            if (ret == null)
            {
                return "FFFFFF";
            }
            return ret;
        }

        private void API_onPlayerFinishedDownload(Client player)
        {
            var players = API.getAllPlayers();
            var list = new List<string>();
            foreach (var ply in players)
            {
                var dic = new Dictionary<string, object>();
                dic["socialClubName"] = ply.socialClubName;
                dic["name"] = ply.name;
                dic["ping"] = ply.ping;
                dic["color"] = ColorForPlayer(ply);
                list.Add(API.toJson(dic));
            }

            API.triggerClientEvent(player, "playerlist", list);
        }

        private void ColoredNames_onGotColoredName(object[] args)
        {
            var client = (Client)args[0];
            API.triggerClientEventForAll("playerlist_join", client.socialClubName, client.name, ColorForPlayer(client));
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "playerlist_pings")
            {
                var players = API.getAllPlayers();
                var list = new List<string>();
                foreach (var ply in players)
                {
                    var dic = new Dictionary<string, object>();
                    dic["socialClubName"] = ply.socialClubName;
                    dic["ping"] = ply.ping;
                    list.Add(API.toJson(dic));
                }
                API.triggerClientEvent(sender, "playerlist_pings", list);
            }
        }

        private void API_onUpdate()
        {
            if ((DateTime.Now - m_lastTick).TotalMilliseconds >= 1000)
            {
                m_lastTick = DateTime.Now;

                var changedNames = new List<string>();
                var players = API.getAllPlayers();
                foreach (var player in players)
                {
                    string lastName = player.getData("playerlist_lastname");

                    if (lastName == null)
                    {
                        player.setData("playerlist_lastname", player.name);
                        continue;
                    }

                    if (lastName != player.name)
                    {
                        player.setData("playerlist_lastname", player.name);

                        var dic = new Dictionary<string, object>();
                        dic["socialClubName"] = player.socialClubName;
                        dic["newName"] = player.name;
                        changedNames.Add(API.toJson(dic));
                    }
                }

                if (changedNames.Count > 0)
                {
                    API.triggerClientEventForAll("playerlist_changednames", changedNames);
                }
            }
        }
    }

    #region enums
    [Flags]
    public enum AnimationFlags
    {
        Loop = 1 << 0,
        StopOnLastFrame = 1 << 1,
        OnlyAnimateUpperBody = 1 << 4,
        AllowPlayerControl = 1 << 5,
        Cancellable = 1 << 7
    }

    /// <summary>
    /// Список работ.
    /// </summary>
    public enum Jobs
    {
        NULL, //безработный
        ADMIN, //Админ
        JOB_IMMIGRANT, // Работа иммигранта
        TRUCK_DRIVER, //Водитель фур
        TRUCK_DRIVER_PRO, //Водитель фур профи
        TRUCK_DRIVER_MASTER, //Водитель фур мастер
        JOB_TAXI_PARK_ONE, //Водитель такси 1
        JOB_TAXI_PARK_TWO, //Водитель такси 2
        JOB_TAXI_PARK_THREE, //Водитель такси 3
        JOB_JOBGRYZ_ONE, //Работа грузчика 1
        JOB_BUS, // Работа водителя
        JOB_BUS_ONE, // Работа водителя автобуса 1
        JOB_BUS_TWO, // Работа водителя автобуса 2
        JOB_BUS_THREE, // Работа водителя автобуса 3
        JOB_BUS_FOUR, // Работа водителя автобуса 4
        JOB_BUS_FIVE, // Работа водителя автобуса 5
        JOB_BUS_SIX, // Работа водителя автобуса 6
        JOB_BUS_SEVEN // Работа водителя автобуса 7
    }

    /// <summary>
    /// Список рангов администрации.
    /// </summary>
    public enum AdmRank
    {
        Игрок,
        Помощник,
        Админ,
        стАдмин,
        Разработчик,
        стРазработчик
    }

    /// <summary>
    /// Список типов бана.
    /// </summary>
    public enum BanTypes
    {
        NULL,
        Сутки,
        Месяц,
        Навсегда
    }

    /// <summary>
    /// Список типов топлива
    /// </summary>
    public enum FuelTypes
    {
        petrol,
        gas,
        diesel,
        kerosene,
        elect
    }

    /// <summary>
    /// Список типов машин
    /// </summary>
    public enum vehClasses
    {
        ClassA,
        ClassB,
        ClassC,
        ClassD,
        ClassE,
        ClassF,
        ClassM,
        ClassS,
        Classj,
        Pickup
    }
    #endregion
    
    public sealed class Global
    {
        public static object Lock = new object();
        private Global() { }
    }
}
