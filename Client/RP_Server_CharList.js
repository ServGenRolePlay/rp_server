var charMenu = null;
var charSubMenu2 = null;
var m_chars = null;
var pool = null;
var indexCurrect = null;

class CharInfo {
    constructor(id, name, unban, banreason) {
        this.id = id;
        this.name = name;
        this.unban = unban;
        this.banreason = banreason;
    }
}

API.onResourceStart.connect(function () {
});

API.onResourceStop.connect(function () {
    RemoveAllDate();
});

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "SHOW_CHARACTER_LIST") {
        SecurityCheck(eventName);
        HUD(false);
        m_chars = [];
        var list = args[0];
        if (list.Count != 0) {
            for (var i = 0; i < list.Count; i++) {
                var obj = JSON.parse(list[i]);
                var newChar = new CharInfo(obj.id, obj.name, obj.unban, obj.banreason);
                m_chars.push(newChar);
            }
        }
        pool = API.getMenuPool();
        CreatecharMenu();
        CreatecharSubMenu2();
        OpencharMenu();
    }
    else if (eventName == "CHARLIST_BUSY_NAME") {
        SecurityCheck(eventName);
        HUD(false);
        if (charMenu != null) charMenu.Visible = false;
        if (charSubMenu2 != null) charSubMenu2.Visible = false;
        CreatecharSubMenu1("Имя занято");
    }
    else if (eventName == "CHARLIST_CLOSE") {
        ClosecharMenu(args[0]);
    }
});

API.onKeyDown.connect(function (sender, e) {
});

API.onKeyUp.connect(function (sender, e) {
});

API.onUpdate.connect(function () {
    if (pool != null) {
        pool.ProcessMenus();
    }
});

function CreatecharMenu() {
    var res = API.getScreenResolution();
    charMenu = API.createMenu("Персонажи", "Выберите персонажа.", res.Width / 2 - 200, res.Height / 2 - 180, 0);

    for (var i = 0; i < 3; i++) {
        if (m_chars.length > i) {
            var character = m_chars[i];
            if (character.unban != "") {
                if (character.unban == "Навсегда") charMenu.AddItem(API.createMenuItem(m_chars[i].name + " Забанен навсегда. >>", "Причина бана: " + character.banreason));
                else charMenu.AddItem(API.createMenuItem(m_chars[i].name + " Забанен до " + character.unban + " >>", "Причина бана: " + character.banreason));
            }
            else charMenu.AddItem(API.createMenuItem(m_chars[i].name + " >>", ""));
        }
        else charMenu.AddItem(API.createMenuItem("Новый персонаж >>", "Создать нового персонажа."));
    }
    charMenu.AddItem(API.createMenuItem("<< Выйти >>", "Выйти с сервера."));

    charMenu.RefreshIndex();
    pool.Add(charMenu);
    charMenu.Visible = false;

    charMenu.OnItemSelect.connect(function (sender, item, index) {
        var character = null;
        if (m_chars.length > index) character = m_chars[index];

        switch (index) {
            case 0:
            case 1:
            case 2:
                indexCurrect = index;
                OpencharSubMenu(character);
                break;

            case 3:
                ClosecharMenu(true);
                break;
        }
    });

    charMenu.OnMenuClose.connect(function (sender) {
        ClosecharMenu(true);
    });
}

function OpencharMenu() {
    charMenu.CurrentSelection = 0;
    charMenu.Visible = true;
    API.displaySubtitle(" ", 1);
}

function ClosecharMenu(exit) {
    if (charMenu != null) charMenu.Visible = false;
    if (charSubMenu2 != null) charSubMenu2.Visible = false;
    RemoveAllDate();
    if (exit) API.disconnect("Quit");
}


function CreatecharSubMenu1(text) {
    var name = API.getUserInput(text, 30);
    Login(name);
}

