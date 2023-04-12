# Notes

Layout has a navbar.

Home page is index
Also Monitor, Settings and About 
Each page has an example of a page redirect and a local post.

The monitor page at the moment has an embedded graph and a component.

***I would like both to be components that can be refreshed individually***

An Error page can show too.

Components:
- Eventlog
***need graphic component(s)***
***need updatable simple binds***

Theory: Code behind can be updated through server-side means. Regions being bound to the components can be updated client-side by js. The trick is to trigger the partial updates remotely.

SignalR is doing this with the graph right now.

So let's trace how that is working.

## SignalR presence:

program
~> hostedService ChartValueGenerator
~> MapHub ChartHub.url

ChartValueGenerator : BackgroundService
<>- hub
<>- buffer
ExecuteAsync:
    hub.Clients.All.SendAsync
        method: "jsMethod",
        bufferMethodThatReturnNewData ...
This loops indefinitely.

buffer == data
hub == just a url

The component is inserted into the Monitor Page as a canvas followed by a script.
```html
<div>
    <div style="width: 600px; height: 400px">
        <canvas id="myChart" width="400" height="200"></canvas>
    </div>
    <script id="data" type="application/json">
    @Json.Serialize(
        new
        {
            labels,
            limit = Buffer.MaxCapacity.GetValueOrDefault(40),
            url = "/chart",
            datasets = new object[]
            {
                new
                {
                    label = "Data flow",
                    data,
                    fill = false,
                    borderColor = "rgb(75, 192, 192)",
                    tension = 0.1
                }
            }
        })    
    </script>
    </div>
```    
But there is more script in index.js.

```js
// this gets the data shown in the page above:
const data = JSON.parse(document.getElementById('data').innerHTML);
// this I guess gets the 2d-canvas that the chart is on?
const ctx = document.getElementById('myChart').getContext('2d');
// this defines the chart
const myChart = new Chart(ctx, {
    type: 'Line',
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
// this makes a connection to the hub
const connection = new signalR.HubConnectionBuilder()
    .withUrl(data.url)
    .configureLogging(signalR.LogLevel.Information)
    .build();
// this starts signalR
async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
}
// this restarts closed connection?
connection.onclose(async () => {
    await start();
});
// this receives the message from the SignalR connection, and adds the new point to the data
connection.on("addChartData", function(point) {
    
    myChart.data.labels.push(point.label);
    myChart.data.datasets.forEach((dataset) => {
        dataset.data.push(point.value);
    });

    myChart.update();
    // this removes the first element if the limit is reached 
    if (myChart.data.labels.length > data.limit) {
        myChart.data.labels.splice(0, 1);
        myChart.data.datasets.forEach((dataset) => {
            dataset.data.splice(0, 1);
        });
        myChart.update();
    }
});

// this starts the SignalR connection
start().then(() => {});
```

So in all:

We have a chart on a canvas on a page. 
The actual live chart is built in js in index.js.
On the pge it's inserted into, the empty chart is defined inline js.
A background service loops through creating new data:
a call is made to the js method to add the new point into the data and trim off overflow.
The JS actually updates the data.

The buffer class is irrelevant: the items are added one by one!

So, let's try make a component with a graph in it. then we can use that as a template to show stuff in various ways.

Generic solution:

1. in ViewComponents, make a XXXComponent.cs file.
2. make Pages/Shared/Components/XXX/default.cshtml
3. in a page, insert the XXX
4. Hook it up to an XXX signalR hub in the Hubs folder 
5. Make a service in the Services folder for data
6. Make a JS function to accept data
7. In the service loop, use the hub to push data from the service to the JS method.
8. Pump some data to a couple of data sets (could use stack and send negative and positive...)

Note: possible strategy when the code-behind has all the data and it is bound, might be to have a StateHasChanged fire from the client, and not need to pass data. In the case of a js control like the chart, we needed to do that: something like a list display might not need that though...

Steps to animate EventLog:

1. Insert a hub for the event log
2. Add code to start the hub in Index.js
3. Add the event to trigger, and a ref to the element
4. Inject the hub into the service
5. Add something in the service to trigger the hub and update the model
6. Add the MapHub to program

