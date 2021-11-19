using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service.Photos.Dal
{
    /// <summary>
    /// Комплекты фотографий сделанные курьером по заявке
    /// </summary>
    [Table("PACKAGE_PHOTO")]
    public class PhotoPackage : Entity
    {
        #region Public properties
        
        /// <summary>
        /// Номер комплекта фотографий
        /// </summary>
        [Required]
        [Column("PACKAGE_NUMBER")]
        public int PackageNumber { get; set; }
        /// <summary>
        /// Ссылка на комплект фотографий для сотрудников УВиА
        /// </summary>
        [Required]
        [Column("PACKAGE_URL", TypeName = "NVARCHAR2")]
        [StringLength(1000)]
        public string PackageUrl { get; set; }
        /// <summary>
        /// Дата и время отправки комплекта документов на проверку
        /// </summary>
        [Column("SEND_PACKAGE_DATE")]
        [DataType(DataType.Date)]
        public DateTime? SendPackageDate { get; set; }
        /// <summary>
        /// Дата и время утверждения комплекта фотографии
        /// </summary>
        [Column("APROVE_DATE")]
        [DataType(DataType.Date)]
        public DateTime? AproveDate { get; set; }
        /// <summary>
        /// Дата выставления ошибки по комплекту документов
        /// </summary>
        [Column("ERROR_DATE")]
        [DataType(DataType.Date)]
        public DateTime? ErrorDate { get; set; }
        /// <summary>
        /// Дата и время исправления ошибки курьером
        /// </summary>
        [Column("ERROR_FIX_DATE")]
        [DataType(DataType.Date)]
        public DateTime? ErrorFixDate { get; set; }

        /// <summary>
        /// Дата обновления
        /// </summary>
        [Column("UPDATE_DATE")]
        [DataType(DataType.Date)]
        public DateTime? UpdateDate { get; set; }
        /// <summary>
        /// Статус комплекта фотографий
        /// </summary>
        [Column("PACKAGE_STATUS_KEY")]
        public PhotoPackageStatus? PackageStatus { get; set; }

        [Column("IS_PHOTO_SEND")]
        public bool IsPhotoSend { get; set; }

        /// <summary>
        /// Идентификатор запроса проверки комплекта на ScanChecker
        /// </summary>
        [Column("REQUEST_ID")]
        public Guid? RequestId { get; set; }

        /// <summary>
        /// Текст ошибки при проверке ко всему комплекту
        /// </summary>
        [Column("ERROR_TEXT")]
        [StringLength(2000)]
        public string ErrorText { get; set; }

        /// <summary>
        /// Описание ошибки
        /// </summary>
        [Column("ERROR_MESSAGE")]
        [StringLength(2000)]
        public string ErrorMessage { get; set; }

        [Column("TYPE")]
        public PackageType Type { get; set; }
        #endregion

        #region Связи
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [Column("USER_ID")]
        public long UserId { get; set; }
        
        /// <summary>
        /// Фотографии к заявке
        /// </summary>
        public virtual ICollection<ApplicationPhoto> ApplicationPhotos { get; set; }

        [Column("APPL_ID")]
        public long ApplicationId { get; set; }

        [Column("USER_SAP_NUMBER")]
        public long UserSapNumber { get; set; }

        #endregion
    }
}