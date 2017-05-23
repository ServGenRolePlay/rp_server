var pool = null;
var sexMenu = null;
var mainMenu = null;
var faceMenu = null;
var advSetMenu = null;
var hairMenu = null;
var eyebrowsMenu = null;
var dressMenu = null;
var mainCamera = null;
var bodyCamera = null;
var faceCamera = null;
var player = null;
var man = null;
var TopType = null;
var menuOpen = null;
var closes = null;
var MoveCount = 0;
var inMain = false;
var firsttime = null;

API.onResourceStart.connect(function () {
    var players = API.getStreamedPlayers();

    for (var i = players.Length - 1; i >= 0; i--) {
        setPedCharacter(players[i]);
    }
});

API.onResourceStop.connect(function () {
    RemoveAllData();
    MoveCount = null;
    inMain = null;
});

API.onServerEventTrigger.connect(function (name, args) {
    if (name == "UPDATE_CHARACTER") {
        setPedCharacter(args[0]);
    }
    else if (name == "CHARACTER_MENU_OPEN") {
        player = args[0];
        firsttime = args[1];

        SecurityCheck(name);
        HUD(false);

        API.setEntityPosition(player, new Vector3(1164.53, -3197.71, -40.01));
        API.callNative("0x8E2530AA8ADA980E", player, API.f(1.09));

        let GamePlayer = API.getGamePlayer();
        API.callNative("0x8D32347D6D4C40A2", GamePlayer, false);

        pool = API.getMenuPool();
        mainCamera = API.createCamera(offsetPos(1164.53, -3197.71, -37.4, 1.09, 5), new Vector3(-10, 0, 181.09));
        bodyCamera = API.createCamera(offsetPos(1164.53, -3197.71, -38.4, 1.09, 3), new Vector3(-10, 0, 181.09));
        faceCamera = API.createCamera(offsetPos(1164.53, -3197.71, -38.4, 1.09, 1), new Vector3(-10, 0, 181.09));
        CreateMainMenu();
        CreateFaceMenu();
        CreateAdvSetMenu();
        CreateHairMenu();
        CreateEyebrowsMenu();
        CreateDressMenu();
        CreateSexMenu();

        API.setActiveCamera(mainCamera);
        menuOpen = false;

        if (firsttime) OpenSexMenu();
        else {
            mainMenu.MenuItems[1].Enabled = true;
            mainMenu.MenuItems[2].Enabled = true;
            OpenMainMenu(false);
        }
    }
    else if (name == "PLAYER_RESET_FACE_DATA_COMPLETE") {
        SecurityCheck(name);
        player = args[0];        
        DefaultSkin();
        setPedCharacter(player);
        EnabledMenu(sexMenu, true);
        sexMenu.Visible = false;
        OpenMainMenu(false);
    }
    else if (name == "CHARACTER_MENU_CLOSE") {
        RemoveAllData();
    }
});

API.onEntityStreamIn.connect(function (ent, entType) {
    if (entType == 6 || entType == 8) {
        setPedCharacter(ent);
    }
});

API.onUpdate.connect(function () {
    if (pool != null) {
        pool.ProcessMenus();
    }
});

API.onKeyUp.connect(function (sender, keyEventArgs) {
    if (menuOpen) {
        var rot = API.getEntityRotation(player);
        let GamePlayer = API.getLocalPlayer();
        if (keyEventArgs.KeyCode == Keys.A && !API.isChatOpen()) {
            if (rot.Z >= -45.0 && rot.Z <= 45.0) {
                API.callNative("0x8E2530AA8ADA980E", player, API.f(-88.91));
                API.setEntityRotation(player, new Vector3(0, 0, 1.09));
                API.sleep(1000);
            }
            else if (rot.Z >= -135.0 && rot.Z < -45.0) {
                API.callNative("0x8E2530AA8ADA980E", player, API.f(-178.91));
                API.setEntityRotation(player, new Vector3(0, 0, -88.91));
                API.sleep(1000);
            }
            else if (rot.Z > 45.0 && rot.Z <= 135.0) {
                API.callNative("0x8E2530AA8ADA980E", player, API.f(1.09));
                API.setEntityRotation(player, new Vector3(0, 0, 91.09));
                API.sleep(1000);
            }
            else {
                API.callNative("0x8E2530AA8ADA980E", player, API.f(91.09));
                API.setEntityRotation(player, new Vector3(0, 0, -178.91));
                API.sleep(1000);
            }

            if (MoveCount == 5) {
                MoveCount = 0;
                API.setEntityPosition(player, new Vector3(1164.53, -3197.71, -40.01));
            }
            else MoveCount++;
            API.displaySubtitle("Используйте кнопки \"A\" и \"D\" для разворота персонажа.", 15 * 1000);
            return;
        }
        if (keyEventArgs.KeyCode == Keys.D && !API.isChatOpen()) {
            if (rot.Z >= -45.0 && rot.Z <= 45.0) {
                API.callNative("0x8E2530AA8ADA980E", player, API.f(91.09));
                API.setEntityRotation(player, new Vector3(0, 0, 1.09));
                API.sleep(1000);
            }
            else if (rot.Z >= -135.0 && rot.Z < -45.0) {
                API.callNative("0x8E2530AA8ADA980E", player, API.f(1.09));
                API.setEntityRotation(player, new Vector3(0, 0, -88.91));
                API.sleep(1000);
            }
            else if (rot.Z > 45.0 && rot.Z <= 135.0) {
                API.callNative("0x8E2530AA8ADA980E", player, API.f(-178.91));
                API.setEntityRotation(player, new Vector3(0, 0, 91.09));
                API.sleep(1000);
            }
            else {
                API.callNative("0x8E2530AA8ADA980E", player, API.f(-88.91));
                API.setEntityRotation(player, new Vector3(0, 0, -178.91));
                API.sleep(1000);
            }

            if (MoveCount == 5) {
                MoveCount = 0;
                API.setEntityPosition(player, new Vector3(1164.53, -3197.71, -40.01));
            }
            else MoveCount++;
            API.displaySubtitle("Используйте кнопки \"A\" и \"D\" для разворота персонажа.", 15 * 1000);
            return;
        }
    }   
});

