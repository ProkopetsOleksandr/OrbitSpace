import React, { useMemo } from 'react';
import { Moon, Sun, Clock, HeartPulse, Battery } from 'lucide-react';
import { cn } from '@/lib/utils';

interface LifeVisualizerProps {
  birthDate: Date;
  expectedYears: number;
  sleepHoursPerDay: number;
}

export const LifeVisualizer: React.FC<LifeVisualizerProps> = ({
  birthDate,
  expectedYears,
  sleepHoursPerDay,
}) => {
  const stats = useMemo(() => {
    const now = new Date();
    const birth = new Date(birthDate);
    const death = new Date(birth);
    death.setFullYear(birth.getFullYear() + expectedYears);

    const totalLifeMs = death.getTime() - birth.getTime();
    const livedMs = now.getTime() - birth.getTime();
    const remainingMs = death.getTime() - now.getTime();

    const livedPercentage = Math.min(100, Math.max(0, (livedMs / totalLifeMs) * 100));
    const remainingPercentage = 100 - livedPercentage;

    const yearsLived = livedMs / (1000 * 60 * 60 * 24 * 365.25);
    const yearsLeft = remainingMs / (1000 * 60 * 60 * 24 * 365.25);

    // Sleep calculations
    const sleepRatio = sleepHoursPerDay / 24;
    const yearsSlept = yearsLived * sleepRatio;
    const yearsAwake = yearsLived * (1 - sleepRatio);
    const yearsLeftToSleep = yearsLeft * sleepRatio;
    const yearsLeftAwake = yearsLeft * (1 - sleepRatio);

    return {
      livedPercentage,
      remainingPercentage,
      yearsLived,
      yearsLeft,
      yearsSlept,
      yearsAwake,
      yearsLeftToSleep,
      yearsLeftAwake,
      totalLifeMs,
    };
  }, [birthDate, expectedYears, sleepHoursPerDay]);

  // Circular Progress Calculation
  const radius = 120;
  const circumference = 2 * Math.PI * radius;
  const strokeDashoffset = circumference - (stats.livedPercentage / 100) * circumference;

  return (
    <div className="w-full max-w-4xl mx-auto p-8 bg-zinc-950 rounded-3xl shadow-2xl border border-zinc-800/50 text-zinc-100 overflow-hidden relative">
      {/* Background Glows */}
      <div className="absolute top-0 left-0 w-full h-full overflow-hidden pointer-events-none">
        <div className="absolute -top-20 -left-20 w-96 h-96 bg-purple-500/10 rounded-full blur-3xl animate-pulse" />
        <div className="absolute -bottom-20 -right-20 w-96 h-96 bg-blue-500/10 rounded-full blur-3xl animate-pulse delay-1000" />
      </div>

      <div className="relative z-10 flex flex-col md:flex-row gap-12 items-center">
        {/* Main Visual: Circular Progress */}
        <div className="relative group">
          <svg width="320" height="320" className="transform -rotate-90 transition-all duration-1000 ease-out">
            {/* Background Circle */}
            <circle
              cx="160"
              cy="160"
              r={radius}
              stroke="currentColor"
              strokeWidth="20"
              fill="transparent"
              className="text-zinc-900"
            />
            {/* Progress Circle */}
            <circle
              cx="160"
              cy="160"
              r={radius}
              stroke="url(#gradient)"
              strokeWidth="20"
              fill="transparent"
              strokeDasharray={circumference}
              strokeDashoffset={strokeDashoffset}
              strokeLinecap="round"
              className="transition-all duration-1000 ease-out drop-shadow-[0_0_10px_rgba(168,85,247,0.5)]"
            />
            <defs>
              <linearGradient id="gradient" x1="0%" y1="0%" x2="100%" y2="0%">
                <stop offset="0%" stopColor="#3b82f6" />
                <stop offset="100%" stopColor="#a855f7" />
              </linearGradient>
            </defs>
          </svg>
          
          {/* Center Text */}
          <div className="absolute inset-0 flex flex-col items-center justify-center text-center">
            <span className="text-5xl font-bold tracking-tighter bg-clip-text text-transparent bg-gradient-to-r from-blue-400 to-purple-500">
              {stats.yearsLeft.toFixed(1)}
            </span>
            <span className="text-sm text-zinc-500 uppercase tracking-widest mt-2 font-medium">Years Left</span>
          </div>
        </div>

        {/* Stats & Details */}
        <div className="flex-1 w-full space-y-8">
          <div>
            <h2 className="text-3xl font-bold mb-2 flex items-center gap-3">
              <HeartPulse className="w-8 h-8 text-red-500" />
              Life Dashboard
            </h2>
            <p className="text-zinc-400">
              Visualizing your journey based on a {expectedYears}-year expectancy.
            </p>
          </div>

          {/* Progress Bars */}
          <div className="space-y-6">
            {/* Lived vs Left */}
            <div className="space-y-2">
              <div className="flex justify-between text-sm font-medium">
                <span className="text-zinc-400">Lived</span>
                <span className="text-zinc-400">Remaining</span>
              </div>
              <div className="h-4 bg-zinc-900 rounded-full overflow-hidden flex relative">
                <div 
                  style={{ width: `${stats.livedPercentage}%` }} 
                  className="h-full bg-gradient-to-r from-blue-600 to-purple-600 relative group"
                >
                    <div className="absolute inset-0 bg-white/20 group-hover:bg-white/30 transition-colors" />
                </div>
              </div>
              <div className="flex justify-between text-xs text-zinc-500">
                <span>{stats.yearsLived.toFixed(1)} years</span>
                <span>{stats.yearsLeft.toFixed(1)} years</span>
              </div>
            </div>

            {/* Sleep Impact */}
            <div className="p-6 bg-zinc-900/50 rounded-2xl border border-zinc-800/50 backdrop-blur-sm">
              <h3 className="text-lg font-semibold mb-4 flex items-center gap-2">
                <Moon className="w-5 h-5 text-indigo-400" />
                Sleep Impact
              </h3>
              
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-1">
                  <div className="text-xs text-zinc-500 uppercase">Time Slept</div>
                  <div className="text-2xl font-mono text-indigo-300">
                    {stats.yearsSlept.toFixed(1)} <span className="text-sm text-zinc-600">yrs</span>
                  </div>
                </div>
                <div className="space-y-1">
                  <div className="text-xs text-zinc-500 uppercase">Time Awake</div>
                  <div className="text-2xl font-mono text-amber-300">
                    {stats.yearsAwake.toFixed(1)} <span className="text-sm text-zinc-600">yrs</span>
                  </div>
                </div>
                
                <div className="col-span-2 pt-4 border-t border-zinc-800">
                   <div className="flex justify-between items-end">
                      <div>
                        <div className="text-xs text-zinc-500 uppercase mb-1">Future Sleep</div>
                        <div className="text-xl font-mono text-zinc-400">{stats.yearsLeftToSleep.toFixed(1)} yrs</div>
                      </div>
                      <div className="text-right">
                        <div className="text-xs text-zinc-500 uppercase mb-1">Future Awake</div>
                        <div className="text-xl font-mono text-emerald-400">{stats.yearsLeftAwake.toFixed(1)} yrs</div>
                      </div>
                   </div>
                   
                   {/* Mini bar for future */}
                   <div className="mt-3 h-2 bg-zinc-800 rounded-full overflow-hidden flex">
                      <div style={{ width: `${(stats.yearsLeftToSleep / stats.yearsLeft) * 100}%` }} className="bg-indigo-900/50 h-full" />
                      <div style={{ width: `${(stats.yearsLeftAwake / stats.yearsLeft) * 100}%` }} className="bg-emerald-900/50 h-full" />
                   </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      {/* Footer / Quote */}
      <div className="mt-8 text-center text-zinc-600 text-sm italic border-t border-zinc-900 pt-4">
        "Time is the most valuable thing a man can spend."
      </div>
    </div>
  );
};
