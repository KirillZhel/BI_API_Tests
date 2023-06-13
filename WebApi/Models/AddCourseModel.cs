using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    /// <summary>
    /// ДТО курса
    /// </summary>
    public class AddCourseModel
    {
        /// <summary>
        /// Название
        /// </summary>
        [MaxLength]
        public string Name { get; set; }
        
        /// <summary>
        /// Стоимость
        /// </summary>
        public decimal Price { get; set; }
    }
}