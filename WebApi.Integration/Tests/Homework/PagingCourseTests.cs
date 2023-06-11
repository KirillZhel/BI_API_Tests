using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Integration.Services;
using WebApi.Models;
using Xunit;

/*
* Написать тесты для метода постраничного получения Course GET: /course/list/{page}/{itemsPerPage} (авторизация с пом. куки):
- список сущностей должен быть упорядочен по идентификатору id
- пейджинг (комбинация полей page и itemsPerPage) должен работать корректно (например, при наличии 4х сущностей постраничный запрос по 2 элемента на странице первой и второй страниц списка должны дать корректные данные)
*/

namespace WebApi.Integration.Tests.Homework
{
    public class PagingCourseTests : IClassFixture<TestFixture>
    {
        private readonly CourseService _courseService;
        private readonly string _courseApiCookie;

        public PagingCourseTests(TestFixture testFixture)
        {
            _courseService = new CourseService();
            _courseApiCookie = testFixture.AuthCookie;
        }
        

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

        //!!! попробовать в автомапере отключить айдишники !!!

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
