class Program
{
    enum Gender { Male, Female}

    enum Profession { Blacksmith, Farmer, 
        Piscal, Trader, Alchemist, Warrior, 
        Leatherworker, King, Herald, Knight, Doctor, Homelles}

    struct Person
    {
        public string Name;
        public int Health;
        public Profession Profession;
        public Gender Gender;
        public byte LuckyLevel;
        public byte Damage;
        public byte Defence;
    }

    enum Changes { Gender, Health, Profession, LuckyLevel, Damage, Defence, History, Death}

    enum Ends { Not, Good, Bad, Death }

    struct QuestFork
    {
        public QuestFork(string question, string[] answers, Changes change, byte[] consequences, Ends end)
        {
            Question = question;
            Answers = answers;
            CurrentChange = change;
            Consequences = consequences;
            End = end;
        }

        public Ends End; 

        public Changes CurrentChange = Changes.History;

        public string Question = "";
        public string[] Answers = null;

        public byte[] Consequences = null;

        public byte AnswerIndex = 0;
        public byte NextQuestIndex = 0;

        public void PrintQuestion()
        {
            Console.WriteLine(Question);
        }

        public void ChangeState(ref Person person)
        {
            switch (CurrentChange)
            {
                case Changes.Gender:
                    if (Answers[AnswerIndex].Equals("Женский"))
                    {
                        person.Gender = Gender.Female;
                    }
                    else
                    {
                        person.Gender = Gender.Male;
                    }
                    NextQuestIndex = Consequences[0];
                    break;
                case Changes.Profession:
                    person.Profession = (Profession)(AnswerIndex);
                    NextQuestIndex = Consequences[0];
                    break;
                case Changes.History:
                    NextQuestIndex = Consequences[AnswerIndex];
                    break;
            }
        }


    }


    public static void HighlightRow(string row)
    {
        Console.BackgroundColor = ConsoleColor.Yellow;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.WriteLine(row);
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
    }

    static QuestFork[] forks;

    static void BuildHistory()
    {
        forks = new QuestFork[10];

        QuestFork firstFork = new QuestFork("Выберите пол персонажа!", 
            new string[2] { "Женский", "Мужской"}, Changes.Gender, new byte[1] { 1 }, Ends.Not);
        forks[0] = firstFork;

        QuestFork secondFork = new QuestFork("Какова роль вашего персонажа?", 
            Profession.GetNames(typeof(Profession)), Changes.Profession, new byte[1] { 2 }, Ends.Not);
        forks[1] = secondFork;

        QuestFork thirdFork = new QuestFork("Гуляя по полю, вы встречаете подземелье. " +
            "Хотите ли вы в него войти?", new string[2] { "Да", "Нет"}, 
            Changes.History, new byte[2] { 3, 10 }, Ends.Not);
        forks[2] = thirdFork;

        QuestFork fourthFork = new QuestFork("Стоит ли осмотреться перед входом?", 
            new string[2] { "Да", "Нет" }, Changes.History, new byte[2] { 8, 9 }, Ends.Not);
        forks[3] = fourthFork;

        QuestFork goodEndFork = new QuestFork("Вы нашли крутой сундук и теперь вы богаты! Поздравляю=)", null,
            Changes.Death, null, Ends.Good);
        forks[8] = goodEndFork;

        QuestFork lastFork = new QuestFork("Жаль, но вы мертвы. Вы попали в ловушку и вам отрубило голову=(", null,
            Changes.Death, null, Ends.Death);
        forks[9] = lastFork;

    }

    static void Main()
    {
        BuildHistory();
        Person person = new Person();
        person.Defence = 0;
        person.Damage = 0;
        person.LuckyLevel = 100;

        Console.WriteLine("Как зовут вашего персонажа?");
        string? name = Console.ReadLine();
        person.Name = name == null ? "Денис" : name;

        Console.Clear();

        ConsoleKeyInfo key;
        sbyte curentRow = 0;
        byte currentQuestion = 0;
        do
        {
            var currentFork = forks[currentQuestion];
            if (currentFork.CurrentChange == Changes.Death)
            {
                currentFork.PrintQuestion();
                break;
            }

            Console.WriteLine(currentFork.Question);
            string[] answers = currentFork.Answers;
            for (int i = 0; i < answers.Length; i++)
            {
                if (i == curentRow)
                    HighlightRow(answers[i]);
                else
                    Console.WriteLine(answers[i]);
            }

            key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    curentRow = --curentRow > 0 ? curentRow : (sbyte)0;
                    break;
                case ConsoleKey.DownArrow:
                    curentRow = ++curentRow >= answers.Length ? 
                        (sbyte)(answers.Length - 1) : curentRow;
                    break;
                case ConsoleKey.Enter:
                    currentFork.AnswerIndex = (byte)curentRow;
                    //currentQuestion++;
                    currentFork.ChangeState(ref person);
                    currentQuestion = currentFork.NextQuestIndex;
                    curentRow = 0;
                    break;
            }
            Console.Clear();


        } while (key.Key != ConsoleKey.Escape && forks[currentQuestion].End == Ends.Not);

        var theend = forks[currentQuestion];
        theend.PrintQuestion();
        Console.WriteLine("Спасибо за игру");
        Console.ReadKey();

    }
}