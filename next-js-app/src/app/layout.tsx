import Providers from '@/components/layout/providers';
import '@/styles/globals.css';
import type { Metadata } from 'next';
import { Inter, Poppins } from 'next/font/google';
import DashboardLayout from '../components/layout/dashboard-layout';

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

export default function RootLayout({
  children
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <Providers>
      <html lang="en" className={`${inter.variable} ${poppins.variable} antialiased`}>
        <body className="font-inter">
          <DashboardLayout>{children}</DashboardLayout>
        </body>
      </html>
    </Providers>
  );
}