function setPedCharacter(ent) {
    if (API.isPed(ent) &&
        API.getEntitySyncedData(ent, "PLAYER_FACE_HAS_CHARACTER_DATA") === true &&
        (API.getEntityModel(ent) == 1885233650 || // FreemodeMale
         API.getEntityModel(ent) == -1667301416)) // FreemodeFemale
    {
        // FACE
        var shapeFirstId = API.getEntitySyncedData(ent, "PLAYER_FACE_SHAPE_FIRST_ID");
        var shapeSecondId = API.getEntitySyncedData(ent, "PLAYER_FACE_SHAPE_SECOND_ID");

        var skinFirstId = API.getEntitySyncedData(ent, "PLAYER_FACE_SKIN_FIRST_ID");
        var skinSecondId = API.getEntitySyncedData(ent, "PLAYER_FACE_SKIN_SECOND_ID");

        var shapeMix = API.f(API.getEntitySyncedData(ent, "PLAYER_FACE_SHAPE_MIX"));
        var skinMix = API.f(API.getEntitySyncedData(ent, "PLAYER_FACE_SKIN_MIX"));

        API.callNative("SET_PED_HEAD_BLEND_DATA", ent, shapeFirstId, shapeSecondId, 0, skinFirstId, skinSecondId, 0, shapeMix, skinMix, 0, false);

        // HAIR COLOR
        var hairColor = API.getEntitySyncedData(ent, "PLAYER_FACE_HAIR_COLOR");
        var highlightColor = API.getEntitySyncedData(ent, "PLAYER_FACE_HAIR_HIGHLIGHT_COLOR");

        API.callNative("_SET_PED_HAIR_COLOR", ent, hairColor, highlightColor);

        // EYE COLOR

        var eyeColor = API.getEntitySyncedData(ent, "PLAYER_FACE_EYE_COLOR");

        API.callNative("_SET_PED_EYE_COLOR", ent, eyeColor);

        // EYEBROWS, MAKEUP, LIPSTICK
        var eyebrowsStyle = API.getEntitySyncedData(ent, "PLAYER_FACE_EYEBROWS");
        var eyebrowsColor = API.getEntitySyncedData(ent, "PLAYER_FACE_EYEBROWS_COLOR");
        var eyebrowsColor2 = API.getEntitySyncedData(ent, "PLAYER_FACE_EYEBROWS_COLOR2");

        API.callNative("SET_PED_HEAD_OVERLAY", ent, 2, eyebrowsStyle, API.f(1));

        API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", ent, 2, 1, eyebrowsColor, eyebrowsColor2);

        if (API.hasEntitySyncedData(ent, "PLAYER_FACE_LIPSTICK")) {
            var lipstick = API.getEntitySyncedData(ent, "PLAYER_FACE_LIPSTICK");
            var lipstickColor = API.getEntitySyncedData(ent, "PLAYER_FACE_LIPSTICK_COLOR");
            var lipstickColor2 = API.getEntitySyncedData(ent, "PLAYER_FACE_LIPSTICK_COLOR2");

            API.callNative("SET_PED_HEAD_OVERLAY", ent, 8, lipstick, API.f(1));
            API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", ent, 8, 2, lipstickColor, lipstickColor2);
        }

        if (API.hasEntitySyncedData(ent, "PLAYER_FACE_MAKEUP")) {
            var makeup = API.getEntitySyncedData(ent, "PLAYER_FACE_MAKEUP");
            var makeupColor = API.getEntitySyncedData(ent, "PLAYER_FACE_MAKEUP_COLOR");
            var makeupColor2 = API.getEntitySyncedData(ent, "PLAYER_FACE_MAKEUP_COLOR2");

            API.callNative("SET_PED_HEAD_OVERLAY", ent, 4, makeup, API.f(1));
            API.callNative("SET_PED_HEAD_OVERLAY", ent, 8, lipstick, API.f(1));
            API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", ent, 4, 0, makeupColor, makeupColor2);
        }

        // FACE FEATURES (e.g. nose length, chin shape, etc)

        var faceFeatureList = API.getEntitySyncedData(ent, "PLAYER_FACE_FEATURES_LIST");

        for (var i = 0; i < 21; i++) {
            API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.f(faceFeatureList[i]));
        }
    }
}

function offsetPos(x, y, z, аngle, offset) {
    var degree = -аngle * (Math.PI / 180);
    var x1 = x + (offset * Math.sin(degree));
    var y1 = y + (offset * Math.cos(degree));
    return new Vector3(x1, y1, z);
}


function CreateSexMenu()
{
    var res = API.getScreenResolution();
    sexMenu = API.createMenu("Пол", "Выберите пол персонажа.", res.Width / 2 - 200, res.Height / 2 - 180, 0);

    sexMenu.AddItem(API.createMenuItem("~g~Мужской ~s~>>", "Сбросит данные о персонаже."));
    sexMenu.AddItem(API.createMenuItem("~r~Женский ~s~>>", "Сбросит данные о персонаже."));
    sexMenu.AddItem(API.createMenuItem("<< Отмена >>", ""));

    sexMenu.RefreshIndex();
    pool.Add(sexMenu);
    sexMenu.Visible = false;

    sexMenu.OnItemSelect.connect(function (sender, item, index) {
        EnabledMenu(sender, false);
        if (index == 0) {
            man = true;
        }
        else if (index == 1) {
            man = false;
        }
        else if (index == 2) {
            CloseSexMenu();
            return;
        }
        API.triggerServerEvent("PLAYER_RESET_FACE_DATA", man);
    });

    sexMenu.OnMenuClose.connect(function (sender){
        CloseSexMenu();
    });
}

function OpenSexMenu()
{
    mainMenu.Visible = false;
    sexMenu.CurrentSelection = 0;
    sexMenu.Visible = true;
}

function CloseSexMenu()
{
    if (inMain) {
        sexMenu.Visible = false;
        OpenMainMenu(false);
    }
    else {
        CloseChampCreated(firsttime);
    }
}


function CreateMainMenu()
{
    mainMenu = API.createMenu("Персонаж", "Выберите что хотите изменить.", 0, 0, 6);

    mainMenu.AddItem(API.createMenuItem("~g~Лицо ~s~>>", "Изменить настройки лица."));
    mainMenu.AddItem(API.createMenuItem("~g~Пол ~s~>>", "Изменить пол персонажа."));
    mainMenu.AddItem(API.createMenuItem("~g~Одежда ~s~>>", "Изменить одежду."));
    mainMenu.AddItem(API.createMenuItem("<< Подтвердить >>", "Завершить создание персонажа."));

    mainMenu.RefreshIndex();
    pool.Add(mainMenu);
    mainMenu.Visible = false;

    mainMenu.OnItemSelect.connect(function (sender, item, index) {
        switch (index) {
            case 0:
                API.setEntityPosition(player, new Vector3(1164.53, -3197.71, -40.01));
                if (API.getActiveCamera() != faceCamera) {
                    API.interpolateCameras(API.getActiveCamera(), faceCamera, 1000, true, true);
                    API.sleep(1000);
                    API.setActiveCamera(faceCamera);
                }
                OpenFaceMenu();
                break;

            case 1:
                OpenSexMenu();
                break;

            case 2:
                API.setEntityPosition(player, new Vector3(1164.53, -3197.71, -40.01));
                if (API.getActiveCamera() != bodyCamera) {
                    API.interpolateCameras(API.getActiveCamera(), bodyCamera, 1000, true, true);
                    API.sleep(1000);
                    API.setActiveCamera(bodyCamera);
                }
                OpenDressMenu();
                break;

            case 3:
                CloseMainMenu();
                break;
        }
    });

    mainMenu.OnMenuClose.connect(function (sender) {
        CloseChampCreated(firsttime);
    });
}

