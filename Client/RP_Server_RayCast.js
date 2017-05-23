var lastobject = null;
var markerEyes = null;
var marker = null;
var lastCheck = null;
var Check = null;

API.onResourceStart.connect(function () {
    lastCheck = new Date().getTime();
});

API.onResourceStop.connect(function () {
    if (markerEyes != null) {
        API.deleteEntity(markerEyes);
        markerEyes = null;
    }
    if (marker != null) {
        API.deleteEntity(marker);
        marker = null;
    }
    lastobject = null;
    lastCheck = null;
    Check = null;
});

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "SHOW_RAY") {
        Check = 1;
    }
    if (eventName == "CLOSE_RAY") {
        Check = null;
    }
});

API.onUpdate.connect(function () {
    if (Check == null) return;
    var currentTime = new Date().getTime();
    if (currentTime - lastCheck > 300) {
        lastCheck = currentTime;
        var player = API.getLocalPlayer();
        if (API.isPlayerInAnyVehicle(player)) return;

        var startPoint = API.getGameplayCamPos();
        var aimPoint = getAimPoint();

        startPoint.Add(aimPoint);
        var endPoint = Vector3Lerp(startPoint, aimPoint, 1.1);       

        if (!inCircle(player, aimPoint, 160)) {
            if (marker != null) {
                API.deleteEntity(marker);
                marker = null;
                lastobject = null;
            }
            return;
        }

        if (markerEyes != null) API.deleteEntity(markerEyes);
        markerEyes = API.createMarker(3, endPoint, new Vector3(1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 1), 237, 36, 36, 100);

        var rayCast = API.createRaycast(startPoint, endPoint, 16 | 2, null);

        if (!rayCast.didHitEntity && marker != null) {
            API.deleteEntity(marker);
            marker = null;
            lastobject = null;
            return;
        }

        if (rayCast.didHitEntity && lastobject != rayCast.hitEntity.ToString()) {
            var hitEntityHandle = rayCast.hitEntity;
            if (API.getEntityPosition(hitEntityHandle).DistanceTo(API.getEntityPosition(API.getLocalPlayer())) < 4) {
                lastobject = hitEntityHandle.ToString();
                if (marker != null) {
                    API.deleteEntity(marker);
                    marker = null;
                }
                var entposs = API.getEntityPosition(hitEntityHandle);
                marker = API.createMarker(1, entposs, new Vector3(1, 1, 1), new Vector3(36, 1, 1), new Vector3(1.5, 1.5, 5), 237, 36, 36, 100);
                API.sendChatMessage(hitEntityHandle.ToString() + " | " + API.getEntityModel(hitEntityHandle).toString());
                API.sendChatMessage("X: " + entposs.X.toFixed(2).toString() + " Y: " + entposs.Y.toFixed(2).toString() + " Z: " + entposs.Z.toFixed(2).toString());
            }
        }
    }
    
    //var rayCast = API.createRaycast(startPoint, endPoint, Enums.IntersectOptions.Everything, null);
   // if (!rayCast.didHitEntity) return null;
   // var hitEntityHandle = rayCast.hitEntity;
    //var entityModel = API.getEntityModel(hitEntityHandle);
    //if (GasPumpModels.indexOf(entityModel) == -1) return null;
    //if (API.getEntityPosition(hitEntityHandle).DistanceTo(API.getEntityPosition(API.getLocalPlayer())) > 3) return null;
});

function inCircle(player, coord, angle) {
    var playerpos = API.getEntityPosition(player);
    var ang = API.getEntityRotation(player).Z;
    var kut = API.getGameplayCamRot().Z;

    if (ang < 0) ang += 360;
    if (kut < 0) kut += 360;
    var start = ang - angle / 2;
    var end = ang + angle / 2;

    if (start < 0) {
        if (start + 360 <= kut && kut <= 360 || 0 <= kut && kut <= end) return true;
        else return false;
    }
    else if (end > 360) {
        if (start <= kut && kut <= 360 || 0 <= kut && kut <= end - 360) return true;
        else return false;
    }
    else if (start < kut && kut < end) return true;
    else return false;
}

function Vector3Lerp(start, end, fraction) {
    return new Vector3(
        (start.X + (end.X - start.X) * fraction),
        (start.Y + (end.Y - start.Y) * fraction),
        (start.Z + (end.Z - start.Z) * fraction)
    );
}

function getAimPoint() {
    var resolution = API.getScreenResolutionMantainRatio();
    return API.screenToWorldMantainRatio(new PointF(resolution.Width / 2, resolution.Height / 2));
}

function offsetPos(x, y, z, аngle, offset) {
    var degree = -аngle * (Math.PI / 180);
    var x1 = x + (offset * Math.sin(degree));
    var y1 = y + (offset * Math.cos(degree));
    return new Vector3(x1, y1, z);
}