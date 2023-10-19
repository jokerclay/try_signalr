using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using TrySignalR.Server.Hubs;
using TrySignalR.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


#region 配置 Swagger
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "v1",
            Title = "Try SignalR Api",
        });
    }
);

#endregion



#region SignalR
// **********************************
// SignalR
// on server side, SignalR is build in 
// it doesn't require install any packages
// **********************************
builder.Services.AddSignalR();

// service
builder.Services.AddHostedService<ServerTimeNotifier>();    // start it when the backend application start
#endregion




#region  CORS
builder.Services.AddCors();
#endregion



#region Auth
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
#endregion






var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();


app.UseRouting();


// 使用 Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

// config hub
// your hub class and route 
app.MapHub<NotificationsHub>("/notifications");

#region useAuth

app.UseAuthorization();


#endregion



#region  CORS policy
// AllowAny Client connect  to our backend to avoid cors issues
// In production, configure policy to match  specific client application
app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
#endregion


app.Run();
