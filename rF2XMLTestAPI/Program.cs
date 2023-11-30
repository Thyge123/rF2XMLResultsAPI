using Microsoft.EntityFrameworkCore;
using rF2XMLTestAPI.DBContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll",
                              policy =>
                              {
                                  policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                              });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<DriverContext>(opt => opt.UseSqlServer(Secrets.connectionString));
builder.Services.AddDbContext<LapsContext>(opt => opt.UseSqlServer(Secrets.connectionString));
builder.Services.AddDbContext<RaceResultContext>(opt => opt.UseSqlServer(Secrets.connectionString));
builder.Services.AddDbContext<TrackContext>(opt => opt.UseSqlServer(Secrets.connectionString));
builder.Services.AddDbContext<Context>(opt => opt.UseSqlServer(Secrets.connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
