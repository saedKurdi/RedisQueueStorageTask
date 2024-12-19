using RedisOMDBTask.MVCApp.Services.Abstract;
using RedisOMDBTask.MVCApp.Services.Concrete;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IQueueService>(qs => new QueueService("DefaultEndpointsProtocol=https;AccountName=omdbmovies;AccountKey=pe3WpayamqnibQVqb9lpgjzTKgm+4A4AEckXFDEREJKZ2pd3bd6N3jF1j4G996Jwz7Z7B5WMudsl+AStduaQAA==;BlobEndpoint=https://omdbmovies.blob.core.windows.net/;QueueEndpoint=https://omdbmovies.queue.core.windows.net/;TableEndpoint=https://omdbmovies.table.core.windows.net/;FileEndpoint=https://omdbmovies.file.core.windows.net/;", "omdbmovienames"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
