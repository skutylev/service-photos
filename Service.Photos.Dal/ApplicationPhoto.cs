using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service.Photos.Dal
{
    [Table("APPLICATION_PHOTO")]
    public class ApplicationPhoto : Entity
    {
        #region Public properties

        /// <summary>
        /// Дата и время обновления записи
        /// </summary>
        [Column("UPDATE_DATE")]
        [DataType(DataType.Date)]
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// Текст ошибки, выставленный сотрудником УВиА при просмотре комплекта документов.
        ///Либо текст ошибки по коду scanChecker
        /// </summary>
        [Column("ERROR_TEXT", TypeName = "NVARCHAR2")]
        [StringLength(1000)]
        public string ErrorText { get; set; }

        /// <summary>
        /// Код ошибки (scanChecker)
        /// </summary>
        [Column("ERROR_CODE")]
        [StringLength(1000)]
        public string ErrorCode { get; set; }

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
        /// <summary>
        /// Признак удаления
        /// </summary>
        [Column("IS_DELETED")]
        public bool? IsDeleted { get; set; }
        /// <summary>
        /// Признак сохранения (отправки) на скан-сервер
        /// </summary>
        [Column("IS_SAVED")]
        public bool? IsSaved { get; set; }

        /// <summary>
        /// Система создания прикрепления
        /// </summary>
        [Column("ATTACHED_BY")]
        [DefaultValue(AttachmentSystem.Mobile)]
        public AttachmentSystem AttachedBy { get; set; }
        #endregion

        #region Связи
        /// <summary>
        /// Идентификатор пакета фото
        /// </summary>
        [Column("PACKAGE_PHOTO_ID")]
        public long PhotoPackageId { get; set; }
        /// <summary>
        /// Ссылка на ATTACHMENT.ID
        /// </summary>
        [Column("ATTACHMENT_ID")]
        public long AttachmentId { get; set; }

        /// <summary>
        /// Вложения по заявке
        /// </summary>
        public virtual Attachment Attachment { get; set; }
        #endregion
    }
}