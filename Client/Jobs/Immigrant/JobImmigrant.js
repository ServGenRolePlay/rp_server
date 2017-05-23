var Browser = null;
var JobImmigrant = null;
var bottles = 0;
var paper = 0;
var cabel = 0;
var percent = 0;

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
    if (JobImmigrant == null) JobImmigrant = new Cef('Client/Jobs/Immigrant/CEF/JobImmigrant.html');
});

API.onServerEventTrigger.connect(function (eventName, args) {
    switch (eventName) {
        case "SEND_BOMJ_DATA":
            bottles = args[0];
            paper = args[1];
            cabel = args[2];
            percent = parseInt(((parseInt(bottles) * 0.1 + parseInt(cabel) + parseInt(paper)) / 40) * 100); // 40 кг сумка вмещает, больше нормальному человеку не унести
            if (percent > 100) { percent = 100; }
            var SEND_BOMJ_DATA = 'document.getElementById("bag_percent").innerHTML = ' + percent + ';\
            document.getElementById("cabel").innerHTML = ' + cabel + ';\
            document.getElementById("paper").innerHTML = '+ paper + ';\
            document.getElementById("bottles").innerHTML = '+ bottles + ';\
            document.getElementById("bag_percent_progress").style.width = "'+ percent + '%";';
            JobImmigrant.eval(SEND_BOMJ_DATA);
            break;

        case "OPEN_TEST_BAG":
            JobImmigrant.show();
            break;

        case "CLOSE_TEST_BAG":
            if (JobImmigrant.open) JobImmigrant.destroy();
            break;
    }
});