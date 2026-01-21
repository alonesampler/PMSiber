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
    <div className="panel">
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

      <div className="spacer" />

      <div className="grid" style={{ gridTemplateColumns: "repeat(2, minmax(0, 1fr))" }}>
        {employees.slice(0, 8).map((e) => (
          <button key={e.id} className="btn" onClick={() => pick(e)}>
            {format(e)}
          </button>
        ))}
      </div>

      <div className="spacer" />

      <div className="muted">
        Текущий менеджер:{" "}
        <b>{selectedLabel || (draft.managerId ? shortId(draft.managerId) : "не выбран")}</b>
      </div>
    </div>
  );
};

export default Step3Manager;

function format(e: EmployeeResponseDto) {
  return `${e.lastName} ${e.firstName}${e.middleName ? " " + e.middleName : ""} — ${e.email}`;
}

function shortId(id: string) {
  return id.length > 10 ? `${id.slice(0, 8)}...` : id;
}
