import { Collapsible, CollapsibleContent } from '@/components/ui/collapsible';
import { CollapsibleTrigger } from '@radix-ui/react-collapsible';
import { ChevronRight, Square } from 'lucide-react';

const tasks = [
  {
    id: 1,
    title: 'My main task of the day',
    additionalInfo: ['Pomodoro', '3/10']
  },
  {
    id: 2,
    title: 'Other important task',
    additionalInfo: ['15:30 - 17:30']
  },
  {
    id: 3,
    title: 'Italian lesson',
    additionalInfo: ['18:00 - 19:00']
  }
];

export default function Tasks() {
  return (
    <ul className="space-y-3">
      {tasks.map(task => (
        <li key={task.id} className="bg-white p-3 rounded-md shadow-sm hover:bg-gray-50 transition-colors duration-200">
          <div className="text-gray-700 font-medium mb-2">{task.title}</div>
          <div className="flex gap-2">
            {task.additionalInfo.map(info => (
              <span className="text-sm text-gray-500 bg-gray-100 px-2 rounded-full">{info}</span>
            ))}
          </div>
        </li>
      ))}
    </ul>
  );
}
