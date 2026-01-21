import type { ReactNode } from "react";
import { Link, useLocation } from "react-router-dom";

type Props = {
  children: ReactNode;
};

const Layout = ({ children }: Props) => {
  const { pathname } = useLocation();

  const isActive = (path: string) => pathname === path || pathname.startsWith(`${path}/`);

  return (
    <div className="appShell flex flex-col min-h-screen">
      <nav className="navbar">
        <div className="container nav-content">
          <Link to="/" className="nav-logo">
            <span className="text-2xl">📊</span>
            <span>PMS Pro</span>
          </Link>
          
          <div className="nav-links">
            <Link 
              to="/employees" 
              className={`nav-link ${isActive('/employees') ? 'active' : ''}`}
            >
              👥 Сотрудники
            </Link>
            <Link 
              to="/projects" 
              className={`nav-link ${isActive('/projects') ? 'active' : ''}`}
            >
              📁 Проекты
            </Link>
          </div>
        </div>
      </nav>

      <main className="flex-1">
        {children}
      </main>

      <footer className="footer">
        <div className="container footer-content">
          <div>
            <div className="font-semibold text-gray-900">Project Management System</div>
            <div className="text-sm text-gray-600 mt-1">Управление проектами и командой</div>
          </div>
          <div className="text-sm text-gray-500">
            © {new Date().getFullYear()} PMS Pro. Все права защищены.
          </div>
        </div>
      </footer>
    </div>
  );
};

export default Layout;