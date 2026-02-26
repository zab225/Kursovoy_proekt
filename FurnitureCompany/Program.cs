using FurnitureCompany.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// регистрируем хелпер как синглтон 
builder.Services.AddSingleton<DatabaseHelper>();

var app = builder.Build();

// на локалке показываем подробные ошибки — удобно дебажить
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
// без неё ни css, ни логотип 
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();