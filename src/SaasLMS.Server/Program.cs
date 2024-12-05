using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using SaasLMS.Server.Auth;
using SaasLMS.Server.Data;
using SaasLMS.Server.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<AuthConfiguration>(
    builder.Configuration.GetSection("AzureAdB2C"));

// Configure database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure multi-tenancy
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpContextAccessor();

// Configure authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        builder.Configuration.Bind("AzureAdB2C", options);
        options.TokenValidationParameters.NameClaimType = "name";
    }, options => { builder.Configuration.Bind("AzureAdB2C", options); });

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthConstants.Policies.RequireSystemAdmin, policy =>
        policy.RequireRole(AuthConstants.Roles.SystemAdmin));

    options.AddPolicy(AuthConstants.Policies.RequireTenantAdmin, policy =>
        policy.RequireRole(AuthConstants.Roles.TenantAdmin));

    options.AddPolicy(AuthConstants.Policies.RequireInstructor, policy =>
        policy.RequireRole(AuthConstants.Roles.Instructor));

    options.AddPolicy(AuthConstants.Policies.RequireTenantAccess, policy =>
        policy.Requirements.Add(new TenantRequirement()));
});

// Register authorization handlers
builder.Services.AddScoped<IAuthorizationHandler, TenantAuthorizationHandler>();

builder.Services.AddControllers();
builder.Services.AddRazorPages();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseCors();

// Add authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

// Add tenant middleware
app.UseMiddleware<TenantMiddleware>();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
