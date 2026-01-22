import { Link } from "react-router-dom";
import "../styles/pages/home.css";

export default function HomePage() {
  return (
    <div className="home-page">
      <div className="container">
        <div className="home-hero">
          <h1 className="home-title">Добро пожаловать в PMS</h1>
          <div className="home-subtitle">
            Простая и эффективная система управления проектами
          </div>

          <div className="home-features-grid">
            <div className="home-feature-card">
              <span className="home-feature-icon">👥</span>
              <h2 className="home-feature-title">Сотрудники</h2>
              <div className="home-feature-description">
                Управление командой и поиск сотрудников
              </div>
              <Link to="/employees">
                <button className="btn btnPrimary w-full">Перейти</button>
              </Link>
            </div>

            <div className="home-feature-card">
              <span className="home-feature-icon">📁</span>
              <h2 className="home-feature-title">Проекты</h2>
              <div className="home-feature-description">
                Создание, управление и отслеживание проектов
              </div>
              <Link to="/projects">
                <button className="btn btnPrimary w-full">Перейти</button>
              </Link>
            </div>
          </div>

          <div className="spacer-xl" />

          <div className="home-actions-panel">
            <h3 className="home-actions-title">Быстрые действия</h3>
            <div className="home-actions-buttons">
              <Link to="/projects/create">
                <button className="btn btnSuccess">
                  <span className="mr-2">+</span>
                  Новый проект
                </button>
              </Link>
              <Link to="/employees">
                <button className="btn btnSecondary">
                  <span className="mr-2">👤</span>
                  Добавить сотрудника
                </button>
              </Link>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}