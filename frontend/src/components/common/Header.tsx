import { Link, useLocation } from "react-router-dom";

const Header = () => {
  const { pathname } = useLocation();

  const link = (to: string, label: string) => (
    <Link
      to={to}
      style={{
        padding: "8px 12px",
        borderRadius: 8,
        background: pathname === to ? "#eef2ff" : "transparent",
        textDecoration: "none",
        color: "#111",
        fontWeight: 500,
      }}
    >
      {label}
    </Link>
  );

  return (
    <div
      style={{
        borderBottom: "1px solid #eee",
        padding: "12px 24px",
        display: "flex",
        alignItems: "center",
        justifyContent: "space-between",
      }}
    >
      <Link to="/" style={{ fontWeight: 700, fontSize: 18 }}>
        PMS
      </Link>

      <div style={{ display: "flex", gap: 8 }}>
        {link("/employees", "Сотрудники")}
        {link("/projects", "Проекты")}
      </div>
    </div>
  );
};

export default Header;
