using System;
using System.Linq;
using System.Threading;

namespace LiveChat
{
    /// <summary>
    /// Чат-бот
    /// </summary>
    public class ChatBot
    {
        /// <summary>
        /// Штука необходимая для генерации случайных чисел
        /// </summary>
        private Random _random;

        /// <summary>
        /// Имя чат-бота
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Состояние чат-бота
        /// Может быть следующим:
        /// - ничего не делает
        /// - думает
        /// - вошел в чат
        /// - вышел из чата
        /// </summary>
        private string _state;
        public string State
        {
            get => _state;
            set
            {
                _state = value;
                StateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Добавляем событие для отслеживания состояния чат-бота
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// Конструктор чат-бота
        /// </summary>
        /// <param name="name"></param>
        public ChatBot(string name)
        {
            Name = name;
            _random = new Random(DateTime.Now.Millisecond * name.GetHashCode());
            State = "ничего не делает";
        }

        /// <summary>
        /// Метод запуска чат-бота (с ограничением количества попыток подбора слова)
        /// Работа чат-бота выполняется в отдельном потоке!
        /// </summary>
        /// <param name="chat">Чат в который мы хотим запустить чат-бота</param>
        /// <param name="attemptsCount">Количество попыток подбора слова</param>
        public void Start(Chat chat, int attemptsCount)
        {
            // Создаем новый поток и тут же его запускаем            
            new Thread(() =>
            {
                try
                {
                    // Входим в чат
                    chat.Enter();
                    State = "вошел в чат";
                    for (var i = 0; i < attemptsCount; i++)
                    {
                        State = "думает";
                        var newWord = FindRandomWordByFirstChar(chat.LastChar); 
                        chat.TryAddWord(this, newWord);
                    }
                }
                finally
                {
                    // После завершения работы выходим из чата
                    State = "вышел из чата";
                    chat.Leave();
                }
            }).Start();
        }

        /// <summary>
        /// Метод поиска случайного слова, которое начинается на указанную букву
        /// </summary>
        /// <param name="firstChar">Первая буква слова</param>
        /// <returns>Слово на букву firstChar</returns>
        private string FindRandomWordByFirstChar(char firstChar)
        {
            try
            {
                // Алгоритм не самый оптимальный (не важно для занятия)
                var words = Program.Words.Where(w => w.StartsWith(firstChar)).ToArray();
                var index = _random.Next(0, words.Length);
                return words[index];
            }
            catch
            {
                return null;
            }
        }
    }
}
