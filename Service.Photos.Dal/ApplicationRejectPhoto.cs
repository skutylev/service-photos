using System.ComponentModel.DataAnnotations.Schema;

namespace Service.Photos.Dal
{
    [Table("PHOTOS")]
    public class ApplicationRejectPhoto : Entity
    {
        #region Public properties

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
        /// Тип прикрепления
        /// </summary>
        [Column("TYPE")]
        public AttachmentType Type { get; set; } = AttachmentType.Reject;

        /// <summary>
        /// Идентификатор родительского элемента
        /// </summary>
        [Column("PARENT_ID")]
        public long ApplicationId { get; set; }

        #endregion

        #region Связи
        /// <summary>
        /// Ссылка на ATTACHMENT.ID
        /// </summary>
        [Column("ATTACHMENT_ID")]
        public long AttachmentId { get; set; }

        /// <summary>
        /// Прикрепление
        /// </summary>
        public virtual Attachment Attachment { get; set; }
        #endregion
    }
}