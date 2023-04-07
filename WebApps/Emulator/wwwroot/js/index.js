const data = JSON.parse(document.getElementById('data').innerHTML);
const ctx = document.getElementById('myChart').getContext('2d');
const myEventLog = document.getElementById('eventLog');
const myChart = new Chart(ctx, {
    type: 'line',
    data: data,
    options : {
        scales: {
            y : {
                suggestedMax: 10,
                suggestedMin: 1
            }
        }
    }
});

const chartConnection = new signalR.HubConnectionBuilder()
    .withUrl('/chart')
    .configureLogging(signalR.LogLevel.Information)
    .build();

    const eventLogConnection = new signalR.HubConnectionBuilder()
    .withUrl('/event')
    .configureLogging(signalR.LogLevel.Information)
    .build();

async function start() {
    try {
        await chartConnection.start();
        console.log("*** SignalR Connected Chart.");
        await eventLogConnection.start();
        console.log("*** SignalR Connected Event.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
}

chartConnection.onclose(async () => {
    await start();
});
 eventLogConnection.onclose(async () => {
     await start();
 });

chartConnection.on("addChartData", function(point) {
    
    myChart.data.labels.push(point.label);
    myChart.data.datasets.forEach((dataset) => {
        dataset.data.push(point.value);
    });

    myChart.update();

    if (myChart.data.labels.length > data.limit) {
        myChart.data.labels.splice(0, 1);
        myChart.data.datasets.forEach((dataset) => {
            dataset.data.splice(0, 1);
        });
        myChart.update();
    }
});

eventLogConnection.on("addEvent", function (event) {
    const li = document.createElement("li");
    li.textContent = JSON.stringify(event);
    document.getElementById("eventList").appendChild(li);
 
 });

// Start the connection.
start().then(() => {});