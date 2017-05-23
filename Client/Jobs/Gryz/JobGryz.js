var Browser = null;
var JobGryz1 = null;

class Cef {
    constructor(resourcePath) {
        this.path = resourcePath;
        this.open = false;
    }

    show() {
        if (this.open === false) {
            this.open = true
            var resolution = API.getScreenResolution()
            this.browser = API.createCefBrowser(resolution.Width, resolution.Height, true);
            API.waitUntilCefBrowserInit(this.browser);
            API.setCefBrowserPosition(this.browser, 0, 0);
            API.loadPageCefBrowser(this.browser, this.path);
            API.setCefBrowserHeadless(this.browser, false);
            Browser = this.browser;
        }
    }

    destroy() {
        this.open = false;
        API.destroyCefBrowser(this.browser);
    }

    eval(string) {
        this.browser.eval(string);
    }
}

API.onResourceStart.connect(function () {
    if (JobGryz1 == null) JobGryz1 = new Cef('Client/Jobs/Gryz/CEF/JobGryz_Menu_1.html');
});

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "JobGryzStart") {
        API.showCursor(true);
        JobGryz1.show();
    }
});

function JobStart() { //функция Начать работу
    API.showCursor(false);
    if (JobGryz1.open) JobGryz1.destroy();

    API.triggerServerEvent("JobGryzClothes");

}
function Yval() { //функция Уволиться
    API.showCursor(false);
    if (JobGryz1.open) JobGryz1.destroy();
    API.triggerServerEvent("JobGryzYval");
}
function Close() { //функция закрытия меню
    API.showCursor(false);
    if (JobGryz1.open) JobGryz1.destroy();
}
/*
// Проверка очередности снятий коробок
API.onKeyDown.connect(function (Player, args) {
    if (args.KeyCode == Keys.E && !API.isChatOpen()) {
       API.triggerServerEvent("LOL");
    }
});
*/