function OpenMainMenu(pause)
{
    mainMenu.CurrentSelection = 0;
    mainMenu.Visible = true;
    menuOpen = false;
    inMain = true;
    API.displaySubtitle("");
    API.setEntityPosition(player, new Vector3(1164.53, -3197.71, -40.01));
    MoveCount = 0;
    let dir = API.getEntityRotation(player);
    if (dir.Z < 1.0 || dir.Z > 2.0) {
        API.callNative("0x8E2530AA8ADA980E", player, API.f(1.09));
        API.setEntityRotation(player, dir);
    }
    if (pause) API.sleep(1000);
}

function CloseMainMenu()
{
    mainMenu.Visible = false;
    RemoveAllData();
    FinishChampCreated();
}


function CreateFaceMenu()
{
    faceMenu = API.createMenu("Голова", "Выберите что хотите изменить", 0, 0, 6);


    var list = new List(String);
    for (var i = 0; i < 19; i++) {
        let ind = i;
        ind++;
        list.Add("     Тип " + ind.toString());
    }

    var Wrinkleslist = new List(String);
    for (var i = 0; i < 3; i++) {
        let ind = i;
        ind++;
        Wrinkleslist.Add("     Тип " + ind.toString());
    }

    var eyeColorlist = new List(String);
    for (var i = 0; i < 15; i++) {
        let ind = i;
        ind++;
        eyeColorlist.Add("     Тип " + ind.toString());
    }

    var SHAPE_FIRST = API.createListItem("~o~Форма головы", "Изменить форму головы.", list, 0);
    var SHAPE_SECOND = API.createListItem("~o~Морщины", "Добавить/убрать морщины.", Wrinkleslist, 0);
    var SKIN_FIRST = API.createListItem("~o~Цвет кожи", "Изменить цвет кожи.", list, 0);
    var eyeColor = API.createListItem("~o~Цвет глаз", "Изменить цвет глаз.", eyeColorlist, 0);

    faceMenu.AddItem(SHAPE_FIRST);
    faceMenu.AddItem(SHAPE_SECOND);
    faceMenu.AddItem(SKIN_FIRST);
    faceMenu.AddItem(eyeColor);
    faceMenu.AddItem(API.createMenuItem("~g~Расширенные настройки лица ~s~>>", "Тонкая настройка лица персонажа."));
    faceMenu.AddItem(API.createMenuItem("~g~Прическа ~s~>>", "Изменить прическу."));
    faceMenu.AddItem(API.createMenuItem("~g~Брови ~s~>>", "Изменить брови."));
    faceMenu.AddItem(API.createMenuItem("<< Назад", "Вернуться в основное меню."));

    faceMenu.RefreshIndex();
    pool.Add(faceMenu);

    //faceMenu.AddInstructionalButton(new InstructionalButton("A", "Повернуть персонажа направо"));
    //faceMenu.AddInstructionalButton(new InstructionalButton("D", "Повернуть персонажа налево"));
    faceMenu.Visible = false;

    faceMenu.OnListChange.connect(function (sender, item, index) {
        if (item == SHAPE_FIRST) {
            switch (index) {
                case 0:
                    if (man) API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 0);
                    else API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 31);
                    break;

                case 1:
                    if (man) API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 1);
                    else API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 21);
                    break;

                case 2:
                    if (man) API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 2);
                    else API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 22);
                    break;

                case 3:
                    if (man) API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 3);
                    else API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 25);
                    break;

                case 4:
                    if (man) API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 4);
                    else API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 26);
                    break;

                case 5:
                    if (man) API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 5);
                    else API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 27);
                    break;

                case 6:
                    if (man) API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 6);
                    else API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 28);
                    break;

                case 7:
                    if (man) API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 8);
                    else API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 29);
                    break;

                case 8:
                    if (man) API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 9);
                    else API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 33);
                    break;

                case 9:
                    if (man) API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 10);
                    else API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 34);
                    break;
										
				case 10:
                    if (man) API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 11);
                    else API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 23);
                    break;	               

                case 11:
                    if (man) API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 12);
                    else API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 24);
                    break;	

                case 12:
                    if (man) API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 13);
                    else API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 35);
                    break;	

                case 13:
                    if (man) API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 14);
                    else API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 36);
                    break;	
                    
                case 14:
                    if (man) API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 15);
                    else API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 37);
                    break;	

                case 15:
                    if (man) API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 16);
                    else API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 38);
                    break;	

                case 16:
                    if (man) API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 17);
                    else API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 39);
                    break;	

                case 17:
                    if (man) API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 18);
                    else API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 40);
                    break;	

                case 18:
                    if (man) API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 19);
                    else API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 45);
                    break;				
            }
        }
        else if (item == SHAPE_SECOND) {
            API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_SECOND_ID", index);
        }
        else if (item == SKIN_FIRST) {
            API.setEntitySyncedData(player, "PLAYER_FACE_SKIN_FIRST_ID", index);
        }        
        else if (item == eyeColor) {
            API.setEntitySyncedData(player, "PLAYER_FACE_EYE_COLOR", index);
        }        
        setPedCharacter(player);
    });

    faceMenu.OnItemSelect.connect(function (sender, item, index) {
        switch (index) {
            case 4:
                OpenAdvSetMenu();
                break;

            case 5:
                OpenHairMenu();
                break;

            case 6:
                OpenEyebrowsMenu();
                break;

            case 7:
                CloseFaceMenu();
                break;
        }
    });

    faceMenu.OnMenuClose.connect(function (sender) {
        CloseFaceMenu();
    });
}

function OpenFaceMenu()
{
    mainMenu.Visible = false;
    faceMenu.CurrentSelection = 0;
    faceMenu.Visible = true;
    menuOpen = true;
    API.displaySubtitle("Используйте кнопки \"A\" и \"D\" для разворота персонажа.", 15 * 1000);
}

