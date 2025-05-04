namespace TrueOrFalseGame
{

    public interface IQuestionSource
    {
        /// <summary>
        /// Загружает вопросы из источника данных
        /// </summary>
        public IEnumerable<string> NegativeAnswersArray { get;}
        public IEnumerable<string> PositiveAnswersArray { get;}
        public IEnumerable<Question> LoadQuestions();

        /// <summary>
        /// Возвращает максимальное допустимое количество ошибок
        /// </summary>

        /// <summary>
        /// Проверяет доступность источника данных
        /// </summary>
        bool IsAvailable();
    }
}