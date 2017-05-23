var countErrorPass = 0;
var RegPage = null;
var LogPage = null;
var SelectCharPage = null;
var Browser = null;
var SEND_CHARACTER_DATA = "";

class Cef {
    constructor(resourcePath) {
        this.path = resourcePath;
        this.open = false;
    }

    show() {
        if (this.open === false) {
            this.open = true
            API.displaySubtitle("Идет загрузка... Может занять несколько минут...", 60 * 1000);
            var resolution = API.getScreenResolution()
            this.browser = API.createCefBrowser(resolution.Width, resolution.Height, true);
            API.waitUntilCefBrowserInit(this.browser);
            API.setCefBrowserPosition(this.browser, 0, resolution.Height / 2 - 400);
            API.loadPageCefBrowser(this.browser, this.path);
            API.showCursor(true);
            API.setCefBrowserHeadless(this.browser, false);
            Browser = this.browser;
            API.displaySubtitle(" ", 1);
        }
    }

    destroy() {
        this.open = false;
        API.destroyCefBrowser(this.browser);
        API.showCursor(false);
    }

    eval(string) {
        this.browser.eval(string);
    }
}

API.onResourceStart.connect(function () {
    if (RegPage == null) RegPage = new Cef('Client/AuthorSystem/CEF/RegisterPage.html');
    if (LogPage == null) LogPage = new Cef('Client/AuthorSystem/CEF/LoginPage.html');
    if (SelectCharPage == null) SelectCharPage = new Cef('Client/AuthorSystem/CEF/SelectCharPage.html');
});

API.onResourceStop.connect(function () {
    RemoveAllData();
});

API.onServerEventTrigger.connect(function (evName, args) {
    if (evName == "SHOW_CHARACTER_LIST") {
        HUD(false);
        if (RegPage.open) RegPage.destroy();
        if (LogPage.open) LogPage.destroy();
        SelectCharPage.show();
        API.sleep(100);
        var list = args[0];
        SEND_CHARACTER_DATA = "";
        if (list.Count != 0) {           
            for (var i = 0; i < list.Count; i++) {
                var obj = JSON.parse(list[i]);
                if (obj.ban > 0) {
                    SEND_CHARACTER_DATA = SEND_CHARACTER_DATA + '<li><span class="menu-box-tab" style="border-bottom: 1px solid #1f253d;"><span class="icon fa-ban scnd-font-color"></span><span class="tooltips">' + obj.name + ' <font color="red">Заблокирован</font><span>Причина: ' + obj.banreason + '<br/>Будет разблокирован: ' + obj.unban + '</span></span></span></li>';
                } else {
                    SEND_CHARACTER_DATA = SEND_CHARACTER_DATA + '<li id="char' + i + '"><a class="menu-box-tab CHARLIST_START_PLAY" href="#" data-char="' + i + '" style="border-bottom: 1px solid #1f253d;"><span class="icon ' + (obj.sex == 1 ? "fa-male" : "fa-female") + ' scnd-font-color"></span>' + obj.name + ' <div class="menu-box-number" style="width: 28px;"> <span data-char="' + i + '" class="fa-trash-o CHARLIST_REMOVE_CHAR"></span></div></a></li>';
                }
            }
        }
        if (list.Count < 3) {
            SEND_CHARACTER_DATA = SEND_CHARACTER_DATA + '<li><span class="menu-box-tab" style="border-bottom: 1px solid #1f253d;" onclick="resourceCall(\'CreateChar\')"><span class="icon fa-plus scnd-font-color"></span>Создать персонаж</span></li>';
        }
        Browser.call("SEND_CHARACTER_DATA", SEND_CHARACTER_DATA);
    }
    else if (evName == "CHARLIST_BUSY_NAME") {
        HUD(false);
        CreateCharMenu("Имя занято");
    }
    else if (evName == "SHOW_REGISTER_PAGE") {
        SecurityCheck(evName);
        HUD(false);
        RegPage.show();
    }
    else if (evName == "SHOW_LOGIN_PAGE") {
        SecurityCheck(evName);
        HUD(false);
        LogPage.show();
    }
    else if (evName == "CLOSE_PAGE") {
        ClosePage();
    }
    else if (evName == "SHOW_ERROR_MESSAGE") {
        SecurityCheck(evName);
        if (args[1] == "1") {
            countErrorPass++;
            if (countErrorPass < 3) Browser.call("ErrorMessage", args[0] + "<br/>Использовано попыток " + countErrorPass + " из 3х.");
            else {
                ClosePage();
                API.disconnect("Превышено кол-во ошибочных вводов пароля.");
            }
        }
        else Browser.call("ErrorMessage", args[0]);
    }
});

function SecurityCheck(eventName) {
    var player = API.getLocalPlayer();
    if (API.hasEntitySyncedData(player, "PLAYER_ISLOGIN")) API.disconnect("Подозрение в читерстве. Код: 1. Ивент: " + eventName);
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

function Login(password) {
    if (RegPage.open) API.triggerServerEvent("PLAYER_REGISTER", password);
    if (LogPage.open) API.triggerServerEvent("PLAYER_LOGIN", password);
}

function ClosePage() {
    if (RegPage.open) RegPage.destroy();
    if (LogPage.open) LogPage.destroy();
    if (SelectCharPage.open) SelectCharPage.destroy();
    HUD(true);
    countErrorPass = 0;
}

function QuitPage() {
    RemoveAllData();
    API.disconnect("Quit");
}

function StartPlay(index) {
    API.callNative("0x891B5B39AC6302AF", 100);
    API.sleep(100);
    API.triggerServerEvent("CHARLIST_START_PLAY", index);
    ClosePage();
}

function CreateCharMenu(text) {
    HUD(false);
    API.showCursor(true);
    var name = API.getUserInput(text, 30);
    CheckLogin(name);
}

function CreateChar() {    
    ClosePage();
    CreateCharMenu("Введите имя персонажа");
}

function DeleteChar(index) {
    if (SelectCharPage.open) SelectCharPage.destroy();
    var delete_char = API.getUserInput("Напишите \"Удалить\", или \"Нет\".", 30);
    if (delete_char.toLowerCase() == "удалить"){
        API.triggerServerEvent("CHARLIST_REMOVE_CHAR", index);
        API.displaySubtitle("Персонаж был успешно удален.");
    }
    API.triggerServerEvent("SHOW_CHARACTER_LIST");
}
 
function CheckLogin(username) {
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
        API.showCursor(false);
    }
    else CreateCharMenu("Ошибка! Пример: Имя_Фамилия.");
}

function RemoveAllData() {
    countErrorPass = null;
    RegPage = null;
    LogPage = null;
    Browser = null;
    m_chars = null;
    HUD(true);
}
