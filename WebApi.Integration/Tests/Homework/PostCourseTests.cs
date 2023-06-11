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

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
                Name = "    ",
                Price = 0
            };

            // Act
            var response = await _courseService.AddCourseInternalAsync(initialCourseModel, _courseApiCookie);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
