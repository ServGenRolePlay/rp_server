var mainMenu = null;
var FaceMenu = null;

var CharCompMenu = null;
var CharSetClothMenu = null;
var ClothSlot = null;

var CharPropsMenu = null;
var CharSetAccesMenu = null;
var AccesSlot = null;

var pool = null;
var list = null;
var menuOpen = null;
var player = null;


API.onResourceStart.connect(function () {
});

API.onResourceStop.connect(function () {
    RemoveAllDate();
});

API.onKeyDown.connect(function (sender, e) {
    if (e.KeyCode == Keys.N && !API.isChatOpen() && menuOpen == null) {
        if (SecurityCheck()) {
            player = API.getLocalPlayer();
            list = new List(String);
            for (var i = 0; i <= 250; i++) {
                list.Add(i.toString());
            }
            pool = API.getMenuPool();
            CreateMainMenu();
            CreateFaceMenu();
            CreateCharCompMenu();
            CreateCharSetClothMenu();
            CreateCharPropsMenu();
            CreateCharSetAccesMenu();
            OpenMainMenu();
            menuOpen = true;
        }
    }
});

API.onServerEventTrigger.connect(function (eventName, args) {
});

API.onUpdate.connect(function () {
    if (pool != null) {
        pool.ProcessMenus();
    }
});

