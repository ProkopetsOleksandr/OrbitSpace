import type { Metadata } from 'next';
import { Inter } from 'next/font/google';
import '../globals.css';

export const metadata: Metadata = {
  title: 'OrbitSpace'
};

const inter = Inter({
  subsets: ['latin'], // Для кириллицы добавь 'cyrillic', если нужно
  weight: ['400', '500', '600', '700', '800'], // Нужные начертания
  variable: '--font-inter', // CSS-переменная для Tailwind
  display: 'swap' // Оптимизация загрузки
});

export default function RootLayout({
  children
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" className={`${inter.variable}`}>
      <head>
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
      </head>
      <body className="font-inter">
        <div className="h-screen bg-gradient-to-br from-gray-100 to-blue-50 grid place-content-center">{children}</div>
      </body>
    </html>
  );
}
