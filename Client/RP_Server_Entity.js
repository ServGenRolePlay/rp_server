var id = null;
var type = null;
var massEntity = [];

class LocalEntity {
    constructor(type, entity, id) {
        this.type = type
        this.entity = entity;
        this.id = id;
    }
    DelEntity() {
        API.deleteEntity(this.entity);
    }
}

function DelArrayEntity(element, index, array) {
    if (element.type == type && element.id.X == id.X && element.id.Y == id.Y && element.id.Z == id.Z) {
        element.DelEntity();
        massEntity.splice(index, 1);
        return;
    }
}

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "CREATE_ENTITY") {
        type = args[0];
        switch (type)
        {
            case "MARKER":
                let localMarker = new LocalEntity(type, API.createMarker(args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]), args[2]);
                massEntity.push(localMarker);
                break;

            case "WAYPOINT":
                if (API.isWaypointSet()) API.removeWaypoint();
                API.setWaypoint(args[1], args[2]);
                break;
                
            case "BLIP":
                let blip = API.createBlip(args[1]);
                API.setBlipSprite(blip, args[2]);
                API.setBlipColor(blip, args[3]);
                API.setBlipShortRange(blip, args[4]);
                API.setBlipName(blip, args[5]);
                API.setBlipRouteVisible(blip, args[6]);
                API.setBlipRouteColor(blip, args[7]);
                let localblip = new LocalEntity(type, blip, args[1]);
                massEntity.push(localblip);
                break;

            case "TEXT":
                let localText = new LocalEntity(type, API.createTextLabel(args[1], args[2], args[3], args[4], args[5]), args[2]);
                massEntity.push(localText);
                break;           
        }
    }
    else if (eventName == "DELETE_ENTITY") {
        type = args[0];
        switch (type) {

            case "WAYPOINT":
                if (API.isWaypointSet()) API.removeWaypoint();
                break;

            default:
                id = args[1];
                massEntity.forEach(DelArrayEntity);
                break;
        }
    }
});

API.onResourceStart.connect(function () {
});

API.onResourceStop.connect(function () {
    id = null;
    type = null;
    massEntity = null;
});