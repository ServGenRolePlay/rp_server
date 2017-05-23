var fuelStatus = null;
var lastUpdate = null;
var currentlyDrivingVehicle = null;

API.onResourceStart.connect(function () {
    lastUpdate = new Date().getTime();
});

API.onResourceStop.connect(function () {
    fuelStatus = null;
});

API.onServerEventTrigger.connect(function (evName, args) {
    if (evName == "FUEL_ENTER") {
        var player = API.getLocalPlayer();
        var currentVehicle = API.getPlayerVehicle(player);
        var seat = API.getPlayerVehicleSeat(player);
        if (seat != -1) {
            return;
        }

        if (fuelStatus != null) {
            return;
        }

        if (!API.hasEntitySyncedData(currentVehicle, "VEHICLE_FUEL")) return;

        var fuelStatusJson = API.getEntitySyncedData(currentVehicle, "VEHICLE_FUEL");
        fuelStatus = JSON.parse(fuelStatusJson);

        if (typeof (fuelStatus) == "undefined" || fuelStatus == null) {
            fuelStatus == null;
            return;
        }

        currentlyDrivingVehicle = currentVehicle;
        API.sendChatMessage("Fuel status: " + fuelStatus.tank.toString());
    }
    else if (evName == "FUEL_EXIT") {
        if (fuelStatus != null) {
            var player = API.getLocalPlayer();
            var currentVehicle = API.getPlayerVehicle(player);
            API.setEntitySyncedData(currentVehicle, "VEHICLE_FUEL", JSON.stringify(fuelStatus));
            fuelStatus = null;
            currentlyDrivingVehicle = null;
        }
    }
});

API.onUpdate.connect(function (sender, args) {
    if (fuelStatus == null && currentlyDrivingVehicle == null) return;
    if (!API.getVehicleEngineStatus(currentlyDrivingVehicle)) return;

    var currentTime = new Date().getTime();
    var updateDelta = currentTime - lastUpdate;
    lastUpdate = currentTime;

    var rpm = API.getVehicleRPM(currentlyDrivingVehicle);
    var consumption = ((rpm * fuelStatus.consum) / 100000) * updateDelta;
    fuelStatus.tank = Math.max(0, fuelStatus.tank - consumption);

    API.sendChatMessage("RPM: " + rpm.toString() + " Затраты: " + consumption.toString() + " Бак: " + fuelStatus.tank.toString());

    if (fuelStatus.tank <= 0 && !API.getVehicleEngineStatus(currentlyDrivingVehicle)) {
        API.setVehicleEngineStatus(currentlyDrivingVehicle, false);
        API.sendChatMessage("Кончился бензин");
    }
});

/*
API.onEntityDataChange.connect(function (entity, key, oldValue) {
    if (key == "VEHICLE_FUEL") {
        if (!API.hasEntitySyncedData(entity, "VEHICLE_FUEL")) return;
        var fuelStatusJson = API.getEntitySyncedData(entity, "VEHICLE_FUEL");
        var chFuel = JSON.parse(fuelStatusJson);
        if (fuelStatus == null) return;
        if (fuelStatus != chFuel) API.setEntitySyncedData(veh, "VEHICLE_FUEL", JSON.stringify(fuelStatus));
    }
});*/

function onEnterVehicle() {
    var player = API.getLocalPlayer();
    var currentVehicle = API.getPlayerVehicle(player);
    var seat = API.getPlayerVehicleSeat(player);
    if (seat != -1) {
        return;
    }

    if (fuelStatus != null) {
        return;
    }

    if (!API.hasEntitySyncedData(currentVehicle, "VEHICLE_FUEL")) return;

    var fuelStatusJson = API.getEntitySyncedData(currentVehicle, "VEHICLE_FUEL");
    fuelStatus = JSON.parse(fuelStatusJson);

    if (typeof (fuelStatus) == "undefined" || fuelStatus == null) {
        fuelStatus == null;
        return;
    }

    currentlyDrivingVehicle = currentVehicle;
    API.sendChatMessage("Fuel status: " + fuelStatus.tank.toString());
}

function onExitVehicle() {
    if (fuelStatus != null) {
        var player = API.getLocalPlayer();
        var currentVehicle = API.getPlayerVehicle(player);
        API.setEntitySyncedData(currentVehicle, "VEHICLE_FUEL", JSON.stringify(fuelStatus));
        fuelStatus = null;
        currentlyDrivingVehicle = null;
    }
}

/*API.onPlayerEnterVehicle.connect(function (veh) {
    var player = API.getLocalPlayer();
    var seat = API.getPlayerVehicleSeat(player);
    if (seat != -1) {
        return;
    }

    if (fuelStatus != null) {
        return;
    }

    if (!API.hasEntitySyncedData(veh, "VEHICLE_FUEL")) return;

    var fuelStatusJson = API.getEntitySyncedData(veh, "VEHICLE_FUEL");
    fuelStatus = JSON.parse(fuelStatusJson);

    if (typeof (fuelStatus) == "undefined" || fuelStatus == null) {
        fuelStatus == null;
        return;
    }

    currentlyDrivingVehicle = veh;
    API.sendChatMessage("Fuel status: " + fuelStatus.tank.ToString());
});

API.onPlayerExitVehicle.connect(function (veh) {
    if (fuelStatus != null) {
        API.setEntitySyncedData(veh, "VEHICLE_FUEL", JSON.stringify(fuelStatus));
        fuelStatus = null;
        currentlyDrivingVehicle = null;
    }
});*/