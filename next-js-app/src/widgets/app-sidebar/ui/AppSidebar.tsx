'use client';

import { Orbit, Settings } from 'lucide-react';

import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarRail
} from '@/shared/components/ui/sidebar';
import { navItems } from '../config/nav';
import { NavGroup } from './NavGroup';
import { NavUser } from './NavUser';

export const AppSidebar = ({ ...props }: React.ComponentProps<typeof Sidebar>) => {
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
        <NavGroup label="Application" items={navItems.navMainItems} />
        <NavGroup label="To-be developed" items={navItems.navSecondaryItems} />
      </SidebarContent>
      <SidebarFooter>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton asChild tooltip="Settings">
              <div className="cursor-pointer">
                <Settings />
                Settings
              </div>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
        <NavUser user={navItems.user} />
      </SidebarFooter>
      <SidebarRail />
    </Sidebar>
  );
};
