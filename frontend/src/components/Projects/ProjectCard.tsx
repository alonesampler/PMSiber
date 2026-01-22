import type { ProjectResponseDto } from "../../types/project.types";
import { Link } from "react-router-dom";

type Props = {
  project: ProjectResponseDto;
  onDelete: (id: string) => void;
};

const ProjectCard = ({ project, onDelete }: Props) => {
  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString("ru-RU");
  };

  const getProjectStatus = () => {
    const now = new Date();
    const start = new Date(project.startDate);
    const end = new Date(project.endDate);
    
    if (now < start) return { 
      text: "Запланирован", 
      color: "badge-primary",
      bgColor: "bg-blue-50",
      textColor: "text-blue-800",
      icon: "📅"
    };
    if (now > end) return { 
      text: "Завершен", 
      color: "badge-success",
      bgColor: "bg-green-50",
      textColor: "text-green-800",
      icon: "✅"
    };
    return { 
      text: "В работе", 
      color: "badge-warning",
      bgColor: "bg-yellow-50",
      textColor: "text-yellow-800",
      icon: "🚀"
    };
  };

  const status = getProjectStatus();

  const handleDelete = (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();  
    onDelete(project.id);
  };

  const handleCardClick = (e: React.MouseEvent) => {
    if ((e.target as HTMLElement).closest('.project-card-action')) {
      e.preventDefault();
    }
  };

  return (
    <Link 
      to={`/projects/${project.id}`} 
      className="project-card block"
      onClick={handleCardClick}
    >
      <div className="project-card-content">
        {/* Шапка карточки */}
        <div className="project-card-header">
          <div className="project-card-title">
            <h3 className="project-card-name">{project.name}</h3>
            <span className={`project-card-status ${status.bgColor} ${status.textColor}`}>
              <span className="status-icon">{status.icon}</span>
              {status.text}
            </span>
          </div>
          
          <div className="project-card-priority">
            <span className={`priority-value priority-${Math.ceil(project.priority / 2)}`}>
              {project.priority}/10
            </span>
          </div>
        </div>

        {/* Информация о проекте */}
        <div className="project-card-info">
          <div className="info-row">
            <span className="info-label">👤 Менеджер</span>
            <span className="info-value">
              {project.manager?.lastName} {project.manager?.firstName}
            </span>
          </div>
          
          <div className="info-row">
            <span className="info-label">🏢 Заказчик</span>
            <span className="info-value truncate">{project.customerCompanyName}</span>
          </div>
          
          <div className="info-row">
            <span className="info-label">📅 Сроки</span>
            <span className="info-value">
              {formatDate(project.startDate)} – {formatDate(project.endDate)}
            </span>
          </div>
          
          <div className="info-row">
            <span className="info-label">👥 Команда</span>
            <span className="info-value">{project.employees?.length || 0} человек</span>
          </div>
        </div>

        {/* Кнопки действий */}
        <div className="project-card-actions">
          <Link 
            to={`/projects/${project.id}/edit`}
            className="project-card-action btn btn-secondary btn-sm"
            onClick={(e) => e.stopPropagation()}
          >
            ✏️ Редактировать
          </Link>
          
          <button 
            className="project-card-action btn btn-danger btn-sm"
            onClick={handleDelete}
          >
            🗑️ Удалить
          </button>
        </div>
      </div>
    </Link>
  );
};

export default ProjectCard;