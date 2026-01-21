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
          startDate: projectData.startDate.split('T')[0], // Для input[type="date"]
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
      alert(error.message || "Ошибка при обновлении проекта");
    } finally {
      setSaving(false);
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
    } catch (error) {
      alert("Ошибка при удалении документа");
    }
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

  if (!formData) {
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
      <div className="header">
        <div>
          <h1 className="h1">Редактирование проекта</h1>
          <div className="sub">ID: {id}</div>
        </div>
        <div className="flex gap-2">
          <button 
            className="btn"
            onClick={() => navigate(`/projects/${id}`)}
          >
            Отмена
          </button>
          <button 
            className="btn btnPrimary"
            onClick={handleSave}
            disabled={saving}
          >
            {saving ? "Сохранение..." : "Сохранить изменения"}
          </button>
        </div>
      </div>

      <div className="grid" style={{ gridTemplateColumns: "2fr 1fr", gap: "24px" }}>
        <div>
          <ProjectEditForm 
            formData={formData}
            onChange={setFormData}
          />
        </div>

        <div>
          <div className="panel">
            <h2 className="h2">Документы проекта</h2>
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
                        <div className="muted" style={{ fontSize: "12px" }}>
                          {new Date(doc.uploadDate).toLocaleDateString("ru-RU")}
                        </div>
                      </div>
                      
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

export default ProjectEdit;