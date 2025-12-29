using CookingRecipesWeb.Services;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

// =======================
// MVC + Session
// =======================
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// =======================
// HTTP CLIENT (GLOBAL)
// =======================
builder.Services.AddHttpClient();

// =======================
// SUPABASE
// =======================
var supabaseUrl = builder.Configuration["Supabase:Url"]
    ?? throw new InvalidOperationException("Supabase:Url not set");

var supabaseKey = builder.Configuration["Supabase:AnonKey"]
    ?? throw new InvalidOperationException("Supabase:AnonKey not set");

builder.Services.AddSingleton(_ =>
    new Supabase.Client(
        supabaseUrl,
        supabaseKey,
        new Supabase.SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        }
    )
);

// =======================
// APP SERVICES
// =======================
builder.Services.AddScoped<CookingRecipesWeb.Services.RecipeService>();
builder.Services.AddScoped<CookingRecipesWeb.Services.UserService>(provider =>
    new CookingRecipesWeb.Services.UserService(
        provider.GetRequiredService<Supabase.Client>(),
        provider.GetRequiredService<IConfiguration>(),
        provider.GetRequiredService<ILogger<CookingRecipesWeb.Services.UserService>>()
    ));

var app = builder.Build();

// =======================
// Middleware
// =======================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

// =======================
// Routes
// =======================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// =======================
// Railway PORT binding
// =======================
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();
