using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.AddressDtos
{
    public class AddressForGetByIdDto
    {
        /// <summary>
        /// 地址 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? Id { get; set; }
    }
}
