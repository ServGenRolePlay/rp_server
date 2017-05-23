var Browser = null;
var JobBus1 = null;
var JobBus2 = null;

class Cef {
    constructor(resourcePath) {
        this.path = resourcePath;
        this.open = false;
    }

    show() {
        if (this.open === false) {
            this.open = true
            var resolution = API.getScreenResolution();
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
    if (JobBus1 === null) JobBus1 = new Cef('Client/Jobs/Bus/CEF/JobBus_Menu_1.html');
    if (JobBus2 === null) JobBus2 = new Cef('Client/Jobs/Bus/CEF/JobBus_Menu_2.html');
});

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "JobBus") {
        API.showCursor(true);
        JobBus1.show();
    }
});

function JobSelectMarshryt() {
    if (JobBus1.open) JobBus1.destroy();
    JobBus2.show();
}

function Yval() { //функция Уволиться
    API.showCursor(false);
    if (JobBus1.open) JobBus1.destroy();
    API.triggerServerEvent("Yval");
}
function JobBusStart(args) {
    API.showCursor(false);
    if (JobBus2.open) JobBus2.destroy();
    API.triggerServerEvent("JobBusStart", args);
}

function MenuBack() {
    if (JobBus2.open) JobBus2.destroy();
    JobBus1.show();
}

function CloseMenu() { //функция закрытия меню
    API.showCursor(false);
    if (JobBus1.open) JobBus1.destroy();
}