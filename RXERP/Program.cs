using Microsoft.EntityFrameworkCore;
using RXERP.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


// connect AppDbContext to database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// this step means we are configuring the de
// AddDbContext service to use MySQL with the specified connection string
builder.Services.AddDbContext<AppDbContent>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

//save values in Session HttpContext
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

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

//enable session middleware
app.UseSession();
app.MapRazorPages();

app.Run();
