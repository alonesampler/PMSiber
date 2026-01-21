import { Link } from "react-router-dom";

export default function HomePage() {
  return (
    <div className="container">
      <div className="page" style={{ textAlign: "center", paddingTop: "60px" }}>
        <h1 className="h1" style={{ marginBottom: 16 }}>Добро пожаловать в PMS</h1>
        <div className="sub" style={{ fontSize: 18, marginBottom: 40 }}>
          Простая и эффективная система управления проектами
        </div>

        <div className="grid" style={{ gridTemplateColumns: "repeat(2, 1fr)", maxWidth: 600, margin: "0 auto" }}>
          <div className="card" style={{ padding: 24, textAlign: "center" }}>
            <h2 className="h2" style={{ marginBottom: 12 }}>👥 Сотрудники</h2>
            <div className="muted" style={{ marginBottom: 20 }}>
              Управление командой и поиск сотрудников
            </div>
            <Link to="/employees">
              <button className="btn btnPrimary">Перейти</button>
            </Link>
          </div>

          <div className="card" style={{ padding: 24, textAlign: "center" }}>
            <h2 className="h2" style={{ marginBottom: 12 }}>📁 Проекты</h2>
            <div className="muted" style={{ marginBottom: 20 }}>
              Создание, управление и отслеживание проектов
            </div>
            <Link to="/projects">
              <button className="btn btnPrimary">Перейти</button>
            </Link>
          </div>
        </div>

        <div className="spacer" />

        <div className="panel" style={{ maxWidth: 800, margin: "0 auto" }}>
          <h3 className="h3" style={{ marginBottom: 16 }}>Быстрые действия</h3>
          <div className="row" style={{ justifyContent: "center", gap: 16 }}>
            <Link to="/projects/create">
              <button className="btn btnSuccess">+ Новый проект</button>
            </Link>
            <Link to="/employees">
              <button className="btn">Добавить сотрудника</button>
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}