function CreatecharSubMenu2() {
    var res = API.getScreenResolution();
    charSubMenu2 = API.createMenu("Персонажи", "Действия.", res.Width / 2 - 200, res.Height / 2 - 180, 0);

    charSubMenu2.AddItem(API.createMenuItem("Начать игру", ""));
    charSubMenu2.AddItem(API.createMenuItem("Удалить персонажа", ""));
    charSubMenu2.AddItem(API.createMenuItem("<< Назад", ""));

    charSubMenu2.RefreshIndex();
    pool.Add(charSubMenu2);
    charSubMenu2.Visible = false;

    charSubMenu2.OnItemSelect.connect(function (sender, item, index) {
        switch (index) {
            case 0:
                API.callNative("0x891B5B39AC6302AF", 100);
                API.sleep(100);
                API.triggerServerEvent("CHARLIST_START_PLAY", indexCurrect);
                charSubMenu2.Visible = false;
                ClosecharMenu(false);

                break;

            case 1:
                API.triggerServerEvent("CHARLIST_REMOVE_CHAR", indexCurrect);
                m_chars.splice(indexCurrect, 1);
                pool = null;
                charMenu = null;
                charSubMenu2 = null;
                pool = API.getMenuPool();
                CreatecharMenu();
                CreatecharSubMenu2();
                OpencharMenu();
                API.displaySubtitle("Персонаж был успешно удален.");
                break;

            case 2:
                ClosecharSubMenu();
                break;
        }
    });

    charSubMenu2.OnMenuClose.connect(function (sender) {
        ClosecharSubMenu();
    });
}

function OpencharSubMenu(character) {
    charMenu.Visible = false;
    if (character != null) {
        if (character.unban != "") {
            charSubMenu2.MenuItems[0].Enabled = false;
            API.displaySubtitle("Внимание! Аккаунт будет забанен после 3х забаненых персонажей.", 10 * 1000);
        }
        else charSubMenu2.MenuItems[0].Enabled = true;
        charSubMenu2.CurrentSelection = 0;
        charSubMenu2.Visible = true;
    }
    else CreatecharSubMenu1("Введите имя персонажа");
}

function ClosecharSubMenu() {
    charSubMenu2.Visible = false;
    OpencharMenu();
}

function SecurityCheck(eventName) {
    var player = API.getLocalPlayer();
    if (API.hasEntitySyncedData(player, "PLAYER_ISLOGIN")) {
        if (API.getEntitySyncedData(player, "PLAYER_ISLOGIN")) API.disconnect("Подозрение в читерстве. Код: 2. Ивент: " + eventName + " HAS: " + API.hasEntitySyncedData(player, "PLAYER_ISLOGIN").toString() + " " + API.getEntitySyncedData(player, "PLAYER_ISLOGIN").toString());
    }
    else API.disconnect("Подозрение в читерстве. Код: 2. Ивент: " + eventName + " HAS: " + API.hasEntitySyncedData(player, "PLAYER_ISLOGIN").toString());
}

function HUD(toggle) {
    if (!toggle) {
        let newCamera = API.createCamera(new Vector3(-27.31, 606.78, 307.39), new Vector3(-10, 0, 152.69));
        API.setActiveCamera(newCamera);
        API.setHudVisible(false);
        API.setCanOpenChat(false);
        API.setChatVisible(false);
        API.displaySubtitle(" ", 1);
    }
    else {
        API.setActiveCamera(null);
        API.setHudVisible(true);
        API.setCanOpenChat(true);
        API.setChatVisible(true);
        API.showCursor(false);
        API.displaySubtitle(" ", 1);
    }
}

function Login(username) {
    let regex = /[а-яА-Яa-zA-Z_]/;
    let prob = /_/;
    let counter_numb = 0;
    let counter_prob = 0;
    for (var i = 0; i < username.length; i++) {
        if (!regex.test(username[i])) counter_numb++;
        if (prob.test(username[i])) counter_prob++;
    }

    if (username[username.length - 1] == " " || username[0] == " ") counter_numb++;

    if (counter_numb == 0 && counter_prob == 1) {
        API.triggerServerEvent("CHARLIST_CREATE_NEW", username);
        ClosecharMenu(false);
    }
    else CreatecharSubMenu1("Ошибка Пример: Имя_Фамилия.");
}

function RemoveAllDate() {
    pool = null;
    charMenu = null;
    charSubMenu2 = null;
    m_chars = null;
    indexCurrect = null;
    HUD(true);
}
