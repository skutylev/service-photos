using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service.Photos.Dal
{
     /// <summary>
    /// Исторические данные по пакету фотографий
    /// </summary>
    [Table("PACKAGE_PHOTO_HISTORY")]
    public class PhotoPackageHistory: Entity
    {
        #region Public properties
        /// <summary>
        /// Дата и время окончание действия записи
        /// </summary>
        [Column("END_DATE")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Идентификатор пакета фотографий
        /// </summary>
        [Column("PACKAGE_ID")]
        public long PackageId { get; set; }
        /// <summary>
        /// Дата и время утверждения комплекта фотографии
        /// </summary>
        [Column("APPROVE_DATE")]
        [DataType(DataType.Date)]
        public DateTime? ApproveDate { get; set; }
        /// <summary>
        /// Дата выставления ошибки по комплекту документов
        /// </summary>
        [Column("ERROR_DATE")]
        [DataType(DataType.Date)]
        public DateTime? ErrorDate { get; set; }
        /// <summary>
        /// ФИО сотрудника УВиА, обработавшего пакет
        /// </summary>
        [Column("FULL_NAME", TypeName = "NVARCHAR2")]
        [StringLength(365)]
        public string FullName { get; set; }
        /// <summary>
        /// Табельный номер сотрудника
        /// </summary>
        [Column("SAP_NUMBER")]
        public int SapNumber { get; set; }
        /// <summary>
        /// Стстус комплекта фотографий
        /// </summary>
        [Column("PACKAGE_STATUS_KEY")]
        public PhotoPackageStatus? PackageStatus { get; set; }
        /// <summary>
        /// Дата и время отправки комплекта документов на проверку
        /// </summary>
        [Column("SEND_PACKAGE_DATE")]
        [DataType(DataType.Date)]
        public DateTime? SendPackageDate { get; set; }
        /// <summary>
        /// Номер комплекта фотографий
        /// </summary>
        [Required]
        [Column("PACKAGE_NUMBER")]
        public int PackageNumber { get; set; }
        /// <summary>
        /// Банкир, отправивший пакет
        /// </summary>
        [Column("USER_ID")]
        public long? UserId { get; set; }

        [Column("USER_SAP_NUMBER")]
        public long? UserSapNumber { get; set; }

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
        
        #endregion

        #region Связи
        /// <summary>
        /// Фотографии к заявке
        /// </summary>
        public virtual ICollection<ApplicationPhotoHistory> ApplicationPhotosHistory { get; set; }
        #endregion
    }
}