function CreateMainMenu() {
    mainMenu = API.createMenu("DevTool", "Выберите пункт.", 0, 0, 6);

    mainMenu.AddItem(API.createMenuItem("Face >>", "Работа с лицом персонажа."));
    mainMenu.AddItem(API.createMenuItem("Char Components >>", "Работа с компонентами персонажа."));
    mainMenu.AddItem(API.createMenuItem("Props >>", "Работа с реквизитом."));
    mainMenu.AddItem(API.createMenuItem("<< Закрыть >>", "Закрыть меню."));

    mainMenu.RefreshIndex();
    pool.Add(mainMenu);
    mainMenu.Visible = false;
    mainMenu.MenuItems[0].Enabled = false;

    mainMenu.OnItemSelect.connect(function (sender, item, index) {
        switch (index) {
            case 0:
                OpenFaceMenu();
                break;
            case 1:
                OpenCharCompMenu();
                break;
            case 2:
                OpenCharPropsMenu();
                break;
            case 3:
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
    RemoveAllDate();
}


function CreateFaceMenu() {
    FaceMenu = API.createMenu("Char Face", "Выберите что хотите изменить.", 0, 0, 6);

    var shapeFirstId = 0;
    var shapeSecondId = 0;
    var skinFirstId = 0;
    var skinSecondId = 0;
    var shapeMix = API.f(0);
    var skinMix = API.f(0);

    var linelist = [];
    var item1 = API.createListItem("Drawable", "Возможно изменять от 0 до 250.", list, 0);
    var item2 = API.createListItem("Texture", "Возможно изменять от 0 до 250.", list, 0);
    linelist.push(item1);
    linelist.push(item2);

    FaceMenu.AddItem(item1);
    FaceMenu.AddItem(item2);
    FaceMenu.AddItem(API.createMenuItem("<< Назад", ""));

    FaceMenu.RefreshIndex();
    pool.Add(FaceMenu);
    FaceMenu.Visible = false;

    FaceMenu.OnItemSelect.connect(function (sender, item, index) {
        if (index == 0) linelist[0].Index = 0;
        else if (index == 1) linelist[1].Index = 0;
        else if (index == 2) {
            linelist[0].Index = 0;
            linelist[1].Index = 0;
            ClothDraw = 0;
            ClothText = 0;
            CloseCharSetClothMenu();
        }
    });

    FaceMenu.OnListChange.connect(function (sender, item, index) {
        ClothDraw = linelist[0].Index;
        ClothText = linelist[1].Index;
        API.sendChatMessage("SetClothes Slot: " + ClothSlot + " Draw: " + ClothDraw + " Texture: " + ClothText);
        API.setPlayerClothes(player, ClothSlot, ClothDraw, ClothText);
    });

    FaceMenu.OnMenuClose.connect(function (sender) {
        CloseFaceMenu();
    });
}

function OpenFaceMenu() {
    mainMenu.Visible = false;
    FaceMenu.CurrentSelection = 0;
    FaceMenu.Visible = true;
}

function CloseFaceMenu() {
    FaceMenu.Visible = false;
    OpenMainMenu();
}


function CreateCharCompMenu() {
    CharCompMenu = API.createMenu("Char Components", "Выберите компонент.", 0, 0, 6);

    CharCompMenu.AddItem(API.createMenuItem("Face >>", "Лицо."));
    CharCompMenu.AddItem(API.createMenuItem("Mask >>", "Маски."));
    CharCompMenu.AddItem(API.createMenuItem("Hair >>", "Волосы."));
    CharCompMenu.AddItem(API.createMenuItem("Torso >>", "Торс."));
    CharCompMenu.AddItem(API.createMenuItem("Legs >>", "Ноги."));
    CharCompMenu.AddItem(API.createMenuItem("Bags and backpacks >>", "Сумки и рюкзаки."));
    CharCompMenu.AddItem(API.createMenuItem("Feet >>", "Ступни."));
    CharCompMenu.AddItem(API.createMenuItem("Accessories >>", "Принадлежности."));
    CharCompMenu.AddItem(API.createMenuItem("Undershirt >>", "Нижняя рубашка."));
    CharCompMenu.AddItem(API.createMenuItem("Body Armor >>", "Нагрудная броня."));
    CharCompMenu.AddItem(API.createMenuItem("Decals >>", "Надписи."));
    CharCompMenu.AddItem(API.createMenuItem("Tops >>", "Верхняя одежда."));
    CharCompMenu.AddItem(API.createMenuItem("<< Назад", "Вернуться в главное меню."));

    CharCompMenu.RefreshIndex();
    pool.Add(CharCompMenu);
    CharCompMenu.Visible = false;

    CharCompMenu.OnItemSelect.connect(function (sender, item, index) {
        if (index == 12) {
            CloseCharCompMenu();
            return;
        }
        ClothSlot = index;
        OpenCharSetClothMenu();
    });

    CharCompMenu.OnMenuClose.connect(function (sender) {
        CloseCharCompMenu();
    });
}

function OpenCharCompMenu() {
    mainMenu.Visible = false;
    CharCompMenu.CurrentSelection = 0;
    CharCompMenu.Visible = true;
}

function CloseCharCompMenu() {
    CharCompMenu.Visible = false;
    OpenMainMenu();
}

function CreateCharSetClothMenu() {
    CharSetClothMenu = API.createMenu("Char Components", "Выберите компонент.", 0, 0, 6);

    var ClothDraw = 0;
    var ClothText = 0;

    var linelist = [];
    var item1 = API.createListItem("Drawable", "Возможно изменять от 0 до 250.", list, 0);
    var item2 = API.createListItem("Texture", "Возможно изменять от 0 до 250.", list, 0);
    linelist.push(item1);
    linelist.push(item2);

    CharSetClothMenu.AddItem(item1);
    CharSetClothMenu.AddItem(item2);
    CharSetClothMenu.AddItem(API.createMenuItem("<< Назад", ""));

    CharSetClothMenu.RefreshIndex();
    pool.Add(CharSetClothMenu);
    CharSetClothMenu.Visible = false;

    CharSetClothMenu.OnItemSelect.connect(function (sender, item, index) {
        if (index == 0) linelist[0].Index = 0;
        else if (index == 1) linelist[1].Index = 0;
        else if (index == 2) {
            linelist[0].Index = 0;
            linelist[1].Index = 0;
            ClothDraw = 0;
            ClothText = 0;
            CloseCharSetClothMenu();
        }
    });

    CharSetClothMenu.OnListChange.connect(function (sender, item, index) {
        ClothDraw = linelist[0].Index;
        ClothText = linelist[1].Index;
        API.sendChatMessage("SetClothes Slot: " + ClothSlot + " Draw: " + ClothDraw + " Texture: " + ClothText);
        API.setPlayerClothes(player, ClothSlot, ClothDraw, ClothText);
    });

    CharSetClothMenu.OnMenuClose.connect(function (sender) {
        CloseCharSetClothMenu();
    });
}

function OpenCharSetClothMenu() {
    CharCompMenu.Visible = false;
    CharSetClothMenu.CurrentSelection = 0;
    CharSetClothMenu.Visible = true;
}

function CloseCharSetClothMenu() {
    CharSetClothMenu.Visible = false;
    OpenCharCompMenu();
}


function CreateCharPropsMenu() {
    CharPropsMenu = API.createMenu("Char Props", "Выберите реквизит.", 0, 0, 6);

    CharPropsMenu.AddItem(API.createMenuItem("Hats >>", "Шляпы."));
    CharPropsMenu.AddItem(API.createMenuItem("Glasses >>", "Очки."));
    CharPropsMenu.AddItem(API.createMenuItem("Ears >>", "Серьги."));
    CharPropsMenu.AddItem(API.createMenuItem("<< Назад", "Вернуться в главное меню."));

    CharPropsMenu.RefreshIndex();
    pool.Add(CharPropsMenu);
    CharPropsMenu.Visible = false;

    CharPropsMenu.OnItemSelect.connect(function (sender, item, index) {
        if (index == 3) {
            CloseCharPropsMenu();
            return;
        }
        AccesSlot = index;
        OpenCharSetAccesMenu();
    });

    CharPropsMenu.OnMenuClose.connect(function (sender) {
        CloseCharPropsMenu();
    });
}

function OpenCharPropsMenu() {
    mainMenu.Visible = false;
    CharPropsMenu.CurrentSelection = 0;
    CharPropsMenu.Visible = true;
}

function CloseCharPropsMenu() {
    CharPropsMenu.Visible = false;
    OpenMainMenu();
}

function CreateCharSetAccesMenu() {
    CharSetAccesMenu = API.createMenu("Char Accessory", "Выберите реквизит.", 0, 0, 6);

    var AccesDraw = 0;
    var AccesText = 0;

    var linelist = [];
    var item1 = API.createListItem("Drawable", "Возможно изменять от 0 до 250.", list, 0);
    var item2 = API.createListItem("Texture", "Возможно изменять от 0 до 250.", list, 0);
    linelist.push(item1);
    linelist.push(item2);

    CharSetAccesMenu.AddItem(item1);
    CharSetAccesMenu.AddItem(item2);
    CharSetAccesMenu.AddItem(API.createMenuItem("<< Назад", ""));

    CharSetAccesMenu.RefreshIndex();
    pool.Add(CharSetAccesMenu);
    CharSetAccesMenu.Visible = false;

    CharSetAccesMenu.OnItemSelect.connect(function (sender, item, index) {
        if (index == 0) linelist[0].Index = 0;
        else if (index == 1) linelist[1].Index = 0;
        else if (index == 2) {
            linelist[0].Index = 0;
            linelist[1].Index = 0;
            AccesDraw = 0;
            AccesText = 0;
            CloseCharSetAccesMenu();
        }
    });

    CharSetAccesMenu.OnListChange.connect(function (sender, item, index) {
        AccesDraw = linelist[0].Index;
        AccesText = linelist[1].Index;
        API.sendChatMessage("SetAcces Slot: " + AccesSlot + " Draw: " + AccesDraw + " Texture: " + AccesText);
        API.setPlayerAccessory(player, AccesSlot, AccesDraw, AccesText);
    });

    CharSetAccesMenu.OnMenuClose.connect(function (sender) {
        CloseCharSetAccesMenu();
    });
}

function OpenCharSetAccesMenu() {
    CharPropsMenu.Visible = false;
    CharSetAccesMenu.CurrentSelection = 0;
    CharSetAccesMenu.Visible = true;
}

function CloseCharSetAccesMenu() {
    CharSetAccesMenu.Visible = false;
    OpenCharPropsMenu();
}


function DropDress()
{
    var player = API.getLocalPlayer();
    API.setPlayerClothes(player, 0, 0, 0);
    API.setPlayerClothes(player, 1, 0, 0);
    API.setPlayerClothes(player, 2, 0, 0);
    API.setPlayerClothes(player, 3, 0, 0);
    API.setPlayerClothes(player, 4, 0, 0);
    API.setPlayerClothes(player, 5, 0, 0);
    API.setPlayerClothes(player, 6, 0, 0);
    API.setPlayerClothes(player, 7, 0, 0);
    API.setPlayerClothes(player, 8, 0, 0);
    API.setPlayerClothes(player, 9, 0, 0);
    API.setPlayerClothes(player, 10, 0, 0);
    API.setPlayerClothes(player, 11, 0, 0);
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
    FaceMenu = null;
    CharCompMenu = null;
    CharSetClothMenu = null;
    ClothSlot = null;
    CharPropsMenu = null;
    CharSetAccesMenu = null;
    AccesSlot = null;
    player = null;
    list = null;
    menuOpen = null;
}