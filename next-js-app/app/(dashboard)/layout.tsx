import { DashboardLayout } from '@/widgets/dashboard-layout';

export default function AppLayout({ children }: { children: React.ReactNode }) {
  return <DashboardLayout>{children}</DashboardLayout>;
}
