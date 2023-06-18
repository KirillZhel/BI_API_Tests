using System.Net;
using System.Threading.Tasks;
using WebApi.Integration.Services;
using WebApi.Models;
using Xunit;

/*
* Написать тесты для метода создания сущности Course POST: /course(авторизация с пом.куки):
* - При Price = 0 метод создания должен возвращать ошибку
* - При пустом Name метод создания должен возвращать ошибку
* - При заполненных Price и Name метод создания должен завершиться успешно
*/

namespace WebApi.Integration.Tests.Homework
{
    public class PostCourseTests : IClassFixture<TestFixture>
    {
        private readonly CourseService _courseService;
        private readonly string _courseApiCookie;

        public PostCourseTests(TestFixture testFixture)
        {
            _courseService = new CourseService();
            _courseApiCookie = testFixture.AuthCookie;
        }

        /// <summary>
        /// Курс имеет нулевую цену и в ответ должна вернуться ошибка
        /// </summary>
        [Fact]
        public async Task IfPriseIsZero_ReturnBadRequesStatusCode()
        {
            // Arrange 
            var initialCourseModel = new AddCourseModel
            {
                Name = "course_name",
                Price = 0
            };

            // Act
            var response = await _courseService.AddCourseInternalAsync(initialCourseModel, _courseApiCookie);
            var responseMessage = await response.Content.ReadAsStringAsync(); //RPRY Если вам нужен такой результат, нужно сделать в CourseService соотвествующую перегрузку метода. Здесь и в других тестах 

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(Errors.Поле_Price_должно_быть_больше_нуля, responseMessage);
        }

        /// <summary>
        /// Курс имеет пустое имя и в ответ должна вернуться ошибка
        /// </summary>
        [Fact]
        public async Task IfNameIsEmpty_ReturnBadRequesStatusCode()
        {
            // Arrange 
            var initialCourseModel = new AddCourseModel
            {
                Name = "    ", //RPRY проверять пустую строку а не 4 пробела) И так же можно NULL, параметризовав тест
                Price = 1000
            };

            // Act
            var response = await _courseService.AddCourseInternalAsync(initialCourseModel, _courseApiCookie);
            var responseMessage = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(Errors.Поле_Name_не_должно_быть_пустым, responseMessage);

        }

        /// <summary>
        /// Курс имеет не пустое имя и не нулевую цену и метод должен завершиться успешно
        /// </summary>
        [Fact]
        public async Task IfNameNotEmptyAndPriceNotNull_ReturnOkStatusCode()
        {
            // Arrange 
            var initialCourseModel = new AddCourseModel
            {
                Name = "course_name",
                Price = 1000
            };

            // Act
            var response = await _courseService.AddCourseInternalAsync(initialCourseModel, _courseApiCookie);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode); //RPRY лучше запросить созданную сущность из АПИ и убедиться в том что она создана
        }

    }
}
