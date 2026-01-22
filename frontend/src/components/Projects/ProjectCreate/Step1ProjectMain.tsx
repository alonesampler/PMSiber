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
      
      <div className="wizard-form-group">
        <input
          className="input"
          placeholder="Название проекта *"
          value={draft.name}
          onChange={(e) => setDraft({ ...draft, name: e.target.value })}
          required
        />
      </div>

      <div className="wizard-row">
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

      <div className="wizard-form-group">
        <label className="muted">Приоритет: {draft.priority}/10</label>
        <div className="range-container">
          <input
            type="range"
            min="1"
            max="10"
            value={draft.priority}
            onChange={(e) => setDraft({ ...draft, priority: Number(e.target.value) })}
          />
        </div>
      </div>
    </div>
  );
};

export default Step1ProjectMain;