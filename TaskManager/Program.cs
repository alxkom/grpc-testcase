using TaskManager.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<IDataService, DataService>();
builder.Services.AddAutoMapper(typeof(AppMappingProfile));

// Read gRPC configuration settings and setup the client channel.
var gRPCProtocolSettings = new gRPCProtocolSettings("http", "localhost", 5001);
builder.Configuration.GetRequiredSection(nameof(gRPCProtocolSettings)).Bind(gRPCProtocolSettings);
builder.Services.AddGrpcClient<TaskManagerProvider.TaskManager.TaskManagerClient>(o =>
{
    o.Address = new Uri($"{gRPCProtocolSettings.Protocol}://{gRPCProtocolSettings.Url}:{gRPCProtocolSettings.Port}");
})
.ConfigureChannel(o =>
{
    o.MaxReceiveMessageSize = 5 * 1024 * 1024; // 5 MB
    o.MaxSendMessageSize = 2 * 1024 * 1024; // 2 MB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
