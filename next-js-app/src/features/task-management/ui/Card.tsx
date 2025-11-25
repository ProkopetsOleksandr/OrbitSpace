import { ReactNode } from 'react';

export const Card = ({ children }: { children: ReactNode }) => {
  return <div className="p-4 bg-gray-50 rounded-lg shadow-sm">{children}</div>;
};
