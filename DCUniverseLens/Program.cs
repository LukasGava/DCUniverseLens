

using DCUniverseLens.Entidades.EF;
using DCUniverseLens.Logica;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IPersonajeLogica, PersonajeLogica>();

builder.Services.AddScoped<IPeliculaLogica, PeliculaLogica>();

builder.Services.AddScoped<PersonajeLogica>();

//agregar dbcontext 
builder.Services.AddDbContext<Pw3DcuniverseLensContext>();

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
    pattern: "{controller=Personaje}/{action=Index}/{id?}");

app.Run();
