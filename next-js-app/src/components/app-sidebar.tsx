'use client';

import { NavUser } from '@/components/nav-user';
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarRail
} from '@/components/ui/sidebar';
import { Activity, Book, Brain, Calendar, CheckCircle, Home, ListTodo, Orbit, PieChart, Target } from 'lucide-react';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import * as React from 'react';

// This is sample data.
const data = {
  user: {
    name: 'shadcn',
    email: 'm@example.com',
    avatar: '/avatars/shadcn.jpg'
  }
};

export function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {
  const pathname = usePathname();

  const applicationItems = [
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
  ];

  const tobeDevelopedItems = [
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
  ];

  return (
    <Sidebar collapsible="icon" {...props}>
      <SidebarHeader>
        <SidebarMenu>
          <SidebarMenuButton
            size="lg"
            className="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground">
            <div className="bg-sidebar-primary text-sidebar-primary-foreground flex aspect-square size-8 items-center justify-center rounded-lg">
              <Orbit className="size-4" />
            </div>
            <div className="leading-tight">
              <span className="truncate font-medium">Orbit Space</span>
            </div>
          </SidebarMenuButton>
        </SidebarMenu>
      </SidebarHeader>
      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupLabel>Application</SidebarGroupLabel>
          <SidebarGroupContent>
            <SidebarMenu>
              {applicationItems.map(item => (
                <SidebarMenuItem key={item.title}>
                  <SidebarMenuButton asChild isActive={pathname === item.url} tooltip={item.title}>
                    <Link href={item.url}>
                      <item.icon />
                      <span>{item.title}</span>
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              ))}
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>

        <SidebarGroup>
          <SidebarGroupLabel>To-be developed</SidebarGroupLabel>
          <SidebarGroupContent>
            <SidebarMenu>
              {tobeDevelopedItems.map(item => (
                <SidebarMenuItem key={item.title}>
                  <SidebarMenuButton asChild isActive={pathname === item.url} tooltip={item.title}>
                    <Link href={item.url}>
                      <item.icon />
                      <span>{item.title}</span>
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              ))}
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>
      <SidebarFooter>
        <NavUser user={data.user} />
      </SidebarFooter>
      <SidebarRail />
    </Sidebar>
  );
}
