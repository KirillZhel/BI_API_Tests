using System.Net;
using System.Threading.Tasks;
using WebApi.Integration.Services;
using WebApi.Models;
using Xunit;

/*
* Написать тесты для метода редактирования сущности Course PUT: /course(авторизация с пом.куки):
* - При редактировании полей новые значения полей name, price должны измениться
* - Метод создания должен завершиться успешно
*/

namespace WebApi.Integration.Tests.Homework
{
    public class PutCourseTests : IClassFixture<TestFixture>
    {
        private readonly CourseService _courseService;
        private readonly string _courseApiCookie;

        public PutCourseTests(TestFixture testFixture)
        {
            _courseService = new CourseService();
            _courseApiCookie = testFixture.AuthCookie;
        }

        /// <summary>
        /// добавляется новый курс, после чего изменяется его имя и цена, метод должен завершиться без ошибки
        /// </summary>
        [Fact]
        public async Task IfUpdateCourseById_ReturnCourseWithNewNameAndPriceAndOkStatusCode()
        {
            // Arrange 
            var initialCourseModel = new AddCourseModel
            {
                Name = "old_course_name",
                Price = 1000
            };
            var courseId = await _courseService.AddCourseAsync(initialCourseModel, _courseApiCookie);

            // Act
            var updatedCourseModel = new AddCourseModel() //RPRY в этом блоке должен быть только вызов метода. Создание модели - в Arrange, вызов метода - GetCourseInternalAsync - в Arrange    
            {
                Name = "new_course_name",
                Price = 10000
            };
            var updateResponse = await _courseService.UpdateCourseInternalAsync(courseId, updatedCourseModel, _courseApiCookie);
            var updatedCourse = await _courseService.GetCourseInternalAsync(courseId);

            // Assert
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            Assert.Equal(updatedCourseModel.Name, updatedCourse.Name);
            Assert.Equal(updatedCourseModel.Price, updatedCourse.Price);
        }
    }
}
