'use client';

import { cn } from '@/shared/lib/utils';
import { Badge } from '@/shared/ui/badge';
import { Button } from '@/shared/ui/button';
import { Popover, PopoverContent, PopoverTrigger } from '@/shared/ui/popover';
import { Separator } from '@/shared/ui/separator';
import { Check, PlusCircle } from 'lucide-react';
import { useState } from 'react';

interface DataTableFacetedFilterProps {
  title: string;
  options: { label: string; value: string; icon?: React.ComponentType<any>; className?: string }[];
}

export function DataTableFacetedFilter({ title, options }: DataTableFacetedFilterProps) {
  const [selectedValues, setSelectedValues] = useState<Set<string>>(new Set());

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button variant="outline" size="sm" className="h-8 border-dashed">
          <PlusCircle />
          {title}
          {selectedValues?.size > 0 && (
            <>
              <Separator orientation="vertical" className="mx-2 h-4" />
              <Badge variant="secondary" className="rounded-sm px-1 font-normal lg:hidden">
                {selectedValues.size}
              </Badge>
              <div className="hidden space-x-1 lg:flex">
                {selectedValues.size > 2 ? (
                  <Badge variant="secondary" className="rounded-sm px-1 font-normal">
                    {selectedValues.size} selected
                  </Badge>
                ) : (
                  options
                    .filter(option => selectedValues.has(option.value))
                    .map(option => (
                      <Badge variant="secondary" key={option.value} className="rounded-sm px-1 font-normal">
                        {option.label}
                      </Badge>
                    ))
                )}
              </div>
            </>
          )}
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-[200px] p-2" align="start">
        {options.map(option => {
          const isSelected = selectedValues.has(option.value);
          return (
            <div
              key={option.value}
              onClick={() => {
                if (isSelected) {
                  selectedValues.delete(option.value);
                } else {
                  selectedValues.add(option.value);
                }

                const filterValues = Array.from(selectedValues);
                setSelectedValues(new Set(filterValues.length ? filterValues : []));
              }}>
              <div className="flex items-center cursor-pointer hover:bg-accent hover:text-accent-foreground rounded-sm p-1">
                <div
                  className={cn(
                    'mr-2 flex h-4 w-4 items-center justify-center rounded-sm border border-primary',
                    isSelected ? 'bg-primary text-primary-foreground' : 'opacity-50 [&_svg]:invisible'
                  )}>
                  <Check />
                </div>
                {option.icon && <option.icon className={cn('mr-2 h-4 w-4 text-muted-foreground', option.className)} />}
                <span>{option.label}</span>
              </div>
            </div>
          );
        })}
      </PopoverContent>
    </Popover>
  );
}
