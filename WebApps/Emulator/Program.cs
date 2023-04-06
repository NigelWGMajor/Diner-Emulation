using Emulator.Hubs;
using Emulator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
// our data source, could be a database
builder.Services.AddSingleton(_ => {
    var buffer = new Buffer<Point>(10);
    // start with something that can grow
    for (var i = 0; i < 7; i++) 
        buffer.AddNewRandomPoint();

    return buffer;
});
builder.Services.AddSingleton<IStorage, Storage>();
builder.Services.AddHostedService<ChartValueGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapHub<ChartHub>(ChartHub.Url);
app.Run();
