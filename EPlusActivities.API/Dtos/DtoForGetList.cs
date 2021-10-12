using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos
{
    public class DtoForGetList
    {
        /// <summary>
        /// 第几页
        /// </summary>
        /// <value></value>
        [Range(1, int.MaxValue)]
        [Required]
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页多少个
        /// </summary>
        /// <value></value>
        [Range(0, int.MaxValue)]
        [Required]
        public int PageSize { get; set; }
    }
}