function CloseFaceMenu()
{
    faceMenu.Visible = false;
    if (API.getActiveCamera() != mainCamera) {
        API.interpolateCameras(API.getActiveCamera(), mainCamera, 1000, true, true);
        OpenMainMenu(true);
        API.setActiveCamera(mainCamera);
    }
    else OpenMainMenu(true);
}


function CreateAdvSetMenu()
{
    advSetMenu = API.createMenu("Расш. настройки", "Выберите что хотите изменить", 0, 0, 6);

    var list = new List(String);
    for (var i = -15; i <= 15; i++) {
        list.Add(i.toString());
    }

    var textList = ["~o~Размер ноздрей", "~o~Высота носа", "~o~Глубина носа", "~o~Длина горбинки носа", "~o~Высота кончика носа", "~o~Искривление носа",
        "~o~Высота бровей", "~o~Глубина бровей", "~o~Высота верхней части щек", "~o~Ширина верхней части щек", "~o~Ширина щек", "~o~Разрез глаз", "~o~Размер губ",
        "~o~Ширина скул", "~o~Длина скул", "~o~Высота подбородка", "~o~Длина подбородка", "~o~Ширина подбородка", "~o~Размер ямки на подбородке", "~o~Размер шеи", "~o~Длина шеи"];

    var linelist = [];
    for (var i = 0; i < 21; i++) {
        var item = API.createListItem(textList[i], "Возможно изменять от -15 до 15.", list, 15)
        linelist.push(item);
        advSetMenu.AddItem(item);
    }

    advSetMenu.AddItem(API.createMenuItem("<< Назад", "Вернуться в меню настройки головы."));

    advSetMenu.RefreshIndex();
    pool.Add(advSetMenu);
    advSetMenu.Visible = false;

    advSetMenu.OnItemSelect.connect(function (sender, item, index) {
        if (index == 21) {
            CloseadvSetMenu();
        }
    });

    advSetMenu.OnListChange.connect(function (sender, item, index) {
        var ind = linelist.indexOf(item, 0);
        if (ind >= 0) {
            var data = parseInt(list[index], 10) / 10;
            var faceFeatureList = API.getEntitySyncedData(player, "PLAYER_FACE_FEATURES_LIST");
            faceFeatureList[ind] = data;
            API.setEntitySyncedData(player, "PLAYER_FACE_FEATURES_LIST", faceFeatureList);
            setPedCharacter(player);
        }
    });

    advSetMenu.OnMenuClose.connect(function (sender) {
        CloseadvSetMenu();
    });
}

function OpenAdvSetMenu()
{
    faceMenu.Visible = false;
    advSetMenu.CurrentSelection = 0;
    advSetMenu.Visible = true;
}

function CloseadvSetMenu()
{
    advSetMenu.Visible = false;
    OpenFaceMenu();
}


function CreateHairMenu()
{
    hairMenu = API.createMenu("Прическа", "Выберите что хотите изменить", 0, 0, 6);
  

    var Hairstylelist = new List(String);
    for (var i = 0; i < 5; i++) {
        let ind = i;
        ind++;
        Hairstylelist.Add("     Тип " + ind.toString());
    }

    var Colorlist = new List(String);
    for (var i = 0; i < 16; i++) {
        let ind = i;
        ind++;
        Colorlist.Add("     Тип " + ind.toString());
    }

    var hair_Style = API.createListItem("~o~Прическа", "Выберите прическу.", Hairstylelist, 0);
    var hairColor = API.createListItem("~o~Цвет волос", "Задайте цвет волос.", Colorlist, 0);
    var highlightColor = API.createListItem("~o~Осветление волос", "Задайте осветление волос.", Colorlist, 0);

    hairMenu.AddItem(hair_Style);
    hairMenu.AddItem(hairColor);
    hairMenu.AddItem(highlightColor);
    hairMenu.AddItem(API.createMenuItem("<< Назад", "Вернуться в меню настройки головы."));

    hairMenu.RefreshIndex();
    pool.Add(hairMenu);
    hairMenu.Visible = false;

    hairMenu.OnItemSelect.connect(function (sender, item, index) {
        if (index == 3) {
            CloseHairMenu();
        }
    });

    hairMenu.OnListChange.connect(function (sender, item, index) {
        if (item == hair_Style) {
            switch (index) {
                case 0:
                    if (man) { API.setPlayerClothes(player, 2, 0, 0); closes[1] = 0; }
                    else { API.setPlayerClothes(player, 2, 3, 0); closes[1] = 3; }
                    break;

                case 1:
                    if (man) { API.setPlayerClothes(player, 2, 1, 0); closes[1] = 1; }
                    else { API.setPlayerClothes(player, 2, 0, 0); closes[1] = 0; }
                    break;

                case 2:
                    if (man) { API.setPlayerClothes(player, 2, 5, 0); closes[1] = 5; }
                    else { API.setPlayerClothes(player, 2, 14, 0); closes[1] = 14; }
                    break;

                case 3:
                    if (man) { API.setPlayerClothes(player, 2, 14, 0); closes[1] = 14; }
                    else { API.setPlayerClothes(player, 2, 23, 0); closes[1] = 23; }
                    break;

                case 4:
                    if (man) { API.setPlayerClothes(player, 2, 22, 0); closes[1] = 22; }
                    else { API.setPlayerClothes(player, 2, 37, 0); closes[1] = 37; }
                    break;
            }
            return;
        }
        else if (item == hairColor) {
            API.setEntitySyncedData(player, "PLAYER_FACE_HAIR_COLOR", index);
        }
        else if (item == highlightColor) {
            API.setEntitySyncedData(player, "PLAYER_FACE_HAIR_HIGHLIGHT_COLOR", index);
        }        
        setPedCharacter(player);
    });

    hairMenu.OnMenuClose.connect(function (sender) {
        CloseHairMenu();
    });
}

function OpenHairMenu()
{
    if (API.hasEntitySyncedData(player, "PLAYER_FACE_CLOSES_LIST")) closes = API.getEntitySyncedData(player, "PLAYER_FACE_CLOSES_LIST");
    else closes = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

    faceMenu.Visible = false;
    hairMenu.CurrentSelection = 0;
    hairMenu.Visible = true;
}

function CloseHairMenu()
{
    API.setEntitySyncedData(player, "PLAYER_FACE_CLOSES_LIST", closes);
    hairMenu.Visible = false;
    OpenFaceMenu();
}


