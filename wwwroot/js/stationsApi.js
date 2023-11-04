async function addStation() {
    const route = "Epicentr";
    const response = await fetch(`https://localhost:7205/api/${route}/stations`, {
        method: "POST",
        headers: { 
            "Accept": "application/json", "Content-Type": "application/json",
            "RequestKey": "key123" 
        },
        body: JSON.stringify({
            id: 20,
            name: "Z",
            leftStationName: "H",
            lengthToLeftStation: parseInt(15, 10)
        })
    });
    if (response.ok === true) {
        const station = await response.json();
        console.log(station);
    }
    else {
        const error = await response.json();
        console.log(error.message);
    }
}

document.getElementById("addStationButton").addEventListener("click", async function(){
    console.log("Add station button work");
    await addStation();
});