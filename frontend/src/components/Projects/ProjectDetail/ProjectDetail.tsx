import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { ProjectsApi } from "../../../api/projects.api";
import { DocumentsApi } from "../../../api/documents.api";
import type { ProjectResponseDto, DocumentResponseDto } from "../../../types/project.types";

const ProjectDetail = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [project, setProject] = useState<ProjectResponseDto | null>(null);
  const [documents, setDocuments] = useState<DocumentResponseDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [uploading, setUploading] = useState(false);
  const [selectedFiles, setSelectedFiles] = useState<FileList | null>(null);

  useEffect(() => {
    if (id) {
      loadProjectAndDocuments(id);
    }
  }, [id]);

  const loadProjectAndDocuments = async (projectId: string) => {
    try {
      const [projectData, documentsData] = await Promise.all([
        ProjectsApi.getById(projectId),
        DocumentsApi.getByProject(projectId)
      ]);
      setProject(projectData);
      setDocuments(documentsData);
    } catch (error) {
      alert("Проект не найден");
      navigate("/projects");
    } finally {
      setLoading(false);
    }
  };

  const handleFileUpload = async () => {
    if (!project || !selectedFiles || selectedFiles.length === 0) {
      alert("Выберите файлы для загрузки");
      return;
    }

    setUploading(true);
    try {
      for (let i = 0; i < selectedFiles.length; i++) {
        const file = selectedFiles[i];
        await DocumentsApi.upload(project.id, file);
      }
      
      const updatedDocuments = await DocumentsApi.getByProject(project.id);
      setDocuments(updatedDocuments);
      setSelectedFiles(null);
      
      alert("Файлы успешно загружены!");
    } catch (error) {
      alert("Ошибка при загрузке файлов");
    } finally {
      setUploading(false);
    }
  };

  const handleDeleteDocument = async (docId: string) => {
    if (!confirm("Удалить документ?")) return;

    try {
      await DocumentsApi.delete(docId);
      if (project) {
        const updatedDocuments = await DocumentsApi.getByProject(project.id);
        setDocuments(updatedDocuments);
      }
    } catch (error) {
      alert("Ошибка при удалении документа");
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString("ru-RU", {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  };

  const formatFileSize = (bytes: number) => {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  };

  if (loading) {
    return (
      <div className="project-detail-container">
        <div className="panel">
          <div className="muted">Загрузка...</div>
        </div>
      </div>
    );
  }

  if (!project) {
    return (
      <div className="project-detail-container">
        <div className="panel">
          <div className="muted">Проект не найден</div>
        </div>
      </div>
    );
  }

  return (
    <div className="project-detail-container">
      <div className="project-detail-header">
        <div>
          <h1 className="project-title">{project.name}</h1>
          <div className="project-subtitle">ID: {project.id}</div>
        </div>
        <div style={{ display: "flex", gap: "8px" }}>
          <button 
            className="btn btnSecondary"
            onClick={() => navigate(`/projects/${id}/edit`)}
          >
            Редактировать
          </button>
          <button 
            className="btn"
            onClick={() => navigate("/projects")}
          >
            Назад к списку
          </button>
        </div>
      </div>

      <div className="project-detail-grid">
        <div>
          <div className="project-info-card">
            <h2 className="h2">Основная информация</h2>
            
            <div className="project-info-list">
              <div className="project-info-item">
                <span className="project-info-label">Заказчик:</span>
                <span className="project-info-value">{project.customerCompanyName}</span>
              </div>
              <div className="project-info-item">
                <span className="project-info-label">Исполнитель:</span>
                <span className="project-info-value">{project.executorCompanyName}</span>
              </div>
              <div className="project-info-item">
                <span className="project-info-label">Дата начала:</span>
                <span className="project-info-value">{formatDate(project.startDate)}</span>
              </div>
              <div className="project-info-item">
                <span className="project-info-label">Дата окончания:</span>
                <span className="project-info-value">{formatDate(project.endDate)}</span>
              </div>
              <div className="project-info-item">
                <span className="project-info-label">Приоритет:</span>
                <span className="project-info-value">{project.priority}/10</span>
              </div>
            </div>
          </div>

          <div style={{ height: "16px" }}></div>

          <div className="project-info-card">
            <h2 className="h2">Команда проекта</h2>
            
            <div className="project-team-section">
              <h3 className="h3">Менеджер проекта</h3>
              <div className="manager-card">
                <div className="employee-name">
                  {project.manager?.lastName} {project.manager?.firstName}
                  {project.manager?.middleName && ` ${project.manager.middleName}`}
                </div>
                <div className="employee-email">{project.manager?.email}</div>
              </div>
            </div>

            {project.employees && project.employees.length > 0 && (
              <div className="project-team-section">
                <h3 className="h3">Исполнители ({project.employees.length})</h3>
                <div className="employees-grid">
                  {project.employees.map(emp => (
                    <div key={emp.id} className="employee-card">
                      <div className="employee-name">
                        {emp.lastName} {emp.firstName}
                        {emp.middleName && ` ${emp.middleName}`}
                      </div>
                      <div className="employee-email">{emp.email}</div>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </div>
        </div>

        <div className="documents-panel">
          <div className="project-info-card">
            <h2 className="h2">Документы</h2>
            
            <div className={`upload-area ${selectedFiles ? 'drag-over' : ''}`}>
              <div className="upload-title">
                {selectedFiles ? "Отпустите для загрузки" : "Загрузите документы"}
              </div>
              <div className="upload-hint">
                Перетащите файлы или кликните для выбора
              </div>
              <input
                type="file"
                multiple
                onChange={(e) => setSelectedFiles(e.target.files)}
                className="input"
                style={{ marginTop: "8px", width: "100%" }}
              />
              
              {selectedFiles && selectedFiles.length > 0 && (
                <div className="upload-count">
                  Выбрано файлов: {selectedFiles.length}
                </div>
              )}
            </div>

            <button
              className="btn btnPrimary"
              onClick={handleFileUpload}
              disabled={!selectedFiles || selectedFiles.length === 0 || uploading}
              style={{ width: "100%", marginTop: "16px" }}
            >
              {uploading ? "Загрузка..." : "Загрузить документы"}
            </button>

            <div className="spacer"></div>
            
            {documents.length === 0 ? (
              <div className="muted" style={{ textAlign: "center", padding: "20px" }}>
                Документы отсутствуют
              </div>
            ) : (
              <div className="documents-list">
                {documents.map(doc => (
                  <div key={doc.id} className="document-item">
                    <div className="document-info">
                      <div className="document-name">{doc.fileName}</div>
                      <div className="document-meta">
                        <span>{formatFileSize(doc.fileSize)}</span>
                        <span>•</span>
                        <span>{doc.contentType}</span>
                      </div>
                    </div>
                    
                    <div className="document-actions">
                      <a
                        href={doc.downloadUrl || `/api/documents/${doc.id}/download`}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="btn"
                      >
                        Скачать
                      </a>
                      <button
                        className="btn btnDanger"
                        onClick={() => handleDeleteDocument(doc.id)}
                      >
                        Удалить
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProjectDetail;