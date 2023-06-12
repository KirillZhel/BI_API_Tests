using System.Linq;
using System.Threading.Tasks;
using WebApi.Integration.Services;
using Xunit;

/*
* Написать тесты на метод получения куки /Auth/Login (см. сервис CookieAuthService):
* - При корректных кредах (admin, admin) метод должен вернуть true и куки
* - При некорректных кредах метод должен вернуть false
*/

namespace WebApi.Integration.Tests.Homework
{
    public class CookieTests : IClassFixture<TestFixture>
    {
        private readonly CookieService _cookieService;

        public CookieTests(TestFixture testFixture)
        {
            _cookieService = new CookieService();
        }

        /// <summary>
        /// Пользователь ввёл корректные данные и должен получить true и куки
        /// </summary>
        [Fact]
        public async Task IfUserHaveCorrectNameAndPassword_ReturnCookieAndTrue()
        {
            // Arrange
            var correctUsername = "admin";
            var correctPassword = "admin";
            var expectedMessageContent = "true";

            // Act
            var httpResponseMessage = await _cookieService.GetCookieInternalAsync(correctUsername, correctPassword);

            // Assert
            var messageContent = await httpResponseMessage.Content.ReadAsStringAsync();
            Assert.Equal(expectedMessageContent, messageContent);
            var setCookieValue = httpResponseMessage.Headers.FirstOrDefault(h => h.Key == "Set-Cookie").Value.ToList().First();
            Assert.NotNull(setCookieValue);
        }

        /// <summary>
        /// Пользователь ввёл некорректные имя и пароль и должен получить false
        /// </summary>
        [Fact]
        public async Task IfUserHaveWrongNameAndWrongPassword_ReturnFalse()
        {
            // Arrange
            var correctUsername = "wrongname";
            var wrongPassword = "wrongpassword";
            var expectedMessageContent = "false";

            // Act
            var httpResponseMessage = await _cookieService.GetCookieInternalAsync(correctUsername, wrongPassword);

            // Assert
            var messageContent = await httpResponseMessage.Content.ReadAsStringAsync();
            Assert.Equal(expectedMessageContent, messageContent);
        }

        /// <summary>
        /// Пользователь ввёл некорректное имя и корректный пароль и должен получить false
        /// </summary>
        [Fact]
        public async Task IfUserHaveWrongNameAndCorrectPassword_ReturnFalse()
        {
            // Arrange
            var wrongUsername = "wrongname";
            var correctPassword = "admin";
            var expectedMessageContent = "false";

            // Act
            var httpResponseMessage = await _cookieService.GetCookieInternalAsync(wrongUsername, correctPassword);

            // Assert
            var messageContent = await httpResponseMessage.Content.ReadAsStringAsync();
            Assert.Equal(expectedMessageContent, messageContent);
        }

        /// <summary>
        /// Пользователь ввёл корректное имя и некорректный пароль и должен получить false
        /// </summary>
        [Fact]
        public async Task IfUserHaveCorrectNameAndWrongPassword_ReturnFalse()
        {
            // Arrange
            var correctUsername = "admin";
            var wrongPassword = "wrongpassword";
            var expectedMessageContent = "false";

            // Act
            var httpResponseMessage = await _cookieService.GetCookieInternalAsync(correctUsername, wrongPassword);
            var messageContent = await httpResponseMessage.Content.ReadAsStringAsync();
            var setCookieValue = httpResponseMessage.Headers.FirstOrDefault(h => h.Key == "Set-Cookie").Value.ToList().First();

            // Assert
            Assert.Equal(expectedMessageContent, messageContent);
            Assert.Null(setCookieValue);
        }
    }
}
