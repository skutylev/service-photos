using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service.Photos.Dal
{
    [Table("CURRENT_PHOTO_REQUIRED")]
    public class CurrentPhotoRequired : Entity
    {
        #region Public properties
        /// <summary>
        /// ID типа фотографии
        /// </summary>
        [Required]
        [Column("PHOTO_TYPE_ID")]
        public int PhotoTypeId { get; set; }
        /// <summary>
        /// Ключ типа кредита
        /// </summary>
        [Required]
        [Column("CREDIT_TYPE_KEY")]
        public int CreditType { get; set; }
        /// <summary>
        /// Обязательность документа
        /// </summary>
        [Required]
        [Column("IS_REQUIRED")]
        public bool IsRequired { get; set; }
        /// <summary>
        /// Видимость фотографии
        /// </summary>
        [Required]
        [Column("VISIBILITY")]
        public bool Visibility { get; set; }
        /// <summary>
        /// Признак дистанционного подписания договора
        /// </summary>
        [Column("IS_REMOTE_SIGN")]
        public bool? IsRemoteSign { get; set; }
        /// <summary>
        /// Признак цифровой карты
        /// </summary>
        [Column("IS_DIGITAL")]
        public bool? IsDigital { get; set; }
        #endregion
    }
}