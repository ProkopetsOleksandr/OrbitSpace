import { Suspense } from 'react';

import { GoalsPage } from '@/pages/goals-page';

export default function Page() {
  return (
    <Suspense>
      <GoalsPage />
    </Suspense>
  );
}
