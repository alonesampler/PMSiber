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
    
    if (now < start) return { text: "Запланирован", color: "badge-primary" };
    if (now > end) return { text: "Завершен", color: "badge-success" };
    return { text: "В работе", color: "badge-warning" };
  };

  const status = getProjectStatus();

  return (
    <div className="card">
      <div className="flex justify-between items-start mb-4">
        <h3 className="h3">{project.name}</h3>
        <span className={`badge ${status.color}`}>{status.text}</span>
      </div>

      <div className="space-y-2">
        <p>
          <span className="muted">Заказчик:</span> {project.customerCompanyName}
        </p>
        <p>
          <span className="muted">Исполнитель:</span> {project.executorCompanyName}
        </p>
        <p>
          <span className="muted">Менеджер:</span>{" "}
          {project.manager?.lastName} {project.manager?.firstName}
        </p>
        <p>
          <span className="muted">Приоритет:</span>{" "}
          <span className={`font-semibold ${project.priority >= 8 ? 'text-red-600' : project.priority >= 5 ? 'text-yellow-600' : 'text-green-600'}`}>
            {project.priority}/10
          </span>
        </p>
        <p>
          <span className="muted">Даты:</span> {formatDate(project.startDate)} - {formatDate(project.endDate)}
        </p>
        <p>
          <span className="muted">Команда:</span> {project.employees?.length || 0} чел.
        </p>
      </div>

      <div className="flex gap-2 mt-6">
        <Link to={`/projects/${project.id}`} className="btn flex-1">
          Подробнее
        </Link>
        <Link to={`/projects/${project.id}/edit`} className="btn btn-secondary">
          Редактировать
        </Link>
        <button 
          className="btn btnDanger"
          onClick={() => onDelete(project.id)}
        >
          Удалить
        </button>
      </div>
    </div>
  );
};

export default ProjectCard;