var builder = WebApplication.CreateBuilder(args);

// --- all builder.Services.Add... calls go here ---
builder.Services.AddControllersWithViews();
// ...your existing services...
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
});

var app = builder.Build();   // <-- services get locked in here

// --- everything below is app.Use... / app.Map... ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();          // <-- must come after UseRouting(), before UseAuthorization()/MapControllerRoute
app.UseAuthorization();

app.MapControllerRoute(
    name: "Login",
    pattern: "Login",
    defaults: new { controller = "Home", action = "Login" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();