export default function AuthLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="min-h-screen flex items-center justify-center bg-background">
      <div className="w-full max-w-md px-4">
        <div className="text-center mb-8">
          <h1 className="text-3xl font-poppins font-bold text-foreground">OrbitSpace</h1>
          <p className="text-muted-foreground mt-2">Your life operating system</p>
        </div>
        <div className="bg-card border rounded-lg p-8 shadow-sm">{children}</div>
      </div>
    </div>
  );
}
