dotnet new webapi -n LoginApi
cd LoginApi
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}
app.MapPost("/register", (User user) =>
{
    // Veritabanına kullanıcı kaydet
    return Results.Ok("Kayıt başarılı");
});
app.MapPost("/login", (LoginRequest login) =>
{
    // Email ve şifreyi kontrol et
    // Doğruysa JWT Token üret
    return Results.Ok(new { Token = "jwt-token" });


