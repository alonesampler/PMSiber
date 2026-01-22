import { useMemo } from "react";
import type { ProjectDraft } from "../../../hooks/useWizard";
import { useEmployeesSearch } from "../../../hooks/useEmployeesSearch";
import type { EmployeeResponseDto } from "../../../types/employee.types";

type Props = { draft: ProjectDraft; setDraft: (v: ProjectDraft) => void };

const Step3Manager = ({ draft, setDraft }: Props) => {
  const { query, setQuery, employees, loading } = useEmployeesSearch();

  const selectedLabel = useMemo(() => {
    if (!draft.managerId) return "";
    const found = employees.find((x) => x.id === draft.managerId);
    return found ? format(found) : "";
  }, [draft.managerId, employees]);

  const pick = (e: EmployeeResponseDto) => {
    setDraft({ ...draft, managerId: e.id });
    setQuery(format(e));
  };

  return (
    <div className="employee-search-container">
      <div className="sub" style={{ marginBottom: 8 }}>
        Вводи фамилию/имя/отчество/почту — дергаем /employees/search
      </div>

      <input
        className="input"
        placeholder="Поиск менеджера..."
        value={query}
        onChange={(e) => setQuery(e.target.value)}
      />

      <div className="muted" style={{ marginTop: 8 }}>
        {loading
          ? "Поиск..."
          : query.trim()
          ? `Найдено: ${employees.length}`
          : "Введите хотя бы 1 символ"}
      </div>

      <div className="employee-search-results">
        <div className="employee-grid">
          {employees.slice(0, 8).map((e) => (
            <div 
              key={e.id} 
              className={`employee-card ${draft.managerId === e.id ? 'selected' : ''}`}
              onClick={() => pick(e)}
            >
              <div className="employee-info">
                <div className="employee-name">{formatName(e)}</div>
                <div className="employee-email">{e.email}</div>
              </div>
              <div className="employee-status">
                {draft.managerId === e.id ? "Выбран менеджером" : "Нажмите для выбора"}
              </div>
            </div>
          ))}
        </div>
      </div>

      <div className="spacer" />

      <div className="muted">
        Текущий менеджер:{" "}
        <b>{selectedLabel || (draft.managerId ? shortId(draft.managerId) : "не выбран")}</b>
      </div>
    </div>
  );
};

function formatName(e: EmployeeResponseDto) {
  return `${e.lastName} ${e.firstName}${e.middleName ? " " + e.middleName : ""}`;
}

function format(e: EmployeeResponseDto) {
  return `${formatName(e)} — ${e.email}`;
}

function shortId(id: string) {
  return id.length > 10 ? `${id.slice(0, 8)}...` : id;
}

export default Step3Manager;