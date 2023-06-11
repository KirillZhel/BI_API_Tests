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
            var expectedBodyContent = "true";

            // Act
            var httpResponseMessage = await _cookieService.GetCookieInternalAsync(correctUsername, correctPassword);
            var setCookieValue = httpResponseMessage.Headers.FirstOrDefault(h => h.Key == "Set-Cookie").Value.ToList().First();
            var messageContent = await httpResponseMessage.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(expectedBodyContent, messageContent);
            Assert.NotNull(setCookieValue);
        }

        /// <summary>
        /// Пользователь ввёл корректное имя и некорректный пароль и должен получить false
        /// </summary>
        [Fact]
        public async Task IfUserHaveCorrectNameAndWrongPassword_ReturnFalse()
        {
            // Arrange
            var correctUsername = "admin";
            var wrongPassword = "admin123";
            var expectedBodyContent = "false";

            // Act
            var httpResponseMessage = await _cookieService.GetCookieInternalAsync(correctUsername, wrongPassword);
            var setCookieValue = httpResponseMessage.Headers.FirstOrDefault(h => h.Key == "Set-Cookie").Value.ToList().First();
            var messageContent = await httpResponseMessage.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(expectedBodyContent, messageContent);
            Assert.Null(setCookieValue);
        }

        /// <summary>
        /// Пользователь ввёл некорректное имя и корректный пароль и должен получить false
        /// </summary>
        [Fact]
        public async Task IfUserHaveCorrectWrongNameAndPassword_ReturnFalse()
        {
            // Arrange
            var wrongUsername = "admin123";
            var correctPassword = "admin";
            var expectedBodyContent = "false";

            // Act
            var httpResponseMessage = await _cookieService.GetCookieInternalAsync(wrongUsername, correctPassword);
            var setCookieValue = httpResponseMessage.Headers.FirstOrDefault(h => h.Key == "Set-Cookie").Value.ToList().First();
            var messageContent = await httpResponseMessage.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(expectedBodyContent, messageContent);
            Assert.Null(setCookieValue);
        }
    }
}
