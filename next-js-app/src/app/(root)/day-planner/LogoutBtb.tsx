'use client';

import { logout } from '../../login/actions';

export default function LogoutBtb() {
  return (
    <div>
      <button onClick={() => logout()}>Logout</button>
    </div>
  );
}
