import { LifeVisualizer } from '@/features/dashboard/ui/LifeVisualizer';
import React from 'react';

export default function DashboardPage() {
  // Example data - in a real app this would come from a user profile or settings
  const birthDate = new Date('1990-06-15');
  const expectedYears = 80;
  const sleepHoursPerDay = 7.5;

  return (
    <div className="min-h-screen bg-black text-white p-8 flex flex-col items-center justify-center">
      <div className="w-full max-w-5xl space-y-8">
        <div className="text-center space-y-2">
          <h1 className="text-4xl font-extrabold tracking-tight lg:text-5xl bg-clip-text text-transparent bg-gradient-to-b from-white to-zinc-500">
            Your Life Orbit
          </h1>
          <p className="text-zinc-400 max-w-2xl mx-auto">
            A perspective on time, sleep, and the journey ahead.
          </p>
        </div>
        
        <LifeVisualizer 
          birthDate={birthDate} 
          expectedYears={expectedYears} 
          sleepHoursPerDay={sleepHoursPerDay} 
        />

        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mt-12 text-zinc-500 text-sm text-center">
           <div className="p-4 rounded-xl bg-zinc-900/30 border border-zinc-800">
              <div className="font-semibold text-zinc-300 mb-1">Birth Date</div>
              {birthDate.toLocaleDateString()}
           </div>
           <div className="p-4 rounded-xl bg-zinc-900/30 border border-zinc-800">
              <div className="font-semibold text-zinc-300 mb-1">Expectancy</div>
              {expectedYears} Years
           </div>
           <div className="p-4 rounded-xl bg-zinc-900/30 border border-zinc-800">
              <div className="font-semibold text-zinc-300 mb-1">Daily Sleep</div>
              {sleepHoursPerDay} Hours
           </div>
        </div>
      </div>
    </div>
  );
}
