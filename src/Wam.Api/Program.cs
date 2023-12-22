using HexMaster.RedisCache;
using Wam.Core.Configuration;
using Wam.Users.Repositories;
using Wam.Users.Services;

var corsPolicyName = "DefaultCors";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAzureConfiguration(builder.Configuration);
builder.Services.AddHexMasterCache(builder.Configuration);

builder.Services.AddTransient<IUsersService, UsersService>();
builder.Services.AddTransient<IUsersRepository, UsersRepository>();

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://app.tinylnk.nl")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddControllers()
    .AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(corsPolicyName);
app.UseAuthorization();
app.MapControllers();

app.Run();