function CreateEyebrowsMenu()
{
    eyebrowsMenu = API.createMenu("Брови", "Выберите что хотите изменить.", 0, 0, 6);


    var list = new List(String);
    for (var i = 0; i < 10; i++) {
        let ind = i;
        ind++;
        list.Add("     Тип " + ind.toString());
    }

    var Colorlist = new List(String);
    for (var i = 0; i < 16; i++) {
        let ind = i;
        ind++;
        Colorlist.Add("     Тип " + ind.toString());
    }

    var eyebrowsStyle = API.createListItem("~o~Вид бровей", "Изменить вид бровей.", Colorlist, 0);
    var eyebrowsColor = API.createListItem("~o~Цвет бровей", "Изменить цвет бровей.", Colorlist, 0);
    var eyebrowsColor2 = API.createListItem("~o~Осветление бровей", "Изменить осветление бровей.", Colorlist, 0);

    eyebrowsMenu.AddItem(eyebrowsStyle);
    eyebrowsMenu.AddItem(eyebrowsColor);
    eyebrowsMenu.AddItem(eyebrowsColor2);
    eyebrowsMenu.AddItem(API.createMenuItem("<< Назад", "Вернуться в меню настройки головы."));

    eyebrowsMenu.RefreshIndex();
    pool.Add(eyebrowsMenu);
    eyebrowsMenu.Visible = false;

    eyebrowsMenu.OnItemSelect.connect(function (sender, item, index) {
        if (index == 3) {
            CloseEyebrowsMenu();
        }
    });

    eyebrowsMenu.OnListChange.connect(function (sender, item, index) {
        if (item == eyebrowsStyle) {
            API.setEntitySyncedData(player, "PLAYER_FACE_EYEBROWS", index);
        }
        else if (item == eyebrowsColor) {
            API.setEntitySyncedData(player, "PLAYER_FACE_EYEBROWS_COLOR", index);
        }
        else if (item == eyebrowsColor2) {
            API.setEntitySyncedData(player, "PLAYER_FACE_EYEBROWS_COLOR2", index);
        }
        setPedCharacter(player);
    });

    eyebrowsMenu.OnMenuClose.connect(function (sender) {
        CloseEyebrowsMenu();
    });
}

function OpenEyebrowsMenu()
{
    faceMenu.Visible = false;
    eyebrowsMenu.CurrentSelection = 0;
    eyebrowsMenu.Visible = true;
}

function CloseEyebrowsMenu()
{
    eyebrowsMenu.Visible = false;
    OpenFaceMenu();
}


