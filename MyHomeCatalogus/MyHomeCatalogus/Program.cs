using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.Authorization.Handlers;
using MyHomeCatalogus.Authorization.Requirements;
using MyHomeCatalogus.Components;
using MyHomeCatalogus.Components.Account;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Data.Interceptors;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();

builder.Services.AddRazorComponents(options =>
	options.DetailedErrors = builder.Environment.IsDevelopment());


builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
	{
		options.DefaultScheme = IdentityConstants.ApplicationScheme;
		options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
	})
	.AddIdentityCookies();

//EF Core DbContext
var connectionString =
	builder.Configuration.GetConnectionString("LocalHostConnection")
	?? throw new InvalidOperationException("Connection string"
										   + "'LocalHostConnection' not found.");

builder.Services.AddDbContextFactory<AppDbContext>(options =>
	options.UseSqlServer(connectionString)
		.AddInterceptors(new StockItemAuditInterceptor())
	);

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddEntityFrameworkStores<AppDbContext>()
	.AddSignInManager()
	.AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

//Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
builder.Services.AddScoped<IPurchaseUnitService, PurchaseUnitService>();
builder.Services.AddScoped<IRecipeIngredientService, RecipeIngredientService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IStockItemAuditService, StockItemAuditService>();
builder.Services.AddScoped<IStockItemService, StockItemService>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IStockUnitService, StockUnitService>();
builder.Services.AddScoped<IStorageUnitService, StorageUnitService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IShelfService, ShelfService>();
builder.Services.AddScoped<IShoppingListService, ShoppingListService>();
builder.Services.AddScoped<IShoppingListItemService, ShoppingListItemService>();
builder.Services.AddScoped<IProductThresholdService, ProductThresholdService>();

builder.Services.AddScoped<IToastService, ToastService>();

builder.Services.AddScoped<IAuthorizationHandler, ApprovedUserHandler>();

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("ApprovedOnly", policy =>
		policy.Requirements.Add(new ApprovedUserRequirement()));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(typeof(MyHomeCatalogus.Client._Imports).Assembly)
	.RequireAuthorization("ApprovedOnly");

app.Run();
