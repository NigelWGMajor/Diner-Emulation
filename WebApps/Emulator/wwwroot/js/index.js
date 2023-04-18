const data = JSON.parse(document.getElementById('data').innerHTML);
const ctx = document.getElementById('myChart').getContext('2d');
const myEventLog = document.getElementById('eventLog');
const myChart = new Chart(ctx, {
    type: 'line',
    data: data,
    options: {
        scales: {
            y: {
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
chartConnection.on("addChartData", function (pointset) {

    myChart.data.labels.push(pointset.label);
    //myChart.data.datasets.forEach((dataset) => {
    //    i = 1;
    //    dataset.data.push(pointset.values[i]);
    //    i++;
    //});
    myChart.data.datasets[0].data.push(pointset.values[0]);
    myChart.data.datasets[1].data.push(pointset.values[1]);
    myChart.data.datasets[2].data.push(pointset.values[2]);
    myChart.data.datasets[3].data.push(pointset.values[3]);

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
    if (event.length > 0) {
        const li = document.createElement("ul");
        li.innerHTML = event;
        li.className = event.className;
        var target = document.getElementById("eventList");
        target.replaceChildren(li);
        //if (target.children.length > 10) {
        //    target.children[0].remove();
        //}
        target.appendChild(li);
        //target.children.innerHTML = event; 
    }
});

// Start the connection.
start().then(() => { });