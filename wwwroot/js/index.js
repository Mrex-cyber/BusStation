getRoutes();
getBuses();

async function getBuses(){    
    const response = await fetch(`https://localhost:7205/api/buses`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json;charset=utf-8",
            "RequestKey": "key123"
        },
    });

    const busesArray = await response.json();
    busesArray.forEach(bus => createBusNode(bus));
}
function createBusNode(bus){
    const busesList = document.getElementById("busesList");
    const container = document.createElement("div");
    container.setAttribute("busid", bus.id);
    container.style.display = "inline-block";
        
    const busNumber = document.createElement("h3");
    busNumber.textContent = "Number: " + bus.number;
    const busCapacity = document.createElement("h3");
    busCapacity.textContent = "Capacity: " + bus.capacity;    

    container.appendChild(busNumber);
    container.appendChild(busCapacity);

    busesList.appendChild(container);
}

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
function createRouteNode(route){  
    const routesList = document.getElementById("routesList");             
    const container = document.createElement("div");
    container.style.display = "inline-block";
        
    const routeName = document.createElement("h3");
    routeName.textContent = route.name;
    const busName = document.createElement("h3");
    busName.textContent = "Bus number: " + route.bus.number;
    const stations = document.createElement("ol");

    route.stationsOnRoute.forEach(station => {            
        const stationContainer = document.createElement("div");

        const stationName = document.createElement("li");
        stationName.textContent = station.name;
        const labelIn = document.createElement("label");
        labelIn.textContent = "Entered ";
        const peopleIn = document.createElement("input");
        const labelOut = document.createElement("label");
        labelOut.textContent = "Exited ";
        const peopleOut = document.createElement("input");
        
        peopleIn.type = "number";
        peopleIn.classList.add("peopleIn");
        peopleOut.type = "number";
        peopleOut.classList.add("peopleOut");

        stationContainer.appendChild(stationName);
        stationContainer.appendChild(labelIn);
        stationContainer.appendChild(peopleIn);
        stationContainer.appendChild(labelOut);
        stationContainer.appendChild(peopleOut);

        stations.appendChild(stationContainer);
    });
    
    container.appendChild(routeName);
    container.appendChild(busName);
    container.appendChild(stations);

    const timeField = document.createElement("input");
    timeField.setAttribute("type", "time");
    timeField.id = route.name;

    const sendButton = document.createElement("button");
    sendButton.type = "button";
    sendButton.id = routeName + "Button";
    sendButton.textContent = "Check people";

    

    sendButton.addEventListener("click", function(){
        const fullTime = timeField.value.split(":");
        sendPeople(timeField.id, fullTime[0], fullTime[1]);
    } );

    container.appendChild(timeField);
    container.appendChild(sendButton);    

    routesList.appendChild(container);    
}

async function checkBusTime(){
    const timeField = document.getElementById("timeField").value.split(':');
    const busField = document.getElementById("busField").value;

    console.log(timeField);
    const response = await fetch(`https://localhost:7205/api/time/${busField}/${timeField[0]}/${timeField[1]}`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json;charset=utf-8",
            "RequestKey": "key123"
        },
    });

    const text = await response.json();

    alert(text);
}
async function sendPeople(routeName, hours, minutes){
    const inputClassesIn = document.getElementsByClassName("peopleIn");
    const peopleEntered = [];
    for (let i = 0; i < inputClassesIn.length; i++){
        if (inputClassesIn[i].value == ""){
            peopleEntered.push("0");
        }
        else {
            peopleEntered.push(inputClassesIn[i].value);
        }
    }

    const inputClassesOut = document.getElementsByClassName("peopleOut");
    const peopleExited = [];
    for (let i = 0; i < inputClassesOut.length; i++){
        if (inputClassesOut[i].value == ""){
            peopleExited.push("0");
        }
        else {
            peopleExited.push(inputClassesOut[i].value);
        }
    }

    const response = await fetch(`https://localhost:7205/api/time/${routeName}/people/${hours}/${minutes}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json;charset=utf-8",
            "RequestKey": "key123"
        },
        body: JSON.stringify({
            entered: peopleEntered,
            exited: peopleExited
        })
    });

    const text = await response.json();

    alert(text);
}