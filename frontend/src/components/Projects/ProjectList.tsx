import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { ProjectsApi } from "../../api/projects.api";
import type { ProjectResponseDto } from "../../types/project.types";
import ProjectCard from "./ProjectCard";
import ProjectSearch from "./ProjectSearch";

const ProjectList = () => {
  const navigate = useNavigate();
  const [projects, setProjects] = useState<ProjectResponseDto[]>([]);
  const [filteredProjects, setFilteredProjects] = useState<ProjectResponseDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchQuery, setSearchQuery] = useState("");

  const loadProjects = async () => {
    try {
      const projectsData = await ProjectsApi.getAll();
      setProjects(projectsData);
      setFilteredProjects(projectsData);
    } catch (error) {
      console.error("Failed to load projects:", error);
      alert("Ошибка при загрузке проектов");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadProjects();
  }, []);

  useEffect(() => {
    if (searchQuery.trim() === "") {
      setFilteredProjects(projects);
    } else {
      const filtered = projects.filter(project =>
        project.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
        project.customerCompanyName.toLowerCase().includes(searchQuery.toLowerCase()) ||
        project.executorCompanyName.toLowerCase().includes(searchQuery.toLowerCase())
      );
      setFilteredProjects(filtered);
    }
  }, [searchQuery, projects]);

  const handleDelete = async (id: string) => {
    if (!confirm("Вы уверены, что хотите удалить этот проект?")) return;

    try {
      await ProjectsApi.delete(id);
      await loadProjects(); // Перезагружаем список
    } catch (error: any) {
      console.error("Delete failed:", error);
      alert(error?.response?.data?.message || "Ошибка при удалении проекта");
    }
  };

  if (loading) {
    return (
      <div className="container">
        <div className="page">
          <div className="panel">
            <div className="muted">Загрузка проектов...</div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="container">
      <div className="page">
        <div className="header">
          <div>
            <h1 className="h1">Проекты</h1>
            <div className="sub">
              Всего проектов: <strong>{projects.length}</strong>
            </div>
          </div>

          <button 
            className="btn btnPrimary"
            onClick={() => navigate("/projects/create")}
          >
            + Создать проект
          </button>
        </div>

        <div className="panel">
          <ProjectSearch 
            value={searchQuery}
            onChange={setSearchQuery}
          />
          {searchQuery && (
            <div className="muted" style={{ marginTop: 8 }}>
              Найдено проектов: {filteredProjects.length}
            </div>
          )}
        </div>

        <div className="spacer" />

        {filteredProjects.length === 0 ? (
          <div className="panel">
            <div style={{ textAlign: "center", padding: "40px 20px" }}>
              <h3 className="h3" style={{ marginBottom: 12 }}>Проекты не найдены</h3>
              <div className="muted" style={{ marginBottom: 24 }}>
                {searchQuery ? "Попробуйте изменить поисковый запрос" : "Создайте первый проект"}
              </div>
              <button 
                className="btn btnPrimary"
                onClick={() => navigate("/projects/create")}
              >
                + Создать проект
              </button>
            </div>
          </div>
        ) : (
          <div className="grid">
            {filteredProjects.map(project => (
              <ProjectCard
                key={project.id}
                project={project}
                onDelete={handleDelete}
              />
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default ProjectList;