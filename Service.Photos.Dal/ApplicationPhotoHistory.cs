using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service.Photos.Dal
{
    [Table("APPLICATION_PHOTO_HISTORY")]
    public class ApplicationPhotoHistory : Entity
    {
        #region Public properties

         /// <summary>
        /// Текст ошибки, выставленный сотрудниками УВиА
        /// Либо текст ошибки по коду scanChecker (см. коды ошибок scanChecker)
        /// </summary>
        [Column("ERROR_TEXT", TypeName = "NVARCHAR2")]
        [StringLength(1000)]
        public string ErrorText { get; set; }

        /// <summary>
        /// Код ошибки (см. коды ошибок scanChecker)
        /// </summary>
        [Column("ERROR_CODE")]
        [StringLength(1000)]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Признак удаления
        /// </summary>
        [Column("IS_DELETED")]
        public bool? IsDeleted { get; set; }
        /// <summary>
        /// Широта
        /// </summary>
        [Column("LATITUDE")]
        public double Latitude { get; set; }
        /// <summary>
        /// Долгота
        /// </summary>
        [Column("LONGITUDE")]
        public double Longitude { get; set; }
        /// <summary>
        /// Дата определения координат
        /// </summary>
        [DataType(DataType.Date)]
        [Column("FOUND_DATE")]
        public DateTime FoundDate { get; set; }
        /// <summary>
        /// ID типа фотографии
        /// </summary>
        [Column("PHOTO_TYPE_ID")]
        public int PhotoTypeId { get; set; }
        /// <summary>
        /// ID типа фотографии
        /// </summary>
        [Column("PARENT_PHOTO_TYPE_ID")]
        public int? ParentPhotoTypeId { get; set; }
        #endregion

        #region Связи
        /// <summary>
        /// идентификатор пакета фотографий
        /// </summary>
        [Column("PACKAGE_HISTORY_ID")]
        public long PackageHistoryId { get; set; }
        #endregion
    }
}