using Firebase.Database;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FirebaseClient>(new FirebaseClient("your-firebase-database-url"));

//FirebaseApp.Create(new AppOptions
//{
//    Credential = GoogleCredential.FromFile("nutrition-advisor-app-firebase-adminsdk-k0tnm-9a05b26217.json")
//});
var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
