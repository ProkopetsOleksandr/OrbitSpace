import {
  Activity,
  Book,
  Brain,
  Calendar,
  CheckCircle,
  Home,
  ListTodo,
  LucideIcon,
  PieChart,
  Settings,
  Target
} from 'lucide-react';

export interface NavItem {
  title: string;
  url: string;
  icon: LucideIcon;
}

export const navItems: {
  navMainItems: NavItem[];
  navSecondaryItems: NavItem[];
  navBottomItems: NavItem[];
  user: { name: string; email: string; avatar: string };
} = {
  navMainItems: [
    {
      title: 'Dashboard',
      url: '/',
      icon: Home
    },
    {
      title: 'Tasks',
      url: '/task-management',
      icon: ListTodo
    }
  ],
  navSecondaryItems: [
    {
      title: 'Goals',
      url: '/goals',
      icon: Target
    },
    {
      title: 'Activities',
      url: '/activities',
      icon: Activity
    },
    {
      title: 'Calendar',
      url: '/calendar',
      icon: Calendar
    },
    {
      title: 'Habits',
      url: '/habits',
      icon: CheckCircle
    },
    {
      title: 'Books',
      url: '/books',
      icon: Book
    },
    {
      title: 'Statistics',
      url: '/statistics',
      icon: PieChart
    },
    {
      title: 'Reflection',
      url: '/reflection',
      icon: Brain
    }
  ],
  navBottomItems: [{ title: 'Settings', url: '/settings', icon: Settings }],
  user: {
    name: 'shadcn',
    email: 'm@example.com',
    avatar: '/avatars/shadcn.jpg'
  }
};
