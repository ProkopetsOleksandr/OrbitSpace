import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import type { Metadata } from 'next';
import { Inter, Poppins } from 'next/font/google';
import '../globals.css';
import DashboardLayout from './dashboard-layout';
import './sidebar.css';

const inter = Inter({
  subsets: ['latin'],
  weight: ['400', '500', '600', '700', '800'],
  variable: '--font-inter',
  display: 'swap'
});

const poppins = Poppins({
  subsets: ['latin'],
  weight: ['400', '600', '700'],
  variable: '--font-poppins',
  display: 'swap'
});

export const metadata: Metadata = {
  title: 'OrbitSpace'
};

const queryClient = new QueryClient();

export default function RootLayout({
  children
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" className={`${inter.variable} ${poppins.variable} antialiased`}>
      <body className="font-inter">
        <QueryClientProvider client={queryClient}>
          <DashboardLayout>{children}</DashboardLayout>
        </QueryClientProvider>
      </body>
    </html>
  );
}
