var money = 100;
var resolution = API.getScreenResolutionMantainRatio();
var g_currentState = 0;
var bottles = 0;
var paper = 0;
var cabel = 0;

var kekWidth = resolution.Width - 120;
// Get list data
var listLine = 24;
var listBorder = 1;
var listPadding = 2;
var listWidth = resolution.Width * 0.19;
var listHeight = 150;
var listX = resolution.Width - 200 - listWidth / 2;
var listY = resolution.Height * 0.2 - listHeight / 2;

API.onServerEventTrigger.connect(function (eventName, args) {
    switch (eventName) {
        case "SEND_BOMJ_DATA":
            bottles = args[0];
            paper = args[1];
            cabel = args[2];
            break;

        case "OPEN_TEST_BAG":
            g_currentState = 1;
            break;

        case "CLOSE_TEST_BAG":
            g_currentState = 0;
            break;

        case "SEND_MONEY":
            money = args[0];
            break;

        case "CLOSE_BAG":
            getser.Visible = false;
            break;
    }
});

API.onUpdate.connect(function () {
    API.drawText("~g~$" + money, kekWidth, 10, 0.6, 255, 255, 255, 255, 4, 0, true, true, 0);
    if (g_currentState == 1) {
        // Fill
        API.drawRectangle(listX, listY, listWidth, listHeight, 0, 0, 0, 100);
        // Separator
        API.drawRectangle(listX, listY + listLine, listWidth, listBorder, 100, 100, 100, 255);
        // Left
        API.drawRectangle(listX - listBorder, listY - listBorder, listBorder, listHeight + listBorder * 2, 255, 255, 255, 220);
        // Right
        API.drawRectangle(listX + listWidth, listY - listBorder, listBorder, listHeight + listBorder * 2, 255, 255, 255, 220);
        // Top
        API.drawRectangle(listX, listY - listBorder, listWidth, listBorder, 255, 255, 255, 220);
        // Bottom
        API.drawRectangle(listX, listY + listHeight, listWidth, listBorder, 255, 255, 255, 220);
        //TEXT
        API.drawText("Инвентарь. Вы можете сдать в пункт приёма.", listX + listPadding, listY - listPadding / 2, 0.38, 255, 255, 0, 255, 1, 0, true, true, 0);
        API.drawText("Бутылок: " + bottles + " шт. $1шт.", listX + 3 + listPadding, listY + 25 - listPadding, 0.6, 255, 255, 0, 255, 1, 0, true, true, 0);
        API.drawText("Бумаги: " + paper + " кг. $3кг.", listX + 3 + listPadding, listY + 60 - listPadding, 0.6, 255, 255, 0, 255, 1, 0, true, true, 0);
        API.drawText("Цвет. металла: " + cabel + " кг. $5кг.", listX + 3 + listPadding, listY + 100 - listPadding, 0.6, 255, 255, 0, 255, 1, 0, true, true, 0);
    }
});