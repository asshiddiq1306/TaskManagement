using TaskManagement.Web.Components;
using TaskManagement.Web.Services;

namespace TaskManagement.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // Add HTTP Client to communicate with API
            builder.Services.AddHttpClient("TaskManagementAPI", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7102/"); // API URL
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // Add API services
            builder.Services.AddScoped<TaskApiService>();
            builder.Services.AddScoped<UserApiService>();

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
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
