using System.Threading.Tasks;
using WebApi.Integration.Services;
using WebApi.Models;
using Xunit;

/*
* Написать тесты для метода удаления сущности Course DELETE: /course(авторизация с пом.куки):
* - При удалении сущности Course ее поле IsDeleted должно изменить значение на true
*/

namespace WebApi.Integration.Tests.Homework
{
    public class DeleteCourseTests : IClassFixture<TestFixture>
    {
        private readonly CourseService _courseService;
        private readonly string _courseApiCookie;

        public DeleteCourseTests(TestFixture testFixture)
        {
            _courseService = new CourseService();
            _courseApiCookie = testFixture.AuthCookie;
        }

        /// <summary>
        /// добавляется новый курс, после чего удаляется, у курса должно измениться поле Deleted с false на true
        /// </summary>
        [Fact]
        public async Task IfCourseIsDeleted_ReturnTrue()
        {
            // Arrange 
            var initialCourseModel = new AddCourseModel
            {
                Name = "deleted_course_name",
                Price = 1000
            };
            var courseId = await _courseService.AddCourseAsync(initialCourseModel, _courseApiCookie);

            // Act
            var cource = await _courseService.GetCourseInternalAsync(courseId, _courseApiCookie); //RPRY --> Arrange
            await _courseService.DeleteCourseInternalAsync(courseId, _courseApiCookie);
            var deletedCourse = await _courseService.GetCourseInternalAsync(courseId, _courseApiCookie); //RPRY --> Assert

            // Assert
            Assert.False(cource.Deleted);
            Assert.True(deletedCourse.Deleted);
        }
    }
}
