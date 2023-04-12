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

 // An example of injecting into a js object from the SignalR ChartHub
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
// An example of how to inject an element from the SignalR EventHub directly into the html.
// THis is injecting into the EventLog component at pages/components/eventlog/default
eventLogConnection.on("addEvent", function (event) {
    const li = document.createElement("li");
    li.innerHTML = event;
    li.className = event.className;
    var target = document.getElementById("eventList");
    if (target.children.length > 10)    {
        target.children[0].remove();
    }
    target.appendChild(li);
  });

// Start the connection.
start().then(() => {});