function CreateDressMenu()
{
    dressMenu = API.createMenu("Одежда", "Выберите что хотите изменить", 0, 0, 6);

    var Toplist = new List(String);
    for (var i = 0; i < 13; i++) {
        let ind = i;
        ind++;
        Toplist.Add("     Тип " + ind.toString());
    }

    var Undershirtlist = new List(String);
    for (var i = 0; i < 10; i++) {
        let ind = i;
        ind++;
        Undershirtlist.Add("     Тип " + ind.toString());
    }

    var list = new List(String);
    for (var i = 0; i < 8; i++) {
        let ind = i;
        ind++;
        list.Add("     Тип " + ind.toString());
    }

    var Top = API.createListItem("~o~Верхняя одежда", "Куртка/плащ.", Toplist, 0);
    var Undershirt = API.createListItem("~o~Под верхней одеждой", "Рубашка / футболка.", Undershirtlist, 0);
    var Legs = API.createListItem("~o~Штаны / шорты", "Изменить одежду на ногах.", list, 0);
    var Feet = API.createListItem("~o~Обувь", "Изменить обувь.", list, 0);

    dressMenu.AddItem(Top);
    dressMenu.AddItem(Undershirt);
    dressMenu.AddItem(Legs);
    dressMenu.AddItem(Feet);
    dressMenu.AddItem(API.createMenuItem("<< Назад", "Вернуться в основное меню."));

    dressMenu.RefreshIndex();
    pool.Add(dressMenu);
    dressMenu.Visible = false;

    dressMenu.OnListChange.connect(function (sender, item, index) {
        if (item == Top) {
            switch(index)
            {
                case 0:
                    if (man) {
                        API.setPlayerClothes(player, 11, 3, 0);
                        API.setPlayerClothes(player, 8, 0, 0);
                        API.setPlayerClothes(player, 3, 14, 0);
                        closes[9] = 3;
                        closes[7] = 0;
                        closes[2] = 14;
                        TopType = 1;
                    }
                    else {
                        API.setPlayerClothes(player, 11, 1, 0);
                        API.setPlayerClothes(player, 8, 0, 0);
                        API.setPlayerClothes(player, 3, 5, 0);
                        closes[9] = 1;
                        closes[7] = 0;
                        closes[2] = 5;
                        TopType = 1;
                    }
                    break;

                case 1:
                    if (man) {
                        API.setPlayerClothes(player, 11, 56, 0);
                        API.setPlayerClothes(player, 8, 15, 0);
                        API.setPlayerClothes(player, 3, 0, 0);
                        closes[9] = 56;
                        closes[7] = 15;
                        closes[2] = 0;
                        TopType = 2;
                    }
                    else {
                        API.setPlayerClothes(player, 11, 8, 0);
                        API.setPlayerClothes(player, 8, 0, 0);
                        API.setPlayerClothes(player, 3, 5, 0);
                        closes[9] = 8;
                        closes[7] = 0;
                        closes[2] = 5;
                        TopType = 1;
                    }
                    break;

                case 2:
                    if (man) {
                        API.setPlayerClothes(player, 11, 57, 0);
                        API.setPlayerClothes(player, 8, 15, 0);
                        API.setPlayerClothes(player, 3, 8, 0);
                        closes[9] = 57;
                        closes[7] = 15;
                        closes[2] = 8;
                        TopType = 2;
                    }
                    else {
                        API.setPlayerClothes(player, 11, 31, 0);
                        API.setPlayerClothes(player, 8, 0, 0);
                        API.setPlayerClothes(player, 3, 5, 0);
                        closes[9] = 31;
                        closes[7] = 0;
                        closes[2] = 5;
                        TopType = 1;
                    }
                    break;

                case 3:
                    if (man) {
                        API.setPlayerClothes(player, 11, 68, 0);
                        API.setPlayerClothes(player, 8, 20, 0);
                        API.setPlayerClothes(player, 3, 14, 0);
                        closes[9] = 68;
                        closes[7] = 20;
                        closes[2] = 14;
                        TopType = 3;
                    }
                    else {
                        API.setPlayerClothes(player, 11, 35, 0);
                        API.setPlayerClothes(player, 8, 0, 0);
                        API.setPlayerClothes(player, 3, 5, 0);
                        closes[9] = 35;
                        closes[7] = 0;
                        closes[2] = 5;
                        TopType = 1;
                    }
                    break;

                case 4:
                    if (man) {
                        API.setPlayerClothes(player, 11, 69, 0);
                        API.setPlayerClothes(player, 8, 20, 0);
                        API.setPlayerClothes(player, 3, 14, 0);
                        closes[9] = 69;
                        closes[7] = 20;
                        closes[2] = 14;
                        TopType = 3;
                    }
                    else {
                        API.setPlayerClothes(player, 11, 50, 0);
                        API.setPlayerClothes(player, 8, 2, 0);
                        API.setPlayerClothes(player, 3, 3, 0);
                        closes[9] = 50;
                        closes[7] = 2;
                        closes[2] = 3;
                        TopType = 2;
                    }
                    break;

                case 5:
                    if (man) {
                        API.setPlayerClothes(player, 11, 86, 0);
                        API.setPlayerClothes(player, 8, 15, 0);
                        API.setPlayerClothes(player, 3, 0, 0);
                        closes[9] = 86;
                        closes[7] = 15;
                        closes[2] = 0;
                        TopType = 2;
                    }
                    else {
                        API.setPlayerClothes(player, 11, 55, 0);
                        API.setPlayerClothes(player, 8, 2, 0);
                        API.setPlayerClothes(player, 3, 3, 0);
                        closes[9] = 55;
                        closes[7] = 2;
                        closes[2] = 3;
                        TopType = 2;
                    }
                    break;

                case 6:
                    if (man) {
                        API.setPlayerClothes(player, 11, 153, 0);
                        API.setPlayerClothes(player, 8, 15, 0);
                        API.setPlayerClothes(player, 3, 0, 0);
                        closes[9] = 153;
                        closes[7] = 15;
                        closes[2] = 0;
                        TopType = 2;
                    }
                    else {
                        API.setPlayerClothes(player, 11, 63, 0);
                        API.setPlayerClothes(player, 8, 41, 0);
                        API.setPlayerClothes(player, 3, 0, 0);
                        closes[9] = 63;
                        closes[7] = 41;
                        closes[2] = 3;
                        TopType = 3;
                    }
                    break;

                case 7:
                    if (man) {
                        API.setPlayerClothes(player, 11, 167, 0);
                        API.setPlayerClothes(player, 8, 0, 0);
                        API.setPlayerClothes(player, 3, 14, 0);
                        closes[9] = 167;
                        closes[7] = 0;
                        closes[2] = 14;
                        TopType = 1;
                    }
                    else {
                        API.setPlayerClothes(player, 11, 75, 0);
                        API.setPlayerClothes(player, 8, 2, 0);
                        API.setPlayerClothes(player, 3, 0, 0);
                        closes[9] = 75;
                        closes[7] = 2;
                        closes[2] = 0;
                        TopType = 2;
                    }
                    break;

                case 8:
                    if (man) {
                        API.setPlayerClothes(player, 11, 184, 0);
                        API.setPlayerClothes(player, 8, 15, 0);
                        API.setPlayerClothes(player, 3, 0, 0);
                        closes[9] = 184;
                        closes[7] = 15;
                        closes[2] = 0;
                        TopType = 2;
                    }
                    else {
                        API.setPlayerClothes(player, 11, 77, 0);
                        API.setPlayerClothes(player, 8, 2, 0);
                        API.setPlayerClothes(player, 3, 0, 0);
                        closes[9] = 77;
                        closes[7] = 2;
                        closes[2] = 0;
                        TopType = 2;
                    }
                    break;

                case 9:
                    if (man) {
                        API.setPlayerClothes(player, 11, 185, 0);
                        API.setPlayerClothes(player, 8, 0, 0);
                        API.setPlayerClothes(player, 3, 14, 0);
                        closes[9] = 185;
                        closes[7] = 0;
                        closes[2] = 14;
                        TopType = 1;
                    }
                    else {
                        API.setPlayerClothes(player, 11, 78, 0);
                        API.setPlayerClothes(player, 8, 2, 0);
                        API.setPlayerClothes(player, 3, 0, 0);
                        closes[9] = 78;
                        closes[7] = 2;
                        closes[2] = 0;
                        TopType = 2;
                    }
                    break;

                case 10:
                    if (man) {
                        API.setPlayerClothes(player, 11, 188, 0);
                        API.setPlayerClothes(player, 8, 15, 0);
                        API.setPlayerClothes(player, 3, 0, 0);
                        closes[9] = 188;
                        closes[7] = 15;
                        closes[2] = 0;
                        TopType = 2;
                    }
                    else {
                        API.setPlayerClothes(player, 11, 148, 0);
                        API.setPlayerClothes(player, 8, 0, 0);
                        API.setPlayerClothes(player, 3, 5, 0);
                        closes[9] = 148;
                        closes[7] = 0;
                        closes[2] = 5;
                        TopType = 1;
                    }
                    break;

                case 11:
                    if (man) {
                        API.setPlayerClothes(player, 11, 189, 0);
                        API.setPlayerClothes(player, 8, 0, 0);
                        API.setPlayerClothes(player, 3, 14, 0);
                        closes[9] = 189;
                        closes[7] = 0;
                        closes[2] = 14;
                        TopType = 1;
                    }
                    else {
                        API.setPlayerClothes(player, 11, 164, 0);
                        API.setPlayerClothes(player, 8, 0, 0);
                        API.setPlayerClothes(player, 3, 5, 0);
                        closes[9] = 164;
                        closes[7] = 0;
                        closes[2] = 5;
                        TopType = 1;
                    }
                    break;

                case 12:
                    if (man) {
                        API.setPlayerClothes(player, 11, 191, 0);
                        API.setPlayerClothes(player, 8, 0, 0);
                        API.setPlayerClothes(player, 3, 14, 0);
                        closes[9] = 191;
                        closes[7] = 0;
                        closes[2] = 14;
                        TopType = 1;
                    }
                    else {
                        API.setPlayerClothes(player, 11, 186, 0);
                        API.setPlayerClothes(player, 8, 2, 0);
                        API.setPlayerClothes(player, 3, 3, 0);
                        closes[9] = 186;
                        closes[7] = 2;
                        closes[2] = 3;
                        TopType = 2;
                    }
                    break;
            }
        }
        else if (item == Undershirt) {
            switch(index)
            {
                case 0:
                    if (TopType == 1) { API.setPlayerClothes(player, 8, 0, 0); closes[7] = 0; }
                    else if (TopType == 3) {
                        if (man) { API.setPlayerClothes(player, 8, 20, 0); closes[7] = 20; }
                        else { API.setPlayerClothes(player, 8, 41, 0); closes[7] = 41; }
                    }
                    else API.displaySubtitle("Невозможно одеть одежду под эту верхнюю.");
                    break;

                case 1:
                    if (TopType == 1) {
                        if (man) { API.setPlayerClothes(player, 8, 1, 0); closes[7] = 1; }
                        else { API.setPlayerClothes(player, 8, 11, 0); closes[7] = 11; }
                    }
                    else if (TopType == 3) {
                        if (man) { API.setPlayerClothes(player, 8, 24, 0); closes[7] = 24; }
                        else { API.setPlayerClothes(player, 8, 44, 0); closes[7] = 44; }
                    }
                    else API.displaySubtitle("Невозможно одеть одежду под эту верхнюю.");
                    break;

                case 2:
                    if (TopType == 1) {
                        if (man) { API.setPlayerClothes(player, 8, 9, 0); closes[7] = 9; }
                        else { API.setPlayerClothes(player, 8, 13, 0); closes[7] = 13; }
                    }
                    else if (TopType == 3) {
                        if (man) { API.setPlayerClothes(player, 8, 39, 0); closes[7] = 39; }
                        else { API.setPlayerClothes(player, 8, 46, 0); closes[7] = 46; }
                    }
                    else API.displaySubtitle("Невозможно одеть одежду под эту верхнюю.");
                    break;

                case 3:
                    if (TopType == 1) {
                        if (man) { API.setPlayerClothes(player, 8, 41, 0); closes[7] = 41; }
                        else { API.setPlayerClothes(player, 8, 16, 0); closes[7] = 16; }
                    }
                    else if (TopType == 3) {
                        if (man) { API.setPlayerClothes(player, 8, 40, 0); closes[7] = 40; }
                        else { API.setPlayerClothes(player, 8, 55, 0); closes[7] = 55; }
                    }
                    else API.displaySubtitle("Невозможно одеть одежду под эту верхнюю.");
                    break;

                case 4:
                    if (TopType == 1) {
                        if (man) { API.setPlayerClothes(player, 8, 42, 0); closes[7] = 42; }
                        else { API.setPlayerClothes(player, 8, 26, 0); closes[7] = 26; }
                    }
                    else if (TopType == 3) {
                        if (man) { API.setPlayerClothes(player, 8, 42, 0); closes[7] = 42; }
                        else { API.setPlayerClothes(player, 8, 56, 0); closes[7] = 56; }
                    }
                    else API.displaySubtitle("Невозможно одеть одежду под эту верхнюю.");
                    break;

                case 5:
                    if (TopType == 1) {
                        if (man) { API.setPlayerClothes(player, 8, 47, 0); closes[7] = 47; }
                        else { API.setPlayerClothes(player, 8, 27, 0); closes[7] = 27; }
                    }
                    else if (TopType == 3) {
                        if (man) { API.setPlayerClothes(player, 8, 44, 0); closes[7] = 44; }
                        else { API.setPlayerClothes(player, 8, 57, 0); closes[7] = 57; }
                    }
                    else API.displaySubtitle("Невозможно одеть одежду под эту верхнюю.");
                    break;

                case 6:
                    if (TopType == 1) {
                        if (man) { API.setPlayerClothes(player, 8, 53, 0); closes[7] = 53; }
                        else { API.setPlayerClothes(player, 8, 49, 0); closes[7] = 49; }
                    }
                    else if (TopType == 3) {
                        if (man) { API.setPlayerClothes(player, 8, 48, 0); closes[7] = 48; }
                        else { API.setPlayerClothes(player, 8, 60, 0); closes[7] = 60; }
                    }
                    else API.displaySubtitle("Невозможно одеть одежду под эту верхнюю.");
                    break;

                case 7:
                    if (TopType == 1) {
                        if (man) { API.setPlayerClothes(player, 8, 65, 0); closes[7] = 65; }
                        else { API.setPlayerClothes(player, 8, 50, 0); closes[7] = 50; }
                    }
                    else if (TopType == 3) {
                        if (man) { API.setPlayerClothes(player, 8, 54, 0); closes[7] = 54; }
                        else { API.setPlayerClothes(player, 8, 61, 0); closes[7] = 61; }
                    }
                    else API.displaySubtitle("Невозможно одеть одежду под эту верхнюю.");
                    break;

                case 8:
                    if (TopType == 1) {
                        if (man) { API.setPlayerClothes(player, 8, 67, 0); closes[7] = 67; }
                        else { API.setPlayerClothes(player, 8, 52, 0); closes[7] = 52; }
                    }
                    else if (TopType == 3) {
                        if (man) { API.setPlayerClothes(player, 8, 66, 0); closes[7] = 66; }
                        else { API.setPlayerClothes(player, 8, 66, 0); closes[7] = 66; }
                    } 
                    else API.displaySubtitle("Невозможно одеть одежду под эту верхнюю.");
                    break;

                case 9:
                    if (TopType == 1) {
                        if (man) { API.setPlayerClothes(player, 8, 76, 0); closes[7] = 76; }
                        else { API.setPlayerClothes(player, 8, 80, 0); closes[7] = 80; }
                    }
                    else if (TopType == 3) {
                        if (man) { API.setPlayerClothes(player, 8, 68, 0); closes[7] = 68; }
                        else { API.setPlayerClothes(player, 8, 79, 0); closes[7] = 79; }
                    }
                    else API.displaySubtitle("Невозможно одеть одежду под эту верхнюю.");
                    break;
            }
        }
        else if (item == Legs) {
            switch(index)
            {
                case 0:
                    if (man) { API.setPlayerClothes(player, 4, 0, 0); closes[3] = 0; }
                    else { API.setPlayerClothes(player, 4, 1, 0); closes[3] = 1; }
                    break;

                case 1:
                    if (man) { API.setPlayerClothes(player, 4, 3, 0); closes[3] = 3; }
                    else { API.setPlayerClothes(player, 4, 3, 0); closes[3] = 3; }
                    break;

                case 2:
                    if (man) { API.setPlayerClothes(player, 4, 5, 0); closes[3] = 5; }
                    else { API.setPlayerClothes(player, 4, 11, 0); closes[3] = 11; }
                    break;

                case 3:
                    if (man) { API.setPlayerClothes(player, 4, 6, 0); closes[3] = 6; }
                    else { API.setPlayerClothes(player, 4, 12, 0); closes[3] = 12; }
                    break;

                case 4:
                    if (man) { API.setPlayerClothes(player, 4, 7, 0); closes[3] = 7; }
                    else { API.setPlayerClothes(player, 4, 25, 0); closes[3] = 25; }
                    break;

                case 5:
                    if (man) { API.setPlayerClothes(player, 4, 9, 0); closes[3] = 9; }
                    else { API.setPlayerClothes(player, 4, 41, 0); closes[3] = 41; }
                    break;

                case 6:
                    if (man) { API.setPlayerClothes(player, 4, 27, 0); closes[3] = 27; }
                    else { API.setPlayerClothes(player, 4, 47, 0); closes[3] = 47; }
                    break;

                case 7:
                    if (man) { API.setPlayerClothes(player, 4, 63, 0); closes[3] = 63; }
                    else { API.setPlayerClothes(player, 4, 58, 0); closes[3] = 58; }
                    break;
            }            
        }
        else if (item == Feet) {
            switch(index)
            {
                case 0:
                    if (man) { API.setPlayerClothes(player, 6, 5, 0); closes[5] = 5; }
                    else { API.setPlayerClothes(player, 6, 1, 0); closes[5] = 1; }
                    break;

                case 1:
                    if (man) { API.setPlayerClothes(player, 6, 12, 0); closes[5] = 12; }
                    else { API.setPlayerClothes(player, 6, 3, 0); closes[5] = 3; }
                    break;

                case 2:
                    if (man) { API.setPlayerClothes(player, 6, 14, 0); closes[5] = 14; }
                    else { API.setPlayerClothes(player, 6, 4, 0); closes[5] = 4; }
                    break;

                case 3:
                    if (man) { API.setPlayerClothes(player, 6, 23, 0); closes[5] = 23; }
                    else { API.setPlayerClothes(player, 6, 5, 0); closes[5] = 5; }
                    break;

                case 4:
                    API.setPlayerClothes(player, 6, 24, 0);
                    closes[5] = 24;
                    break;

                case 5:
                    API.setPlayerClothes(player, 6, 27, 0);
                    closes[5] = 27;
                    break;

                case 6:
                    if (man) { API.setPlayerClothes(player, 6, 48, 0); closes[5] = 48; }
                    else { API.setPlayerClothes(player, 6, 25, 0); closes[5] = 25; }
                    break;

                case 7:
                    if (man) { API.setPlayerClothes(player, 6, 56, 0); closes[5] = 56; }
                    else { API.setPlayerClothes(player, 6, 30, 0); closes[5] = 30; }
                    break;
            }
        }
    });

    dressMenu.OnItemSelect.connect(function (sender, item, index) {
        if (index == 4) {
            CloseDressMenu();
        }
    });

    dressMenu.OnMenuClose.connect(function (sender) {
        CloseDressMenu();
    });
}

