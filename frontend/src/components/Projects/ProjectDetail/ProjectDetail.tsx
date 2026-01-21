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
      <div className="container">
        <div className="panel">
          <div className="muted">Загрузка...</div>
        </div>
      </div>
    );
  }

  if (!project) {
    return (
      <div className="container">
        <div className="panel">
          <div className="muted">Проект не найден</div>
        </div>
      </div>
    );
  }

  return (
    <div className="container">
      <div style={{ marginBottom: "24px" }}>
        <div className="header">
          <div>
            <h1 className="h1">{project.name}</h1>
            <div className="sub">ID: {project.id}</div>
          </div>
          <button 
            className="btn"
            onClick={() => navigate("/projects")}
          >
            Назад к списку
          </button>
        </div>
      </div>

      <div className="grid" style={{ gridTemplateColumns: "2fr 1fr", gap: "24px" }}>
        <div>
          <div className="panel">
            <h2 className="h2">Основная информация</h2>
            
            <div style={{ marginTop: "16px" }}>
              <p><strong>Заказчик:</strong> {project.customerCompanyName}</p>
              <p><strong>Исполнитель:</strong> {project.executorCompanyName}</p>
              <p><strong>Дата начала:</strong> {formatDate(project.startDate)}</p>
              <p><strong>Дата окончания:</strong> {formatDate(project.endDate)}</p>
              <p><strong>Приоритет:</strong> {project.priority}/10</p>
            </div>
          </div>

          <div style={{ height: "16px" }}></div>

          <div className="panel">
            <h2 className="h2">Команда проекта</h2>
            
            <div style={{ marginTop: "16px" }}>
              <h3 className="h3">Менеджер проекта</h3>
              <div className="card" style={{ marginTop: "8px" }}>
                <p>
                  {project.manager?.lastName} {project.manager?.firstName}
                  {project.manager?.middleName && ` ${project.manager.middleName}`}
                </p>
                <p className="muted">{project.manager?.email}</p>
              </div>
            </div>

            {project.employees && project.employees.length > 0 && (
              <div style={{ marginTop: "24px" }}>
                <h3 className="h3">Исполнители ({project.employees.length})</h3>
                <div className="grid" style={{ marginTop: "12px", gridTemplateColumns: "repeat(2, 1fr)" }}>
                  {project.employees.map(emp => (
                    <div key={emp.id} className="card">
                      <p>
                        {emp.lastName} {emp.firstName}
                        {emp.middleName && ` ${emp.middleName}`}
                      </p>
                      <p className="muted">{emp.email}</p>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </div>
        </div>

        <div>
          <div className="panel">
            <h2 className="h2">Документы</h2>
            <div className="spacer" />
            
            <div className="card" style={{ marginBottom: "16px" }}>
              <div style={{ marginBottom: "12px" }}>
                <div className="muted">Загрузка новых документов</div>
                <input
                  type="file"
                  multiple
                  onChange={(e) => setSelectedFiles(e.target.files)}
                  className="input"
                  style={{ marginTop: "8px" }}
                />
              </div>
              
              {selectedFiles && selectedFiles.length > 0 && (
                <div style={{ marginBottom: "12px" }}>
                  <div className="muted">Выбрано файлов: {selectedFiles.length}</div>
                </div>
              )}
              
              <button
                className="btn btnPrimary"
                onClick={handleFileUpload}
                disabled={!selectedFiles || selectedFiles.length === 0 || uploading}
              >
                {uploading ? "Загрузка..." : "Загрузить документы"}
              </button>
            </div>

            <div className="spacer" />
            
            {documents.length === 0 ? (
              <div className="muted" style={{ textAlign: "center", padding: "20px" }}>
                Документы отсутствуют
              </div>
            ) : (
              <div className="grid" style={{ gridTemplateColumns: "1fr" }}>
                {documents.map(doc => (
                  <div key={doc.id} className="card">
                    <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start" }}>
                      <div style={{ flex: 1 }}>
                        <div style={{ fontWeight: "500", marginBottom: "4px" }}>
                          {doc.fileName}
                        </div>
                        <div className="muted" style={{ fontSize: "12px", marginBottom: "4px" }}>
                          {formatFileSize(doc.fileSize)} • {doc.contentType}
                        </div>
                      </div>
                      
                      <div style={{ display: "flex", gap: "8px" }}>
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