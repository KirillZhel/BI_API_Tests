using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Берётся первая страница курсов, копируется и сортируется по Id. В результате отсортированный и неотсортированный списки должны быть равны
        /// </summary>
        [Fact]
        public async Task CoursePageIsTaken_ReturnSortedListOfCourses()
        {
            // Arrange
            var page = 1;
            var itemsPerPage = 20;

            // Act
            var courseOnPage = await _courseService.GetPageCourseAsync(page, itemsPerPage, _courseApiCookie);
            var sortedByIdPage = courseOnPage.OrderBy(c => c.Id).ToList(); //RPRY --> Assert

            // Assert
            Assert.Equal(sortedByIdPage, courseOnPage);
        }

        /// <summary> //RPRY если это описание для меня то ОК, но вообще здесь пишется краткая емкая фраза без деталей. "При таких-то условиях вызов такого-то метода должен дать такой-то эффект". Здесь и в других тестах.
        /// Берётся случайное значение для itemsPerPage, после чего рассчитывается сколько на каждой странице элементов должно быть. Проверяется количество на первой, последней и случайной странице
        /// </summary>
        [Fact]
        public async Task TakenAnyPage_ReturnCorrectCountCourse()
        {
            // Arrange
            Random rnd = new Random();
            var courseCount = (await _courseService.GetPageCourseAsync(1, 1000000, _courseApiCookie)).Count;
            var itemsPerPage = rnd.Next(1, 20);
            var expectedCountCoursePerPage = Enumerable.Repeat(itemsPerPage, courseCount / itemsPerPage).ToList();
            expectedCountCoursePerPage.Add(courseCount % itemsPerPage); //RPRY ненужная сложность. Можно задать константное itemsPerPage от этого проверка не пострадает
            var randomPage = rnd.Next(2, expectedCountCoursePerPage.Count - 1);

            // Act //RPRY в Act один вызов, иначе сложно понять что происходит.
            var courseOnFirstPage = await _courseService.GetPageCourseAsync(1, itemsPerPage, _courseApiCookie);
            var courseOnLastPage = await _courseService.GetPageCourseAsync(expectedCountCoursePerPage.Count, itemsPerPage, _courseApiCookie);
            var courseOnRandomPage = await _courseService.GetPageCourseAsync(randomPage, itemsPerPage, _courseApiCookie);

            // Assert
            Assert.Equal(expectedCountCoursePerPage.First(), courseOnFirstPage.Count);
            Assert.Equal(expectedCountCoursePerPage[randomPage], courseOnRandomPage.Count);
            Assert.Equal(expectedCountCoursePerPage.Last(), courseOnLastPage.Count);
        }
    }
}
