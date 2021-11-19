using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service.Photos.Dal
{
    /// <summary>
    /// Таблица с вложениями по заявке
    /// </summary>
    [Table("ATTACHMENT")]
    public class Attachment : Entity
    {
        #region Public properties

        /// <summary>
        /// Вложение
        /// </summary>
        [Column("PHOTO")]
        public byte[] Photo { get; set; }

        /// <summary>
        /// Мини-версия вложения
        /// </summary>
        [Column("MINI_PHOTO")]
        public byte[] MiniPhoto { get; set; }

        /// <summary>
        /// Связывающий идентификатор
        /// </summary>
        [Column("UUID")]
        public Guid? UUID { get; set; }

        #endregion
    }
}