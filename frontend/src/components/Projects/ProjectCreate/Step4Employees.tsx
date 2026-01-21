import { useMemo } from "react";
import type { ProjectDraft } from "../../../hooks/useWizard";
import { useEmployeesSearch } from "../../../hooks/useEmployeesSearch";
import type { EmployeeResponseDto } from "../../../types/employee.types";

type Props = { draft: ProjectDraft; setDraft: (v: ProjectDraft) => void };

const Step4Employees = ({ draft, setDraft }: Props) => {
  const { query, setQuery, employees, loading } = useEmployeesSearch();

  const selectedSet = useMemo(() => new Set(draft.employeesIds), [draft.employeesIds]);

  const toggle = (id: string) => {
    if (id === draft.managerId) return;

    const next = selectedSet.has(id)
      ? draft.employeesIds.filter((x) => x !== id)
      : [...draft.employeesIds, id];

    setDraft({ ...draft, employeesIds: next });
  };

  const clearAll = () => setDraft({ ...draft, employeesIds: [] });

  return (
    <div className="panel">
      <div className="row" style={{ justifyContent: "space-between" }}>
        <div>
          <div className="sub">Выбор исполнителей (можно пусто)</div>
          <div className="muted" style={{ marginTop: 6 }}>
            Выбрано: <b>{draft.employeesIds.length}</b>
          </div>
        </div>

        <button className="btn" onClick={clearAll} disabled={!draft.employeesIds.length}>
          Очистить
        </button>
      </div>

      <div className="spacer" />

      <input
        className="input"
        placeholder="Поиск исполнителей..."
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
        {employees.slice(0, 10).map((e) => {
          const disabled = e.id === draft.managerId;
          const active = selectedSet.has(e.id);

          return (
            <div key={e.id} className="card" style={{ padding: 12, opacity: disabled ? 0.6 : 1 }}>
              <div style={{ fontWeight: 600 }}>{format(e)}</div>
              <div className="muted" style={{ marginTop: 6 }}>
                {disabled ? "Это менеджер проекта" : active ? "Выбран" : "Не выбран"}
              </div>

              <div className="spacer" />

              <button className="btn btnPrimary" disabled={disabled} onClick={() => toggle(e.id)}>
                {active ? "Убрать" : "Добавить"}
              </button>
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default Step4Employees;

function format(e: EmployeeResponseDto) {
  return `${e.lastName} ${e.firstName}${e.middleName ? " " + e.middleName : ""} — ${e.email}`;
}
