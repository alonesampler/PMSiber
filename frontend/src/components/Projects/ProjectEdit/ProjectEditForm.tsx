import { useEffect, useState } from "react";
import { EmployeesApi } from "../../../api/employees.api";
import type { ProjectDraftDto } from "../../../types/project.types";
import type { EmployeeResponseDto } from "../../../types/employee.types";

type Props = {
  formData: ProjectDraftDto;
  onChange: (data: ProjectDraftDto) => void;
};

const ProjectEditForm = ({ formData, onChange }: Props) => {
  const [employees, setEmployees] = useState<EmployeeResponseDto[]>([]);
  const [query, setQuery] = useState("");

  useEffect(() => {
    EmployeesApi.getAll().then(setEmployees);
  }, []);

  const search = async (q: string) => {
    setQuery(q);
    if (!q.trim()) {
      setEmployees(await EmployeesApi.getAll());
    } else {
      setEmployees(await EmployeesApi.search(q));
    }
  };

  return (
    <div className="panel">
      <h3 className="h3">Проект</h3>

      <input
        className="input"
        placeholder="Название"
        value={formData.params.name}
        onChange={e =>
          onChange({
            ...formData,
            params: { ...formData.params, name: e.target.value },
          })
        }
      />

      <div className="row">
        <input
          type="date"
          className="input"
          value={formData.params.startDate}
          onChange={e =>
            onChange({
              ...formData,
              params: { ...formData.params, startDate: e.target.value },
            })
          }
        />

        <input
          type="date"
          className="input"
          value={formData.params.endDate}
          onChange={e =>
            onChange({
              ...formData,
              params: { ...formData.params, endDate: e.target.value },
            })
          }
        />
      </div>

      <input
        className="input"
        placeholder="Компания-заказчик"
        value={formData.params.customerCompanyName}
        onChange={e =>
          onChange({
            ...formData,
            params: { ...formData.params, customerCompanyName: e.target.value },
          })
        }
      />

      <input
        className="input"
        placeholder="Компания-исполнитель"
        value={formData.params.executorCompanyName}
        onChange={e =>
          onChange({
            ...formData,
            params: { ...formData.params, executorCompanyName: e.target.value },
          })
        }
      />

      <div className="spacer" />

      <h3 className="h3">Менеджер</h3>

      <input
        className="input"
        placeholder="Поиск сотрудника"
        value={query}
        onChange={e => search(e.target.value)}
      />

      {employees.slice(0, 5).map(e => (
        <button
          key={e.id}
          className={`btn ${formData.managerId === e.id ? "btnPrimary" : ""}`}
          onClick={() => onChange({ ...formData, managerId: e.id })}
        >
          {e.lastName} {e.firstName}
        </button>
      ))}

      <div className="spacer" />

      <h3 className="h3">Исполнители</h3>

      {employees.slice(0, 8).map(e => {
        const selected = formData.employeesIds.includes(e.id);
        const isManager = e.id === formData.managerId;

        return (
          <div key={e.id} className="card">
            <div className="row" style={{ justifyContent: "space-between" }}>
              <div>
                {e.lastName} {e.firstName}
              </div>

              {!isManager && (
                <button
                  className={`btn ${selected ? "danger" : "btnPrimary"}`}
                  onClick={() =>
                    onChange({
                      ...formData,
                      employeesIds: selected
                        ? formData.employeesIds.filter(x => x !== e.id)
                        : [...formData.employeesIds, e.id],
                    })
                  }
                >
                  {selected ? "Убрать" : "Добавить"}
                </button>
              )}
            </div>
          </div>
        );
      })}
    </div>
  );
};

export default ProjectEditForm;
