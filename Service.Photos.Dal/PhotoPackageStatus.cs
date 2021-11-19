using System;

namespace Service.Photos.Dal
{
    /// <summary>
    /// Статус пакета фотографий
    /// </summary>
    public enum PhotoPackageStatus
    {
        /// <summary>
        /// Нет значения
        /// </summary>
        NotSet = 0,
        /// <summary>
        /// Произошла ошибка при отправке фотографий УВиА
        /// </summary>
        [Obsolete]
        SendError = 1,
        /// <summary>
        /// Фотографии находятся на проверке
        /// </summary>
        Sent = 2,
        /// <summary>
        /// Фотографии УВиА прверил и принял
        /// </summary>
        Accepted = 3,
        /// <summary>
        /// Фотографии отклонены УВиА
        /// </summary>
        Rejected = 4,
        /// <summary>
        /// Фотографии находятся на проверке, но не были проверены до конца дня
        /// </summary>
        [Obsolete]
        Unknown = 5
    }
}