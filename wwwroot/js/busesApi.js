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
async function getBus(id){
    const response = await fetch(`https://localhost:7205/api/bus/${id}`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json;charset=utf-8",
            "RequestKey": "key123"
        },
    });
    
    const bus = await response.json();

    document.getElementById("busId").value = bus.id;
    document.getElementById("busNumber").value = bus.number;
    document.getElementById("busCapacity").value = bus.capacity;
}
async function editBus(busId, busNumber, busCapacity) {
    const response = await fetch(`https://localhost:7205/api/buses`, {
        method: "PUT",
        headers: { 
            "Accept": "application/json", "Content-Type": "application/json",
            "RequestKey": "key123" 
        },
        body: JSON.stringify({
            id: parseInt(busId, 10),
            number: busNumber,
            capacity: parseInt(busCapacity, 10)
        })
    });
    if (response.ok === true) {
        const bus = await response.json();
        document.querySelector(`div[busid='${busId}']`).replaceWith("");
        createBusNode(bus)
    }
    else {
        const error = await response.json();
        console.log(error.message);
    }
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
    const editButton = document.createElement("button");
    editButton.textContent = "Edit";
    editButton.addEventListener("click", async() => await getBus(bus.id));

    container.appendChild(busNumber);
    container.appendChild(busCapacity);
    container.appendChild(editButton);

    busesList.appendChild(container);
}

function resetBus() {
    document.getElementById("busId").value = "";
    document.getElementById("busNumber").value = "";
    document.getElementById("busCapacity").value = "";
}
async function updateBus(){
    const id = document.getElementById("busId").value;
    const number = document.getElementById("busNumber").value;
    const capacity = document.getElementById("busCapacity").value;

    if (id !== "") await editBus(id, number, capacity);
    resetBus();
}
document.getElementById("saveBusButton").addEventListener("click", async () => updateBus());
