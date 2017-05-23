var mainMenu = null;
var SubmainMenu = null;
var pool = null;
var menuOpen = null;
var jobstrings = ["Безработный", "Админ", "Водитель Автобуса", "Дальнобойщик", "Дальнобойщик", "Дальнобойщик", "Водитель Такси", "Водитель Такси", "Водитель Такси"];

API.onResourceStop.connect(function () {
    RemoveAllDate();
});

API.onKeyDown.connect(function (sender, e) {
    if (e.KeyCode == Keys.M && !API.isChatOpen() && menuOpen == null) {
        if (SecurityCheck()) {
            pool = API.getMenuPool();
            CreateMainMenu();
            CreateSubMainMenu();
            OpenMainMenu();
            menuOpen = true;
        }
    }
});

API.onUpdate.connect(function () {
    if (pool != null) {
        pool.ProcessMenus();
    }
});

function CreateMainMenu() {
    var res = API.getScreenResolution();
    mainMenu = API.createMenu("Осн. Меню", "Выберите пункт.", res.Width / 2 - 200, res.Height / 2 - 180, 0);

    mainMenu.AddItem(API.createMenuItem("Статистика >>", "Статистика персонажа."));
    mainMenu.AddItem(API.createMenuItem("Репорт >>", "Сообщение для админов."));
    mainMenu.AddItem(API.createMenuItem("Помощь >>", "Информация для новичков."));
    mainMenu.AddItem(API.createMenuItem("Настройки >>", ""));
    mainMenu.AddItem(API.createMenuItem("<< Закрыть >>", "Закрыть меню."));

    mainMenu.RefreshIndex();
    pool.Add(mainMenu);
    mainMenu.Visible = false;
    mainMenu.MenuItems[2].Enabled = false;
    mainMenu.MenuItems[3].Enabled = false;

    mainMenu.OnItemSelect.connect(function (sender, item, index) {
        switch (index) {
            case 0:
                OpenSubMainMenu();
                break;
            case 1:
                mainMenu.Visible = false;
                CreateSubMainMenu2();
                break;
            case 2:
            case 3:
            case 4:
                CloseMainMenu();
                break;
        }
    });

    mainMenu.OnMenuClose.connect(function (sender) {
        CloseMainMenu();
    });
}

function OpenMainMenu() {
    mainMenu.CurrentSelection = 0;
    mainMenu.Visible = true;
}

function CloseMainMenu() {
    if (mainMenu != null) mainMenu.Visible = false;
    if (SubmainMenu != null) SubmainMenu.Visible = false;
    RemoveAllDate();
}

function CreateSubMainMenu2() {
    var GamePlayer = API.getGamePlayer();
    API.setCanOpenChat(false);
    API.callNative("0x8D32347D6D4C40A2", GamePlayer, false);

    var message = API.getUserInput("Введите своё сообщение (100 символов).", 100);
    API.triggerServerEvent("MAINMENU_REPORT", message);

    API.setCanOpenChat(true);
    API.callNative("0x8D32347D6D4C40A2", GamePlayer, true);
    CloseMainMenu();
}

function CreateSubMainMenu() {
    var res = API.getScreenResolution();
    SubmainMenu = API.createMenu("Статистика", "Данные о персонаже.", res.Width / 2 - 200, res.Height / 2 - 180, 0);

    var player = API.getLocalPlayer();
    var name = "";
    var lvl = 0;
    var exp = 0;
    var dolvl = 0;
    var money = 0;
    var bankmoney = 0;
    var job = 0;
    var frac = 0;
    var fracrank = 0;

    if (API.hasEntitySyncedData(player, "PLAYER_NAME")) name = API.getEntitySyncedData(player, "PLAYER_NAME");
    if (API.hasEntitySyncedData(player, "PLAYER_LEVEL")) lvl = API.getEntitySyncedData(player, "PLAYER_LEVEL");
    if (API.hasEntitySyncedData(player, "PLAYER_EXP")) exp = API.getEntitySyncedData(player, "PLAYER_EXP");
    if (API.hasEntitySyncedData(player, "PLAYER_MONEY")) money = API.getEntitySyncedData(player, "PLAYER_MONEY");
    if (API.hasEntitySyncedData(player, "PLAYER_BANKMONEY")) bankmoney = API.getEntitySyncedData(player, "PLAYER_BANKMONEY");
    if (API.hasEntitySyncedData(player, "PLAYER_JOB")) job = API.getEntitySyncedData(player, "PLAYER_JOB");
    if (API.hasEntitySyncedData(player, "PLAYER_FACTION")) frac = API.getEntitySyncedData(player, "PLAYER_FACTION");
    if (API.hasEntitySyncedData(player, "PLAYER_FACTRANK")) fracrank = API.getEntitySyncedData(player, "PLAYER_FACTRANK");
    dolvl = (lvl + 2) * (lvl + 1);

    SubmainMenu.AddItem(API.createMenuItem("Имя: ~g~" + name, ""));
    SubmainMenu.AddItem(API.createMenuItem("Уровень: ~g~" + lvl, ""));
    SubmainMenu.AddItem(API.createMenuItem("Опыт: ~g~" + exp + " ~s~/ " + dolvl, ""));
    SubmainMenu.AddItem(API.createMenuItem("Налич. средства: ~g~" + money, ""));
    SubmainMenu.AddItem(API.createMenuItem("Банк. счет: ~g~" + bankmoney, ""));
    SubmainMenu.AddItem(API.createMenuItem("Работа: ~g~" + jobstrings[job], ""));
    SubmainMenu.AddItem(API.createMenuItem("Фракция: ~g~" + frac, ""));
    SubmainMenu.AddItem(API.createMenuItem("Ранг: ~g~" + fracrank, ""));
    SubmainMenu.AddItem(API.createMenuItem("<< Назад", ""));

    SubmainMenu.RefreshIndex();
    pool.Add(SubmainMenu);
    SubmainMenu.Visible = false;

    SubmainMenu.OnItemSelect.connect(function (sender, item, index) {
        if (index == 8) CloseSubMainMenu();
    });

    SubmainMenu.OnMenuClose.connect(function (sender) {
        CloseSubMainMenu();
    });
}

function OpenSubMainMenu() {
    mainMenu.Visible = false;
    SubmainMenu.CurrentSelection = 0;
    SubmainMenu.Visible = true;
}

function CloseSubMainMenu() {
    SubmainMenu.Visible = false;
    OpenMainMenu();
}

function SecurityCheck() {
    var player = API.getLocalPlayer();
    if (API.hasEntitySyncedData(player, "PLAYER_ISLOGIN")) {
        return API.getEntitySyncedData(player, "PLAYER_ISLOGIN");
    }
    else return false;
}

function RemoveAllDate() {
    pool = null;
    mainMenu = null;
    SubmainMenu = null;
    menuOpen = null;
}