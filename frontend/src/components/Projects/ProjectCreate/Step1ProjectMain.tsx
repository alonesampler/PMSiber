import type { ProjectDraft } from "./Wizard";

type Props = { 
  draft: ProjectDraft; 
  setDraft: (v: ProjectDraft) => void 
};

const Step1ProjectMain = ({ draft, setDraft }: Props) => {
  return (
    <div>
      <h2 className="h2">Основная информация</h2>
      <div className="spacer" />
      
      <div style={{ marginBottom: "16px" }}>
        <input
          className="input"
          placeholder="Название проекта *"
          value={draft.name}
          onChange={(e) => setDraft({ ...draft, name: e.target.value })}
          required
        />
      </div>

      <div className="row">
        <input
          type="date"
          className="input"
          value={draft.startDate}
          min={new Date().toISOString().split('T')[0]}
          onChange={(e) => setDraft({ ...draft, startDate: e.target.value })}
          required
        />
        <input
          type="date"
          className="input"
          value={draft.endDate}
          min={draft.startDate}
          onChange={(e) => setDraft({ ...draft, endDate: e.target.value })}
          required
        />
      </div>

      <div style={{ marginTop: "16px" }}>
        <label className="muted">Приоритет: {draft.priority}/10</label>
        <input
          type="range"
          min="1"
          max="10"
          value={draft.priority}
          onChange={(e) => setDraft({ ...draft, priority: Number(e.target.value) })}
          style={{ width: "100%" }}
        />
      </div>
    </div>
  );
};

export default Step1ProjectMain;