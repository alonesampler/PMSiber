import type { ReactNode } from "react";
import { Link, useLocation } from "react-router-dom";
import "../../styles/components/layout.css";

type Props = {
  children: ReactNode;
};

const Layout = ({ children }: Props) => {
  const { pathname } = useLocation();

  const isActive = (path: string) => 
    pathname === path || pathname.startsWith(`${path}/`);

  return (
    <div className="appShell">
      {/*(Header) */}
      <nav className="layout-navbar">
        <div className="container layout-nav-content">
          <Link to="/" className="layout-nav-logo">
            <span className="text-2xl">📊</span>
            <span>PMS Pro</span>
          </Link>
          
          <div className="layout-nav-links">
            <Link 
              to="/employees" 
              className={`layout-nav-link ${isActive('/employees') ? 'active' : ''}`}
            >
              👥 Сотрудники
            </Link>
            <Link 
              to="/projects" 
              className={`layout-nav-link ${isActive('/projects') ? 'active' : ''}`}
            >
              📁 Проекты
            </Link>
          </div>
        </div>
      </nav>

      {/* main */}
      <main className="layout-main">
        <div className="container" style={{ paddingTop: 'var(--space-lg)', paddingBottom: 'var(--space-xl)' }}>
          {children}
        </div>
      </main>

      {/* Footer */}
      <footer className="layout-footer">
        <div className="container layout-footer-content">
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