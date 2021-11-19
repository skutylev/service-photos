namespace Service.Photos.Dal
{
    public enum DocumentType
    {
        /// <summary>
        /// Нет значения
        /// </summary>
        NotSet,
        /// <summary>
        /// Основной документ
        /// </summary>
        Main,
        /// <summary>
        /// Дополнительный документ
        /// </summary>
        Additional,
        /// <summary>
        /// Паспорт гражданина РФ
        /// </summary>
        Passport,
        /// <summary>
        /// СНИЛС
        /// </summary>
        Snils
    }
}