function OpenDressMenu()
{
    if (API.hasEntitySyncedData(player, "PLAYER_FACE_CLOSES_LIST")) closes = API.getEntitySyncedData(player, "PLAYER_FACE_CLOSES_LIST");
    else closes = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

    mainMenu.Visible = false;
    dressMenu.CurrentSelection = 0;
    dressMenu.Visible = true;
    menuOpen = true;
    API.displaySubtitle("Используйте кнопки \"A\" и \"D\" для разворота персонажа.", 15 * 1000);
}

function CloseDressMenu()
{
    API.setEntitySyncedData(player, "PLAYER_FACE_CLOSES_LIST", closes);
    dressMenu.Visible = false;
    if (API.getActiveCamera() != mainCamera) {
        API.interpolateCameras(API.getActiveCamera(), mainCamera, 1000, true, true);
        OpenMainMenu(true);
        API.setActiveCamera(mainCamera);
    }
    else OpenMainMenu(true);
}


function FinishChampCreated()
{
    let GamePlayer = API.getGamePlayer();
    API.callNative("0x8D32347D6D4C40A2", GamePlayer, true);
    API.triggerServerEvent("PLAYER_COMPLETE_FACE_CREATE");
}

function CloseChampCreated(ft)
{
    let GamePlayer = API.getGamePlayer();
    API.callNative("0x8D32347D6D4C40A2", GamePlayer, true);
    RemoveAllData();
    if (ft) API.disconnect("Отмена создания персонажа.");
    else FinishChampCreated();
}

