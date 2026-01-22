import { useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { ProjectsApi } from "../../../api/projects.api";
import { DocumentsApi } from "../../../api/documents.api";
import type { ProjectUpsertDto } from "../../../types/project.types";
import Step1ProjectMain from "./Step1ProjectMain";
import Step2Companies from "./Step2Companies";
import Step3Manager from "./Step3Manager";
import Step4Employees from "./Step4Employees";
import Step5Documents from "./Step5Documents";

export type ProjectDraft = {
  name: string;
  startDate: string;
  endDate: string;
  priority: number;
  customerCompanyName: string;
  executorCompanyName: string;
  managerId: string;
  employeesIds: string[];
  files: File[];
};

const getInitialDates = () => {
  const today = new Date();
  const thirtyDaysLater = new Date();
  thirtyDaysLater.setDate(today.getDate() + 30);
  
  return {
    startDate: today.toISOString().split('T')[0],
    endDate: thirtyDaysLater.toISOString().split('T')[0]
  };
};

const initialDraft: ProjectDraft = {
  name: "",
  startDate: getInitialDates().startDate,
  endDate: getInitialDates().endDate,
  priority: 5,
  customerCompanyName: "",
  executorCompanyName: "",
  managerId: "",
  employeesIds: [],
  files: [],
};

const Wizard = () => {
  const navigate = useNavigate();
  const [step, setStep] = useState(1);
  const [draft, setDraft] = useState<ProjectDraft>(initialDraft);
  const [saving, setSaving] = useState(false);

  const canNext = useMemo(() => {
    const name = draft.name.trim();
    const customer = draft.customerCompanyName.trim();
    const executor = draft.executorCompanyName.trim();

    if (step === 1) return name.length > 0 && !!draft.startDate && !!draft.endDate;
    if (step === 2) return customer.length > 0 && executor.length > 0;
    if (step === 3) return !!draft.managerId;
    return true;
  }, [step, draft]);

  const finish = async () => {
    setSaving(true);
    
    try {
      const projectData: ProjectUpsertDto = {
        params: {
          name: draft.name.trim(),
          customerCompanyName: draft.customerCompanyName.trim(),
          executorCompanyName: draft.executorCompanyName.trim(),
          startDate: draft.startDate,
          endDate: draft.endDate,
          priority: draft.priority,
        },
        managerId: draft.managerId,
        employeesIds: draft.employeesIds,
      };
      
      const created = await ProjectsApi.create(projectData);

      if (draft.files.length > 0) {
        for (const file of draft.files) {
          await DocumentsApi.upload(created.id, file);
        }
      }

      alert(`Проект "${created.name}" успешно создан!`);
      navigate(`/projects/${created.id}`);
      
    } catch (error: any) {
      const errorData = error.response?.data;
      let errorMessage = "Ошибка при создании проекта";
      
      if (errorData?.errors) {
        const errors: string[] = [];
        Object.entries(errorData.errors).forEach(([messages]) => {
          if (Array.isArray(messages)) {
            errors.push(...messages);
          }
        });
        errorMessage = errors.join('\n');
      } else if (errorData?.message) {
        errorMessage = errorData.message;
      }
      
      alert(errorMessage);
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="wizard-container">
      <div className="wizard-header">
        <div>
          <h1 className="h1">Создание проекта</h1>
          <div className="sub">Шаг {step} из 5</div>
        </div>
        <button className="btn" onClick={() => navigate("/projects")}>
          Назад к списку
        </button>
      </div>

      <div className="wizard-steps">
        {[1, 2, 3, 4, 5].map((num) => (
          <div key={num} style={{ textAlign: "center", flex: 1 }}>
            <div className={`step-indicator ${step === num ? 'active' : step > num ? 'completed' : ''}`}>
              {num}
            </div>
            <div className="muted step-label">
              {num === 1 && "Основное"}
              {num === 2 && "Компании"}
              {num === 3 && "Менеджер"}
              {num === 4 && "Команда"}
              {num === 5 && "Документы"}
            </div>
          </div>
        ))}
      </div>

      <div className="wizard-step-content">
        {step === 1 && <Step1ProjectMain draft={draft} setDraft={setDraft} />}
        {step === 2 && <Step2Companies draft={draft} setDraft={setDraft} />}
        {step === 3 && <Step3Manager draft={draft} setDraft={setDraft} />}
        {step === 4 && <Step4Employees draft={draft} setDraft={setDraft} />}
        {step === 5 && <Step5Documents draft={draft} setDraft={setDraft} />}
      </div>

      <div className="wizard-navigation">
        <button 
          className="btn" 
          disabled={step === 1 || saving} 
          onClick={() => setStep(s => s - 1)}
        >
          Назад
        </button>

        <div className="wizard-navigation-buttons">
          <button 
            className="btn" 
            disabled={saving} 
            onClick={() => {
              setDraft(initialDraft);
              setStep(1);
            }}
          >
            Сбросить
          </button>

          {step < 5 ? (
            <button 
              className="btn btnPrimary" 
              disabled={!canNext || saving} 
              onClick={() => setStep(s => s + 1)}
            >
              Далее
            </button>
          ) : (
            <button 
              className="btn btnSuccess" 
              disabled={saving} 
              onClick={finish}
            >
              {saving ? "Создание..." : "Создать проект"}
            </button>
          )}
        </div>
      </div>
    </div>
  );
};

export default Wizard;