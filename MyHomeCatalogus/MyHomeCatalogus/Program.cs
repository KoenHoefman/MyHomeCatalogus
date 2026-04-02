using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.Authorization.Handlers;
using MyHomeCatalogus.Authorization.Requirements;
using MyHomeCatalogus.Authorization.Roles;
using MyHomeCatalogus.Components;
using MyHomeCatalogus.Components.Account;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Data.Interceptors;
using MyHomeCatalogus.Email;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Settings;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) =>
	configuration.ReadFrom.Configuration(context.Configuration)
);

// Add services to the container.
builder.Services.AddRazorComponents(options =>
		options.DetailedErrors = builder.Environment.IsDevelopment())
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();


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
	builder.Configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddSingleton<StockItemAuditInterceptor>();

builder.Services.AddDbContextFactory<AppDbContext>((serviceProvider, options) =>
	options.UseSqlServer(connectionString)
		.AddInterceptors(serviceProvider.GetRequiredService<StockItemAuditInterceptor>())
);

//Appsettings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

//Needed for Identity
builder.Services.AddScoped<AppDbContext>(p => p.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext());

builder.Services.AddIdentityCore<ApplicationUser>(options =>
	{
		options.SignIn.RequireConfirmedAccount = true;
		options.Password.RequireDigit = true;
		options.Password.RequireLowercase = true;
		options.Password.RequireUppercase = true;
		options.Password.RequireNonAlphanumeric = true;
		options.Password.RequiredLength = 11;
	})
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<AppDbContext>()
	.AddSignInManager()
	.AddDefaultTokenProviders();

builder.Services.AddTransient<IEmailSender<ApplicationUser>, EmailService>();

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
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IUserNotificationService, UserNotificationService>();

builder.Services.AddAuthorizationBuilder()
	.AddPolicy("ApprovedOnly", policy =>
		policy.AddRequirements(new ApprovedUserRequirement()))

	.AddPolicy("AdminOnly", policy =>
		policy.RequireRole(RoleConstants.Admin))

	.AddPolicy("UserOrAdmin", policy =>
		policy.RequireRole(RoleConstants.Admin, RoleConstants.RegularUser));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	await RoleSeeder.SeedRolesAsync(scope.ServiceProvider);
}

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

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(typeof(MyHomeCatalogus.Client._Imports).Assembly)
	.RequireAuthorization("ApprovedOnly");

app.MapAdditionalIdentityEndpoints();

app.Run();