function DefaultSkin()
{
    if (API.hasEntitySyncedData(player, "PLAYER_FACE_CLOSES_LIST")) closes = API.getEntitySyncedData(player, "PLAYER_FACE_CLOSES_LIST");
    else closes = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

    if(man) {
        API.setPlayerClothes(player, 11, 3, 0);
        API.setPlayerClothes(player, 8, 0, 0);
        API.setPlayerClothes(player, 3, 14, 0);
        API.setPlayerClothes(player, 4, 0, 0);
        API.setPlayerClothes(player, 6, 1, 0);
        TopType = 1;

        closes[9] = 3;
        closes[2] = 14;
        closes[5] = 1;
    }
    else {
        API.setEntitySyncedData(player, "PLAYER_FACE_SHAPE_FIRST_ID", 31);
        API.setPlayerClothes(player, 2, 3, 0);
        API.setPlayerClothes(player, 11, 1, 0);
        API.setPlayerClothes(player, 8, 0, 0);
        API.setPlayerClothes(player, 3, 5, 0);
        API.setPlayerClothes(player, 4, 1, 0);
        API.setPlayerClothes(player, 6, 1, 0);
        TopType = 1;

        closes[1] = 3;
        closes[9] = 1;
        closes[2] = 5;
        closes[3] = 1;
        closes[5] = 1;
    }
    API.setEntitySyncedData(player, "PLAYER_FACE_CLOSES_LIST", closes);
}

function RemoveAllData()
{
    pool = null;
    sexMenu = null;
    mainMenu = null;
    faceMenu = null;
    advSetMenu = null;
    hairMenu = null;
    eyebrowsMenu = null;
    dressMenu = null;
    mainCamera = null;
    bodyCamera = null;
    faceCamera = null;
    player = null;
    man = null;
    TopType = null;
    menuOpen = null;
    closes = null;
    firsttime = null;
    HUD(true);
}

function EnabledMenu(menu, enabled)
{
    for (var i = 0; i < menu.MenuItems.Count; i++) {
        menu.MenuItems[i].Enabled = enabled;
    }
}

function SecurityCheck(eventName) {
    var player = API.getLocalPlayer();
    if (API.hasEntitySyncedData(player, "PLAYER_ISLOGIN")) {
        if (API.getEntitySyncedData(player, "PLAYER_ISLOGIN")) API.disconnect("Подозрение в читерстве. Код: 3. Ивент: " + eventName);
    }
    else API.disconnect("Подозрение в читерстве. Код: 3. Ивент: " + eventName);
}

function HUD(toggle) {
    if (!toggle) {
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