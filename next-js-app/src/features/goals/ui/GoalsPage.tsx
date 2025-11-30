import { GoalList } from './GoalList/GoalList';
import { StatusCards } from './GoalList/StatusCards';

export type Goal = {
  id: string;
  title: string;
  description: string;
  completedDate: Date | null;
  createdAt: Date;
  status: 'New' | 'Active' | 'Completed' | 'OnHold';
  deadline: Date | null;
  category: string;
};

export const goalsTestData: Goal[] = [
  {
    id: '1',
    title: 'Начать изучать итальянский B2',
    description: 'Ежедневные занятия, пройти минимум один учебник уровня B2.',
    completedDate: null,
    createdAt: new Date('2025-01-02'),
    status: 'Active',
    deadline: new Date('2025-12-01'),
    category: 'Personal Development'
  },
  {
    id: '2',
    title: 'Пробежать полумарафон',
    description: 'Тренировки 3 раза в неделю, участие в забеге в сентябре.',
    completedDate: null,
    createdAt: new Date('2025-01-05'),
    status: 'New',
    deadline: new Date('2025-09-15'),
    category: 'Health'
  },
  {
    id: '3',
    title: 'Переписать личный сайт на Next.js',
    description: 'Создать новый дизайн, добавить блог и проекты.',
    completedDate: null,
    createdAt: new Date('2025-01-10'),
    status: 'Active',
    deadline: new Date('2025-03-01'),
    category: 'Programming'
  },
  {
    id: '4',
    title: 'Сделать ремонт в гостиной',
    description: 'Покраска стен, новая мебель, освещение.',
    completedDate: null,
    createdAt: new Date('2025-02-01'),
    status: 'OnHold',
    deadline: null,
    category: 'Home'
  },
  {
    id: '5',
    title: 'Прочитать 12 книг',
    description: 'По одной книге в месяц, фиксировать заметки.',
    completedDate: null,
    createdAt: new Date('2025-01-01'),
    status: 'Active',
    deadline: new Date('2025-12-31'),
    category: 'Education'
  },
  {
    id: '6',
    title: 'Сделать медицинский чекап',
    description: 'Общий анализ крови, обследование сердца, стоматолог.',
    completedDate: null,
    createdAt: new Date('2025-03-02'),
    status: 'New',
    deadline: new Date('2025-06-01'),
    category: 'Health'
  },
  {
    id: '7',
    title: 'Научить собаку новую сложную команду',
    description: "Команда 'Прыжок через препятствие'.",
    completedDate: null,
    createdAt: new Date('2025-01-18'),
    status: 'Active',
    deadline: null,
    category: 'Pets'
  },
  {
    id: '8',
    title: 'Выучить 1000 новых итальянских слов',
    description: 'Использовать Anki ежедневно.',
    completedDate: null,
    createdAt: new Date('2025-01-03'),
    status: 'Active',
    deadline: new Date('2025-12-31'),
    category: 'Language'
  },
  {
    id: '9',
    title: 'Купить новую камеру',
    description: 'Для записи видео и фото.',
    completedDate: null,
    createdAt: new Date('2025-02-10'),
    status: 'OnHold',
    deadline: null,
    category: 'Hobby'
  },
  {
    id: '10',
    title: 'Сделать 100 видео для TikTok',
    description: 'Тематика — язык, фитнес, организация.',
    completedDate: null,
    createdAt: new Date('2025-01-07'),
    status: 'New',
    deadline: new Date('2025-12-31'),
    category: 'Content'
  },
  {
    id: '11',
    title: 'Получить повышение',
    description: 'Улучшить навыки .NET, взять больше ответственности.',
    completedDate: null,
    createdAt: new Date('2025-01-01'),
    status: 'Active',
    deadline: null,
    category: 'Career'
  },
  {
    id: '12',
    title: 'Путешествовать по Италии',
    description: 'Посетить минимум 5 новых городов.',
    completedDate: null,
    createdAt: new Date('2025-01-04'),
    status: 'New',
    deadline: new Date('2025-11-01'),
    category: 'Travel'
  },
  {
    id: '13',
    title: 'Освоить основы Docker глубже',
    description: 'Создать несколько проектов с docker-compose.',
    completedDate: null,
    createdAt: new Date('2025-02-01'),
    status: 'Active',
    deadline: new Date('2025-05-01'),
    category: 'Programming'
  },
  {
    id: '14',
    title: 'Улучшить гибкость',
    description: 'Регулярная растяжка 3 раза в неделю.',
    completedDate: null,
    createdAt: new Date('2025-01-15'),
    status: 'Active',
    deadline: null,
    category: 'Health'
  },
  {
    id: '15',
    title: 'Написать мини-курс по Notion',
    description: 'Сделать структуру, записать видео, оформить гайд.',
    completedDate: null,
    createdAt: new Date('2025-03-10'),
    status: 'New',
    deadline: new Date('2025-07-01'),
    category: 'Content'
  },
  {
    id: '16',
    title: 'Собрать портфолио проектов',
    description: '3 полноценных проекта + описание.',
    completedDate: null,
    createdAt: new Date('2025-01-20'),
    status: 'Active',
    deadline: new Date('2025-09-01'),
    category: 'Career'
  },
  {
    id: '17',
    title: 'Улучшить технику сна',
    description: 'Ложиться до 23:00, отслеживать график.',
    completedDate: new Date('2025-04-12'),
    createdAt: new Date('2025-01-01'),
    status: 'Completed',
    deadline: null,
    category: 'Health'
  },
  {
    id: '18',
    title: 'Вести дневник рефлексий ежедневно',
    description: 'Продолжить привычку и улучшить структуру записей.',
    completedDate: null,
    createdAt: new Date('2025-01-01'),
    status: 'Active',
    deadline: null,
    category: 'Personal Development'
  },
  {
    id: '19',
    title: 'Сделать 200 тренировок в зале',
    description: 'Примерно 4 тренировки в неделю.',
    completedDate: null,
    createdAt: new Date('2025-01-02'),
    status: 'Active',
    deadline: new Date('2025-12-31'),
    category: 'Health'
  },
  {
    id: '20',
    title: 'Оптимизировать домашний бюджет',
    description: 'Создать таблицу расходов, анализировать раз в месяц.',
    completedDate: null,
    createdAt: new Date('2025-02-05'),
    status: 'New',
    deadline: new Date('2025-05-01'),
    category: 'Finance'
  }
];

export const GoalsPage = () => {
  return (
    <div className="space-y-6">
      <StatusCards activeCount={6} onHoldCount={5} newCount={10} completedCount={221} />
      <GoalList />
    </div>
  );
};
