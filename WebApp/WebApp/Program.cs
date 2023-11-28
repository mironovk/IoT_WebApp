using WebApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Add(new ServiceDescriptor(typeof(DataBaseModel), new DataBaseModel()));
//builder.Services.AddOptions();
//builder.Services.Configure<>
////var section = ConfigurationBuilder.ReferenceEquals()
//var section = ConfigurationBinder.GetValue("ConnectionStrings");
//services.AddConfiguration();
//var config = new IConfiguration
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
    //pattern: "{controller=Home}/{action=Index}/{id?}");
    pattern: "{controller=DataBase}/{action=Index}/{id?}");
//app.Configuration.GetValue()
app.Run();
