import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { ProjectsApi } from "../../../api/projects.api";
import { DocumentsApi } from "../../../api/documents.api";
import type { ProjectFormData, DocumentResponseDto } from "../../../types/project.types";
import ProjectEditForm from "./ProjectEditForm";

const ProjectEdit = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [documents, setDocuments] = useState<DocumentResponseDto[]>([]);
  const [formData, setFormData] = useState<ProjectFormData | null>(null);

  useEffect(() => {
    if (id) {
      loadProjectData(id);
    }
  }, [id]);

  const loadProjectData = async (projectId: string) => {
    try {
      const [projectData, documentsData] = await Promise.all([
        ProjectsApi.getById(projectId),
        DocumentsApi.getByProject(projectId)
      ]);
      
      setFormData({
        params: {
          name: projectData.name,
          customerCompanyName: projectData.customerCompanyName,
          executorCompanyName: projectData.executorCompanyName,
          startDate: projectData.startDate.split('T')[0],
          endDate: projectData.endDate.split('T')[0],
          priority: projectData.priority
        },
        managerId: projectData.manager.id,
        employeesIds: projectData.employees.map(emp => emp.id)
      });
      
      setDocuments(documentsData);
    } catch (error) {
      alert("Проект не найден");
      navigate("/projects");
    } finally {
      setLoading(false);
    }
  };

  const handleSave = async () => {
    if (!formData || !id) return;

    setSaving(true);
    try {
      await ProjectsApi.update(id, {
        params: {
          ...formData.params,
          startDate: `${formData.params.startDate}T00:00:00`,
          endDate: `${formData.params.endDate}T00:00:00`
        },
        managerId: formData.managerId,
        employeesIds: formData.employeesIds
      });

      alert("Проект успешно обновлен!");
      navigate(`/projects/${id}`);
    } catch (error: any) {
      const errorMessage = error.response?.data?.message || error.message || "Ошибка при обновлении проекта";
      alert(errorMessage);
    } finally {
      setSaving(false);
    }
  };

  const handleDeleteProject = async () => {
    if (!id) return;
    
    if (!confirm("Вы уверены, что хотите удалить этот проект? Это действие нельзя отменить.")) {
      return;
    }

    setDeleting(true);
    try {
      await ProjectsApi.delete(id);
      alert("Проект успешно удален!");
      navigate("/projects");
    } catch (error: any) {
      const errorMessage = error.response?.data?.message || error.message || "Ошибка при удалении проекта";
      alert(errorMessage);
    } finally {
      setDeleting(false);
    }
  };

  const handleDeleteDocument = async (docId: string) => {
    if (!confirm("Удалить документ?")) return;

    try {
      await DocumentsApi.delete(docId);
      if (id) {
        const updatedDocuments = await DocumentsApi.getByProject(id);
        setDocuments(updatedDocuments);
      }
      alert("Документ удален");
    } catch (error) {
      alert("Ошибка при удалении документа");
    }
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
      <div className="container project-edit-container">
        <div className="panel">
          <div className="muted text-center py-8">Загрузка проекта...</div>
        </div>
      </div>
    );
  }

  if (!formData) {
    return (
      <div className="container project-edit-container">
        <div className="panel">
          <div className="muted text-center py-8">Проект не найден</div>
        </div>
      </div>
    );
  }

  return (
    <div className="container project-edit-container">
      <div className="project-edit-header">
        <div className="header-title">
          <h1 className="h1">Редактирование проекта</h1>
          <div className="sub text-gray-600">ID: {id}</div>
        </div>
        
        <div className="project-edit-actions">
          <button 
            className="btn btn-danger"
            onClick={handleDeleteProject}
            disabled={deleting}
          >
            {deleting ? "Удаление..." : "Удалить проект"}
          </button>
          
          <button 
            className="btn btn-secondary"
            onClick={() => navigate(`/projects/${id}`)}
          >
            Отмена
          </button>
          
          <button 
            className="btn btn-primary"
            onClick={handleSave}
            disabled={saving}
          >
            {saving ? "Сохранение..." : "Сохранить изменения"}
          </button>
        </div>
      </div>

      <div className="project-edit-grid">
        <div className="edit-main-content">
          <ProjectEditForm 
            formData={formData}
            onChange={setFormData}
          />
        </div>

        <div className="edit-sidebar">
          <div className="sidebar-panel">
            <div className="sidebar-header">
              <h2 className="h2">Документы проекта</h2>
              <span className="badge badge-primary">{documents.length}</span>
            </div>
            
            {documents.length === 0 ? (
              <div className="empty-state">
                <div className="text-gray-500 mb-2">Документы отсутствуют</div>
                <div className="text-sm text-gray-400">
                  Загрузите документы на странице проекта
                </div>
              </div>
            ) : (
              <div className="documents-sidebar-list">
                {documents.map(doc => (
                  <div key={doc.id} className="document-sidebar-item">
                    <div className="document-sidebar-info">
                      <div className="document-sidebar-name">
                        {doc.fileName}
                      </div>
                      <div className="document-sidebar-meta">
                        <span>{formatFileSize(doc.fileSize)}</span>
                        <span>•</span>
                        <span>{new Date(doc.uploadDate).toLocaleDateString("ru-RU")}</span>
                      </div>
                    </div>
                    
                    <div className="document-sidebar-actions">
                      <a
                        href={doc.downloadUrl || `/api/documents/${doc.id}/download`}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="btn btn-icon btn-sm"
                        title="Скачать"
                      >
                        📥
                      </a>
                      <button
                        className="btn btn-icon btn-sm btn-danger"
                        onClick={() => handleDeleteDocument(doc.id)}
                        title="Удалить"
                      >
                        🗑️
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            )}
            
            <div className="mt-6 pt-4 border-t border-gray-200">
              <button
                className="btn btn-secondary w-full"
                onClick={() => navigate(`/projects/${id}#documents`)}
              >
                Управление документами
              </button>
            </div>
          </div>

          <div className="sidebar-panel">
            <h3 className="h3 mb-4">Быстрые действия</h3>
            <div className="quick-actions">
              <button
                className="btn btn-secondary quick-action-btn"
                onClick={() => navigate(`/projects/${id}`)}
              >
                ← Вернуться к просмотру
              </button>
              <button
                className="btn btn-secondary quick-action-btn"
                onClick={() => {
                  if (confirm("Сбросить все изменения?")) {
                    loadProjectData(id!);
                  }
                }}
              >
                ↺ Сбросить изменения
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProjectEdit;