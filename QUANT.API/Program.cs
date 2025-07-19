using QUANT.PATTERNS.Base;
using QUANT.PATTERNS.TradingView;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPatternBase, PatternBase>();
builder.Services.AddScoped<ITradingViewDraw, TradingViewDraw>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "lol",
                      policy =>
                      {
                          policy.AllowAnyHeader().AllowAnyOrigin();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("lol");

app.UseAuthorization();

app.MapControllers();

app.Run();
