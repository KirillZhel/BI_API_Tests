using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebApi.Integration.Services;
using WebApi.Models;
using Xunit;

namespace WebApi.Integration.Tests.Homework
{
    public class MyTests : IClassFixture<TestFixture>
    {
        private readonly LessonService _lessonService;
        private readonly CourseService _courseService;
        private readonly string _lessonApiToken;
        private readonly string _courseApiCookie;

        private readonly CookieService _cookieService;

        public MyTests(TestFixture testFixture)
        {
            _lessonService = new LessonService();
            _courseService = new CourseService();
            _lessonApiToken = testFixture.Token;
            _courseApiCookie = testFixture.AuthCookie;

            _cookieService = new CookieService();
        }

        /*
         * Написать тесты на метод получения куки /Auth/Login (см. сервис CookieAuthService). 
         * - При корректных кредах (admin, admin) метод должен вернуть true и куки
         * - При некорректных кредах метод должен вернуть false
         */
        [Fact]
        public async Task FirstTest_GetCookie()
        {
            // Arrange
            var correctUsername = "admin";
            var correctPassword = "admin";
            // Act
            var httpResponseMessage = await _cookieService.GetCookieInternalAsync("admin", "admin");
            var setCookieValue = httpResponseMessage.Headers.FirstOrDefault(h => h.Key == "Set-Cookie").Value.ToList().First();
            var body = await httpResponseMessage.Content.ReadAsStringAsync();
            // Assert
            Assert.Equal("true", body);
            Assert.NotNull(setCookieValue);
        }

        /*
         * Написать тесты для метода создания сущности Course POST:  /course(авторизация с пом.куки) :
         * - При Price = 0 метод создания должен возвращать ошибку
         * - При пустом Name метод создания должен возвращать ошибку
         * - При заполненных Price и Name метод создания должен завершиться успешно
        */
        [Fact]
        public async Task SecondTest_PostCourse()
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

        [Fact]
        public async Task SecondTest_PostCourse_3()
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

        [Fact]
        public async Task SecondTest_PostCourse_2()
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

        /*
         * Написать тесты для метода редактирования сущности Course PUT:  /course(авторизация с пом.куки) :
         * - При редактировании полей новые значения полей name,
         * price должны измениться метод создания должен завершиться успешно
         */

        [Fact]
        public async Task ThirdTest_PutCourse()
        {
            // Arrange 
            var initialCourseModel = new AddCourseModel
            {
                Name = "course_name",
                Price = 1000
            };
            var courseId = await _courseService.AddCourseAsync(initialCourseModel, _courseApiCookie);

            // Act
            var updatedCourseModel = new AddCourseModel
            {
                Name = "course_name_2",
                Price = 10000
            };
            var oldCourse = await _courseService.GetCourseInternalAsync(courseId);
            var updateResponse = await _courseService.UpdateCourseInternalAsync(courseId, updatedCourseModel, _courseApiCookie);
            var newCourse = await _courseService.GetCourseInternalAsync(courseId);
            // Assert
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            Assert.Equal(updatedCourseModel.Name, newCourse.Name);
            Assert.Equal(updatedCourseModel.Price, newCourse.Price);
        }

        /*
         * Написать тесты для метода удаления сущности Course DELETE:  /course(авторизация с пом.куки) :
         * - При удалении сущности Course ее поле IsDeleted должно изменить значение на true
        */
        [Fact]
        public async Task FourthTest_DeleteCourse()
        {
            // Arrange 
            var initialCourseModel = new AddCourseModel
            {
                Name = "course_name",
                Price = 1000
            };

            var courseId = await _courseService.AddCourseAsync(initialCourseModel, _courseApiCookie);
            var course = await _courseService.GetCourseInternalAsync(courseId);

            await _courseService.DeleteCourseInternalAsync(courseId, _courseApiCookie);

            var deletedCourse = await _courseService.GetCourseInternalAsync(courseId, _courseApiCookie);

            Assert.True(deletedCourse.Deleted);
        }

        /*
         * Написать тесты для метода постраничного получения Course
            GET:  /course/list/{page}/{itemsPerPage} (авторизация с пом. куки):
            - список сущностей должен быть упорядочен по идентификатору id <<<< взять список, сделать копию и отсортировать её -> сравнить отсортированный(ожидаемый) и не отсортированный(актуальный)
            - пейджинг (комбинация полей page и itemsPerPage) должен работать корректно (например, при наличии
        4х сущностей постраничный запрос по 2 элемента на странице первой и второй страниц списка должны дать корректные данные)
         */

        [Fact]
        public async Task FifthTest_GetPage()
        {
            // Arrange

            // Act
            var page = await _courseService.GetPageCourseAsync(2, 20, _courseApiCookie);
            var copyPage = new List<CourseModel>(page);

            // Assert
            Assert.Equal(copyPage, page);
        }

        // реализовать метод (смотри метод GetCourseInternalAsync в CourseService) пейджинга на уровне CourseApiClient(namespace WebApi.Integration.Services;) и CourseService(namespace WebApi.Integration.Services;)
        // ответ на запрос будет являться списком, нужно это учесть при десериализации
        // перед тестом или в тесте создать предварительно несколько курсов (можно в цикле), чтобы было доступно несколько страниц
        // проверки: запрос первой страницы + кол-во элементов менее количества существующих элементов
        // переключение страницы + кол-во элементов больше доступного
        // комбинация страницы и кол-ва невалидное (так, чтобы заданной страницы не существовало). Например, при общем кол-ве элементов 15, задать страницу 2 и кол-во элементов 20
        // вместо числовых значений параметров передать нечисловые
        // передать нулевое значение страницы или кол-ва элементов
        // передать негативное значение параметров
    }
}
