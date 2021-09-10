using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.PrizeItemDtos
{
    public class PrizeItemForGetItemListDto
    {
        /// <summary>
        /// 第几页
        /// </summary>
        /// <value></value>
        [Required]
        public int Page { get; set; }

        /// <summary>
        /// 每页显示多少奖品
        /// </summary>
        /// <value></value>
        [Required]
        public int Num { get; set; }
    }
}
