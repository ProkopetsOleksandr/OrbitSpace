import { Card, CardContent, CardTitle } from '@/shared/ui/card';

interface StatusCardProps {
  title: string;
  count: number;
  icon?: React.ReactNode;
  variant: 'new' | 'active' | 'completed' | 'onHold';
}

const variantStyles = {
  new: 'border-blue-500 bg-blue-50 dark:bg-blue-950/20',
  active: 'border-amber-500 bg-amber-50 dark:bg-amber-950/20',
  completed: 'border-green-500 bg-green-50 dark:bg-green-950/20',
  onHold: 'border-gray-500 bg-gray-50 dark:bg-gray-950/20'
};

const variantTextStyles = {
  new: 'text-blue-700 dark:text-blue-400',
  active: 'text-amber-700 dark:text-amber-400',
  completed: 'text-green-700 dark:text-green-400',
  onHold: 'text-gray-700 dark:text-gray-400'
};

export const GoalStatusCard = ({ title, count, icon, variant }: StatusCardProps) => {
  return (
    <Card className={`border-l-4 ${variantStyles[variant]}`}>
      <CardContent>
        <div className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="text-sm font-medium">{title}</CardTitle>
          {icon && <div className={variantTextStyles[variant]}>{icon}</div>}
        </div>
        <div className={`text-2xl font-bold ${variantTextStyles[variant]}`}>{count}</div>
      </CardContent>
    </Card>
  );
};
