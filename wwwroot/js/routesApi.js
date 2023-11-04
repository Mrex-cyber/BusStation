getRoutes();

async function getRoutes(){   
    const response = await fetch(`https://localhost:7205/api/routes`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json;charset=utf-8",
            "RequestKey": "key123"
        },
    });

    const routesArray = await response.json();

    routesArray.forEach(route => createRouteNode(route));
}
async function getRoute(id){
    const response = await fetch(`https://localhost:7205/api/route/${id}`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json;charset=utf-8",
            "RequestKey": "key123"
        },
    });
    
    const route = await response.json();

    document.getElementById("routeId").value = route.id;
    document.getElementById("routeName").value = route.name;

    const stationsEditingContainer = document.getElementById("stationsContainer");
    stationsEditingContainer.innerHTML = "";
    route.stationsOnRoute.forEach(station => {
        const stationName = document.createElement("label");
        stationName.textContent = station.name + " ";
        const inputField = document.createElement("input");
        inputField.type = "number";
        inputField.setAttribute("routeidstation", route.id);
        inputField.value = parseInt(station.lengthToRight, 10);

        stationsEditingContainer.appendChild(stationName);
        stationsEditingContainer.appendChild(inputField);
    });
    stationsEditingContainer.removeChild(stationsEditingContainer.lastChild);

}
async function editRoute(routeId, routeName, routeStations) {
    const response = await fetch(`https://localhost:7205/api/route`, {
        method: "PUT",
        headers: { 
            "Accept": "application/json", "Content-Type": "application/json",
            "RequestKey": "key123"
        },
        body: JSON.stringify({
            id: parseInt(routeId, 10),
            name: routeName,
            stations: routeStations
        })
    });
    if (response.ok === true) {
        const route = await response.json();
        document.querySelector(`div[routeid='${routeId}']`).replaceWith("");
        createRouteNode(route)
    }
    else {
        const error = await response.json();
        console.log(error.message);
    }
}
function createRouteNode(route){  
    const routesList = document.getElementById("routesList");             
    const container = document.createElement("div");
    container.style.display = "inline-block";
    container.setAttribute("routeid", route.id);

    const routeName = document.createElement("h3");
    routeName.textContent = route.name;
    const busName = document.createElement("h3");
    busName.textContent = "Bus number: " + route.bus.number;
    const stations = document.createElement("ol");
    
    
    route.stationsOnRoute.forEach(station => {   
        const stationContainer = document.createElement("div");
        stationContainer.setAttribute("routeclass", station.name);

        const stationName = document.createElement("li");
        if (station.lengthToRight > 0) stationName.textContent = station.name + " (distance to next: " + station.lengthToRight + " km)";
        else stationName.textContent = station.name;       

        stationContainer.appendChild(stationName);

        stations.appendChild(stationContainer);
    });
    
    
    container.appendChild(routeName);
    container.appendChild(busName);
    container.appendChild(stations);    

    const editButton = document.createElement("button");
    editButton.textContent = "Edit";
    editButton.addEventListener("click", async() => await getRoute(route.id));
    
    container.appendChild(editButton);    

    routesList.appendChild(container);    
}

function resetRoute() {
    document.getElementById("routeId").value = "";
    document.getElementById("routeName").value = "";
}
async function updateRoute(){
    const id = document.getElementById("routeId").value;
    const name = document.getElementById("routeName").value;
    const stations = document.querySelectorAll(`input[routeidstation='${id}']`);
    stationsInts = [];
    stations.forEach(s => stationsInts.push(parseInt(s.value, 10)));
    if (id !== "") await editRoute(id, name, stationsInts);
    resetBus();
}
document.getElementById("changeRouteButton").addEventListener("click", async () => updateRoute());